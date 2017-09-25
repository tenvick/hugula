using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Hugula.Loader;
using Hugula.Utils;
using System;
using System.IO;
using Hugula.Collections;

namespace Hugula.Update
{

    [SLua.CustomLuaClassAttribute]
    public class BackGroundDownload : MonoBehaviour
    {
        private const string _keyCarrierDataNetwork = "_keyCarrierDataNetwork";
        public string[] hosts;

        // public string host1 = string.Empty;

        public int loadingCount = 0;
        //开启下载
        public bool alwaysDownLoad
        {
            get
            {
                return m_alwaysDownLoad;
            }
            set
            {
                m_alwaysDownLoad = value;
            }
        }

        private static int _carrierDataNetwork = -1;
        public static bool carrierDataNetwork
        {
            get
            {
                if (_carrierDataNetwork == -1)
                    _carrierDataNetwork = PlayerPrefs.GetInt(_keyCarrierDataNetwork, 0);
                return _carrierDataNetwork == 1;
            }
            set
            {
                _carrierDataNetwork = value ? 1 : 0;
                PlayerPrefs.SetInt("_keyCarrierDataNetwork", _carrierDataNetwork);
            }
        }

        public System.Action<BackGroundDownload> onNetSateChange;
#if UNITY_EDITOR
        [SLua.DoNotToLuaAttribute]
        public int currentLoadingCount = 0;
        [SLua.DoNotToLuaAttribute]
        public string loadingList;
#endif
        private bool m_alwaysDownLoad = false;
        private NetworkReachability netState;
        private Dictionary<int, BackGroundQueue> loadQueueDic = new Dictionary<int, BackGroundQueue>();
        private LinkedList<BackGroundQueue> loadQueue = new LinkedList<BackGroundQueue>();
        private SafeDictionary<ABInfo, WebDownload> loadingTasks = new SafeDictionary<ABInfo, WebDownload>();

        private ABInfoComparer abInfoCompare = new ABInfoComparer();

        ArrayList webClients = ArrayList.Synchronized(new ArrayList());
        string outputPath;
        void Awake()
        {
            outputPath = CUtils.GetRealPersistentDataPath() + "/";
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            netState = Application.internetReachability;
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            CheckLoadingCount();
            if (this.loadingCount == 0 && onNetSateChange != null) onNetSateChange(this);
        }
        // Update is called once per frame
        void Update()
        {
            if (netState != Application.internetReachability)
            {
                netState = Application.internetReachability;
                CheckLoadingCount();
                if (onNetSateChange != null) onNetSateChange(this);
            }

            while (webClients.Count > 0)
            {
                var download = (WebDownload)webClients[0];
                var arr = (object[])download.userData;
                webClients.RemoveAt(0);
                bool isError = !string.IsNullOrEmpty(download.error);
                WebDownload.Release(download);
                RemoveTask((ABInfo)arr[0], (BackGroundQueue)arr[1], isError);
            }

            LoadingQueue();

#if UNITY_EDITOR
            currentLoadingCount = loadingTasks.Count;
            var str = string.Empty;
            foreach (var abInfo in loadingTasks)
            {
                str += "\r\n" + abInfo.Key.abName;
            }

            loadingList = str;
#endif
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

            loadingTasks.Clear();
            loadQueue.Clear();
            loadQueueDic.Clear();
        }


        void LoadingQueue()
        {

            LinkedListNode<BackGroundQueue> fristNode = this.loadQueue.First;

            for (int i = 0; i < this.loadingCount - this.loadingTasks.Count; i++)
            {
                while (fristNode != null)
                {
                    BackGroundQueue value = fristNode.Value;
                    if (value.Count > 0)
                    {
                        var abInfo = value.Dequeue();
                        if (!loadingTasks.ContainsKey(abInfo))
                        {
                            RunningTask(abInfo, value);
                            break;
                        }

                    }
                    fristNode = fristNode.Next;
                    this.loadQueue.Remove(value);
                }
                if (fristNode == null)
                {
                    break;
                }
            }
        }

