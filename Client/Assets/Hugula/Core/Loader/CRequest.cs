// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System;
using System.Collections.Generic;
using Hugula.Utils;

namespace Hugula.Loader
{
    /// <summary>
    /// Request.
    /// </summary>
    [SLua.CustomLuaClass]
    public class CRequest : IDisposable
    {
        /// <summary>
        /// Request
        /// </summary>
        public CRequest()
        {

        }

        public virtual void Dispose()
        {
            this._url = string.Empty;
            this._relativeUrl = string.Empty;
            this._key = string.Empty;
            this._udKey = string.Empty;
            this._udAssetKey = string.Empty;

            this.priority = 0;
            this.index = 0;
            this._keyHashCode = 0;

            this._uris = null;
            this.data = null;
            this.head = null;
            this.headers = null;
            this.userData = null;
            this.group = null;
            this.isDisposed = true;
            _assetName = string.Empty;
            assetType = null;//string.Empty;
#if UNITY_IPHONE
            async = false;
#else
            async = true;
#endif

            pool = false;
            isAdditive = false;
            isShared = false;
            needUriGroup = false;

            this.assetOperation = null;
            this.dependencies = null;

            this.OnComplete = null;
            this.OnEnd = null;
        }

#if HUGULA_PROFILER_DEBUG
        [SLua.DoNotToLua]
        public System.DateTime beginQueueTime;
        [SLua.DoNotToLua]
        public System.DateTime beginLoadTime;
#endif


        private string _relativeUrl;

        private string _key, _udKey, _udAssetKey;

        private int _keyHashCode = 0;

        private string _url;

        private string _assetName = string.Empty;

        private UriGroup _uris;

        /// <summary>
        /// Gets the relative URL.
        /// </summary>
        /// <value>The relative URL.</value>
        public string relativeUrl
        {
            set
            {
                _url = null;
                _udKey = null;
                _key = null;
                _keyHashCode = 0;
                _relativeUrl = value;
                needUriGroup = CheckNeedUriGroup(value);
            }
            get { return _relativeUrl; }
        }

        /// <summary>
        /// assetbundleName
        /// </summary>
        public string assetBundleName
        {
            get
            {
                return key;
            }
        }

        /// <summary>
        /// 要加载的asset 名称
        /// </summary>
        public string assetName
        {
            get
            {
                if (string.IsNullOrEmpty(_assetName)) _assetName = CUtils.GetAssetName(relativeUrl); //Get// key;
                return _assetName;
            }
            set { _assetName = value; }
        }

        private Type _assetType;
        /// <summary>
        /// asset Type name
        /// </summary>
		public Type assetType
        {
            get
            {
                if (_assetType == null)
                    _assetType = CacheManager.Typeof_Object;
                return _assetType;
            }
            set { _assetType = value; }
        }

        /// <summary>
        /// 设置加载的数据
        /// </summary>
        public object head;

        /// <summary>
        /// 加载的头信息
        /// </summary>
        public Dictionary<string,string> headers;

        /// <summary>
        /// 加载的数据
        /// </summary>
        public object data;

        /// <summary>
        /// The user data.
        /// </summary>
        public object userData;

        public System.Action<CRequest> OnEnd;

        public System.Action<CRequest> OnComplete;

        public void DispatchComplete()
        {
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("CRequest({0},{1}).DispatchComplete()",this.assetName,this.key));

            if (OnComplete != null)
                OnComplete(this);

            Profiler.EndSample();
#else
            if (OnComplete != null)
                OnComplete(this);
#endif
        }

        public void DispatchEnd()
        {
            if (OnEnd != null)
                OnEnd(this);
        }

        public bool isShared { get; internal set; }

