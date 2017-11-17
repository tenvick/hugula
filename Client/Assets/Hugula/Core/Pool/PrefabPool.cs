// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using Hugula.Utils;

namespace Hugula.Pool
{
    /// <summary>
    /// prefab 缓存池
    /// </summary>
    [SLua.CustomLuaClass]
    public class PrefabPool : MonoBehaviour
    {
        #region const config

        public const float deltaTime30 = 0.033f;
        public const float deltaTime25 = 0.04f;
        public const float deltaTime20 = 0.05f;
        public const float deltaTime15 = 0.067f;

        #endregion
        #region debug
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
        private static string cacheKey = string.Empty;
        private static string cacheKey1 = string.Empty;

#endif
        #endregion

        #region instance
        private static int removeCount = 1;
        private static float lastGcTime = 0;//上传检测GC时间
        private static float gcDeltaTime = 0;//上传检测GC时间
        //标记回收
        private static Dictionary<int, float> removeMark = new Dictionary<int, float>(512);

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// 内存阈值检测和自动对象回收
        /// </summary>
        void LateUpdate() //每帧删除
        {
            if (willGcList.Count == 0) //如果正在gc不需要判断
            {
                gcDeltaTime = Time.unscaledTime - lastGcTime;
                if (gcDeltaTime >= gcDeltaTimeConfig)//20s检测一次
                {
                    float totalMemory = HugulaProfiler.GetTotalAllocatedMemory();
                    //Debug.Log(totalMemory);
                    if (totalMemory > threshold3)
                    {
                        AutoGC(gcSegment3);
                    }
                    else if (totalMemory > threshold2)
                    {
                        AutoGC(gcSegment2);
                    }
                    else if (totalMemory > threshold1)
                    {
                        AutoGC(gcSegment1);
                    }

                    lastGcTime = Time.unscaledTime;
                }// 
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

                DisposeRefer(removeCount);//移除数量
            }

        }

        /// <summary>
        /// 真正销毁对象
        /// </summary>
        /// <param name="count"></param>
        void DisposeRefer(int count)
        {
            if (count > willGcList.Count) count = willGcList.Count;
            int referKey;//要删除的项目
            var begin = System.DateTime.Now;
            while (count > 0)
            {
                referKey = willGcList.Dequeue();
                if (removeMark.ContainsKey(referKey))
                {
                    ClearKey(referKey);
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("real clear key {0},frame={1}",referKey,Time.frameCount);
#endif
                    removeMark.Remove(referKey);
                }
                var ts = System.DateTime.Now - begin;
                if (ts.TotalSeconds > deltaTime25) break;
                // Debug.Log("Dispose " + referRemove.name);
                count--;
            }

        }

        void OnDestroy()
        {
            if (_gameObject == this.gameObject)
                _gameObject = null;
            ClearAllCache();
            Clear();
        }


        #endregion

        #region static


        #region config
        /// <summary>
        /// 两次GC检测间隔时间
        /// </summary>
        public static float gcDeltaTimeConfig = 10f;//两次GC检测时间S

#if UNITY_EDITOR
        /// <summary>
        /// 内存阈值
        /// </summary>
        public static float threshold1 = 150f;
        public static float threshold2 = 190f;
        public static float threshold3 = 250f;
#else
    /// <summary>
    /// 内存阈值
    /// </summary>
    public static float threshold1 = 50f;
    public static float threshold2 = 100f;
    public static float threshold3 = 150f;
#endif

        /// <summary>
        /// 回收片段
        /// </summary>
        private const byte gcSegment1 = 1;//片段索引
        private const byte gcSegment2 = 3;
        private const byte gcSegment3 = 6;
        #endregion

        #region private member
        /// <summary>
        /// 总共分0-8
        /// </summary>
        private const byte SegmentSize = 8;

        /// <summary>
        /// 回收列表
        /// </summary>
        private static Queue<int> willGcList = new Queue<int>();

