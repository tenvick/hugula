using System.Collections;
using System.Collections.Generic;
using System.Net;
using Hugula.Collections;
using Hugula.Utils;
using SLua;
using UnityEngine;

namespace Hugula.Loader {

    [SLua.CustomLuaClassAttribute]
    public class ResourcesLoader : MonoBehaviour {
        #region 属性
        /// <summary>
        /// 大于此size的ab使用异步加载
        /// </summary>
        static public uint asyncSize = 0 * 1024;

        /// <summary>
        /// 最大同时加载assetbundle asset数量
        /// </summary>
        static public int maxLoading = 2;

        /// <summary>
        /// 最大同时加载assetbundle数量
        /// </summary>
        static public int bundleMax = 4;
        /// <summary>
        /// 加载bundle耗时跳出判断时间
        /// </summary>
        static public float BundleLoadBreakMilliSeconds = 25;

        //total count
        static private int totalCount = 0;
        //current loaded
        static private int currLoaded = 0;

        public delegate string OverrideBaseDownloadingURLDelegate (string bundleName);

        /// <summary>
        /// Implements per-bundle base downloading URL override.
        /// The subscribers must return null values for unknown bundle names;
        /// </summary>
        public static event OverrideBaseDownloadingURLDelegate overrideBaseDownloadingURL;
        static private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

        //protected
        static protected LoadingEventArg loadingEvent;

        ///<summary>
        ///等待请求列表
        ///<summary>
        static protected Queue<CRequest> waitRequest = new Queue<CRequest> ();

        static List<CRequest> inProgressBundleOperations = new List<CRequest> ();

        static List<ResourcesLoadOperation> inProgressOperations = new List<ResourcesLoadOperation> ();
        //loading asset list
        static protected List<CRequest> loadingTasks = new List<CRequest> ();
        static List<string> downloadingBundles = new List<string> ();
        #endregion

        #region public
        /// <summary>
        /// for lua加载ab资源
        /// </summary>
        /// <param name="reqs"></param>
        static public void LoadLuaTable (LuaTable reqs, System.Action<bool> groupCompleteFn, System.Action<LoadingEventArg> groupProgressFn, int priority = 0) {
            var groupQueue = BundleGroundQueue.Get ();
            groupQueue.onComplete = groupCompleteFn;
            groupQueue.onProgress = groupProgressFn;

            // 
            CRequest reqitem;
            foreach (var req in reqs) {
                reqitem = (CRequest) req.value;
                reqitem.priority = priority;
                groupQueue.Enqueue (reqitem);
            }

            LoadGroupAsset (groupQueue);

        }

        /// <summary>
        /// load group request
        /// </summary>
        /// <param name="bGroup"></param>
        static public void LoadGroupAsset (BundleGroundQueue bGroup) {
            while (bGroup != null && bGroup.Count > 0) {
                var req = bGroup.Dequeue ();
                LoadAsset (req);
            }
        }

        static public void LoadAsset (CRequest req) {
            string relativeUrl = ManifestManager.RemapVariantName (req.vUrl);
            req.vUrl = relativeUrl;

            //check asset cache
            if (CheckAssetIsLoaded (req)) {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (req.key, "<color=#15A0A1>2.0.0 LoadAssetFromCache Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
                return;
            }
            waitRequest.Enqueue (req); //等待

            LoadingQueue ();
        }

        /// <summary>
        /// 以 Coroutine方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        static public CRequest LoadAssetCoroutine (string abName, string assetName, System.Type type, int priority = 0) {
            var req = new CRequest ();
            req.priority = priority;
            req.assetName = assetName;
            req.assetType = type;
            req.vUrl = abName;
            LoadAsset (req);
            return req;
        }

