using System;
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
        /// 下载res文件下的cdn地址组
        /// </summary>
        public static string[] rootHosts;

        ///<summary>
        /// 拼接cdn与fileResinfo.name然后下载资源，注意下载文件名需要带上crc信息
        /// </summary>
        public static Func<FileResInfo, string, string, string> InternalIdTransformFunc;

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
        public static int BreakPointLength = 4194304 / 2;

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

            foreach(var bQueue in  loadingFolders)
            {
                bQueue.group.DispatchProgressChanged();
            }

            // foreach (var bQueue in loadingTasks.Values)
            //     bQueue.group.DispatchProgressChanged();

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            WebDownload web;
            for (int i = 0; i < loadingWebDownload.Count; i++) //
            {
                web = loadingWebDownload[i];
                web.CancelAsync();
                web.Dispose();
            }

            loadingWebDownload.Clear();

        }
        #endregion


        #region  下载相关
        bool isPause = false;
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
        /// 加载器列表
        /// </summary>
        List<WebDownload> loadingWebDownload = new List<WebDownload>();

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
        /// 下载zip文件
        /// </summary>
        public uint AddZipFolderManifest(FolderManifest folder, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onItemComplete, System.Action<FolderQueueGroup, bool> onAllComplete)
        {
            return AddFolderManifest(CheckZipOrFiles(folder), onProgress, onItemComplete, onAllComplete);
        }

        /// <summary>
        /// 检测是zip下载还是单文件
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private FolderManifest CheckZipOrFiles(FolderManifest f)
        {
            FolderManifest persisFolderManifest = null;
            var zipOutPath = f.GetZipOutFolderPath();
            zipOutPath = Path.Combine(zipOutPath, f.fileName + Common.CHECK_ASSETBUNDLE_SUFFIX);
            var newFastManifest = f.CloneWithOutAllFileInfos();
            newFastManifest.transformZipFolder = true;
            bool needDownloadZipfile = true;
            if (File.Exists(zipOutPath))
            {
#if !HUGULA_NO_LOG
                Debug.Log($"zip folder({zipOutPath}) has already loaded ,check incremental updating");
#endif

                AssetBundle ab = null;
                //读取persistent文件夹下的package文件
                if ((ab = AssetBundle.LoadFromFile(zipOutPath, 0, Common.BUNDLE_OFF_SET)) != null)
                {
                    var folderManifests = ab.LoadAllAssets<FolderManifest>();
                    if (folderManifests.Length > 0)
                        persisFolderManifest = folderManifests[0];

                    ab.Unload(false);
                }

                if (persisFolderManifest != null)
                {
                    var streamingFolderManifest = FileManifestManager.FindStreamingFolderManifest(f.fileName);
                    if (streamingFolderManifest != null)
                    {
                        var flist = streamingFolderManifest.NotSafeCompare(persisFolderManifest,true); 
                        //check size
                        if (flist.Count == 0)//没有变更
                        {
                            needDownloadZipfile = false;
                            f.MarkZipDone(); //标记完成
                            return newFastManifest;
                        }

                        uint size = 0;
                        foreach (var file in flist)
                        {
                            size += file.size;
                        }

                        if (size < f.zipSize) //增量更新
                        {
                            newFastManifest.allFileInfos = flist;
                            needDownloadZipfile = false;
#if !HUGULA_NO_LOG
                            Debug.Log($"zip persisFolderManifest info");
                            Debug.Log(persisFolderManifest.ToString());
                            Debug.Log($"add filelist:{flist.Count},size={size} new load:");
                            Debug.Log(newFastManifest.ToString());
#endif
                        }
                        else
                        {
                            needDownloadZipfile = true;//增量大于了zip包尺寸全量更新
                        }
                    }
                }
            }

            if (f.zipSize > 0 && needDownloadZipfile) //zip包方式加载
            {
                uint crc = 0;
                uint.TryParse(f.zipVersion, out crc);
                newFastManifest.Add(new FileResInfo(f.fileName + ".zip", crc, f.zipSize));
#if !HUGULA_NO_LOG
                print($"add zipName:{f.zipName}.zip");
                Debug.Log(f.ToString());
                Debug.Log(newFastManifest.ToString());
#endif
            }

            return newFastManifest;
        }

        ///<summary>
        /// 下载多个zip文件
        /// </summary>
        public uint AddZipFolderManifests(List<FolderManifest> folders, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onItemComplete, System.Action<FolderQueueGroup, bool> onAllComplete)
        {
            List<FolderManifest> newFolders = new List<FolderManifest>();
            foreach (var f in folders)
            {
                newFolders.Add(CheckZipOrFiles(f));
            }

            return AddFolderManifests(newFolders, onProgress, onItemComplete, onAllComplete);
        }

        ///<summary>
        /// 添加一组下载文件
        /// </summary>
        public uint AddFolderManifest(FolderManifest folder, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onItemComplete, System.Action<FolderQueueGroup, bool> onAllComplete)
        {
            // if (folder.Count <= 0) return 0;
            var folderQueue = new FolderManifestQueue();
            var group = new FolderQueueGroup(onProgress, onItemComplete, onAllComplete);
            folderQueue.SetFolder(folder, group);
            if (folder.Count > 0)
            {
                willLoadFolders.Add(folderQueue);
                willLoadFolders.Sort((a, b) =>
                {
                    return a.priority - b.priority;
                });
                return folder.totalSize;
            }
            else
            {
                folderQueue.Complete(null, false);
            }

           return 0;
        }

        ///<summary>
        /// 下载address依赖的zip包
        /// </summary>
        public int AddZipFolderByAddress(string address, System.Type type = null, System.Action<LoadingEventArg> onProgress = null, System.Action<FolderManifestQueue, bool> onItemComplete = null, System.Action<FolderQueueGroup, bool> onAllComplete = null)
        {
            var folders = FileManifestManager.FindFolderManifestByAddress(address, type);
            if (folders == null)
            {
                return -1;//没有找到文件夹信息
            }
            //except stream
            for (int i = 0; i < folders.Count; i++)
            {
                if (folders[i].fileName == Common.FOLDER_STREAMING_NAME)
                {
                    folders.RemoveAt(i);
                    break;
                }
            }

            return (int)AddZipFolderManifests(folders, onProgress, onItemComplete, onAllComplete);
        }

        ///<summary>
        /// 添加多组下载文件
        /// </summary>
        public uint AddFolderManifests(List<FolderManifest> folders, System.Action<LoadingEventArg> onProgress, System.Action<FolderManifestQueue, bool> onItemComplete, System.Action<FolderQueueGroup, bool> onAllComplete)
        {
            FolderManifest item;
            var group = new FolderQueueGroup(onProgress, onItemComplete, onAllComplete);
            for (int i = 0; i < folders.Count; i++)
            {
                var folderQueue = new FolderManifestQueue();
                item = folders[i];
                folderQueue.SetFolder(item, group);
                if (item.Count > 0)
                {
                    willLoadFolders.Add(folderQueue);
                }
                else
                {
                    folderQueue.Complete(null, false);
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
                    queue.ReEnqueueLoadError();
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
            if (isPause && loadingWebDownload.Count > 0)
            {
                isPause = false;
                ContinueDownLoad();//继续下载
            }
            else
            {
                isPause = false;
                LoadingQueue();
            }
        }

        ///<summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            enabled = false;
            isPause = true;
            for (int i = 0; i < loadingWebDownload.Count; i++) //
            {
                loadingWebDownload[i].CancelAsync();
            }
            ReSetReceiveBytes();
        }

        ///<summary>
        /// 暂停下载
        /// </summary>
        void ContinueDownLoad()
        {
            WebDownload webDownload;
            for (int i = 0; i < loadingWebDownload.Count; i++) //
            {
                webDownload = loadingWebDownload[i];
                var groupMap = (FileGroupMap)webDownload.userData;
                FileResInfo abInfo = groupMap.fileResInfo;
                webDownload.tryTimes = 0;
                internalLoad(webDownload, abInfo, rootHosts[0]);
            }
        }

        void LoadingQueue()
        {
            lock (syncRoot)
            {
                if (isPause) return;
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
                            if (reqInfo.size > 0) //存在的资源才会下载
                                RunningTask(reqInfo, queue);
                            if (loadingTasks.Count >= MaxLoadCount) return;//队列满了
                        }
                        if (queue.isEmpty)
                        {
                            loadingFolders.RemoveAt(i);
                            // if(queue.isDown) //空列表触发下载完成事件
                            //     queue.Complete(null, false);
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        fGroup = list[i];
                        fGroup.groupQueue.Complete(fGroup.fileResInfo, isError);
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
                    duploadingWait.Add(fileInfo.name, list);
                }
                return;
            }

#if !HUGULA_NO_LOG
            Debug.LogFormat("RunningTask fileName={0},state={1}, time={2}", fileInfo.name, fileInfo.state, System.DateTime.Now);
#endif              
            if (FileManifestManager.CheckPersistentCrc(fileInfo)) //只做了文件大小验证
            {
                if (onZipFileDownload == null)
                {
                    onZipFileDownload = OnZipFileDownload;
                }

                if (fileInfo.name.EndsWith("zip")) //如果是zip文件自动解压
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

                loadingTasks.Add(fileInfo.name, bQueue);
                loadedFiles.Add(fileInfo);
#if !HUGULA_NO_LOG
                Debug.LogFormat("RunningTask CheckPersistentCrc({0},state={1}) time={2}", fileInfo.name, fileInfo.state, System.DateTime.Now);
#endif    
                return;
            }
            else
            {

                loadingTasks.Add(fileInfo.name, bQueue);
                if (fileInfo.state == FileInfoState.Fail)
                    FileHelper.DeletePersistentFile(fileInfo.name);
                var download = new WebDownload();//  WebDownload.Get();
                var fileGroupMap = fileGroupMapPool.Get();
                fileGroupMap.SetMap(fileInfo, bQueue);
                download.userData = fileGroupMap;
                download.DownloadFileCompleted = OnDownloadFileCompleted;
                download.DownloadProgressChanged = OnDownloadProgressChanged;
                string urlHost = rootHosts[0];
                if (!loadingWebDownload.Contains(download))
                    loadingWebDownload.Add(download);
                internalLoad(download, fileInfo, urlHost);
            }


        }

        void internalLoad(WebDownload download, FileResInfo fileInfo, string urlHost, string timestamp = "")
        {
            Uri url = null;

            var groupMap = (FileGroupMap)download.userData;
            var folderManifest = groupMap.groupQueue.currFolder;
            //下载目录拼接
            if (folderManifest.transformZipFolder) //zip文件夹 下载目录与普通热更新不一样
            {
                if (fileInfo.name.EndsWith(".zip"))
                {
                    urlHost = CUtils.PathCombine(urlHost, HotResConfig.PACKAGE_FOLDER_NAME);
                }
                else
                {
                    urlHost = Path.Combine(urlHost, HotResConfig.PACKAGE_FOLDER_NAME, folderManifest.fileName);
                }
            }
            else
            {
                urlHost = CUtils.PathCombine(urlHost, Common.RES_VER_FOLDER);
            }

            if (InternalIdTransformFunc != null)
                url = new Uri(InternalIdTransformFunc(fileInfo, urlHost, timestamp));
            else
                url = new Uri(OverrideFullHostURL(fileInfo, urlHost, timestamp));

            var savePath = GetPersistentFilePath(folderManifest, fileInfo);
            FileHelper.CheckCreateFilePathDirectory(savePath);
            if (fileInfo.size < BreakPointLength)
                download.DownloadFileAsync(url, savePath);
            else
                download.DownloadFileMultiAsync(url, savePath);

#if !HUGULA_NO_LOG
            Debug.LogFormat(" begin load {0} ,save path ={1},abInfo.state={2} ,webClient({3}) dateTime={4}", url.AbsoluteUri, savePath, fileInfo.state, download, System.DateTime.Now);
#endif
        }

        string GetPersistentFilePath(FolderManifest folderManifest, FileResInfo abInfo)
        {
            var fname = CUtils.GetFileName(abInfo.name);
            string path = string.Empty;
            if (folderManifest.transformZipFolder && !abInfo.name.EndsWith(".zip"))
            {
                path = Path.Combine(persistentPath, folderManifest.fileName, fname);
            }
            else
            {
                path = CUtils.PathCombine(persistentPath, fname);
            }

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
                progressChangedEventArgs.received += e.BytesRead;
        }

        void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            WebDownload webd = (WebDownload)sender;
            var groupMap = (FileGroupMap)webd.userData;
            FileResInfo abInfo = groupMap.fileResInfo;
