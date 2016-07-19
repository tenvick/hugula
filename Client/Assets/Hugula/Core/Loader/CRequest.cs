// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.IO;
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
        /// 加载请求 
        /// </summary>
        /// <param name="url"></param>
        public CRequest(string relativeUrl)
        {
            this.relativeUrl = relativeUrl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">地址 相对路径</param>
        /// <param name="assetName">加载的资源名</param>
        /// <param name="assetType">资源类型，默认为GameObject</param>
        public CRequest(string relativeUrl, string assetName, string assetType)
        {
            this.relativeUrl = relativeUrl;
            this.assetName = assetName;
            this.assetType = assetType;
        }

        public void Dispose()
        {
            this._url = null;
            this.relativeUrl = null;
            this.priority = 0;
            this._key = null;
            this._uris = null;
            this._uri = null;
            this.index = 0;
            this._data = null;
            this._head = null;
            this.userData = null;
        }

        private object _data;

        private object _head;

        private string _key, _udKey;

        private int _keyHashCode;

        private string _url;

        private string _assetBundleName = string.Empty;

        private string _assetName = string.Empty;

        private bool _async = true;

        private string[] _uris;

        private string _uri;

        public string uri
        {
            get
            {

                if (string.IsNullOrEmpty(_uri))
                    _uri = CUtils.GetUri(uris, index);
                return _uri;
            }
            set
            {
                _uri = value;
                this._url = Path.Combine(_uri, this.relativeUrl);
                _udKey = CUtils.GetUDKey(_uri, relativeUrl);  //CryptographHelper.CrypfString(this._url);//_key=CUtils.getURLFullFileName(url);		    	
            }
        }

        /// <summary>
        /// Gets the relative URL.
        /// </summary>
        /// <value>The relative URL.</value>
        public string relativeUrl { private set; get; }

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
        public string assetType = string.Empty;

        /// <summary>
        /// 加载的头信息
        /// </summary>
        public object head
        {
            get { return this._head; }
            set { this._head = value; }
        }


        /// <summary>
        /// 加载的数据
        /// </summary>
        public object data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// The user data.
        /// </summary>
        public object userData;

        public event CompleteHandle OnEnd;

        public event CompleteHandle OnComplete;

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
        /// 请求地址 网址，相对路径，绝对路径
        /// </summary>
        public string url
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                    _url = Path.Combine(uri, relativeUrl); //Path.Combine (uri, this.relativeUrl);
                return _url;
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
                    _key = CUtils.GetKeyURLFileName(relativeUrl);
                return _key;
            }
            //        set
            //        {
            //            _key = value;
            //        }
        }

        /// <summary>
        /// assetBundle key的hashcode 用于计算缓存的资源key
        /// </summary>
        public int keyHashCode
        {
            get
            {
                if (_keyHashCode == 0)
                    _keyHashCode = LuaHelper.StringToHash(key);
                return _keyHashCode;
            }
            set
            {
                _keyHashCode = value;
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
                    _udKey = CUtils.GetUDKey(uri, relativeUrl);//_key=CUtils.getURLFullFileName(url);		    	
                return _udKey;
            }
        }


        /// <summary>
        /// 是否异步加载
        /// </summary>
        public bool async
        {
            get
            {
                return _async;
            }
            set
            {
                _async = value;
            }
        }

        /// <summary>
        ///  优先等级
        ///  降序
        ///  值越大优先级越高
        /// </summary>
        public int priority = 0;//优先级

        /// <summary>
        /// 加载次数
        /// </summary>
        public int index = 0;

        /// <summary>
        /// 资源位置列表
        /// </summary>
        public string[] uris
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

    }

    public delegate void CompleteHandle(CRequest req);
}