        /// <summary>
        /// 以 回调方式加载 ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="priority"></param>
        static public void LoadAsset (string abName, string assetName, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd, int priority = 0) {
            var req = CRequest.Get ();
            req.priority = priority;
            req.assetName = assetName;
            req.assetType = type;
            req.vUrl = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            LoadAsset (req);
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public CRequest UnityWebRequestCoroutine (string url, object uploadData, System.Type type) {
            var req = new CRequest ();
            req.uploadData = uploadData;
            req.assetType = type;
            req.vUrl = url;
            LoadUnityHttpInternal (req);
            return req;
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        static public void UnityWebRequest (string url, object uploadData, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd) {
            var req = CRequest.Get ();
            req.vUrl = url;
            req.uploadData = uploadData;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            LoadUnityHttpInternal (req);
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public void UnityWebRequest (CRequest req) {
            LoadUnityHttpInternal (req);
        }

        static public CRequest HttpWebRequestCoroutine (string url, object uploadData, System.Type type) {
            var req = new CRequest ();
            req.uploadData = uploadData;
            req.assetType = type;
            req.vUrl = url;
            LoadWebHttpInternal (req);
            return req;
        }

        static public void HttpWebRequest (string url, object uploadData, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd) {
            var req = CRequest.Get ();
            req.vUrl = url;
            req.uploadData = uploadData;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            LoadWebHttpInternal (req);
        }

        static public void HttpWebRequest (CRequest req) {
            LoadWebHttpInternal (req);
        }

        public static void RegisterOverrideBaseAssetbundleURL (OverrideBaseDownloadingURLDelegate baseDownloadingURLDelegate) {
            overrideBaseDownloadingURL -= baseDownloadingURLDelegate;
            overrideBaseDownloadingURL += baseDownloadingURLDelegate;
        }

        #endregion

        #region load logic
        internal static string GetAssetBundleDownloadingURL (string bundleName) {
            if (overrideBaseDownloadingURL != null) {
                foreach (OverrideBaseDownloadingURLDelegate method in overrideBaseDownloadingURL.GetInvocationList ()) {
                    string res = method (bundleName);
                    if (res != null)
                        return res;
                }
            }

            return CUtils.PathCombine (CUtils.GetRealStreamingAssetsPath (), bundleName);
        }

        /// <summary>
        /// check the queue and load assetbundle and asset
        /// </summary>
        static protected bool LoadingQueue () {
            while (waitRequest.Count > 0 && maxLoading - loadingTasks.Count > 0) {
                var req = waitRequest.Dequeue ();
                LoadAssetFromBundle (req);
            }
            return false;
        }

        /// <summary>
        /// load asset from cache
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static bool LoadAssetFromCache (CRequest req) {
            var cache = CacheManager.TryGetCache (req.keyHashCode);
            if (cache != null) {
                var asset = cache.GetAsset (req.udAssetKey);
                if (asset != null) {
                    req.data = asset;
                    // Debug.LogFormat("LoadAssetFromCache Req(assetBundleName={0},assetName={1},data={2}); frame={3}", req.assetBundleName, req.assetName, asset, Time.frameCount);
                    return true;
                }
            }

            return false;
        }

        static protected bool CheckAssetIsLoaded (CRequest req) {
            if (LoadAssetFromCache (req)) {
                ABDelayUnloadManager.CheckRemove (req.keyHashCode);
                DispatchReqAssetOperation (req, false);
                return true;
            } else
                return false;
        }

        /// <summary>
        /// load assetbundle 
        /// </summary>
        /// <param name="req"></param>
        static internal void LoadAssetFromBundle (CRequest req) {

#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor) {
                LoadAssetInternalSimulation (req);
                return;
            }
#endif
            totalCount++; //count ++

            if (CheckAssetIsLoaded (req)) return;

            //remove delay unload assetbundle 
            ABDelayUnloadManager.CheckRemove (req.keyHashCode);

            loadingTasks.Add (req);

            //loading assetbundle 

            if (!downloadingBundles.Contains (req.key) && CacheManager.GetCache (req.keyHashCode) == null) //check is loading
            {
                //load dependencies and refrenece count
                string[] deps = null;
                if (ManifestManager.fileManifest != null && (deps = ManifestManager.fileManifest.GetDirectDependencies (req.key)).Length > 0)
                    req.dependencies = LoadDependencies (req, deps);

                //load assetbundle
                LoadAssetBundleInternal (req);
            }

            LoadAssetInternal (req);

        }

        /// <summary>
        /// load LoadDependencies assetbundle
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected int[] LoadDependencies (CRequest req, string[] deps) {
            // string[] deps = ManifestManager.fileManifest.GetDirectDependencies (req.key);
            if (deps.Length == 0) return null;
            string abName = string.Empty;

            string dep_url;
            string depAbName = "";
            CRequest item;
            int[] hashs = new int[deps.Length];
            int keyhash;

            for (int i = 0; i < deps.Length; i++) {
                depAbName = CUtils.GetBaseName (deps[i]);
                dep_url = ManifestManager.RemapVariantName (depAbName); //string gc alloc

                keyhash = LuaHelper.StringToHash (dep_url);
                hashs[i] = keyhash;

                CacheData sharedCD = CacheManager.TryGetCache (keyhash);
                if (sharedCD != null) {
                    sharedCD.count++;
                    int count = sharedCD.count; //CountMananger.WillAdd(keyhash); //引用数量加1;
#if HUGULA_CACHE_DEBUG
                    HugulaDebug.FilterLogFormat (dep_url, " <color=#fcfcfc> add  (assetBundle={0},parent={1},hash={2},count={3}) frameCount={4}</color>", dep_url, req.key, keyhash, count, UnityEngine.Time.frameCount);
#endif
                    if (count == 1) //相加后为1，可能在回收列表，需要对所有依赖项目引用+1
                        ABDelayUnloadManager.CheckRemove (keyhash);
                } else {
#if HUGULA_CACHE_DEBUG
                    int count = CountMananger.WillAdd (keyhash); //引用数量加1
                    HugulaDebug.FilterLogFormat (dep_url, " <color=#fcfcfc> will add  (assetBundle={0},parent={1},hash={2},count={3}) frameCount={4}</color>", dep_url, req.key, keyhash, count, UnityEngine.Time.frameCount);
#else

                    CountMananger.WillAdd (keyhash); //引用数量加1
#endif
                    item = CRequest.Get ();
                    item.vUrl = dep_url;
                    item.isShared = true;
                    item.async = req.async;
                    item.priority = req.priority;
                    string[] deps1 = ManifestManager.fileManifest.GetDirectDependencies (item.key);
                    if (deps1.Length > 0)
                        item.dependencies = LoadDependencies (req, deps1);
                    LoadAssetBundleInternal (item);
                }
            }

            return hashs;
        }

        /// <summary>
        /// cache asset
        /// </summary>
        /// <param name="req"></param>
        static protected void SetCacheAsset (CRequest req) {
            //cache asset
            var cache = CacheManager.TryGetCache (req.keyHashCode);
            if (cache != null) {
                cache.SetAsset (req.udAssetKey, req.data);
            }
        }

        /// <summary>
        /// dispatch request event 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="isError"></param>
        static internal void DispatchReqAssetOperation (CRequest req, bool isError) {
            var group = req.group;

            currLoaded++;

            OnProcess ();

            if (isError)
                req.DispatchEnd ();
            else
                req.DispatchComplete ();

            if (group != null) group.Complete (req, isError);

            req.ReleaseToPool ();

        }

        static protected void LoadUnityHttpInternal (CRequest req) {
            var op = OperationPools<UnityWebRequestOperation>.Get ();
            op.SetRequest (req);
            inProgressOperations.Add (op);
            op.Start ();
        }

        static protected void LoadWebHttpInternal (CRequest req) {
            var op = OperationPools<HttpWebRequestOperation>.Get ();
            op.SetRequest (req);
            inProgressOperations.Add (op);
            op.Start ();
        }

        static protected bool LoadAssetBundleInternal (CRequest req) {
            if (!downloadingBundles.Contains (req.key) && CacheManager.GetCache (req.keyHashCode) == null) {

                if (bundleMax - downloadingBundles.Count > 0) {
                    var op = OperationPools<LoadAssetBundleInternalOperation>.Get ();
                    op.SetRequest (req);
                    inProgressOperations.Add (op);
                    op.Start ();
                    downloadingBundles.Add (req.key);

                    return true;
                } else {
                    inProgressBundleOperations.Add (req); //wait bundle
                }
            }
            return false;
        }

        /// <summary>
        /// load bundles
        /// </summary>
        /// <param name="operation"></param>
        static protected void LoadingBundleQueue()
        {
            while(bundleMax - downloadingBundles.Count > 0 && inProgressBundleOperations.Count > 0) {
                var req = inProgressBundleOperations[0];
                inProgressBundleOperations.RemoveAt(0);
                LoadAssetBundleInternal(req);
            }
        }

        static protected void LoadAssetInternal (CRequest req) {
            ResourcesLoadOperation op = null;
            if (LoaderType.Typeof_ABScene.Equals (req.assetType))
                op = OperationPools<LoadLevelOperation>.Get ();
            else
                op = OperationPools<LoadAssetOperation>.Get ();

            op.SetRequest (req);
            inProgressOperations.Add (op);
            op.Start ();
        }

#if UNITY_EDITOR
        static protected void LoadAssetInternalSimulation (CRequest req) {
            ResourcesLoadOperation op = null;
            if (LoaderType.Typeof_ABScene.Equals (req.assetType))
                op = OperationPools<LoadLevelOperationSimulation>.Get ();
            else
                op = OperationPools<LoadAssetOperationSimulation>.Get ();

            op.SetRequest (req);
            inProgressOperations.Add (op);
            op.Start ();
        }

#endif

        //========================================
        //---------------event--------------------
        //========================================

        static protected void OnProcess () {
            loadingEvent.total = totalCount;
            loadingEvent.current = currLoaded;
            if (OnProgress != null && totalCount > 0) {
                loadingEvent.progress = (float) loadingEvent.current / (float) loadingEvent.total;
                OnProgress (loadingEvent);
            }
        }

        /// <summary>
        /// the queue is complete
        /// </summary>
        static protected void CheckAllComplete () {
            if (waitRequest.Count == 0 && loadingTasks.Count == 0) {
                totalCount = 0;
                currLoaded = 0;

                if (OnAllComplete != null)
                    OnAllComplete ();
            }
        }

        /// <summary>
        /// assetbundle download error
        /// </summary>
        /// <param name="req"></param>
        static protected void CallOnAssetBundleErr (CRequest req) {
            if (OnAssetBundleErr != null) OnAssetBundleErr (req);
        }

        /// <summary>
        /// assetbundle is done
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ab"></param>
        static protected void CallOnAssetBundleComplete (CRequest req, AssetBundle ab) {
            if (OnAssetBundleComplete != null) OnAssetBundleComplete (req, ab);
        }

        #endregion

        #region mono

#if HUGULA_LOADER_DEBUG  || UNITY_EDITOR
        [SLua.DoNotToLua]
        public string LoadCountInfo;

        [SLua.DoNotToLua]
        public string DebugInfo;

        private System.Text.StringBuilder DebugSB = new System.Text.StringBuilder ();
#endif

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake () {
            DontDestroyOnLoad (this.gameObject);
            loadingEvent = new LoadingEventArg ();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update () {
            //begin load bunld
            watch.Reset ();
            watch.Start ();

            // Update all in progress operations
            for (int i = 0; i < inProgressOperations.Count;) {
                var operation = inProgressOperations[i];
                if (operation.Update ()) {
                    i++;
                } else {
                    inProgressOperations.RemoveAt (i);
                    ProcessFinishedOperation (operation);
                }

                if (watch.ElapsedMilliseconds >= BundleLoadBreakMilliSeconds * 3f)
                    break;
            }

            if (inProgressOperations.Count == 0)
                LoadingQueue ();

#if HUGULA_LOADER_DEBUG || UNITY_EDITOR
            LoadCountInfo = string.Format ("wait={0},loading={1} ,frame={2}\r\n", waitRequest.Count, inProgressOperations.Count, Time.frameCount);
            DebugSB.Length = 0;
            DebugSB.Append (LoadCountInfo);
            for (int i = 0; i < inProgressOperations.Count; i++) {
                var op = inProgressOperations[i];
                var req = op.cRequest;
                DebugSB.AppendFormat ("  CRequest({0},{1}) op={2} is loading.\r\n", req.key, req.assetName, op);
                var denps = req.dependencies;
                if (denps != null) {
                    CacheData cache = null;
                    int hash = 0;
                    for (int j = 0; j < denps.Length; j++) {
                        hash = denps[j];
                        if ((cache = CacheManager.TryGetCache (hash)) != null) {
                            if (!cache.isDone) // if (!cache.isAssetLoaded || !cache.canUse)
                                DebugSB.AppendFormat ("\tdenpens({0},{1}) is not done!\r\n", cache.assetBundleKey, cache.assetHashCode);
                        } else {
                            DebugSB.AppendFormat ("\tdenpens({0},{1}) is not exists!\r\n", cache.assetBundleKey, cache.assetHashCode);
                        }
                    }
                }
            }

            //
            // for (int i = 0; i < loadingTasks.Count; i++) {
            //     var operation = loadingTasks[i];
            //     var req = operation;
            //     DebugSB.AppendFormat ("CRequest({0},{1}) asset operation={2} loading.\r\n", req.key, req.assetName, operation);
            // }
            DebugInfo = DebugSB.ToString ();
            if (Time.frameCount % 240 == 0)
                Debug.LogFormat ("LOADER_DEBUG_info : {0}, frameCount={1}", DebugInfo, Time.frameCount);
#endif
        }

        /// <summary>
        /// auto clear assetbundle
        /// </summary>
        void LateUpdate () {
            ABDelayUnloadManager.Update ();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy () {
            waitRequest.Clear ();
            loadingTasks.Clear ();
            downloadingBundles.Clear ();

            OnAllComplete = null;
            OnProgress = null;

            OnAssetBundleComplete = null;
            OnAssetBundleErr = null;
        }

        #endregion

        /// <summary>
        /// finish asset or http request
        /// </summary>
        /// <param name="operation"></param>
        internal static void ProcessFinishedOperation (ResourcesLoadOperation operation) {

            HttpLoadOperation httpLoad;
            var req = operation.cRequest;
            operation.Done ();

            if (operation is LoadAssetBundleInternalOperation) {
                downloadingBundles.Remove (req.key);
                if (req.isShared) req.ReleaseToPool ();
                LoadingBundleQueue();
            } else if ((httpLoad = operation as HttpLoadOperation) != null) {
                bool isError = !string.IsNullOrEmpty (httpLoad.error);
                if (isError && CUtils.IsResolveHostError (httpLoad.error) && !CUtils.IsHttps (req.url)) // http dns 
                {
                    // req.error = httpLoad.error;
                    Debug.LogFormat("dns resolve error url={0} ", req.url);
                    // httpLoad.error = string.Format ("dns resolve error url={0} ", req.url);
                    var httpDnsOp = OperationPools<HttpDnsResolve>.Get (); // HttpDnsResolve.Get();
                    httpDnsOp.SetRequest (req);
                    httpDnsOp.SetOriginalOperation (httpLoad);
                    inProgressOperations.Add (httpDnsOp);
                    httpDnsOp.Start ();
                } else {
                    
                    operation.ReleaseToPool ();
    
                    if (isError)
                        req.DispatchEnd ();
                    else
                        req.DispatchComplete ();

                    if (req.group != null) req.group.Complete (req, isError);

                    req.ReleaseToPool ();

                }
            } else {

                loadingTasks.Remove (req);
                operation.ReleaseToPool ();

                DispatchReqAssetOperation (req, req.error != null);

                CheckAllComplete ();
            }

        }
        #region event

        public static System.Action OnAllComplete;
        public static System.Action<LoadingEventArg> OnProgress;

        public static System.Action<CRequest, AssetBundle> OnAssetBundleComplete;
        public static System.Action<CRequest> OnAssetBundleErr;
        #endregion

        #region static

        protected static ResourcesLoader _instance;

        public static ResourcesLoader instance {
            get {
                if (_instance == null) {
                    var go = new GameObject ("AssetBundleLoader");
                    DontDestroyOnLoad (go);
                    _instance = go.AddComponent<ResourcesLoader> ();
                }
                return _instance;
            }
        }
        public static void Initialize () {
            var ins = instance;
        }
        #endregion

    }

}