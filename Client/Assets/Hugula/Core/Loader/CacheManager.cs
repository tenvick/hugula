// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Hugula.Utils;

namespace Hugula.Loader
{


    /// <summary>
    /// 缓存资源
    /// </summary>
    public class CacheData : IDisposable
    {

        public CacheData()
        {
            state = CacheDataState.Empty;
        }

        public enum CacheDataState
        {
            Empty, //创建
            Loading,//开始加载
            Error,//加载出错
            Done, //加载完成
            Unloaded //已经被卸载
        }


        /// <summary>
        /// assetBundle Name
        /// </summary>
        public string assetBundleName { get; private set; }

        /// <summary>
        /// assetbundle对象
        /// </summary>
        public AssetBundle assetBundle { get; private set; }

        /// <summary>
        /// 当前引用数量
        /// </summary>
        public int count;

        /// <summary>
        /// 当前状态 
        /// </summary>
        public CacheDataState state { get; private set; }

        //能否使用
        public bool canUse
        {
            get
            {
                return state == CacheDataState.Done && assetBundle != null;
            }
        }

        //是否加载完成
        public bool isDone
        {
            get
            {
                return state == CacheDataState.Done || state == CacheDataState.Error;
            }
        }
        public void Dispose()
        {
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("Dispose  CacheData({0},)),frame={1}  ", assetBundleName, Time.frameCount);
#endif
            if (assetBundle) assetBundle.Unload(true); //Loading.LockPersistentManger Windows 141ms
            assetBundle = null;
            state = CacheDataState.Empty;

            assetBundleName = string.Empty;
            count = 0;
        }

        public void Unload()
        {
            if (assetBundle) assetBundle.Unload(false);
            state = CacheDataState.Unloaded;
#if HUGULA_CACHE_DEBUG
            Debug.LogFormat("Unload  CacheData({0})),frame={1}  ", assetBundleName, Time.frameCount);
#endif
        }

        #region  tool

        /// <summary>
        /// Set Cache Data 
        /// </summary>
        internal static void SetCacheData(CacheData cache, AssetBundle assetBundle, string assetBundleName)
        {
            cache.assetBundle = assetBundle;
            cache.assetBundleName = assetBundleName;
            cache.state = CacheDataState.Done;
        }

        /// <summary>
        /// 创建的时候设置assetbundle name
        /// </summary>
        internal static void SetCacheDataAssetBundleName(CacheData cache, string assetBundleName)
        {
            cache.assetBundleName = assetBundleName;
            cache.state = CacheDataState.Empty;
        }

        /// <summary>
        /// 加载出错的时候设置状态
        /// </summary>
        internal static void SetCacheDataError(CacheData cache)
        {
            cache.state = CacheDataState.Error;
        }

        /// <summary>
        /// 改变状态为loading 
        /// </summary>
        internal static void SetCacheDataLoding(CacheData cache)
        {
            cache.state = CacheDataState.Loading;
        }

        #endregion


        #region ObjectPool
        static ObjectPool<CacheData> pool = new ObjectPool<CacheData>(null, m_ActionOnRelease);

        private static void m_ActionOnGet(CacheData cd)
        {
            // cd.Dispose();
        }
        private static void m_ActionOnRelease(CacheData cd)
        {
            cd.Dispose();
        }

        public static CacheData Get()
        {
            return pool.Get();
        }

        public static void Release(CacheData toRelease)
        {
            pool.Release(toRelease);
        }
        #endregion
    }

    /// <summary>
    /// 缓存管理
    /// </summary>
    public static class CacheManager
    {
        /// <summary>
        /// 下载完成的assetbundle缓存
        /// </summary>
#if UNITY_EDITOR
        public static Dictionary<string, CacheData> m_Caches = new Dictionary<string, CacheData>();
#else
        internal static Dictionary<string, CacheData> m_Caches = new Dictionary<string, CacheData>();
#endif

        /// <summary>
        /// assetbundle依赖关系
        /// </summary>
        internal static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();

        /// <summary>
        /// 加载完成的场景
        /// </summary>
        static Dictionary<string, string> loadedScenes = new Dictionary<string, string>(5);

        /// <summary>
        /// 加载中的场景
        /// </summary>
        static Dictionary<string, AsyncOperation> loadingScenes = new Dictionary<string, AsyncOperation>(5);

        /// <summary>
        /// 改变cachedata状态为loading
        /// </summary>
        public static void SetCacheDataLoding(string assetbundle)
        {
            CacheData cache = TryGetCache(assetbundle);
            CacheData.SetCacheDataLoding(cache);
        }

