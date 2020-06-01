// Copyright (c) 2019 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;

namespace Hugula.Loader
{
    [XLua.LuaCallCSharp]
    public sealed class ResourcesLoader : MonoBehaviour
    {
#if UNITY_EDITOR
        const string kSimulateAssetBundles = "SimulateAssetBundles";
        static int m_SimulateAssetBundleInEditor = -1;
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
#if USE_BUNDLE
                return false;
#endif

                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }

#endif
        public delegate string OverrideBaseDownloadingURLDelegate(string bundleName);

        /// <summary>
        /// Implements per-bundle base downloading URL override.
        /// The subscribers must return null values for unknown bundle names;
        /// </summary>
        public static event OverrideBaseDownloadingURLDelegate overrideBaseDownloadingURL;
        public static System.Action OnAllComplete; //所有加载完成回调
        public static System.Action<LoadingEventArg> OnProgress; //当前进度
        static public int maxLoading = 5; // 最大同时加载请求asset数量
        // static ManifestManager manifest;

        //total count
        static private int totalCount = 0;
        //current loaded
        static private int currLoaded = 0;
        static LoadingEventArg loadingEvent; //事件进度
        static List<AssetOperation> inProgressOperations = new List<AssetOperation>(); //正在加载的资源asset列表

        static List<string> downloadingBundles = new List<string>(); // 正在加载的assetbundle name
        static List<BundleOperation> inBundleProgressOperations = new List<BundleOperation>(); //加载的assetbundle列表
        static HashSet<int> completeOper = new HashSet<int>(); //正常回调
        static Queue<AssetOperation> waitOperations = new Queue<AssetOperation>(); //等待队列

        #region  group load

        //当前组的所有加载队列
        static private List<CRequest> m_Groupes = new List<CRequest>();
        //标注为一个组加载
        static private bool m_MarkGroup = false;
        static private int m_TotalGroupCount = 0;
        static private int m_minTotalGroupCount = 0;
        static private int m_CurrGroupLoaded = 0;
        static public void BeginMarkGroup(int minTotalCount = 0)
        {
            m_MarkGroup = true;
            m_minTotalGroupCount = minTotalCount;
        }

        static public void EndMarkGroup()
        {
            m_MarkGroup = false;
        }

        public static System.Action OnGroupComplete; //group加载完成回调
        public static System.Action<LoadingEventArg> OnGroupProgress; //当前group进度

        #endregion

        /// <summary>
        /// 以同步方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        static public T LoadAsset<T>(string abName, string assetName)
        {
            var req = CRequest.Get();
            req.assetName = assetName;
            req.assetType = typeof(T);
            req.assetBundleName = abName;
            LoadAsset(req, false);
            T t = (T)req.data;
            req.ReleaseToPool();
            return t;
        }

        /// <summary>
        /// 以同步方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        static public UnityEngine.Object LoadAsset(string abName, string assetName, Type assetType)
        {
            var req = CRequest.Get();
            req.assetName = assetName;
            req.assetType = assetType;
            req.assetBundleName = abName;
            LoadAsset(req, false);
            UnityEngine.Object t = (UnityEngine.Object)req.data;
            req.ReleaseToPool();
            return t;
        }

        /// <summary>
        /// 以 Coroutine方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        static public CRequest LoadAssetCoroutine<T>(string abName, string assetName)
        {
            return LoadAssetCoroutine(abName, assetName, typeof(T));
        }

