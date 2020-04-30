// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Profiling;

namespace Hugula.Loader {
    /// <summary>
    /// Request.
    /// </summary>
    public sealed class CRequest : IEnumerator, IDisposable {
        /// <summary>
        /// Request
        /// </summary>
        public CRequest () {
#if HUGULA_PROFILER_DEBUG
            watch.Start ();
#endif
        }

        #region IEnumerator
        public object Current {
            get {
                return null;
            }
        }

        public bool MoveNext () { //false move 
            // if (error != null)
            //     return false;
            // else
            return !isDone;
        }

        public void Reset () {

        }

        #endregion

        public void Dispose () {
            // this._url = string.Empty;
            this._assetBundleName = string.Empty;
            this.error = null;
            this.priority = 0;
            this.progress = 0f;
            this.data = null;
            _assetName = string.Empty;
            assetType = null; //string.Empty;
            async = true;
            pool = false;
            isDone = false;
            userData = null;

            this.OnComplete = null;
            this.OnEnd = null;
        }

#if HUGULA_PROFILER_DEBUG
        internal System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
#endif

        private string _assetBundleName, _assetName;

        /// <summary>
        /// the asset BundleName URL.
        /// </summary>
        /// <value>virtual url</value>
        public string assetBundleName {
            set {
                _assetBundleName = value;
            }
            get { return _assetBundleName; }
        }

        /// <summary>
        /// 要加载的asset 名称
        /// </summary>
        public string assetName {
            get {
                if (string.IsNullOrEmpty (_assetName))
                    _assetName = CUtils.GetAssetName (assetBundleName); //Get// key;
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
                    _assetType = typeof (UnityEngine.Object);
                return _assetType;
            }
            internal set { _assetType = value; }
        }

        public string error {
            get;
            private set;
        }

        /// <summary>
        /// 用户自己带的信息
        /// </summary>
        public object userData;

        /// <summary>
        /// What's the operation's progress.
        /// </summary>
        public float progress { get; private set; }

        /// <summary>
        /// 加载的数据
        /// </summary>
        public object data { get; private set; }

        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool isDone { get; private set; }
        public System.Action<object,object> OnEnd;

        public System.Action<object,object> OnComplete;

        public void DispatchComplete () {
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample (string.Format ("CRequest({0},{1}).DispatchComplete()", this.assetName, this.assetBundleName));
            if (OnComplete != null)
                OnComplete (data,userData);
            Profiler.EndSample ();
            watch.Stop ();
            // Debug.LogFormat ("CRequest({0},{1}).data = {2} cost={3}s", this.assetBundleName, this.assetName, this.data, watch.ElapsedMilliseconds / 1000d);
#else

            if (OnComplete != null)
                OnComplete (data,userData);
#endif
        }

        public void DispatchEnd () {
            if (OnEnd != null)
                OnEnd (data,userData);
        }

        /// <summary>
        /// asset 是否异步加载
        /// </summary>
        public bool async = true;

        /// <summary>
        ///  优先等级
        ///  降序
        ///  值越大优先级越高
        /// </summary>
        public int priority = 0; //优先级

        /// <summary>
        /// 放入内存池
        /// </summary>
        bool pool = false;

        public void ReleaseToPool () {
            if (pool)
                Release (this);
        }

        internal static void SetDone (CRequest req) {
            req.isDone = true;
        }

        internal static void SetData (CRequest req, object data) {
            req.data = data;
        }

        internal static void SetError (CRequest req, string error) {
            req.error = error;
            req.isDone = true;
        }

        internal static void SetProgress (CRequest req, float progress) {
            req.progress = progress;
        }

        //创建一个CRequest
        // public static CRequest Create (string assetBundleName, string assetName, Type assetType, System.Action<CRequest> onComp, System.Action<CRequest> onEnd) {
        //     var req = CRequest.Get ();
        //     req.assetBundleName = assetBundleName;
        //     req.assetName = assetName;
        //     req.assetType = assetType;

        //     req.OnComplete = onComp;
        //     req.OnEnd = onEnd;

        //     return req;
        // }

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
            req.pool = true;
#if HUGULA_PROFILER_DEBUG
            req.watch.Start ();
            req.watch.Restart ();
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