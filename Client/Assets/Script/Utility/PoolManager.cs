// #define HUGULA_CACHE_DEBUG

using System;
using System.Collections.Generic;
using Hugula;
using Hugula.Framework;
using UnityEngine;
using Hugula.Collections;
using MemoryInfo;


namespace Hugula.Utility
{
    public struct Request
    {
        public string key;
        public object arg;
        public Action<object, string, UnityEngine.Object> action;
        public int priority;
        public UnityEngine.Object element;

        public UnityEngine.Transform parent;

        public bool isPreLoad;

        public void Dispose()
        {
            arg = null;
            action = null;
            key = null;
            element = null;
            parent = null;
        }
    }

    /// <summary>
    /// 异步对象池
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    [XLua.LuaCallCSharp]
    public class PoolManager : BehaviourSingleton<PoolManager>
    {
        /// <summary>
        /// 最大同时加载assetbundle asset数量
        /// </summary>
        static public int maxLoading = 12;

        /// <summary>
        /// 加载asset耗时跳出判断时间
        /// </summary>
        static public float BundleLoadBreakMilliSeconds = 200;

        static public Vector3 InitPosition = Vector3.one * -2000;

        static public Quaternion InitQuaternion = Quaternion.identity;

        /// <summary>
        /// 空闲对象
        /// </summary>
        /// <returns></returns>
        private SafeDictionary<string, Stack<UnityEngine.Object>> m_StackDic =
            new SafeDictionary<string, Stack<UnityEngine.Object>>();

        /// <summary>
        /// source原始对象
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="UnityEngine.Object"></typeparam>
        /// <returns></returns>
        internal SafeDictionary<string, UnityEngine.Object> m_Source = new SafeDictionary<string, UnityEngine.Object>();

        /// <summary>
        /// source原始对象的引用计数
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="int"></typeparam>
        /// <returns></returns>
        internal SafeDictionary<string, int>
            m_SourceRef = new SafeDictionary<string, int>(); //http://10.23.0.3:8081/browse/SLGL2-9765 多线程导致？

        /// <summary>
        /// 元素与key的映射
        /// </summary>
        private Dictionary<UnityEngine.Object, string> elementKeyMap = new Dictionary<UnityEngine.Object, string>(512);


        /// <summary>
        /// 耗时统计
        /// </summary>
        /// <returns></returns>
        static private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        static MemoryInfoPlugin memoryInfoPlugin ;

        List<Request> callbackList = new List<Request>();

        List<Request> waitingTasks = new List<Request>();
        List<Request> inProgressOperations = new List<Request>();

        List<Request> waitingSourceTasks = new List<Request>();

        #region 队列加载

        /// <summary>
        /// 队列加载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        public void GetAsyncArgPriority(object arg, string key, Action<object, string, UnityEngine.Object> onComplete,
            int priority = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsyncArgPriority key is nil \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request
            {
                key = key,
                action = onComplete,
                arg = arg,
                priority = priority
            };

            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            AddToWaitingTasks(req);
        }

        public void GetAsyncPriority(string key, Action<object, string, UnityEngine.Object> onComplete, int priority = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsyncPriority key is nil \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request
            {
                key = key,
                action = onComplete,
                priority = priority
            };

            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            AddToWaitingTasks(req);
        }

        /// <summary>
        /// 放入加载队列
        /// </summary>
        /// <param name="req">Request</param>
        private void AddToWaitingTasks(Request req)
        {
            //
            int priority = req.priority;
            int len = waitingTasks.Count;
            Request item;
            if (priority <= 0) //默认最小为0
            {
                waitingTasks.Add(req);
            }
            else
            {
                bool added = false;
                for (int i = 0; i < len; i++)
                {
                    item = waitingTasks[i];
                    if (priority > item.priority) //如果优先级更高
                    {
                        waitingTasks.Insert(i, req);
                        added = true;
                        break;
                    }
                }

                if (!added) waitingTasks.Add(req);
            }
        }