#if !HUGULA_NO_LOG
            Debug.Log($"OnDownloadFileCompleted({sender}),fileName:{abInfo.name},{System.DateTime.Now}\r\nerror:{e.Error}");
#endif
            if (webd.interrupt) //如果是用户中断不处理
            {
                Debug.LogWarningFormat("background download interrupt ab:{0}, tryTimes={1},host={2}\r\nerror:{3}", abInfo.name, webd.tryTimes, rootHosts[webd.tryTimes % rootHosts.Length], e.Error);
                return;
            }
            webd.tryTimes++;
            int tryTimes = webd.tryTimes;
            if (e.Error != null)
            {
                if (tryTimes <= rootHosts.Length * 3)
                {
                    int i = tryTimes % rootHosts.Length;
                    Debug.LogWarning(string.Format("background download error ab:{0}, tryTimes={1},host={2}\r\nerror:{3}", abInfo.name, webd.tryTimes, rootHosts[i], e.Error));
                    internalLoad(webd, abInfo, rootHosts[i], tryTimes.ToString());
                    return;
                }
                else
                {
                    Debug.LogErrorFormat("background download file({0}) error message {1} \r\n trace {2}", abInfo.name, e.Error.Message, e.Error.StackTrace);
                    abInfo.state = FileInfoState.Fail; // none or fail?
                    loadedFiles.Add(abInfo);
                    ReleaseWebDonwLoad(webd);
                }
            }
            else
            {
                string path = GetPersistentFilePath(groupMap.groupQueue.currFolder, abInfo);
                FileInfo tmpFile = new FileInfo(path);
                if (tmpFile.Length == abInfo.size) //check size
                {
                    abInfo.state = FileInfoState.Success;
                    if (onZipFileDownload == null)
                    {
                        onZipFileDownload = OnZipFileDownload;
                    }
                    if (abInfo.name.EndsWith("zip")) //如果是zip文件自动解压
                    {
                        try
                        {
                            onZipFileDownload(groupMap);
                        }
                        catch (Exception ex)
                        {
                            abInfo.state = FileInfoState.Fail;
                            Debug.LogError($"zip file({abInfo.name}) \r\n error:{ex.Message} \r\n:{ex.StackTrace}");
                        }
                    }

                    loadedFiles.Add(abInfo);
                    ReleaseWebDonwLoad(webd);
#if !HUGULA_NO_LOG
                    Debug.LogWarning($"background download completed ab:{abInfo.name},crc={abInfo.crc32}, tryTimes={tryTimes},host={rootHosts[tryTimes % rootHosts.Length]}");
#endif
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
                // WebDownload.Release(webd);
                loadingWebDownload.Remove(webd);
                webd.Dispose();
            }
        }

        #endregion

        #region  辅助
        ///<summary>
        /// 默认解压函数
        ///</summary>
        public static void OnZipFileDownload(FileGroupMap groupMap)
        {
            lock (groupMap.fileResInfo)
            {
                var abinfo = groupMap.fileResInfo;
                var name = Hugula.Utils.CUtils.GetFileName(abinfo.name);
                var folderName = Path.GetFileNameWithoutExtension(name);
                var source = Path.Combine(CUtils.realPersistentDataPath, name);
                var targetFolder = Path.Combine(CUtils.realPersistentDataPath, folderName);

                Debug.Log($"zipfile:{source} to :{targetFolder} crc:{abinfo.crc32} {System.DateTime.Now}");
                ZipHelper.UnpackZipByPath(source, targetFolder);
                Hugula.Executor.Execute(groupMap.groupQueue.currFolder.MarkZipDone);//放主线程执行
                File.Delete(source); //删除zip文件
                Debug.Log($"finish zipfile:{source} {System.DateTime.Now}");
            }
        }

        public static string OverrideFullHostURL(FileResInfo fileInfo, string host, string timestamp = "")
        {
            string fileName = fileInfo.name;
            //文件名带crc信息
            if (fileInfo.crc32 > 0) // ==0不需要添加crc号
            {
                fileName = CUtils.InsertAssetBundleName(fileName, $"_{fileInfo.crc32}");
            }
            // string resFolder = Common.RES_VER_FOLDER; // HotResConfig.RESOURCE_FOLDER_NAME;
            // if (fileInfo.name.EndsWith(".zip"))
            // {
            //     resFolder = HotResConfig.PACKAGE_FOLDER_NAME;
            // }

            if (!CUtils.CheckFullUrl(fileName))
            {
                fileName = Path.Combine(host, fileName); //new Uri(CUtils.PathCombine(urlHost, fileName + "?" + timestamp));
                if (!string.IsNullOrEmpty(timestamp))
                    fileName = fileName + "?" + timestamp;
            }

            return fileName;
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