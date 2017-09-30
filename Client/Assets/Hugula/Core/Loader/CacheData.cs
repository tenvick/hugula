// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System;
using System.Collections.Generic;
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
            isUnloaded = false;
            assets = new Dictionary<string, object>();
        }

        /// <summary>
        /// Set Cache Data 
        /// </summary>
        public void SetCacheData(AssetBundle assetBundle, string assetBundleName,int bundleNameHashCode=0)
        {
            this.assetBundle = assetBundle;
            this.assetBundleKey = assetBundleName;
            this.isUnloaded = false;
            if (bundleNameHashCode == 0 && !string.IsNullOrEmpty(assetBundleName))
            {
                int hash = LuaHelper.StringToHash(assetBundleKey);
                this.assetHashCode = hash;
            }else
            {
                this.assetHashCode = bundleNameHashCode;
            }
            // Debug.LogWarningFormat("CacheData assetBundleName({0})'s hashcode is worng", assetBundleName);
        }


        /// <summary>
        /// assetBundle Name
        /// </summary>
        public string assetBundleKey { get; internal set; }

        /// <summary>
        /// hashcode
        /// </summary>
        public int assetHashCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// assetbundle对象
        /// </summary>
        public AssetBundle assetBundle;

        // /// <summary>
        // /// www data
        // /// </summary>
        // public object www;

        /// <summary>
        /// 当前引用数量
        /// </summary>
        public int count;

        /// <summary>
        /// 所有依赖项目
        /// </summary>
        public int[] dependencies { get; internal set; }

        public Dictionary<string, object> assets;

        /// <summary>
        /// is loaded done 
        /// </summary>
        /// <value><c>true</c> if is assetbundle loaded; otherwise, <c>false</c>.</value>

        public bool isDone { get; internal set; }

        /// <summary>
        /// is loaded Error 
        /// </summary>
        /// <value><c>true</c> if is asset loaded; otherwise, <c>false</c>.</value>
        public bool isError { get; internal set; }

        /// <summary>
        /// is assetbundle unload(false)
        /// </summary>
        /// <value><c>true</c> if is assetbundle unload(false); otherwise, <c>false</c>.</value>
        public bool isUnloaded { get; private set; }

        public bool canUse
        {
            get
            {
                return !isUnloaded && assetBundle != null;
            }
        }
        public void Dispose()
        {
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogFormat (assetBundleKey,"Dispose  CacheData({0},assetHashCode({1})),frame={2}  ", assetBundleKey, assetHashCode,Time.frameCount);
#endif
            if (assetBundle) assetBundle.Unload(true);
            assets.Clear();
            assetBundle = null;
            dependencies = null;
            isUnloaded = false;
            // isAssetLoaded = false;
            isError = false;
            isDone = false;

            assetBundleKey = string.Empty;
            assetHashCode = 0;
            count = 0;
        }
        public void Unload()
        {
            if (assetBundle) assetBundle.Unload(false);
            isUnloaded = true;
#if HUGULA_CACHE_DEBUG
             HugulaDebug.FilterLogFormat (assetBundleKey,"Unload  CacheData({0},assetHashCode({1})),frame={2}  ", assetBundleKey, assetHashCode,Time.frameCount);
#endif
        }

        public void SetAsset(string name, object asset)
        {
            assets[name] = asset;
        }

        public object GetAsset(string name)
        {
            object asset = null;
            assets.TryGetValue(name, out asset);
            return asset;
        }

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

}