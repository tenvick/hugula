// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
using System.Collections.Generic;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Loader {

    /// <summary>
    /// 新的资源加载类
    /// </summary>
    [SLua.CustomLuaClass]
    public abstract class CResLoader : MonoBehaviour {
#if UNITY_EDITOR    
        [SLua.DoNotToLuaAttribute]
        public int loadingAssetBundleQueueCount;
        [SLua.DoNotToLuaAttribute]
        public int loadingAssetQueueCount;
        [SLua.DoNotToLuaAttribute]
        public string DebugInfo;

        const string kSimulateAssetBundles = "SimulateAssetBundles";
        static int m_SimulateAssetBundleInEditor = -1;
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        [SLua.DoNotToLuaAttribute]
        public static bool SimulateAssetBundleInEditor {
            get {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool (kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set {
                if (value) PLua.isDebug = true;
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor) {
                    m_SimulateAssetBundleInEditor = newValue;
                    UnityEditor.EditorPrefs.SetBool (kSimulateAssetBundles, value);
                }
            }
        }

#endif
        #region public static
        static int _maxLoading = 3;
        /// <summary>
        /// 最大同时加载数量
        /// </summary>
        public static int maxLoading {
            get { return _maxLoading; }
            set {
                _maxLoading = value;
                CreateFreeLoader ();
            }
        }

        /// <summary>
        /// 即将加载的总数
        /// </summary>
        public static int totalLoading { private set; get; }

        /// <summary>
        /// 当前同事正在加载数量
        /// </summary>
        public static int currentLoading {
            get {
                int count = 0;
                for (int i = 0; i < loaderPool.Count; i++) {
                    if (!loaderPool[i].isFree)
                        count++;
                }
                //				count += freeLoader.Count;
                return count;
            }
        }

        /// <summary>
        /// 当前加载完成的数量
        /// </summary>
        public static int currentLoaded;

        static UriGroup _uriList;
        /// <summary>
        /// The URI list.
        /// </summary>
        public static UriGroup uriList {
            get {
                if (_uriList == null) {
                    _uriList = new UriGroup ();
                    _uriList.Add (CUtils.GetRealPersistentDataPath (), true);
                    _uriList.Add (CUtils.GetRealStreamingAssetsPath ());
                }
                return _uriList;
            }
            set {
                _uriList = value;
            }
        }

        #endregion

        #region static member
        /// <summary>
        /// 所有请求队列
        /// </summary>
        static protected PriorityQueue<CRequest> queue = new PriorityQueue<CRequest> (new CRequestComparer ());

        /// <summary>
        /// 异步ab加载队列
        /// </summary>
        static protected List<CRequest> loadingAssetBundleQueue = new List<CRequest> ();

        ///<summary>
        ///异步asset加载队列
        ///</summary>
        static protected List<CRequest> loadingAssetQueue = new List<CRequest> ();

        /// <summary>
        /// 正在下载Loader
        /// </summary>
        static Dictionary<string, CCar> downloadings = new Dictionary<string, CCar> ();

        /// <summary>
        /// 即将开始下载的队列
        /// </summary>
        static Queue<CRequest> realyLoadingQueue = new Queue<CRequest> ();

        /// <summary>
        /// 回调列表
        /// </summary>
        static Dictionary<string, List<CRequest>> requestCallBackList = new Dictionary<string, List<CRequest>> ();

        /// <summary>
        /// asset回调列表
        /// </summary>
        static Dictionary<string, List<CRequest>> assetCallBackList = new Dictionary<string, List<CRequest>> ();

        /// <summary>
        /// 加载缓存池
        /// </summary>
        static List<CCar> loaderPool = new List<CCar> ();

        /// <summary>
        /// 非队列加载
        /// </summary>
        static List<CCar> freeLoader = new List<CCar> ();

        /// <summary>
        /// 加载完成解压回调队列
        /// </summary>
        static Queue<CRequest> loadedAssetQueue = new Queue<CRequest> ();

        /// <summary>
        /// 组回调
        /// </summary>
        static Dictionary<CRequest, GroupRequestRecord> currGroupRequestsRef = new Dictionary<CRequest, GroupRequestRecord> ();

        /// <summary>
        /// All group request record.
        /// </summary>
        static List<GroupRequestRecord> allGroupRequestRecord = new List<GroupRequestRecord> ();
        #endregion

        #region mono

        protected LoadingEventArg loadingEvent;

        void Awake () {
            DontDestroyOnLoad (this.gameObject);
            loadingEvent = new LoadingEventArg ();
            CreateFreeLoader ();
        }

        // Update is called once per frame
        void Update () {
            for (int i = 0; i < freeLoader.Count;) {
                CCar load = freeLoader[i];
                if (load.enabled) load.Update ();

                if (load.isFree) {
                    freeLoader.RemoveAt (i);
                    ReleaseCCar (load); //放回对象池
                } else
                    i++;
            }

            //1 load assetbunlde
            for (int i = 0; i < loaderPool.Count; i++) {
                CCar load = loaderPool[i];
                if (load.isFree && realyLoadingQueue.Count > 0) {
                    var req = realyLoadingQueue.Dequeue ();
                    if (CheckLoadedAssetBundle (req)) {
                        CheckLoadAssetAsync (req);
                    } else if (CheckLoadingAssetBundle (req)) {
                        AddReqToQueue (req);
                    } else {
                        downloadings[req.udKey] = load;
                        load.BeginLoad (req);
                    }
                }
                if (load.enabled) {
                    load.Update ();
                }
            }
#if UNITY_EDITOR
            loadingAssetBundleQueueCount = loadingAssetBundleQueue.Count;
            loadingAssetQueueCount = loadingAssetQueue.Count;
            DebugInfo = "";
#endif
            //2 wait dependencies set asset 
            for (int i = 0; i < loadingAssetBundleQueue.Count;) {
                var item = loadingAssetBundleQueue[i];
                if (CacheManager.CheckDependenciesComplete (item)) { //判断依赖项目是否加载完成
                    if (CacheManager.SetRequestDataFromCache (item)) //设置缓存数据。
                        loadingAssetQueue.Add (item); //加载资源
                    else {
                        loadedAssetQueue.Enqueue (item);
#if HUGULA_LOADER_DEBUG
                        Debug.LogFormat ("<color=red> 2.2 SetRequestDataFromCache false Req(assetname={0},url={1}) frame={2}loadingAssetBundleQueue.Count={3} </color>", item.assetName, item.url, Time.frameCount, loadingAssetBundleQueue.Count);
#endif
                    }

                    loadingAssetBundleQueue.RemoveAt (i);
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" 2.3 <color=#15C132>DependenciesComplete Req(assetname={0},url={1},async={2}) frameCount{3},loadingAssetBundleQueue.Count={4}</color>", item.assetName, item.url, item.async, Time.frameCount, loadingAssetBundleQueue.Count);
#endif
                } else {
                    i++;
#if UNITY_EDITOR
                    DebugInfo += string.Format (" -2.3CheckDependenciesComplete Req(assetname={0},url={1}) frameCount{2},loadingAssetBundleQueue.Count={3}</color>", item.assetName, item.url, Time.frameCount, loadingAssetBundleQueue.Count);
#endif
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" -2.3 <color=#15C132>CheckDependenciesComplete Req(assetname={0},url={1}) frameCount{2},loadingAssetBundleQueue.Count={3}</color>", item.assetName, item.url, Time.frameCount, loadingAssetBundleQueue.Count);
#endif
                }
            }

            //3 load asset
            for (int i = 0; i < loadingAssetQueue.Count; i++) {
                var item = loadingAssetQueue[i];
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat (" 3.2.-1 <color=#A9C115> Req(assetname={0},url={1}) loadingAssetQueue.Count={2} frameCount{3} </color>", item.assetName, item.url, loadingAssetQueue.Count, Time.frameCount);
#endif
                if (item.assetBundleRequest != null && item.assetBundleRequest.isDone) //如果加载完成
                {
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" 3.2.0 <color=#A9C115>set Req(assetname={0},url={1}).data asnyc Count{2} frameCount{3} </color>", item.assetName, item.url, loadingAssetQueue.Count, Time.frameCount);
#endif
                    if (item.assetBundleRequest is AssetBundleRequest) {
                        if (CacheManager.Typeof_ABAllAssets.Equals (item.assetType))
                            item.data = ((AssetBundleRequest) item.assetBundleRequest).allAssets; //赋值
                        else
                            item.data = ((AssetBundleRequest) item.assetBundleRequest).asset; //赋值

                    } else
                        item.data = item.assetBundleRequest;

#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" 3.2.1 <color=#A9C115>set Req(assetname={0},url={1} data{2}) end </color>", item.assetName, item.url, item.data);
#endif
                    loadedAssetQueue.Enqueue (item);
                    loadingAssetQueue.RemoveAt (i);
                } else if (item.assetBundleRequest == null) //非异步
                {
                    loadedAssetQueue.Enqueue (item);
                    loadingAssetQueue.RemoveAt (i);

#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" 3.2 <color=#A9C115>set Req(assetname={0},url={1}).data Async  Count{2} frameCount{3}</color>", item.assetName, item.url, loadingAssetQueue.Count, Time.frameCount);
#endif
                } else
                    i++;
            }

            //4 complete asset
            while (loadedAssetQueue.Count > 0) {
                LoadAssetComplate (loadedAssetQueue.Dequeue ());
            }

            //group check
            for (int i = 0; i < allGroupRequestRecord.Count;) {
                var group = allGroupRequestRecord[i];
                group.Progress ();
                if (group.Count > 0) {
                    i++;
                } else {
                    var act = group.onGroupComplate;
                    GroupRequestRecord.Release (group);
                    allGroupRequestRecord.RemoveAt (i);
                    if (act != null) {
                        act (group);
                    }

                }
            }
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate () {
            ABDelayUnloadManager.Update ();
        }

        #endregion

        #region static load

        /// <summary>
        /// 将多个资源加载到本地并缓存。
        /// </summary>
        /// <param name="req"></param>
        public void LoadReq (IList<CRequest> req, System.Action<object> onGroup, System.Action<LoadingEventArg> onProgress) //onAllCompleteHandle onAllCompletehandle=null,onProgressHandle onProgresshandle=null
        {
            GroupRequestRecord groupFn = null;
            if (onGroup != null) {
                groupFn = GroupRequestRecord.Get ();
                groupFn.onGroupComplate = onGroup;
                groupFn.onGroupProgress = onProgress;
            }
            for (int i = 0; i < req.Count; i++)
                AddReqToQueue (req[i], groupFn);

            BeginQueue ();
        }

        /// <summary>
        /// 将多个资源加载到本地并缓存。
        /// </summary>
        /// <param name="req"></param>
        public void LoadReq (CRequest req) {
            AddReqToQueue (req);
            BeginQueue ();
        }

        /// <summary>
        /// Loads the req.
        /// </summary>
        /// <param name="req">Req.</param>
        /// <param name="group">Group.</param>
        public void LoadReq (CRequest req, GroupRequestRecord group) {
            AddReqToQueue (req, group);
            BeginQueue ();
        }

        /// <summary>
        /// append request to  assetCallBackList
        /// </summary>
        /// <param name="req">Req.</param>
        protected static void AddReqToAssetCallBackList (CRequest req) {
            string key = req.udAssetKey;
            List<CRequest> list = null;
            if (assetCallBackList.TryGetValue (key, out list)) //回调列表
            {
                list.Add (req);
            } else {
                list = ListPool<CRequest>.Get ();
                assetCallBackList.Add (key, list);
                loadingAssetBundleQueue.Add (req);
            }

        }

        /// <summary>
        /// append request to queue.
        /// </summary>
        /// <param name="req">Req.</param>
        /// <param name="group">Group.</param>
        protected static bool AddReqToQueue (CRequest req, GroupRequestRecord group = null) {
            if (req == null) return false;
            string key = req.udKey; //the udkey never change 第一个URI和relativeUrl评级而成。

            UriGroup.CheckRequestUrlIsAssetbundle (req);

            if (!req.isShared && group != null) {
                PushGroup (req, group);
            }

            if (CheckLoadAssetAsync (req)) //已经下载
            {
                return false;
#if UNITY_EDITOR
            } else if (SimulateAssetBundleInEditor && req.isAssetBundle) //
            {
                CallbackError (req);
                return false;
#endif
            } else if (!UriGroup.CheckRequestCurrentIndexCrc (req)) //如果校验失败
            {
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat (" 0.0 <color=#ff0000>CheckCrcUri0Exists==false Req(assetname={0},url={1})  </color>", req.assetName, req.url);
#endif
                CallbackError (req);
                return false;
            }

            List<CRequest> list = null;
            if (requestCallBackList.TryGetValue (key, out list)) //.ContainsKey(key)) //回调列表
            {
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat (" 0.1 <color=#15A0A1>requestCallBackList.ContainsKey Req(assetname={0},url={1})  </color>", req.assetName, req.url);
#endif
                list.Add (req);
                return true;
            } else {
                var listreqs = ListPool<CRequest>.Get ();
                requestCallBackList.Add (key, listreqs);
                listreqs.Add (req);

                if (queue.Count == 0 && currentLoading == 0 && loadingAssetBundleQueue.Count == 0) {
                    totalLoading = 0;
                    currentLoaded = 0;
                }
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat (" 0.1 <color=#15A0A1>LoadAssetBundle  Req(assetname={0},url={1} isShared={2},isNormal={3})  </color>", req.assetName, req.url, req.isShared, req.isNormal);
#endif

                if (req.isShared) {
                    QueueOrLoad (req); //  LoadAssetBundle(req);// QueueOrLoad (req);//realyLoadingQueue.Enqueue (req);
                } else if (!req.isNormal) {
                    LoadAssetBundle (req);

                } else {
                    queue.Push (req);
                    totalLoading++;
                }

                return true;

            }
        }

        /// <summary>
        /// check load from cache
        /// </summary>
        /// <param name="req"></param>
        static bool CheckLoadAssetAsync (CRequest req) {

#if UNITY_EDITOR
            if(SimulateAssetBundleInEditor && CacheManager.SetRequestDataFromPrefab(req))
            {
                loadingAssetQueue.Add(req);
                return true;
            }
#endif

            ABDelayUnloadManager.CheckRemove (req.keyHashCode);
            if (CacheManager.Contains(req.keyHashCode)) {
                AddReqToAssetCallBackList(req);    
                return true;
            }
            return false;
        }

        /// <summary>
        /// begin  queue .
        /// </summary>
        protected static void BeginQueue () {
            if (queue.Count > 0 && currentLoading < maxLoading) {
                var cur = queue.Pop ();
                LoadAssetBundle (cur);
            }
        }

        /// <summary>
        /// load assetbundle and Dependencies
        /// </summary>
        static protected void LoadAssetBundle (CRequest req) {
            if (assetBundleManifest != null && req.isAssetBundle)
                req.allDependencies = LoadDependencies (req); //加载依赖

            QueueOrLoad (req); //realyLoadingQueue.Enqueue(req);//加载自己

        }

        /// <summary>
        /// 排队或者加载
        /// </summary>
        /// <param name="req">Req.</param>
        static void QueueOrLoad (CRequest req) {
#if HUGULA_LOADER_DEBUG
            Debug.LogFormat ("  0.2 <color=#15A0A1>QueueOrLoad Request(assetName={0}, url={1},isNormal={2},isShared={3})</color>", req.assetName, req.url, req.isNormal, req.isShared);
#endif
            if (req.isNormal) {
                realyLoadingQueue.Enqueue (req);
            } else {
                CCar car = GetCCar ();
                freeLoader.Add (car);
                car.BeginLoad (req);
            }
        }

        /// <summary>
        /// assetbunlde load complete
        /// </summary>
        /// <param name="loader">loader.</param>
        /// <param name="req">Req.</param>
        static void LoadComplete (CCar loader, CRequest creq) {
#if HUGULA_LOADER_DEBUG
            Debug.LogFormat (" 1.3 <color=#15C1B2>CResLoader.LoadComplete Request(assetName={0}, hash={1},isShared={2})</color>", creq.assetName, creq.keyHashCode, creq.isShared);
#endif
            downloadings.Remove (creq.udKey);
            CallbackAsyncList (creq);
            if (!creq.isShared || !creq.isNormal) //共享和非普通都不计数
                currentLoaded++;
            if (!creq.isShared && creq.isAssetBundle && _instance != null && _instance.OnAssetBundleComplete != null)
                _instance.OnAssetBundleComplete (creq);
        }

        /// <summary>
        /// assetbunlde CallbackAsyncList load asset
        /// </summary>
        /// <param name="req">Req.</param>
        protected static void CallbackAsyncList (CRequest creq) {
            List<CRequest> callbacklist = null; // requestCallBackList[creq.udKey];
            string udkey = creq.udKey;

            if (!creq.isShared && creq.isAssetBundle) CacheManager.SetAssetLoaded (creq.keyHashCode);

            if (requestCallBackList.TryGetValue (udkey, out callbacklist)) {
                requestCallBackList.Remove (udkey);
                int count = callbacklist.Count;
                CRequest reqitem;
                for (int i = 0; i < count; i++) {
                    reqitem = callbacklist[i];

                    if (!creq.isAssetBundle) { //如果加载的不是assetbundle
                        reqitem.data = creq.data;
                        loadedAssetQueue.Enqueue (reqitem);
#if HUGULA_LOADER_DEBUG
                        Debug.LogFormat ("2.1.0 <color=#15C132>  Add all to loadedAssetQueue Req(assetname={0},url={1} )frame={2} count={3}  </color>", creq.assetName, creq.url, Time.frameCount, count);
#endif
                    } else { // if (!creq.isShared) { //非共享资源需要回调
                        AddReqToAssetCallBackList (reqitem);
#if HUGULA_LOADER_DEBUG
                        Debug.LogFormat ("2.1.0 <color=#15C132>  Add all to loadingAssetBundleQueue Req(assetname={0},url={1}) frame={2} count = {3} </color>", creq.assetName, creq.url, Time.frameCount, count);
#endif
                    }
                }
                ListPool<CRequest>.Release (callbacklist);

            } else {
                if (!creq.isAssetBundle) //r如果不是assetbundle不需要load asset
                {
                    loadedAssetQueue.Enqueue (creq);
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat ("2.1.1 <color=#15C132> Add one to loadingAssetBundleQueue Req(assetname={0},url={1}) frame={2} </color>", creq.assetName, creq.url, Time.frameCount);
#endif
                } else {
                    AddReqToAssetCallBackList (creq);
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat ("2.1.1 <color=#15C132> Add one to loadingAssetBundleQueue Req(assetname={0},url={1}) frame={2} </color>", creq.assetName, creq.url, Time.frameCount);
#endif
                }

            }
        }

        /// <summary>
        /// Load Asset Complate Dispatch CRequest onComplete Event
        /// </summary>
        /// <param name="req">Req.</param>
        protected static void LoadAssetComplate (CRequest req) {
#if HUGULA_LOADER_DEBUG
            Debug.LogFormat (" 4. <color=yellow>LoadAssetComplate Req(assetname={0},url={1}) frameCount{2}</color>", req.assetName, req.url, Time.frameCount);
#endif

            string key = req.udAssetKey;
            List<CRequest> list = null;
            if (assetCallBackList.TryGetValue (key, out list)) //回调列表
            {
                assetCallBackList.Remove (key);
                int count = list.Count;
                CRequest reqitem = null;
                for (int i = 0; i < count; i++) {
                    reqitem = list[i];
                    reqitem.data = req.data;
                    // Debug.LogFormat("DispatchReqComplete cout={3},data={0} assetName={1} url={2}", req.data, req.assetName, req.url, count);
                    DispatchReqComplete (reqitem);
                }
                ListPool<CRequest>.Release (list);
            }

            DispatchReqComplete (req);
            BeginQueue ();
            CheckAllComplete ();
        }

        /// <summary>
        /// dispatch request complete event
        /// </summary>
        /// <param name="req"></param>
        private static void DispatchReqComplete (CRequest reqitem) {
            if (reqitem.isShared) {
                CacheManager.SetAssetLoaded (reqitem.keyHashCode);
                if (_instance && _instance.OnSharedComplete != null)
                    _instance.OnSharedComplete (reqitem);

                if (reqitem.pool) LRequest.Release (reqitem);
            } else {
                reqitem.DispatchComplete ();
                PopGroup (reqitem);
            }
        }

        /// <summary>
        /// Load AssetBundle Error
        /// </summary>
        /// <param name="req">Req.</param>        
        static void LoadError (CCar cloader, CRequest req) {
            downloadings.Remove (req.udKey);

#if HUGULA_LOADER_DEBUG
            Debug.LogFormat (" 1.3 <color=red>CResLoader.LoadError Request(assetName={0}, url={1},isShared={2} req.index={3},req.uris.count={4})</color>", req.assetName, req.url, req.isShared, req.index, req.uris == null ? 0 : req.uris.count);
#endif
            if (req.uris != null && req.index < req.uris.count - 1 && UriGroup.CheckAndSetNextUriGroup (req)) // CUtils.SetRequestUri(req, req.index))
            {
                QueueOrLoad (req);
            } else {
                if (!req.isShared)
                    currentLoaded++;
                CallbackError (req);
                if (!req.isShared && !string.IsNullOrEmpty (req.assetBundleName) && _instance != null && _instance.OnAssetBundleErr != null)
                    _instance.OnAssetBundleErr (req);
                BeginQueue ();
                CheckAllComplete ();
            }
        }

        /// <summary>
        /// Load Asset Error Callback
        /// </summary>
        /// <param name="req">Req.</param>       
        protected static void CallbackError (CRequest creq) {
#if HUGULA_LOADER_DEBUG
            Debug.LogFormat ("4 <color=green>CallbackError DispatchEnd Req(assetname={0},url={1}) frame={2} </color>", creq.assetName, creq.url, Time.frameCount);
#endif
            List<CRequest> callbacklist = null; // requestCallBackList[creq.udKey];
            string udkey = creq.udKey;
            // load
            if (creq.isShared) {
                if (_instance.OnSharedErr != null)
                    _instance.OnSharedErr (creq);
            }

            if (requestCallBackList.TryGetValue (udkey, out callbacklist)) {
                requestCallBackList.Remove (udkey);
                int count = callbacklist.Count;
                CRequest reqitem;
                for (int i = 0; i < count; i++) {
                    reqitem = callbacklist[i];
                    reqitem.DispatchEnd ();
                    PopGroup (reqitem);
                }
                ListPool<CRequest>.Release (callbacklist);
            } else {
                creq.DispatchEnd ();
                PopGroup (creq);
            }

            CheckAllComplete ();
        }

        /// <summary>
        /// check all request is complete
        /// </summary>
        /// <param name="req"></param>
        protected static void CheckAllComplete () {
            if (currentLoading <= 0 && queue.Count == 0) {
                var loadingEvent = _instance.loadingEvent;
                loadingEvent.target = _instance;
                loadingEvent.total = totalLoading;
                loadingEvent.progress = 1;
                loadingEvent.current = currentLoaded;
                if (_instance.OnProgress != null && totalLoading > 0) {
                    _instance.OnProgress (loadingEvent);
                }

                if (_instance.OnAllComplete != null)
                    _instance.OnAllComplete (_instance);
            }
        }

        static void PushGroup (CRequest req, GroupRequestRecord group) {
            if (group != null) {
                currGroupRequestsRef[req] = group;
                group.Add (req);
                if (!allGroupRequestRecord.Contains (group))
                    allGroupRequestRecord.Add (group);
            }
        }

        static void PopGroup (CRequest req) {
            GroupRequestRecord group;
            if (currGroupRequestsRef.TryGetValue (req, out group)) {
                currGroupRequestsRef.Remove (req);
                group.Complete (req);
            }

            if (req.pool) LRequest.Release (req);
        }

        #endregion

        #region  eventhandle

        static void OnProcess (CCar loader, float progress) {
            var loadingEvent = _instance.loadingEvent;
            loadingEvent.target = _instance;
            if (totalLoading <= 0)
                loadingEvent.total = 1;
            else
                loadingEvent.total = totalLoading;
            if (loadingEvent.current < currentLoaded) loadingEvent.current = currentLoaded;
            loadingEvent.progress = progress;
            if (_instance && _instance.OnProgress != null && totalLoading > 0) {
                _instance.OnProgress (loadingEvent);
            }
        }

        #endregion

        #region static assetbundle

        /// <summary>
        /// 加载依赖项目
        /// </summary>
        /// <param name="req"></param>
        static protected int[] LoadDependencies (CRequest req) {
            string[] deps = assetBundleManifest.GetDirectDependencies (req.assetBundleName);
            if (deps.Length == 0) return null;

            string dep_url;
            string depAbName = "";
            CRequest item;
            int[] hashs = new int[deps.Length];
            int keyhash;

            for (int i = 0; i < deps.Length; i++) {
                depAbName = deps[i];
                if (string.IsNullOrEmpty (depAbName)) // Dependency assetbundle name is empty  unity bug?
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat ("the request({0},{1}) Dependencies {2} is empty ", req.assetName, req.url, i);
#endif
                    hashs[i] = 0;
                    continue;
                }
                dep_url = RemapVariantName (depAbName);
                keyhash = LuaHelper.StringToHash (dep_url);
                hashs[i] = keyhash;
#if UNITY_EDITOR
                CountMananger.WillAdd (dep_url); //引用数量加1
#else
                CountMananger.WillAdd (keyhash); //引用数量加1
#endif

                CacheData sharedCD = CacheManager.GetCache (keyhash);
                if (sharedCD != null) {
                    if (!sharedCD.isAssetLoaded) {
                        item = LRequest.Get ();
                        item.relativeUrl = dep_url;
                        item.isShared = true;
                        item.async = false;
                        item.isAssetBundle = true;

                        CacheManager.SetRequestDataFromCache (req);
                        if (_instance && _instance.OnSharedComplete != null)
                            _instance.OnSharedComplete (item);

                        LRequest.Release (item);
                    }
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat (" 0.3 <color=#15A0A1>Request(assetName={0}, url={1},isShared={2}) Dependencies  CacheManager.Contains(url={3},sharedCD.isAssetLoaded={4}) </color>", req.assetName, req.url, req.isShared, dep_url, sharedCD.isAssetLoaded);
#endif
                } else {

                    item = LRequest.Get ();
                    item.relativeUrl = dep_url;
                    item.isShared = true;
                    item.async = false;
                    item.isAssetBundle = true;
                    item.allDependencies = LoadDependencies (item);
                    item.isNormal = false;
                    item.priority = req.priority;
                    item.uris = req.uris;
#if HUGULA_LOADER_DEBUG
                    Debug.LogFormat ("<color=#15A0A1>0.5  Request(assetname={0}) Begin Load  Dependencies Req({1},allDependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.assetName, item.assetName, item.keyHashCode, item.allDependencies == null ? 0 : item.allDependencies.Length, Time.frameCount);
#endif
                    AddReqToQueue (item);
                }
            }

            return hashs;
        }

        /// <summary>
        /// 判断加载
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected bool CheckLoadedAssetBundle (CRequest req) {
            if (CacheManager.Contains (req.keyHashCode)) return true;
            return false;
        }

        static protected bool CheckLoadingAssetBundle (CRequest req) {
            if (downloadings.ContainsKey (req.udKey) && req.index == 0) return true;
            return false;
        }

        static int CompareFunc (CRequest a, CRequest b) {
            if (a.priority < b.priority) return 1;
            if (a.priority > b.priority) return -1;
            return 0;
        }

        /// <summary>
        /// ab manifest
        /// </summary>
        public static AssetBundleManifest assetBundleManifest;

        static string[] _activeVariants = { };

        /// <summary>
        ///  Variants which is used to define the active variants.
        /// </summary>
        public static string[] ActiveVariants {
            get { return _activeVariants; }
            set { _activeVariants = value; }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        static protected string RemapVariantName (string assetBundleName) {
            string[] bundlesWithVariant = assetBundleManifest.GetAllAssetBundlesWithVariant ();

            // Get base bundle name
            string baseName = assetBundleName.Split ('.')[0];

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++) {
                string[] curSplit = bundlesWithVariant[i].Split ('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                int found = System.Array.IndexOf (_activeVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit) {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1) {
#if UNITY_EDITOR
                Debug.LogWarning ("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
#endif
            }

            if (bestFitIndex != -1) {
                return bundlesWithVariant[bestFitIndex];
            } else {
                return assetBundleName;
            }
        }

        #endregion

        #region static load pool

        /// <summary>
        /// 获取空闲的loader
        /// </summary>
        /// <returns></returns>
        internal static void CreateFreeLoader () {
            int dtCount = loaderPool.Count - maxLoading; //计算差值

            if (dtCount < 0) {
                for (int i = dtCount; i <= -1; i++) {
                    CCar cts = GetCCar (); //new CCar ();
                    loaderPool.Add (cts);
                }
            } else if (dtCount > 0) {
                for (int i = dtCount; i >= 1; i--) {
                    int rIndex = loaderPool.Count - 1;
                    CCar cts = loaderPool[rIndex];
                    loaderPool.RemoveAt (rIndex);
                    freeLoader.Add (cts);
                }
            }
            //			Debug.LogFormat(" count = {0},dtCount {1} maxLoading {2}",loaderPool.Count,dtCount,maxLoading);
        }

        #endregion

        #region  delegate and event

        public System.Action<CResLoader> OnAllComplete;
        public System.Action<LoadingEventArg> OnProgress;
        public System.Action<CRequest> OnSharedComplete;
        public System.Action<CRequest> OnSharedErr;
        public System.Action<CRequest> OnAssetBundleComplete;
        public System.Action<CRequest> OnAssetBundleErr;
        #endregion

        #region instance
        protected static CResLoader _instance;
        #endregion

        #region car pool 

        static ObjectPool<CCar> freeLoaderPool = new ObjectPool<CCar> (m_CCarOnGet, m_CCarOnRelease);

        static void m_CCarOnGet (CCar cts) {
            cts.OnComplete = LoadComplete;
            cts.OnError = LoadError;
            cts.OnProcess = OnProcess;
        }

        static void m_CCarOnRelease (CCar re) {
            //re.isFree = true;
        }

        static CCar GetCCar () {
            return freeLoaderPool.Get ();
        }

        static void ReleaseCCar (CCar car) {
            freeLoaderPool.Release (car);
        }

        #endregion

        #region stop load
        /// <summary>
        /// stop the req.
        /// </summary>
        /// <param name="req">Req.</param>
        /// <param name="group">Group.</param>
        public void StopReq (CRequest reqUrl) {
            string udKey = reqUrl.udKey;
            string udAssetKey = reqUrl.udAssetKey;

            //step 1 remove request list
            List<CRequest> removeReqs = ListPool<CRequest>.Get ();
            CRequest req;
            for (int i = 0; i < queue.Count;) {
                req = queue[i];
                if (req.udKey.Equals (udKey)) {
                    queue.RemoveAt (i);
                    removeReqs.Add (req);
                } else
                    i++;
            }

            // step 2 remove loading assetbundle
            CCar load = null;
            if (downloadings.TryGetValue (udKey, out load)) {
                removeReqs.Add (load.req);
                load.StopLoad ();
                downloadings.Remove (udKey);
            }

            for (int i = 0; i < loadingAssetBundleQueue.Count;) {
                req = loadingAssetBundleQueue[i];
                if (req.udKey.Equals (udKey)) {
                    loadingAssetBundleQueue.RemoveAt (i);
                    removeReqs.Add (req);
                } else
                    i++;
            }

            //step 3 remove loadingAssetQueue
            for (int i = 0; i < loadingAssetQueue.Count;) {
                req = loadingAssetQueue[i];
                if (req.udKey.Equals (udKey)) {
                    loadingAssetQueue.RemoveAt (i);
                    removeReqs.Add (req);
                } else
                    i++;
            }

            //step 4 remove loadedAssetQueue
            // while (loadedAssetQueue.Count > 0)
            // {
            //     req = loadedAssetQueue.Dequeue();
            //     removeReqs.Add(req);
            // }

            //remove from  requestCallBackList
            List<CRequest> callbacklist = null;
            if (requestCallBackList.TryGetValue (udKey, out callbacklist)) {
                requestCallBackList.Remove (udKey);
                ListPool<CRequest>.Release (callbacklist);
            }

            if (assetCallBackList.TryGetValue (udAssetKey, out callbacklist)) {
                assetCallBackList.Remove (udAssetKey);
                ListPool<CRequest>.Release (callbacklist);
            }

            for (int i = 0; i < removeReqs.Count; i++) {
                req = removeReqs[i];
                PopGroup (req);
            }

            ListPool<CRequest>.Release (removeReqs);
        }

        /// <summary>
        /// stop the url.
        /// </summary>
        /// <param name="req">Req.</param>
        /// <param name="group">Group.</param>
        public void StopURL (string url) {
            LRequest reqUrl = LRequest.Get ();
            reqUrl.relativeUrl = url;
            StopReq (reqUrl);
            LRequest.Release (reqUrl);
        }

        public void StopAll () {

            List<CRequest> removeReqs = ListPool<CRequest>.Get ();
            CRequest req = null;
            for (int i = 0; i < queue.Count;) {
                req = queue[i];
                queue.RemoveAt (i);
                removeReqs.Add (req);
            }

            // step 2 remove loading assetbundle
            CCar load = null;

            foreach (var kv in downloadings) {
                load = kv.Value;
                removeReqs.Add (load.req);
                load.StopLoad ();
            }
            downloadings.Clear ();

            for (int i = 0; i < loadingAssetBundleQueue.Count;) {
                req = loadingAssetBundleQueue[i];
                loadingAssetBundleQueue.RemoveAt (i);
                removeReqs.Add (req);
            }

            //step 3 remove loadingAssetQueue
            for (int i = 0; i < loadingAssetQueue.Count;) {
                req = loadingAssetQueue[i];
                loadingAssetQueue.RemoveAt (i);
                removeReqs.Add (req);
            }

            //step 4 remove loadedAssetQueue
            while (loadedAssetQueue.Count > 0) {
                req = loadedAssetQueue.Dequeue ();
                removeReqs.Add (req);
            }

            //remove from  requestCallBackList
            List<CRequest> callbacklist = null;
            foreach (var kv in requestCallBackList) {
                callbacklist = kv.Value;
                ListPool<CRequest>.Release (callbacklist);
            }

            foreach (var kv in assetCallBackList) {
                callbacklist = kv.Value;
                ListPool<CRequest>.Release (callbacklist);
            }

            for (int i = 0; i < removeReqs.Count; i++) {
                req = removeReqs[i];
                PopGroup (req);
            }

            ListPool<CRequest>.Release (removeReqs);

        }

        /// <summary>
        /// Removes all actions.
        /// </summary>
        public virtual void RemoveAllEvents () {
            StopAll ();
            OnAllComplete = null;
            OnProgress = null;
            OnSharedComplete = null;
            OnSharedErr = null;
            OnAssetBundleComplete = null;
            OnAssetBundleErr = null;
        }

        #endregion
    }

    [SLua.CustomLuaClass]
    public class LoadingEventArg {
        //public int number;//current loading number
        public object target;
        public int total;
        public int current;
        public float progress;
    }

    public class CRequestComparer : IComparer<CRequest> {
        public int Compare (CRequest a, CRequest b) {
            if (a.priority < b.priority) return 1;
            if (a.priority > b.priority) return -1;
            return 0;
        }
    }

}