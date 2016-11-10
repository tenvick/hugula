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

        #region instance
        private static int removeCount = 1;
        private static float lastGcTime = 0;//上传检测GC时间
        private static float gcDeltaTime = 0;//上传检测GC时间

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// 内存阈值检测和自动对象回收
        /// </summary>
        void Update() //每帧删除
        {
            if (willGcList.Count == 0) //如果正在gc不需要判断
            {
                gcDeltaTime = Time.time - lastGcTime;
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
                    else
                    {
                        gcSegmentQueue.Clear(); //不需要回收了
                    }
                }

                if (gcSegmentQueue.Count > 0) //队列有需要回收的片段
                {
                    byte segmentIndex = gcSegmentQueue.Dequeue();
                    GC(segmentIndex, false);
                }
            }



            if (willGcList.Count > 0) //如果有需要回收的gameobject
            {
                if (Time.deltaTime >= deltaTime30)
                {
                    removeCount = 5;
                }
                else if (Time.deltaTime >= deltaTime25)
                {
                    removeCount = 3;
                }
                else if (Time.deltaTime >= deltaTime20)
                {
                    removeCount = 2;
                }
                else if (Time.deltaTime >= deltaTime15)
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
            ReferGameObjects referRemove;//要删除的项目
            while (count > 0)
            {
                referRemove = willGcList.Dequeue();
                //Debug.Log("Dispose " + referRemove.name);
                if (referRemove) GameObject.Destroy(referRemove.gameObject);
                count--;
            }
            referRemove = null;

        }

		void OnDestroy()
		{
			if (_gameObject == this.gameObject)
				_gameObject = null;
			ClearAllCache ();
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
        /// 自动回收的类别
        /// </summary>
        private const byte SegmentGC = 6;

        /// <summary>
        /// 片段回收顺序队列
        /// </summary>
        private static Queue<byte> gcSegmentQueue = new Queue<byte>();

        /// <summary>
        /// 回收列表
        /// </summary>
        private static Queue<ReferGameObjects> willGcList = new Queue<ReferGameObjects>();

        /// <summary>
        /// 原始缓存
        /// </summary>
        private static Dictionary<int, GameObject> originalPrefabs = new Dictionary<int, GameObject>();

        /// <summary>
        /// 类型用于做回收策略
        /// </summary>
        private static Dictionary<int, byte> prefabsType = new Dictionary<int, byte>();

        /// <summary>
        /// 按照类型回收
        /// </summary>
        private static List<List<int>> segment = new List<List<int>>();

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
                GameObject.Destroy(item);
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
        /// 添加原始项目
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="value"></param>
        internal static bool Add(int hash, GameObject value, byte type)
        {
            bool contains = originalPrefabs.ContainsKey(hash);
            if (!contains) //不能重复添加
            {
                if (type > SegmentSize) type = SegmentSize;
                LuaHelper.AddComponent(value,typeof(ReferGameObjects));
                originalPrefabs[hash] = value;
                prefabFreeQueue[hash] = new Queue<ReferGameObjects>(); //空闲队列
                prefabsType[hash] = type;
                prefabRefCaches[hash] = new HashSet<ReferGameObjects>();//引用列表

                if (type <= SegmentSize)
                {
                    for (int i = segment.Count; i <= type; i++)
                        segment.Add(new List<int>());

                    segment[type].Add(hash);
                }

#if UNITY_EDITOR
                LuaHelper.RefreshShader(value);
#endif
                return true;
            }
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
            int hash = LuaHelper.StringToHash(key);
            return ContainsKey(hash);
        }

        internal static bool ContainsKey(int hash)
        {
            return originalPrefabs.ContainsKey(hash);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ReferGameObjects GetCache(string key)
        {
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
            ReferGameObjects refer = null;
            //空闲队列
            Queue<ReferGameObjects> prefabQueueMe = null;
            if (!prefabFreeQueue.TryGetValue(key, out prefabQueueMe)) return null;

            //引用列表
            HashSet<ReferGameObjects> prefabCachesMe = null;
            if (!prefabRefCaches.TryGetValue(key, out prefabCachesMe)) return null;

            if (prefabQueueMe.Count > 0)
            {
                refer = prefabQueueMe.Dequeue();//出列
                //保持引用
                prefabCachesMe.Add(refer);
                return refer;
            }
            else
            {
                GameObject prefab = Get(key);
                if (prefab == null) //如果没有 返回NUll
                    return null;

                GameObject clone = (GameObject)GameObject.Instantiate(prefab);
                var comp = clone.GetComponent<ReferGameObjects>();
                if (comp == null)
                {
                    comp = clone.AddComponent<ReferGameObjects>();
                }
                refer = comp;
                refer.cacheHash = key;
                //保持引用
                prefabCachesMe.Add(refer); //放入引用列表
            }

            return refer;
        }

        /// <summary>
        /// 放入缓存
        /// </summary>
        /// <param name="cop"></param>
        public static bool StoreCache(ReferGameObjects refer)
        {
            HashSet<ReferGameObjects> prefabCachesMe = null;
            if (prefabRefCaches.TryGetValue(refer.cacheHash, out prefabCachesMe))
            {//从引用列表寻找
                bool isremove = prefabCachesMe.Remove(refer);
                if (isremove)
                {
                    Queue<ReferGameObjects> prefabQueueMe = null;
                    if (prefabFreeQueue.TryGetValue(refer.cacheHash, out prefabQueueMe))
                    {
                        prefabQueueMe.Enqueue(refer);//入列
                        return true;
                    }
                }
            }
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

        public static void Remove(string key)
        {
            int hash = LuaHelper.StringToHash(key);
            Remove(hash);
        }

		/// <summary>
		/// Clears all cache.
		/// </summary>
		public static void ClearAllCache()
		{
			prefabsType.Clear();
			willGcList.Clear ();
			var freeValues = prefabFreeQueue.Values.GetEnumerator();
			while (freeValues.MoveNext ()) {
				var queue = freeValues.Current;
				while (queue.Count > 0)
				{
					var refer = queue.Dequeue();
					if(refer)GameObject.Destroy(refer.gameObject);
				}
			}
			prefabFreeQueue.Clear ();

			var refValues = prefabRefCaches.Values.GetEnumerator ();
			HashSet<ReferGameObjects> reflist;
			ReferGameObjects item;
			while (refValues.MoveNext ()) {
				var items = refValues.Current.GetEnumerator ();
				while (items.MoveNext())
				{
					item = items.Current;
					if(item)GameObject.Destroy(item.gameObject);
				}
				items.Dispose ();
			}
			prefabRefCaches.Clear ();
		}

        internal static void Remove(int key)
        {
            GameObject obj = null;
            if (originalPrefabs.TryGetValue(key, out obj))
            {
                Object.Destroy(obj);
                //Object.DestroyImmediate(obj, true);
            }

            originalPrefabs.Remove(key);
            prefabsType.Remove(key);
            prefabFreeQueue.Remove(key);
            prefabRefCaches.Remove(key);

            //if (obj != null)
            //{
            //    //Debug.Log(string.Format(" ClearChache original key {0} ", obj.name));
            //    GameObject.Destroy(obj);
            //}
        }

        /// <summary>
        /// 清理当前key的缓存实例
        /// </summary>
        /// <param name="key"></param>
        internal static bool ClearCache(int key, bool force)
        {
            if (CanGC(key, force))
            {
                Queue<ReferGameObjects> freequeue;
                ReferGameObjects refer = null;
                if (prefabFreeQueue.TryGetValue(key, out freequeue))
                {
                    while (freequeue.Count > 0)
                    {
                        refer = freequeue.Dequeue();
                        willGcList.Enqueue(refer);//入队
                    }
                }
#if UNITY_EDITOR
                //Debug.LogFormat("<color=yellow>ClearCache {0} key{1}</color>", refer == null ? "" : refer.name, refer.GetComponent<ReferenceCount>().assetBundleName);
#endif
                Remove(key); //移除原始项目
                return true;
            }

            return false;
        }

        public static bool ClearCache(string key)
        {
            int hash = LuaHelper.StringToHash(key);
            return ClearCache(hash, true);
        }

        /// <summary>
        /// 能否回收
        /// </summary>
        /// <param name="key"></param>
        /// <param name="force"></param>
        /// <returns></returns>
         internal static bool CanGC(int key, bool force)
        {
            HashSet<ReferGameObjects> reflist;
            prefabRefCaches.TryGetValue(key, out reflist);

            Queue<ReferGameObjects> prefabQueueMe = null;
            prefabFreeQueue.TryGetValue(key, out prefabQueueMe);

            if (prefabQueueMe == null || reflist == null) return true; //不合法需要回收

            if (force && reflist.Count == 0) return true; //如果是强行回收 没有引用就可以回收

            if (reflist.Count <= 0 && prefabQueueMe.Count > 0) return true;//非强行回收，没有引用且使用过

            return false;
        }

        /// <summary>
        /// 自动回收对象当前segmentindex和之前的所有片段
        /// </summary>
        internal static void AutoGC(byte segmentIndex)
        {
            if (segmentIndex > SegmentGC) segmentIndex = SegmentGC;//不能超过最大值
            if (segmentIndex > (byte)segment.Count) segmentIndex = (byte)segment.Count;//不能超过现有的最大值
            gcSegmentQueue.Clear();//清理以前的队列
            for (byte i = 0; i <= segmentIndex; i++) //
            {
                gcSegmentQueue.Enqueue(i);
            }
            //Debug.Log(string.Format("AutoGC segmentSize = {0} count = {1} ", segmentSize, gcSegmentQueue.Count));
        }

        /// <summary>
        /// 回收当前片段所有可回收对象
        /// </summary>
        /// <param name="segmentIndex">片段索引</param>
        /// <param name="force"> 表示强行回收 </param>
        internal static bool GC(byte segmentIndex, bool force)
        {
            List<int> checkGcList = null;
            bool checkhas = false;

            if (segmentIndex < segment.Count)
            {
                checkGcList = segment[segmentIndex];
                _removedList.Clear();
                for (int j = 0; j < checkGcList.Count; j++)
                {
                    int hash = checkGcList[j];
                    if (ClearCache(hash, force))
                    {
                        _removedList.Add(hash);
                    }
                }

                for (int j = 0; j < _removedList.Count; j++)
                {
                    checkGcList.Remove(_removedList[j]);
                    checkhas = true;//有对象可回收
                }
            }
            lastGcTime = Time.time;
            //Debug.Log(string.Format("GC {0} segmentIndex {1} checkhas{2} ", segmentIndex, checkGcList == null ? 0 : checkGcList.Count, checkhas));
            return checkhas;
        }

        /// <summary>
        /// 手动回收 可以释放的对象
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <returns></returns>
        public static bool GCCollect(byte segmentIndex)
        {
            return GC(segmentIndex, true);
        }

        #region tmp var
        private static List<int> _removedList = new List<int>();

        #endregion

        #endregion

        #endregion

		#region gameobject
		private static GameObject _gameObject;

		public static GameObject instance
		{
			get {
				if (_gameObject == null) {
					_gameObject = new GameObject ("PrefabPool");
					_gameObject.AddComponent<PrefabPool> ();
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
            float totalMemory = (float)(Profiler.GetTotalAllocatedMemory() / m_KBSize);
            return totalMemory;
        }
    }
}