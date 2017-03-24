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
namespace Hugula.Loader {
    /// <summary>
    /// 缓存管理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CacheManager {
        /// <summary>
        /// 下载完成的资源
        /// </summary>
        [SLua.DoNotToLua]
        public static Dictionary<int, CacheData> caches = new Dictionary<int, CacheData> ();

        /// <summary>
        /// Sets the asset loaded.
        /// </summary>
        /// <param name="hashkey">Hashkey.</param>
        internal static void SetAssetLoaded (int hashkey) {
            CacheData cacheout = null;
            if (caches.TryGetValue (hashkey, out cacheout)) {
                cacheout.isAssetLoaded = true;
            }
#if UNITY_EDITOR
            else {
                Debug.LogWarningFormat ("SetAssetLoaded false ({0})", hashkey);
            }
#endif
        }

        /// <summary>
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearCache (string assetBundleName) {
            int hash = LuaHelper.StringToHash (assetBundleName);
            ClearCache (hash);
        }

        /// <summary>
        /// unload assetbundle false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadCacheFalse (string assetBundleName) {
            int hash = LuaHelper.StringToHash (assetBundleName);
            if (!UnloadCacheFalse (hash)) {
#if HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat ("Unload Cache False {0} fail is null or locked ", assetBundleName);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// unload assetbundle false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadCacheFalse (int assethashcode) {
            CacheData cache = TryGetCache (assethashcode);
            if (cache != null) {
                cache.Unload ();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// unload assetbundle and Dependencies false
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static bool UnloadDependenciesCacheFalse (string assetBundleName) {
            int hash = LuaHelper.StringToHash (assetBundleName);
            if (!UnloadDependenciesCacheFalse (hash)) {
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat ("Unload Dependencies Cache False {0} fail is null or locked ", assetBundleName);
#endif
                return false;
            }
            return true;
        }

        /// <summary>
        /// unload assetbundle and Dependencies false
        /// </summary>
        /// <param name="assethashcode"></param>
        public static bool UnloadDependenciesCacheFalse (int assethashcode) {
            CacheData cache = TryGetCache (assethashcode);

            if (cache != null) {
                int[] alldep = cache.allDependencies;
                CacheData cachetmp = null;
                cache.Unload ();
                if (alldep != null) {
                    for (int i = 0; i < alldep.Length; i++) {
                        cachetmp = TryGetCache (alldep[i]);
                        if (cachetmp != null) {
                            cachetmp.Unload ();
                        }
                    }
                }
                return true;
            } else {
#if HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat ("Unload Dependencies Cache False {0} fail is null or locked ", assethashcode);
#endif
                return false;
            }
        }

        /// <summary>
        /// 清理缓存释放资源
        /// </summary>
        /// <param name="assetBundleName"></param>
        public static void ClearCache (int assethashcode) {
            CacheData cache = TryGetCache (assethashcode);

            if (cache != null) {
#if HUGULA_CACHE_DEBUG
                Debug.LogFormat (" <color=#8cacbc>ClearCache (assetBundle={0}) frameCount{1}</color>", cache.assetBundleKey, Time.frameCount);
#endif
                ABDelayUnloadManager.Add (assethashcode);
            } else {
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
                Debug.LogWarningFormat ("ClearCache {0} fail ", assethashcode);
#endif
            }
        }

        /// <summary>
        /// 获取可以使用的缓存
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        [SLua.DoNotToLua]
        public static CacheData GetCache (string assetBundleName) {
            int hash = LuaHelper.StringToHash (assetBundleName);
            return GetCache (hash);
        }

        /// <summary>
        /// 获取可以使用的缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static CacheData GetCache (int assethashcode) {
            CacheData cache = null;
            caches.TryGetValue (assethashcode, out cache);
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
        internal static CacheData TryGetCache (int assethashcode) {
            CacheData cache = null;
            caches.TryGetValue (assethashcode, out cache);
            return cache;
        }

        /// <summary>
        /// 获取或者创建一个缓存
        /// </summary>
        /// <param name="assethashcode"></param>
        /// <returns></returns>
        internal static bool CreateOrGetCache (int keyhashcode, out CacheData cache) {
            // CacheData cache = null;
            if (caches.TryGetValue (keyhashcode, out cache)) {

            } else {
                cache = CacheData.Get ();
                cache.assetHashCode = keyhashcode;
                caches.Add (keyhashcode, cache);
            }

            return cache.canUse;
        }

        internal static bool CreateOrGetCache (string key, out CacheData cache) {

            int keyhashcode = LuaHelper.StringToHash (key);
            if (caches.TryGetValue (keyhashcode, out cache)) {

            } else {
                cache = CacheData.Get ();
                cache.SetCacheData (null, key);
                caches.Add (cache.assetHashCode, cache);
            }

            return cache.canUse;
        }

        /// <summary>
        /// Adds the source cache data from WW.
        /// </summary>
        /// <param name="www">Www.</param>
        /// <param name="req">Req.</param>
        internal static bool AddSourceCacheDataFromWWW (WWW www, CRequest req) {
            var ab = www.assetBundle;
            req.isAssetBundle = false;

            if (ab != null) {
                AddSourceCacheDataFromWWW (ab, req);
            } else if (Typeof_String.Equals (req.assetType)) {
                req.data = www.text;
            } else if (Typeof_AudioClip.Equals (req.assetType)) {
                req.data = www.audioClip;
            } else if (Typeof_Texture2D.Equals (req.assetType)) {
                if (req.assetName.Equals ("textureNonReadable"))
                    req.data = www.textureNonReadable;
                else
                    req.data = www.texture;
            }

            if (Typeof_Bytes.Equals (req.assetType)) {
                req.data = www.bytes;
                req.isAssetBundle = false;
            }

            UriGroup.CheckWWWComplete (req, www);
            www.Dispose ();

            return req.isAssetBundle;
        }

        internal static bool AddSourceCacheDataFromWWW (AssetBundle ab, CRequest req) {
            if (ab) {
                CacheData cacheData = null;
                CreateOrGetCache (req.keyHashCode, out cacheData);
                req.isAssetBundle = true;
                cacheData.SetCacheData (ab, req.key); //缓存
                cacheData.allDependencies = req.allDependencies;
#if HUGULA_CACHE_DEBUG
                UnityEngine.Debug.LogFormat (" <color=#ffffff>LoadDone add  (assetBundle={0},hash={1},count={2})  frameCount{3}</color>", cacheData.assetBundleKey, req.keyHashCode, cacheData.count, UnityEngine.Time.frameCount);
#endif
                return req.isAssetBundle;
            } else {
                req.isAssetBundle = false;
                return false;
            }
        }

        internal static bool AddErrorSourceCacheDataFromReq (CRequest req) {
            CacheData cacheData = null;
            CreateOrGetCache (req.keyHashCode, out cacheData);
            cacheData.SetCacheData (null, req.key); //缓存
            cacheData.allDependencies = req.allDependencies;
            cacheData.isAssetLoaded = true;
            cacheData.isError = true;
            return req.isAssetBundle;
        }

        /// <summary>
        /// 从缓存设置数据
        /// </summary>
        /// <param name="req"></param>
        internal static bool SetRequestDataFromCache (CRequest req) {
            bool re = false;
#if UNITY_EDITOR
            if (CResLoader.SimulateAssetBundleInEditor) {
                if (!req.isAssetBundle) return false;
                System.Type assetType = req.assetType;
                if (assetType == null) assetType = Typeof_Object;

                if (Typeof_ABScene.Equals (assetType)) {
                    string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (req.assetBundleName, req.assetName);
                    if (levelPaths.Length == 0) {
                        ///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
                        //        from that there right scene does not exist in the asset bundle...

                        Debug.LogError ("There is no scene with name \"" + req.assetName + "\" in " + req.assetBundleName);
                        return false;
                    }

                    if (req.isAdditive)
                        req.assetBundleRequest = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode (levelPaths[0]);
                    else
                        req.assetBundleRequest = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode (levelPaths[0]);
                } else {
                    string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (req.assetBundleName, req.assetName);
                    if (assetPaths.Length == 0) {
                        Debug.LogError ("There is no asset with name \"" + req.assetName + "\" in " + req.assetBundleName);
                        return false;
                    }

                    UnityEngine.Object target = UnityEditor.AssetDatabase.LoadAssetAtPath (assetPaths[0], assetType);
                    req.data = target;

                }

                return true;
            }
#endif

            int keyhash = req.keyHashCode;
            CacheData cachedata = GetCache (keyhash);
            if (cachedata != null) {
                AssetBundle abundle = cachedata.assetBundle;
                System.Type assetType = req.assetType;
                if (assetType == null) assetType = Typeof_Object;
                if (req.isShared) //共享的
                {
                    req.data = abundle;
                    re = true;
                } else if (Typeof_AssetBundle.Equals (assetType)) {
                    req.data = cachedata.assetBundle;
                    re = true;
                } else if (Typeof_ABScene.Equals (assetType)) {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                    if (req.isAdditive)
                        req.assetBundleRequest = Application.LoadLevelAdditiveAsync (req.assetName);
                    else
                        req.assetBundleRequest = Application.LoadLevelAsync (req.assetName);
#else
                    req.assetBundleRequest = SceneManager.LoadSceneAsync (req.assetName, req.isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
#endif
                    re = true;
                } else {
                    if (abundle == null) {
#if UNITY_EDITOR
                        Debug.LogWarningFormat ("SetRequestDataFromCache Assetbundle is null request(url={0},assetName={1},assetType={2})  ", req.url, req.assetName, req.assetType);
#endif
                    } else if (req.async)
                        req.assetBundleRequest = abundle.LoadAssetAsync (req.assetName, assetType);
                    else {
                        req.data = abundle.LoadAsset (req.assetName, assetType);
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
        internal static bool CheckDependenciesComplete (CRequest req) {
            if (req.allDependencies == null || req.allDependencies.Length == 0) return true;

            int[] denps = req.allDependencies;
            CacheData cache = null;
            int hash = 0;
            for (int i = 0; i < denps.Length; i++) {
                hash = denps[i];
                if (hash == 0) {

                } else if (caches.TryGetValue (hash, out cache)) {
                    // Debug.LogFormat("waiting for req(assetName{0} hash{1}))'s Dependencies hash{2},assetBundleKey={3},count={4},retun if({5}||{6}) return false",req.assetName,req.keyHashCode,cache.assetHashCode,cache.assetBundleKey,cache.count,!cache.isAssetLoaded,!cache.canUse);
                    if (!cache.isAssetLoaded || !cache.canUse)
                        return false;
                } else {
                    // Debug.LogFormat("waiting for req(assetName{0} hash{1}))'s Dependencies hash{2}",req.assetName,req.keyHashCode,hash);
                    return false;
                }
            }

            return true;
        }

        internal static bool Unload (int hashcode) {
            CacheData cache = TryGetCache (hashcode);
            if (cache != null && cache.count == 0) {
                caches.Remove (cache.assetHashCode); //删除
                CacheData.Release (cache);
                return true;
            } else if (cache != null) {
#if UNITY_EDITOR
                Debug.LogWarningFormat ("<color=#ff00ff> can't unload  cache assetBundle={0},keyhashcode({1},count={2})   </color>", cache.assetBundleKey, cache.assetHashCode, cache.count);
#endif
            }
            return false;
        }

        /// <summary>
        /// 是否下载过资源
        /// </summary>
        /// <returns></returns>
        public static bool Contains (int keyhash) {
            CacheData cdata = GetCache (keyhash);
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
        public static bool Contains (string key) {
            int keyhash = LuaHelper.StringToHash (key);
            return Contains (keyhash);
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public static void ClearAll () {

            var items = caches.GetEnumerator ();
            while (items.MoveNext ()) {
                items.Current.Value.Dispose ();
            }

            caches.Clear ();
        }

        #region check type
        public static readonly Type Typeof_String = typeof (System.String);
        public static readonly Type Typeof_Bytes = typeof (System.Byte[]);
        public static readonly Type Typeof_AssetBundle = typeof (AssetBundle);
        public static readonly Type Typeof_ABScene = typeof (AssetBundleScene);
        public static readonly Type Typeof_AudioClip = typeof (AudioClip);
        public static readonly Type Typeof_Texture2D = typeof (Texture2D);
        public static readonly Type Typeof_Object = typeof (UnityEngine.Object);

        #endregion
    }
}