// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using Hugula.Utils;

namespace Hugula.Loader {
    /// <summary>
    /// 计数器管理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class CountMananger {

        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static int Subtract (int hashcode) {
#if UNITY_EDITOR
            if (CResLoader.SimulateAssetBundleInEditor) {
                return 1;
            }
#endif
            CacheData cached = CacheManager.TryGetCache (hashcode);
            if (cached != null && cached.count >= 1) {
                cached.count--; // = cached.count - 1;
#if HUGULA_CACHE_DEBUG
                UnityEngine.Debug.LogFormat (" <color=#8cacbc>Subtract (assetBundle={0},count={1}) frameCount{2}</color>", cached.assetBundleKey, cached.count, UnityEngine.Time.frameCount);
#endif
                if (cached.count == 0) //所有引用被清理。
                {

                    CacheManager.ClearCache (hashcode);
                    int[] alldep = cached.allDependencies;
                    CacheData cachetmp = null;
                    if (alldep != null) {
                        for (int i = 0; i < alldep.Length; i++) {
                            cachetmp = CacheManager.TryGetCache (alldep[i]);
                            if (cachetmp != null && cachetmp.assetHashCode != hashcode) {
                                Subtract (cachetmp.assetHashCode);
                            }
                        }
                    }
                }
                return cached.count;
            }

            return -1;
        }

        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Subtract (string key) {
            int hashcode = LuaHelper.StringToHash (key);
            return Subtract (hashcode);
        }

        /// <summary>
        /// 目标引用加一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static int Add (int hashcode) {
#if UNITY_EDITOR
            if (CResLoader.SimulateAssetBundleInEditor) {
                return 1;
            }
#endif
            CacheData cached = CacheManager.TryGetCache (hashcode);
            if (cached != null) {
                ABDelayUnloadManager.CheckRemove (hashcode);
                cached.count++; //= cached.count + 1;
#if HUGULA_CACHE_DEBUG 
                UnityEngine.Debug.LogFormat (" <color=#0cbcbc>add  (assetBundle={0},count={1})  frameCount{2}</color>", cached.assetBundleKey, cached.count, UnityEngine.Time.frameCount);
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
        public static int Add (string key) {
            int hashcode = LuaHelper.StringToHash (key);
            return Add (hashcode);
        }

        public static int WillAdd (int hashcode) {
#if UNITY_EDITOR
            if (CResLoader.SimulateAssetBundleInEditor) {
                return 1;
            }
#endif
            CacheData cached = null;
            CacheManager.CreateOrGetCache (hashcode, out cached);
            cached.count++;
#if HUGULA_CACHE_DEBUG
            UnityEngine.Debug.LogFormat (" <color=#fcfcfc> will add  (assetBundle={0},,hash={1},count={2}) frameCount{3}</color>", cached.assetBundleKey, cached.assetHashCode, cached.count, UnityEngine.Time.frameCount);
#endif
            return cached.count;
        }

        public static int WillAdd (string key) {
            CacheData cached = null;
            CacheManager.CreateOrGetCache (key, out cached);
            cached.count++;
#if HUGULA_CACHE_DEBUG
            UnityEngine.Debug.LogFormat (" <color=#fcfcfc> will add  (assetBundle={0},hash={1},count={2}) frameCount{3}</color>", cached.assetBundleKey, cached.assetHashCode, cached.count, UnityEngine.Time.frameCount);
#endif
            return cached.count;

        }

        /// <summary>
        /// Adds the dependencies.
        /// </summary>
        /// <param name="hashcode">Hashcode.</param>
        internal static void WillAddDependencies (int[] allDependencies) {
            if (allDependencies != null) {
                foreach (int hash in allDependencies)
                WillAdd (hash);
            }

        }
    }
}