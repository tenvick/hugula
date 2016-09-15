// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_5_3 || UNITY_5_4
using UnityEngine.SceneManagement;
#endif
using Hugula.Utils;

namespace Hugula.Loader
{

    /// <summary>
    /// 缓存资源
    /// </summary>
    [SLua.CustomLuaClass]
    public class CacheData : IDisposable
    {

        public CacheData(object data, AssetBundle assetBundle, string assetBundleName)
        {
            this.www = data;
            this.assetBundle = assetBundle;
            this.assetBundleKey = assetBundleName;
            this.assetHashCode = LuaHelper.StringToHash(assetBundleName);
        }

        /// <summary>
        /// assetBundle Name
        /// </summary>
        public string assetBundleKey { get; private set; }

        /// <summary>
        /// hashcode
        /// </summary>
        public int assetHashCode { get; private set; }

        /// <summary>
        /// assetbundle对象
        /// </summary>
        public AssetBundle assetBundle;

        /// <summary>
        /// www data
        /// </summary>
        public object www;

        /// <summary>
        /// 当前引用数量
        /// </summary>
        [SLua.DoNotToLua]
        public int count;

        /// <summary>
        /// 所有依赖项目
        /// </summary>
        public int[] allDependencies { get; internal set; }

        /// <summary>
        /// is asset loaded
        /// </summary>
        /// <value><c>true</c> if is asset loaded; otherwise, <c>false</c>.</value>
        [SLua.DoNotToLua]
        public bool isAssetLoaded { get; internal set; }

        public void Dispose()
        {
            if (assetBundle) assetBundle.Unload(true);
            www = null;
            assetBundle = null;
            allDependencies = null;
        }
    }