        /// <summary>
        /// 请求地址 网址，绝对路径
        /// </summary>
        public string url
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                    url = string.Empty; //;
                return _url;
            }
            internal set
            {
                if (value == null)
                    _url = null;
                else
                {
                    if (uris == null)
                    {
                        _url = relativeUrl;
                    }
                    else
                    {
                        _url = GetURL(this);//
                    }
                }
            }
        }

        /// <summary>
        /// 缓存用的关键字
        /// 如果没有设定 则为默认key生成规则
        /// </summary>
        public string key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                    key = string.Empty;
                return _key;
            }
            internal set
            {
                if (value == null)
                    _key = null;
                else
                    _key = CUtils.GetAssetBundleName(relativeUrl);
            }
        }

        /// <summary>
        /// assetBundle key的hashcode 用于计算缓存的资源key
        /// </summary>
        public int keyHashCode
        {
            get
            {
                if (_keyHashCode == 0)
                    keyHashCode = 1;

                return _keyHashCode;
            }
            internal set
            {
                if (value == 0)
                    _keyHashCode = value;
                else
                    _keyHashCode = LuaHelper.StringToHash(key);
            }
        }

        /// <summary>
        /// The url unique key.
        /// </summary>
        /// <value>
        /// The url unique key.
        /// </value>
        public string udKey
        {
            get
            {
                if (string.IsNullOrEmpty(_udKey))
                    udKey = string.Empty;
                return _udKey;
            }
            set
            {
                if (value == null)
                    _udKey = null;
                else
                    _udKey = CUtils.GetUDKey(url);
            }
        }

        /// <summary>
        /// Sets the url and asse unique key
        /// </summary>
        /// <value>
        /// The unique key.
        /// </value>
        public string udAssetKey
        {
            get
            {
                if (string.IsNullOrEmpty(_udAssetKey))
                    udAssetKey = string.Empty;
                return _udAssetKey;
            }
            set
            {
                if (value == null)
                    _udAssetKey = null;
                else
                {
                    _udAssetKey = string.Format("{0}+{1}", key, assetName);
                }
            }
        }


        /// <summary>
        /// 是否异步加载
        /// </summary>
#if UNITY_IPHONE
        public bool  async = false;
#else
        public bool async = true;
#endif

        /// <summary>
        ///  优先等级
        ///  降序
        ///  值越大优先级越高
        /// </summary>
        public int priority = 0;//优先级

        /// <summary>
        /// 加载uri索引
        /// </summary>
        public int index = 0;

        /// <summary>
        /// uri组策略
        /// </summary>
        public UriGroup uris
        {
            get
            {
                if (_uris == null && needUriGroup)
                {
                    _uris = UriGroup.uriList;
                }
                return _uris;
            }

            set
            {
                _uris = value;
            }
        }

        /// <summary>
        /// 场景加载追加模式
        /// </summary>
        public bool isAdditive = false;

        internal GroupQueue<CRequest> group;

        /// <summary>
        /// dependencies count;
        /// </summary>
        internal int[] dependencies;


        /// <summary>
        /// 协同加载ab
        /// </summary>
        internal ResourcesLoadOperation assetOperation;

        /// <summary>
        /// 放入内存池
        /// </summary>
        internal bool pool = false;

        internal bool isDisposed = false;

        internal bool needUriGroup = false;
        public void ReleaseToPool()
        {
            if (pool)
                Release(this);
        }

        /// <summary>
        /// 获取当前 URL
        /// </summary>
        private static string GetURL(CRequest req)
        {
            string url = string.Empty;
            var uris = req.uris;
            int index = req.index;
            if (uris != null && uris.count > index && index >= 0)
            {
                url = CUtils.PathCombine(uris[index], req.relativeUrl);
            }
            return url;
        }

        /// <summary>
        /// 获取key URL
        /// </summary>
        public static string GetUDKeyURL(CRequest req)
        {
            string url = string.Empty;
            var uris = req.uris;
            int index = 0;
            if (uris != null && uris.count > index && index >= 0)
            {
                url = CUtils.PathCombine(uris[index], req.relativeUrl);
            }
            else
            {
                url = req.relativeUrl;
            }
            return url;
        }

        /// <summary>
        /// check need set uri group
        /// </summary>
        public static bool CheckNeedUriGroup(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            if (url.StartsWith("http")
                || url.IndexOf("://") != -1
                || url.StartsWith(CUtils.realPersistentDataPath)
                || url.StartsWith(CUtils.realStreamingAssetsPath)
               )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //创建一个CRequest
        public static CRequest Create(string relativeUrl, string assetName, Type assetType, System.Action<CRequest> onComp, System.Action<CRequest> onEnd,
        object head, bool async)
        {
            var req = CRequest.Get();
            req.relativeUrl = relativeUrl;
            req.assetName = assetName;
            req.assetType = assetType;

            req.OnComplete = onComp;
            req.OnEnd = onEnd;
            req.head = head;
            req.async = async;

            return req;
        }

        #region ObjectPool 
        static ObjectPool<CRequest> objectPool = new ObjectPool<CRequest>(m_ActionOnGet, m_ActionOnRelease);
        private static void m_ActionOnGet(CRequest req)
        {
            // Debug.LogFormat("",req.isShared)
            req.pool = true;
            req.isDisposed = false;
#if HUGULA_PROFILER_DEBUG
            req.beginQueueTime = System.DateTime.Now;
            req.beginLoadTime = System.DateTime.Now;
#endif
        }

        private static void m_ActionOnRelease(CRequest req)
        {
            req.Dispose();
        }

        public static CRequest Get()
        {
            return objectPool.Get();
        }

        public static void Release(CRequest toRelease)
        {
            objectPool.Release(toRelease);
        }
        #endregion
    }


}