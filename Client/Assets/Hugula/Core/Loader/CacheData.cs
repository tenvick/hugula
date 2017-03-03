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
            isAssetLoaded =false;
        }

        /// <summary>
        /// Set Cache Data 
        /// </summary>
        public void SetCacheData(object data, AssetBundle assetBundle, string assetBundleName)
        {
            this.www = data;
            this.assetBundle = assetBundle;
            this.assetBundleKey = assetBundleName;
            this.assetHashCode = LuaHelper.StringToHash(assetBundleKey);
        }


        /// <summary>
        /// assetBundle Name
        /// </summary>
        public string assetBundleKey { get; private set; }

        /// <summary>
        /// hashcode
        /// </summary>
        public int assetHashCode { 
            get;
            private set;
         }

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
        public int count;

        /// <summary>
        /// 所有依赖项目
        /// </summary>
        public int[] allDependencies { get; internal set; }

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
        public bool isUnloaded{ get; private set; }
        
        public void Dispose()
        {
            #if HUGULA_CACHE_DEBUG
                    Debug.LogWarningFormat("Dispose  CacheData({0})  ", assetBundleKey);
            #endif
            if (assetBundle) assetBundle.Unload(true);
            www = null;
            assetBundle = null;
            allDependencies = null;
            isUnloaded = false;
            isAssetLoaded =false;
            isError = false;

            assetBundleKey = string.Empty;
            assetHashCode = 0;
            count = 0;
        }
        public void Unload()
        {
            if (assetBundle) assetBundle.Unload(false);
            isUnloaded = true;
            #if HUGULA_CACHE_DEBUG
                    Debug.LogWarningFormat("Unload  CacheData({0})  ", assetBundleKey);
            #endif
        }
    }
  
}