        // /// <summary>
        // /// unload assetbundle false
        // /// </summary>
        // /// <param name="assetBundleName"></param>
        // public static bool UnloadCacheFalse(string key)
        // {
        //     CacheData cache = TryGetCache(key);
        //     if (cache != null)
        //     {
        //         cache.Unload();
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }

        /// <summary>
        /// 添加场景与assetbunlde的关系
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="abName"></param>
        /// <return></return>
        public static void AddScene(string sceneName, string abName)
        {
            loadedScenes[sceneName] = abName;
#if UNITY_EDITOR
            Debug.LogFormat("AddScene({0},{1})", sceneName, abName);
#endif
        }

        /// <summary>
        /// 添加场景与assetbunlde的关系
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="abName"></param>
        /// <return></return>
        public static void AddLoadingScene(string sceneName, AsyncOperation request)
        {
            loadingScenes[sceneName] = request;
#if UNITY_EDITOR
            Debug.LogFormat("AddLoadingScene({0},{1})", sceneName, request);
#endif
        }

        public static void RemoveLoadingScene(string sceneName)
        {
            loadingScenes.Remove(sceneName);
        }

        public static AsyncOperation GetLoadingScene(string sceneName)
        {
            AsyncOperation request = null;
            loadingScenes.TryGetValue(sceneName, out request);
            return request;
        }

        /// <summary>
        /// 获取场景的assetbundle可用于判断场景是否加载
        /// </summary>
        /// <param name="sceneName"></param>
        /// <return></return>
        public static string GetSceneBundle(string sceneName)
        {
            string abName = null;
            loadedScenes.TryGetValue(sceneName, out abName);
            return abName;
        }

        /// <summary>
        /// 卸载指定场景同时卸载assetbundle
        /// </summary>
        /// <param name="sceneName"></param>
        /// <return></return>
        public static AsyncOperation UnloadScene(string sceneName)
        {
            loadingScenes.Remove(sceneName);
            AsyncOperation async = null;
            string abName;
            if (loadedScenes.TryGetValue(sceneName, out abName))
            {
                loadedScenes.Remove(sceneName);
                async = SceneManager.UnloadSceneAsync(sceneName);//卸载场景
                Subtract(abName);//卸载ab
            }
            else
            {
                Debug.LogWarningFormat("the scene {0} you want to unload is no exist", sceneName);
            }
            return async;
        }

        /// <summary>
        /// 卸载所有加载了的场景但是不卸载assetbundle
        /// </summary>
        /// <param name="exclude">注意需要小写</param>
        /// <return></return>
        public static void UnloadAllScenes(string exclude = null)
        {
            loadingScenes.Clear();
            foreach (var kv in loadedScenes)
            {
                if (!kv.Key.ToLower().Equals(exclude))
                {
                    Scene scene = SceneManager.GetSceneByName(kv.Key);
                    if (scene != null && scene.IsValid())
                        SceneManager.UnloadSceneAsync(scene);//卸载场景
                    else
                        Debug.LogWarningFormat("Unload Scene {0} isInvalid ", kv.Key);
                }
            }
            loadedScenes.Clear();
        }

        /// <summary>
        /// 延时清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void Subtract(string key)
        {
            CacheData cached = TryGetCache(key);
            if (cached != null && cached.count >= 1)
            {
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat(" <color=#8cacbc>Subtract (assetBundle={0},count={1}) frameCount{2}</color>", cached.assetBundleName, cached.count, UnityEngine.Time.frameCount);
#endif
                if (--cached.count == 0) //所有引用被清理。
                {
                    ABDelayUnloadManager.Add(key);//放入回收队列
                }// end if (cached.count-- == 0)
            }
#if UNITY_EDITOR
            else if (!ManifestManager.SimulateAssetBundleInEditor)
            {
                Debug.LogWarningFormat("Subtract cacheData {0} is null ", key);
            }
#endif
        }

        /// <summary>
        /// 延时清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearDelay(string key)
        {
            CacheData cache = TryGetCache(key);
            if (cache != null)
            {
                // #if HUGULA_CACHE_DEBUG
                //                 Debug.LogFormat(" <color=#8cacbc>ClearDelay Cache (assetBundle={0}),frameCount{1}</color>", cache.key, Time.frameCount);
                // #endif
                ABDelayUnloadManager.Add(key);
            }
#if UNITY_EDITOR && !HUGULA_RELEASE
            else if (!ManifestManager.SimulateAssetBundleInEditor)
            {
                Debug.LogWarningFormat("ClearDelay Cache {0} is null ", key);
            }
#endif
        }

