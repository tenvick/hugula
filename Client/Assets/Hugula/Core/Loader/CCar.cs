// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using System.Collections;
using System;
using Hugula.Utils;

namespace Hugula.Loader
{
    /// <summary>
    /// 加载资源
    /// </summary>
    [SLua.CustomLuaClass]
    public class CCar
    {
        public CCar()
        {
            isFree = true;
            enabled = false;
        }

        #region protected
        private WWW www;
        private CRequest _req;
        #endregion

        #region public member
        /// <summary>
        /// if transport car is free
        /// </summary>
        /// <value><c>true</c> if is free; otherwise, <c>false</c>.</value>
        public bool isFree { private set; get; }

        /// <summary>
        /// The req.
        /// </summary>
        public CRequest req
        {
            get
            {
                return _req;
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool enabled;
        #endregion

        #region mono

        public void Update()
        {
            if (enabled && www != null && isFree == false)
            {
                float pro = www.progress;
                if (OnProcess != null)
                    OnProcess(this, pro);
                //Debug.LogFormat(" <color=green>loading update  pro {0} isDone{1}</color>", pro, www.isDone);
                if (www.isDone)
                    LoadDone();
            }
        }

        #endregion

        #region public

        public void BeginLoad(CRequest req)
        {
            this.isFree = false;
            this._req = req;
            this.enabled = true;
            string url = CUtils.CheckWWWUrl(req.url);
     
            if (req.head is WWWForm)
            {
                www = new WWW(url, (WWWForm)req.head);
            }
            else if (req.head is byte[])
            {
                www = new WWW(url, (byte[])req.head);
            }
            else
            {
                //			if (url.StartsWith ("http")) {
                //				if(CResLoader.assetBundleManifest!=null)
                //					www = WWW.LoadFromCacheOrDownload (url, CResLoader.assetBundleManifest.GetAssetBundleHash (req.assetBundleName), 0);
                //				else
                //					www = WWW.LoadFromCacheOrDownload (url, 0);
                //			}
                //            else
                www = new WWW(url);
            }

            //Debug.LogFormat("<color=green> begin load {0} </color>", url);
        }

        #endregion

        #region evnet
        public System.Action<CCar, float> OnProcess;
        public System.Action<CCar, CRequest> OnComplete;
        public System.Action<CCar, CRequest> OnError;
        #endregion

        #region proetced method
        protected void LoadDone()
        {

#if HUGULA_PROFILE_DEBUG
            Profiler.BeginSample(this.req.key, this.gameObject);
#endif
            isFree = true;
            enabled = false;
            if (www.error != null)
            {
                Debug.LogWarning(" (" + req.key + ")url(" + req.url + ") \n error:" + www.error);
                DispatchErrorEvent(req);
                www = null;
            }
            else
            {
                if (OnProcess != null)
                    OnProcess(this, 1);
                //Debug.LogFormat("<color=yellow>will complete : url({0}),key:({1}) ab({2}) t({3}) bit({4})</color>", req.url, req.key, req.keyHashCode, www.text, www.bytes.Length);
                object data = null;
                var ab = www.assetBundle;
                if (ab == null) data = www.bytes;
                if (req.assetType != null && req.assetType.Equals("System.Byte[]"))
                    data = www.bytes;
                CacheData cacheData = new CacheData(data, null, req.key);//缓存
                CacheManager.AddCache(cacheData);
                cacheData.allDependencies = this._req.allDependencies;
                cacheData.assetBundle = ab;
                www.Dispose();
                DispatchCompleteEvent(this._req);

            }
#if HUGULA_PROFILE_DEBUG
            Profiler.EndSample();
#endif
        }

        private void DispatchCompleteEvent(CRequest cReq)
        {
            if (OnComplete != null)
            {
                OnComplete(this, cReq);
            }
        }

        private void DispatchErrorEvent(CRequest cReq)
        {
            if (OnError != null)
            {
                OnError(this, cReq);
            }
        }
        #endregion

    }

}