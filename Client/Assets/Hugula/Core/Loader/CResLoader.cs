// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
//

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using Hugula.Update;

namespace Hugula.Loader
{

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


        static UriGroup _uriList;
        /// <summary>
        /// The URI list.
        /// </summary>
        public static UriGroup uriList
        {
            get
            {
                if (_uriList == null)
                {
                    _uriList = new UriGroup();
                    _uriList.Add(CUtils.GetRealPersistentDataPath());
                    _uriList.Add(CUtils.GetRealStreamingAssetsPath());
                    _uriList.SetCrcIndex(0);
                }
                return _uriList;
            }
            set
            {
                _uriList = value;
            }
        }

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
        static Dictionary<CRequest, GroupRequestRecord> currGroupRequestsRef = new Dictionary<CRequest, GroupRequestRecord>();

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
                    load.Update();
                }
            }

            //ab
            for (int i = 0; i < loadingAssetBundleQueue.Count; )
            {
                var item = loadingAssetBundleQueue[i];
				if (CacheManager.CheckDependenciesComplete (item)) {//判断依赖项目是否加载完成
					CacheManager.SetRequestDataFromCache (item);//设置缓存数据。
					loadingAssetQueue.Add (item);
					if (item.assetBundleRequest != null)
						CacheManager.AddLock (item.keyHashCode);//异步需要锁定
					loadingAssetBundleQueue.RemoveAt (i);
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat (" 4. <color=yellow>DependenciesComplete Req(assetname={0},url={1}) frameCount{2}</color>", item.assetName, item.url, Time.frameCount);
					#endif
				} else {
					i++;
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat (" -4. <color=yellow>CheckDependenciesComplete Req(assetname={0},url={1}) frameCount{2}</color>", item.assetName, item.url, Time.frameCount);
					#endif
				}
            }

            //asset
            for (int i = 0; i < loadingAssetQueue.Count; i++)
            {
                var item = loadingAssetQueue[i];
                if (item.assetBundleRequest != null && item.assetBundleRequest.isDone) //如果加载完成
                {
                    if (item.assetBundleRequest is AssetBundleRequest)
                        item.data = ((AssetBundleRequest)item.assetBundleRequest).asset;//赋值
                    else
                        item.data = item.assetBundleRequest;
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat(" 5. <color=yellow>set Req(assetname={0},url={1}).data asnyc frameCount{2}</color>",item.assetName,item.url,Time.frameCount);
					#endif
                    loadedAssetQueue.Enqueue(item);
                    loadingAssetQueue.RemoveAt(i);
                }
                else if (item.assetBundleRequest == null) //非异步
                {
                    loadedAssetQueue.Enqueue(item);
                    loadingAssetQueue.RemoveAt(i);
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat(" 5. <color=yellow>set Req(assetname={0},url={1}).data frameCount{2}</color>",item.assetName,item.url,Time.frameCount);
					#endif
				}
                else
                    i++;
            }

            while (loadedAssetQueue.Count > 0)
            {
                LoadAssetComplate(loadedAssetQueue.Dequeue());
            }
        }

        #endregion

        #region static load

        /// <summary>
        /// 将多个资源加载到本地并缓存。
        /// </summary>
        /// <param name="req"></param>
        public void LoadReq(IList<CRequest> req, System.Action<object> onGroup)//onAllCompleteHandle onAllCompletehandle=null,onProgressHandle onProgresshandle=null
        {
            GroupRequestRecord groupFn = null;
            if (onGroup != null)
            {
                groupFn = GroupRequestRecordPool.Get();
                groupFn.onGroupComplate = onGroup;
            }
            for (int i = 0; i < req.Count; i++)
                AddReqToQueue(req[i], groupFn);

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

        protected static bool AddReqToQueue(CRequest req, GroupRequestRecord group = null)
        {
            if (req == null) return false;

            if (!CrcCheck.CheckUriCrc(req)) //如果校验失败
            {
#if HUGULA_LOADER_DEBUG
				Debug.LogFormat(" 0. <color=yellow>CheckCrcUri0Exists==false Req(assetname={0},url={1})  </color>",req.assetName,req.url);
#endif
				CallbackError(req);
                return false;
            }

            string key = req.udKey;//need re
            if (CheckLoadAssetAsync(req)) //已经下载
            {
                return false;
            }
            else if (requestCallBackList.ContainsKey(key)) //回调列表
            {
                requestCallBackList[key].Add(req);
				if (!req.isShared) {
					totalLoading++;
					if (group != null)
						PushGroup (req, group);
				}
                return true;
            }
            else
            {
                var listreqs = ListPool<CRequest>.Get(); //new List<CRequest>();
                requestCallBackList.Add(key, listreqs);
                listreqs.Add(req);

                if (queue.Size() == 0 && currentLoading == 0)
                {
                    totalLoading = 1;
                    currentLoaded = 0;
                }

				if (req.isShared) {
					realyLoadingQueue.Enqueue (req);
				} else {
					queue.Add (req);
					totalLoading++;
					if (group != null)
						PushGroup (req, group);
				}
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
                requestCallBackList.Remove(udkey);
                int count = callbacklist.Count;
                CRequest reqitem;
                for (int i = 0; i < count; i++)
                {
                    reqitem = callbacklist[i];
                    loadingAssetBundleQueue.Add(reqitem);
                }
				ListPool<CRequest>.Release(callbacklist);
				#if HUGULA_LOADER_DEBUG
				Debug.LogFormat("3. <color=green>  Add all to loadingAssetBundleQueue Req(assetname={0},url={1})  </color>",creq.assetName,creq.url);
				#endif
            }
            else
            {
                loadingAssetBundleQueue.Add(creq);
				#if HUGULA_LOADER_DEBUG
				Debug.LogFormat("3. <color=green> Add one to loadingAssetBundleQueue Req(assetname={0},url={1})  </color>",creq.assetName,creq.url);
				#endif
            }
        }

        protected static void CallbackError(CRequest creq)
         {
#if HUGULA_LOADER_DEBUG
			Debug.LogFormat("<color=green>CallbackError DispatchEnd Req(assetname={0},url={1})  </color>",creq.assetName,creq.url);
#endif
             List<CRequest> callbacklist = null;// requestCallBackList[creq.udKey];
            string udkey = creq.udKey;
			if (creq.isShared) {
				if (_instance.OnSharedErr != null)
					_instance.OnSharedErr (creq);

				if (creq.pool) LRequestPool.Release (creq);
			} else if (requestCallBackList.TryGetValue (udkey, out callbacklist)) {
				requestCallBackList.Remove (udkey);
				int count = callbacklist.Count;
				CRequest reqitem;
				for (int i = 0; i < count; i++) {
					reqitem = callbacklist [i];
					reqitem.DispatchEnd ();
					currentLoaded++;
					PopGroup (reqitem);
				}
				ListPool<CRequest>.Release (callbacklist);
			} else {
				creq.DispatchEnd();
				PopGroup(creq);
			}

            CheckAllComplete();
        }

        protected static void CheckAllComplete()
        {
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
            }
        }

        protected static void LoadAssetComplate(CRequest req)
        {
			#if HUGULA_LOADER_DEBUG
			Debug.LogFormat(" 6. <color=yellow>LoadAssetComplate Req(assetname={0},url={1}) frameCount{2}</color>",req.assetName,req.url,Time.frameCount);
			#endif
			if (req.isShared) {
				CountMananger.Add(req.keyHashCode);
				if (_instance && _instance.OnSharedComplete != null)
					_instance.OnSharedComplete(req);

				CacheManager.SetAssetLoaded (req.keyHashCode);
				if (req.pool) LRequestPool.Release (req);
			} else {
				currentLoaded++;
				ClearNoABCache (req);
				req.DispatchComplete ();
				PopGroup (req);
			}
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
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat("<color=yellow>from cache CheckLoadAssetAsync=true Req(assetname={0},url={1})  </color>",req.assetName,req.url);
					#endif
                }
                else
                {
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat("<color=yellow>from cache CheckLoadAssetAsync=false Req(assetname={0},url={1})  </color>",req.assetName,req.url);
					#endif
                    LoadAssetComplate(req);
                }
                return true;
            }

            return false;
        }

        static void PushGroup(CRequest req, GroupRequestRecord group)
        {
            currGroupRequestsRef[req] = group;
            group.Add(req);
        }

        static void PopGroup(CRequest req)
        {
            GroupRequestRecord group;
            if (currGroupRequestsRef.TryGetValue(req, out group))
            {
                currGroupRequestsRef.Remove(req);
                group.Complete(req);
                if (group.Count == 0)
                {
                    var act = group.onGroupComplate;
                    GroupRequestRecordPool.Release(group);
                    if (act != null)
                    {
                        act(req);
                    }
                }
            }

            if (req.pool) LRequestPool.Release(req);
        }

        /// <summary>
        /// 清理没有ab的缓存
        /// </summary>
        /// <param name="req"></param>
        static void ClearNoABCache(CRequest req)
        {
            if (req.assetBundleRequest != null) CacheManager.RemoveLock(req.keyHashCode);

            if (req.clearCacheOnComplete)
            {
#if HUGULA_LOADER_DEBUG
                Debug.LogFormat("<color=#8cacbc>{0} ClearCache</color>", req.key);
#endif
                CacheManager.ClearCache(req.keyHashCode);
            }
        }

        #endregion

        #region  eventhandle

        static void LoadComplete(CCar loader, CRequest creq)
        {
			#if HUGULA_LOADER_DEBUG
			Debug.LogFormat(" 2. <color=green>CResLoader.LoadComplete Request(assetName={0}, url={1},isShared={2})</color>", creq.assetName,creq.url,creq.isShared);
			#endif
            downloadings.Remove(creq.udKey);
            CallbackAsyncList(creq);
        }

        static void LoadError(CCar cloader, CRequest req)
        {
            string oldUdKey = req.udKey;
            downloadings.Remove(oldUdKey);
            req.index++;
			#if HUGULA_LOADER_DEBUG
			Debug.LogFormat(" 2.<color=green>CResLoader.LoadError Request(assetName={0}, url={1},isShared={2})</color>", req.assetName,req.url,req.isShared);
			#endif
            if (req.index < req.uris.count && req.uris.SetNextUri(req))// CUtils.SetRequestUri(req, req.index))
            {
                List<CRequest> callbacklist = null;
                if (requestCallBackList.TryGetValue(oldUdKey, out callbacklist))
                {
                    requestCallBackList.Remove(oldUdKey);
                }
                var udkey = req.udKey;
                if (callbacklist == null)
                    callbacklist = ListPool<CRequest>.Get();//new List<CRequest>();

                if (requestCallBackList.ContainsKey(udkey))
                { //if 正在加载
                    var old = requestCallBackList[udkey];

                    CRequest chekReq = null;
                    for (int i = 0; i < callbacklist.Count; i++)
                    {
                        chekReq = callbacklist[i];
                        if (!old.Contains(chekReq))
                            old.Add(chekReq);
                    }
                }
                else
                {
                    if (!callbacklist.Contains(req))
                        callbacklist.Add(req);
                    requestCallBackList[udkey] = callbacklist;
                    realyLoadingQueue.Enqueue(req);
                }

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
				req.allDependencies = LoadDependencies(req.assetBundleName); //加载依赖

            realyLoadingQueue.Enqueue(req);//加载自己

        }

        /// <summary>
        /// 加载依赖项目
        /// </summary>
        /// <param name="req"></param>
		static protected int[] LoadDependencies(string assetBundleName)
        {
            string[] deps = assetBundleManifest.GetAllDependencies(assetBundleName);

			if (deps.Length == 0) return null;
            string dep_url;
			string depAbName = "";
            CRequest item;
            int[] hashs = new int[deps.Length];
			int keyhash;

            for (int i = 0; i < deps.Length; i++)
            {
				depAbName = deps [i];
				dep_url = RemapVariantName(depAbName);
                keyhash = LuaHelper.StringToHash(CUtils.GetKeyURLFileName(dep_url));
                hashs[i] = keyhash;
				if (CacheManager.Contains (keyhash)) {
					CountMananger.Add (keyhash); //引用数量加1
				} else {
					item = LRequestPool.Get(); //new CRequest(dep_url);
                    item.relativeUrl = dep_url;
                    item.isShared = true;
                    item.async = false;
					//依赖项目
					string[] depds = assetBundleManifest.GetAllDependencies(item.assetBundleName);
					if(depds.Length > 0)
					{
						int[] hash1s = new int[depds.Length];
						for (int i1 = 0; i1 < depds.Length; i1++)
						{
							depAbName = depds [i1];
							dep_url = RemapVariantName(depAbName);
							keyhash = LuaHelper.StringToHash(CUtils.GetKeyURLFileName(dep_url));
							hash1s[i1] = keyhash;
						}
						item.allDependencies = hash1s;
					}
					#if HUGULA_LOADER_DEBUG
					Debug.LogFormat("<color=yellow> Req(assetname={0}) /r/n Begin LoadDependencies Req(assetname={1},url={2}) frameCount{3}</color>",assetBundleName,item.assetName,item.url,Time.frameCount);
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

        public System.Action<CResLoader> OnAllComplete;
        public System.Action<CResLoader> OnGroupComplete;
        public System.Action<CResLoader, LoadingEventArg> OnProgress;
        public System.Action<CRequest> OnSharedComplete;
		public System.Action<CRequest> OnSharedErr;

        #endregion

        #region instance
        protected static CResLoader _instance;
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

    public class GroupRequestRecordPool
    {
        static ObjectPool<GroupRequestRecord> pool = new ObjectPool<GroupRequestRecord>(null, m_ActionOnRelease);

        private static void m_ActionOnRelease(GroupRequestRecord re)
        {
            re.Count = 0;
            re.onGroupComplate = null;
        }

        public static GroupRequestRecord Get()
        {
            return pool.Get();
        }

        public static void Release(GroupRequestRecord toRelease)
        {
            pool.Release(toRelease);
        }
    }

    public class GroupRequestRecord
    {
        public System.Action<object> onGroupComplate;

        HashSet<CRequest> groupRes = new HashSet<CRequest>();

        public void Add(CRequest req)
        {
            groupRes.Add(req);
        }

        public void Complete(CRequest req)
        {
            groupRes.Remove(req);
        }

        public int Count
        {
            get
            {
                return groupRes.Count;
            }
            set
            {
                groupRes.Clear();
            }
        }

    }

}