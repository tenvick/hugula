// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2

#else
using UnityEngine.SceneManagement;
#endif
using Hugula.Utils;
namespace Hugula.Loader
{
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
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearDelay(string assetBundleName)
        {
            assetBundleName = ManifestManager.RemapVariantName(assetBundleName);
            int hash = LuaHelper.StringToHash(assetBundleName);
            ClearDelay(hash);
        }

        /// <summary>
        /// unload assetbundle false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadCacheFalse(string assetBundleName)
        {
            assetBundleName = ManifestManager.RemapVariantName(assetBundleName);
            int hash = LuaHelper.StringToHash(assetBundleName);
            if (!UnloadCacheFalse(hash))
            {
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogWarningFormat (assetBundleName,"Unload Cache False {0} fail is null or locked ", assetBundleName);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// unload assetbundle false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadCacheFalse(int assethashcode)
        {
            CacheData cache = TryGetCache(assethashcode);
            if (cache != null)
            {
                cache.Unload();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// unload assetbundle and Dependencies false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadDependenciesCacheFalse(string assetBundleName)
        {
            assetBundleName = ManifestManager.RemapVariantName(assetBundleName);
            int hash = LuaHelper.StringToHash(assetBundleName);
            if (!UnloadDependenciesCacheFalse(hash))
            {
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogWarningFormat(assetBundleName, "Unload Dependencies Cache False {0} fail is null or locked ", assetBundleName);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// unload assetbundle and Dependencies false
        /// </summary>
        /// <param name="assethashcode"></param>
        public static bool UnloadDependenciesCacheFalse(int assethashcode)
        {
            CacheData cache = TryGetCache(assethashcode);

            if (cache != null)
            {
                int[] alldep = cache.dependencies;
                CacheData cachetmp = null;
                cache.Unload();
                if (alldep != null)
                {
                    for (int i = 0; i < alldep.Length; i++)
                    {
                        cachetmp = TryGetCache(alldep[i]);
                        if (cachetmp != null)
                        {
                            cachetmp.Unload();
                        }
                    }
                }
                return true;
            }
            else
            {
#if HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat("Unload Dependencies Cache False {0} fail is null or locked ", assethashcode);
#endif
                return false;
            }
        }

        /// <summary>
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearDelay(int assethashcode)
        {
            CacheData cache = TryGetCache(assethashcode);

            if (cache != null)
            {
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogWarningFormat(cache.assetBundleKey," <color=#8cacbc>ClearDelay Cache (assetBundle={0},hash={1}),frameCount{2}</color>", cache.assetBundleKey,assethashcode, Time.frameCount);
#endif
                ABDelayUnloadManager.Add(assethashcode);
            }
            else
            {
#if UNITY_EDITOR || !HUGULA_RELEASE
                Debug.LogWarningFormat("ClearCache {0} fail ", assethashcode);
#endif
            }
        }

        /// <summary>
        /// 获取可以使用的缓存
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        [SLua.DoNotToLua]
        public static CacheData GetCache(string assetBundleName)
        {
            int hash = LuaHelper.StringToHash(assetBundleName);
            return GetCache(hash);
        }

        /// <summary>
        /// 获取可以使用的缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static CacheData GetCache(int assethashcode)
        {
            CacheData cache = null;
            caches.TryGetValue(assethashcode, out cache);
            if (cache != null && !cache.isError && cache.canUse)
                return cache;
            else
                return null;
        }

        /// <summary>
        /// 获取存在的缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static CacheData TryGetCache(int assethashcode)
        {
            CacheData cache = null;
            caches.TryGetValue(assethashcode, out cache);
            return cache;
        }

        /// <summary>
        /// 获取或者创建一个缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static bool CreateOrGetCache(int keyhashcode, out CacheData cache)
        {
            if (caches.TryGetValue(keyhashcode, out cache))
            {

            }
            else
            {
                cache = CacheData.Get();
                cache.assetHashCode = keyhashcode;
                caches.Add(keyhashcode, cache);
            }

            return cache.canUse;
        }

        internal static bool CreateOrGetCache(string key, out CacheData cache)
        {

            int keyhashcode = LuaHelper.StringToHash(key);
            if (caches.TryGetValue(keyhashcode, out cache))
            {

            }
            else
            {
                cache = CacheData.Get();
                cache.SetCacheData(null, key,keyhashcode);
                caches.Add(cache.assetHashCode, cache);
            }

            return cache.canUse;
        }

        internal static bool AddSourceCacheDataFromWWW(AssetBundle ab, CRequest req)
        {
            CacheData cacheData = null;
            CreateOrGetCache(req.keyHashCode, out cacheData);
            cacheData.SetCacheData(ab, req.key,req.keyHashCode); //缓存
            cacheData.dependencies = req.dependencies;
            cacheData.isDone = true;
#if HUGULA_CACHE_DEBUG
            HugulaDebug.FilterLogFormat(req.key," <color=#ffffff>LoadDone (assetBundle={0},hash={1},count={2})  frameCount{3}</color>", cacheData.assetBundleKey, req.keyHashCode, cacheData.count, UnityEngine.Time.frameCount);
#endif
            return true;

        }

        internal static bool AddErrorSourceCacheDataFromReq(CRequest req)
        {
            CacheData cacheData = null;
            CreateOrGetCache(req.keyHashCode, out cacheData);
            cacheData.SetCacheData(null, req.key,req.keyHashCode); //缓存
            cacheData.dependencies = req.dependencies;
            cacheData.isError = true;
            cacheData.isDone = true;
            return true;
        }

        /// <summary>
        /// 判断所有依赖项目是否加载完成
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        internal static bool CheckDependenciesComplete(CRequest req)
        {
            if (req.dependencies == null || req.dependencies.Length == 0) return true;

            int[] denps = req.dependencies;
            CacheData cache = null;
            int hash = 0;
            for (int i = 0; i < denps.Length; i++)
            {
                hash = denps[i];
                if(hash == 0)
                {
                   continue;     
                }
                else if (caches.TryGetValue(hash, out cache))
                {
                    if (!cache.isDone) // if (!cache.isAssetLoaded || !cache.canUse)
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 立即卸载资源
        /// </summary>
        /// <returns></returns>
        public static bool Unload(string key)
        {
            int keyhash = LuaHelper.StringToHash(key);
            return Unload(keyhash);
        }

        /// <summary>
        /// 立即卸载资源
        /// </summary>
        /// <returns></returns>
        public static bool Unload(int hashcode)
        {
            CacheData cache = TryGetCache(hashcode);
            if (cache != null && cache.count == 0)
            {
#if UNITY_EDITOR
                // Debug.LogWarningFormat ("<color=#ffff00> unload  cache assetBundle={0},keyhashcode({1},count={2})   </color>", cache.assetBundleKey, cache.assetHashCode, cache.count);
#endif
                caches.Remove(cache.assetHashCode); //删除
                CacheData.Release(cache);
                return true;
            }
#if UNITY_EDITOR
            else if (cache != null)
            {
                Debug.LogFormat("<color=#cccccc> can't unload  cache assetBundle={0},keyhashcode({1},count={2})   </color>", cache.assetBundleKey, cache.assetHashCode, cache.count);
            }
#endif
            return false;
        }

        /// <summary>
        /// 是否下载过资源
        /// </summary>
        /// <returns></returns>
        public static bool Contains(int keyhash)
        {
            CacheData cdata = GetCache(keyhash);
            if (cdata != null)
                return true;
            else
                return false;
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
        /// 清理所有资源
        /// </summary>
        public static void ClearAll()
        {

            var items = caches.GetEnumerator();
            while (items.MoveNext())
            {
                items.Current.Value.Dispose();
            }

            caches.Clear();
        }
       
    }
}