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
            this._uri = string.Empty;
            this._udKey = string.Empty;

            this.priority = 0;
            this.index = 0;
            this._keyHashCode = 0;
            maxTimes = 2;

            this._uris = null;
            this.data = null;
            this.head = null;
            this.userData = null;

            _assetBundleName = string.Empty;
            _assetName = string.Empty;
			assetType = null;//string.Empty;

            async = true;
            pool = false;
            isShared = false;
            clearCacheOnComplete = false;

            this.assetBundleRequest = null;
            this.allDependencies = null;
        }

        private string _relativeUrl;

        private string _key, _udKey;

        private int _keyHashCode = 0;

        private string _url;

        private string _assetBundleName = string.Empty;

        private string _assetName = string.Empty;

        private UriGroup _uris;

        private string _uri;

        public string uri
        {
            get
            {
                if (string.IsNullOrEmpty(_uri))
                    _uri = uris.GetUri(index);
                return _uri;
            }
            set
            {
                _url = null;
                _udKey = null;
                _uri = uris.GetUri(index);
                //_uri = value;
                //_url = Path.Combine(_uri, this.relativeUrl);
                //_udKey = CUtils.GetUDKey(_uri, relativeUrl);  //CryptographHelper.CrypfString(this._url);//_key=CUtils.getURLFullFileName(url);		    	
            }
        }

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
                    _assetBundleName = CUtils.GetURLFullFileName(relativeUrl);
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
		public Type assetType ;//= string.Empty;

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
        Profiler.BeginSample(this.key+"_onComplete");

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
                    _url = Path.Combine(uri, relativeUrl); //Path.Combine (uri, this.relativeUrl);
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
                    _key = CUtils.GetKeyURLFileName(relativeUrl);
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
                    _udKey = CUtils.GetUDKey(uri, relativeUrl);
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
                if (_uris == null)
                    _uris = CResLoader.uriList;

                return _uris;
            }

            set
            {
                _uris = value;
            }
        }

        /// <summary>
        /// The max try uri times.
        /// </summary>
        public int maxTimes = 2;

        /// <summary>
        /// dependencies count;
        /// </summary>
        internal int[] allDependencies;

        /// <summary>
        /// 异步请求的ab
        /// </summary>
        internal AssetBundleRequest assetBundleRequest;

        /// <summary>
        /// 加载完成后清理缓存
        /// </summary>
        internal bool clearCacheOnComplete = false;

        /// <summary>
        /// 放入内存池
        /// </summary>
        internal bool pool = false;
    }


    /// <summary>
    /// uri 组策略
    /// </summary>
    [SLua.CustomLuaClass]
    public class UriGroup
    {
        #region member
        //private
        private List<string> uris;

        //public
        /// <summary>
        /// 需要进行crc check 的uri索引
        /// </summary>
        public int crcIndex { private set; get; }

        public int count { get { return uris.Count; } }
        #endregion

        public UriGroup()
        {
            uris = new List<string>();
            crcIndex = -1;
        }

        /// <summary>
        /// 添加uri
        /// </summary>
        /// <param name="uri"></param>
        public void Add(string uri)
        {
            uris.Add(uri);
        }

        /// <summary>
        /// 设置需要校验的索引
        /// </summary>
        /// <param name="i"></param>
        public void SetCrcIndex(int i)
        {
            crcIndex = i;
        }

        /// <summary>
        /// 判断当前索引是否需要校验
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public bool IsCrcIndex(int index)
        {
            return index == crcIndex;
        }

        /// <summary>
        /// 获取当前索引的uri
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetUri(int index)
        {
            string uri = "";
            if (uris.Count > index && index >= 0)
            {
                uri = uris[index];
            }
            return uri;
        }

        /// <summary>
        /// 设置req index处的uri
        /// </summary>
        /// <param name="req"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetNextUri(CRequest req)
        {
            int index = req.index + 1;
            if (index >= count) index = 0;
            string uri = GetUri(index);
            if (!string.IsNullOrEmpty(uri))
            {
                req.index = index;
                req.uri = uri;
                return true;
            }
            return false;
        }
    }
}