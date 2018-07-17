using UnityEngine;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using Hugula.Collections;
using SLua;

namespace Hugula.Loader
{

    [SLua.CustomLuaClassAttribute]
    public class ResourcesLoader : MonoBehaviour
    {
        #region 属性
        /// <summary>
        /// 大于此size的ab使用异步加载
        /// </summary>
        static public uint asyncSize = 0 * 1024;

        /// <summary>
        /// 最大同时加载assetbundle asset数量
        /// </summary>
        static public int maxLoading = 5;

        /// <summary>
        /// 加载bundle耗时跳出判断时间
        /// </summary>
        static public float BundleLoadBreakMilliSeconds = 25;


        //total count
        static private int totalCount = 0;
        //current loaded
        static private int currLoaded = 0;

        public delegate string OverrideBaseDownloadingURLDelegate(string bundleName);

        /// <summary>
        /// Implements per-bundle base downloading URL override.
        /// The subscribers must return null values for unknown bundle names;
        /// </summary>
        public static event OverrideBaseDownloadingURLDelegate overrideBaseDownloadingURL;
        static private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        //protected
        static protected LoadingEventArg loadingEvent;

   
        ///<summary>
        ///等待请求列表
        ///<summary>
        static protected Queue<CRequest> waitRequest = new Queue<CRequest>();
        //loading asset list
        static protected List<CRequest> loadingTasks = new List<CRequest>();
        //load asset or www operations
        static List<ResourcesLoadOperation> inProgressOperations = new List<ResourcesLoadOperation>();
        //load assetbundle operations
        static List<AssetBundleDownloadOperation> inProgressBundleOperations = new List<AssetBundleDownloadOperation>();
        // asset callback list
        static Dictionary<string, List<CRequest>> assetCallBackList = new Dictionary<string, List<CRequest>>();
        //dowding assetbundle
        static Dictionary<string, AssetBundleDownloadOperation> downloadingBundles = new Dictionary<string, AssetBundleDownloadOperation>();
        #endregion

        #region public
        /// <summary>
        /// for lua加载ab资源
        /// </summary>
        /// <param name="reqs"></param>
        static public void LoadLuaTable(LuaTable reqs, System.Action<bool> groupCompleteFn, System.Action<LoadingEventArg> groupProgressFn, int priority = 0)
        {
            var groupQueue = BundleGroundQueue.Get();
            groupQueue.onComplete = groupCompleteFn;
            groupQueue.onProgress = groupProgressFn;

            // 
            CRequest reqitem;
            foreach (var req in reqs)
            {
                reqitem = (CRequest)req.value;
                reqitem.priority = priority;
                groupQueue.Enqueue(reqitem);
            }

            LoadGroupAsset(groupQueue);

        }

        /// <summary>
        /// load group request
        /// </summary>
        /// <param name="bGroup"></param>
        static public void LoadGroupAsset(BundleGroundQueue bGroup)
        {
            while (bGroup != null && bGroup.Count > 0)
            {
                var req = bGroup.Dequeue();
                LoadAsset(req);
            }
        }


