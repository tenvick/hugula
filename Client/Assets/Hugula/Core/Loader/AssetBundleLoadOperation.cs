using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using Hugula.Utils;
using Hugula.Update;

namespace Hugula.Loader
{
    #region load assetbundle

    public abstract class AssetBundleDownloadOperation : ResourcesLoadOperation
    {
        protected bool isDone;
        public string error { get; internal set; }
        /// <summary>
        /// assetbundle
        /// </summary>
        public AssetBundle assetBundle { get; protected set; }

        private System.Action m_BeginDownload;

        protected void RegisterBeginDownLoad(System.Action beginDownload)
        {
            m_BeginDownload = beginDownload;
        }

        public void BeginDownload()
        {
#if HUGULA_LOADER_DEBUG
            var req = cRequest;
            HugulaDebug.FilterLogFormat(req.key, "<color=#10f010>1.3 BeginDownload  AssetBundle Request(key={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", req.key, req.assetName, req.keyHashCode, req.dependencies == null ? 0 : req.dependencies.Length, Time.frameCount);
#endif
#if HUGULA_PROFILER_DEBUG
            Profiler.BeginSample(string.Format("AssetBundleDownloadOperation.BeginDownload CRequest({0},{1},shared={2}) ,", cRequest.assetName, cRequest.key, cRequest.isShared));
#endif
            if (m_BeginDownload != null)
                m_BeginDownload();
#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
        }

        public void AddNext(ResourcesLoadOperation assetOperation)
        {
            ResourcesLoadOperation n = this;
            ResourcesLoadOperation next = this.next;
            while (next != null)
            {
                n = next;
                next = n.next;
            }
            n.next = assetOperation;
        }

        public override void Reset()
        {
            base.Reset();
            assetBundle = null;
            error = null;
            isDone = false;
        }

        protected bool _IsDone()
        {
            return isDone;
        }

    }

    //crc check error
    public sealed class AssetBundleDownloadErrorOperation : AssetBundleDownloadOperation
    {
        public AssetBundleDownloadErrorOperation()
        {
            isDone = true;
            RegisterEvent(_Update, _IsDone);
        }

        //il2cpp 
        public override void Reset()
        {
            base.Reset();
        }

        private bool _Update()
        {
            return false;
        }


    }

    public sealed class AssetBundleDownloadFromWebOperation : AssetBundleDownloadOperation
    {
        private UnityWebRequest m_webrequest;
        private AsyncOperation m_asyncOperation;

        public AssetBundleDownloadFromWebOperation()
        {
            RegisterEvent(_Update, _IsDone);
            RegisterBeginDownLoad(_BeginDownload);
        }

        private bool _Update()
        {
            if (!isDone && downloadIsDone)
            {
                FinishDownload();
                isDone = true;
            }

            return !isDone;
        }

