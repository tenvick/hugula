// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula

using UnityEngine;
using Hugula.Utils;
using System;

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

        private AssetBundleCreateRequest abRequest;
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
            if (enabled && isFree == false)
            {
                if (www != null)
                {
                    float pro = www.progress;
                    if (OnProcess != null)
                        OnProcess(this, pro);
                    if (www.isDone)
                        WWWLoadDone();
                }
                if (abRequest != null)
                {
                    float pro = abRequest.progress;
                    if (OnProcess != null)
                        OnProcess(this, pro);
                    if (abRequest.isDone)
                        AssetBundleFileLoadDone();
                }

            }
        }

        #endregion

        #region public

        public void BeginLoad(CRequest req)
        {
            this.isFree = false;
            this._req = req;
            this.enabled = true;
            string url = req.url;
#if HUGULA_WEB_MODE
            if (req.isAssetBundle) url = req.uris.OnOverrideUrl(req);
#endif
            url = CUtils.CheckWWWUrl(url);

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            if (req.isLoadFromCacheOrDownload)
#else
            if (req.isNativeFile && req.isAssetBundle)
            {
                /*
                *Stream Compressed (LZMA) Mem: LZ4 compressed bundle size.  Perf: reading from disk + LZMA decompression + LZ4 compression.
                *Chunk Compressed (LZ4)   Mem: no extra memory is used.     Perf: reading from disk.	
                */
                #if UNITY_ANDROID && !UNITY_EDITOR
                    string android_url = req.url;
                    using(gstring.Block())
                    {
                        gstring gurl = android_url;
                        gurl = gurl.Replace(Common.JAR_FILE,""); //                    android_url = android_url.Replace(Common.JAR_FILE,"").Replace(;
                        gurl = gurl.Replace("apk!/assets","apk!assets");
                        android_url = gurl.Intern();
                    }
                    abRequest = AssetBundle.LoadFromFileAsync(android_url);
                #else
                    abRequest = AssetBundle.LoadFromFileAsync(req.url);
                #endif
                // Debug.LogFormat(" 0. <color=#8cacbc> begin load : url({0}),key:({1}) assetName({2}) abName({3}) frame{4} isAssetBundle:{5} )</color>", url, req.key, req.assetName,req.assetBundleName,Time.frameCount,req.isAssetBundle);
            } 
            else if (req.isLoadFromCacheOrDownload)
#endif
            {
                if (CResLoader.assetBundleManifest != null)
                {
                    www = WWW.LoadFromCacheOrDownload(url, CResLoader.assetBundleManifest.GetAssetBundleHash(req.assetBundleName), 0);
                }
                else
                {
                    www = WWW.LoadFromCacheOrDownload(url, 0);
                }
            }
            else if (req.head is WWWForm)
            {
                www = new WWW(url, (WWWForm)req.head);
            }
            else if (req.head is byte[])
            {
                www = new WWW(url, (byte[])req.head);
            }
            else
            {
                www = new WWW(url);
            }

            if (www != null)
            {
                if (req.priority > 10000)
                    www.threadPriority = ThreadPriority.High;
                else if (req.priority < -10000)
                    www.threadPriority = ThreadPriority.Low;
                else if (req.priority < 0)
                    www.threadPriority = ThreadPriority.BelowNormal;
            }
            if(abRequest!=null)
            {
                abRequest.priority = req.priority;
            }
#if HUGULA_LOADER_DEBUG
			Debug.LogFormat(" 0. <color=#8cacbc> begin load : url({0}),key:({1}) assetName({2}) abName({3}) isNativeFile({4}) isAssetBundle({5}) frame{6} )</color>", url, req.key, req.assetName,req.assetBundleName,req.isNativeFile,req.isAssetBundle,Time.frameCount);
#endif
        }

        public void StopLoad()
        {
            if (isFree && !enabled)
            {
                enabled = false;
                isFree = true;
                if (www != null)
                {
                    www.Dispose();
                    www = null;
                }
                if (abRequest != null) abRequest = null;
                if (this.req != null && req.pool)
                {
                    LRequestPool.Release(req);
                }
                this._req = null;
            }

        }

        #endregion

        #region evnet
        public System.Action<CCar, float> OnProcess;
        public System.Action<CCar, CRequest> OnComplete;
        public System.Action<CCar, CRequest> OnError;
        #endregion

        #region proetced method
        protected void WWWLoadDone()
        {

#if HUGULA_PROFILE_DEBUG
            Profiler.BeginSample("CCar.LoadDone:" + this.req.assetName);
#endif
            isFree = true;
            enabled = false;
            if (www.error != null)
            {
                Debug.LogWarningFormat(" ({0})url({1}) isNormal({2}) \n error:{3}", req.assetName, req.url, req.isNormal, www.error);
                DispatchErrorEvent(req);
                www = null;
            }
            else
            {
                if (OnProcess != null)
                    OnProcess(this, 1);
#if HUGULA_LOADER_DEBUG
				Debug.LogFormat(" 1. <color=#8cacbc> will complete : url({0}),key:({1}) assetName({2})  len({3} frame{4})</color>", req.url, req.key, req.assetName, www.bytes.Length,Time.frameCount);
#endif
                if (req.isLoadFromCacheOrDownload) //load from cache no www.bytes
                {
                    CacheManager.AddSourceCacheDataFromWWW(www.assetBundle, this._req);
                }
                else
                {
                    CacheManager.AddSourceCacheDataFromWWW(www, this._req);
                }
                DispatchCompleteEvent(this._req);
                www = null;
            }
#if HUGULA_PROFILE_DEBUG
            Profiler.EndSample();
#endif
        }

        protected void AssetBundleFileLoadDone()
        {

#if HUGULA_PROFILE_DEBUG
            Profiler.BeginSample("CCar.AssetBundleFileLoadDone:" + this.req.assetName);
#endif
            isFree = true;
            enabled = false;

            var ab = abRequest.assetBundle;
            if (ab == null)
            {
                Debug.LogWarningFormat(" ({0}) isNormal({2}) \n error:Unable to open archive file:{1}", req.assetName, req.url, req.isNormal);
                DispatchErrorEvent(req);
                abRequest = null;
            }
            else
            {
                if (OnProcess != null)
                    OnProcess(this, 1);
#if HUGULA_LOADER_DEBUG
				Debug.LogFormat(" 1. <color=#8cacbc> will complete : url({0}),key:({1}) assetName({2}) frame{3})</color>", req.url, req.key, req.assetName, Time.frameCount);
#endif
                CacheManager.AddSourceCacheDataFromWWW(ab, this._req);
                DispatchCompleteEvent(this._req);
                abRequest = null;
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
            this._req = null;
        }

        private void DispatchErrorEvent(CRequest cReq)
        {
            if (OnError != null)
            {
                OnError(this, cReq);
            }
            this._req = null;
        }
        #endregion

    }

}