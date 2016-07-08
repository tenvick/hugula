using UnityEngine;
using System.Collections.Generic;
using System;

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
        this.assetBundleName = assetBundleName;
        this.assetHashCode = LuaHelper.StringToHash(assetBundleName);
    }

    /// <summary>
    /// assetBundle Name
    /// </summary>
    public string assetBundleName { get; private set; }

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
            Debug.LogWarning(string.Format("AddCache  {0} assetBundleName = {1} has exist", assetHashCode, cacheout.assetBundleName));
#endif
            caches.Remove(assetHashCode);
        }
        caches.Add(assetHashCode, cacheData);
    }

    /// <summary>
    /// 锁定
    /// </summary>
    /// <param name="hashkey"></param>
    internal static void AddLock(int hashkey)
    {
        lockedCaches.Add(hashkey);
    }

    /// <summary>
    /// 移除锁定
    /// </summary>
    /// <param name="hashkey"></param>
    internal static void RemoveLock(int hashkey)
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
                Debug.LogWarningFormat(" the cache ab({0},{1}) are locked,cant delete.) ", cache.assetBundleName, cache.assetBundle);
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
            System.Type assetType = LuaHelper.GetType(req.assetType);
            if (assetType == null) assetType = typeof(UnityEngine.Object);

            if (req.isShared) //共享的
            {
                req.data = abundle;
                re = true;
            }
            else if (assetType.Equals(Typeof_String))
            {

                var str = string.Empty;
                if (cachedata.www is byte[])
                {
                    str = LuaHelper.GetUTF8String((byte[])cachedata.www);
                }
                req.data = new string[] { str };

                req.clearCacheOnComplete = true;
                re = true;
            }
            else if (assetType.Equals(Typeof_Bytes))
            {
                req.data = cachedata.www;
                req.clearCacheOnComplete = true;
                re = true;
            }
            else if (assetType.Equals(Typeof_AssetBundle))
            {
                req.data = cachedata.assetBundle;
                re = true;
            }
            else
            {
                if (req.async)
                    req.assetBundleRequest = abundle.LoadAssetAsync(req.assetName, assetType);
                else
                {
                    req.data = abundle.LoadAsset(req.assetName, assetType);
                    //Debug.LogFormat("<color=yellow>set data {0},{1},{2},{3},{4}</color>",req.key,req.data, req.assetName, req.assetType, assetType);
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

        for (int i = 0; i < denps.Length; i++)
        {
            if (!caches.ContainsKey(denps[i]))
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

    #region check type
    private static Type Typeof_String = typeof(System.String);
    private static Type Typeof_Bytes = typeof(System.Byte[]);
    private static Type Typeof_AssetBundle = typeof(AssetBundle);

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
    internal static bool Add(int hashcode,int add)
    {
        CacheData cached = CacheManager.GetCache(hashcode);
        if (cached != null)
        {
            cached.count += add;//= cached.count + 1;
            return true;
        }
        return false;
    }
}