        /// <summary>
        /// 以 Coroutine方式加载ab资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        static public CRequest LoadAssetCoroutine(string abName, string assetName, Type assetType)
        {
            var req = new CRequest();
            req.assetName = assetName;
            req.assetType = assetType;
            req.assetBundleName = abName;
            LoadAsset(req);
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
        /// <returns >返回标识id可以取消完成后的complete回调</returns>
        static public int LoadAssetAsync(string abName, string assetName, System.Type type, System.Action<object, object> onComplete, System.Action<object, object> onEnd, object userData = null)
        {
            var req = CRequest.Get();
            req.assetName = assetName;
            req.assetType = type;
            req.assetBundleName = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            req.userData = userData;
            return LoadAsset(req);
        }

        /// <summary>
        /// 以同步方式加载 sub资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="priority"></param>
        static public UnityEngine.Object[] LoadAssetWithSubAssets(string abName, string assetName, System.Type type)
        {
            var req = CRequest.Get();
            req.assetName = assetName;
            req.assetType = type;
            req.assetBundleName = abName;
            LoadAsset(req, false, true);
            UnityEngine.Object[] t = (UnityEngine.Object[])req.data;
            req.ReleaseToPool();
            return t;
        }

        /// <summary>
        /// 以 回调方式加载 sub资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="priority"></param>
        /// <returns >返回标识id可以取消完成后的complete回调</returns>
        static public int LoadAssetWithSubAssetsAsync(string abName, string assetName, System.Type type, System.Action<object, object> onComplete, System.Action<object, object> onEnd)
        {
            var req = CRequest.Get();
            req.assetName = assetName;
            req.assetType = type;
            req.assetBundleName = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            return LoadAsset(req, true, true);
        }

        /// <summary>
        /// 以协程方式加载 sub资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="priority"></param>
        static public CRequest LoadAssetWithSubAssetsCoroutine(string abName, string assetName, System.Type type, System.Action<object, object> onComplete, System.Action<object, object> onEnd)
        {
            var req = new CRequest();
            req.assetName = assetName;
            req.assetType = type;
            req.assetBundleName = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            LoadAsset(req, true, true);
            return req;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="sceneName"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="allowSceneActivation"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        static public CRequest LoadSceneCoroutine(string abName, string sceneName, bool allowSceneActivation = true, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            var req = new CRequest();
            req.assetName = sceneName;
            req.assetType = LoaderType.Typeof_ABScene;
            req.assetBundleName = abName;
            LoadSceneAsset(req, allowSceneActivation, loadSceneMode);
            return req;
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="sceneName"></param>
        /// <param name="onComplete"></param>
        /// <param name="onEnd"></param>
        /// <param name="allowSceneActivation"></param>
        /// <param name="loadSceneMode"></param>
        /// <returns></returns>
        static public void LoadScene(string abName, string sceneName, System.Action<object, object> onComplete, System.Action<object, object> onEnd, object userData = null, bool allowSceneActivation = true, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            var req = CRequest.Get();
            req.assetName = sceneName;
            req.assetType = LoaderType.Typeof_ABScene;
            req.assetBundleName = abName;
            req.OnComplete = onComplete;
            req.OnEnd = onEnd;
            req.userData = userData;

            AsyncOperation request = null;
            if ((request = CacheManager.GetLoadingScene(sceneName)) != null)
            {
                request.allowSceneActivation = allowSceneActivation; //开始激活
                UnityEngine.Events.UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
                onSceneLoaded = (scene, model) =>
                 {
                     SceneManager.sceneLoaded -= onSceneLoaded;
                     CacheManager.RemoveLoadingScene(req.assetName);
                     DispatchReqAssetOperation(req, false, 0, true);
                     CheckAllComplete();
                 };

                SceneManager.sceneLoaded += onSceneLoaded;
            }
            else
            {
                LoadSceneAsset(req, allowSceneActivation, loadSceneMode);
            }

        }

        /// <summary>
        /// 停止正常回调，改为end回调。
        /// </summary>
        /// <param name="opID">返回的Operater id</param>
        /// <returns></returns>
        static public bool StopComplete(int opID)
        {
            bool ret = completeOper.Remove(opID);
#if UNITY_EDITOR
            Debug.LogFormat("StopComplete({0}) is {1}", opID, ret);
#endif
            return ret;
        }

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        static public void SetActiveScene(string sceneName)
        {
            Scene scene;
            if ((scene = SceneManager.GetSceneByName(sceneName)) != null)
            {
                SceneManager.SetActiveScene(scene);
            }

        }

        /// <summary>
        /// assetbundle计数减一 为0的时候自动卸载
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        static public void Subtract(string abName)
        {
            CacheManager.Subtract(abName);
        }

        /// <summary>
        /// 卸载场景和bundle
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        static public AsyncOperation UnloadScene(string sceneName)
        {
            return CacheManager.UnloadScene(sceneName);
        }

        /// <summary>
        /// 开启一个协程
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        static public Coroutine StartCoroutine1(IEnumerator coroutine)
        {
            if (_instance != null)
                return _instance.StartCoroutine(coroutine);

            return null;
        }

        #region monobeh
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            loadingEvent = new LoadingEventArg();
        }

        void Update()
        {
            // 加载assetbundle
            for (int i = 0; i < inBundleProgressOperations.Count;)
            {
                var operation = inBundleProgressOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    inBundleProgressOperations.RemoveAt(i);
                    downloadingBundles.Remove(operation.assetBundleName);
                    operation.ReleaseToPool();
                }
            }

            //asset加载
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
                    operation.Done();
                    var req = operation.request;
                    int opId = operation.id;
                    operation.ReleaseToPool();
                    DispatchReqAssetOperation(req, req.error != null, opId);

                    CheckAllComplete();
                }
            }

