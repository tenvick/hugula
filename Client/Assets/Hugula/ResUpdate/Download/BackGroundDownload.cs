﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Hugula.Collections;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using Hugula.Framework;

namespace Hugula.ResUpdate
{
    ///<summary>
    /// 资源加载
    /// 1.只统计整体进度。
    /// 2.需要支持组foldermanifest加载?
    /// 3.支持大文件加载。
    /// 4.可以调整组foldermanifest的优先级。
    /// 5.一个组加载完毕才能进行下一个组的加载。
    ///</summary>
    public class BackGroundDownload : BehaviourSingleton<BackGroundDownload>
    {
        ///<summary>
        /// 同时最大下载文件数量
        /// </summary>
        public static int MaxLoadCount = 3;

        ///<summary>
        /// 下载的cdn地址组
        /// </summary>
        public static string[] rootHosts;

        ///<summary>
        /// zip文件处理,子线程
        /// </summary>
        public static System.Action<FileGroupMap> onZipFileDownload;

        public static void Init()
        {
            onZipFileDownload = OnZipFileDownload;
        }

        #region 进度相关
        private readonly object syncRoot = new object();
        private DownloadingProgressChangedEventArgs progressChangedEventArgs;

        private static int totalReceived;
        private static long lastRecordTime;
        //每秒接受到的字节数量
        public static int BytesReceivedPerSecond
        {
            get;
            private set;
        }

        static void ReSetReceiveBytes()
        {
            lastRecordTime = 0;
            // totalReceived = 0;
        }

        static void CalcReceiveBytes(int num)
        {
            if (lastRecordTime == 0)
            {
                lastRecordTime = System.DateTime.Now.Ticks;
                totalReceived = num;
            }
            else
            {
                totalReceived += num;
                var ts = new TimeSpan(System.DateTime.Now.Ticks - lastRecordTime);
                if (ts.TotalSeconds >= 1)
                {
                    BytesReceivedPerSecond = (int)(totalReceived / ts.TotalSeconds);
                    ReSetReceiveBytes();
                }
            }
        }

        // 超过4M的资源使用断点续传多线程方式下载
        public static int BreakPointLength = 4194304;

        #endregion

        #region  monobehav

        void Update()
        {

            while (loadedFiles.Count > 0)
            {
                var fileResInfo = (FileResInfo)loadedFiles[0];
                loadedFiles.RemoveAt(0);
                RemoveTask(fileResInfo);
            }

            CalcReceiveBytes(0);

            foreach (var bQueue in loadingTasks.Values)
                bQueue.DispatchProgressChanged();

        }
        #endregion


        #region  下载相关
        ///<summary>
        /// 将要下载的文件组
        /// </summary>
        List<FolderManifestQueue> willLoadFolders = new List<FolderManifestQueue>();

        ///<summary>
        /// 正在下载的文件组
        /// </summary>
        List<FolderManifestQueue> loadingFolders = new List<FolderManifestQueue>();

        ///<summary>
        /// 下载完成的文件列表
        /// </summary>
        ArrayList loadedFiles = ArrayList.Synchronized(new ArrayList());

        ///<summary>
        /// 下载文件信息记录
        /// </summary>
        SafeDictionary<string, FolderManifestQueue> loadingTasks = new SafeDictionary<string, FolderManifestQueue>();

        ///<summary>
        /// 有同名的文件在被加载
        /// </summary>
        SafeDictionary<string, List<FileGroupMap>> duploadingWait = new SafeDictionary<string, List<FileGroupMap>>();

