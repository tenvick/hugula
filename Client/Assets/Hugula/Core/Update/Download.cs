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
        }

        private class LoadedInfo
        {
            public string url;
            public bool isError;
        }

        #endregion

        #region member

        public int progressPercentage { get; private set; }

        /// <summary>
        /// The max loading.
        /// </summary>
        public int maxLoading { get; private set; }

        bool isAllDone;

        WebDownload[] webClients;
        System.Action<string, bool> onOneDone;
        System.Action<bool> allDone;
        string outputPath;
        string[] hosts;
        readonly Dictionary<string, ReqInfo> downloadings = new Dictionary<string, ReqInfo>();
        readonly Dictionary<string, ReqInfo> waiting = new Dictionary<string, ReqInfo>();
        ArrayList queue = ArrayList.Synchronized(new ArrayList());
        readonly Dictionary<int, int> erroHostLog = new Dictionary<int, int>();
        int errorHostIndex = -1;
        #endregion

        #region method
        public void Init(string[] hosts, int maxLoading, System.Action<string, bool> oneDone, System.Action<bool> allDone)
        {
            this.hosts = hosts;
            this.maxLoading = maxLoading;
            webClients = new WebDownload[maxLoading];
            WebDownload item;
            for (int i = 0; i < webClients.Length; i++)
            {
                item = new WebDownload();
                webClients[i] = item;
                item.DownloadFileCompleted += OnDownloadFileCompleted;
                item.DownloadProgressChanged += OnDownloadProgressChanged;
                item.isFree = true;
            }
            this.onOneDone = oneDone;
            this.allDone = allDone;
            outputPath = CUtils.GetRealPersistentDataPath() + "/";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        }

        public void Load(string url, string path)
        {
            string key = CUtils.GetUDKey("", url);
            if (downloadings.ContainsKey(key))
                return;

            var req = new ReqInfo();
            req.url = url;
            req.saveName = path;//CUtils.GetFileName (url);
            req.index = 0;

            if (GetFree() != null)
            {
                downloadings.Add(key, req);
                BeginLoad(req);
            }
            else
            {
                if (waiting.ContainsKey(key))
                    waiting[key] = req;
                else
                    waiting.Add(key, req);
            }
        }

        void BeginLoad(ReqInfo req)
        {
            int index = req.index;
            if (index == errorHostIndex)
            {
                int newIndex = index + 1;
                if (newIndex < hosts.Length)
                {
                    req.index = newIndex;
                    index = newIndex;
                }
                else
                {
                    index = 0;
                    req.index = 0;
                }
            }

            Uri url = new Uri(hosts[index] + req.url);
            string path = outputPath + req.saveName;
            //			Debug.LogFormat (" begin load {0} ,save path ={1}", url.AbsoluteUri, path);
            var freedownload = GetFree();
            freedownload.isFree = false;
            freedownload.DownloadFileAsync(url, path, req);
        }

        /// <summary>
        /// Gets the free.
        /// </summary>
        /// <returns>The free.</returns>
        WebDownload GetFree()
        {
            WebDownload item;
            for (int i = 0; i < webClients.Length; i++)
            {
                item = webClients[i];
                if (item.isFree)
                    return item;
            }
            return null;
        }

        void OnFileDown(string url, bool isComplate)
        {
            string key = CUtils.GetUDKey("", url);
            downloadings.Remove(key);

            LoadedInfo loadedInfo = new LoadedInfo { url = url, isError = isComplate };
            queue.Add(loadedInfo);

            if (waiting.Count > 0)
            {
                var e = waiting.GetEnumerator();
                e.MoveNext();
                var pfn = e.Current.Key;
                var pinfo = e.Current.Value;
                waiting.Remove(pfn);

                if (downloadings.ContainsKey(pfn))
                    downloadings[pfn] = pinfo;
                else
                    downloadings.Add(pfn, pinfo);

                BeginLoad(pinfo);
            }
            else if (downloadings.Count == 0)
            {
                isAllDone = true;
            }

        }

        void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            WebDownload webd = (WebDownload)sender;
            webd.isFree = true;
            ReqInfo req = (ReqInfo)e.UserState;
            if (e.Error == null)
            { //success
#if UNITY_EDITOR
                Debug.LogFormat("OnDownloadFileCompleted {0}", req.url);
#endif
                OnFileDown(req.url, true);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning(e.Error);
#endif
                //纪录错误
                if (erroHostLog.ContainsKey(req.index))
                {
                    int logtimes = erroHostLog[req.index];
                    logtimes++;
                    erroHostLog[req.index] = logtimes;
                    if (logtimes >= 5)
                    {
                        errorHostIndex = req.index;
                        //						Debug.LogFormat(" errorHostIndex = {0}", errorHostIndex);
                    }
                }
                else
                    erroHostLog[req.index] = 1;


                string key = CUtils.GetUDKey("", req.url);
                var info = downloadings[key];
                if (info.index < hosts.Length - 1)
                {
                    info.index++;
#if UNITY_EDITOR
                    Debug.LogFormat("reload {0}  save path = {1} ", req.url, req.saveName);
#endif
                    BeginLoad(info);
                }
                else
                {
                    OnFileDown(req.url, false);
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
            while (queue.Count > 0)
            {
                var info = (LoadedInfo)queue[0];
                queue.RemoveAt(0);

                if (onOneDone != null)
                    onOneDone(info.url, info.isError);
            }
            if (queue.Count == 0 && isAllDone)
            {

                if (allDone != null)
                    allDone(isAllDone);

                isAllDone = false;
            }
        }


        void OnDestroy()
        {
            WebDownload item;
            for (int i = 0; i < webClients.Length; i++)
            {
                item = webClients[i];
                item.Dispose();
            }
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



    public class WebDownload : WebClient
    {
        public bool isFree = true;

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