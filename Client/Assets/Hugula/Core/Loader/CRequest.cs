// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Hugula.Utils;

namespace Hugula.Loader
{
    /// <summary>
    /// Request.
    /// </summary>
    [SLua.CustomLuaClass]
    public class CRequest // IDispose
    {
        /// <summary>
        /// Request
        /// </summary>
        public CRequest()
        {

        }

        /// <summary>
        /// 加载请求 
        /// </summary>
        /// <param name="url"></param>
        public CRequest(string relativeUrl)
        {
            this._relativeUrl = relativeUrl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">地址 相对路径</param>
        /// <param name="assetName">加载的资源名</param>
        /// <param name="assetType">资源类型，默认为GameObject</param>
		public CRequest(string relativeUrl, string assetName, Type assetType)
        {
            this._relativeUrl = relativeUrl;
            this.assetName = assetName;
            this.assetType = assetType;
        }

        public virtual void Dispose()
        {
            this._url = string.Empty;
            this._relativeUrl = string.Empty;
            this._key = string.Empty;
            this._udKey = string.Empty;

            this.priority = 0;
            this.index = 0;
            this._keyHashCode = 0;

            this._uris = null;
            this.data = null;
            this.head = null;
            this.userData = null;

            _assetBundleName = string.Empty;
            _assetName = string.Empty;
            assetType = null;//string.Empty;

            async = true;
            pool = false;
            isAdditive = false;
            isShared = false;
            isNormal = true;
            isAssetBundle = false;
            isLoadFromCacheOrDownload = false;
            isNativeFile = false;

            this.assetBundleRequest = null;
            this.allDependencies = null;

            this.OnComplete = null;
            this.OnEnd = null;

        }

        private string _relativeUrl;

        private string _key, _udKey;

        private int _keyHashCode = 0;

        private string _url;

        private string _assetBundleName = string.Empty;

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
                _assetBundleName = null;
                _url = null;
                _udKey = null;
                _key = null;
                _keyHashCode = 0;
                _relativeUrl = value;
            }
            get { return _relativeUrl; }
        }

        /// <summary>
        /// assetbundleName 根据url计算出来
        /// </summary>
        public string assetBundleName
        {
            get
            {
                if (string.IsNullOrEmpty(_assetBundleName))
                    _assetBundleName = CUtils.GetAssetBundleName(relativeUrl);
                return _assetBundleName;
            }
            set
            {
                _assetBundleName = value;
            }
        }

        /// <summary>
        /// 要加载的asset 名称
        /// </summary>
        public string assetName
        {
            get
            {
                if (string.IsNullOrEmpty(_assetName)) _assetName = key;
                return _assetName;
            }
            set { _assetName = value; }
        }

        /// <summary>
        /// asset Type name
        /// </summary>
		public Type assetType;//= string.Empty;

        /// <summary>
        /// 加载的头信息
        /// </summary>
        public object head;

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
#if HUGULA_PROFILE_DEBUG
        Profiler.BeginSample(this.assetName+"_req_onComplete");

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
            set
            {
                if (value == null)
                    _url = null;
                else
				{
					if(_uris == null)
					{
						_url = relativeUrl;
					}else
					{
						_url = GetURL(this);//CUtils.PathCombine (uri, this.relativeUrl);
					}
					CheckNavtiveFile(_url);
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
                //Debug.Log(" set keyHashCode" + value.ToString()+",key="+key);
                if (value == 0)
                    _keyHashCode = value;
                else
                    _keyHashCode = LuaHelper.StringToHash(key);
            }
        }

        /// <summary>
        /// Sets the U dkey.
        /// </summary>
        /// <value>
        /// The U dkey.
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
        /// 是否异步加载
        /// </summary>
        public bool async = true;

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
                // if (_uris == null)
                //     _uris = LResLoader.uriList;

                return _uris;
            }

            set
            {
                _uris = value;
            }
        }

        /// <summary>
        /// 是否普通加载 普通加载需要等待加载池空闲
        /// </summary>
        public bool isNormal = true;

        /// <summary>
        /// 场景加载追加模式
        /// </summary>
        public bool isAdditive = false;

        /// <summary>
        /// www.LoadFromCacheOrDownload
        /// </summary>
        public bool isLoadFromCacheOrDownload = false;

        /// <summary>
        /// 是否加载本地文件
        /// </summary>
        internal bool isNativeFile{private set;get;}

        /// <summary>
        /// dependencies count;
        /// </summary>
        internal int[] allDependencies;

        /// <summary>
        /// 异步请求的ab
        /// </summary>
        internal AsyncOperation assetBundleRequest;

        /// <summary>
        /// The is asset bundle.
        /// </summary>
        internal bool isAssetBundle = false;
        /// <summary>
        /// 放入内存池
        /// </summary>
        internal bool pool = false;

        private void CheckNavtiveFile(string uri_str)
        {
            if (!string.IsNullOrEmpty(uri_str))
            {
                uri_str = uri_str.ToLower();
                if(uri_str.StartsWith(Common.HTTP_STRING) || uri_str.StartsWith(Common.HTTP_STRING))
                    isNativeFile = false;
                else
                    isNativeFile = true;
            }
        }

		/// <summary>
		/// 获取当前 URL
		/// </summary>
		private static string GetURL(CRequest req)
		{
			string url = string.Empty;
            var uris = req.uris;
            int index = req.index;
            if(uris!=null && uris.count > index && index >= 0)
            {
                url = CUtils.PathCombine(uris[index],req.relativeUrl);
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
            if(uris!=null && uris.count > index && index >= 0)
            {
                url = CUtils.PathCombine(uris[index],req.relativeUrl);
            }
            else
            {
                url = req.relativeUrl;
            }
			return url;
        }
    }

}