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

            Dictionary<string,string> headers = null;
            if(cRequest.head != null)
            {
                headers = new Dictionary<string,string>();
                foreach(var k in cRequest.head.AllKeys)
                {
                    headers[k] = cRequest.head.Get(k);
                }
            }
            

            var userdata = cRequest.userData;
            string url = CUtils.CheckWWWUrl (cRequest.url);

            if (userdata is WWWForm) {
                m_webrequest = new WWW (url, (WWWForm) userdata);
            } else if (userdata is string) {
                var bytes = LuaHelper.GetBytes (userdata.ToString ());
                if (headers != null)
                    m_webrequest = new WWW (url, (byte[]) userdata, headers);
                else
                    m_webrequest = new WWW (url, bytes);
            } else if (userdata is System.Array) {
                if (headers != null)
                    m_webrequest = new WWW (url, (byte[]) userdata, headers);
                else
                    m_webrequest = new WWW (url, (byte[]) userdata);
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
            if (LoaderType.Typeof_AudioClip.Equals (type)) {
#if UNITY_2017
                m_Data = WWWAudioExtensions.GetAudioClip (m_webrequest);
#else
				m_Data = m_webrequest.GetAudioClip (false);
#endif
            } else if (LoaderType.Typeof_Texture2D.Equals (type)) {
                if (!string.IsNullOrEmpty (cRequest.assetName) && cRequest.assetName.Equals ("textureNonReadable"))
                    m_Data = m_webrequest.textureNonReadable;
                else
                    m_Data = m_webrequest.texture;
            } else if (LoaderType.Typeof_AssetBundle.Equals (type)) {
                m_Data = m_webrequest.assetBundle;
            } else if (LoaderType.Typeof_Bytes.Equals (type)) {
                m_Data = m_webrequest.bytes;
            } else
                m_Data = m_webrequest.text;

            // UriGroup.CheckWWWComplete (cRequest, m_webrequest);

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

    public sealed class HttpWebRequestOperation : HttpLoadOperation {
        private HttpWebRequest m_webrequest;
        private WebResponse m_webresponse;

        private Thread async_thread = null;

        private bool httpIsDone = false;

        public HttpWebRequestOperation () {
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

        private void DownloadFileCore (HttpWebRequest m_webrequest, System.Type typ = null) {
            HttpWebRequest webRequest = m_webrequest;
            try {
                m_webrequest.Method = "GET";
                WebResponse webResponse = m_webrequest.GetResponse (); //this.GetWebResponse (webRequest);
                Stream responseStream = webResponse.GetResponseStream ();

                #if !HUGULA_RELEASE
                if(webResponse.Headers!=null)
                {
                    foreach(var k in webResponse.Headers.AllKeys)
                    {
                        Debug.LogFormat("{0}={1}",k,webResponse.Headers.Get(k));
                    }
                }
                #endif

                if(webResponse.ContentType.ToLower().Equals("text/plain") || LoaderType.Typeof_String.Equals(typ))
                {
                    StreamReader sr = new StreamReader(responseStream,System.Text.Encoding.UTF8,true);
                    m_Data = sr.ReadToEnd();
                    cRequest.data = m_Data;
                }else
                {
                    int num = (int) webResponse.ContentLength;
                    byte[] array = new byte[num];
                    while (responseStream.Read (array, 0, num)!= 0) {
                    }
                    m_Data = array;
                    cRequest.data = array;
                }

                webResponse.Close ();

            } catch (ThreadInterruptedException) {
                this.error = string.Format ("ThreadInterruptedException  url:{0}  ", cRequest.url);
            } finally {
                httpIsDone = true;
                if (webRequest != null) {
                    webRequest.Abort ();
                }
            }

        }

        private void _BeginDownload () {
            m_Data = null;
            httpIsDone = false;

            async_thread = new Thread (delegate()
            {
                m_webrequest = LoaderHelper.SetupRequest (cRequest);
                DownloadFileCore(m_webrequest,cRequest.assetType);
            }) ;

            async_thread.Start();
        }

        private void FinishDownload () {
            if (m_webrequest == null) {
                error = string.Format ("webrequest is null , {0} ", cRequest.key);
                Debug.LogErrorFormat ("url:{0},erro:{1}", cRequest.url, error);
                return;
            }

            if (!string.IsNullOrEmpty (error)) {
                Debug.LogErrorFormat ("url:{0},erro:{1}", cRequest.url, error);
                return;
            }
        }

        public override void Reset () {
            base.Reset ();
            m_Data = null;
        }

        private bool downloadIsDone {
            get {
                return httpIsDone;
                // return m_webrequest == null || m_webrequest.isDone;
            }

        }

        public override void ReleaseToPool () {
            if (pool)
                Release (this);
        }

        #region pool
        static ObjectPool<HttpWebRequestOperation> webOperationPool = new ObjectPool<HttpWebRequestOperation> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (HttpWebRequestOperation op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (HttpWebRequestOperation op) {
            op.Reset ();
            op.httpIsDone = false;
        }

        public static HttpWebRequestOperation Get () {
            return webOperationPool.Get ();
        }

        public static void Release (HttpWebRequestOperation toRelease) {
            webOperationPool.Release (toRelease);
        }

        #endregion
    }

    public sealed class UnityWebRequestOperation : HttpLoadOperation {
        private UnityWebRequest m_webrequest;
        private AsyncOperation m_asyncOperation;

        public UnityWebRequestOperation () {
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
            var userData = cRequest.userData;

            if (LoaderType.Typeof_AudioClip.Equals (type)) {
                AudioType au = AudioType.MOD;
                if (userData is AudioType) au = (AudioType) userData;
#if UNITY_2017
                m_webrequest = UnityWebRequestMultimedia.GetAudioClip (cRequest.url, au);
#else
                m_webrequest = UnityWebRequest.GetAudioClip (cRequest.url, au);
#endif
            } else if (LoaderType.Typeof_AssetBundle.Equals (type)) {
                m_webrequest = UnityWebRequest.GetAssetBundle (cRequest.url);
            } else if (LoaderType.Typeof_Texture2D.Equals (type)) {
#if UNITY_2017
                if (userData is bool)
                    m_webrequest = UnityWebRequestTexture.GetTexture (cRequest.url, (bool) userData);
                else
                    m_webrequest = UnityWebRequestTexture.GetTexture (cRequest.url);
#else
                if (userData is bool)
                    m_webrequest = UnityWebRequest.GetTexture (cRequest.url, (bool) userData);
                else
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

            WebHeaderCollection headers = (WebHeaderCollection)cRequest.head;
            
            if (headers != null) {
                foreach (var k in headers.AllKeys)
                    m_webrequest.SetRequestHeader (k, headers.Get(k));
            }

            m_asyncOperation = m_webrequest.Send ();

        }

        private void FinishDownload () {
            if (m_webrequest == null) {
                error = string.Format ("webrequest is null ,key={0},url={1} ", cRequest.key, cRequest.url);
                return;
            }
            var responseHeaders = m_webrequest.GetResponseHeaders ();
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
            if (LoaderType.Typeof_AudioClip.Equals (type)) {
                m_Data = DownloadHandlerAudioClip.GetContent (m_webrequest);
            } else if (LoaderType.Typeof_Texture2D.Equals (type)) {
                m_Data = DownloadHandlerTexture.GetContent (m_webrequest);
            } else if (LoaderType.Typeof_Bytes.Equals (type)) {
                m_Data = m_webrequest.downloadHandler.data;
            } else if (LoaderType.Typeof_AssetBundle.Equals (type)) {
                m_Data = DownloadHandlerAssetBundle.GetContent (m_webrequest);
            } else
                m_Data = DownloadHandlerBuffer.GetContent (m_webrequest);

            // if(!LoaderType.Typeof_AssetBundle.Equals (type))
            //     UriGroup.CheckWWWComplete (cRequest, m_webrequest);

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
        static ObjectPool<UnityWebRequestOperation> webOperationPool = new ObjectPool<UnityWebRequestOperation> (m_ActionOnGet, m_ActionOnRelease);

        private static void m_ActionOnGet (UnityWebRequestOperation op) {
            op.pool = true;
        }

        private static void m_ActionOnRelease (UnityWebRequestOperation op) {
            op.Reset ();
        }

        public static UnityWebRequestOperation Get () {
            return webOperationPool.Get ();
        }

        public static void Release (UnityWebRequestOperation toRelease) {
            webOperationPool.Release (toRelease);
        }

        #endregion
    }

    #endregion

    #region  http dns operation
    public sealed class HttpDnsResolve : ResourcesLoadOperation {
        private ResourcesLoadOperation originalOperation;
        public HttpDnsResolve () {
            RegisterEvent (_Update, _IsDone);
        }

        public void SetOriginalOperation (ResourcesLoadOperation originalOperation) {
            this.originalOperation = originalOperation;
        }

        bool _Update () {
            string url = HttpDns.GetUrl (cRequest.url); // get dsn ip
            if (string.IsNullOrEmpty (url)) {
                return true; //wait for ip 
            }

            if (url != cRequest.url) {

                var headers = cRequest.head;
                if (headers == null) {
                    headers = new WebHeaderCollection();
                    cRequest.head = headers;
                }

                if (string.IsNullOrEmpty(headers.Get("host"))) {
                    headers.Add ("host", new System.Uri (cRequest.url).Host);
                }
                // Debug.LogFormat("request ip {0}  override host {1} ",url,cRequest.overrideHost);
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

        bool _IsDone () {
            string url = HttpDns.GetUrl (cRequest.url);
            return !string.IsNullOrEmpty (url);
        }

        public override void Reset () {
            base.Reset ();
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

}