            //判断等待队列
            // while (waitOperations.Count > 0 && inProgressOperations.Count < maxLoading) //如果有
            // {
            //     inProgressOperations.Add(waitOperations.Dequeue()); //放入加载队列
            // }


#if UNITY_EDITOR
            // if (Time.frameCount % 180 == 0)
            // {
            //     Debug.LogFormat("inProgressOperations:{0},inBundleProgressOperations:{1},waitOperations:{2} ", inProgressOperations.Count, inBundleProgressOperations.Count, waitOperations.Count);
            //     if(inProgressOperations.Count>0)
            //     {
            //         foreach(var i in inProgressOperations)
            //         {
            //             Debug.LogFormat("item={0},progress={1} ",i.request.assetName,i.DebugString());
            //         }
            //     }
            // }
#endif
        }

        /// <summary>
        /// auto clear assetbundle
        /// </summary>
        void LateUpdate()
        {
            ABDelayUnloadManager.Update();
        }

        #endregion

        #region 

        public static void RegisterOverrideBaseAssetbundleURL(OverrideBaseDownloadingURLDelegate baseDownloadingURLDelegate)
        {
            overrideBaseDownloadingURL -= baseDownloadingURLDelegate;
            overrideBaseDownloadingURL += baseDownloadingURLDelegate;
        }

        #endregion

        #region load logic
        /// <summary>
        /// 获取重写的URL
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        internal static string GetAssetBundleDownloadingURL(string bundleName)
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

            return CUtils.PathCombine(CUtils.realStreamingAssetsPath, bundleName);
        }

        #endregion

        #region  资源加载
        /// <summary>
        /// 放入加载队列开始加载assetbundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="async">异步加载</param>
        /// <returns></returns>
        static void LoadAssetBundleInternal(string assetBundleName, bool async)
        {
            CacheData cache = CacheManager.TryGetCache(assetBundleName);
            if (cache.canUse) return; //可以使用

            // bool isIn = downloadingBundles.Contains(assetBundleName);//正在队列中

            if (async && !downloadingBundles.Contains(assetBundleName)) //异步加载
            {
                var op = OperationPools<BundleOperation>.Get();
                op.assetBundleName = assetBundleName;
                inBundleProgressOperations.Add(op);
                downloadingBundles.Add(assetBundleName);
            }
            else if (!async )//&& cache.state != CacheData.CacheDataState.Loading) //如果是同步加载，
            {
                var op = OperationPools<BundleOperation>.Get();
                op.assetBundleName = assetBundleName;
                op.StartSync(); //同步加载
                op.ReleaseToPool();
            }
            // else if (!async)
            // {
            //     Debug.LogWarningFormat("the assetbundle({0}) can't be sync loaded 因为它正在异步加载中!", assetBundleName);
            // }

        }

        /// <summary>
        /// 加载assetbundle
        /// </summary>
        /// <param name="assetBundleName"> string 加载资源名</param>
        /// <param name="async"> bool 异步加载默认ture</param>
        /// <returns></returns>
        internal static bool LoadAssetBundle(string assetBundleName, bool async = true)
        {
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                return false;
            }
#endif

