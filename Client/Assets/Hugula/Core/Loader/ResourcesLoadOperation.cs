using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using Hugula.Utils;
using Hugula.Update;

namespace Hugula.Loader
{
    public abstract class ResourcesLoadOperation : IEnumerator, IReleaseToPool
    {
        protected bool pool { get; set; }
        private System.Func<bool> m_OnUpdate;
        private System.Func<bool> m_IsDone;
        internal ResourcesLoadOperation next;
        public object Current
        {
            get
            {
                return null;
            }
        }

        public bool MoveNext()
        {
            return !IsDone();
        }

        public virtual void Reset()
        {
            pool = false;
            cRequest = null;
            next = null;
        }

        public bool Update() //virtual function is bad for il2cpp so we use Action
        {
            return m_OnUpdate();
            // return true;
        }

        public bool IsDone() //virtual function is bad for il2cpp so we use Action
        {
            return m_IsDone();
            // return true;
        }

        public virtual void ReleaseToPool()
        {

        }

        internal CRequest cRequest { get; private set; }

        //regiset Update IsDone function
        protected void RegisterEvent(System.Func<bool> onUPdate, System.Func<bool> isDone)
        {
            this.m_OnUpdate = onUPdate;
            this.m_IsDone = isDone;
        }

        public void SetRequest(CRequest req)
        {
            this.cRequest = req;
        }

        public static void SetRequestData(CRequest req, object data)
        {
            if (!req.isDisposed) req.data = data;
        }
    }


    #region http request


    public abstract class HttpLoadOperation : ResourcesLoadOperation
    {
        protected bool isDone;
        protected object m_Data;
        public object GetAsset()
        {
            return m_Data;
        }

        public string error { get; protected set; }

        private System.Action m_BeginDownload;

        protected void RegisterBeginDownLoad(System.Action beginDownload)
        {
            m_BeginDownload = beginDownload;
        }

        public void BeginDownload()
        {
            if (m_BeginDownload != null) m_BeginDownload();
        }

        public override void Reset()
        {
            base.Reset();
            error = null;
            isDone = false;
        }

        protected bool _IsDone()
        {
            return isDone;
        }
    }


    public sealed class WWWRequestOperation : HttpLoadOperation
    {
        private WWW m_webrequest;

        public WWWRequestOperation()
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
            m_Data = null;
            var head = cRequest.head;
            string url = CUtils.CheckWWWUrl(cRequest.url);

            if (head is WWWForm)
            {
                m_webrequest = new WWW(url, (WWWForm)head);
            }
            else if (head is string)
            {
                var bytes = LuaHelper.GetBytes(head.ToString());
                m_webrequest = new WWW(url, bytes);
            }
            else if (head is System.Array)
                m_webrequest = new WWW(url, (byte[])head, cRequest.headers);
            else
                m_webrequest = new WWW(url);

        }

        private void FinishDownload()
        {
            if (m_webrequest == null)
            {
                error = string.Format("webrequest is null , {0} ", cRequest.key); ;
                return;
            }

            error = m_webrequest.error;

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogErrorFormat("url:{0},erro:{1}", cRequest.url, error);
                return;
            }

            var type = cRequest.assetType;
            if (CacheManager.Typeof_AudioClip.Equals(type))
            {
#if UNITY_2017
                m_Data = WWWAudioExtensions.GetAudioClip(m_webrequest);
#else
                m_Data = m_webrequest.audioClip;
#endif
            }
            else if (CacheManager.Typeof_Texture2D.Equals(type))
            {
                if (!string.IsNullOrEmpty(cRequest.assetName) && cRequest.assetName.Equals("textureNonReadable"))
                    m_Data = m_webrequest.textureNonReadable;
                else
                    m_Data = m_webrequest.texture;
            }
            else if (CacheManager.Typeof_AssetBundle.Equals(type))
            {
                m_Data = m_webrequest.assetBundle;
            }
            else if (CacheManager.Typeof_Bytes.Equals(type))
            {
                m_Data = m_webrequest.bytes;
            }
            else
                m_Data = m_webrequest.text;

