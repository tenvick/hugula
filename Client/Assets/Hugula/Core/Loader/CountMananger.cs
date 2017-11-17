// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using Hugula.Utils;

namespace Hugula.Loader
{
    /// <summary>
    /// 计数器管理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CountMananger
    {
 
        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static int Subtract(int hashcode)
        {
#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor)
            {
                return 1;
            }
#endif
            CacheData cached = CacheManager.TryGetCache(hashcode);
            if (cached != null && cached.count >= 1)
            {
                cached.count--; // = cached.count - 1;
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogFormat(cached.assetBundleKey, " <color=#8cacbc>Subtract (assetBundle={0},hashcode={1},count={2}) frameCount{3}</color>", cached.assetBundleKey, hashcode, cached.count, UnityEngine.Time.frameCount);
#endif
                if (cached.count == 0) //所有引用被清理。
                {
                    int[] alldep = cached.dependencies;
                    CacheManager.ClearDelay(hashcode);
                    int tmpDenHash = 0;
                    if (alldep != null)
                    {
                        for (int i = 0; i < alldep.Length; i++)
                        {
                            tmpDenHash = alldep[i];
                            if (tmpDenHash != hashcode)
                            {
                                Subtract(tmpDenHash);
                            }
                        }
                    }
                }
                return cached.count;
            }
#if UNITY_EDITOR || !HUGULA_RELEASE
            else if (cached != null && cached.count <= 0)
            {
                UnityEngine.Debug.LogWarningFormat("CountManager.Subtract (assetBundle={0},hashcode={1},count={2}) frameCount{3}", cached.assetBundleKey, hashcode, cached.count, UnityEngine.Time.frameCount);
            }
#endif

            return -1;
        }

        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Subtract(string key)
        {
            int hashcode = LuaHelper.StringToHash(key);
            return Subtract(hashcode);
        }

        /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static int Add(int hashcode)
        {
#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor)
            {
                return 1;
            }
#endif
            CacheData cached = CacheManager.TryGetCache(hashcode);
            if (cached != null)
            {
                cached.count++; //= cached.count + 1;
#if HUGULA_CACHE_DEBUG 
                HugulaDebug.FilterLogFormat(cached.assetBundleKey, " <color=#0cbcbc>add  (assetBundle={0},hashcode={1},count={2})  frameCount{3}</color>", cached.assetBundleKey, hashcode, cached.count, UnityEngine.Time.frameCount);
#endif
                return cached.count;
            }
            return -1;
        }

        /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Add(string key)
        {
            int hashcode = LuaHelper.StringToHash(key);
            return Add(hashcode);
        }

        internal static int WillAdd(int hashcode)
        {
#if UNITY_EDITOR
            if (ManifestManager.SimulateAssetBundleInEditor)
            {
                return 1;
            }
#endif
            CacheData cached = null;
            CacheManager.CreateOrGetCache(hashcode, out cached);
            cached.count++;
            return cached.count;
        }

        // internal static int WillAdd(string key)
        // {
        //     CacheData cached = null;
        //     CacheManager.CreateOrGetCache(key, out cached);
        //     cached.count++;
        //     return cached.count;

        // }

        // /// <summary>
        // /// Adds the dependencies.
        // /// </summary>
        // /// <param name="hashcode">Hashcode.</param>
        // internal static void WillAddDependencies(int[] allDependencies)
        // {
        //     if (allDependencies != null)
        //     {
        //         foreach (int hash in allDependencies)
        //             WillAdd(hash);
        //     }

        // }
    }
}