#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample("LoadAssetBundle:" + assetBundleName);
#endif
            //查找缓存
            CacheData cacheData = null;
            CacheManager.CreateOrGetCache(assetBundleName, out cacheData);
            cacheData.count++; //引用计数加1
            ABDelayUnloadManager.Remove(assetBundleName); //从回收列表移除
            if (cacheData.canUse)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
                return true;
            }

            //开始加载依赖
            string[] deps = null;
            if (ManifestManager.fileManifest != null && (deps = ManifestManager.fileManifest.GetDirectDependencies(assetBundleName)).Length > 0)
            {
                LoadDependencies(assetBundleName, deps, async);
            }

            //加载assetbundle
            LoadAssetBundleInternal(assetBundleName, async);
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
            return false;
        }

        /// <summary>
        /// 加载assetbundle
        /// </summary>
        /// <param name="string">assetBundleName</param>
        /// <param name="bool">async = true 异步加载</param>
        /// <returns></returns>       
        static void LoadDependencies(string assetBundleName, string[] deps, bool async)
        {
            string item = null;
            if (deps.Length > 0)
            {
                // Debug.LogFormat("LoadDependencies assetBundleName={0},deps={0},len={1}", assetBundleName, string.Concat(deps), deps.Length);
                CacheManager.AddDependencies(assetBundleName, deps); //记录引用关系
                //开始加载依赖
                for (int i = 0; i < deps.Length; i++)
                {
                    item = deps[i];
                    if (!item.Equals(assetBundleName))
                    {
                        CacheData cacheData = null;
                        CacheManager.CreateOrGetCache(item, out cacheData);
                        cacheData.count++; //引用计数加1
                        ABDelayUnloadManager.Remove(item); //从列表移除
                        if (!cacheData.canUse)
                            LoadAssetBundleInternal(item, async);
                    }
                }
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="CRequest">request</param>
        /// <param name="bool">async = true 异步加载</param>
        /// <returns></returns>
        static int LoadAsset(CRequest request, bool async = true, bool subAssets = false)
        {
            totalCount++; //count ++

            if (m_MarkGroup)
            {
                m_Groupes.Add(request);
                m_TotalGroupCount++;
            }

            int opID = 0;
            LoadAssetBundle(request.assetBundleName, async);
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                AssetOperationSimulation assetOperS = OperationPools<AssetOperationSimulation>.Get();
                assetOperS.request = request;
                assetOperS.subAssets = subAssets;
                assetOperS.StartSync();

                assetOperS.ReleaseToPool();
                DispatchReqAssetOperation(request, request.error != null, 0, false); //模拟器模式下只有同步所以需要调用回调
                CheckAllComplete();
            }
            else
#endif
            {
                var assetOper = OperationPools<AssetOperation>.Get();
                assetOper.request = request;
                assetOper.subAssets = subAssets;
                if (async) //如果异步加载 设置id可以阻止回调
                {
                    AssetOperation.SetId(assetOper);
                    opID = assetOper.id;
                    completeOper.Add(opID);
                }

                if (!async) //同步加载
                {
                    assetOper.StartSync();
                    assetOper.ReleaseToPool();
                }
                // else if (inProgressOperations.Count >= maxLoading) //如果大于最大值
                // {
                //     waitOperations.Enqueue(assetOper); //放入等待列表
                // }
                else
                {
                    inProgressOperations.Add(assetOper);
                }
                // Debug.LogFormat("LoadAsset({0},{1},{2})", opID, request.assetBundleName,request.assetName);
            }
            return opID;
        }

        static int LoadSceneAsset(CRequest request, bool allowSceneActivation = true, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            totalCount++; //count ++

            if (m_MarkGroup)
            {
                m_Groupes.Add(request);
                m_TotalGroupCount++;
            }

            LoadAssetBundle(request.assetBundleName);
            int opID = 0;

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                var assetOper = OperationPools<AssetOperationSimulation>.Get();
                assetOper.request = request;
                assetOper.loadSceneMode = loadSceneMode;
                assetOper.allowSceneActivation = allowSceneActivation;
                assetOper.Update();
                AssetOperation.SetId(assetOper);
                opID = assetOper.id;
                completeOper.Add(opID);

                // if (inProgressOperations.Count >= maxLoading) //如果大于最大值
                //     waitOperations.Enqueue(assetOper); //放入等待列表
                // else
                    inProgressOperations.Add(assetOper);
            }
            else
#endif
            {
                AssetOperation assetOper = OperationPools<AssetOperation>.Get();
                assetOper.request = request;
                assetOper.loadSceneMode = loadSceneMode;
                assetOper.allowSceneActivation = allowSceneActivation;
                AssetOperation.SetId(assetOper);
                opID = assetOper.id;
                completeOper.Add(opID);

                // if (inProgressOperations.Count >= maxLoading) //如果大于最大值
                //     waitOperations.Enqueue(assetOper); //放入等待列表
                // else
                    inProgressOperations.Add(assetOper);
            }

            // Debug.LogFormat("LoadSceneAsset({0},{1})", opID, request.assetBundleName);

            return opID;
        }

        /// <summary>
        /// dispatch request event 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="isError"></param>
        static internal void DispatchReqAssetOperation(CRequest req, bool isError, int opID, bool backPool = true)
        {

            currLoaded++;

            int g_Idx = m_Groupes.IndexOf(req);
            if (g_Idx >= 0)
            {
                m_Groupes.RemoveAt(g_Idx);
                m_CurrGroupLoaded++;
            }

            OnProcess();
            try
            {
                if (isError)
                    req.DispatchEnd();
                else if (opID == 0 || completeOper.Contains(opID)) //正常回调
                {
                    completeOper.Remove(opID);
                    req.DispatchComplete();
                }
                else
                    req.DispatchEnd();

            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }

            if (backPool) req.ReleaseToPool();

        }
        #endregion

        #region  event

        static void OnProcess()
        {

            if (OnGroupProgress != null && m_TotalGroupCount > 0)
            {
                loadingEvent.total = m_TotalGroupCount > m_minTotalGroupCount ? m_TotalGroupCount : m_minTotalGroupCount;
                loadingEvent.current = m_CurrGroupLoaded;
                loadingEvent.progress = (float)loadingEvent.current / (float)loadingEvent.total;
                OnGroupProgress(loadingEvent);
            }

            if (OnProgress != null && totalCount > 0)
            {
                loadingEvent.total = totalCount;
                loadingEvent.current = currLoaded;
                loadingEvent.progress = (float)loadingEvent.current / (float)loadingEvent.total;
                OnProgress(loadingEvent);
            }


        }

        /// <summary>
        /// the queue is complete
        /// </summary>
        static void CheckAllComplete()
        {
            if (OnGroupComplete != null && m_TotalGroupCount <= m_CurrGroupLoaded && m_CurrGroupLoaded >= m_minTotalGroupCount && m_Groupes.Count == 0)
            {
                m_TotalGroupCount = 0;
                m_CurrGroupLoaded = 0;
                OnGroupComplete();
            }


            if (inBundleProgressOperations.Count == 0 && inProgressOperations.Count == 0)
            {
                totalCount = 0;
                currLoaded = 0;

                if (OnAllComplete != null)
                    OnAllComplete();
            }
        }

        public static void Clear()
        {
            OnGroupProgress = null;
            OnProgress = null;

            OnGroupComplete = null;
            OnAllComplete = null;
        }
        #endregion

        #region static

        static ResourcesLoader _instance;
        static ResourcesLoader instance
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
            // ReadManifest();
        }

        //         public static void ReadManifest()
        //         {
        // #if UNITY_EDITOR
        //             if (SimulateAssetBundleInEditor) //模拟模式不需要加载manifest
        //             {
        //                 Debug.LogFormat("<color=green>当前使用Simulation模式,如果使用assetbundle模式请使用菜单 /AssetBundles/Simulation Mode </color>");
        //                 return;
        //             }
        // #endif
        //             string url = ResourcesLoader.GetAssetBundleDownloadingURL(CUtils.platformFloder); // set full url
        //             url = CUtils.GetAndroidABLoadPath(url);
        //             var m_abRequest = AssetBundle.LoadFromFile(url);
        //             var all = m_abRequest.LoadAllAssets<AssetBundleManifest>();
        //             manifest = all[0];
        //             Debug.LogFormat("manifest {0} is Down！", manifest.name);
        //             m_abRequest.Unload(false);//卸载ab
        //         }
        #endregion
    }

}