        static public void LoadAsset(CRequest req)
        {
            string relativeUrl = ManifestManager.RemapVariantName(req.vUrl);
            req.vUrl = relativeUrl;

            //check asset cache
            if (LoadAssetFromCache(req))
            {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(req.key, "<color=#15A0A1>2.0.0 LoadAssetFromCache Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
                DispatchReqAssetOperation(req, false);
                return;
            }

            //check loading count
            if(maxLoading - loadingTasks.Count <= 0 )
            {
                waitRequest.Enqueue(req);//等待
            }
            else
            {
                LoadAssetBundle(req);//real load
            }
        }

        /// <summary>
        /// 以 Coroutine方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        static public AssetBundleLoadAssetOperation LoadAssetCoroutine(string abName, string assetName, System.Type type, int priority = 0)
        {
            var req = CRequest.Get();
            req.priority = priority;
            req.assetName = assetName;
            req.assetType = type;
            req.vUrl = abName;
            return LoadAssetCoroutine(req);
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
        static public void LoadAsset(string abName, string assetName, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd, int priority = 0)
        {
            var req = CRequest.Get();
            req.priority = priority;
            req.assetName = assetName;
            req.assetType = type;
            req.vUrl = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            LoadAsset(req);
        }

        /// <summary>
        /// LoadAsset
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public AssetBundleLoadAssetOperation LoadAssetCoroutine(CRequest req)
        {
            AssetBundleLoadAssetOperation op = null;
#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor)
                op = new AssetBundleLoadAssetOperationSimulation();
            else
                op = new AssetBundleLoadAssetOperationFull();
#else
            op = new AssetBundleLoadAssetOperationFull();
#endif
                op.SetRequest(req);
                req.assetOperation = op;
                op.Update();

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (req.key, "<color=#15A0A1>0.0.1  before LoadGroupAsset, ResourcesLoader.LoadAsset(Request(url={0},assetname={1},keyhash={2}),coroutine={3}),assetOperation={4},frameCount={5}</color>", req.url, req.assetName,req.keyHashCode ,coroutine,op,Time.frameCount);
#endif

            LoadAsset(req);
            return op;
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public HttpLoadOperation UnityWebRequestCoroutine(string url, WebHeaderCollection head, System.Type type)
        {
            var req = CRequest.Get();
            req.head = head;
            req.assetType = type;
            req.vUrl = url;
            var op = new UnityWebRequestOperation();
            req.assetOperation = op;
            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
            return op;
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        static public void UnityWebRequest(string url, WebHeaderCollection head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd)
        {
            var req = CRequest.Get();
            req.vUrl = url;
            req.head = head;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            UnityWebRequest(req);
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public void UnityWebRequest(CRequest req)
        {
            UnityWebRequestOperation op = UnityWebRequestOperation.Get();;
            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
        }

        /// <summary>
        /// 以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        static public void HttpWebRequest(string url, WebHeaderCollection head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd)
        {
            var req = CRequest.Get();
            req.vUrl = url;
            req.head = head;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            HttpWebRequest(req);
        }

        /// <summary>
        ///  以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public HttpLoadOperation HttpWebRequestCoroutine(string url, WebHeaderCollection head, System.Type type)
        {
            var req = CRequest.Get();
            req.vUrl = url;
            req.head = head;
            req.assetType = type;
            HttpWebRequestOperation op = new HttpWebRequestOperation();
            req.assetOperation = op;
            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
            return op;
        }

        /// <summary>
        ///  以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public void HttpWebRequest(CRequest req)
        {
            HttpWebRequestOperation op = HttpWebRequestOperation.Get();
            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
        }
        
        public static void RegisterOverrideBaseAssetbundleURL(OverrideBaseDownloadingURLDelegate baseDownloadingURLDelegate)
        {
            overrideBaseDownloadingURL -= baseDownloadingURLDelegate;
            overrideBaseDownloadingURL += baseDownloadingURLDelegate;
        } 

        #endregion

        #region load logic
        protected static string GetAssetBundleDownloadingURL(string bundleName)
        {
            if (overrideBaseDownloadingURL != null)
            {
                foreach (OverrideBaseDownloadingURLDelegate method in overrideBaseDownloadingURL.GetInvocationList())
                {
                    string res = method(bundleName);
                    if (res != null)
                        return res;
                }
            }

            return CUtils.PathCombine (CUtils.GetRealStreamingAssetsPath(), bundleName);
        }

        static protected bool CheckAssetIsLoaded(CRequest req)
        {
             if (LoadAssetFromCache(req))
             {
                ABDelayUnloadManager.CheckRemove (req.keyHashCode);
                DispatchReqAssetOperation (req, false);
                return true;
            }else
                return false;
        }


        /// <summary>
        /// check the queue and load assetbundle and asset
        /// </summary>
        static protected bool LoadingQueue()
        {
            // if (inProgressBundleOperations.Count > 1) return false; //wait bundle load

            while (waitRequest.Count > 0 && maxLoading - loadingTasks.Count > 0)
            {
                var req =waitRequest.Dequeue();
                LoadAssetBundle(req);
            }

            return false;
        }

        /// <summary>
        /// load asset from cache
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static internal bool LoadAssetFromCache(CRequest req)
        {
            var cache = CacheManager.TryGetCache(req.keyHashCode);
            if (cache != null)
            {
                var asset = cache.GetAsset(req.udAssetKey);
                if (asset != null)
                {
                    req.data = asset;
                    // Debug.LogFormat("LoadAssetFromCache Req(assetBundleName={0},assetName={1},data={2}); frame={3}", req.assetBundleName, req.assetName, asset, Time.frameCount);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// load assetbundle 
        /// </summary>
        /// <param name="req"></param>
        static protected void LoadAssetBundle(CRequest req)
        {

#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor)
            {
                //load asset
                ResourcesLoadOperation operation1;
                var tp1 = req.assetType;

                if (req.assetOperation != null)
                {
                    operation1 = req.assetOperation;
                }
                else if (LoaderType.Typeof_ABScene.Equals(tp1))
                {
                    operation1 = new AssetBundleLoadLevelSimulationOperation();
                    operation1.SetRequest(req);
                }
                else
                {
                    operation1 = new AssetBundleLoadAssetOperationSimulation();
                    operation1.SetRequest(req);
                }

                bool isLoading1 = false;

                if (operation1 is AssetBundleLoadAssetOperation)
                {
                    isLoading1 = AddAssetBundleLoadAssetOperationToCallBackList((AssetBundleLoadAssetOperation)operation1);
                }

                if (!isLoading1)
                {
                    inProgressOperations.Add(operation1);
                    loadingTasks.Add(req);
                }
                return;
            }
#endif

            //remove delay unload assetbundle 
            ABDelayUnloadManager.CheckRemove(req.keyHashCode);

            totalCount++;//count ++

            AssetBundleDownloadOperation abDownloadOperation = null;

            if (downloadingBundles.TryGetValue(req.key, out abDownloadOperation)) //check is loading
            {

            }
            else if (CheckAssetBundleCanLoad(req)) //need load
            {
                //load dependencies and refrenece count
                if (ManifestManager.fileManifest != null)
                    req.dependencies = LoadDependencies(req, null); //load dependencies assetbundle
                //load assetbundle
                abDownloadOperation = LoadAssetBundleInternal(req);
            }


            //load asset
            ResourcesLoadOperation operation;
            var tp = req.assetType;

            if (req.assetOperation != null)
            {
                operation = req.assetOperation;
            }
            else if (LoaderType.Typeof_ABScene.Equals(tp))
            {
                operation = new AssetBundleLoadLevelOperation();
                operation.SetRequest(req);
            }
            else
            {
                operation = AssetBundleLoadAssetOperationFull.Get();
                operation.SetRequest(req);
            }

            bool isLoading = false;
            //the same asset be one Operation
            if (operation is AssetBundleLoadAssetOperation)
            {
                isLoading = AddAssetBundleLoadAssetOperationToCallBackList((AssetBundleLoadAssetOperation)operation);
            }

            if (!isLoading)
            {
                if (abDownloadOperation != null)
                    abDownloadOperation.AddNext(operation);//wait for assetbunle complete
                else
                    inProgressOperations.Add(operation);// the assetbundle is done

                loadingTasks.Add(req);
            }

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(req.key, "<color=#15A0A1>2.0.1 LoadAssetBundle Asset Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
        }

        /// <summary>
        /// load LoadDependencies assetbundle
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected int[] LoadDependencies(CRequest req, CRequest parent)
        {
            string[] deps = ManifestManager.fileManifest.GetDirectDependencies(req.key);
            if (deps.Length == 0) return null;
            string abName = string.Empty;
            if (parent != null) abName = CUtils.GetBaseName(parent.key);

            string dep_url;
            string depAbName = "";
            CRequest item;
            int[] hashs = new int[deps.Length];
            int keyhash;

            for (int i = 0; i < deps.Length; i++)
            {
                depAbName = CUtils.GetBaseName(deps[i]);
                if (abName == depAbName)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("Dependencies({1}) Contains the parent({0}) ! ", req.key, abName);
#else
                    Debug.LogWarningFormat("Dependencies({1}) Contains the parent({0}) ! ",req.key,abName);
#endif
                    hashs[i] = 0;
                    continue;
                }

                dep_url = ManifestManager.RemapVariantName(depAbName);//string gc alloc

                keyhash = LuaHelper.StringToHash(dep_url);
                hashs[i] = keyhash;

                CacheData sharedCD = CacheManager.TryGetCache(keyhash);
                if (sharedCD != null)
                {
                    sharedCD.count++;
                    int count = sharedCD.count;//CountMananger.WillAdd(keyhash); //引用数量加1;
#if HUGULA_CACHE_DEBUG
                    HugulaDebug.FilterLogFormat(dep_url, " <color=#fcfcfc> add  (assetBundle={0},parent={1},hash={2},count={3}) frameCount={4}</color>", dep_url,req.key ,keyhash, count, UnityEngine.Time.frameCount);
#endif
                    if (count == 1)//相加后为1，可能在回收列表，需要对所有依赖项目引用+1
                        ABDelayUnloadManager.CheckRemove(keyhash);
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(req.key, " <color=#15A0A1> 1.1 Shared  AssetBundle is Done Request(assetName={0})  Parent Req(assetName={1},key={2},isShared={3}) </color>", sharedCD.assetBundleKey, req.assetName, req.key, req.isShared);
#endif
                }
                else
                {
#if HUGULA_CACHE_DEBUG
                    int count = CountMananger.WillAdd(keyhash); //引用数量加1
                    HugulaDebug.FilterLogFormat(dep_url, " <color=#fcfcfc> will add  (assetBundle={0},parent={1},hash={2},count={3}) frameCount={4}</color>", dep_url, req.key,keyhash, count, UnityEngine.Time.frameCount);
#else

                    CountMananger.WillAdd(keyhash); //引用数量加1
#endif
                    item = CRequest.Get();
                    item.vUrl = dep_url;
                    item.isShared = true;
                    item.async = req.async;
                    item.priority = req.priority;
                    item.dependencies = LoadDependencies(item, req);
                    // item.uris = req.uris;
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(req.key, "<color=#15A0A1>1.1  Load Dependencies Req({1},keyHashCode{2})  AssetBundleInternal  Parent Request(assetname={0}), dependencies.count={3},frameCount{4}</color>", req.assetName, item.assetName, item.keyHashCode, item.dependencies == null ? 0 : item.dependencies.Length, Time.frameCount);
#endif

                    LoadAssetBundleInternal(item);
                }
            }

            return hashs;
        }

        /// <summary>
        /// real load assetbundle
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected AssetBundleDownloadOperation LoadAssetBundleInternal(CRequest req)
        {
            AssetBundleDownloadOperation abDownloadOp = null;
            if (!downloadingBundles.TryGetValue(req.key, out abDownloadOp))
            {
                req.url = GetAssetBundleDownloadingURL(req.vUrl);// set full url

                if (req.url.StartsWith(Common.HTTP_STRING))  //load assetbunlde
                {
                    abDownloadOp = AssetBundleDownloadFromWebOperation.Get();
                }
                else
                {
                    abDownloadOp = AssetBundleDownloadFromDiskOperation.Get();
                }
                abDownloadOp.SetRequest(req);
                downloadingBundles.Add(req.key, abDownloadOp);

                CacheData cached = null;
                CacheManager.CreateOrGetCache(req.keyHashCode, out cached);//cache data
                inProgressBundleOperations.Add(abDownloadOp);
                abDownloadOp.BeginDownload();

#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.2 LoadAssetBundleInternal Request(key={0},isShared={1},assetname={2},dependencies.count={4})keyHashCode{3}, frameCount{5}</color>", req.key, req.isShared, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
            }
            else if (req.isShared)
            {
                req.ReleaseToPool();
            }

            return abDownloadOp;
        }

        /// <summary>
        /// check the assetbundle is loaded
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected bool CheckAssetBundleCanLoad(CRequest req)
        {
            if (CacheManager.GetCache(req.keyHashCode) != null) return false;//cached
            return true;
        }

        /// <summary>
        /// append request to  assetCallBackList
        /// </summary>
        /// <param name="AssetBundleLoadAssetOperation">operation.</param>
        static protected bool AddAssetBundleLoadAssetOperationToCallBackList(AssetBundleLoadAssetOperation operation)
        {
            bool isLoading = false;
            var req = operation.cRequest;
            string key = req.udAssetKey;
            List<CRequest> list = null;
            if (assetCallBackList.TryGetValue(key, out list)) //回调列表
            {
                list.Add(req);
                isLoading = true;
                operation.ReleaseToPool();
            }
            else
            {
                list = ListPool<CRequest>.Get();
                assetCallBackList.Add(key, list);
                list.Add(req);
            }
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(req.key, "<color=#ffff00>2.0.0 AddAssetBundleLoadAssetOperationToCallBackList  Request(url={0},assetname={1},keyHashCode{2},asyn={4}) list.count={3}, frameCount{5}</color>", req.url, req.assetName, req.keyHashCode, list.Count, req.async, Time.frameCount);
#endif
            return isLoading;
        }

        /// <summary>
        /// cache asset
        /// </summary>
        /// <param name="req"></param>
        static protected void SetCacheAsset(CRequest req)
        {
            //cache asset
            var cache = CacheManager.TryGetCache(req.keyHashCode);
            if (cache != null)
            {
                cache.SetAsset(req.udAssetKey, req.data);
            }
        }

        /// <summary>
        /// dispatch request event 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="isError"></param>
        static internal void DispatchReqAssetOperation(CRequest req, bool isError)
        {
            var group = req.group;

            if (isError)
                req.DispatchEnd();
            else
                req.DispatchComplete();

            if (group != null) group.Complete(req, isError);
#if HUGULA_PROFILER_DEBUG && !HUGULA_NO_LOG
            var now = System.DateTime.Now;
            var dt1 = now - req.beginQueueTime;
            var dt2 = now - req.beginLoadTime;
            Debug.LogFormat("<color=#0a9a0a>asset complete Request(asset={0},key={1}) alltime={2},loadtime={3};frame={4}</color>", req.assetName, req.key, dt1.TotalSeconds, dt2.TotalSeconds, Time.frameCount);
#endif

            req.ReleaseToPool();

        }

        /// <summary>
        /// asset complete 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="isError"></param>
        static internal void DispatchAssetBundleLoadAssetOperation(CRequest req, bool isError)
        {
            //source data
            var data = req.data;

            //call back
            string key = req.udAssetKey;
            List<CRequest> list = null;

            if (assetCallBackList.TryGetValue(key, out list)) //回调列表
            {
                assetCallBackList.Remove(key);
                int count = list.Count;
                CRequest reqitem = null;
                for (int i = 0; i < count; i++)
                {
                    reqitem = list[i];
                    reqitem.data = data;
                    currLoaded++;

                    OnProcess();
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(req.key, "DispatchReqComplete Request(data={0},assetName={1},url={2})isError={3},count={4},frameCount={5}", data, reqitem.assetName, reqitem.url, isError, count, Time.frameCount);
#endif
                    DispatchReqAssetOperation(reqitem, isError);

                }
                ListPool<CRequest>.Release(list);
            }
            else
            {
                currLoaded++;

                OnProcess();

                DispatchReqAssetOperation(req, isError);
            }
        }

        //========================================
        //---------------event--------------------
        //========================================

        static protected void OnProcess()
        {
            loadingEvent.total = totalCount;
            loadingEvent.current = currLoaded;
            if (OnProgress != null && totalCount > 0)
            {
                loadingEvent.progress = (float)loadingEvent.current / (float)loadingEvent.total;
                OnProgress(loadingEvent);
            }
        }

        /// <summary>
        /// the queue is complete
        /// </summary>
        static protected void CheckAllComplete()
        {
            if (inProgressBundleOperations.Count == 0 && loadingTasks.Count == 0)
            {
                totalCount = 0;
                currLoaded = 0;

                if (OnAllComplete != null)
                    OnAllComplete();
            }
        }

        /// <summary>
        /// assetbundle download error
        /// </summary>
        /// <param name="req"></param>
        static protected void CallOnAssetBundleErr(CRequest req)
        {
            if (OnAssetBundleErr != null) OnAssetBundleErr(req);
        }

        /// <summary>
        /// assetbundle is done
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ab"></param>
        static protected void CallOnAssetBundleComplete(CRequest req, AssetBundle ab)
        {
            if (OnAssetBundleComplete != null) OnAssetBundleComplete(req, ab);
#if HUGULA_PROFILER_DEBUG && !HUGULA_NO_LOG
            var now = System.DateTime.Now;
            var dt1 = now - req.beginQueueTime;
            var dt2 = now - req.beginLoadTime;
            var size = 0u;
            var abinfo = ManifestManager.GetABInfo(req.key);
            if (abinfo != null) size = abinfo.size;
            string tips = "AssetBundle";
            if (req.isShared)
            {
                tips = "Shared AssetBundle";
            }
            Debug.LogFormat("<color=#0a9a0a>{0} Request(asset={1},key={2},size={3}) alltime={4}s,loadtime={5}s,frame={6}</color>", tips, req.assetName, req.key, size, dt1.TotalSeconds, dt2.TotalSeconds, Time.frameCount);
#endif
        }

        //========================================
        //--------------- assetbundle asset logic--------------------
        //========================================

        /// <summary>
        /// Insert AssetBundleLoadAssetOperation to inProgressOperations list
        /// </summary>
        /// <param name="operation"></param>
        static void InsertAssetBundleLoadAssetOperation(AssetBundleDownloadOperation operation)
        {
            ResourcesLoadOperation rp = operation.next;
            while (rp != null)
            {
                inProgressOperations.Add(rp);
                rp = rp.next;
            }
        }

        /// <summary>
        /// finish assetbundle
        /// </summary>
        /// <param name="operation"></param>
        static void ProcessFinishedBundleOperation(AssetBundleDownloadOperation operation)
        {
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("ResourcesLoader.ProcessFinishedBundleOperation CRequest({0},shared={1})", operation.cRequest.assetName, operation.cRequest.isShared));
#endif
            var req = operation.cRequest;
            bool isError = operation.assetBundle == null;
            var ab = operation.assetBundle;
            AssetBundleDownloadOperation download = operation;

            if (!isError)
            {
                CacheManager.AddSourceCacheDataFromWWW(ab, req);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning(operation.error);
#endif
                CacheManager.AddErrorSourceCacheDataFromReq(req);
            }
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.9  ProcessFinishedBundleOperation AssetBundle Loaded Request(url={0},assetname={1},dependencies.count={3},keyHashCode{2}),isError={4} frameCount{5}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, isError, Time.frameCount);
#endif

            //begin load asset
            InsertAssetBundleLoadAssetOperation(download);

            downloadingBundles.Remove(req.key);
            download.ReleaseToPool();

            if (isError)
                CallOnAssetBundleErr(req);
            else
                CallOnAssetBundleComplete(req, ab);

            if (req.isShared) req.ReleaseToPool();

        }

        /// <summary>
        /// finish asset or http request
        /// </summary>
        /// <param name="operation"></param>
        internal static void ProcessFinishedOperation(ResourcesLoadOperation operation)
        {
            var req = operation.cRequest;
            bool isError = false;
            AssetBundleLoadAssetOperation assetLoad = operation as AssetBundleLoadAssetOperation;
            HttpLoadOperation httpLoad;
            if (assetLoad != null)
            {
                loadingTasks.Remove(req);
                isError = assetLoad.error != null;
                SetCacheAsset(req);// set asset cache
                DispatchAssetBundleLoadAssetOperation(req, isError);
                assetLoad.ReleaseToPool();//relase AssetBundleLoadAssetOperation

                CheckAllComplete();//check all complete 
            }
            else if ((httpLoad = operation as HttpLoadOperation) != null)
            {
                isError = !string.IsNullOrEmpty(httpLoad.error);

                if (isError && CUtils.IsResolveHostError(httpLoad.error) && !CUtils.IsHttps(req.url))// http dns 
                {
                    httpLoad.error = string.Format("dns resolve error url={0} ",req.url);
                    var httpDnsOp = HttpDnsResolve.Get();
                    httpDnsOp.SetRequest(req);
                    httpDnsOp.SetOriginalOperation(httpLoad);
                    inProgressOperations.Add(httpDnsOp);
                }
                else
                {
                    httpLoad.ReleaseToPool();

                    if (isError)
                        req.DispatchEnd();
                    else
                        req.DispatchComplete();

                    if (req.group != null) req.group.Complete(req, isError);

                    req.ReleaseToPool();
                }
            }else
            {
                operation.ReleaseToPool();
            }

        }

        #endregion

        #region mono

#if HUGULA_LOADER_DEBUG
        [SLua.DoNotToLua]
        public string LoadCountInfo;

        [SLua.DoNotToLua]
        public string DebugInfo;

        private System.Text.StringBuilder DebugSB = new System.Text.StringBuilder();
#endif

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            loadingEvent = new LoadingEventArg();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            watch.Reset();
            watch.Start();
   
            //load bundle
            for (int i = 0; i < inProgressBundleOperations.Count; )
            {
                var operation = inProgressBundleOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    inProgressBundleOperations.RemoveAt(i);
                    ProcessFinishedBundleOperation(operation);
                    //check frame time
                    // if (watch.ElapsedMilliseconds >= BundleLoadBreakMilliSeconds * 3f)
                    // {
                    //     return;
                    // }
                    // else if (watch.ElapsedMilliseconds >= BundleLoadBreakMilliSeconds * 2)
                    //     break;
                }
            }

            //load  asset or http req
            for (int i = 0; i < inProgressOperations.Count; )
            {
                var operation = inProgressOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    inProgressOperations.RemoveAt(i);
                    ProcessFinishedOperation(operation);
                }

                if (watch.ElapsedMilliseconds >= BundleLoadBreakMilliSeconds * 3f)
                    break;
            }

            //begin load bunld
            if (inProgressBundleOperations.Count == 0)
                LoadingQueue();

#if HUGULA_LOADER_DEBUG
            LoadCountInfo = string.Format("wait={0},bundleLoading={1},assetLoad={2}\r\n", waitRequest.Count, inProgressBundleOperations.Count, inProgressOperations.Count);
            DebugSB.Length = 0;
            DebugSB.Append(LoadCountInfo);
            for (int i = 0; i < inProgressBundleOperations.Count; i++)
            {
                var operation = inProgressBundleOperations[i];
                var req = operation.cRequest;
                DebugSB.AppendFormat("  CRequest({0},{1})  assetbundle  operation={2} loading.\r\n", req.key, req.assetName,operation);
                var denps = req.dependencies;
                if (denps != null)
                {
                    CacheData cache = null;
                    int hash = 0;
                    for (int j = 0; j < denps.Length; j++)
                    {
                        hash = denps[j];
                        if ((cache = CacheManager.TryGetCache(hash)) != null)
                        {
                            if (!cache.isDone) // if (!cache.isAssetLoaded || !cache.canUse)
                                DebugSB.AppendFormat("\tdenpens({0},{1}) is not done!\r\n", cache.assetBundleKey, cache.assetHashCode);
                        }
                        else
                        {
                            DebugSB.AppendFormat("\tdenpens({0},{1}) is not exists!\r\n", cache.assetBundleKey, cache.assetHashCode);
                        }
                    }
                }
            }

            //
            for (int i = 0; i < inProgressOperations.Count; i++)
            {
                var operation = inProgressOperations[i];
                var req = operation.cRequest;
                DebugSB.AppendFormat("CRequest({0},{1}) asset operation={2} loading.\r\n", req.key, req.assetName,operation);
            }   
            DebugInfo = DebugSB.ToString();
            if(Time.frameCount % 60 == 0)
                Debug.LogFormat("LOADER_DEBUG_info : {0}, frameCount={1}",DebugInfo,Time.frameCount);
#endif
        }

        /// <summary>
        /// auto clear assetbundle
        /// </summary>
        void LateUpdate()
        {
            ABDelayUnloadManager.Update();
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            waitRequest.Clear();
            loadingTasks.Clear();
            inProgressOperations.Clear();
            inProgressBundleOperations.Clear();
            downloadingBundles.Clear();
            assetCallBackList.Clear();

            OnAllComplete = null;
            OnProgress = null;

            OnAssetBundleComplete = null;
            OnAssetBundleErr = null;
        }

        #endregion

        #region event

        public static System.Action OnAllComplete;
        public static System.Action<LoadingEventArg> OnProgress;

        public static System.Action<CRequest, AssetBundle> OnAssetBundleComplete;
        public static System.Action<CRequest> OnAssetBundleErr;
        #endregion

        #region static

        protected static ResourcesLoader _instance;

        public static ResourcesLoader instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("AssetBundleLoader");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<ResourcesLoader>();
                }
                return _instance;
            }
        }
        public static void Initialize()
        {
            var ins = instance;
        }
        #endregion

    }

}