        /// <summary>
        /// 获取可以使用的缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        public static CacheData GetCache(string key)
        {
            CacheData cache = null;
            m_Caches.TryGetValue(key, out cache);
            if (cache != null && cache.canUse)
                return cache;
            else
                return null;
        }

        /// <summary>
        /// 获取存在的缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static CacheData TryGetCache(string abName)
        {
            CacheData cache = null;
            m_Caches.TryGetValue(abName, out cache);
            return cache;
        }

        /// <summary>
        /// 获取或者创建一个缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static bool CreateOrGetCache(string abName, out CacheData cache)
        {
            if (!m_Caches.TryGetValue(abName, out cache))
            {
                cache = CacheData.Get();
                CacheData.SetCacheDataAssetBundleName(cache, abName);
                m_Caches.Add(abName, cache);
            }

            return cache.canUse;
        }


        internal static bool AddCacheData(AssetBundle ab, string assetBundleName)
        {
            CacheData cacheData = null;
            CreateOrGetCache(assetBundleName, out cacheData);
            CacheData.SetCacheData(cacheData, ab, assetBundleName); //缓存
            return true;
        }

        internal static bool AddErrorCacheData(string assetBundleName)
        {
            CacheData cacheData = null;
            CreateOrGetCache(assetBundleName, out cacheData);
            CacheData.SetCacheDataError(cacheData); //缓存
            return true;
        }

        /// <summary>
        /// 添加依赖项目
        /// </summary>
        /// <param name="string">assetbundle name</param>
        /// <param name="string">dependencies</param>
        /// <returns></returns>
        internal static void AddDependencies(string abName, string[] dependencies)
        {
            m_Dependencies[abName] = dependencies;
        }

        /// <summary>
        /// 判断所有依赖项目是否加载完成
        /// </summary>
        /// <param name="string">assetbundle name</param>
        /// <param name="string">dependencies</param>
        /// <returns></returns>
        internal static string[] GetDependencies(string abName)
        {
            string[] dependencies = null;
            m_Dependencies.TryGetValue(abName, out dependencies);
            return dependencies;
        }

        /// <summary>
        /// 判断所有依赖项目是否加载完成
        /// </summary>
        /// <param name="dependencies"></param>
        /// <returns></returns>
        internal static bool CheckDependenciesComplete(string abName)
        {
            string[] deps = null;
            if (m_Dependencies.TryGetValue(abName, out deps))
            {
                CacheData cache = null;
                string hash = string.Empty;
                for (int i = 0; i < deps.Length; i++)
                {
                    hash = deps[i];
                    if (string.IsNullOrEmpty(hash))
                    {
                        continue;
                    }
                    else if (m_Caches.TryGetValue(hash, out cache))
                    {
                        if (!cache.isDone) // if (!cache.isAssetLoaded || !cache.canUse)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 安全卸载资源
        /// </summary>
        /// <returns></returns>
        public static bool UnloadSecurity(string abName)
        {
            CacheData cache = TryGetCache(abName);
            if (cache != null && cache.count == 0)
            {
#if HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat("<color=#ffff00> unload  cache assetBundle={0},count={1})   </color>", cache.assetBundleName, cache.count);
#endif
                //处理依赖项目
                string[] deps = null;
                if (m_Dependencies.TryGetValue(cache.assetBundleName, out deps))
                {
                    string tmpName;
                    CacheData cachedChild = null;
                    for (int i = 0; i < deps.Length; i++)
                    {
                        tmpName = deps[i];
                        if (m_Caches.TryGetValue(tmpName, out cachedChild) && cachedChild.count >= 1)
                        {
                            if (--cachedChild.count == 0)
                            {
                                ABDelayUnloadManager.AddDep(tmpName);
                            }
                            // ABDelayUnloadManager.Add(tmpName);
                        }
                    }
                }//end if
                m_Caches.Remove(cache.assetBundleName); //删除
                m_Dependencies.Remove(cache.assetBundleName);//依赖关系移除？
                CacheData.Release(cache);
                return true;
            }
#if UNITY_EDITOR
            else if (cache != null)
            {
                Debug.LogFormat("<color=#cccccc> can't unload  cache assetBundle={0},count={1})   </color>", cache.assetBundleName, cache.count);
            }
#endif
            return false;
        }

        /// <summary>
        /// 是否下载过资源
        /// </summary>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            CacheData cdata = GetCache(key);
            if (cdata != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public static void ClearAll()
        {
            var items = m_Caches.GetEnumerator();
            while (items.MoveNext())
            {
                items.Current.Value.Dispose();
            }

            m_Caches.Clear();
            m_Dependencies.Clear();

        }
    }

}