        /// <summary>
        /// 原始缓存
        /// </summary>
        private static Dictionary<int, GameObject> originalPrefabs = new Dictionary<int, GameObject>();

        /// <summary>
        /// 原始资源标记缓存
        /// </summary>
        private static Dictionary<GameObject, bool> assetsFlag = new Dictionary<GameObject, bool>();

        /// <summary>
        /// 类型用于做回收策略
        /// </summary>
        private static Dictionary<int, byte> prefabsType = new Dictionary<int, byte>();

        /// <summary>
        /// 被引用
        /// </summary>
        private static Dictionary<int, HashSet<ReferGameObjects>> prefabRefCaches = new Dictionary<int, HashSet<ReferGameObjects>>();

        /// <summary>
        /// 可用队列
        /// </summary>
        private static Dictionary<int, Queue<ReferGameObjects>> prefabFreeQueue = new Dictionary<int, Queue<ReferGameObjects>>();

        #endregion

        #region pulic method
        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void Clear()
        {
            var values = originalPrefabs.Values.GetEnumerator();
            //foreach (Object item in values)
            while (values.MoveNext())
            {
                var item = values.Current;
                DestroyOriginalPrefabs(item);
            }

            originalPrefabs.Clear();
        }

        /// <summary>
        /// 添加原始缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static bool Add(string key, GameObject value, byte type)
        {
            int hashkey = LuaHelper.StringToHash(key);
            return Add(hashkey, value, type);
        }

        /// <summary>
        /// 添加原始缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static bool Add(string key, GameObject value, byte type, bool isAsset)
        {
            int hashkey = LuaHelper.StringToHash(key);
            return Add(hashkey, value, type, isAsset);
        }

        internal static void DestroyOriginalPrefabs(GameObject obj)
        {
            if (assetsFlag.ContainsKey(obj))
            {
                assetsFlag.Remove(obj);
            }
            else
            {
                GameObject.Destroy(obj);
            }
        }

        private static void AddRemoveMark(int hash)
        {
            removeMark[hash] = Time.unscaledTime + 0.5f;
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("AddRemoveMark={0},hash={1},frame={2}", GetStringKey(hash), hash, Time.frameCount);
#endif
        }

        private static void DeleteRemveMark(int hash)
        {
            removeMark.Remove(hash);
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("DeleteRemveMark={0},hash={1},frame={2}", GetStringKey(hash), hash, Time.frameCount);
#endif
        }

        /// <summary>
        /// 添加原始项目
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        internal static bool Add(int hash, GameObject value, byte type, bool isAsset = false)
        {
            bool contains = ContainsKey(hash);

            if (!contains) //不能重复添加
            {
                if (isAsset) assetsFlag[value] = isAsset;
                originalPrefabs[hash] = value;
                if (type > SegmentSize) type = SegmentSize;
                prefabFreeQueue[hash] = new Queue<ReferGameObjects>(); //空闲队列
                prefabsType[hash] = type;
                prefabRefCaches[hash] = new HashSet<ReferGameObjects>();//引用列表

#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("PrefabPool.Add({0},key={1},hash={2}),type = {3},frame={4}", value.name, GetStringKey(hash), hash, type, Time.frameCount);
#endif
#if UNITY_EDITOR
                LuaHelper.RefreshShader(value);
#endif
                return true;
            }
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            else
                Debug.LogWarningFormat("PrefabPool.Add has contains({0}) key={1},frame={2} ", value.name, GetStringKey(hash), Time.frameCount);
#endif
            return false;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GameObject Get(string key)
        {
            int hash = LuaHelper.StringToHash(key);
            GameObject re = Get(hash);
            return re;
        }

        internal static GameObject Get(int hash)
        {
            GameObject obj = null;
            originalPrefabs.TryGetValue(hash, out obj);
            return obj;
        }

        /// <summary>
        /// 是否包含cacheKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
#if HUGULA_CACHE_DEBUG
            cacheKey1 = key;
#endif
            int hash = LuaHelper.StringToHash(key);
            return ContainsKey(hash);
        }