        private BackGroundQueue GetLoadQueue(int priority)
        {
            // priority
            BackGroundQueue bQueue = null;
            if (loadQueueDic.ContainsKey(priority))
            {
                bQueue = this.loadQueueDic[priority];
            }
            else
            {
                bQueue = new BackGroundQueue(priority);
                this.loadQueueDic[priority] = bQueue;
            }

            bool flag = false;
            for (LinkedListNode<BackGroundQueue> fristNode = this.loadQueue.First; fristNode != null; fristNode = fristNode.Next)
            {
                if (fristNode.Value.priority == priority)
                {
                    this.loadQueue.AddAfter(fristNode, bQueue); //Debug.LogFormat("  fristNode.Value.priority={0}<priority{1}", fristNode.Value.priority, priority);
                    flag = true;
                    break;
                }
                if (fristNode.Value.priority < priority)
                {
                    this.loadQueue.AddBefore(fristNode, bQueue);// Debug.LogFormat("  fristNode.Value.priority={0}<priority{1}", fristNode.Value.priority, priority);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.loadQueue.AddLast(bQueue);
            }

            return bQueue;
        }

        [SLua.DoNotToLuaAttribute]
        public uint AddTask(ABInfo abinfo, int priority, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            if (abinfo == null) return 0;
            var bQueue = GetLoadQueue(priority);
            bQueue.onProgress = onProgress;
            bQueue.onComplete = onComplete;
            bQueue.Enqueue(abinfo);
#if UNITY_EDITOR
            Debug.LogFormat("bQueue.Count = {0},priority={1}", bQueue.Count, priority);
#endif
            return abinfo.size;
        }

        [SLua.DoNotToLuaAttribute]
        public uint AddTask(List<ABInfo> abInfos, int priority, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            uint size = 0;

            // Debug.LogFormat("AddTask.Count = {0},priority={1}", abInfos.Count, priority);
            if (abInfos.Count == 0)
            {
                return size;
            }

            var bQueue = GetLoadQueue(priority);
            bQueue.onProgress = onProgress;
            bQueue.onComplete = onComplete;

            bQueue.Enqueue(abInfos);

            foreach (var abInfo in abInfos)
                size += abInfo.size;
            // Debug.LogFormat("bQueue.Count = {0},priority={1}", bQueue.Count, priority);
            return size;
        }

        public uint AddFirstManifestTask(FileManifest mainifest1, FileManifest mainifest2, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            if (mainifest1 == null) return 0;

            List<ABInfo> re = new List<ABInfo>();

            var localABInfos = mainifest1.allAbInfo;
            var diffABInfos = mainifest1.CompareFileManifest(mainifest2); //mainifest2.appNumVersion - mainifest1.appNumVersion >= 0;
            ABInfo abInfo;
            for (int i = 0; i < localABInfos.Count; i++)
            {
                abInfo = localABInfos[i];
                if (abInfo.priority >= FileManifestOptions.FirstLoadPriority
               && abInfo.priority < FileManifestOptions.AutoHotPriority) //首包下载内容
                {
                    re.Add(abInfo);
                }
            }

            for (int i = 0; i < diffABInfos.Count; i++)
            {
                abInfo = diffABInfos[i];
                re.Add(abInfo);
            }

            re.Sort(abInfoCompare);

            return AddTask(re, FileManifestOptions.ManualPriority, onProgress, onComplete);
        }


        public uint AddDiffManifestTask(FileManifest mainifest1, FileManifest mainifest2, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            if (mainifest1 == null) return 0;
            var splite = Hugula.HugulaSetting.instance.spliteExtensionFolder;
            var loadInfos = mainifest1.CompareFileManifest(mainifest2);
            List<ABInfo> re = new List<ABInfo>();
            foreach (var abInfo in loadInfos)
            {
                if (!splite)
                {
                    re.Add(abInfo);
                }
                else if (abInfo.priority <= FileManifestOptions.AutoHotPriority) //非自动下载级别 
                {
                    re.Add(abInfo);
                }
                else if (FileHelper.PersistentFileExists(abInfo.abName))//存在表示已经下载过但是版本不对
                {
                    re.Add(abInfo);
                }
            }

            re.Sort(abInfoCompare);

            return AddTask(re, FileManifestOptions.ManualPriority, onProgress, onComplete);
        }

        public uint AddBackgroundManifestTask(FileManifest mainifest1, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            if (mainifest1 == null) return 0;

            var allAbInfos = mainifest1.allAbInfo;
            List<ABInfo> loadInfos = new List<ABInfo>();
            ABInfo abInfo;
            for (int i = 0; i < allAbInfos.Count; i++)
            {
                abInfo = allAbInfos[i];
                if (abInfo.priority < FileManifestOptions.ManualPriority
                && abInfo.priority >= FileManifestOptions.AutoHotPriority
                && !FileHelper.PersistentFileExists(abInfo.abName))
                {
                    loadInfos.Add(abInfo);
                }
            }
            loadInfos.Sort(abInfoCompare);
            return AddTask(loadInfos, FileManifestOptions.StreamingAssetsPriority, onProgress, onComplete);
        }

        public uint AddManualManifestTask(FileManifest mainifest1, string folder, System.Action<LoadingEventArg> onProgress, System.Action<bool> onComplete)
        {
            if (mainifest1 == null) return 0;

            var allAbInfos = mainifest1.allAbInfo;
            List<ABInfo> loadInfos = new List<ABInfo>();
            ABInfo abInfo;
            for (int i = 0; i < allAbInfos.Count; i++)
            {
                abInfo = allAbInfos[i];
                if (abInfo.abName.Contains(folder) && !FileHelper.PersistentFileExists(abInfo.abName))// && abInfo.priority > FileManifestOptions.AutoHotPriority)
                {
                    loadInfos.Add(abInfo);
                }
            }

            loadInfos.Sort(abInfoCompare);

            return AddTask(loadInfos, FileManifestOptions.ManualPriority, onProgress, onComplete);
        }

        internal void RemoveTask(ABInfo abInfo, BackGroundQueue bQueue, bool isError)
        {
            if (isError)
                abInfo.state = ABInfoState.Fail;
            else
                abInfo.state = ABInfoState.Success;

            var mainAbInfo = ManifestManager.GetABInfo(abInfo.abName);
            if (mainAbInfo != null) mainAbInfo.state = abInfo.state;
            this.loadingTasks.Remove(abInfo);
            // Debug.LogFormat("task complete abName={0},size={1},loadingTasks.Count={2},bQueue.count={3}", abInfo.abName, abInfo.size, loadingTasks.Count, bQueue.Count);
            bQueue.Complete(abInfo, isError);
        }

        internal void RunningTask(ABInfo abInfo, BackGroundQueue bQueue)
        {
            var download = WebDownload.Get();
            download.userData = new object[] { abInfo, bQueue, };
            download.DownloadFileCompleted -= OnDownloadFileCompleted;
            download.DownloadProgressChanged -= OnDownloadProgressChanged;
            download.DownloadFileCompleted += OnDownloadFileCompleted;
            download.DownloadProgressChanged += OnDownloadProgressChanged;
            loadingTasks[abInfo] = download;

            if (ManifestManager.CheckPersistentCrc(abInfo)) //验证crc
            {
                webClients.Add(download);
                return;
            }
            else
            {
                string urlHost = hosts[0];
                RealLoad(download, abInfo, bQueue, urlHost);
            }
        }

        internal void RealLoad(WebDownload download, ABInfo abInfo, BackGroundQueue bQueue, string urlHost = "")
        {
            string abName = abInfo.abName;
            bool appCrc = HugulaSetting.instance != null ? HugulaSetting.instance.appendCrcToFile : false;
            if (abInfo.crc32 > 0 && appCrc)
            {
                abName = CUtils.InsertAssetBundleName(abName, "_" + abInfo.crc32.ToString());
            }

            Uri url = new Uri(CUtils.PathCombine(urlHost, abName));
            string path = GetTmpFilePath(abInfo);
            FileHelper.CheckCreateFilePathDirectory(path);
            download.DownloadFileAsync(url, path);
#if !HUGULA_RELEASE
            Debug.LogFormat(" background load {0} ,save path ={1},abInfo.state={2} ,webClient({3})", url.AbsoluteUri, path, abInfo.state, download);
#endif
        }

        string GetTmpFilePath(ABInfo abInfo)
        {
            string path = CUtils.PathCombine(outputPath, abInfo.abName + ".tmp");
            return path;
        }

        void OnDownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            WebDownload webd = (WebDownload)sender;
            object[] arr = (object[])webd.userData;
            ABInfo abInfo = null;
            BackGroundQueue bQueue = null;
            webd.tryTimes++;
            int tryTimes = webd.tryTimes;

            if (arr != null && arr.Length >= 2)
            {
                abInfo = ((ABInfo)arr[0]);
                bQueue = (BackGroundQueue)arr[1];
            }

            webd.error = e.Error == null ? string.Empty : e.Error.Message;
            if (!string.IsNullOrEmpty(webd.error))
            {
                FileHelper.DeletePersistentFile(abInfo.abName + ".tmp");
                if (tryTimes < hosts.Length)
                {
                    Debug.Log(string.Format("web download url:{0}\r\n times={1},error:{2}\r\n", hosts[tryTimes] + "/" + abInfo.abName, webd.tryTimes, e.Error));
                    RealLoad(webd, abInfo, bQueue, hosts[tryTimes]);
                    return;
                    //
                }
                else
                    Debug.Log(string.Format("web download url:{0}\r\n error:{1}", hosts[hosts.Length - 1] + "/" + abInfo.abName, e.Error));
            }
            else
            {
                string path = GetTmpFilePath(abInfo);
                FileInfo tmpFile = new FileInfo(path);
                if (tmpFile.Length == abInfo.size) //check size
                {
                    abInfo.state = ABInfoState.Success;
                    string realPath = CUtils.PathCombine(outputPath, abInfo.abName);
                    if (File.Exists(realPath)) File.Delete(realPath);
                    tmpFile.MoveTo(realPath);//rename
                }
                else if (tryTimes < hosts.Length)
                {
                    FileHelper.DeletePersistentFile(abInfo.abName + ".tmp");
                    RealLoad(webd, abInfo, bQueue, hosts[tryTimes]);
                    return;
                }
                else
                {
                    webd.error = string.Format("background ab:{0} crc check is wrong", hosts[hosts.Length - 1] + "/" + abInfo.abName);
                    Debug.Log(webd.error);
                }
            }

#if !HUGULA_RELEASE
            Debug.LogFormat("background ab:{0}\r\n is done", ((ABInfo)((object[])webd.userData)[0]).abName);
#endif
            webClients.Add(webd);

        }

