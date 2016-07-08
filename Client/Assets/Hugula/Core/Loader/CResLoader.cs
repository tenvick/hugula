using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 新的资源加载类
/// </summary>
[SLua.CustomLuaClass]
public class CResLoader : MonoBehaviour
{
    #region public static
    /// <summary>
    /// 最大同时加载数量
    /// </summary>
    public static int maxLoading = 2;

    /// <summary>
    /// 即将加载的总数
    /// </summary>
    public static int totalLoading { private set; get; }

    /// <summary>
    /// 当前同事正在加载数量
    /// </summary>
    public static int currentLoading
    {
        get
        {
            int count = 0;
            for (int i = 0; i < loaderPool.Count; i++)
            {
                if (!loaderPool[i].isFree)
                    count++;
            }
            return count;
        }
    }

    /// <summary>
    /// 当前加载完成的数量
    /// </summary>
    public static int currentLoaded;

    /// <summary>
    /// 将接下来的所有添加的加载请求放入一组，完成后触发on_allComplete事件
    /// </summary>
    public static bool pushGroup = false;
    #endregion

    #region static member
    /// <summary>
    /// 所有请求队列
    /// </summary>
    static protected CQueueRequest queue = new CQueueRequest();

    /// <summary>
    /// 异步加载队列
    /// </summary>
    static protected List<CRequest> loadingAssetBundleQueue = new List<CRequest>();

    ///<summary>
    ///异步加载队列
    ///</summary>
    static protected List<CRequest> loadingAssetQueue = new List<CRequest>();

    /// <summary>
    /// 正在下载Loader
    /// </summary>
    static Dictionary<string, bool> downloadings = new Dictionary<string, bool>();

    /// <summary>
    /// 即将开始下载的队列
    /// </summary>
    static Queue<CRequest> realyLoadingQueue = new Queue<CRequest>();

    /// <summary>
    /// 回调列表
    /// </summary>
    static Dictionary<string, List<CRequest>> requestCallBackList = new Dictionary<string, List<CRequest>>();

    /// <summary>
    /// 加载缓存池
    /// </summary>
    static List<CCar> loaderPool = new List<CCar>();

    /// <summary>
    /// 加载完成解压回调队列
    /// </summary>
    static Queue<CRequest> loadedAssetQueue = new Queue<CRequest>();

    /// <summary>
    /// 组回调
    /// </summary>
    static HashSet<CRequest> currGroupRequests;
    static HashSet<CRequest> currGroupRequestsRef = new HashSet<CRequest>();

    /// <summary>
    /// 引用计数记录
    /// </summary>
    static Dictionary<int, int> referenceRecord = new Dictionary<int, int>();
    #endregion

