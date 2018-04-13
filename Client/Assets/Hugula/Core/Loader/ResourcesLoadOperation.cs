using System.Collections;
using System.Collections.Generic;
using Hugula.Update;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Hugula.Loader {
    public abstract class ResourcesLoadOperation : IEnumerator, IReleaseToPool {
        protected bool pool = false;
        private System.Func<bool> m_OnUpdate;
        private System.Func<bool> m_IsDone;
        internal ResourcesLoadOperation next;
        public object Current {
            get {
                return null;
            }
        }

        public bool MoveNext () {
            return !IsDone ();
        }

        public virtual void Reset () {
            pool = false;
            cRequest = null;
            next = null;
        }

        public bool Update () //virtual function is bad for il2cpp so we use Action
        {
            return m_OnUpdate ();
            // return true;
        }

        public bool IsDone () //virtual function is bad for il2cpp so we use Action
        {
            return m_IsDone ();
            // return true;
        }

        public virtual void ReleaseToPool () {

        }

        internal CRequest cRequest { get; private set; }

        //regiset Update IsDone function
        protected void RegisterEvent (System.Func<bool> onUPdate, System.Func<bool> isDone) {
            this.m_OnUpdate = onUPdate;
            this.m_IsDone = isDone;
        }

        public void SetRequest (CRequest req) {
            this.cRequest = req;
        }

        public static void SetRequestData (CRequest req, object data) {
            if (!req.isDisposed) req.data = data;
        }
    }

    #region http request

    public abstract class HttpLoadOperation : ResourcesLoadOperation {
        protected bool isDone;
        protected object m_Data;
        public object GetAsset () {
            return m_Data;
        }

        public string error { get; internal set; }

        private System.Action m_BeginDownload;

        protected void RegisterBeginDownLoad (System.Action beginDownload) {
            m_BeginDownload = beginDownload;
        }

        public void BeginDownload () {
            if (m_BeginDownload != null) m_BeginDownload ();
        }

        public override void Reset () {
            base.Reset ();
            error = null;
            isDone = false;
        }

        protected bool _IsDone () {
            return isDone;
        }
    }

    public sealed class WWWRequestOperation : HttpLoadOperation {
        private WWW m_webrequest;

        public WWWRequestOperation () {
            RegisterEvent (_Update, _IsDone);
            RegisterBeginDownLoad (_BeginDownload);
        }

        private bool _Update () {
            if (!isDone && downloadIsDone) {
                FinishDownload ();
                isDone = true;
            }

            return !isDone;
        }

        private void _BeginDownload () {
            m_Data = null;
            var head = cRequest.head;
            string url = CUtils.CheckWWWUrl (cRequest.url);
            bool isOverrideHost = string.IsNullOrEmpty (cRequest.overrideHost);

            if (head is WWWForm) {
                m_webrequest = new WWW (url, (WWWForm) head);
            } else if (head is string) {
                var bytes = LuaHelper.GetBytes (head.ToString ());
                m_webrequest = new WWW (url, bytes);
            } else if (isOverrideHost) {
                System.Array bytes = null;
                if (head is System.Array) bytes = (byte[]) head;
                Dictionary<string, string> headers = new Dictionary<string, string> ();
                if (cRequest.headers != null) {
                    foreach (var kv in cRequest.headers) {
                        headers[kv.Key] = kv.Value;
                    }
                }
                headers["host"] = cRequest.overrideHost;
                m_webrequest = new WWW (url, (byte[]) head, cRequest.headers);
            } else
                m_webrequest = new WWW (url);

        }

        private void FinishDownload () {
            if (m_webrequest == null) {
                error = string.Format ("webrequest is null , {0} ", cRequest.key);
                Debug.LogErrorFormat ("url:{0},erro:{1}", cRequest.url, error);
                return;
            }

            error = m_webrequest.error;

            if (!string.IsNullOrEmpty (error)) {
                Debug.LogErrorFormat ("url:{0},erro:{1}", cRequest.url, error);
                return;
            }

            var type = cRequest.assetType;
            if (CacheManager.Typeof_AudioClip.Equals (type)) {
#if UNITY_2017
                m_Data = WWWAudioExtensions.GetAudioClip (m_webrequest);
#else
                m_Data = m_webrequest.GetAudioClip ();
#endif
            } else if (CacheManager.Typeof_Texture2D.Equals (type)) {
                if (!string.IsNullOrEmpty (cRequest.assetName) && cRequest.assetName.Equals ("textureNonReadable"))
                    m_Data = m_webrequest.textureNonReadable;
                else
                    m_Data = m_webrequest.texture;
            } else if (CacheManager.Typeof_AssetBundle.Equals (type)) {
                m_Data = m_webrequest.assetBundle;
            } else if (CacheManager.Typeof_Bytes.Equals (type)) {
                m_Data = m_webrequest.bytes;
            } else
                m_Data = m_webrequest.text;

            UriGroup.CheckWWWComplete (cRequest, m_webrequest);

            cRequest.data = m_Data;
            m_webrequest.Dispose ();
            m_webrequest = null;
        }

        public override void Reset () {
            base.Reset ();
            m_Data = null;
        }

        private bool downloadIsDone {
            get {
                return m_webrequest == null || m_webrequest.isDone;
            }

        }

        public override void ReleaseToPool () {
            if (pool)
                Release (this);
        }

        #region pool
        static ObjectPool<WWWRequestOperation> webOperationPool = new ObjectPool<WWWRequestOperation> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (WWWRequestOperation op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (WWWRequestOperation op) {
            op.Reset ();
        }

        public static WWWRequestOperation Get () {
            return webOperationPool.Get ();
        }

        public static void Release (WWWRequestOperation toRelease) {
            webOperationPool.Release (toRelease);
        }

        #endregion
    }

    public sealed class WebRequestOperation : HttpLoadOperation {
        private UnityWebRequest m_webrequest;
        private AsyncOperation m_asyncOperation;

        public WebRequestOperation () {
            RegisterEvent (_Update, _IsDone);
            RegisterBeginDownLoad (_BeginDownload);
        }

        private bool _Update () {
            if (!isDone && downloadIsDone) {
                FinishDownload ();
                isDone = true;
            }

            return !isDone;
        }

        private void _BeginDownload () {
            m_Data = null;
            var type = cRequest.assetType;
            var head = cRequest.head;
            if (CacheManager.Typeof_AudioClip.Equals (type)) {
                AudioType au = AudioType.MOD;
                if (cRequest.head is AudioType) au = (AudioType) cRequest.head;
#if UNITY_2017
                m_webrequest = UnityWebRequestMultimedia.GetAudioClip (cRequest.url, au);
#else
                m_webrequest = UnityWebRequest.GetAudioClip (cRequest.url, au);
#endif
            }else if(CacheManager.Typeof_AssetBundle.Equals (type)) {
                m_webrequest =  UnityWebRequest.GetAssetBundle(cRequest.url);
            }            
            else if (CacheManager.Typeof_Texture2D.Equals (type)) {
#if UNITY_2017
                if (cRequest.head is bool)
                    m_webrequest = UnityWebRequestTexture.GetTexture (cRequest.url, (bool) cRequest.head);
                else
                    m_webrequest = UnityWebRequestTexture.GetTexture (cRequest.url);
#else
                if (cRequest.head is bool)
                    m_webrequest = UnityWebRequest.GetTexture (cRequest.url, (bool) cRequest.head);
                else
                    m_webrequest = UnityWebRequest.GetTexture (cRequest.url);
#endif
            } else if (head is WWWForm) {
                m_webrequest = UnityWebRequest.Post (cRequest.url, (WWWForm) head);
            } else if (head is string) {
                var bytes = LuaHelper.GetBytes (head.ToString ());
                m_webrequest = UnityWebRequest.Put (cRequest.url, bytes);
            } else if (head is System.Array)
                m_webrequest = UnityWebRequest.Put (cRequest.url, (byte[]) head);
            else
                m_webrequest = UnityWebRequest.Get (cRequest.url);

            if (cRequest.headers != null) {
                var headers = cRequest.headers;
                foreach (var kv in headers)
                    m_webrequest.SetRequestHeader (kv.Key, kv.Value);
            }

            if (!string.IsNullOrEmpty (cRequest.overrideHost)) {
                m_webrequest.SetRequestHeader ("host", cRequest.overrideHost);
            }

            m_asyncOperation = m_webrequest.Send ();

        }

        private void FinishDownload () {
            if (m_webrequest == null) {
                error = string.Format ("webrequest is null ,key={0},url={1} ", cRequest.key, cRequest.url);
                return;
            }
            cRequest.responseHeaders = m_webrequest.GetResponseHeaders();
#if UNITY_2017
            if (m_webrequest.isNetworkError)
#else
                if (m_webrequest.isError)
#endif
            {
                error = string.Format ("url:{0},erro:{1}", cRequest.url, m_webrequest.error);
                Debug.LogError (error);
                return;
            }

            if (!(m_webrequest.responseCode == 200 || m_webrequest.responseCode == 0)) {
                error = string.Format ("response error code = {0},url={1}", m_webrequest.responseCode, cRequest.url); // m_webrequest.error;
                Debug.LogError (error);
                return;
            }

            var type = cRequest.assetType;
            if (CacheManager.Typeof_AudioClip.Equals (type)) {
                m_Data = DownloadHandlerAudioClip.GetContent (m_webrequest);
            } else if (CacheManager.Typeof_Texture2D.Equals (type)) {
                m_Data = DownloadHandlerTexture.GetContent (m_webrequest);
            } else if (CacheManager.Typeof_Bytes.Equals (type)) {
                m_Data = m_webrequest.downloadHandler.data;
            } else if (CacheManager.Typeof_AssetBundle.Equals (type)) {
                m_Data = DownloadHandlerAssetBundle.GetContent(m_webrequest);
            } else
                m_Data = DownloadHandlerBuffer.GetContent (m_webrequest);

            if(!CacheManager.Typeof_AssetBundle.Equals (type))
                UriGroup.CheckWWWComplete (cRequest, m_webrequest);

            cRequest.data = m_Data;
            m_webrequest.Dispose ();
            m_webrequest = null;
            m_asyncOperation = null;
        }

        public override void Reset () {
            base.Reset ();
            m_Data = null;
        }

        private bool downloadIsDone {
            get {
                return m_webrequest == null || m_webrequest.isDone;
            }

        }

        public override void ReleaseToPool () {
            if (pool)
                Release (this);
        }

        #region pool
        static ObjectPool<WebRequestOperation> webOperationPool = new ObjectPool<WebRequestOperation> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (WebRequestOperation op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (WebRequestOperation op) {
            op.Reset ();
        }

        public static WebRequestOperation Get () {
            return webOperationPool.Get ();
        }

        public static void Release (WebRequestOperation toRelease) {
            webOperationPool.Release (toRelease);
        }

        #endregion
    }

    #endregion

    #region  http dns operation
    public sealed class HttpDnsResolve :ResourcesLoadOperation
    {
        private ResourcesLoadOperation originalOperation;
        public HttpDnsResolve () {
            RegisterEvent (_Update, _IsDone);
        }

        public void SetOriginalOperation(ResourcesLoadOperation originalOperation)
        {
            this.originalOperation = originalOperation;
        }

         bool _Update () {
             string url = HttpDns.GetUrl(cRequest.url);// get dsn ip
             if(string.IsNullOrEmpty(url))
             {
                 return true; //wait for ip 
             }

            if (url != cRequest.url) {
                cRequest.overrideHost = cRequest.uri.Host;// set host
                cRequest.overrideUrl = url;
                // request 
                Debug.LogFormat("request ip {0}  override host {1} ",url,cRequest.overrideHost);
                ResourcesLoader.HttpRequest(cRequest);
            }else if(originalOperation!=null)
            {
                Debug.LogFormat(" dns resolve fail request url {0}   ",url);
                ResourcesLoader.ProcessFinishedOperation(originalOperation);
            }else
            {
                Debug.LogFormat("dns resolve fail , complete request url {0} ",url);

                cRequest.DispatchEnd();

                if (cRequest.group != null) cRequest.group.Complete(cRequest, true);

                cRequest.ReleaseToPool();
            }
                
            return false;
        }

        bool _IsDone () {
            string url = HttpDns.GetUrl(cRequest.url);
            return  !string.IsNullOrEmpty(url);
        }

        public override void Reset()
        {
            base.Reset();
            this.originalOperation = null;
        }

        public override void ReleaseToPool () {
            if (pool)
                Release (this);
        }

        #region pool
        static ObjectPool<HttpDnsResolve> httpDnsOperationPool = new ObjectPool<HttpDnsResolve> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (HttpDnsResolve op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (HttpDnsResolve op) {
            op.Reset ();
        }

        public static HttpDnsResolve Get () {
            return httpDnsOperationPool.Get ();
        }

        public static void Release (HttpDnsResolve toRelease) {
            httpDnsOperationPool.Release (toRelease);
        }

        #endregion
    }

    #endregion




    #region load asset

    public abstract class AssetBundleLoadAssetOperation : ResourcesLoadOperation {
        protected object m_Data;
        public string error { get; protected set; }
        public T GetAsset<T> () where T : UnityEngine.Object {
            return m_Data as T;
        }

        public override void Reset () {
            base.Reset ();
            error = null;
            m_Data = null;
        }
    }
    

    public sealed class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation {

        private AssetBundleRequest m_Request = null;
        private bool isLoadAll = false;
        string url;

#if HUGULA_LOADER_DEBUG
        string assetName;
#endif
        public AssetBundleLoadAssetOperationFull () {
            RegisterEvent (_Update, _IsDone);
        }

        // Returns true if more Update calls are required.
        private bool _Update () {
            url = cRequest.url;

            if (m_Request != null) { // wait asset complete
                // if (cRequest.OnComplete != null) return !_IsDone (); // wait asset complete
                return !_IsDone ();
            }

            CacheData bundle = CacheManager.TryGetCache (cRequest.keyHashCode);
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.0 AssetBundleLoadAssetOperationFull.update  loadasset Request(url={0},assetname={1},CheckDependenciesComplete={3})bundle={2},asyn={4},frameCount={5}</color>", cRequest.url, cRequest.assetName, bundle, CacheManager.CheckDependenciesComplete (cRequest), cRequest.async, Time.frameCount);
#endif
            if (bundle != null && bundle.isDone && CacheManager.CheckDependenciesComplete (cRequest)) {
                if (bundle.isError || !bundle.canUse) {
                    error = string.Format ("load asset({0}) from bundle({1})  error", cRequest.assetName, cRequest.key);
                    return false;
                } else {
#if HUGULA_LOADER_DEBUG
                    assetName=cRequest.assetName;
                    HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.0.1 AssetBundleLoadAssetOperationFull.update  loadasset Request(url={0},assetname={1},dependencies.count={3})keyHashCode{2},asyn={4},frameCount={5}</color>", cRequest.url, cRequest.assetName, cRequest.keyHashCode, cRequest.dependencies == null ? 0 : cRequest.dependencies.Length, cRequest.async, Time.frameCount);
#endif
                    var typ = cRequest.assetType;
                    isLoadAll = CacheManager.Typeof_ABAllAssets.Equals (typ);

                    if (cRequest.async) {
                        if (isLoadAll)
                            m_Request = bundle.assetBundle.LoadAllAssetsAsync ();
                        else
                            m_Request = bundle.assetBundle.LoadAssetAsync (cRequest.assetName, typ);

                    } else {
                        if (isLoadAll)
                            m_Data = bundle.assetBundle.LoadAllAssets ();
                        else
                            m_Data = bundle.assetBundle.LoadAsset (cRequest.assetName, typ);

                        if (m_Data == null) error = string.Format ("load asset({0}) from {1}  error", cRequest.assetName, cRequest.key);

                    }
#if HUGULA_LOADER_DEBUG
                    HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.0.2 AssetBundleLoadAssetOperationFull.update isdone Request(url={0},assetname={1}),m_Request={2},m_Data={3},error={4},frameCount={5}</color>", cRequest.url, cRequest.assetName, m_Request,m_Data,error, Time.frameCount);
#endif
                    return !_IsDone ();

                } // check bundle

            } else {
                return true;
            }
        }

        bool _IsDone () {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.1.1 AssetBundleLoadAssetOperationFull._IsDone Request(url={0},assetName={1}) ,m_Request={2},m_Data={3},error={4},frameCount={5}</color>", url,assetName,m_Request,m_Data,error, Time.frameCount);
#endif
            // error 
            if (error != null) {
                Debug.LogError (error);
                return true;
            }

            if (m_Request != null && m_Request.isDone) {

                if (isLoadAll)
                    m_Data = m_Request.allAssets;
                else
                    m_Data = m_Request.asset;

                if (m_Data == null) error = string.Format ("load asset({0}) from {1}  error", m_Request, url);

                SetRequestData (cRequest, m_Data);
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.1.2 AssetBundleLoadAssetOperationFull._IsDone async isdone Request(url={0},assetname={1}),m_Request={2},m_Data={3},error={4},loadAll={5},frameCount={6}</color>", url,assetName, m_Request,m_Data,error,isLoadAll, Time.frameCount);
#endif
                return true;
            } 
            
            //no async load asset
            if (m_Data != null) {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1.1.3 AssetBundleLoadAssetOperationFull._IsDone isdone Request(url={0},assetName={1}) ,m_Request={2},m_Data={3},error={4},frameCount={5}</color>", url,assetName,m_Request,m_Data,error, Time.frameCount);
#endif
                SetRequestData (cRequest, m_Data);
                return true;
            }
                
            return false;
        }

        public override void Reset () {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1 AssetBundleLoadAssetOperationFull.Reset Request(url={0},assetName={1}) ,m_Request={2},m_Data={3},error={4},frameCount={5}</color>", url,assetName,m_Request,m_Data,error, Time.frameCount);
#endif
            base.Reset ();
            url = null;
            m_Request = null;
            isLoadAll = false;

        }

        public override void ReleaseToPool () {
#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, "<color=#15A0A1>2.1 AssetBundleLoadAssetOperationFull.ReleaseToPool Request(url={0},assetName={1}) ,m_Request={2},m_Data={3},error={4},pool={5},frameCount={6}</color>", url,assetName,m_Request,m_Data,error, pool,Time.frameCount);
#endif
            if (pool)
                Release (this);
        }

        #region pool
        static ObjectPool<AssetBundleLoadAssetOperationFull> webOperationPool = new ObjectPool<AssetBundleLoadAssetOperationFull> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (AssetBundleLoadAssetOperationFull op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (AssetBundleLoadAssetOperationFull op) {
            op.Reset ();
        }

        public static AssetBundleLoadAssetOperationFull Get () {
            return webOperationPool.Get ();
        }

        public static void Release (AssetBundleLoadAssetOperationFull toRelease) {
            webOperationPool.Release (toRelease);
        }
        #endregion
    }

#if UNITY_EDITOR
    public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation {

        public AssetBundleLoadAssetOperationSimulation () {
            RegisterEvent (_Update, _IsDone);
        }

        bool _Update () {
            return LoadAssetFromPrefab ();
        }

        bool _IsDone () {
            if (m_Data == null && error != null) {
                return true;
            }

            if (m_Data != null) {
                cRequest.data = m_Data;
                return true;
            }
            return false;
        }

        bool LoadAssetFromPrefab () {
            var req = cRequest;
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (req.assetBundleName, req.assetName);
            if (assetPaths.Length == 0) {
                error = "There is no asset with name \"" + req.assetName + "\" in " + req.assetBundleName;
                Debug.LogError (error);
                return false;
            }
            var assetType = req.assetType;
            bool loadAll = CacheManager.Typeof_ABAllAssets.Equals (assetType);
            object data = null;
            List<UnityEngine.Object> datas = null;
            foreach (var p in assetPaths) {
                if (loadAll) {
                    // data
                    UnityEngine.Object[] target = UnityEditor.AssetDatabase.LoadAllAssetsAtPath (p);
                    if (datas == null) {
                        datas = new List<UnityEngine.Object> ();
                        datas.AddRange (target);
                    } else {
                        datas.AddRange (target);
                    }
                    data = datas.ToArray ();
                } else {
                    UnityEngine.Object target = UnityEditor.AssetDatabase.LoadAssetAtPath (p, assetType);
                    if (target) {
                        data = target;
                        break;
                    }
                }
            }

            m_Data = data;
            cRequest.data = m_Data;

            return false;
        }

        public override void Reset () {
            base.Reset ();
        }
    }

    public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadAssetOperation {
        AsyncOperation m_Operation = null;
        public AssetBundleLoadLevelSimulationOperation () {
            RegisterEvent (_Update, _IsDone);
        }

        protected void LoadLevelSimulation () {
            string assetBundleName = cRequest.assetBundleName;
            string levelName = cRequest.assetName;
            bool isAdditive = cRequest.isAdditive;
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, levelName);
            if (levelPaths.Length == 0) {
                error = string.Format ("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
                Debug.LogError (error);
                return;
            }

            if (isAdditive)
                m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode (levelPaths[0]);
            else
                m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode (levelPaths[0]);
        }

        private bool _Update () {
            LoadLevelSimulation ();
            return false;
        }

        public bool _IsDone () {
            return m_Operation == null || m_Operation.isDone;
        }

        public override void Reset () {
            base.Reset ();
        }
    }
#endif

    public class AssetBundleLoadLevelOperation : AssetBundleLoadAssetOperation {
        protected AsyncOperation m_Request;

        public AssetBundleLoadLevelOperation () {
            RegisterEvent (_Update, _IsDone);
        }

        private bool _Update () {
            if (m_Request != null)
                return false;

            CacheData bundle = CacheManager.TryGetCache (cRequest.keyHashCode);
            if (bundle != null && bundle.isDone) {
                if (bundle.isError || !bundle.canUse) {
                    error = string.Format ("load asset form {0} error", cRequest.assetName, cRequest.key);
                    return false;
                } else {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                    if (cRequest.isAdditive)
                        m_Request = Application.LoadLevelAdditiveAsync (cRequest.assetName);
                    else
                        m_Request = Application.LoadLevelAsync (cRequest.assetName);
#else
                    m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (cRequest.assetName, cRequest.isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
#endif
                    return false;
                }
            } else
                return true;
        }

        bool _IsDone () {
            // Return if meeting downloading error.
            // m_DownloadingError might come from the dependency downloading.
            if (m_Request == null && error != null) {
                Debug.LogError (error);
                return true;
            }

            return m_Request != null && m_Request.isDone;
        }

        //il2cpp 
        public override void Reset () {
            base.Reset ();
        }

        

    }

    #endregion

}