        /// <summary>
        /// 队列加载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        public void GetSourcePriority(string key, Action<object, string, UnityEngine.Object> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetSourcePriority key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request
            {
                key = key,
                action = onComplete,
            };
            waitingSourceTasks.Add(req);
        }

        public int CompareTo(Request x, Request y)
        {
            return x.priority - y.priority;
        }

        public void Clear()
        {
            waitingTasks.Clear();
            inProgressOperations.Clear();
            m_SourceRef.Clear();
            m_StackDic.Clear();
            m_Source.Clear();
        }

        #endregion

        #region config

        public const float deltaTime30 = 0.033f;
        public const float deltaTime25 = 0.04f;
        public const float deltaTime20 = 0.05f;
        public const float deltaTime15 = 0.067f;

        /// <summary>
        /// 两次GC检测间隔时间
        /// </summary>
        public static float gcDeltaTimeConfig = 3f; //两次GC检测时间S

        /// <summary>
        /// 延时删除时间
        /// </summary>
        public static float delayRemoveTime = 4.5f;

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// 内存阈值
        /// </summary>
        public static float threshold1 = 75*1024f;
        public static float threshold2 = 100*1024f;
        public static float threshold3 = 125*1024f;
#elif UNITY_IOS && !UNITY_EDITOR
        /// <summary>
        /// 内存阈值
        /// </summary>
        public static float threshold1 = 75*1024f;
        public static float threshold2 = 90*1024f;
        public static float threshold3 = 110*1024f;
#else
        /// <summary>
        /// 内存阈值
        /// </summary>
        public static float threshold1 = 260 * 1024f;
        public static float threshold2 = 400 * 1024f;
        public static float threshold3 = 600 * 1024f;