            UriGroup.CheckWWWComplete(cRequest, m_webrequest);
            cRequest.data = m_Data;
            m_webrequest.Dispose();
            m_webrequest = null;
        }


        public override void Reset()
        {
            base.Reset();
            m_Data = null;
        }

        private bool downloadIsDone
        {
            get
            {
                return m_webrequest == null || m_webrequest.isDone;
            }

        }

        #region pool
        static ObjectPool<WWWRequestOperation> webOperationPool = new ObjectPool<WWWRequestOperation>(m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet(WWWRequestOperation op)
        {
            op.pool = true;
        }

        private static void m_ActionOnRelease(WWWRequestOperation op)
        {
            op.Reset();
        }

        public static WWWRequestOperation Get()
        {
            return webOperationPool.Get();
        }

        public static void Release(WWWRequestOperation toRelease)
        {
            webOperationPool.Release(toRelease);
        }

        #endregion
    }


    public sealed class WebRequestOperation : HttpLoadOperation
    {
        private UnityWebRequest m_webrequest;
        private AsyncOperation m_asyncOperation;

        public WebRequestOperation()
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
            m_Data = null;
            var type = cRequest.assetType;
            var head = cRequest.head;
            if (CacheManager.Typeof_AudioClip.Equals(type))
            {
                AudioType au = AudioType.MOD;
                if (cRequest.head is AudioType) au = (AudioType)cRequest.head;
#if UNITY_2017
                m_webrequest = UnityWebRequestMultimedia.GetAudioClip(cRequest.url, au);
#else
                m_webrequest = UnityWebRequest.GetAudioClip(cRequest.url, au);
#endif
            }
            else if (CacheManager.Typeof_Texture2D.Equals(type))
            {
#if UNITY_2017
                if (cRequest.head is bool)
                    m_webrequest = UnityWebRequestTexture.GetTexture(cRequest.url, (bool)cRequest.head);
                else
                    m_webrequest = UnityWebRequestTexture.GetTexture(cRequest.url);
#else
                if (cRequest.head is bool)
                    m_webrequest = UnityWebRequest.GetTexture(cRequest.url, (bool)cRequest.head);
                else
                    m_webrequest = UnityWebRequest.GetTexture(cRequest.url);
#endif
            }
            else if (head is WWWForm)
            {
                m_webrequest = UnityWebRequest.Post(cRequest.url, (WWWForm)head);
            }
            else if (head is string)
            {
                var bytes = LuaHelper.GetBytes(head.ToString());
                m_webrequest = UnityWebRequest.Put(cRequest.url, bytes);
            }
            else if (head is System.Array)
                m_webrequest = UnityWebRequest.Put(cRequest.url, (byte[])head);
            else
                m_webrequest = UnityWebRequest.Get(cRequest.url);


            if (cRequest.headers != null)
            {
                var headers = cRequest.headers;
                foreach (var kv in headers)
                    m_webrequest.SetRequestHeader(kv.Key, kv.Value);
            }

            m_asyncOperation = m_webrequest.Send();
        }

        private void FinishDownload()
        {
            if (m_webrequest == null)
            {
                error = string.Format("webrequest is null ,key={0},url={1} ", cRequest.key, cRequest.url);
                return;
            }

            if (m_webrequest.isNetworkError)
            {
                error = string.Format("url:{0},erro:{1}", cRequest.url, m_webrequest.error);
                Debug.LogErrorFormat(error);
                return;
            }

            if (m_webrequest.responseCode != 200)
            {
                error = string.Format("response error code = {0},url={1}", m_webrequest.responseCode, cRequest.url); // m_webrequest.error;
                Debug.LogError(error);
                return;
            }

            var type = cRequest.assetType;
            if (CacheManager.Typeof_AudioClip.Equals(type))
            {
                m_Data = DownloadHandlerAudioClip.GetContent(m_webrequest);
            }
            else if (CacheManager.Typeof_Texture2D.Equals(type))
            {
                m_Data = DownloadHandlerTexture.GetContent(m_webrequest);
            }
            else if (CacheManager.Typeof_Bytes.Equals(type))
            {
                m_Data = m_webrequest.downloadHandler.data;
            }
            else
                m_Data = DownloadHandlerBuffer.GetContent(m_webrequest);

            UriGroup.CheckWWWComplete(cRequest, m_webrequest);

            cRequest.data = m_Data;
            m_webrequest.Dispose();
            m_webrequest = null;
            m_asyncOperation = null;
        }


        public override void Reset()
        {
            base.Reset();
            m_Data = null;
        }

        private bool downloadIsDone
        {
            get
            {
                return m_webrequest == null || m_webrequest.isDone;
            }

        }

        #region pool
        static ObjectPool<WebRequestOperation> webOperationPool = new ObjectPool<WebRequestOperation>(m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet(WebRequestOperation op)
        {
            op.pool = true;
        }

        private static void m_ActionOnRelease(WebRequestOperation op)
        {
            op.Reset();
        }

        public static WebRequestOperation Get()
        {
            return webOperationPool.Get();
        }

        public static void Release(WebRequestOperation toRelease)
        {
            webOperationPool.Release(toRelease);
        }

        #endregion
    }


    #endregion


    #region load asset

    public abstract class AssetBundleLoadAssetOperation : ResourcesLoadOperation
    {
        protected object m_Data;
        public string error { get; protected set; }
        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return m_Data as T;
        }

        public override void Reset()
        {
            base.Reset();
            error = null;
            m_Data = null;
        }
    }

    public sealed class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
    {

        private AssetBundleRequest m_Request = null;

        public AssetBundleLoadAssetOperationFull()
        {
            RegisterEvent(_Update, _IsDone);
        }

        // Returns true if more Update calls are required.
        private bool _Update()
        {
            if (m_Request != null)
            {
                if (cRequest.OnComplete != null) return !_IsDone();// wait asset complete
                return false;
            }

            CacheData bundle = CacheManager.TryGetCache(cRequest.keyHashCode);
            if (bundle != null && bundle.isDone && CacheManager.CheckDependenciesComplete(cRequest))
            {
                if (bundle.isError || !bundle.canUse)
                {
                    error = string.Format("load asset({0}) from bundle({1})  error", cRequest.assetName, cRequest.key);
                    return false;
                }
                else
                {
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>2.1 AssetBundleLoadAssetOperationFull  begin Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},frameCount{5}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, Time.frameCount);
#endif
                    var typ = cRequest.assetType;
                    bool loadAll = CacheManager.Typeof_ABAllAssets.Equals(typ);

                    if (cRequest.async)
                    {
                        if (loadAll)
                            m_Request = bundle.assetBundle.LoadAllAssetsAsync();
                        else
                            m_Request = bundle.assetBundle.LoadAssetAsync(cRequest.assetName, typ);

                    }
                    else
                    {
                        if (loadAll)
                            m_Data = bundle.assetBundle.LoadAllAssets();
                        else
                            m_Data = bundle.assetBundle.LoadAsset(cRequest.assetName, typ);

                        if (m_Data == null) error = string.Format("load asset({0}) from {1}  error", cRequest.assetName, cRequest.key);

                    }

                    return !_IsDone();

                }// check bundle

            }
            else
            {
                return true;
            }
        }

        bool _IsDone()
        {
            // error 
            if (error != null)
            {
                Debug.LogWarning(error);
                return true;
            }

            //no async load asset
            if (m_Data != null)
            {
                SetRequestData(cRequest, m_Data);
                return true;
            }

            if (m_Request != null && m_Request.isDone)
            {
                bool loadAll = CacheManager.Typeof_ABAllAssets.Equals(cRequest.assetType);
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(cRequest.key, "<color=#15A0A1>2.1 AssetBundleLoadAssetOperationFull  isDone Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},loadAll={5},frameCount{6}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, loadAll, Time.frameCount);
#endif
                if (loadAll)
                    m_Data = m_Request.allAssets;
                else
                    m_Data = m_Request.asset;

                if (m_Data == null) error = string.Format("load asset({0}) from {1}  error", cRequest.assetName, cRequest.key);

                SetRequestData(cRequest, m_Data);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Reset()
        {
            base.Reset();
            m_Request = null;
        }

        public override void ReleaseToPool()
        {
            if (pool)
                Release(this);
        }

        #region pool
        static ObjectPool<AssetBundleLoadAssetOperationFull> webOperationPool = new ObjectPool<AssetBundleLoadAssetOperationFull>(m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet(AssetBundleLoadAssetOperationFull op)
        {
            op.pool = true;
        }

        private static void m_ActionOnRelease(AssetBundleLoadAssetOperationFull op)
        {
            op.Reset();
        }

        public static AssetBundleLoadAssetOperationFull Get()
        {
            return webOperationPool.Get();
        }

        public static void Release(AssetBundleLoadAssetOperationFull toRelease)
        {
            webOperationPool.Release(toRelease);
        }
        #endregion
    }


#if UNITY_EDITOR
    public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
    {

        public AssetBundleLoadAssetOperationSimulation()
        {
            RegisterEvent(_Update, _IsDone);
        }

        bool _Update()
        {
            return LoadAssetFromPrefab();
        }

        bool _IsDone()
        {
            if (m_Data == null && error != null)
            {
                return true;
            }

            if (m_Data != null)
            {
                cRequest.data = m_Data;
                return true;
            }
            return false;
        }

        bool LoadAssetFromPrefab()
        {
            var req = cRequest;
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(req.assetBundleName, req.assetName);
            if (assetPaths.Length == 0)
            {
                error = "There is no asset with name \"" + req.assetName + "\" in " + req.assetBundleName;
                Debug.LogError(error);
                return false;
            }
            var assetType = req.assetType;
            bool loadAll = CacheManager.Typeof_ABAllAssets.Equals(assetType);
            object data = null;
            List<UnityEngine.Object> datas = null;
            foreach (var p in assetPaths)
            {
                if (loadAll)
                {
                    // data
                    UnityEngine.Object[] target = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(p);
                    if (datas == null)
                    {
                        datas = new List<UnityEngine.Object>();
                        datas.AddRange(target);
                    }
                    else
                    {
                        datas.AddRange(target);
                    }
                    data = datas.ToArray();
                }
                else
                {
                    UnityEngine.Object target = UnityEditor.AssetDatabase.LoadAssetAtPath(p, assetType);
                    if (target)
                    {
                        data = target;
                        break;
                    }
                }
            }

            m_Data = data;
            cRequest.data = m_Data;

            return false;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadAssetOperation
    {
        AsyncOperation m_Operation = null;
        public AssetBundleLoadLevelSimulationOperation()
        {
            RegisterEvent(_Update, _IsDone);
        }

        protected void LoadLevelSimulation()
        {
            string assetBundleName = cRequest.assetBundleName;
            string levelName = cRequest.assetName;
            bool isAdditive = cRequest.isAdditive;
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, levelName);
            if (levelPaths.Length == 0)
            {
                error = string.Format("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
                Debug.LogError(error);
                return;
            }

            if (isAdditive)
                m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
            else
                m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
        }

        private bool _Update()
        {
            LoadLevelSimulation();
            return false;
        }

        public bool _IsDone()
        {
            return m_Operation == null || m_Operation.isDone;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
#endif


    public class AssetBundleLoadLevelOperation : AssetBundleLoadAssetOperation
    {
        protected AsyncOperation m_Request;

        public AssetBundleLoadLevelOperation()
        {
            RegisterEvent(_Update, _IsDone);
        }

        private bool _Update()
        {
            if (m_Request != null)
                return false;

            CacheData bundle = CacheManager.TryGetCache(cRequest.keyHashCode);
            if (bundle != null && bundle.isDone)
            {
                if (bundle.isError || !bundle.canUse)
                {
                    error = string.Format("load asset form {0} error", cRequest.assetName, cRequest.key);
                    return false;
                }
                else
                {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                if (cRequest.isAdditive)
                    m_Request = Application.LoadLevelAdditiveAsync(cRequest.assetName);
                else
                    m_Request = Application.LoadLevelAsync(cRequest.assetName);
#else
                    m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(cRequest.assetName, cRequest.isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
#endif
                    return false;
                }
            }
            else
                return true;
        }

        bool _IsDone()
        {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && error != null)
            {
                Debug.LogError(error);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }

        //il2cpp 
        public override void Reset()
        {
            base.Reset();
        }

    }


    #endregion

}