    /// <summary>
    /// 缓存管理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CacheManager
    {
        /// <summary>
        /// 下载完成的资源
        /// </summary>
        [SLua.DoNotToLua]
        public static Dictionary<int, CacheData> caches = new Dictionary<int, CacheData>();

        /// <summary>
        /// 锁定的缓存key，锁定后不能删除（异步解压使用）。
        /// </summary>
        private static List<int> lockedCaches = new List<int>();

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="www"></param>
        internal static void AddCache(CacheData cacheData)
        {
            int assetHashCode = cacheData.assetHashCode;
            CacheData cacheout = null;
            if (caches.TryGetValue(assetHashCode, out cacheout))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("AddCache  {0} assetBundleName = {1} has exist", assetHashCode, cacheout.assetBundleKey));
#endif
                caches.Remove(assetHashCode);
            }
            caches.Add(assetHashCode, cacheData);
        }

        /// <summary>
        /// Sets the asset loaded.
        /// </summary>
        /// <param name="hashkey">Hashkey.</param>
        internal static void SetAssetLoaded(int hashkey)
        {
            CacheData cacheout = null;
            if (caches.TryGetValue(hashkey, out cacheout))
            {
                cacheout.isAssetLoaded = true;
            }
        }

        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="hashkey"></param>
		public static void AddLock(int hashkey)
        {
            lockedCaches.Add(hashkey);
        }

        /// <summary>
        /// 移除锁定
        /// </summary>
        /// <param name="hashkey"></param>
		public static void RemoveLock(int hashkey)
        {
            lockedCaches.Remove(hashkey);
        }

        /// <summary>
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearCache(string assetBundleName)
        {
            int hash = LuaHelper.StringToHash(assetBundleName);
            ClearCache(hash);
        }

        /// <summary>
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearCache(int assethashcode)
        {
            CacheData cache = null;
            caches.TryGetValue(assethashcode, out cache);
            if (cache != null)
            {
                if (lockedCaches.Contains(assethashcode)) //被锁定了不能删除
                {
#if UNITY_EDITOR
                    Debug.LogWarningFormat(" the cache ab({0},{1}) are locked,cant delete.) ", cache.assetBundleKey, cache.assetBundle);
#endif
                }
                else
                {
                    caches.Remove(assethashcode);//删除
                    int[] alldep = cache.allDependencies;
                    CacheData cachetmp = null;
                    cache.Dispose();
                    if (alldep != null)
                    {
                        for (int i = 0; i < alldep.Length; i++)
                        {
                            cachetmp = GetCache(alldep[i]);
                            if (cachetmp != null)
                            {
                                cachetmp.count--;// = cachetmp.count - 1; //因为被销毁了。
                                if (cachetmp.count <= 0)
                                    ClearCache(cachetmp.assetHashCode);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("ClearCache {0} fail ", assethashcode);
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static CacheData GetCache(string assetBundleName)
        {
            int hash = LuaHelper.StringToHash(assetBundleName);
            return GetCache(hash);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static CacheData GetCache(int assethashcode)
        {
            CacheData cache = null;
            caches.TryGetValue(assethashcode, out cache);
            return cache;
        }

        /// <summary>
        /// Adds the source cache data from WW.
        /// </summary>
        /// <param name="www">Www.</param>
        /// <param name="req">Req.</param>
		internal static bool AddSourceCacheDataFromWWW(WWW www, CRequest req)
        {
            object data = null;
            var ab = www.assetBundle;
			req.isAssetBundle = false;

			if (ab != null) {
				data = ab;
				req.isAssetBundle = true;
				CacheData cacheData = new CacheData (data, null, req.key);//缓存
				CacheManager.AddCache (cacheData);
				cacheData.allDependencies = req.allDependencies;
				cacheData.assetBundle = ab;
			} else if (Typeof_String.Equals (req.assetType)) {
				req.data = www.text;
			}
			else if(Typeof_AudioClip.Equals(req.assetType)) {
				req.data = www.audioClip;
			}else if(Typeof_Texture2D.Equals(req.assetType) ) {
				if (req.assetName.Equals ("textureNonReadable"))
					req.data = www.textureNonReadable;
				else
					req.data = www.texture;
			}
				
			if (Typeof_Bytes.Equals (req.assetType)) {
				req.data = www.bytes;
				req.isAssetBundle = false;
			}
            www.Dispose();
			return req.isAssetBundle;
        }

        /// <summary>
        /// 从缓存设置数据
        /// </summary>
        /// <param name="req"></param>
        public static bool SetRequestDataFromCache(CRequest req)
        {
            bool re = false;
            int keyhash = req.keyHashCode;
            CacheData cachedata = GetCache(keyhash);

            if (cachedata != null)
            {
                AssetBundle abundle = cachedata.assetBundle;
                System.Type assetType = req.assetType;
				if (assetType == null) assetType = Typeof_Object;
                if (req.isShared) //共享的
                {
                    req.data = abundle;
                    re = true;
                }
				else if (Typeof_AssetBundle.Equals(assetType))
                {
                    req.data = cachedata.assetBundle;
                    re = true;
                }
				else if (Typeof_ABScene.Equals(assetType))
                {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                	if (req.isAdditive)
                    	req.assetBundleRequest = Application.LoadLevelAdditiveAsync(req.assetName);
                	else
                    	req.assetBundleRequest = Application.LoadLevelAsync(req.assetName);
#else
                    req.assetBundleRequest = SceneManager.LoadSceneAsync(req.assetName, req.isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
#endif
                    re = true;
                }
                else
                {
                    if (abundle == null)
                    {
#if UNITY_EDITOR
                        Debug.LogWarningFormat("SetRequestDataFromCache Assetbundle is null request(url={0},assetName={1},assetType={2})  ", req.url, req.assetName, req.assetType);
#endif
                    }
                    else if (req.async)
                        req.assetBundleRequest = abundle.LoadAssetAsync(req.assetName, assetType);
                    else
                    {
                        req.data = abundle.LoadAsset(req.assetName, assetType);
                    }
                    re = true;
                }
            }

            return re;
        }

        /// <summary>
        /// 判断所有依赖项目是否加载完成
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckDependenciesComplete(CRequest req)
        {
            if (req.allDependencies == null || req.allDependencies.Length == 0) return true;

            int[] denps = req.allDependencies;
            CacheData cache = null;

            for (int i = 0; i < denps.Length; i++)
            {
                if (caches.TryGetValue(denps[i], out cache))
                {
                    if (!cache.isAssetLoaded)
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 是否下载过资源
        /// </summary>
        /// <returns></returns>
        public static bool Contains(int keyhash)
        {
            return caches.ContainsKey(keyhash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            int keyhash = LuaHelper.StringToHash(key);
            return Contains(keyhash);
        }

        /// <summary>
        /// 清理所有缓存
        /// </summary>
        public static void RemoveAllLock()
        {
            lockedCaches.Clear();
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public static void ClearAll()
        {
            lockedCaches.Clear();

            var items = caches.GetEnumerator();
            while(items.MoveNext())
            {
                items.Current.Value.Dispose();
            }

            caches.Clear();
        }

        #region check type
        public static readonly Type Typeof_String = typeof(System.String);
        public static readonly Type Typeof_Bytes = typeof(System.Byte[]);
        public static readonly Type Typeof_AssetBundle = typeof(AssetBundle);
        public static readonly Type Typeof_ABScene = typeof(AssetBundleScene);
		public static readonly Type Typeof_AudioClip = typeof(AudioClip);
		public static readonly Type Typeof_Texture2D = typeof(Texture2D);
		public static readonly Type Typeof_Object = typeof(UnityEngine.Object);

        #endregion
    }

    /// <summary>
    /// 计数器管理
    /// </summary>
    public static class CountMananger
    {
        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        internal static bool Subtract(int hashcode)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count--;// = cached.count - 1;
                if (cached.count <= 0) //所有引用被清理。
                {
                    CacheManager.ClearCache(hashcode);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        internal static bool Add(int hashcode)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count++;//= cached.count + 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 目标引用加n
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        internal static bool Add(int hashcode, int add)
        {
            CacheData cached = CacheManager.GetCache(hashcode);
            if (cached != null)
            {
                cached.count += add;//= cached.count + 1;
                return true;
            }
            return false;
        }

		/// <summary>
		/// Adds the dependencies.
		/// </summary>
		/// <param name="hashcode">Hashcode.</param>
		internal static void AddDependencies(int hashcode)
		{
			CacheData cached = CacheManager.GetCache(hashcode);
			if (cached != null && cached.allDependencies!=null)
			{
				foreach (int hash in cached.allDependencies)
					Add (hash);
			}
		}
    }

}