        private void _BeginDownload()
        {
            m_webrequest = UnityWebRequest.GetAssetBundle(this.cRequest.url);
            if (m_webrequest != null)
            {
                m_asyncOperation = m_webrequest.Send();
            }
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>1.4 AssetBundleDownloadFromWebOperation Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2}, frameCount{4}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, Time.frameCount);
#endif
        }

        private bool downloadIsDone { get { return (m_webrequest == null) || (m_webrequest.isDone && CacheManager.CheckDependenciesComplete(cRequest)); } }

        private void FinishDownload()
        {
            if (m_webrequest == null)
            {
                error = string.Format("the webrequest is null CRequest({0},{1})", cRequest.key, cRequest.assetName);
                return;
            }

            error = m_webrequest.error;

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }
            else if ((assetBundle = DownloadHandlerAssetBundle.GetContent(m_webrequest)) == null)
            {
                error = string.Format("the asset bundle({0}) is not exist. CRequest({1})", cRequest.key, cRequest.assetName);
            }
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>1.5 AssetBundleDownloadFromWebOperation is done Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},frameCount{5}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, Time.frameCount);
#endif
            m_webrequest.Dispose();
            m_webrequest = null;
            m_asyncOperation = null;
        }

        public override void ReleaseToPool()
        {
            if (pool)
                Release(this);
        }

        //il2cpp 
        public override void Reset()
        {
            base.Reset();
        }

        #region pool
        static ObjectPool<AssetBundleDownloadFromWebOperation> webOperationPool = new ObjectPool<AssetBundleDownloadFromWebOperation>(m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet(AssetBundleDownloadFromWebOperation op)
        {
            op.pool = true;
        }

        private static void m_ActionOnRelease(AssetBundleDownloadFromWebOperation op)
        {
            op.Reset();
        }

        public static AssetBundleDownloadFromWebOperation Get()
        {
            return webOperationPool.Get();
        }

        public static void Release(AssetBundleDownloadFromWebOperation toRelease)
        {
            webOperationPool.Release(toRelease);
        }
        #endregion
    }

    public sealed class AssetBundleDownloadFromDiskOperation : AssetBundleDownloadOperation
    {
        AssetBundleCreateRequest m_abRequest;

        public AssetBundleDownloadFromDiskOperation()
        {
            RegisterEvent(_Update, _IsDone);
            RegisterBeginDownLoad(_BeginDownload);
        }

        private bool _Update()
        {
            if (!isDone && downloadIsDone)
            {
                FinishDownload();
                isDone = true;
            }

            return !isDone;
        }

        private void _BeginDownload()
        {
            /*
            *Stream Compressed (LZMA) Mem: LZ4 compressed bundle size.  Perf: reading from disk + LZMA decompression + LZ4 compression.
            *Chunk Compressed (LZ4)   Mem: no extra memory is used.     Perf: reading from disk.	
            */
            string url = CUtils.GetAndroidABLoadPath(cRequest.url);
            var abInfo = ManifestManager.GetABInfo(cRequest.key);
            if (abInfo != null && abInfo.size < ResourcesLoader.asyncSize)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample("AssetBundle.LoadFromFile:" + url);
#endif
                assetBundle = AssetBundle.LoadFromFile(url);
            }
            else
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample("AssetBundle.LoadFromFileAsync:" + url);
#endif
                m_abRequest = AssetBundle.LoadFromFileAsync(url);
            }

#if HUGULA_PROFILER_DEBUG
            Profiler.EndSample();
#endif
            if (m_abRequest != null) m_abRequest.priority = cRequest.priority;

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>1.4 AssetBundleDownloadFromDiskOperation begin Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},frameCount{5}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, Time.frameCount);
#endif
        }

        private bool downloadIsDone
        {
            get
            {
                return ((m_abRequest != null && m_abRequest.isDone) || assetBundle != null || (assetBundle == null && m_abRequest == null)) && CacheManager.CheckDependenciesComplete(cRequest);
            }
        }

        private void FinishDownload()
        {
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>1.5 AssetBundleDownloadFromDiskOperation is done Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},frameCount{5}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, Time.frameCount);
#endif
            if (m_abRequest != null) assetBundle = m_abRequest.assetBundle;
            if (assetBundle == null)
            {
                error = string.Format("the asset bundle({0}) is not exist. CRequest({1})", cRequest.key, cRequest.assetName);
            }
            m_abRequest = null;

        }

        public override void ReleaseToPool()
        {
            if (pool)
                Release(this);
        }

        //il2cpp 
        public override void Reset()
        {
            base.Reset();
        }

        #region pool
        static ObjectPool<AssetBundleDownloadFromDiskOperation> webOperationPool = new ObjectPool<AssetBundleDownloadFromDiskOperation>(m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet(AssetBundleDownloadFromDiskOperation op)
        {
            op.pool = true;
        }

        private static void m_ActionOnRelease(AssetBundleDownloadFromDiskOperation op)
        {
            op.Reset();
        }

        public static AssetBundleDownloadFromDiskOperation Get()
        {
            return webOperationPool.Get();
        }

        public static void Release(AssetBundleDownloadFromDiskOperation toRelease)
        {
            webOperationPool.Release(toRelease);
        }
        #endregion
    }

    #endregion

}