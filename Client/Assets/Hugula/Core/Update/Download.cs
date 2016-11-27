// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Hugula.Utils;
using System.IO;
using Hugula.Collections;

namespace Hugula.Update
{

    /// <summary>
    /// Download.
    /// </summary>
    [SLua.CustomLuaClass]
    public class Download : MonoBehaviour
    {
        #region private class
        private class ReqInfo
        {
            public string url;
            public string saveName;
            public int index;
            public object args;
            public bool isError;
        }

        //        private class LoadedInfo
        //        {
        //            public string url;
        //            public bool isError;
        //        }

        #endregion

        #region member

        public int progressPercentage { get; private set; }

        /// <summary>
        /// The max loading.
        /// </summary>
        public int maxLoading { get; private set; }

        private object locked = new object();
        private bool _isAllDone = false;

        protected bool isAllDone
        {
            get
            {
                lock (locked)
                {
                    return _isAllDone;
                }
            }
            set
            {
                lock (locked)
                {
                    _isAllDone = value;
                }
            }
        }

        ArrayList webClients = ArrayList.Synchronized(new ArrayList());
        System.Action<string, bool, object> onOneDone;
        System.Action<bool> allDone;
        string outputPath;
        string[] hosts;
        SafeDictionary<String, ReqInfo> downloadings = new SafeDictionary<string, ReqInfo>();
        ArrayList queue = ArrayList.Synchronized(new ArrayList());
        ArrayList loadQueue = ArrayList.Synchronized(new ArrayList());
        int errorHostIndex = -1;
        #endregion

        #region method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="maxLoading"></param>
        /// <param name="oneDone"></param>
        /// <param name="allDone"></param>
		public void Init(string[] hosts, int maxLoading, System.Action<string, bool, object> oneDone, System.Action<bool> allDone)
        {
            this.hosts = hosts;
            this.maxLoading = maxLoading;
            //webClients = new WebDownload[maxLoading];
            WebDownload item;
            for (int i = 0; i < maxLoading; i++)
            {
                item = new WebDownload();
                //webClients[i] = item;
                webClients.Add(item);
                item.DownloadFileCompleted += OnDownloadFileCompleted;
                item.DownloadProgressChanged += OnDownloadProgressChanged;
                item.isFree = true;
            }
            this.onOneDone = oneDone;
            this.allDone = allDone;
            outputPath = CUtils.GetRealPersistentDataPath() + "/";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
		public void Load(string url, string path, object args)
        {
            string key = CUtils.GetUDKey(url);
            if (downloadings.ContainsKey(key))
                return;

            var req = new ReqInfo();
            req.url = url;
            req.saveName = path;//CUtils.GetFileName (url);
            req.index = 0;
            req.args = args;

            Load(req);
        }

        /// <summary>
        /// 判断加载
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        bool CheckLoad(ReqInfo req)
        {
            string key = CUtils.GetUDKey(req.url);

            if (downloadings.ContainsKey(key)) return true;

            var freedownload = GetFree();
            if (freedownload != null)
            {
                int index = req.index;
                Uri url = new Uri(CUtils.PathCombine(hosts[index], req.url));
                string path = CUtils.PathCombine(outputPath, req.saveName);
#if UNITY_EDITOR
                Debug.LogFormat(" begin load {0} ,save path ={1} ,index ={2}", url.AbsoluteUri, path, index);
#endif
                freedownload.isFree = false;
                FileHelper.CheckCreateFilePathDirectory(path);//检测存储文件夹是否存在
                freedownload.DownloadFileAsync(url, path, req);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 加入加载队列
        /// </summary>
        /// <param name="req"></param>
        void Load(ReqInfo req)
        {
            loadQueue.Add(req);
        }

        /// <summary>
        /// Gets the free.
        /// </summary>
        /// <returns>The free.</returns>
        WebDownload GetFree()
        {
            WebDownload item;
            for (int i = 0; i < webClients.Count; i++)
            {
                item = (WebDownload)webClients[i];
                if (item.isFree)
                    return item;
            }
            return null;
        }

        void OnFileDown(ReqInfo req, bool isError)
        {
            req.isError = isError;
            queue.Add(req);

            if (downloadings.Count == 0 && loadQueue.Count == 0)
            {
                isAllDone = true;
            }

        }

        void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            WebDownload webd = (WebDownload)sender;
            webd.isFree = true;
            ReqInfo req = (ReqInfo)e.UserState;

            string key = CUtils.GetUDKey(req.url);
            downloadings.Remove(key);

            if (e.Error == null)
            { //success
#if UNITY_EDITOR
                // Debug.LogFormat("OnDownloadFileCompleted {0}", req.url);
#endif
                OnFileDown(req, true);
            }
            else
            {

                if (req.index < hosts.Length - 1)
                {
                    req.index++;
#if UNITY_EDITOR
                    Debug.LogFormat("reload {0}  save path = {1} index = {2} ", req.url, req.saveName, req.index);
#endif
                    Load(req);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat("{0} req.index={1}hosts.Length{2} \r\n {3}", req.url, req.index, hosts.Length, e.Error);
#endif
                    string path = CUtils.PathCombine(outputPath, req.saveName);
                    File.Delete(path);
                    OnFileDown(req, false);
                }
            }
        }

        void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressPercentage = e.ProgressPercentage;
        }

        #endregion


        #region mono

        void Update()
        {
            while (loadQueue.Count > 0)
            {
                var reqinfo = (ReqInfo)loadQueue[0];
                if (CheckLoad(reqinfo))
                    loadQueue.RemoveAt(0);
                else
                    break;
            }

            while (queue.Count > 0)
            {
                var info = (ReqInfo)queue[0];
                queue.RemoveAt(0);

                if (onOneDone != null)
                    onOneDone(info.url, info.isError, info.args);
            }

            if (queue.Count == 0 && loadQueue.Count == 0 && isAllDone)
            {

                if (allDone != null)
                    allDone(isAllDone);

                isAllDone = false;
            }
        }


        void OnDestroy()
        {
            WebDownload item;
            for (int i = 0; i < webClients.Count; i++)
            {
                item = (WebDownload)webClients[i];
                item.Dispose();
            }
            webClients.Clear();
            webClients = null;
        }

        #endregion

        #region static
        /// <summary>
        /// Releases all resource used by the <see cref="Hugula.Update.Download"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Hugula.Update.Download"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Hugula.Update.Download"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the <see cref="Hugula.Update.Download"/> so the garbage
        /// collector can reclaim the memory that the <see cref="Hugula.Update.Download"/> was occupying.</remarks>
        public static void Dispose()
        {
            if (_instance != null)
                GameObject.Destroy(_instance.gameObject);
        }

        static Download _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Download instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObj = new GameObject("Download");
                    _instance = gameObj.AddComponent<Download>();
                }
                return _instance;
            }
        }


        #endregion
    }

    /// <summary>
    /// 超时功能的download
    /// </summary>
    public class WebDownload : WebClient
    {
        private object lockObj = new object();
        private bool _isFree = true;
        public bool isFree
        {
            get
            {
                lock (lockObj)
                {
                    return _isFree;
                }
            }

            set
            {
                lock (lockObj)
                {
                    _isFree = value;
                }
            }
        }

        public int Timeout { get; set; }

        public WebDownload() : this(10000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
                request.ReadWriteTimeout = Timeout;
            }
            return request;
        }
    }
}