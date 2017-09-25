using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
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
        static public int maxLoading = 3;

        /// <summary>
        /// 最大同时加载assetbundle数量
        /// </summary>
        static public int bundleMax = 5;

        /// <summary>
        /// 加载bundle耗时跳出判断时间
        /// </summary>
        static public float BundleLoadBreakMilliSeconds = 25;


        //total count
        static private int totalCount = 0;
        //current loaded
        static private int currLoaded = 0;

        static private System.DateTime frameBegin = System.DateTime.Now;

        //protected
        static protected LoadingEventArg loadingEvent;

        //bundle queue
        static protected Queue<AssetBundleDownloadOperation> bundleQueue = new Queue<AssetBundleDownloadOperation>();
        //BundleGroundQueue
        static protected LinkedList<BundleGroundQueue> bundleGroundQueue = new LinkedList<BundleGroundQueue>();
        //loading group
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
            groupQueue.priority = priority;
            groupQueue.onComplete = groupCompleteFn;
            groupQueue.onProgress = groupProgressFn;

            // 
            foreach (var req in reqs)
            {
                groupQueue.Enqueue((CRequest)req.value);
            }

            LoadGroupAsset(groupQueue);
        }

        /// <summary>
        /// load group request
        /// </summary>
        /// <param name="bGroup"></param>
        static public void LoadGroupAsset(BundleGroundQueue bGroup)
        {
            if (bGroup.Count == 0)
            {
                if (bGroup.onComplete != null) bGroup.onComplete(false);
                return;
            }

            bool flag = false;
            int priority = bGroup.priority;
            for (LinkedListNode<BundleGroundQueue> fristNode = bundleGroundQueue.First; fristNode != null; fristNode = fristNode.Next)
            {
                if (fristNode.Value.priority == priority)
                {
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat("LoadGroupAsset", "<color=#05AA01>0.0 AddAfter BundleGroundQueue(Count={0}priority={1}) frameCount{2}</color>", bGroup.Count, bGroup.priority, Time.frameCount);
#endif
                    bundleGroundQueue.AddAfter(fristNode, bGroup);
                    flag = true;
                    break;
                }
                if (fristNode.Value.priority < priority)
                {
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat("LoadGroupAsset", "<color=#05AA01>0.0 AddBefore BundleGroundQueue(Count={0}priority={1}) frameCount{2}</color>", bGroup.Count, bGroup.priority, Time.frameCount);
#endif
                    bundleGroundQueue.AddBefore(fristNode, bGroup);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat("LoadGroupAsset", "<color=#05AA01>0.0 AddLast BundleGroundQueue(Count={0}priority={1}) frameCount{2}</color>", bGroup.Count, bGroup.priority, Time.frameCount);
#endif
                bundleGroundQueue.AddLast(bGroup);
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
            req.relativeUrl = abName;
            return LoadAsset(req, true);
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
            req.relativeUrl = abName;
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
        static public AssetBundleLoadAssetOperation LoadAsset(CRequest req, bool coroutine = false)
        {
            AssetBundleLoadAssetOperation op = null;
            var groupQueue = BundleGroundQueue.Get();
            groupQueue.priority = req.priority;
            groupQueue.Enqueue(req);
            if (coroutine)
            {
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
            }
            LoadGroupAsset(groupQueue);
            return op;
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public HttpLoadOperation HttpRequestCoroutine(string url, object head, System.Type type)
        {
            var req = CRequest.Get();
            req.relativeUrl = url;
            req.head = head;
            req.assetType = type;
            return HttpRequest(req, true);
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        static public void HttpRequest(string url, object head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd, UriGroup uris)
        {
            var req = CRequest.Get();
            req.relativeUrl = url;
            req.head = head;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            req.uris = uris;
            HttpRequest(req);
        }

        /// <summary>
        /// 以 webreuest 方式加载网络非ab资源
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public HttpLoadOperation HttpRequest(CRequest req, bool coroutine = false)
        {
            WebRequestOperation op = null;
            if (coroutine)
            {
                op = new WebRequestOperation();
                req.assetOperation = op;
            }
            else
                op = WebRequestOperation.Get();
            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
            return op;
        }

        /// <summary>
        /// 以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        static public void WWWRequest(string url, object head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd, UriGroup uris)
        {
            var req = CRequest.Get();
            req.relativeUrl = url;
            req.head = head;
            req.assetType = type;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            req.uris = uris;
            WWWRequest(req);
        }

        /// <summary>
        ///  以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="head"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public HttpLoadOperation WWWRequestCoroutine(string url, object head, System.Type type)
        {
            var req = CRequest.Get();
            req.relativeUrl = url;
            req.head = head;
            req.assetType = type;
            return WWWRequest(req, true);
        }

        /// <summary>
        ///  以 WWW 方式加载网络非ab资源
        /// </summary>
        /// <param name="req"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        static public HttpLoadOperation WWWRequest(CRequest req, bool coroutine = false)
        {
            WWWRequestOperation op = null;
            if (coroutine)
            {
                op = new WWWRequestOperation();
                req.assetOperation = op;
            }
            else
                op = WWWRequestOperation.Get();

            op.SetRequest(req);
            inProgressOperations.Add(op);
            op.BeginDownload();
            return op;
        }

        #endregion

        #region load logic
        /// <summary>
        /// check the queue and load assetbundle and asset
        /// </summary>
        static protected bool LoadingQueue()
        {
            if (inProgressBundleOperations.Count > 0) return false; //wait bundle load

            LinkedListNode<BundleGroundQueue> fristNode = bundleGroundQueue.First;

            while (fristNode != null && maxLoading - loadingTasks.Count > 0)
            {
                BundleGroundQueue value = fristNode.Value;
                if (value.Count > 0)
                {
                    var req = value.Dequeue();
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat("LoadingQueue", "<color=#05AA01>0.1 LoadAssetBundle Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
                    LoadAssetBundle(req);
                }
                else
                {
                    fristNode = fristNode.Next;
                    bundleGroundQueue.Remove(value);
                }

                var ts = System.DateTime.Now - frameBegin;
                if (ts.TotalMilliseconds > BundleLoadBreakMilliSeconds) return true;
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
                    // Debug.LogFormat("LoadAssetFromCache Req(assetBundleName={0},assetName={1},data={2}); url={3}", req.assetBundleName, req.assetName, asset, req.url);
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
            //Variants
            string relativeUrl = ManifestManager.RemapVariantName(req.relativeUrl);
            req.relativeUrl = relativeUrl;
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
                else if (CacheManager.Typeof_ABScene.Equals(tp1))
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

            //check load asset from cache
            if (LoadAssetFromCache(req))
            {
                DispatchReqAssetOperation(req, false);
                return;
            }

            totalCount++;//count ++
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("LoadAssetBundle ({0},{1},{2}) LoadDependencies and LoadAssetBundleInternal", req.assetName, req.key, req.isShared));
#endif
            AssetBundleDownloadOperation abDownloadOperation = null;

            if (downloadingBundles.TryGetValue(req.key, out abDownloadOperation)) //check is loading
            {

            }
            else if (CheckAssetBundleCanLoad(req)) //need load
            {
                //load dependencies and refrenece count
                if (ManifestManager.fileManifest != null)
                    req.dependencies = LoadDependencies(req,null); //load dependencies assetbundle
                //load assetbundle
                abDownloadOperation = LoadAssetBundleInternal(req);
            }
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif

#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("LoadAssetBundle  ({0},{1},{2}) to inProgressOperations", req.assetName, req.key, req.isShared));
#endif
            //load asset
            ResourcesLoadOperation operation;
            var tp = req.assetType;

            if (req.assetOperation != null)
            {
                operation = req.assetOperation;
            }
            else if (CacheManager.Typeof_ABScene.Equals(tp))
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
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(req.key, "<color=#15A0A1>2.0 LoadAssetBundle Asset Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
        }

        /// <summary>
        /// load LoadDependencies assetbundle
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        static protected int[] LoadDependencies(CRequest req,CRequest parent)
        {
            string[] deps = ManifestManager.fileManifest.GetDirectDependencies(req.assetBundleName);
            if (deps.Length == 0) return null;
            string abName = string.Empty;
            if(parent!=null) abName = CUtils.GetBaseName(parent.assetBundleName);

#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("LoadDependencies ({0},{1},{2}) new int[deps.Length]", req.assetName, req.key,req.isShared));
#endif
            string dep_url;
            string depAbName = "";
            CRequest item;
            int[] hashs = new int[deps.Length];
            int keyhash;
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
            for (int i = 0; i < deps.Length; i++)
            {
                depAbName = CUtils.GetBaseName(deps[i]);
                if(abName==depAbName)
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("Dependencies({1}) Contains the parent({0}) ! ",req.assetBundleName,abName);
#else
                    Debug.LogWarningFormat("Dependencies({1}) Contains the parent({0}) ! ",req.assetBundleName,abName);
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
                    int count = 0;
#if HUGULA_CACHE_DEBUG
                    count = CountMananger.WillAdd(keyhash); //引用数量加1
                    HugulaDebug.FilterLogFormat(req.key + "," + dep_url, " <color=#fcfcfc> will add  (assetBundle={0},hash={1},count={2}) frameCount{3}</color>", dep_url, keyhash, count, UnityEngine.Time.frameCount);
#else
                    count = CountMananger.WillAdd(keyhash); //引用数量加1
#endif
                    bool dependenciesRefer = count == 1;//the cache count == 0 
                    if (dependenciesRefer)
                        ABDelayUnloadManager.AddDependenciesReferCount(sharedCD, dependenciesRefer);
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(req.key, " <color=#15A0A1> 1.1 Shared  AssetBundle is Done Request(assetName={0})  Parent Req(assetName={1},key={2},isShared={3}) </color>", sharedCD.assetBundleKey, req.assetName, req.key, req.isShared);
#endif
                }
                else
                {
#if HUGULA_CACHE_DEBUG
                    int count = CountMananger.WillAdd(keyhash); //引用数量加1
                    HugulaDebug.FilterLogFormat(req.key + "," + dep_url, " <color=#fcfcfc> will add  (assetBundle={0},hash={1},count={2}) frameCount{3}</color>", dep_url, keyhash, count, UnityEngine.Time.frameCount);
#else

                    CountMananger.WillAdd(keyhash); //引用数量加1
#endif
                    item = CRequest.Get();
                    item.relativeUrl = dep_url;
                    item.isShared = true;
                    item.async = req.async;
                    item.priority = req.priority;
                    item.dependencies = LoadDependencies(item,req);
                    item.uris = req.uris;
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
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(string.Format("LoadAssetBundleInternal ({0},{1},{2})", req.assetName, req.key,req.isShared));
#endif
                if (!UriGroup.CheckRequestCurrentIndexCrc(req)) //crc 
                {
                    abDownloadOp = new AssetBundleDownloadErrorOperation();
                    abDownloadOp.error = string.Format("assetbundle({0}) crc check wrong ", req.key);
                }
                else if (req.url.StartsWith(Common.HTTP_STRING))  //load assetbunlde
                {
                    abDownloadOp = AssetBundleDownloadFromWebOperation.Get();
                }
                else
                {
                    abDownloadOp = AssetBundleDownloadFromDiskOperation.Get();

                }
                abDownloadOp.SetRequest(req);
                bundleQueue.Enqueue(abDownloadOp);
                downloadingBundles.Add(req.key, abDownloadOp);
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.2 LoadAssetBundleInternal Request(key={0},isShared={1},assetname={2},dependencies.count={4})keyHashCode{3}, frameCount{5}</color>", req.key, req.isShared, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif

#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
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
            Debug.LogFormat("<color=#0a9a0a>asset complete Request(asset={0},key={1}) alltime={2},loadtime={3};frame={4}</color>", req.assetName, req.key, dt1.TotalSeconds, dt2.TotalSeconds,Time.frameCount);
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
            Debug.LogFormat("<color=#0a9a0a>{0} Request(asset={1},key={2},size={3}) alltime={4}s,loadtime={5}s,frame={6}</color>", tips, req.assetName, req.key, size, dt1.TotalSeconds, dt2.TotalSeconds,Time.frameCount);
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
            Profiler.BeginSample(string.Format("ResourcesLoader.ProcessFinishedBundleOperation CRequest({0},shared={1})", operation.cRequest.assetName,operation.cRequest.isShared));
#endif
            var req = operation.cRequest;
            bool isError = operation.assetBundle == null;
            var ab = operation.assetBundle;
            AssetBundleDownloadOperation download = operation;

            if (isError && UriGroup.CheckAndSetNextUriGroup(req))
            {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.9  ProcessFinishedBundleOperation AssetBundle re Loaded Request(url={0},assetname={1},dependencies.count={3},keyHashCode{2}),isError={4} frameCount{5}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, isError, Time.frameCount);
#endif
                //loop load a assetbundle maybe cause a crash :  signal 6 (SIGABRT), code -6 (?), fault addr --------
                inProgressBundleOperations.Add(download);
                download.Reset();
                download.SetRequest(req);
                download.BeginDownload(); //retry
            }
            else
            {
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
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
        }

        /// <summary>
        /// finish asset or http request
        /// </summary>
        /// <param name="operation"></param>
        static void ProcessFinishedOperation(ResourcesLoadOperation operation)
        {
            var req = operation.cRequest;
            bool isError = false;
            AssetBundleLoadAssetOperation assetLoad = operation as AssetBundleLoadAssetOperation;
            HttpLoadOperation httpLoad;
            if (assetLoad != null)
            {
                loadingTasks.Remove(req);
                isError = assetLoad.error != null;
                operation.ReleaseToPool();//relase AssetBundleLoadAssetOperation
                SetCacheAsset(req);// set asset cache
                DispatchAssetBundleLoadAssetOperation(req, isError);

                //等两帧再CheckAllComplete
                allCompleteCheckCount = 0.5f;
                //CheckAllComplete();//check all complete 
            }
            else if ((httpLoad = operation as HttpLoadOperation) != null)
            {
                isError = !string.IsNullOrEmpty(httpLoad.error);

                if (isError && UriGroup.CheckAndSetNextUriGroup(req)) //
                {
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.9  ProcessFinishedOperation re Loaded Request(url={0},assetname={1},dependencies.count={3},keyHashCode{2}),isError={4} frameCount{5}</color>", req.url, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, isError, Time.frameCount);
#endif
                    // Debug.LogFormat(" re try {0};",req.url);
                    inProgressOperations.Add(httpLoad);
                    httpLoad.Reset();
                    httpLoad.SetRequest(req);
                    httpLoad.BeginDownload(); //retry
                }
                else
                {
                    operation.ReleaseToPool();

                    if (isError)
                        req.DispatchEnd();
                    else
                        req.DispatchComplete();

                    if (req.group != null) req.group.Complete(req, isError);

                    req.ReleaseToPool();
                }
            }

        }

        #endregion

        #region mono

#if UNITY_EDITOR  && HUGULA_LOADER_DEBUG
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
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample("ResourcesLoader.LoadingQueue");
#endif
            frameBegin = System.DateTime.Now;
            LoadingQueue();
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif

#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample("ResourcesLoader.LoadBundle" + inProgressBundleOperations.Count);
#endif
            //add bunlde to inProgressOperations
            while (bundleMax - inProgressBundleOperations.Count > 0 && bundleQueue.Count > 0)
            {
                //check frame time
                var ts = (System.DateTime.Now - frameBegin).TotalMilliseconds;
                if (ts >= BundleLoadBreakMilliSeconds * 3f)
                {
#if HUGULA_PROFILER_DEBUG
                    Profiler.EndSample();
#endif
                    return;
                }
                else if (ts >= BundleLoadBreakMilliSeconds * 1.5f)
                {
                    break;
                }

                var bundleOper = bundleQueue.Dequeue();
                inProgressBundleOperations.Add(bundleOper);
#if HUGULA_PROFILER_DEBUG
                var req = bundleOper.cRequest;
                req.beginLoadTime = System.DateTime.Now;
#endif
                bundleOper.BeginDownload();

            }

#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif


#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample("inProgressBundleOperations.Loop count=" + inProgressBundleOperations.Count);
#endif

            //load bundle
            for (int i = 0; i < inProgressBundleOperations.Count;)
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
                    var ts = (System.DateTime.Now - frameBegin).TotalMilliseconds;
                    if (ts >= BundleLoadBreakMilliSeconds * 3f)
                    {
#if HUGULA_PROFILER_DEBUG
                        Profiler.EndSample();
#endif
                        return;
                    }
                    else if (ts >= BundleLoadBreakMilliSeconds * 2)
                        break;
                }
            }
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif

#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample("ResourcesLoader.inProgressOperations" + inProgressOperations.Count);
#endif
            //load  asset or http req
            for (int i = 0; i < inProgressOperations.Count;)
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

                var ts = System.DateTime.Now - frameBegin;
                if (ts.TotalMilliseconds >= BundleLoadBreakMilliSeconds * 3f)
                    break;
            }
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif

#if UNITY_EDITOR && HUGULA_LOADER_DEBUG
            LoadCountInfo = string.Format("wait={0},bundleLoading={1},assetLoad={2}", bundleQueue.Count, inProgressBundleOperations.Count, inProgressOperations.Count);
            DebugSB.Length = 0;
            for (int i = 0; i < inProgressBundleOperations.Count; i++)
            {
                var operation = inProgressBundleOperations[i].cRequest;
                DebugSB.AppendFormat("CRequest({0},{1}) is loading.\r\n", operation.key, operation.assetName);
                var denps = operation.dependencies;
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
            DebugInfo = DebugSB.ToString();
#endif
        }

        /// <summary>
        /// auto clear assetbundle
        /// </summary>
        void LateUpdate()
        {
            ABDelayUnloadManager.Update();
            if (allCompleteCheckCount > 0)
            {
                allCompleteCheckCount -= Time.deltaTime;
                if (allCompleteCheckCount <= 0)
                    CheckAllComplete();//check all complete 
            }
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            bundleQueue.Clear();
            bundleGroundQueue.Clear();
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

        static float allCompleteCheckCount = 0;
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