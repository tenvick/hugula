// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System;
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
            isAssetLoaded = false;
        }

        /// <summary>
        /// Set Cache Data 
        /// </summary>
        public void SetCacheData(AssetBundle assetBundle, string assetBundleName)
        {
            this.assetBundle = assetBundle;
            this.assetBundleKey = assetBundleName;
            this.isUnloaded = false;
            this.isAssetLoaded = false;
            if (this.assetHashCode == 0 && !string.IsNullOrEmpty(assetBundleName))
            {
                int hash = LuaHelper.StringToHash(assetBundleKey);
                this.assetHashCode = hash;
            }// Debug.LogWarningFormat("CacheData assetBundleName({0})'s hashcode is worng", assetBundleName);
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
        public int[] allDependencies { get; internal set; }

        public int[] directDependencies { get; internal set; }

        /// <summary>
        /// is asset loaded
        /// </summary>
        /// <value><c>true</c> if is asset loaded; otherwise, <c>false</c>.</value>
        public bool isAssetLoaded { get; internal set; }

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
            // #if HUGULA_CACHE_DEBUG
            if (assetBundleKey == "ab67987bbc2dfd2eef15a0ed6ca237a1.u3d")
                Debug.LogFormat("Dispose  CacheData({0},assetHashCode({1}))  ", assetBundleKey, assetHashCode);
            // #endif
            if (assetBundle) assetBundle.Unload(true);
            // www = null;
            assetBundle = null;
            allDependencies = null;
            directDependencies = null;
            isUnloaded = false;
            isAssetLoaded = false;
            isError = false;

            assetBundleKey = string.Empty;
            assetHashCode = 0;
            count = 0;
        }
        public void Unload()
        {
            if (assetBundle) assetBundle.Unload(false);
            isUnloaded = true;
            // #if HUGULA_CACHE_DEBUG
            if (assetBundleKey == "ab67987bbc2dfd2eef15a0ed6ca237a1.u3d") Debug.LogFormat("Unload  CacheData({0}assetHashCode({1})  ", assetBundleKey, assetHashCode);
            // #endif
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