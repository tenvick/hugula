// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections.Generic;
using Hugula.Utils;
using System.Collections;
using UnityEngine;
using System.Net;

namespace Hugula.Loader {
    /// <summary>
    /// Request.
    /// </summary>
    [SLua.CustomLuaClass]
    public class CRequest : IEnumerator,IDisposable {
        /// <summary>
        /// Request
        /// </summary>
        public CRequest () {

        }

        #region IEnumerator
        public object Current {
            get {
                return null;
            }
        }

        public bool MoveNext () { //false move 
            bool re = data==null;
            if (error != null)
                re = false;
            // Debug.LogFormat("MoveNext {0},error={1},data={2}",re,error,data);
            return re;
        }

        public void Reset () {
            
        }

        #endregion

        public virtual void Dispose () {
            this._url = string.Empty;
            this._vUrl = string.Empty;
            this._key = string.Empty;
            this._udAssetKey = string.Empty;
            this.error = null;

            this.priority = 0;
            this._keyHashCode = 0;

            this.data = null;
            this.uploadData = null;
            this.webHeader = null;

            _assetName = string.Empty;
            assetType = null; //string.Empty;
#if UNITY_IPHONE
            async = false;
#else
            async = true;
#endif

            pool = false;
            isAdditive = false;
            isShared = false;

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

        private string _vUrl,_url;

        private string _key, _udAssetKey;

        private int _keyHashCode = 0;

        private string _assetName = string.Empty;


        /// <summary>
        ///  virtual url 
        /// the base asset BundleName URL.
        /// or http request url
        /// </summary>
        /// <value>virtual url</value>
        public string vUrl {
            set {
                _key = null;
                _keyHashCode = 0;
                _vUrl = value;
            }
            get { return _vUrl; }
        }
      
        /// <summary>
        /// 要加载的asset 名称
        /// </summary>
        public string assetName {
            get {
                if (string.IsNullOrEmpty (_assetName))
                     _assetName = CUtils.GetAssetName (vUrl); //Get// key;
                return _assetName;
            }
            set { _assetName = value; }
        }

        private Type _assetType;
        /// <summary>
        /// asset Type name
        /// </summary>
        public Type assetType {
            get {
                if (_assetType == null)
                    _assetType = LoaderType.Typeof_Object;
                return _assetType;
            }
            set { _assetType = value; }
        }

        public string error{
            get;internal set;
        }

        /// <summary>
        /// 加载的数据
        /// </summary>
        public object data {get;internal set;}

        /// <summary>
        /// The upload data.
        /// </summary>
        public object uploadData;

        /// <summary>
        /// The http head.
        /// </summary>
        public WebHeaderCollection webHeader;

        public System.Action<CRequest> OnEnd;

        public System.Action<CRequest> OnComplete;

        public void DispatchComplete () {
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample (string.Format ("CRequest({0},{1}).DispatchComplete()", this.assetName, this.key));

            if (OnComplete != null)
                OnComplete (this);

            Profiler.EndSample ();
#else
            if (OnComplete != null)
                OnComplete (this);
#endif
        }

        public void DispatchEnd () {
            if (OnEnd != null)
                OnEnd (this);
        }

        public bool isShared { get; internal set; }

        /// <summary>
        /// 请求真实url地址，可能被改变.
        /// </summary>
        public string url {
            get {
                if(string.IsNullOrEmpty(_url))
                    _url = vUrl;
                return _url;
            }
            internal set {
                _url = value;
            }
        }

        /// <summary>
        /// 缓存用的关键字
        /// 如果没有设定 则为默认key生成规则
        /// </summary>
        public string key {
            get {
                if (string.IsNullOrEmpty (_key))
                    key = string.Empty;
                return _key;
            }
            internal set {
                if (value == null)
                    _key = null;
                else//vUrl))
                    _key = CUtils.GetAssetBundleName (vUrl);
            }
        }

        /// <summary>
        /// assetBundle key的hashcode 用于计算缓存的资源key
        /// </summary>
        internal int keyHashCode {
            get {
                if (_keyHashCode == 0)
                    keyHashCode = 1;

                return _keyHashCode;
            }
            set {
                if (value == 0)
                    _keyHashCode = value;
                else
                    _keyHashCode = LuaHelper.StringToHash (key);
            }
        }

        /// <summary>
        /// Sets the url and asse unique key
        /// </summary>
        /// <value>
        /// The unique key.
        /// </value>
        internal string udAssetKey {
            get {
                if (string.IsNullOrEmpty (_udAssetKey))
                    udAssetKey = string.Empty;
                return _udAssetKey;
            }
            set {
                if (value == null)
                    _udAssetKey = null;
                else {
                    _udAssetKey = string.Format ("{0}+{1}", key, assetName);
                }
            }
        }

        /// <summary>
        /// 是否异步加载
        /// </summary>
#if UNITY_IPHONE
        public bool async = false;
#else
        public bool async = true;
#endif

        /// <summary>
        ///  优先等级
        ///  降序
        ///  值越大优先级越高
        /// </summary>
        public int priority = 0; //优先级

        /// <summary>
        /// 场景加载追加模式
        /// </summary>
        public bool isAdditive = false;

        /// <summary>
        /// 所属组
        /// </summary>
        internal GroupQueue<CRequest> group;

        /// <summary>
        /// dependencies count;
        /// </summary>
        internal int[] dependencies;

        /// <summary>
        /// 放入内存池
        /// </summary>
        internal bool pool = false;

        public void ReleaseToPool () {
            if (pool)
                Release (this);
        }

          //创建一个CRequest
        public static CRequest Create (string assetBundleName, string assetName, Type assetType, System.Action<CRequest> onComp, System.Action<CRequest> onEnd) {
            var req = CRequest.Get ();
            req.vUrl = assetBundleName;
            req.assetName = assetName;
            req.assetType = assetType;

            req.OnComplete = onComp;
            req.OnEnd = onEnd;

            return req;
        }

        //创建一个http CRequest
        // public static CRequest Create(string url,System.Action<CRequest> onComp, System.Action<CRequest> onEnd)
        // {
        //     var req = CRequest.Get ();
        //     req.vUrl = url;

        //     req.OnComplete = onComp;
        //     req.OnEnd = onEnd;
        //     return req;
        // }

        #region ObjectPool 
        static ObjectPool<CRequest> objectPool = new ObjectPool<CRequest> (m_ActionOnGet, m_ActionOnRelease);
        private static void m_ActionOnGet (CRequest req) {
            // Debug.LogFormat("",req.isShared)
            req.pool = true;
#if HUGULA_PROFILER_DEBUG
            req.beginQueueTime = System.DateTime.Now;
            req.beginLoadTime = System.DateTime.Now;
#endif
        }

        private static void m_ActionOnRelease (CRequest req) {
            req.Dispose ();
        }

        public static CRequest Get () {
            return objectPool.Get ();
        }

        public static void Release (CRequest toRelease) {
            objectPool.Release (toRelease);
        }
        #endregion
    }

}