        internal static bool ContainsKey(int hash)
        {
            GameObject obj = null;
            originalPrefabs.TryGetValue(hash, out obj);
            bool re = false;
            if (obj)
                re = true;
            else
                re = false;
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("PrefabPool.ContainsKey is {0} key = {1}({2}),frame={3}",re,hash,cacheKey1, Time.frameCount);
#endif
            return re;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ReferGameObjects GetCache(string key)
        {
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            cacheKey = key;
#endif
            int hash = LuaHelper.StringToHash(key);
            return GetCache(hash);
        }

        /// <summary>
        /// 获取可用的实例
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static ReferGameObjects GetCache(int key)
        {
            //空闲队列
            Queue<ReferGameObjects> prefabQueueMe = null;
            if (!prefabFreeQueue.TryGetValue(key, out prefabQueueMe))
            {
                var keyString = GetStringKey(key);
                Debug.LogErrorFormat("prefabFreeQueue is null key = {0},keystr = {1},frame={2}", key, keyString, Time.frameCount);
                return null;
            }

            //引用列表
            HashSet<ReferGameObjects> prefabCachesMe = null;
            if (!prefabRefCaches.TryGetValue(key, out prefabCachesMe))
            {
                var keyString = GetStringKey(key);
                Debug.LogErrorFormat("prefabRefCaches is null key = {0},keystr={1},frame={2}", key, keyString, Time.frameCount);
                return null;
            }

            //移除回收引用
            DeleteRemveMark(key);

            ReferGameObjects refer = null;
            int i = 0;

            while (prefabQueueMe.Count > 0)//出列
            {
                refer = prefabQueueMe.Dequeue();
                if (refer)
                {
                    refer.cacheHash = key;
                    break;
                }
                i++;
            }

            if (refer == null)
            {
                GameObject prefab = Get(key);
                if (!prefab) //如果没有 返回NUll
                {
                    var keyString = GetStringKey(key);
                    Debug.LogErrorFormat("FATAL EXCEPTION : original gameobject is null key = {0},keystr={1},frame={2}", key, keyString, Time.frameCount);
                    return null;
                }

                GameObject clone = (GameObject)GameObject.Instantiate(prefab);
                var comp = clone.GetComponent<ReferGameObjects>();
                if (comp == null)
                {
                    comp = clone.AddComponent<ReferGameObjects>();
                }
                refer = comp;
                refer.cacheHash = key;
            }

            if (!refer)
            {
                var keyString = GetStringKey(key);
                Debug.LogErrorFormat("prefabQueueMe.Dequeue() is null key = {0},keystr={1},freeQueue.Count={2},reference.count={3},frame={4}", key, keyString, prefabQueueMe.Count, prefabCachesMe.Count, Time.frameCount);
            }
            else
            {
                prefabCachesMe.Add(refer); //保持引用
            }

#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            if (i > 0)
                Debug.LogErrorFormat("prefabQueueMe.Dequeue() contains null key={0}, keyint = {1},find count={2},freeQueue.Count={3},reference.count={4},frame={5}", cacheKey, key, i, prefabQueueMe.Count, prefabCachesMe.Count, Time.frameCount);
#endif
            return refer;
        }

        /// <summary>
        /// 放入缓存
        /// </summary>
        /// <param name="cop"></param>
        public static bool StoreCache(ReferGameObjects refer)
        {
            HashSet<ReferGameObjects> prefabRefers = null;
            int keyHash = refer.cacheHash;
            if (prefabRefCaches.TryGetValue(keyHash, out prefabRefers))
            {//从引用列表寻找
                bool isremove = prefabRefers.Remove(refer);
                if (isremove)
                {
                    Queue<ReferGameObjects> prefabFree = null;
                    if (prefabFreeQueue.TryGetValue(keyHash, out prefabFree) && !prefabFree.Contains(refer))
                    {
                        prefabFree.Enqueue(refer);
#if HUGULA_CACHE_DEBUG
                        Debug.LogFormat("StoreCache key={1},keystr={0},freeQueue.count={2},refers.count={3},frame={4}", refer.name+"   "+refer.cacheHash.ToString(), GetStringKey(keyHash),prefabFree.Count,prefabRefers.Count,Time.frameCount);
#endif
                        if (prefabRefers.Count == 0 && prefabsType[keyHash] < SegmentSize) AddRemoveMark(keyHash);//如果引用为0标记回收
                        return true;
                    }
                    else
                        Debug.LogWarningFormat("StoreCache refer(name={0},hash={1}) prefabFreeQueue is null ", refer.name, GetStringKey(refer.cacheHash));
                }
            }
#if HUGULA_CACHE_DEBUG || UNITY_EDITOR
            else
            {
                Debug.LogWarningFormat("StoreCache prefabRefCaches dont contains(refer(name={0},hash={1}) ) ", refer.name, GetStringKey(refer.cacheHash));
            }
#endif
            return false;
        }

        /// <summary>
        /// 放入缓存
        /// </summary>
        /// <param name="gobj"></param>
        /// <returns></returns>
        public static bool StoreCache(GameObject gobj)
        {
            ReferGameObjects refer = gobj.GetComponent<ReferGameObjects>();
            if (refer != null && refer.cacheHash != 0)
                return StoreCache(refer);
            return false;
        }

        /// <summary>
        /// Clears all cache.
        /// </summary>
        public static void ClearAllCache()
        {
            prefabsType.Clear();
            willGcList.Clear();
            removeMark.Clear();
            var freeValues = prefabFreeQueue.Values.GetEnumerator();
            while (freeValues.MoveNext())
            {
                var queue = freeValues.Current;
                while (queue.Count > 0)
                {
                    var refer = queue.Dequeue();
                    if (refer) GameObject.Destroy(refer.gameObject);
                }
            }
            prefabFreeQueue.Clear();

            var refValues = prefabRefCaches.Values.GetEnumerator();

            ReferGameObjects item;
            while (refValues.MoveNext())
            {
                var items = refValues.Current.GetEnumerator();
                while (items.MoveNext())
                {
                    item = items.Current;
                    if (item) GameObject.Destroy(item.gameObject);
                }
                items.Dispose();
            }
            prefabRefCaches.Clear();
        }

        private static string GetStringKey(int key)
        {
            var cache = Hugula.Loader.CacheManager.TryGetCache(key);
            if (cache != null)
                return cache.assetBundleKey + "   " + key.ToString();
            else
                return key.ToString() + "(null)";
        }

        internal static void Remove(int key)
        {
            GameObject obj = null;
            if (originalPrefabs.TryGetValue(key, out obj))
            {
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("Destroy OriginalPrefabs={0},frame={1}", GetStringKey(key), Time.frameCount);
#endif
                DestroyOriginalPrefabs(obj);
            }

            originalPrefabs.Remove(key);
            prefabsType.Remove(key);
            prefabFreeQueue.Remove(key);
            prefabRefCaches.Remove(key);


        }

        /// <summary>
        /// 标记删除 如果有引用不会被删除
        /// </summary>
        public static int MarkRemove(string key)
        {
            int hash = LuaHelper.StringToHash(key);
            int referCount = 0;
            HashSet<ReferGameObjects> refers = null;
            if (prefabRefCaches.TryGetValue(hash, out refers))
            {
                referCount = refers.Count;
            }
            if (referCount > 0)
            {
                AddRemoveMark(hash);
                // willGcList.Enqueue(hash);
            }
            return referCount;
        }



        /// <summary>
        /// 强行回收
        /// </summary>
        /// <param name="key"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public static bool ClearCacheImmediate(string key)
        {
            int hash = LuaHelper.StringToHash(key);
            // 
            HashSet<ReferGameObjects> refers = null;

            if (prefabRefCaches.TryGetValue(hash, out refers))
            {
                Queue<ReferGameObjects> freequeue;
                if (prefabFreeQueue.TryGetValue(hash, out freequeue))
                {
                    var referItem = refers.GetEnumerator();
                    while (referItem.MoveNext())
                    {
                        freequeue.Enqueue(referItem.Current);
                    }
                }
                refers.Clear();
            }
            ClearKey(hash);
            return true;
        }

        /// <summary>
        /// 清理当前key的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static void ClearKey(int key)
        {
            HashSet<ReferGameObjects> refers = null;
            if (prefabRefCaches.TryGetValue(key, out refers) && refers.Count == 0)
            {
                Queue<ReferGameObjects> freequeue;
                ReferGameObjects refer = null;
                bool hasFree = prefabFreeQueue.TryGetValue(key, out freequeue);
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat("ClearKey key={0},frame={1}", GetStringKey(key), Time.frameCount);
#endif
                Remove(key); //移除原始项目
                if (hasFree)
                {
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("ClearKey destroy freequeue  keystr={0},free.Count={1},frame={2}",GetStringKey(key),freequeue.Count,Time.frameCount);
#endif
                    while (freequeue.Count > 0)
                    {
                        refer = freequeue.Dequeue();
                        if (refer)
                        {
#if HUGULA_CACHE_DEBUG
                            Debug.LogFormat("ClearKey destroy gameobject  keystr={0},name={1},free.Count={2},frame={3}", GetStringKey(key), refer.name, freequeue.Count, Time.frameCount);
#endif
                            GameObject.Destroy(refer.gameObject);
                        }
#if HUGULA_CACHE_DEBUG || UNITY_EDITOR
                        else
                            Debug.LogWarningFormat("ClearKey freequeue item has been destory keystr={0},free.Count={1},frame={2}", GetStringKey(key), freequeue.Count, Time.frameCount);
#endif
                    }
                }
            }
#if UNITY_EDITOR
            else
                Debug.LogWarningFormat("ClearKey fail ,the object(name={1},hash={0}) has refers.count = {2},frame={3}", GetStringKey(key), Get(key) != null ? Get(key).name : "null", refers == null ? -1 : refers.Count, Time.frameCount);
#endif


        }

        /// <summary>
        /// 自动回收对象当前segmentindex和之前的所有片段
        /// </summary>
        internal static void AutoGC(byte segmentIndex, bool compareTime = true)
        {
            var items = removeMark.GetEnumerator();
            int key = 0;
            byte keyType = 0;
            while (items.MoveNext())
            {
                var kv = items.Current;
                key = kv.Key;
                keyType = 0;
                prefabsType.TryGetValue(key, out keyType);
                if (keyType <= segmentIndex && compareTime ? Time.unscaledTime >= kv.Value : true)
                {
#if HUGULA_CACHE_DEBUG
                    Debug.LogFormat("willGcList.Add({0}),frame={1}", GetStringKey(key), Time.frameCount);
#endif
                    if (!willGcList.Contains(key)) willGcList.Enqueue(key);
                }
            }
        }


        /// <summary>
        /// 手动回收 可以释放的对象
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <returns></returns>
        public static void GCCollect(byte segmentIndex)
        {
            AutoGC(segmentIndex, false);
        }

        #endregion

        #endregion

        #region gameobject
        private static GameObject _gameObject;

        public static GameObject instance
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject("PrefabPool");
                    _gameObject.AddComponent<PrefabPool>();
                }

                return _gameObject;
            }
        }
        #endregion
    }


    public static class HugulaProfiler
    {
        const float m_KBSize = 1024.0f * 1024.0f;

        public static float GetTotalAllocatedMemory()
        {
#if UNITY_2017
            float totalMemory = (float)(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / m_KBSize);
#else
            float totalMemory = (float)(Profiler.GetTotalAllocatedMemory() / m_KBSize);
#endif
            return totalMemory;
        }
    }
}