// Copyright (c) 2016 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Hugula.Collections;
using Hugula.Utils;
using UnityEngine;

namespace System.Net {

    public class DownloadingProgressChangedEventArgs : System.ComponentModel.ProgressChangedEventArgs {
        internal long received;

        internal long total;

        internal int read;

        public long BytesReceived {
            get {
                return this.received;
            }
        }

        public long TotalBytesToReceive {
            get {
                return this.total;
            }
        }

        public int BytesRead {
            get {
                return this.read;
            }
        }

        public DownloadingProgressChangedEventArgs (long bytesReceived, long totalBytesToReceive, object userState) : base ((totalBytesToReceive == -1L) ? 0 : ((int) (bytesReceived * 100L / totalBytesToReceive)), userState) {
            this.received = bytesReceived;
            this.total = totalBytesToReceive;
        }

        public DownloadingProgressChangedEventArgs (long bytesReceived, int read, long totalBytesToReceive, object userState) : base ((totalBytesToReceive == -1L) ? 0 : ((int) (bytesReceived * 100L / totalBytesToReceive)), userState) {
            this.received = bytesReceived;
            this.read = read;
            this.total = totalBytesToReceive;
        }

    }

    public class CacheFileException : Exception {
        public CacheFileException (string fileName, string message) : base (message) {
            this.fileName = fileName;
        }
        public string fileName;
    }

    //断点加载
    public class BreakpointWebClient : IDisposable {

        public class BlockThead {
            public Thread async_thread;

            public object[] parameter;

            public int tryTimes = 0;

            public bool clearCache = false;

            public string fileName;

            public int index;

            public void Start () {
                if (async_thread != null) {
                    tryTimes++;
                    async_thread.Start (parameter);
                }
            }

            public void Interrupt () {
                if (this.async_thread != null) {
                    Thread thread = this.async_thread;
                    thread.Interrupt ();
                }
            }

            public void Dispose () {
                Interrupt ();
                parameter = null;
            }
        }

        protected class BlockTheadComparer : IComparer {
            public int Compare (object x, object y) {
                return ((BlockThead) x).index - ((BlockThead) y).index;
            }
        }

        internal DownloadingProgressChangedEventArgs progressChangedEventArgs;

        private Thread async_thread;
        private ArrayList async_children_thread;
        private ArrayList async_children_finished;
        private ArrayList async_children_error;
        private bool is_busy;
        private bool async;
        private string fileName;
        private WebHeaderCollection headers;
        private ICredentials credentials;
        private WebHeaderCollection responseHeaders;

        public BreakpointWebClient () {
            async_children_thread = ArrayList.Synchronized (new ArrayList ());
            async_children_finished = ArrayList.Synchronized (new ArrayList ());
            async_children_error = ArrayList.Synchronized (new ArrayList ());
            this.BlockDownloadFileCompleted = OnBlockDownloadFileCompletedHandler;
            this.BlockDownloadProgressChanged = OnBlockDownloadingProgressChangedHander;
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;//同时http请求数量
        }

        public WebHeaderCollection ResponseHeaders {
            get {
                return this.responseHeaders;
            }
        }

        public bool IsBusy {
            get {
                return this.is_busy;
            }
        }

        public ICredentials Credentials {
            get {
                return this.credentials;
            }
            set {
                this.credentials = value;
            }
        }

        private void CheckBusy () {
            if (this.IsBusy) {
                throw new NotSupportedException ("WebClient does not support conccurent I/O operations.");
            }
        }

        private void SetBusy () {
            lock (this) {
                this.CheckBusy ();
                this.is_busy = true;
            }
        }

        private WebRequest SetupRequest (System.Uri uri) {
            WebRequest webRequest = this.GetWebRequest (uri);
            webRequest.Credentials = this.credentials;
            if (this.headers != null && this.headers.Count != 0 && webRequest is HttpWebRequest) {
                HttpWebRequest httpWebRequest = (HttpWebRequest) webRequest;
                string text = this.headers["Expect"];
                string text2 = this.headers["Content-Type"];
                string text3 = this.headers["Accept"];
                string text4 = this.headers["Connection"];
                string text5 = this.headers["User-Agent"];
                string text6 = this.headers["Referer"];
                this.headers.Remove ("Expect");
                this.headers.Remove ("Content-Type");
                this.headers.Remove ("Accept");
                this.headers.Remove ("Connection");
                this.headers.Remove ("Referer");
                this.headers.Remove ("User-Agent");
                webRequest.Headers = this.headers;
                if (text != null && text.Length > 0) {
                    httpWebRequest.Expect = text;
                }
                if (text3 != null && text3.Length > 0) {
                    httpWebRequest.Accept = text3;
                }
                if (text2 != null && text2.Length > 0) {
                    httpWebRequest.ContentType = text2;
                }
                if (text4 != null && text4.Length > 0) {
                    httpWebRequest.Connection = text4;
                }
                if (text5 != null && text5.Length > 0) {
                    httpWebRequest.UserAgent = text5;
                }
                if (text6 != null && text6.Length > 0) {
                    httpWebRequest.Referer = text6;
                }
            }
            this.responseHeaders = null;
            return webRequest;
        }