        void OnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            WebDownload webd = (WebDownload)sender;
            // Debug.LogFormat("abName={0},BytesReceived={1},ProgressPercentage={2}", ((ABInfo)((object[])webd.userData)[1]).abName, e.BytesReceived, e.ProgressPercentage);
            // object[] arr = (object[]) webd.userData;
            // if(arr!=null)
            // {
            //     ((BackGroundQueue)arr[1]).UpdatePrpgress((ABInfo)arr[1],e.ProgressPercentage);
            // }

        }

        /// <summary>
        /// suspend download
        /// </summary>
        public void Suspend()
        {
            enabled = false;
            this.loadingCount = 0;
        }

        /// <summary>
        /// begin download
        /// </summary>
        public void Begin()
        {
            if (!enabled) enabled = true;
            CheckLoadingCount();
        }

        private void CheckLoadingCount()
        {
            switch (netState)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    this.loadingCount = 3;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    if (carrierDataNetwork || m_alwaysDownLoad)
                        this.loadingCount = 2;
                    else
                        this.loadingCount = 0;
                    break;
                case NetworkReachability.NotReachable:
                    this.loadingCount = 0;
                    break;
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
        public static void Dispose()
        {
            if (_instance != null)
                GameObject.Destroy(_instance.gameObject);
            _instance = null;
        }

        static BackGroundDownload _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static BackGroundDownload instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObj = new GameObject("BackGroundDownload");
                    DontDestroyOnLoad(gameObj);
                    _instance = gameObj.AddComponent<BackGroundDownload>();
                    _instance.Suspend();
                }
                return _instance;
            }
        }


        #endregion

        public class ABInfoComparer : IComparer<ABInfo>
        {
            public int Compare(ABInfo a, ABInfo b)
            {
                if (a.priority < b.priority) return 1;
                if (a.priority > b.priority) return -1;
                return 0;
            }
        }
    }
}