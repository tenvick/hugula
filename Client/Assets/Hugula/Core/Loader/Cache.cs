using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 缓存资源
/// </summary>
[SLua.CustomLuaClass]
public class CacheData : IDisposable
{

    public CacheData(WWW www, AssetBundle assetBundle, string assetBundleName)
    {
        this.www = www;
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
    /// www对象
    /// </summary>
    public WWW www;

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
        if (www != null) www.Dispose();
        www = null;
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
    /// 添加缓存
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="www"></param>
    internal static void AddCache(CacheData cacheData)
    {
        int assetHashCode = cacheData.assetHashCode;
        CacheData cacheout = null;
        caches.TryGetValue(assetHashCode, out cacheout);
        if (cacheout != null)
        {
            Debug.LogWarning(string.Format("AddCache  {0} assetBundleName = {1} has exist", assetHashCode, cacheout.assetBundleName));
            caches.Remove(assetHashCode);
        }
        caches.Add(assetHashCode, cacheData);
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
            caches.Remove(assethashcode);//删除
            cache.Dispose();
            int[] alldep = cache.allDependencies;
            CacheData cachetmp = null;
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
    internal static CacheData GetCache(string assetBundleName)
    {
        int hash = LuaHelper.StringToHash(assetBundleName);
        return GetCache(hash);
    }

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
                WWW www = cachedata.www;
                req.data = new string[] { www.text };
                re = true;
            }
            else if (assetType.Equals(Typeof_Bytes))
            {
                WWW www = cachedata.www;
                req.data = www.bytes;
                re = true;
            }
            else if (assetType.IsArray || assetType.Equals(Typeof_AssetBundle))
            {
                req.data = abundle;//.LoadAllAssets(assetType.UnderlyingSystemType);
                re = true;
            }
            else if (!assetType.IsArray)
            {
                req.data = abundle.LoadAsset(req.assetName, assetType);
                re = true;
            }
        }

        return re;
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

}