    #region mono
    protected LoadingEventArg loadingEvent;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        loadingEvent = new LoadingEventArg();
        CreateFreeLoader();
    }

    // Update is called once per frame
    void Update()
    {
        //加载
        for (int i = 0; i < loaderPool.Count; i++)
        {
            CCar load = loaderPool[i];
            //Debug.LogFormat(" <color=green>chekc loading  isfree {0} count{1}</color>", load.isFree, realyLoadingQueue.Count);
            if (load.isFree && realyLoadingQueue.Count > 0)
            {
                var req = realyLoadingQueue.Dequeue();
                if (!CheckLoadAssetBundle(req))
                {
                    downloadings[req.udKey] = true;
                    load.BeginLoad(req);
                }
            }
            if (load.enabled)
            {
                //Debug.LogFormat(" <color=green>loading  enabled {0} isFree{1}</color>", load.enabled, load.isFree);
                load.Update();
            }
        }

        //ab
        for (int i = 0; i < loadingAssetBundleQueue.Count; )
        {
            var item = loadingAssetBundleQueue[i];
            if (CacheManager.CheckDependenciesComplete(item))//判断依赖项目是否加载完成
            {
                CacheManager.SetRequestDataFromCache(item);//设置缓存数据。
                loadingAssetQueue.Add(item);
                if (item.assetBundleRequest != null) CacheManager.AddLock(item.keyHashCode);//异步需要锁定
                loadingAssetBundleQueue.RemoveAt(i);
            }
            else
                i++;
        }

        //asset
        for (int i = 0; i < loadingAssetQueue.Count; i++)
        {
            var item = loadingAssetQueue[i];
            //if (item.assetBundleRequest != null) Debug.LogFormat("key:{0} loading {1} frameCount{2}", item.key, item.assetBundleRequest.progress, Time.frameCount);
            if (item.assetBundleRequest != null && item.assetBundleRequest.isDone) //如果加载完成
            {
                item.data = item.assetBundleRequest.asset;//赋值
                //Debug.LogFormat("end key:{0}  data:{1} loadcomplete  frameCount{2}", item.key, item.data, Time.frameCount);
                loadedAssetQueue.Enqueue(item);
                loadingAssetQueue.RemoveAt(i);
            }
            else if (item.assetBundleRequest == null) //非异步
            {
                loadedAssetQueue.Enqueue(item);
                loadingAssetQueue.RemoveAt(i);
                //Debug.LogFormat(" non Async end key:{0}  data:{1} loadcomplete  frameCount{2}", item.key, item.data, Time.frameCount);
            }
            else
                i++;
        }

        while (loadedAssetQueue.Count > 0)
        {
            //Debug.LogFormat("loadedAsyncQueue.Count:{0}", loadedAssetQueue.Count);
            LoadAssetComplate(loadedAssetQueue.Dequeue());
        }
    }

    #endregion

    #region static load

    /// <summary>
    /// 将多个资源加载到本地并缓存。
    /// </summary>
    /// <param name="req"></param>
    public void LoadReq(IList<CRequest> req)//onAllCompleteHandle onAllCompletehandle=null,onProgressHandle onProgresshandle=null
    {
        pushGroup = true;

        for (int i = 0; i < req.Count; i++)
            AddReqToQueue(req[i]);

        pushGroup = false;

        BeginQueue();
    }

    /// <summary>
    /// 将多个资源加载到本地并缓存。
    /// </summary>
    /// <param name="req"></param>
    public void LoadReq(CRequest req)
    {
        AddReqToQueue(req);
        BeginQueue();
    }

    protected static bool AddReqToQueue(CRequest req)
    {
        if (req == null) return false;
        string key = req.udKey;

        if (CheckLoadAssetAsync(req)) //已经下载
        {
            return false;
        }
        else if (requestCallBackList.ContainsKey(key)) //回调列表
        {
            requestCallBackList[key].Add(req);
            totalLoading++;
            if (pushGroup) PushGroup(req);
            return true;
        }
        else
        {
            var listreqs = new List<CRequest>();
            requestCallBackList[key] = listreqs;
            listreqs.Add(req);

            if (queue.Size() == 0 && currentLoading == 0)
            {
                totalLoading = 1;
                currentLoaded = 0;
            }
            queue.Add(req);
            totalLoading++;
            if (pushGroup) PushGroup(req);

            return true;

        }
    }

    protected static void BeginQueue()
    {
        if (queue.Size() > 0 && currentLoading < maxLoading)
        {
            var cur = queue.First();
            LoadAssetBundle(cur);
        }
    }

    protected static void CallbackAsyncList(CRequest creq)
    {
        List<CRequest> callbacklist = null;// requestCallBackList[creq.udKey];
        string udkey = creq.udKey;
        if (requestCallBackList.TryGetValue(udkey, out callbacklist))
        {
            //Debug.LogFormat("TryGetValue {0}  true  {1} ", creq.key, creq.udKey);
            requestCallBackList.Remove(creq.udKey);
            int count = callbacklist.Count;
            CRequest reqitem;
            for (int i = 0; i < count; i++)
            {
                reqitem = callbacklist[i];
                //CacheManager.SetRequestDataFromCache(reqitem);
                loadingAssetBundleQueue.Add(reqitem);
                //Debug.LogFormat("creq.key {0}  loadingAsyncQueue  {1} data:({2})", creq.key, loadingAssetQueue.Count, reqitem.data);
            }
            callbacklist.Clear();
            requestCallBackList.Remove(udkey);
        }
        else
        {
            loadingAssetBundleQueue.Add(creq);
            //Debug.LogFormat(" no list creq.key {0}  loadingAsyncQueue  {1} ", creq.key, loadingAssetQueue.Count);

        }
    }

    protected static void CallbackError(CRequest creq)
    {
        List<CRequest> callbacklist = null;// requestCallBackList[creq.udKey];
        string udkey = creq.udKey;
        if (requestCallBackList.TryGetValue(udkey, out callbacklist))
        {
            requestCallBackList.Remove(creq.udKey);
            int count = callbacklist.Count;
            CRequest reqitem;
            for (int i = 0; i < count; i++)
            {
                reqitem = callbacklist[i];
                reqitem.DispatchEnd();
                currentLoaded++;
                PopGroup(reqitem);
            }
            //ClearNoABCache(creq);
            callbacklist.Clear();
            requestCallBackList.Remove(udkey);
        }

        CheckAllComplete();
    }

    protected static void CheckAllComplete()
    {
        if (currGroupRequests != null && currGroupRequests.Count == 0)
        {
            if (_instance && _instance.OnGroupComplete != null)
                _instance.OnGroupComplete(_instance);

            currGroupRequests = null;
        }

        if (currentLoading <= 0 && queue.Size() == 0)
        {
            var loadingEvent = _instance.loadingEvent;
            loadingEvent.target = _instance;
            loadingEvent.total = totalLoading;
            loadingEvent.progress = 1;
            loadingEvent.current = currentLoaded;
            if (_instance.OnProgress != null && totalLoading > 0)
            {
                _instance.OnProgress(_instance, loadingEvent);
            }

            if (_instance.OnAllComplete != null)
                _instance.OnAllComplete(_instance);

            //Debug.Log("AllComplete"+currentLoading);
        }
    }

    protected static void LoadAssetComplate(CRequest req)
    {
        try
        {
            req.DispatchComplete();
        }
        catch { }
        
        currentLoaded++;
        PopGroup(req);
        ClearNoABCache(req);
        BeginQueue();
        CheckAllComplete();
    }

    /// <summary>
    /// 判断异步加载Asset
    /// </summary>
    /// <param name="req"></param>
    static bool CheckLoadAssetAsync(CRequest req)
    {
        if (CacheManager.SetRequestDataFromCache(req))
        {
            if (req.assetBundleRequest != null)
            {
                CacheManager.AddLock(req.keyHashCode);
                loadingAssetQueue.Add(req);
                //Debug.LogFormat("loadingAsyncQueue.Add key:{0} ", req.key);
            }
            else
            {
                //Debug.LogFormat("req.assetBundleRequest != null  key:{0}", req.key);
                LoadAssetComplate(req);
            }
            return true;
        }

        return false;
    }

    static void PushGroup(CRequest req)
    {
        if (currGroupRequests == null) currGroupRequests = currGroupRequestsRef;
        currGroupRequests.Add(req);
    }

    static void PopGroup(CRequest req)
    {
        if (currGroupRequests != null)
            currGroupRequests.Remove(req);
    }

    /// <summary>
    /// 清理没有ab的缓存
    /// </summary>
    /// <param name="req"></param>
    static void ClearNoABCache(CRequest req)
    {
        if (req.assetBundleRequest!=null) CacheManager.RemoveLock(req.keyHashCode);

        if (req.clearCacheOnComplete)
        {
#if UNITY_EDITOR
            Debug.LogFormat("<color=#8cacbc>{0} ClearCache</color>", req.key);
#endif
            CacheManager.ClearCache(req.keyHashCode);
        }
    }

    /// <summary>
    /// 添加引用计数
    /// </summary>
    /// <param name="keyhash"></param>
    static void AddReference(int keyhash)
    {
        if (referenceRecord.ContainsKey(keyhash))
        {
            referenceRecord[keyhash]++;
        }
        else
        {
            referenceRecord[keyhash] = 1;
        }
    }

    /// <summary>
    /// 移除引用记录
    /// </summary>
    /// <param name="keyhash"></param>
    static void RemoveReferenceRecord(CRequest creq)
    {
        int keyhash = creq.keyHashCode;

        if (referenceRecord.ContainsKey(keyhash))
        {
            int refc = referenceRecord[keyhash];
            CountMananger.Add(keyhash, refc);//真实引用
            referenceRecord.Remove(keyhash);
        }
    }

    #endregion

    #region  eventhandle

    static void LoadComplete(CCar loader, CRequest creq)
    {
        //Debug.LogFormat(" <color=green>LoadComplete {0} {1}</color>", creq.key, creq.isShared);
        downloadings.Remove(creq.udKey);

        if (creq.isShared)
        {
            //添加引用计数
            RemoveReferenceRecord(creq);

            //回调
            if (_instance && _instance.OnSharedComplete != null)
                _instance.OnSharedComplete(creq);

        }
        else
        {
            CallbackAsyncList(creq);
        }
    }

    static void LoadError(CCar cloader, CRequest req)
    {
        downloadings.Remove(req.udKey);
        req.times++;
        if (req.times < 2)
        {
            //req.priority = req.priority - 10;
            realyLoadingQueue.Enqueue(req);
        }
        else
        {
            CallbackError(req);
            BeginQueue();
            CheckAllComplete();
        }
    }

    static void OnProcess(CCar loader, float progress)
    {
        var loadingEvent = _instance.loadingEvent;
        loadingEvent.target = _instance;
        if (totalLoading <= 0)
            loadingEvent.total = 1;
        else
            loadingEvent.total = totalLoading;
        if (loadingEvent.current < currentLoaded) loadingEvent.current = currentLoaded;
        loadingEvent.progress = progress;
        if (_instance && _instance.OnProgress != null && totalLoading > 0)
        {
            _instance.OnProgress(_instance, loadingEvent);
        }
    }

    #endregion

    #region static assetbundle

    /// <summary>
    /// check load assetbundle
    /// </summary>
    static protected void LoadAssetBundle(CRequest req)
    {
        if (assetBundleManifest != null)
            LoadDependencies(req); //加载依赖

        realyLoadingQueue.Enqueue(req);//加载自己
        //Debug.LogFormat(" <color=yellow>realyLoadingQueue {0} {1}</color>", req.key, realyLoadingQueue.Count);

    }

    /// <summary>
    /// 加载依赖项目
    /// </summary>
    /// <param name="req"></param>
    static protected void LoadDependencies(CRequest req)
    {
        string[] deps = assetBundleManifest.GetAllDependencies(req.assetBundleName);

        if (deps.Length == 0) return;

        string dep_url;
        CRequest item;
        int[] hashs = new int[deps.Length];

        for (int i = 0; i < deps.Length; i++)
        {
            dep_url = CUtils.GetAssetFullPath(RemapVariantName(deps[i])); ;
            int keyhash = LuaHelper.StringToHash(CUtils.GetKeyURLFileName(dep_url));
            hashs[i] = keyhash;

            if (CacheManager.Contains(keyhash))
            {
                CountMananger.Add(keyhash); //引用数量加1
            }
            else if (downloadings.ContainsKey(CryptographHelper.CrypfString(dep_url))) //loading
            {
                AddReference(keyhash);
            }
            else
            {
                item = new CRequest(dep_url);
                item.isShared = true;
                item.async = false;
                realyLoadingQueue.Enqueue(item);
                AddReference(keyhash);
                //Debug.LogFormat("loading denpendeny {0}，key({1})",dep_url,req.key);
            }
        }

        req.allDependencies = hashs; //记录依赖项目
    }

    /// <summary>
    /// 判断加载
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    static protected bool CheckLoadAssetBundle(CRequest req)
    {
        if (CacheManager.Contains(req.keyHashCode)) return true;

        if (downloadings.ContainsKey(req.udKey)) return true;


        return false;
    }

    /// <summary>
    /// ab manifest
    /// </summary>
    public static AssetBundleManifest assetBundleManifest;

    static string[] m_Variants = { };

    /// <summary>
    ///  Variants which is used to define the active variants.
    /// </summary>
    public static string[] Variants
    {
        get { return m_Variants; }
        set { m_Variants = value; }
    }
    /// <summary>
    ///  Remaps the asset bundle name to the best fitting asset bundle variant.
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    public static string RemapVariantName(string assetBundleName)
    {
        string[] bundlesWithVariant = assetBundleManifest.GetAllDependencies(assetBundleName); //.GetAllAssetBundlesWithVariant();

        // If the asset bundle doesn't have variant, simply return.
        if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
            return assetBundleName;

        string[] split = assetBundleName.Split('.');

        int bestFit = int.MaxValue;
        int bestFitIndex = -1;
        // Loop all the assetBundles with variant to find the best fit variant assetBundle.
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;

            int found = System.Array.IndexOf(m_Variants, curSplit[1]);
            if (found != -1 && found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }

        if (bestFitIndex != -1)
            return bundlesWithVariant[bestFitIndex];
        else
            return assetBundleName;
    }

    #endregion

    #region static load pool

    /// <summary>
    /// 获取空闲的loader
    /// </summary>
    /// <returns></returns>
    public static void CreateFreeLoader()
    {
        for (int i = loaderPool.Count; i < maxLoading; i++)
        {
            CCar cts = new CCar();
            loaderPool.Add(cts);
            cts.OnComplete = LoadComplete;
            cts.OnError = LoadError;
            cts.OnProcess = OnProcess;
        }

    }

    #endregion

    #region  delegate and event

    public event System.Action<CResLoader> OnAllComplete;
    public event System.Action<CResLoader> OnGroupComplete;
    public event System.Action<CResLoader, LoadingEventArg> OnProgress;
    public event System.Action<CRequest> OnSharedComplete;
    #endregion

    #region instance
    protected static CResLoader _instance;
    ///// <summary>
    ///// the GetInstance
    ///// </summary>
    ///// <returns></returns>
    //public static CResLoader GetInstance()
    //{
    //    if (_instance == null)
    //    {
    //        var chighway = new GameObject("CResManager");
    //        _instance = chighway.AddComponent<CResLoader>();
    //    }
    //    return _instance;
    //}
    #endregion
}


[SLua.CustomLuaClass]
public class LoadingEventArg
{
    //public int number;//current loading number
    public object target;
    public int total;
    public int current;
    public float progress;
}