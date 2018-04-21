using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Hugula.Collections;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Update {

    [SLua.CustomLuaClassAttribute]
    public class BackGroundDownload : MonoBehaviour {
        private readonly object syncRoot = new object ();

        private const string _keyCarrierDataNetwork = "_keyCarrierDataNetwork";
        public string[] hosts;

        private static int totalReceived;
        private static long lastRecordTime;
        //每秒接受到的字节数量
        public static int BytesReceivedPerSecond {
            get;
            private set;
        }

        // 超过4M的资源使用断点续传多线程方式下载
        public static int BreakPointLength = 4194304;

        public int loadingCount = 2;

        //开启下载
        public bool alwaysDownLoad {
            get {
                return m_alwaysDownLoad;
            }
            set {
                m_alwaysDownLoad = value;
            }
        }

        private static int _carrierDataNetwork = -1;
        public static bool carrierDataNetwork {
            get {
                if (_carrierDataNetwork == -1)
                    _carrierDataNetwork = PlayerPrefs.GetInt (_keyCarrierDataNetwork, 0);
                return _carrierDataNetwork == 1;
            }
            set {
                _carrierDataNetwork = value ? 1 : 0;
                PlayerPrefs.SetInt ("_keyCarrierDataNetwork", _carrierDataNetwork);
            }
        }

        // public System.Action<BackGroundDownload> onNetSateChange;
#if UNITY_EDITOR
        [SLua.DoNotToLuaAttribute]
        public int currentLoadingCount = 0;
        [SLua.DoNotToLuaAttribute]
        public string loadingList;
#endif
        private DownloadingProgressChangedEventArgs progressChangedEventArgs;
        private bool m_alwaysDownLoad = false;
        private NetworkReachability netState;
        private SafeDictionary<int, BackGroundQueue> loadQueueDic = new SafeDictionary<int, BackGroundQueue> ();
        private LinkedList<BackGroundQueue> loadQueue = new LinkedList<BackGroundQueue> ();
        private SafeDictionary<ABInfo, object> loadingTasks = new SafeDictionary<ABInfo, object> ();

        private ABInfoComparer abInfoCompare = new ABInfoComparer ();

        ArrayList webClients = ArrayList.Synchronized (new ArrayList ());
        string outputPath;
        int completeCount;
        void Awake () {
            outputPath = CUtils.GetRealPersistentDataPath () + "/";
            if (!Directory.Exists (outputPath)) Directory.CreateDirectory (outputPath);
            netState = Application.internetReachability;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start () {
            // CheckLoadingCount ();
            // if (this.loadingCount == 0 ) DispatchNetStateChange();
        }

        /// <summary>
        /// Callback sent to all game objects when the player gets or loses focus.
        /// </summary>
        /// <param name="focusStatus">The focus state of the application.</param>
        void OnApplicationFocus (bool focusStatus) {

        }

        // Update is called once per frame
        void Update () {
            // if (netState != Application.internetReachability) {
            //     netState = Application.internetReachability;
            //     CheckLoadingCount ();
            //     DispatchNetStateChange ();
            // }

            completeCount = 0;
            while (webClients.Count > 0) {
                var arr = (object[]) webClients[0];
                webClients.RemoveAt (0);
                RemoveTask ((ABInfo) arr[0], (BackGroundQueue) arr[1]);
                completeCount++;
                if(completeCount >= loadingCount) break;
            }

            CalcReceiveBytes (0);

            foreach (BackGroundQueue backGroundQueue in loadQueueDic.Values)
                backGroundQueue.DispatchOnProgress ();
                
#if UNITY_EDITOR
            currentLoadingCount = loadingTasks.Count;
            var str = string.Empty;
            // lock (syncRoot) {
            //     foreach (var abInfo in loadingTasks.Keys) {
            //         str += "\r\n" + abInfo.abName;
            //     }
            // }
            loadingList = str;
#endif
        }

        void OnDestroy () {
            // WebDownload item;
            foreach (object item in loadingTasks.Values) {
                if(item is WebDownload)
                    ((WebDownload)item).Dispose ();
            }
            webClients.Clear ();
            webClients = null;

            loadingTasks.Clear ();
            loadQueue.Clear ();
            loadQueueDic.Clear ();
        }

        void LoadingQueue () {
            lock (syncRoot) {

                LinkedListNode<BackGroundQueue> fristNode = this.loadQueue.First;
                #if !HUGULA_NO_LOG
                    Debug.LogFormat("LoadingQueue.count={0},fristNode={1},canload={2},frame={3}",loadQueue.Count,fristNode,this.loadingCount - this.loadingTasks.Count > 0,Time.frameCount);
                #endif
                while (fristNode != null && this.loadingCount - this.loadingTasks.Count > 0) {
                    BackGroundQueue value = fristNode.Value;
                #if !HUGULA_NO_LOG
                    Debug.LogFormat("BackGroundQueue.Count={0},LoadingQueue.count={1},loading={2},frame={3}",value.Count,loadQueue.Count,this.loadingCount - this.loadingTasks.Count,Time.frameCount);
                #endif
                    if (value.Count > 0) {
                        var abInfo = value.Dequeue ();
                        if (!loadingTasks.ContainsKey (abInfo)) {
                            RunningTask (abInfo, value);
                        }
                    } else {
                        fristNode = fristNode.Next;
                        if(value.IsDown && !value.IsError) 
                            this.loadQueue.Remove (value); //如果有错误需要保持在队列中
                    }
                }
            }

        }

        // void DispatchNetStateChange () {
        //     //网络改变
        //     if (this.enabled && loadingTasks.Count > 0 && this.loadingCount > 0)
        //         LoadingQueue ();

        //     if (onNetSateChange != null)
        //         onNetSateChange (this);
        // }

        private BackGroundQueue GetLoadQueue (int priority) {
            // priority
            BackGroundQueue bQueue = null;
            lock (syncRoot) {
                if (loadQueueDic.ContainsKey (priority)) {
                    bQueue = this.loadQueueDic[priority];
                } else {
                    bQueue = new BackGroundQueue (priority);
                    this.loadQueueDic[priority] = bQueue;
                }

                bool flag = false;
                for (LinkedListNode<BackGroundQueue> fristNode = this.loadQueue.First; fristNode != null; fristNode = fristNode.Next) {
                    if (fristNode.Value.priority == priority) {
                        this.loadQueue.AddAfter (fristNode, bQueue); //Debug.LogFormat("  fristNode.Value.priority={0}<priority{1}", fristNode.Value.priority, priority);
                        flag = true;
                        break;
                    }
                    if (fristNode.Value.priority < priority) { //大的排前面
                        this.loadQueue.AddBefore (fristNode, bQueue); // Debug.LogFormat("  fristNode.Value.priority={0}<priority{1}", fristNode.Value.priority, priority);
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    this.loadQueue.AddLast (bQueue);
                }
            }

            return bQueue;
        }

        [SLua.DoNotToLuaAttribute]
        public uint AddTask (ABInfo abinfo, int priority, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            if (abinfo == null) return 0;
            var bQueue = GetLoadQueue (priority);
            bQueue.onProgress = onProgress;
            bQueue.onComplete = onComplete;
            bQueue.Enqueue (abinfo);
#if UNITY_EDITOR
            Debug.LogFormat ("bQueue.Count = {0},priority={1}", bQueue.Count, priority);
#endif
            return abinfo.size;
        }

        [SLua.DoNotToLuaAttribute]
        public uint AddTask (List<ABInfo> abInfos, int priority, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            uint size = 0;

            // Debug.LogFormat("AddTask.Count = {0},priority={1}", abInfos.Count, priority);
            if (abInfos.Count == 0) {
                return size;
            }

            var bQueue = GetLoadQueue (priority);
            bQueue.onProgress = onProgress;
            bQueue.onComplete = onComplete;

            bQueue.Enqueue (abInfos);

            foreach (var abInfo in abInfos)
                size += abInfo.size;
            // Debug.LogFormat("bQueue.Count = {0},priority={1}", bQueue.Count, priority);
            return size;
        }

        /// <summary>
        /// 加载first pack 包和更新内容
        /// </summary>
        public uint AddFirstManifestTask (FileManifest mainifest1, FileManifest mainifest2, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            if (mainifest1 == null) return 0;

            List<ABInfo> re = new List<ABInfo> ();
            Dictionary<string, bool> added = new Dictionary<string, bool> ();
            var localABInfos = mainifest1.allAbInfo;
            var diffABInfos = mainifest1.CompareFileManifest (mainifest2); //mainifest2.appNumVersion - mainifest1.appNumVersion >= 0;
            ABInfo abInfo;
            for (int i = 0; i < diffABInfos.Count; i++) {
                abInfo = diffABInfos[i];
                re.Add (abInfo);
                added[abInfo.abName] = true;
            }

            for (int i = 0; i < localABInfos.Count; i++) {
                abInfo = localABInfos[i];
                if (abInfo.priority >= FileManifestOptions.FirstLoadPriority &&
                    abInfo.priority < FileManifestOptions.AutoHotPriority && !added.ContainsKey (abInfo.abName)) //首包下载内容
                {
                    re.Add (abInfo);
                }
            }
            added.Clear ();

            re.Sort (abInfoCompare);

            return AddTask (re, FileManifestOptions.FirstLoadPriority, onProgress, onComplete);
        }

        /// <summary>
        /// 加载更新内容
        /// </summary>
        public uint AddDiffManifestTask (FileManifest mainifest1, FileManifest mainifest2, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            if (mainifest1 == null) return 0;
            var spliteExtension = Hugula.HugulaSetting.instance.spliteExtensionFolder; //启动了分离

            var loadInfos = mainifest1.CompareFileManifest (mainifest2);
            List<ABInfo> re = new List<ABInfo> ();
            foreach (var abInfo in loadInfos) {
                if (!spliteExtension) {
                    re.Add (abInfo);
                } else if (abInfo.priority <= FileManifestOptions.AutoHotPriority) //非自动下载级别 
                {
                    re.Add (abInfo);
                } else if (FileHelper.PersistentFileExists (abInfo.abName)) //存在表示已经下载过但是版本不对
                {
                    re.Add (abInfo);
                }
            }

            re.Sort (abInfoCompare);

            return AddTask (re, FileManifestOptions.FirstLoadPriority, onProgress, onComplete);
        }

        /// <summary>
        /// 加载后置下载内容
        /// </summary>
        public uint AddBackgroundManifestTask (FileManifest mainifest1, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            if (mainifest1 == null) return 0;

            var allAbInfos = mainifest1.allAbInfo;
            List<ABInfo> loadInfos = new List<ABInfo> ();
            ABInfo abInfo;
            for (int i = 0; i < allAbInfos.Count; i++) {
                abInfo = allAbInfos[i];
                if (abInfo.priority < FileManifestOptions.ManualPriority &&
                    abInfo.priority >= FileManifestOptions.AutoHotPriority &&
                    !FileHelper.PersistentFileExists (abInfo.abName)) {
                    loadInfos.Add (abInfo);
                }
            }
            loadInfos.Sort (abInfoCompare);
            return AddTask (loadInfos, FileManifestOptions.StreamingAssetsPriority, onProgress, onComplete);
        }

        /// <summary>
        /// 加载手动下载内容
        /// </summary>
        public uint AddManualManifestTask (FileManifest mainifest1, string folder, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete) {
            if (mainifest1 == null) return 0;

            var allAbInfos = mainifest1.allAbInfo;
            List<ABInfo> loadInfos = new List<ABInfo> ();
            ABInfo abInfo;
            for (int i = 0; i < allAbInfos.Count; i++) {
                abInfo = allAbInfos[i];
                if (abInfo.abName.Contains (folder) && !FileHelper.PersistentFileExists (abInfo.abName)) // && abInfo.priority > FileManifestOptions.AutoHotPriority)
                {
                    loadInfos.Add (abInfo);
                }
            }

            loadInfos.Sort (abInfoCompare);

            return AddTask (loadInfos, FileManifestOptions.ManualPriority, onProgress, onComplete); //手动下载优先级最高
        }

        internal void RemoveTask (ABInfo abInfo, BackGroundQueue bQueue) {
            loadingTasks.Remove (abInfo);
            bool isError = abInfo.state != ABInfoState.Success;
            var mainAbInfo = ManifestManager.GetABInfo (abInfo.abName);
            if (mainAbInfo != null) mainAbInfo.state = abInfo.state;
#if !HUGULA_NO_LOG
            Debug.LogFormat("task complete abName={0},size={1},isError={2},loadingTasks.Count={3},bQueue.count={4}", abInfo.abName, abInfo.size, isError,loadingTasks.Count, bQueue.Count);
#endif
            bQueue.Complete (abInfo, isError);

            if(!bQueue.IsError)
                LoadingQueue ();
        }

        internal void RunningTask (ABInfo abInfo, BackGroundQueue bQueue) {
            var userData = new object[] { abInfo, bQueue, };

            if (ManifestManager.CheckPersistentCrc (abInfo)) //验证crc
            {
                webClients.Add (userData); // completa
                loadingTasks[abInfo] = abInfo;
#if !HUGULA_NO_LOG
                Debug.LogFormat ("RunningTask abName={0},Persistent is down frame={1}", abInfo.abName,Time.frameCount);
#endif               
                return;
            } else {
                if(abInfo.state == ABInfoState.Fail)
                    FileHelper.DeletePersistentFile(abInfo.abName);
                var download = WebDownload.Get ();
                download.userData = userData;
                download.DownloadFileCompleted = OnDownloadFileCompleted;
                download.DownloadProgressChanged = OnDownloadProgressChanged;
                loadingTasks[abInfo] = download;
                string urlHost = hosts[0];
                RealLoad (download, abInfo, bQueue, urlHost);
            }
        }

        void ReleaseWebDonwLoad (WebDownload webd, ABInfo aBInfo) {
            lock (syncRoot) {
                WebDownload.Release (webd);
            }
        }

        internal void RealLoad (WebDownload download, ABInfo abInfo, BackGroundQueue bQueue, string urlHost, string timestamp = "") {
            string abName = abInfo.abName;
            bool appCrc = HugulaSetting.instance != null ? HugulaSetting.instance.appendCrcToFile : false;
            if (abInfo.crc32 > 0 && appCrc) {
                abName = CUtils.InsertAssetBundleName (abName, "_" + abInfo.crc32.ToString ());
            }

            Uri url = null;
            if (string.IsNullOrEmpty (timestamp))
                url = new Uri (CUtils.PathCombine (urlHost, abName));
            else
                url = new Uri (CUtils.PathCombine (urlHost, abName + "?" + timestamp));
            string path = GetTmpFilePath (abInfo);
            FileHelper.CheckCreateFilePathDirectory (path);
            if (abInfo.size < BreakPointLength)
                download.DownloadFileAsync (url, path);
            else
                download.DownloadFileMultiAsync (url, path);

#if !HUGULA_NO_LOG
            Debug.LogFormat (" begin load {0} ,save path ={1},abInfo.state={2} ,webClient({3})", url.AbsoluteUri, path, abInfo.state, download);
#endif
        }

        string GetTmpFilePath (ABInfo abInfo) {
            string path = CUtils.PathCombine (outputPath, abInfo.abName);
            return path;
        }

        void OnDownloadFileCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            WebDownload webd = (WebDownload) sender;
            object[] arr = (object[]) webd.userData;
            ABInfo abInfo = null;
            BackGroundQueue bQueue = null;
            webd.tryTimes++;
            int tryTimes = webd.tryTimes;

            if (arr != null && arr.Length >= 2) {
                abInfo = ((ABInfo) arr[0]);
                bQueue = (BackGroundQueue) arr[1];
            }

#if !HUGULA_NO_LOG
            // Debug.LogFormat ("background ab:{0}\r\n is completed,error:{1}", abInfo.abName, e.Error==null? string.Empty : e.Error.Message);
#endif
            if (e.Error != null) {
                if (tryTimes <= hosts.Length * 3) {
                    int i = tryTimes % hosts.Length;
                    Debug.LogWarning (string.Format ("background download error ab:{0}, tryTimes={1},host={2},error:{3}", abInfo.abName, webd.tryTimes, hosts[i], e.Error));
                    RealLoad (webd, abInfo, bQueue, hosts[i], tryTimes.ToString ());
                    return;
                } else {
                    Debug.LogErrorFormat ("background download error message {0} \r\n trace {1}", e.Error.Message, e.Error.StackTrace);
                    abInfo.state = ABInfoState.None; // none or fail?
                    webClients.Add (arr);
                    ReleaseWebDonwLoad (webd, abInfo);
                }
            } else {
                string path = GetTmpFilePath (abInfo);
                FileInfo tmpFile = new FileInfo (path);
                if (tmpFile.Length == abInfo.size) //check size
                {
                    abInfo.state = ABInfoState.Success;
                    webClients.Add (arr);
                    ReleaseWebDonwLoad (webd, abInfo);
                } else if (tryTimes <= hosts.Length * 3) {
                    int i = tryTimes % hosts.Length;
                    string error = string.Format ("background complete length check is wrong ab:{0} ,(length:{1}!=size:{2}) crc={3},tryTimes{4},host:{5}", abInfo.abName, tmpFile.Length, abInfo.size, tryTimes, abInfo.crc32, hosts[i]);
                    Debug.LogWarning (error);
                    tmpFile.Delete (); //删除错误文件
                    RealLoad (webd, abInfo, bQueue, hosts[i], tryTimes.ToString ());
                    return;
                } else {
                    string error = string.Format ("background complete length check is wrong tryMaxTimes:{4} .ab:{0} (length:{1}!=size:{2}) crc={3},host:{5}", abInfo.abName, tmpFile.Length, abInfo.size, abInfo.crc32, tryTimes, hosts[tryTimes % hosts.Length]);
                    Debug.LogWarning (error);
                    abInfo.state = ABInfoState.None; // none or fail?
                    webClients.Add (arr);
                    ReleaseWebDonwLoad (webd, abInfo);
                }
            }

        }

        void OnDownloadProgressChanged (object sender, DownloadingProgressChangedEventArgs e) {
            CalcReceiveBytes (e.BytesRead);
            // WebDownload webd = (WebDownload) sender;
            // object[] arr = (object[]) webd.userData;
            // ABInfo abInfo = ((ABInfo) arr[0]);;
            // BackGroundQueue bQueue = (BackGroundQueue) arr[1];;
            // bQueue.Progress (abInfo, e.ProgressPercentage);

            if (progressChangedEventArgs != null)
                progressChangedEventArgs.received += e.BytesReceived;

            // Debug.LogFormat ("background ab:{0}\r\n OnDownloadProgressChanged:{1}", ((ABInfo) ((object[]) webd.userData) [0]).abName, e.ProgressPercentage);

        }

        /// <summary>
        /// suspend download
        /// </summary>
        public void Suspend () {
            enabled = false;
            this.loadingCount = 0;
        }

        /// <summary>
        /// begin download
        /// </summary>
        public void ReloadError()
        {
            lock (syncRoot) {
                LinkedListNode<BackGroundQueue> fristNode = this.loadQueue.First;
                while (fristNode != null) {
                    BackGroundQueue value = fristNode.Value;
                    value.ReLoadError();
                    #if !HUGULA_NO_LOG
                    Debug.LogFormat("ReLoadError  BackGroundQueue.Count = {0} ",value.Count);
                    #endif
                    fristNode = fristNode.Next;
                }
            }
        }

        /// <summary>
        /// begin download
        /// </summary>
        public void Begin () {
            if (!enabled) enabled = true;
            // CheckLoadingCount ();
            ReSetReceiveBytes ();
            LoadingQueue ();
        }

        private static void ReSetReceiveBytes () {
            lastRecordTime = 0;
            // totalReceived = 0;
        }
        private static void CalcReceiveBytes (int num) {
            if (lastRecordTime == 0) {
                lastRecordTime = System.DateTime.Now.Ticks;
                totalReceived = num;
            } else {
                totalReceived += num;
                var ts = new TimeSpan (System.DateTime.Now.Ticks - lastRecordTime);
                if (ts.TotalSeconds >= 1) {
                    BytesReceivedPerSecond = (int) (totalReceived / ts.TotalSeconds);
                    ReSetReceiveBytes ();
                }
            }
        }

        #region static

        /// <summary>
        /// Releases all resource used by the <see cref="Hugula.Update.BackGroundDownload"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Hugula.Update.BackGroundDownload"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Hugula.Update.BackGroundDownload"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the <see cref="Hugula.Update.Dispose"/> so the garbage
        /// collector can reclaim the memory that the <see cref="Hugula.Update.BackGroundDownload"/> was occupying.</remarks>
        public static void Dispose () {
            if (_instance != null)
                GameObject.Destroy (_instance.gameObject);
            _instance = null;
        }

        static BackGroundDownload _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static BackGroundDownload instance {
            get {
                if (_instance == null) {
                    var gameObj = new GameObject ("BackGroundDownload");
                    DontDestroyOnLoad (gameObj);
                    _instance = gameObj.AddComponent<BackGroundDownload> ();
                    _instance.Suspend ();
                }
                return _instance;
            }
        }

        #endregion

        public class ABInfoComparer : IComparer<ABInfo> {
            public int Compare (ABInfo a, ABInfo b) {
                if (a.priority < b.priority) return 1;
                if (a.priority > b.priority) return -1;
                return 0;
            }
        }
    }
}