using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Hugula.Update;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Hugula.Loader {

    public static class OperationPools<T> where T : ResourcesLoadOperation, new () {
        private static readonly ObjectPool<T> s_Pools = new ObjectPool<T> (null, m_ActionOnRelease);

        private static void m_ActionOnGet (T op) {

        }

        private static void m_ActionOnRelease (T op) {
            op.Reset ();
        }

        public static T Get () {
            return s_Pools.Get ();
        }

        public static void Release (T toRelease) {
            s_Pools.Release (toRelease);
        }
    }

    public abstract class ResourcesLoadOperation : IReleaseToPool {

        public ResourcesLoadOperation () {

        }

        protected System.Action m_OnStart;
        protected System.Func<bool> m_OnUpdate;
        protected System.Action m_OnDone;
        // internal ResourcesLoadOperation next;

        public void Start () {
            if (m_OnStart != null) m_OnStart ();
        }

        public bool Update () //virtual function is bad for il2cpp so we use Action
        {
            return m_OnUpdate ();
        }

        public void Done () {
            if (m_OnDone != null) m_OnDone ();
        }

        public virtual void Reset () {
            cRequest = null;
        }

        public abstract void ReleaseToPool ();

        internal CRequest cRequest { get; private set; }

        public void SetRequest (CRequest req) {
            this.cRequest = req;
        }

    }

    public sealed class LoadAssetBundleInternalOperation : ResourcesLoadOperation {
        public LoadAssetBundleInternalOperation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        AsyncOperation m_abRequest;
        UnityWebRequest m_UnityWebRequest;

        int frameBegin = 0;
        int timeOutFrame = 120;
        void m_Start () {
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1 AssetBundle  Request(assetName={0}) key={1},frame={2} </color>", cRequest.assetName, cRequest.key, Time.frameCount);
#endif
            if (cRequest.url.StartsWith ("http")) {
                m_UnityWebRequest = UnityWebRequest.GetAssetBundle (cRequest.url);
                m_abRequest = m_UnityWebRequest.Send ();
            } else {
                cRequest.url = ResourcesLoader.GetAssetBundleDownloadingURL (cRequest.vUrl); // set full url
                string url = CUtils.GetAndroidABLoadPath (cRequest.url);
                // var abInfo = ManifestManager.GetABInfo (cRequest.key);
                // if (abInfo != null && abInfo.size < ResourcesLoader.asyncSize) {
                //     assetBundle = AssetBundle.LoadFromFile (url);
                // } else {
                m_abRequest = AssetBundle.LoadFromFileAsync (url);
            }
            frameBegin = Time.frameCount;
        }

        bool m_Update () {
            if (m_abRequest == null) return false;

            bool isdone = !m_abRequest.isDone;

            if (!CacheManager.CheckDependenciesComplete (cRequest)) // && Time.frameCount - frameBegin >= timeOutFrame)
                return true; //wait

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1 AssetBundle update Request(assetName={0}) key={1},isdone = {2},m_abRequest={3},frame={4} </color>", cRequest.assetName, cRequest.key, isdone, m_abRequest, Time.frameCount);
#endif   
            return isdone;
        }

        void m_Done () {

            if (m_abRequest == null && m_UnityWebRequest == null) {
                cRequest.error = string.Format ("the asset bundle({0}) is wrong .    CRequest({1})", cRequest.key, cRequest.assetName);
                CacheManager.AddErrorSourceCacheDataFromReq (cRequest);
                Debug.LogError (cRequest.error);
            }

            AssetBundle assetBundle = null;

            if (m_UnityWebRequest != null)
                assetBundle = DownloadHandlerAssetBundle.GetContent (m_UnityWebRequest);
            else if (m_abRequest is AssetBundleCreateRequest)
                assetBundle = ((AssetBundleCreateRequest) m_abRequest).assetBundle;

            if (assetBundle == null) {
                cRequest.error = string.Format ("the asset bundle({0}) is not exist. CRequest({1})", cRequest.key, cRequest.assetName);
                CacheManager.AddErrorSourceCacheDataFromReq (cRequest);
                Debug.LogError (cRequest.error);
            } else {
                CacheManager.AddSourceCacheDataFromWWW (assetBundle, cRequest);
            }

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1 AssetBundle is done Request(assetName={0}) key={1},frame={2} </color>", cRequest.assetName, cRequest.key, Time.frameCount);
#endif       

            m_abRequest = null;
            m_UnityWebRequest = null;

        }
        public override void ReleaseToPool () {
            OperationPools<LoadAssetBundleInternalOperation>.Release (this);
        }
    }

    public class LoadAssetOperation : ResourcesLoadOperation {
        public LoadAssetOperation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        CacheData m_Bundle;
        AssetBundleRequest m_Request = null;

        void m_Start () {
            m_Bundle = null;
            m_Request = null;
        }

        bool m_Update () {

            if (!CacheManager.CheckDependenciesComplete (cRequest)) return true; //wait    

            if (cRequest.error != null) return false; //assetbundle is error

            if (m_Bundle == null) m_Bundle = CacheManager.TryGetCache (cRequest.keyHashCode);

            if (m_Bundle == null) return true;

            if (m_Request != null) return !m_Request.isDone;

            if (m_Bundle.isError || !m_Bundle.canUse) {
                cRequest.error = string.Format ("load asset({0}) from bundle({1})  error", cRequest.assetName, cRequest.key);
                Debug.LogError (cRequest.error);
                return false;
            } else {
                string assetName = cRequest.assetName;
                var typ = cRequest.assetType;
                bool isLoadAll = LoaderType.Typeof_ABAllAssets.Equals (typ);
                if (isLoadAll)
                    m_Request = m_Bundle.assetBundle.LoadAllAssetsAsync ();
                else
                    m_Request = m_Bundle.assetBundle.LoadAssetAsync (cRequest.assetName, typ);

#if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1.2 Asset  Request(assetName={0}) is done={1} key={2},frame={3} </color>", cRequest.assetName, m_Request.isDone, cRequest.key, Time.frameCount);
#endif     
                return !m_Request.isDone;
            }
        }

        void m_Done () {

            if (cRequest.error != null) return;

            object m_Data = null;
            bool isLoadAll = LoaderType.Typeof_ABAllAssets.Equals (cRequest.assetType);
            if (isLoadAll)
                m_Data = m_Request.allAssets;
            else
                m_Data = m_Request.asset;

#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1.2 LoadAssetOperation done  Request(assetName={0}) data={1} key={2},frame={3} </color>", cRequest.assetName, m_Data, cRequest.key, Time.frameCount);
#endif     
            if (m_Data == null) {
                cRequest.error = string.Format ("load asset({0}) from {1}  error", m_Request, cRequest.url);
                Debug.LogError (cRequest.error);
            } else {
                cRequest.data = m_Data;
                m_Bundle.SetAsset (cRequest.udAssetKey, m_Data);
            }
        }

        public override void ReleaseToPool () {
            OperationPools<LoadAssetOperation>.Release (this);
        }
    }

    public class LoadLevelOperation : ResourcesLoadOperation {

        public LoadLevelOperation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        CacheData m_Bundle;
        AsyncOperation m_Request = null;

        void m_Start () {
            m_Bundle = null;
            m_Request = null;
        }

        bool m_Update () {
            if (cRequest.error != null) return false; //assetbundle is error

            if (m_Bundle == null) m_Bundle = CacheManager.TryGetCache (cRequest.keyHashCode);

            if (m_Bundle == null) return true;

            if (m_Request != null) return !m_Request.isDone;

            if (m_Bundle.isError || !m_Bundle.canUse) {
                cRequest.error = string.Format ("load asset({0}) from bundle({1})  error", cRequest.assetName, cRequest.key);
                Debug.LogError (cRequest.error);
                return true;
            } else {

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                if (cRequest.isAdditive)
                    m_Request = Application.LoadLevelAdditiveAsync (cRequest.assetName);
                else
                    m_Request = Application.LoadLevelAsync (cRequest.assetName);
#else
                m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (cRequest.assetName, cRequest.isAdditive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : UnityEngine.SceneManagement.LoadSceneMode.Single);
#endif
                return true;
            }
        }

        void m_Done () {
            m_Bundle = null;
            m_Request = null;
        }

        public override void ReleaseToPool () {
            OperationPools<LoadLevelOperation>.Release (this);
        }

    }

    #region load http
    public class HttpLoadOperation : ResourcesLoadOperation {
        public string error;

        public override void ReleaseToPool () {
            // OperationPools<HttpLoadOperation>.Release (this);
        }
    }

    public sealed class UnityWebRequestOperation : HttpLoadOperation {

        public UnityWebRequestOperation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        UnityWebRequest m_webrequest = null;
        AsyncOperation m_asyncOperation = null;

        void m_Start () {

            var type = cRequest.assetType;
            var userData = cRequest.uploadData;

            if (LoaderType.Typeof_AssetBundle.Equals (type)) {
                m_webrequest = UnityWebRequest.GetAssetBundle (cRequest.url);
            }
            if (LoaderType.Typeof_Texture2D.Equals (type)) {
#if UNITY_2017
                m_webrequest = UnityWebRequestTexture.GetTexture (cRequest.url);
#else
                m_webrequest = UnityWebRequest.GetTexture (cRequest.url);
#endif
            } else if (userData is WWWForm) {
                m_webrequest = UnityWebRequest.Post (cRequest.url, (WWWForm) userData);
            } else if (userData is string) {
                var bytes = LuaHelper.GetBytes (userData.ToString ());
                m_webrequest = UnityWebRequest.Put (cRequest.url, bytes);
            } else if (userData is System.Array) {
                m_webrequest = UnityWebRequest.Put (cRequest.url, (byte[]) userData);
            } else
                m_webrequest = UnityWebRequest.Get (cRequest.url);

            System.Net.WebHeaderCollection headers = cRequest.webHeader;

            if (headers != null) {
                foreach (var k in headers.AllKeys)
                    m_webrequest.SetRequestHeader (k, headers.Get (k));
            }

            m_asyncOperation = m_webrequest.Send ();
        }

        bool m_Update () {
            return !m_asyncOperation.isDone;
        }

        void m_Done () {
            var type = cRequest.assetType;
#if UNITY_2017
            if (m_webrequest.isNetworkError)
#else
                if (m_webrequest.isError)
#endif
            {
                var error = string.Format ("url:{0},erro:{1}", cRequest.url, m_webrequest.error);
                cRequest.error = error;
                Debug.LogError (error);
            } else if (!(m_webrequest.responseCode == 200 || m_webrequest.responseCode == 0)) {
                var error = string.Format ("response error code = {0},url={1}", m_webrequest.responseCode, cRequest.url); // m_webrequest.error;
                cRequest.error = error;
                Debug.LogError (error);
            } else {

                object m_Data = null;
                if (LoaderType.Typeof_Bytes.Equals (type)) {
                    m_Data = m_webrequest.downloadHandler.data;
                } else if (LoaderType.Typeof_Texture2D.Equals (type)) {
                    m_Data = DownloadHandlerTexture.GetContent (m_webrequest);
                } else if (LoaderType.Typeof_AssetBundle.Equals (type)) {
                    m_Data = DownloadHandlerAssetBundle.GetContent (m_webrequest);
                } else
                    m_Data = DownloadHandlerBuffer.GetContent (m_webrequest);

                cRequest.data = m_Data;
            }

            m_webrequest.Dispose ();
            m_webrequest = null;
            m_asyncOperation = null;
        }

        public override void ReleaseToPool () {
            OperationPools<UnityWebRequestOperation>.Release (this);
        }
    }

    public sealed class HttpWebRequestOperation : HttpLoadOperation {

        public HttpWebRequestOperation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        HttpWebRequest m_webrequest;
        WebResponse m_webresponse;

        Thread async_thread = null;

        object m_Data;

        bool httpIsDone;

        void DownloadFileCore (HttpWebRequest m_webrequest, System.Type typ = null) {

            try {
                var type = cRequest.assetType;
                var userData = cRequest.uploadData;

                byte[] uploadData = null;
                if (userData is string) {
                    uploadData = LuaHelper.GetBytes (userData.ToString ());
                    m_webrequest.Method = "POST";
                } else if (userData is System.Array) {
                    uploadData = (byte[])userData;
                    m_webrequest.Method = "POST";
                } else
                    m_webrequest.Method = "GET";

                if(m_webrequest.Method == "POST" && uploadData!=null)
                {
                    m_webrequest.ContentLength = uploadData.Length;
                    using(var requestStream = m_webrequest.GetRequestStream())
                    {
                        requestStream.Write(uploadData,0,uploadData.Length);
                    }
                }

                WebResponse webResponse = m_webrequest.GetResponse (); //this.GetWebResponse (webRequest);
                Stream responseStream = webResponse.GetResponseStream ();

#if !HUGULA_RELEASE
                if (webResponse.Headers != null) {
                    foreach (var k in webResponse.Headers.AllKeys) {
                        Debug.LogFormat ("{0}={1}", k, webResponse.Headers.Get (k));
                    }
                }
#endif          

                if (webResponse.ContentType.ToLower ().Equals ("text/plain") || LoaderType.Typeof_String.Equals (typ)) {
                    StreamReader sr = new StreamReader (responseStream, System.Text.Encoding.UTF8, true);
                    m_Data = sr.ReadToEnd ();
                    sr.Close ();
                } else {
                    int num = (int) webResponse.ContentLength;
                    byte[] array = new byte[num];
                    while (responseStream.Read (array, 0, num) != 0) { }
                    m_Data = array;
                    responseStream.Close ();
                }

                webResponse.Close ();

            } catch (ThreadInterruptedException) {
                this.error = string.Format ("ThreadInterruptedException  url:{0}  ", cRequest.url);
            } finally {
                httpIsDone = true;
                if (m_webrequest != null) {
                    m_webrequest.Abort ();
                }
            }

        }
        void m_Start () {
            m_Data = null;
            httpIsDone = false;
            error = null;
            async_thread = new Thread (delegate () {
                m_webrequest = LoaderHelper.SetupRequest (cRequest);
                DownloadFileCore (m_webrequest, cRequest.assetType);
            });

            async_thread.Start ();
        }

        bool m_Update () {
            return !httpIsDone;
        }

        void m_Done () {

            if (m_webrequest == null) {
                error = string.Format ("url:{0}, webrequest is null ", cRequest.url);
                cRequest.error = error;
                Debug.LogError (error);
            }

            if (m_Data == null) {
                error = string.Format ("url:{0} data is null", cRequest.url);
                cRequest.error = error;
                Debug.LogError (error);
            } else {
                cRequest.data = m_Data;
            }

            if(async_thread!=null) async_thread.Abort();
            async_thread = null;
        }

        public override void ReleaseToPool () {
            OperationPools<HttpWebRequestOperation>.Release (this);
        }
    }

    public class HttpDnsResolve : HttpLoadOperation {

        private ResourcesLoadOperation originalOperation;

        public void SetOriginalOperation (ResourcesLoadOperation originalOperation) {
            this.originalOperation = originalOperation;
        }

        public HttpDnsResolve () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        void m_Start () { }

        bool m_Update () {
            string url = HttpDns.GetUrl (cRequest.url); // get dns ip
            if (string.IsNullOrEmpty (url)) {
                return true; //wait for ip 
            }

            if (url != cRequest.url) {

                var headers = cRequest.webHeader;
                if (headers == null) {
                    headers = new WebHeaderCollection ();
                    cRequest.webHeader = headers;
                }

                if (string.IsNullOrEmpty (headers.Get ("host"))) {
                    headers.Add ("host", new System.Uri (cRequest.url).Host);
                }
                ResourcesLoader.UnityWebRequest (cRequest);
            } else if (originalOperation != null) {
                Debug.LogFormat (" dns resolve fail request url {0}   ", url);
                ResourcesLoader.ProcessFinishedOperation (originalOperation);
            } else {
                Debug.LogFormat ("dns resolve fail , complete request url {0} ", url);

                cRequest.DispatchEnd ();

                if (cRequest.group != null) cRequest.group.Complete (cRequest, true);

                cRequest.ReleaseToPool ();
            }

            return false;
        }

        void m_Done () {

        }

        public override void ReleaseToPool () {
            OperationPools<HttpDnsResolve>.Release (this);
        }
    }

    #endregion

    #region Simulate
#if UNITY_EDITOR
    public class LoadAssetOperationSimulation : ResourcesLoadOperation {
        public LoadAssetOperationSimulation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        void m_Start () {
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1.2 LoadAssetOperationSimulation  Request(assetName={0}) key={1} </color>", cRequest.assetName, cRequest.key);
#endif

            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (cRequest.key, cRequest.assetName);
            if (assetPaths.Length == 0) {
                cRequest.error = "There is no asset with name \"" + cRequest.assetName + "\" in " + cRequest.key;
                Debug.LogError (cRequest.error);
            }
            var assetType = cRequest.assetType;
            bool loadAll = LoaderType.Typeof_ABAllAssets.Equals (assetType);
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

            cRequest.data = data;
        }

        bool m_Update () {
            return false;
        }

        void m_Done () {

        }

        public override void ReleaseToPool () {
            OperationPools<LoadAssetOperationSimulation>.Release (this);
        }
    }

    public class LoadLevelOperationSimulation : ResourcesLoadOperation {
        public LoadLevelOperationSimulation () {
            m_OnStart = m_Start;
            m_OnUpdate = m_Update;
            m_OnDone = m_Done;
        }

        AsyncOperation m_Operation = null;

        void m_Start () {
#if HUGULA_LOADER_DEBUG
            HugulaDebug.FilterLogFormat (cRequest.key, " <color=#15A0A1> 1.2 LoadLevelOperationSimulation  Request(assetName={0}) key={1} </color>", cRequest.assetName, cRequest.key);
#endif
            string assetBundleName = cRequest.key;
            string levelName = cRequest.assetName;
            bool isAdditive = cRequest.isAdditive;
            string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, levelName);
            if (levelPaths.Length == 0) {
                cRequest.error = string.Format ("There is no scene with name \"" + levelName + "\" in " + assetBundleName);
                Debug.LogError (cRequest.error);
            }
            if (isAdditive)
                m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode (levelPaths[0]);
            else
                m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode (levelPaths[0]);

        }

        bool m_Update () {
            return !m_Operation.isDone;
        }

        void m_Done () {

        }

        public override void ReleaseToPool () {
            OperationPools<LoadLevelOperationSimulation>.Release (this);
        }
    }

#endif

    #endregion

}