        string m_PersistentPath;
        string persistentPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PersistentPath))
                {
                    m_PersistentPath = CUtils.GetRealPersistentDataPath() + "/";
                    if (!Directory.Exists(m_PersistentPath)) Directory.CreateDirectory(m_PersistentPath);
                }

                return m_PersistentPath;
            }
        }

        ///<summary>
        /// 添加一组下载文件
        /// </summary>
        public uint AddFolderManifest(FolderManifest folder, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onComplete)
        {
            if (folder.Count <= 0) return 0;
            var folderQueue = new FolderManifestQueue();
            folderQueue.SetFolder(folder, null, onProgress, onComplete);

            willLoadFolders.Add(folderQueue);
            willLoadFolders.Sort((a, b) =>
            {
                return a.priority - b.priority;
            });
            return folder.totalSize;
        }

        ///<summary>
        /// 添加多组下载文件
        /// </summary>
        public uint AddFolderManifests(List<FolderManifest> folders, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onComplete)
        {
            FolderManifest item;
            var group = new FolderQueueGroup();
            for (int i = 0; i < folders.Count; i++)
            {
                var folderQueue = new FolderManifestQueue();
                item = folders[i];
                if (item.Count > 0)
                {
                    folderQueue.SetFolder(item, group, onProgress, onComplete);
                    willLoadFolders.Add(folderQueue);
                }
            }
            willLoadFolders.Sort((a, b) =>
            {
                return a.priority - b.priority;
            });
            
            return (uint)group.totalBytesToReceive;
        }

        ///<summary>
        /// 重新加载失败的组
        /// </summary>
        public uint ReLoadErrorGroup(FolderQueueGroup group)
        {
            var children = group.children;
            FolderManifestQueue queue;
            for (int i = 0; i < children.Count; i++)
            {
                queue = children[i];
                if (queue.isError)
                {
                    queue.ReLoadError();
                    willLoadFolders.Add(queue);
                }
            }
            return 0;
        }

        ///<summary>
        /// 开始下载
        /// </summary>
        public void Begin()
        {
            if (!enabled) enabled = true;
            ReSetReceiveBytes();
            LoadingQueue();
        }

        void LoadingQueue()
        {
            lock (syncRoot)
            {
                FolderManifestQueue queue;
                while (loadingTasks.Count < MaxLoadCount) //从will load里面读取文件夹列表
                {
                    //取loading列表
                    while (loadingFolders.Count <= 2 && willLoadFolders.Count > 0) //同时加载两个文件夹
                    {
                        var load = willLoadFolders[0];
                        willLoadFolders.RemoveAt(0);
                        loadingFolders.Add(load);
                    }

                    if (loadingFolders.Count == 0) return; //没有加载文件夹

                    for (int i = 0; i < loadingFolders.Count; i++)
                    {
                        queue = loadingFolders[i];
                        while (!queue.isEmpty)
                        {
                            var reqInfo = queue.Dequeue();
                            RunningTask(reqInfo, queue);
                            if (loadingTasks.Count >= MaxLoadCount) return;//队列满了
                        }
                        if (queue.isEmpty)
                        {
                            loadingFolders.RemoveAt(i);
                            continue;
                        }
                        else
                        {
                            i++;
                        }
                    }

                }

            }
        }

        internal void RemoveTask(FileResInfo fileInfo)
        {
            if (loadingTasks.TryGetValue(fileInfo.name, out var bQueue))
            {
                bool isError = fileInfo.state != FileInfoState.Success;
                loadingTasks.Remove(fileInfo.name);
                bQueue.Complete(fileInfo, isError);
                if (duploadingWait.TryGetValue(fileInfo.name, out var list))
                {
                    FileGroupMap fGroup = null;
                    for(int i=0;i<list.Count;i++)
                    {
                        fGroup = list[i];
                        fGroup.groupQueue.Complete(fGroup.fileResInfo,isError);
                    }
                    list.Clear();
                    duploadingWait.Remove(fileInfo.name);
                }

            }

            LoadingQueue();
        }

        internal void RunningTask(FileResInfo fileInfo, FolderManifestQueue bQueue)
        {
            if (loadingTasks.ContainsKey(fileInfo.name))
            {
#if !HUGULA_NO_LOG
                Debug.LogFormat("RunningTask file({0},state={1})  is loadingtime={2}", fileInfo.name, fileInfo.state, System.DateTime.Now);
#endif             
                var fGroup = new FileGroupMap(fileInfo, bQueue); 
                if (duploadingWait.TryGetValue(fileInfo.name, out var list))
                {
                    list.Add(fGroup);
                }
                else
                {
                    list = new List<FileGroupMap>();
                    list.Add(fGroup);
                    duploadingWait.Add(fileInfo.name,list);
                }
                return;
            }

#if !HUGULA_NO_LOG
            Debug.LogFormat("RunningTask fileName={0},state={1}, time={2}", fileInfo.name, fileInfo.state, System.DateTime.Now);
#endif              
            if (FileManifestManager.CheckPersistentCrc(fileInfo)) //只做了文件大小验证
            {
                if (fileInfo.name.EndsWith("zip") && onZipFileDownload != null) //如果是zip文件自动解压
                {
                    try
                    {
                        onZipFileDownload(new FileGroupMap(fileInfo, bQueue));
                    }
                    catch (Exception ex)
                    {
                        fileInfo.state = FileInfoState.Fail;
                        Debug.LogError($"zip Persistent crc error:  {ex.Message} \r\n:{ex.StackTrace}");
                    }
                }
                // loadingTasks[fileInfo.name] = bQueue; //校验成功？
                loadingTasks.Add(fileInfo.name, bQueue);
                loadedFiles.Add(fileInfo);
#if !HUGULA_NO_LOG
                Debug.LogFormat("RunningTask CheckPersistentCrc({0},state={1}) time={2}", fileInfo.name, fileInfo.state, System.DateTime.Now);
#endif    
                return;
            }
            else //if (!)
            {
                // loadingTasks[fileInfo.name] = bQueue;
                loadingTasks.Add(fileInfo.name, bQueue);
                if (fileInfo.state == FileInfoState.Fail)
                    FileHelper.DeletePersistentFile(fileInfo.name);
                var download = WebDownload.Get();
                var fileGroupMap = fileGroupMapPool.Get();
                fileGroupMap.SetMap(fileInfo, bQueue);
                download.userData = fileGroupMap;
                download.DownloadFileCompleted = OnDownloadFileCompleted;
                download.DownloadProgressChanged = OnDownloadProgressChanged;
                string urlHost = rootHosts[0];
                internalLoad(download, fileInfo, urlHost);
            }


        }

        void internalLoad(WebDownload download, FileResInfo fileInfo, string urlHost, string timestamp = "")
        {
            Uri url = null;
            string fileName = fileInfo.name;
            if (!CUtils.CheckFullUrl(fileName))
            {
                if (string.IsNullOrEmpty(timestamp))
                    url = new Uri(CUtils.PathCombine(urlHost, fileName));
                else
                    url = new Uri(CUtils.PathCombine(urlHost, fileName + "?" + timestamp));
            }
            else
            {
                url = new Uri(fileName);
            }

            string path = GetPersistentFilePath(fileInfo);
            FileHelper.CheckCreateFilePathDirectory(path);
            if (fileInfo.size < BreakPointLength)
                download.DownloadFileAsync(url, path);
            else
                download.DownloadFileMultiAsync(url, path);

#if !HUGULA_NO_LOG
            Debug.LogFormat(" begin load {0} ,save path ={1},abInfo.state={2} ,webClient({3}) dateTime={4}", url.AbsoluteUri, path, fileInfo.state, download, System.DateTime.Now);
#endif
        }

        string GetPersistentFilePath(FileResInfo abInfo)
        {
            var fname = Path.GetFileName(abInfo.name);
            string path = CUtils.PathCombine(persistentPath, fname);
            return path;
        }

        void OnDownloadProgressChanged(object sender, DownloadingProgressChangedEventArgs e)
        {
            CalcReceiveBytes(e.BytesRead);

            WebDownload webd = (WebDownload)sender;
            var groupMap = (FileGroupMap)webd.userData;
            var finfo = groupMap.fileResInfo;
            // Debug.Log($"OnDownloadProgressChanged：{finfo.name} :loadbytes{e.BytesRead} szie：{finfo.size} {System.DateTime.Now}");
            groupMap.groupQueue.OnDownloadProgressChanged(finfo, e);

            if (progressChangedEventArgs != null)
                progressChangedEventArgs.received += e.BytesReceived;
        }

        void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            WebDownload webd = (WebDownload)sender;
            var groupMap = (FileGroupMap)webd.userData;
            FileResInfo abInfo = groupMap.fileResInfo;
            webd.tryTimes++;
            int tryTimes = webd.tryTimes;
            if (e.Error != null)
            {
                if (tryTimes <= rootHosts.Length * 3)
                {
                    int i = tryTimes % rootHosts.Length;
                    Debug.LogWarning(string.Format("background download error ab:{0}, tryTimes={1},host={2},error:{3}", abInfo.name, webd.tryTimes, rootHosts[i], e.Error));
                    internalLoad(webd, abInfo, rootHosts[i], tryTimes.ToString());
                    return;
                }
                else
                {
                    Debug.LogErrorFormat("background download error message {0} \r\n trace {1}", e.Error.Message, e.Error.StackTrace);
                    abInfo.state = FileInfoState.Fail; // none or fail?
                    loadedFiles.Add(abInfo);
                    ReleaseWebDonwLoad(webd);
                }
            }
            else
            {
                string path = GetPersistentFilePath(abInfo);
                FileInfo tmpFile = new FileInfo(path);
                if (tmpFile.Length == abInfo.size) //check size
                {
                    abInfo.state = FileInfoState.Success;
                    if (abInfo.name.EndsWith("zip") && onZipFileDownload != null) //如果是zip文件自动解压
                    {
                        try
                        {
                            onZipFileDownload(groupMap);
                        }
                        catch (Exception ex)
                        {
                            abInfo.state = FileInfoState.Fail;
                            Debug.LogError($"{ex.Message} \r\n:{ex.StackTrace}");
                        }
                    }
                    loadedFiles.Add(abInfo);
                    ReleaseWebDonwLoad(webd);
                }
                else if (tryTimes <= rootHosts.Length * 3)
                {
                    int i = tryTimes % rootHosts.Length;
                    Debug.LogWarningFormat("check file is wrong (length:{1}!=size:{2}) name={0},tryTimes={3},crc={4},host:{5}", abInfo.name, tmpFile.Length, abInfo.size, tryTimes, abInfo.crc32, rootHosts[i]);
                    tmpFile.Delete(); //删除错误文件
                    internalLoad(webd, abInfo, rootHosts[i], tryTimes.ToString());
                    return;
                }
                else
                {
                    Debug.LogWarningFormat("download check file is wrong (length:{1}!=size:{2}) name={0},tryTimes={3},crc={4},host:{5}", abInfo.name, tmpFile.Length, abInfo.size, tryTimes, abInfo.crc32, rootHosts[tryTimes % rootHosts.Length]);
                    abInfo.state = FileInfoState.Fail; // none or fail?
                    loadedFiles.Add(abInfo);
                    ReleaseWebDonwLoad(webd);
                }
            }

        }

        void ReleaseWebDonwLoad(WebDownload webd)
        {
            lock (syncRoot)
            {
                if (webd.userData is FileGroupMap)
                {
                    fileGroupMapPool.Release((FileGroupMap)webd.userData);
                }
                WebDownload.Release(webd);
            }
        }

        #endregion

        #region  解压
        ///<summary>
        /// 默认解压函数
        ///</summary>
        public static void OnZipFileDownload(FileGroupMap groupMap)
        {
            var abinfo = groupMap.fileResInfo;
            var name = Path.GetFileName(abinfo.name);
            var folderName = Path.GetFileNameWithoutExtension(name);
            var source = Path.Combine(CUtils.realPersistentDataPath, name);
            var targetFolder = Path.Combine(CUtils.realPersistentDataPath, folderName);

            Debug.Log($"zipfile:{source} to :{targetFolder} {System.DateTime.Now}");
            ZipHelper.UnpackZipByPath(source, targetFolder);
            Debug.Log($"finish zipfile:{source} {System.DateTime.Now}");

        }
        #endregion

        #region  pool
        public static ObjectPool<FileGroupMap> fileGroupMapPool = new ObjectPool<FileGroupMap>(null, OnFileGroupMapRelease);

        private static void OnFileGroupMapRelease(FileGroupMap fileGroupMap)
        {
            fileGroupMap?.Clear();
        }

        #endregion

    }

    public class FileGroupMap
    {
        public FileGroupMap()
        {

        }

        public FileGroupMap(FileResInfo fileResInfo, FolderManifestQueue groupQueue)
        {
            this.fileResInfo = fileResInfo;
            this.groupQueue = groupQueue;
        }
        public void Clear()
        {
            this.fileResInfo = null;
            this.groupQueue = null;
        }

        public void SetMap(FileResInfo fileResInfo, FolderManifestQueue groupQueue)
        {
            this.fileResInfo = fileResInfo;
            this.groupQueue = groupQueue;
        }

        public FileResInfo fileResInfo;
        public FolderManifestQueue groupQueue;
    }

}