#endif
        /// <summary>
        /// 回收片段
        /// </summary>
        private const byte gcSegment1 = 1; //片段索引
        private const byte gcSegment2 = 3;
        private const byte gcSegment3 = 6;

        /// <summary>
        /// 总共分0-7
        /// </summary>
        private const byte SegmentSize = 7;
        private static int removeCount = 1;
        private static float lastGcTime = 0; //上传检测GC时间
        private static float gcDeltaTime = 0; //上传检测GC时间

        //标记回收
        private Dictionary<string, float> removeMark = new Dictionary<string, float>(512);

        /// <summary>
        /// 回收列表
        /// </summary>
        private Queue<string> willGcList = new Queue<string>(256);

        /// <summary>
        /// 类型用于做回收策略
        /// </summary>
        private Dictionary<string, CateInfo> prefabsType = new Dictionary<string, CateInfo>(256);

        public static class Segment
        {
            public static byte Segment0 = 0;
            public static byte Segment1 = 1;
            public static byte Segment2 = 2;
            public static byte Segment3 = 3;
            public static byte Segment4 = 4;
            public static byte Segment5 = 5;
            public static byte Segment6 = 6;
            public static byte Segment7 = 7;
            public static byte Segment8 = 8;
        }

        /// <summary>
        /// key的描述信息
        /// </summary>
        public struct CateInfo
        {
            /// <summary>
            /// 类别id用于切换统一回收
            /// </summary>
            public int category;
            /// <summary>
            /// 片段索引用于常规回收
            /// </summary>
            public byte segment;
        }

        int lowMemory;
        #endregion

        #region mono

        void Start()
        {
            lastGcTime = Time.unscaledTime;
            lowMemory = 0;
            Application.lowMemory += OnLowMemory;
            memoryInfoPlugin = new MemoryInfoPlugin();
        }


        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            watch.Restart();
            // FrameWatcher.BeginWatch();
            while (waitingSourceTasks.Count > 0)
            {
                if (watch.ElapsedMilliseconds > BundleLoadBreakMilliSeconds)
                {
                    break;
                }


                var req = waitingSourceTasks[0];
                waitingSourceTasks.RemoveAt(0);
                GetSourceAsync(req);
            }

            // waitingTasks
            while (waitingTasks.Count > 0 && maxLoading - inProgressOperations.Count > 0)
            {
                //check frame time
                if (watch.ElapsedMilliseconds > BundleLoadBreakMilliSeconds)
                {
                    break;
                }

                var req = waitingTasks[0];
                waitingTasks.RemoveAt(0);
                LoadAssetAsync(req);
            }

            while (callbackList.Count > 0)
            {
                if (watch.ElapsedMilliseconds > BundleLoadBreakMilliSeconds)
                {
                    break;
                }
                var req = callbackList[0];
                callbackList.RemoveAt(0);
#if PROFILER_DUMP
                using (var Profiler = Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("PoolManager.callbackList:", req.key))
                {
#endif
                    req.action?.Invoke(req.arg, req.key, req.element);
                    req.Dispose();
#if PROFILER_DUMP
                }
#endif
            }
        }

        /// <summary>
        /// 内存阈值检测和自动对象回收
        /// </summary>
        void LateUpdate() //每帧删除
        {
            if (willGcList.Count == 0) //如果正在gc不需要判断
            {
                gcDeltaTime = Time.unscaledTime - lastGcTime;
                if (gcDeltaTime >= gcDeltaTimeConfig) //20s检测一次
                {
                    // float totalMemory = HugulaProfiler.GetTotalAllocatedMemoryMB();
                    var memoryInfo = memoryInfoPlugin.GetMemoryInfo();
                    int totalMemory = memoryInfo.TotalSize;
                    int UsedSize = memoryInfo.UsedSize;
                    int AvailMem = memoryInfo.AvailMem;
                    var systemMemory = HugulaProfiler.systemMemorySize;
#if HUGULA_CACHE_DEBUG
                    Debug.Log($" totalMemory={totalMemory} UsedSize={UsedSize} AvailMem={AvailMem} totalMemory*0.15:{totalMemory * .15f} totalMemory*0.6:{totalMemory * .6f} systemMemoryMB={systemMemory}  lowMemory:{lowMemory},frame:{Time.frameCount}");
#endif

                    if (AvailMem < totalMemory * .15f || lowMemory > 0) //可用内存小于总内存的15% 或者低内存通知
                    {
                        if (lowMemory > 0)
                            lowMemory--;
                        AutoGC(gcSegment3);
                    }
                    else if (UsedSize > totalMemory * .6f)//大于总内存的60%
                    {
                        AutoGC(gcSegment2);
                    }
                    else if (UsedSize > threshold1)
                    {
                        AutoGC(gcSegment1);
                    }

                    lastGcTime = Time.unscaledTime;
                } // 
            }

            if (willGcList.Count > 0) //如果有需要回收的gameobject
            {
                var fps = Time.time / (float)Time.frameCount;
                if (fps >= deltaTime30)
                {
                    removeCount = 4;
                }
                else if (fps >= deltaTime25)
                {
                    removeCount = 3;
                }
                else if (fps >= deltaTime20)
                {
                    removeCount = 2;
                }
                else //if (fps >= deltaTime15)
                {
                    removeCount = 1;
                }

                DisposeRefer(removeCount); //移除数量
            }
        }

        protected override void OnDestroy()
        {
            Clear();
            ClearAllCache();
            base.OnDestroy();
            m_ActionOnRelease = null;
            m_ActionOnGet = null;
        }


        #endregion

        #region 资源释放

        void OnLowMemory()
        {
            lowMemory++;
        }

        /// <summary>
        /// 手动回收 可以释放的对象
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <returns></returns>
        public void GCCollect(byte segmentIndex)
        {
            AutoGC(segmentIndex, false);
        }

        /// <summary>
        /// 释放特定类别的对象
        /// </summary>
        /// <param name="category"></param>
        public void GCCollectCate(int category)
        {
            var items = removeMark.GetEnumerator();
            string key = string.Empty;
            int keyType = 0;
            while (items.MoveNext())
            {
                var kv = items.Current;
                key = kv.Key;
                if (prefabsType.TryGetValue(key, out var cateInfo))
                    keyType = cateInfo.category;
                else
                {
                    keyType = 0;
                }

                if (keyType == category)
                {
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("PoolManager.willGcList.Add({0}), category={1},frame={2}", key, keyType, Time.frameCount);
#endif
                    if (!willGcList.Contains(key)) willGcList.Enqueue(key);
                }
            }
        }

        /// <summary>
        /// 自动回收对象当前segmentindex和之前的所有片段
        /// </summary>
        internal void AutoGC(byte segmentIndex, bool compareTime = true)
        {
            var items = removeMark.GetEnumerator();
            string key = string.Empty;
            byte keyType = 0;
            CateInfo cateInfo;
            while (items.MoveNext())
            {
                var kv = items.Current;
                key = kv.Key;
                if (prefabsType.TryGetValue(key, out cateInfo))
                {
                    keyType = cateInfo.segment;
                }
                else
                {
                    keyType = 0;
                }

                if (keyType <= segmentIndex && (compareTime ? Time.unscaledTime >= kv.Value : true))
                {
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("PoolManager.willGcList.Add({0}), keyType={1},segmentIndex={2},frame={3}", key, keyType, segmentIndex, Time.frameCount);
#endif
                    if (!willGcList.Contains(key)) willGcList.Enqueue(key);
                }
            }
        }

        /// <summary>
        /// 真正销毁对象
        /// </summary>
        /// <param name="count"></param>
        void DisposeRefer(int count)
        {
            if (count > willGcList.Count) count = willGcList.Count;
            string referKey; //要删除的项目
            var begin = System.DateTime.Now;
            while (count > 0)
            {
                referKey = willGcList.Dequeue();
                if (removeMark.Remove(referKey))
                {
                    ClearKey(referKey);
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("PoolManager.real clear key {0},frame={1}", referKey, Time.frameCount);
#endif
                }

                var ts = System.DateTime.Now - begin;
                if (ts.TotalSeconds > deltaTime25) break;
                // Debug.Log("Dispose " + referRemove.name);
                count--;

                // if (FrameWatcher.IsTimeOver(BundleLoadBreakMilliSeconds)) break;
            }
        }

        /// <summary>
        /// 清理引用的key
        /// </summary>
        /// <param name="key"></param>
        internal void Remove(string key)
        {
            UnityEngine.Object obj = null;
            if (m_Source.TryGetValue(key, out obj))
            {
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("PoolManager.Destroy OriginalPrefabs={0},frame={1}", key, Time.frameCount);
#endif
                ResLoader.Release(obj); //释放引用
            }

            m_Source.Remove(key);
            prefabsType.Remove(key);
            m_StackDic.Remove(key);
            m_SourceRef.Remove(key);
        }

        /// <summary>
        /// 清理当前key的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal void ClearKey(string key)
        {
            int count = 0;
            if (m_SourceRef.TryGetValue(key, out count) && count <= 0) //没有被引用
            {
                Stack<UnityEngine.Object> m_Stack;
                bool hasFree = m_StackDic.TryGetValue(key, out m_Stack); //空闲对象
                Remove(key); //移除原始项目
                if (hasFree)
                {
                    UnityEngine.Object item;
                    while (m_Stack.Count > 0)
                    {
                        item = m_Stack.Pop();
                        UnityEngine.Object.Destroy(item);
                    }
                };
            }
        }

        /// <summary>
        /// Clears all cache.
        /// </summary>
        public void ClearAllCache()
        {
            prefabsType.Clear();
            willGcList.Clear();
            removeMark.Clear();
            m_SourceRef.Clear();

            UnityEngine.Object item;
            var freeValues = m_StackDic.Values.GetEnumerator();
            while (freeValues.MoveNext())
            {
                var queue = freeValues.Current;
                while (queue.Count > 0)
                {
                    item = queue.Pop();
                    if (item) GameObject.Destroy(item);
                }
            }

            m_StackDic.Clear();

            var sourceValues = m_Source.Values.GetEnumerator();
            while (sourceValues.MoveNext())
            {
                item = sourceValues.Current;
                if (item) ResLoader.Release(item);
            }
        }

        internal void AddRemoveMark(string key)
        {
            removeMark[key] = Time.unscaledTime + delayRemoveTime;
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("PoolManager.AddRemoveMark={0},frame={1}", key, Time.frameCount);
#endif
        }

        internal void DeleteRemveMark(string key)
        {
            removeMark.Remove(key);
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("PoolManager.DeleteRemveMark={0},frame={1}", key, Time.frameCount);
#endif
        }

        /// <summary>
        /// 标记删除 如果有引用不会被删除
        /// </summary>
        public int MarkRemove(string key)
        {
            int referCount = 0;
            if (m_SourceRef.TryGetValue(key, out referCount) && referCount <= 0)
            {
                AddRemoveMark(key);
            }

            return referCount;
        }

        /// <summary>
        /// 设置资源的回收类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetPrefabType(string key, byte value)
        {
            if (prefabsType.TryGetValue(key, out var cateInfo))
            {
                cateInfo.segment = value;
                prefabsType[key] = cateInfo;
            }
            else
            {
                prefabsType.Add(key, new CateInfo() { segment = value });
            }
        }

        /// <summary>
        /// 设置资源的分类用于切换统一回收
        /// </summary>
        /// <param name="key"></param>
        /// <param name="category"></param>
        public void SetPrefabCate(string key, int category)
        {
            if (prefabsType.TryGetValue(key, out var cateInfo))
            {
                cateInfo.category = category;
                prefabsType[key] = cateInfo;
            }
            else
            {
                prefabsType.Add(key, new CateInfo() { category = category });
            }
        }

        #endregion

        #region 事件相关

        public void AddOnReleaseEvent(Action<UnityEngine.Object> onRelease)
        {
            m_ActionOnRelease += onRelease;
        }

        public void RemoveOnReleaseEvent(Action<UnityEngine.Object> onRelease)
        {
            m_ActionOnRelease -= onRelease;
        }

        public void AddOnGetEvent(Action<UnityEngine.Object> onGet)
        {
            m_ActionOnGet += onGet;
        }

        public void RemoveGetEvent(Action<UnityEngine.Object> onGet)
        {
            m_ActionOnGet -= onGet;
        }

        Action<UnityEngine.Object> m_ActionOnRelease;
        Action<UnityEngine.Object> m_ActionOnGet;

        #endregion

        #region 实例获取与释放

        /// <summary>
        /// 异步获取source 引用计数加1
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        public async void GetSourceAsync(Request req)
        {
            var key = req.key;
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetSourceAsync key is empty");
            }
            UnityEngine.Object element;
            removeMark.Remove(key);
            if (!m_SourceRef.ContainsKey(key)) m_SourceRef.Add(key, 0);

            if (!m_Source.TryGetValue(key, out element))
            {
                inProgressOperations.Add(req);
                var task = ResLoader.LoadAssetAsyncTask<UnityEngine.Object>(key);
                await task;
                inProgressOperations.Remove(req);
                if (m_Source.TryGetValue(key, out element)) //如果已经加载过
                {
                    ResLoader.Release(task.Result); //释放引用
                }
                else
                {
                    element = task.Result;
                    m_Source.Add(key, element);
                }
            }

            if (m_SourceRef.TryGetValue(key, out var count) && count < 0) //如果被释放
            {
                ResLoader.Release(element); //释放引用
                req.Dispose();
                Remove(key);
                return;
            }

            AddReferCount(key, element);

            // if (!prefabsType.ContainsKey(key))
            //     prefabsType[key] = 0;
            AddCallback(req, element);
        }
        private void AddCallback(Request req, UnityEngine.Object element)
        {
            req.element = element;
            callbackList.Add(req);
            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             RemoveLuaTrack(req);
            // #endif
        }

        private void AddReferCount(string key, UnityEngine.Object element)
        {
            int count = -1;
            if (m_SourceRef.TryGetValue(key, out count))
            {
                count++;
                m_SourceRef[key] = count;
                elementKeyMap[element] = key;
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("PoolManager.AddReferCount({0},count{1}){2},frame={3}", key, count, element, Time.frameCount);
#endif
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            else
            {
                UnityEngine.Debug.LogError(
                    $"AddReferCount Error. sourceref doesnt contiansKey({key})  \r\n luatrace:{EnterLua.LuaTraceback()}");
            }
#endif
        }

        private int RemoveReferCount(string key)
        {
            int count = -1;
            if (m_SourceRef.TryGetValue(key, out count))
            {
                count--;
                m_SourceRef[key] = count;
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("PoolManager.RemoveReferCount({0},count={1}),frame={2}", key, count, Time.frameCount);
#endif
            }

            return count;
        }

        /// <summary>
        /// 原始对象引用计数减1
        /// </summary>
        /// <param name="key"></param>
        public void ReleaseSource(string key)
        {
            int count = RemoveReferCount(key);
            if (count <= 0) //没有引用可以标记为回收
            {
                AddRemoveMark(key);
            }
        }

        async void LoadAssetAsync(Request req)
        {
            var key = req.key;
            Stack<UnityEngine.Object> m_Stack;
            removeMark.Remove(key);
            if (!m_SourceRef.ContainsKey(key)) m_SourceRef.Add(key, 0); //标记
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("PoolManager.LoadAssetAsync({0},count={1}),frame={2}", key, m_SourceRef[key], Time.frameCount);
#endif
            UnityEngine.Object element;
            if (m_StackDic.TryGetValue(key, out m_Stack))
            {
                while (m_Stack.Count > 0)
                {
                    element = m_Stack.Pop();
                    if (null != element)
                    {
                        AddReferCount(key, element);
                        if (m_ActionOnGet != null)
                            m_ActionOnGet(element);
                        AddCallback(req, element);
                        key = null;
                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("[PoolManager Error]{0} is disposed", key);
                    }
                }
            }

            if (!m_Source.TryGetValue(key, out element))
            {
                inProgressOperations.Add(req); //异步时候才需要添加进入队列
                System.Threading.Tasks.Task<UnityEngine.Object> task = null;
#if PROFILER_DUMP
                var pkey1 = $"PoolManager.LoadAssetAsyncTask<GameObject>:" + key;
                using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey1))
                {
#endif
                    task = ResLoader.LoadAssetAsyncTask<UnityEngine.Object>(key);
                    await task;
                    ResLoader.LoadAssetAsyncTaskDone(key);
                    inProgressOperations.Remove(req);//移除队列计数
#if PROFILER_DUMP
                }
#endif
                if (m_Source.TryGetValue(key, out element)) //如果已经加载过
                {
#if UNITY_EDITOR || UNITY_STANDALONE
                    Debug.LogWarningFormat("the resource({0}) has already loaded ,now we need release it ({1})。 ",
                        key, task.Result);
#endif
                    ResLoader.Release(task.Result); //释放引用
                }
                else
                {
                    element = task.Result;
                    if (null == element)
                    {
                        Debug.LogErrorFormat("PoolManager.LoadAssetAsync can't find asset ({0})", key);
                        // AddCallback(req, null);
                        // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
                        //                         Debug.LogError($"PoolManager.LoadAssetAsync the key ({key})  res is nil, {GetLuaTrack(req)} ");
                        //                         RemoveLuaTrack(req);
                        // #endif
                        req.Dispose();
                        return;
                    }

                    // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
                    //                     if (!(element is GameObject))
                    //                     {
                    //                         Debug.LogError($"PoolManager.LoadAssetAsync the key ({key})  res is not a GameObject, {GetLuaTrack(req)} ");
                    //                         RemoveLuaTrack(req);
                    //                     }
                    // #endif

                    m_Source.Add(key, element);
                }
            }