        public void CancelAsync () {
            lock (this) {
                if (this.async_thread != null) {
                    Thread thread = this.async_thread;
                    this.CompleteAsync ();
                    thread.Interrupt ();
                }

                if (this.async_children_thread.Count > 0) {
                    for (int i = 0; i < this.async_children_thread.Count; i++) {
                        ((BlockThead) async_children_thread[i]).Dispose ();
                    }
                    async_children_thread.Clear ();
                }
            }
        }

        /// <summary>
        /// 完成异步操作
        /// </summary>
        /// <returns></returns>
        private void CompleteAsync () {
            lock (this) {
                this.is_busy = false;
                this.async_thread = null;
            }
        }

        private void CheckFileDirectory(string filePath)
        {
            FileInfo finfo = new FileInfo(filePath);
            if (!finfo.Directory.Exists) finfo.Directory.Create();
        }

        /// <summary>
        /// 记录etag信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        private void RecordETag (string fileName, string etag) {
            string tmp = GetETagfiedFileName (fileName);
            CheckFileDirectory(tmp);
            using (FileStream fs = new FileStream (tmp, FileMode.Create)) {
                var bts = System.Text.Encoding.UTF8.GetBytes (etag);
                fs.Write (bts, 0, bts.Length);
            }
        }

        /// <summary>
        /// 获取etag信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ReadETag (string fileName) {
            string eFileName = GetETagfiedFileName (fileName);
            if (!File.Exists (eFileName))
                return null;
            else {
                string etag = File.ReadAllText (eFileName);
                return etag;
            }
        }

        /// <summary>
        /// 检查etag信息是否改变
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        private bool CheckCacheModified (string fileName, string etag) {
            string localEtag = ReadETag (fileName);
            return localEtag != etag;
        }

        private string GetETagfiedFileName (string fileName) {
            return fileName + ".etag";
        }
        private string GetTmpFileName (string fileName) {
            return fileName + ".tmp";
        }

        public long GetContentLength (System.Uri address, out Exception ex) {
            WebRequest webRequest = null;
            WebResponse webResponse = null;
            ex = null;
            try {
                webRequest = this.SetupRequest (address);
                webResponse = this.GetWebResponse (webRequest);
                long num = webResponse.ContentLength;
                return num;
            } catch (Exception e) {
                ex = e;
            } finally {
                if (webRequest != null) webRequest.Abort ();
                if (webResponse != null) webResponse.Close ();
            }
            return -1;
        }

        public void DownloadFileMultiAsync (System.Uri address, string fileName, int maxThreading = 2) {
            DownloadFileMultiAsync (address, fileName, null, maxThreading);
        }

        public void DownloadFileMultiAsync (System.Uri address, string fileName, object userToken, int maxThreading = 3) {
            if (address == null) {
                throw new ArgumentNullException ("address");
            }
            if (fileName == null) {
                throw new ArgumentNullException ("fileName");
            }

            CheckFileDirectory(fileName);

            System.Exception ex = null;
            long len = GetContentLength (address, out ex);
            if (ex != null) {
                this.OnBlockDownloadFileCompleted (null, new System.ComponentModel.AsyncCompletedEventArgs (ex, false, userToken));
                return;
            }

            progressChangedEventArgs = new DownloadingProgressChangedEventArgs (0, len, userToken);
            bool delCache = true;
            if (this.responseHeaders != null) {
                //check etag
                string svrETag = responseHeaders["ETag"];
                if(string.IsNullOrEmpty(svrETag))
                {
                    Debug.LogWarningFormat("url {0} Etag is Null",fileName);
                    svrETag =  responseHeaders["Last-Modified"];
                }

                if(string.IsNullOrEmpty(svrETag))
                {
                    Debug.LogWarningFormat("url {0} Last-Modified is Null",fileName);
                    svrETag = System.DateTime.Now.ToString();
                } 

                delCache = CheckCacheModified (fileName, svrETag);
                RecordETag (fileName, svrETag);

                //debug
                // Debug.LogFormat("clear cache {0} ={1}",delCache,fileName);
                // var sb = new System.Text.StringBuilder ();
                // foreach (var kv in responseHeaders.AllKeys) {
                //     sb.AppendFormat ("{0}={1}\r\n", kv, responseHeaders[kv]); //Last-Modified=Tue, 21 Nov 2017 02:03:05 GMT
                //     // Debug.LogFormat("{0}={1}",kv,responseHeaders[kv]);//ETag="5a138959-c68171c"
                // }
                // Debug.Log (sb.ToString ());
            }

            lock (this) {
                this.SetBusy ();
                this.fileName = fileName;
                this.async = true;
                int block = (int) len / maxThreading;

                int from = 0;
                int to = -1;
                string childrenFileName = null;
                this.async_children_thread.Clear ();
                this.async_children_finished.Clear ();
                this.async_children_error.Clear ();
                for (int i = 0; i < maxThreading; i++) {
                    from = to + 1;
                    if (i == maxThreading - 1)
                        to = (int) len - 1;
                    else
                        to = from + block;

                    childrenFileName = string.Format ("{0}.{1}", fileName, i);

                    var blockThead = new BlockThead ();

                    object[] parameter = new object[] {
                        address,
                        childrenFileName,
                        userToken,
                        new int[] { from, to },
                        blockThead
                    };

                    blockThead.index = i;
                    blockThead.fileName = childrenFileName;
                    blockThead.clearCache = delCache;
                    blockThead.async_thread = new Thread (
                        delegate (object state) {
                            object[] array = (object[]) state;
                            try {
                                string fname = (string) array[1];
                                int[] fromAndTo = (int[]) array[3];
                                bool clearCache = ((BlockThead) array[4]).clearCache;
                                if (clearCache && File.Exists (GetTmpFileName (fname))) File.Delete (GetTmpFileName (fname));
                                this.DownloadBlockFileCore ((System.Uri) array[0], fname, array[2], fromAndTo[0], fromAndTo[1]);
                                this.OnBlockDownloadFileCompleted (blockThead, new System.ComponentModel.AsyncCompletedEventArgs (null, false, array[2]));
                            } catch (ThreadInterruptedException e) {
                                this.OnBlockDownloadFileCompleted (blockThead, new System.ComponentModel.AsyncCompletedEventArgs (e, true, array[2]));
                            } catch (Exception error) {
                                this.OnBlockDownloadFileCompleted (blockThead, new System.ComponentModel.AsyncCompletedEventArgs (error, false, array[2]));
                            }
                        });
                    blockThead.parameter = parameter;

                    this.async_children_thread.Add (blockThead);

                    blockThead.Start ();
                }
            }

        }

        public void DownloadFileAsync (System.Uri address, string fileName) {
            DownloadFileAsync (address, fileName, null);
        }

        public void DownloadFileAsync (System.Uri address, string fileName, object userToken) {
            if (address == null) {
                throw new ArgumentNullException ("address");
            }
            if (fileName == null) {
                throw new ArgumentNullException ("fileName");
            }

            CheckFileDirectory(fileName);

            lock (this) {
                this.SetBusy ();
                this.fileName = fileName;
                this.async = true;
                this.async_thread = new Thread (delegate (object state) {
                    object[] array = (object[]) state;
                    try {
                        string fname = (string) array[1];
                        this.DownloadFileCore ((System.Uri) array[0], fname, array[2]);
                        //change name
                        if (File.Exists (fname)) File.Delete (fname);
                        File.Move (GetTmpFileName (fname), fname);
                        this.OnDownloadFileCompleted (new System.ComponentModel.AsyncCompletedEventArgs (null, false, array[2]));
                    } catch (ThreadInterruptedException e) {
                        this.OnDownloadFileCompleted (new System.ComponentModel.AsyncCompletedEventArgs (e, true, array[2]));
                    } catch (Exception error) {
                        this.OnDownloadFileCompleted (new System.ComponentModel.AsyncCompletedEventArgs (error, false, array[2]));
                    }
                });
                object[] parameter = new object[] {
                    address,
                    fileName,
                    userToken
                };
                this.async_thread.Start (parameter);
            }
        }

        private void DownloadBlockFileCore (System.Uri address, string fileName, object userToken, int from, int to) {
            HttpWebRequest webRequest = null;
            string tmp = GetTmpFileName (fileName); // + ".tmp";
            using (FileStream fileStream = new FileStream (tmp, FileMode.OpenOrCreate)) {
                try {
                    long startPos = fileStream.Length;
                    int fileLen = to - from + 1;
                    if (fileLen == startPos) {
                        if (this.async) {
                            this.OnBlockDownloadingProgressChanged (new DownloadingProgressChangedEventArgs (startPos, (int) startPos, fileLen, userToken));
                        }
                        return;
                    } else if (fileLen < startPos) {
                        throw new CacheFileException (tmp, "cache file is error!:" + fileName);
                    } else if (startPos + from >= to) {
                        throw new CacheFileException (tmp, "file block is error!:" + fileName);
                    }

                    webRequest = (HttpWebRequest) this.SetupRequest (address);
                    webRequest.AddRange ((int) startPos + from, to);
                    fileStream.Seek (startPos, SeekOrigin.Current);
                    WebResponse webResponse = this.GetWebResponse (webRequest);
                    Stream responseStream = webResponse.GetResponseStream ();

                    int num = (int) webResponse.ContentLength;
                    int num2 = (num > -1 && num <= 16384) ? num : 16384; //16KB
                    byte[] array = new byte[num2];
                    long num3 = 0L;
                    int num4;
                    int num1 = num + (int) startPos;

                    if (startPos > 0 && async) this.OnBlockDownloadingProgressChanged (new DownloadingProgressChangedEventArgs (startPos, (int) startPos, num1, userToken));

                    while ((num4 = responseStream.Read (array, 0, num2)) != 0) {
                        num3 += (long) num4;
                        fileStream.Write (array, 0, num4);
                        if (this.async) {
                            this.OnBlockDownloadingProgressChanged (new DownloadingProgressChangedEventArgs (num4, num4, num1, userToken));
                        }
                    }

                    webRequest.Abort ();

                    if (num3 + startPos < num1) {
                        throw new Exception (string.Format ("download fail uri:{0} ", address));
                    } else if (num3 + startPos > num1) {
                        throw new CacheFileException (tmp, string.Format ("file is wrong!uri:{0}  ", address));
                    }

                } catch (ThreadInterruptedException) {
                    if (webRequest != null) {
                        webRequest.Abort ();
                    }
                    throw;
                }
            }
        }

        private void DownloadFileCore (System.Uri address, string fileName, object userToken) {
            HttpWebRequest webRequest = null;
            string tmp = GetTmpFileName (fileName); // + ".tmp";
            using (FileStream fileStream = new FileStream (tmp, FileMode.Create)) {
                try {
                    webRequest = (HttpWebRequest) this.SetupRequest (address);
                    WebResponse webResponse = this.GetWebResponse (webRequest);
                    Stream responseStream = webResponse.GetResponseStream ();
                    int num = (int) webResponse.ContentLength;
                    int num2 = (num > -1 && num <= 32768) ? num : 32768; //32KB
                    byte[] array = new byte[num2];
                    long num3 = 0L;
                    int num4;
                    while ((num4 = responseStream.Read (array, 0, num2)) != 0) {
                        num3 += (long) num4;
                        fileStream.Write (array, 0, num4);
                        if (this.async) {
                            this.OnDownloadingProgressChanged (new DownloadingProgressChangedEventArgs (num3, num4, num, userToken));
                        }
                    }

                    webRequest.Abort ();

                    if (num3 < num) {
                        throw new Exception (string.Format ("download fail uri:{0} ", address));
                    } else if (num3 > num) {
                        throw new CacheFileException (tmp, string.Format ("file is wrong!uri:{0}  ", address));
                    }

                } catch (ThreadInterruptedException) {
                    if (webRequest != null) {
                        webRequest.Abort ();
                    }
                    throw;
                }
            }
        }

        private void OnBlockDownloadFileCompletedHandler (object sender, System.ComponentModel.AsyncCompletedEventArgs args) {
            async_children_thread.Remove (sender);
            async_children_finished.Add (sender);
            if (args.Error != null){
                Debug.LogWarning (args.Error);
                if (args.Error is CacheFileException) {
                    string tmpName = ((CacheFileException) args.Error).fileName;
                    File.Delete (tmpName);
                    Debug.Log ("delete error file " + tmpName);
                }
                async_children_error.Add (args);
            }

            if (async_children_thread.Count == 0 && async_children_error.Count == 0 && async_children_finished.Count > 0) {
                async_children_finished.Sort (new BlockTheadComparer ());
                BlockThead block = null; // (BlockThead) async_children_finished[0]; //find first
                string tmpFileName = string.Empty;
                int num2 = 32768; //32KB
                byte[] array = new byte[num2];
                FileStream tmpFs;
                int num4 = 0;
                using (FileStream fileStream = new FileStream (fileName, FileMode.Create)) {
                    for (int i = 0; i < async_children_finished.Count; i++) {
                        block = (BlockThead) async_children_finished[i];
                        tmpFileName = GetTmpFileName (block.fileName);
                        using (tmpFs = new FileStream (tmpFileName, FileMode.Open)) {
                            while ((num4 = tmpFs.Read (array, 0, num2)) != 0) {
                                fileStream.Write (array, 0, num4);
                            }
                        }
                        File.Delete (tmpFileName);
                        Debug.LogFormat ("delete file {0}", tmpFileName);
                    }
                }
                //delete etag 
                string etagFileName = GetETagfiedFileName (fileName);
                File.Delete (etagFileName);
                this.OnDownloadFileCompleted (args);
            } else if (async_children_thread.Count == 0 && async_children_error.Count > 0) {
                this.OnDownloadFileCompleted ((System.ComponentModel.AsyncCompletedEventArgs) async_children_error[0]);
            }
        }

        private void OnBlockDownloadingProgressChangedHander (object sender, DownloadingProgressChangedEventArgs e) {
            if (this.progressChangedEventArgs != null) {
                progressChangedEventArgs.received += e.BytesReceived;
                this.OnDownloadingProgressChanged (new DownloadingProgressChangedEventArgs (progressChangedEventArgs.BytesReceived, e.BytesRead, progressChangedEventArgs.TotalBytesToReceive, progressChangedEventArgs.UserState));
            }
        }

        protected virtual WebRequest GetWebRequest (System.Uri address) {
            return WebRequest.Create (address);
        }

        protected virtual WebResponse GetWebResponse (WebRequest request) {
            WebResponse response = request.GetResponse ();
            this.responseHeaders = response.Headers;
            return response;
        }

        protected void OnDownloadingProgressChanged (DownloadingProgressChangedEventArgs e) {
            if (this.DownloadProgressChanged != null) {
                this.DownloadProgressChanged (this, e);
            }
        }

        public Action<object,DownloadingProgressChangedEventArgs> DownloadProgressChanged;

        protected void OnDownloadFileCompleted (System.ComponentModel.AsyncCompletedEventArgs args) {
            this.CompleteAsync ();
            if (this.DownloadFileCompleted != null) {
                this.DownloadFileCompleted (this, args);
            }
        }

        public Action<object,System.ComponentModel.AsyncCompletedEventArgs> DownloadFileCompleted;

        #region block event

        protected virtual void OnBlockDownloadFileCompleted (BlockThead blockThread, System.ComponentModel.AsyncCompletedEventArgs args) {
            if (this.BlockDownloadFileCompleted != null) {
                this.BlockDownloadFileCompleted (blockThread, args);
            }
        }
        protected System.ComponentModel.AsyncCompletedEventHandler BlockDownloadFileCompleted;

        protected virtual void OnBlockDownloadingProgressChanged (DownloadingProgressChangedEventArgs e) {
            if (this.BlockDownloadProgressChanged != null) {
                this.BlockDownloadProgressChanged (this, e);
            }
        }
        protected Action<object,DownloadingProgressChangedEventArgs> BlockDownloadProgressChanged;

        #endregion

        public void Dispose () {
            this.DownloadProgressChanged = null;
            this.DownloadFileCompleted = null;
            this.async_children_thread.Clear ();
            this.async_children_finished.Clear ();
            this.async_children_error.Clear ();
        }
    }
}

namespace Hugula.Update {

    /// <summary>
    /// 超时功能的download
    /// </summary>
    public class WebDownload : BreakpointWebClient {
        public object userData;

        public string error;

        public int timeout { get; set; }

        public int tryTimes;

        public WebDownload () : this (10000) { }

        public WebDownload (int timeout) {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest (Uri address) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create (address); // base.GetWebRequest (address);
            if (request != null) {
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
            }
            return request;
        }

        #region ObjectPool 
        static ObjectPool<WebDownload> objectPool = new ObjectPool<WebDownload> (m_ActionOnGet, m_ActionOnRelease);
        private static void m_ActionOnGet (WebDownload item) {
            item.error = null;
            item.tryTimes = 0;
        }

        private static void m_ActionOnRelease (WebDownload item) {
            item.userData = null;
            item.Dispose ();
        }

        public static WebDownload Get () {
            return objectPool.Get ();
        }

        public static void Release (WebDownload toRelease) {
            objectPool.Release (toRelease);
        }
        #endregion
    }

}