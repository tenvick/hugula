// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Loader
{

    [SLua.CustomLuaClass]
    public static class ABDelayUnloadManager
    {

        //delay unload time
        public static float delaySecondTime = 1f;
        private static Dictionary<int, float> removeDic = new Dictionary<int, float>();

        private static List<int> deleteList = new List<int>();

        internal static void Add(int keyhashcode)
        {

            if (removeDic.ContainsKey(keyhashcode))
                removeDic.Remove(keyhashcode);
            // Debug.LogFormat("<color=#00ff00> -1 add  keyhashcode({0}) </color>",keyhashcode);
            removeDic.Add(keyhashcode, Time.unscaledTime + delaySecondTime);
        }

        internal static void AddDependenciesReferCount(CacheData cache, bool needAdd)
        {
            if (cache != null)
            {
                int keyhash = 0;
                int[] alldep = cache.dependencies;
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogFormat(cache.assetBundleKey, "<color=#ffff00> CheckRemove  abName({0})'s allDependencies from ABDelayUnloadManager , Dependencies.count = {1}  </color>", cache.assetBundleKey, alldep == null ? 0 : alldep.Length);
#endif          
                if (alldep != null)
                {
                    CacheData pCache = null;
                    for (int i = 0; i < alldep.Length; i++)
                    {
                        keyhash = alldep[i];
                        pCache = CacheManager.TryGetCache(keyhash);
                        if (pCache != null)
                        {
                            if (removeDic.ContainsKey(keyhash)) //
                                removeDic.Remove(keyhash);
                            // pCache.
                            if (needAdd)
                            {

                                pCache.count++;

                                if (pCache.count == 1)
                                    AddDependenciesReferCount(pCache, true);
                            }
                        }
                    }//end for
                }// end if

            }// end cache

        }

        internal static bool CheckRemove(int keyhashcode)
        {
            if (removeDic.ContainsKey(keyhashcode))
            {
                removeDic.Remove(keyhashcode);
                CacheData cache = CacheManager.TryGetCache(keyhashcode);
                AddDependenciesReferCount(cache, true);
                // #if HUGULA_CACHE_DEBUG
                //                 HugulaDebug.FilterLogFormat(cache.assetBundleKey, "<color=#ffff00> -1 remove  abName({0}) for ABDelayUnloadManager ,ref count =  {1},removed={2} </color>", cache.assetBundleKey, cache.count, re);
                // #endif
                //                 if (cache != null && cache.dependencies != null)
                //                 {
                //                     int keyhash = 0;
                //                     int[] alldep = cache.dependencies;
                // #if HUGULA_CACHE_DEBUG
                //                     HugulaDebug.FilterLogFormat(cache.assetBundleKey, "<color=#ffff00> -1 remove  abName({0})'s allDependencies from ABDelayUnloadManager , Dependencies.count = {1}  </color>", cache.assetBundleKey, alldep == null ? 0 : alldep.Length);
                // #endif
                //                     for (int i = 0; i < alldep.Length; i++)
                //                     {
                //                         keyhash = alldep[i];
                //                         CountMananger.WillAdd(keyhash); //引用数量加1
                //                         CheckRemove(keyhash);
                //                     }
                //                 }
                return true;
            }
            else
                return false;
        }

        internal static void Update()
        {
            // var now = Time.unscaledTime;
            deleteList.Clear();

            var items = removeDic.GetEnumerator();

            while (items.MoveNext())
            {
                var kv = items.Current;
                if (Time.unscaledTime >= kv.Value)
                {
                    deleteList.Add(kv.Key);
                    CacheManager.Unload(kv.Key);
                }
            }

            for (int i = 0; i < deleteList.Count; i++)
            {
                removeDic.Remove(deleteList[i]);
            }

        }
    }
}