#if PROFILER_DUMP
            var pkey = $"PoolManager.LoadAssetAsync.Instantiate<GameObject>:" + key;
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler(pkey, null, null, true))
            {
#endif
                var go = ((GameObject)element);
                if (go && !go.activeSelf)
                {
                    var sTrans = go.transform;
                    element = UnityEngine.GameObject.Instantiate(element, sTrans.position, sTrans.rotation, req.parent); //keep position and rotation
                }
                else
                {
                    element = UnityEngine.GameObject.Instantiate(element, InitPosition, InitQuaternion, req.parent);
                }
#if PROFILER_DUMP
            }
#endif

            AddReferCount(key, element);
            // if (!prefabsType.ContainsKey(key))
            //     prefabsType[key] = 0;
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);

            if (req.isPreLoad) //预加载需要立即执行
            {
                req.element = element;
                req.action?.Invoke(req.arg, req.key, req.element);
                req.Dispose();
            }
            else
                AddCallback(req, element);

            key = null;

        }

        /// <summary>
        /// 异步预加载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete">Action<object, string, UnityEngine.Object></param>
        public void PreloadAsync(string key, Action<object, string, UnityEngine.Object> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"PreloadAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request
            {
                key = key,
                action = (o, s, obj) =>
                {
                    if (obj != null)
                    {
                        RemoveReferCount(key);//引用计数-1
                        BackStack(key, obj); //返回给对向池
                    }
                    if (onComplete != null)
                        onComplete(o, s, obj);
                },
                isPreLoad = true,
            };

            LoadAssetAsync(req);
        }

        /// <summary>
        /// 带优先级的异步预加载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete">Action<object, string, UnityEngine.Object></param>
        /// <param name="priority">优先级</param>
        public void PreloadAsyncPriority(string key, Action<object, string, UnityEngine.Object> onComplete, int priority = 0)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"PreloadAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request
            {
                key = key,
                action = (o, s, obj) =>
                {
                    if (obj != null)
                    {
                        RemoveReferCount(key);//引用计数-1
                        BackStack(key, obj); //返回给对向池
                    }
                    if (onComplete != null)
                        onComplete(o, s, obj);
                },
                priority = priority,
                isPreLoad = true,
            };

            AddToWaitingTasks(req);
        }


        /// <summary>
        /// 异步获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="onComplete"></param>
        public void GetAsync(string key, Action<object, string, UnityEngine.Object> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }

            var req = new Request { key = key, action = onComplete };
            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            LoadAssetAsync(req);
        }

        public void GetAsyncParent(string key, Action<object, string, UnityEngine.Object> onComplete, Transform parent)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }
            var req = new Request { key = key, action = onComplete, parent = parent };
            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            LoadAssetAsync(req);
        }

        public void GetAsyncArg(object arg, string key, Action<object, string, UnityEngine.Object> onComplete)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }
            var req = new Request { key = key, action = onComplete, arg = arg };
            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            LoadAssetAsync(req);
        }

        public void GetAsyncArgParent(object arg, string key, Action<object, string, UnityEngine.Object> onComplete, Transform parent)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException($"GetAsync key \r\n luatrace:{EnterLua.LuaTraceback()}");
            }
            var req = new Request { arg = arg, key = key, action = onComplete, parent = parent };
            // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
            //             AddLuaTrack(req, EnterLua.LuaTraceback());
            // #endif
            LoadAssetAsync(req);
        }

        /// <summary>
        /// 返回给池子，引用计算为0时候标记销毁资源
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Release(string key, UnityEngine.Object element)
        {
            BackStack(key, element);
            int count = RemoveReferCount(key);
            if (count == 0) AddRemoveMark(key); //如果引用为0标记回收
        }

        /// <summary>
        /// 返回给池子，引用计算为0时候标记销毁资源,不执行ReleaseAction
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void ReleaseNoAction(string key, UnityEngine.Object element)
        {
            BackStack(key, element, false);
            int count = RemoveReferCount(key);
            if (count == 0) AddRemoveMark(key); //如果引用为0标记回收
        }

        /// <summary>
        /// 返回给池子，引用计算为0时候标记销毁资源
        /// </summary>
        /// <param name="element">UnityEngine.Object</param>
        public void ReleaseObj(UnityEngine.Object element)
        {
            if (elementKeyMap.TryGetValue(element, out var key))
            {
                Release(key, element);
            }
            else
                Debug.LogError($"ReleaseObj error: {element} not in pool");
        }

        /// <summary>
        /// 返回给池子，不会销毁资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="element"></param>
        public void BackStack(string key, UnityEngine.Object element, bool useReleaseAction = true)
        {
            Stack<UnityEngine.Object> m_Stack;
            if (!m_StackDic.TryGetValue(key, out m_Stack))
            {
                m_Stack = new Stack<UnityEngine.Object>();
                m_StackDic.Add(key, m_Stack);
            }

            if (element != null) //有可能还没加载完成就销毁了
            {
                elementKeyMap.Remove(element); //移除key映射
                if (m_Stack.Contains(element)) // m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                {
                    Debug.LogError(
                        $"Internal error. Trying to destroy object {element}  that is already released to pool.");
                    return;
                }

                if (m_ActionOnRelease != null && useReleaseAction)
                    m_ActionOnRelease(element);
                m_Stack.Push(element);
            }
        }

        #endregion

        #region debug
        // #if UNITY_EDITOR && (!LUA_PROFILER_DEBUG || !PROFILER_DUMP || !HUGULA_NO_LOG )
        //
        //         Dictionary<Request, string> m_LuaTrackDic = new Dictionary<Request, string>();
        //         void AddLuaTrack(Request key, string luaTrace)
        //         {
        //             if (m_LuaTrackDic.ContainsKey(key))
        //             {
        //                 m_LuaTrackDic[key] = luaTrace;
        //             }
        //             else
        //             {
        //                 m_LuaTrackDic.Add(key, luaTrace);
        //             }
        //         }
        //
        //         void RemoveLuaTrack(Request key)
        //         {
        //             if (m_LuaTrackDic.ContainsKey(key))
        //             {
        //                 m_LuaTrackDic.Remove(key);
        //             }
        //         }
        //
        //         string GetLuaTrack(Request key)
        //         {
        //             var luaTrace = string.Empty;
        //             m_LuaTrackDic.TryGetValue(key, out luaTrace);
        //             return luaTrace;
        //         }
        //
        // #endif

        #endregion

    }

    /// <summary>
    /// 内存相关
    /// </summary>
    public static class HugulaProfiler
    {
        const float m_MBSize = 1024.0f * 1024.0f;

        public static bool IsMemoryWarning
        {
            get
            {
                return false;
            }
        }

        public static float GetTotalReservedMemoryMB()
        {
#if UNITY_2017 || UNITY_5_6_OR_NEWER
            float totalMemory = (float)(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / m_MBSize);
#else
            float totalMemory = (float) (UnityEngine.Profiler.GetTotalReservedMemory () / m_KBSize);
#endif
            return totalMemory;
        }

        /// <summary>
        /// 获取当前分配的内存 单位MB
        /// </summary>
        /// <returns></returns>
        public static float GetTotalAllocatedMemoryMB()
        {
            var totalMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() + UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();

            return totalMemory / m_MBSize;
        }


        private static float m_MemorySize = float.MinValue;
        public static float systemMemorySize
        {
            get
            {
                if (m_MemorySize == float.MinValue)
                    m_MemorySize = SystemInfo.systemMemorySize;

                return m_MemorySize;
            }
        }
    }

    /// <summary>
    /// 资源分类
    /// </summary>
    public static class ResCategoryType
    {
        public const int None = 0;

        /// <summary>
        /// 大地图
        /// </summary>
        public const int World = 1;

        /// <summary>
        /// 内城
        /// </summary>
        public const int City = 10;

        /// <summary>
        /// 副本
        /// </summary>
        public const int InstanceDungeon = 20;
    }

}