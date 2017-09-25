// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections.Generic;
using Hugula.Update;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace Hugula.Loader
{

    /// <summary>
    /// uri 组策略
    /// </summary>
    [SLua.CustomLuaClass]
    public class UriGroup
    {
        #region member
        //private
        private List<string> uris;
        private Dictionary<int, Action<CRequest, Array>> onWWWCompletes;
        private Dictionary<int, Func<CRequest, bool>> onCrcChecks;

        private Dictionary<int, Func<CRequest, string>> onOverrideUrls;

        public int count { get { return uris.Count; } }

        #endregion

        #region private mothed
        void AddonWWWCompletes(int index, Action<CRequest, Array> onWWWComplete)
        {
            if (onWWWCompletes == null) onWWWCompletes = new Dictionary<int, Action<CRequest, Array>>();
            onWWWCompletes[index] = onWWWComplete;
        }

        void AddonCrcChecks(int index, Func<CRequest, bool> onCrcCheck)
        {
            if (onCrcChecks == null) onCrcChecks = new Dictionary<int, Func<CRequest, bool>>();
            onCrcChecks[index] = onCrcCheck;
        }

        void AddonOverrideUrls(int index, Func<CRequest, string> onOverrideUrl)
        {
            if (onOverrideUrls == null) onOverrideUrls = new Dictionary<int, Func<CRequest, string>>();
            onOverrideUrls[index] = onOverrideUrl;
        }

        #endregion

        public UriGroup()
        {
            uris = new List<string>();
        }

        /// <summary>
        /// 添加uri
        /// </summary>
        /// <param name="uri"></param>
        public int Add(string uri)
        {
            int len = uris.Count;
            uris.Add(uri);
            return len;
        }

        public void Add(string uri, bool needCheckCrc)
        {
            int index = Add(uri);

            if (needCheckCrc) AddonCrcChecks(index, CrcCheck.CheckUriCrc);
        }

        public void Add(string uri, bool needCheckCrc, bool onWWWComp, bool onOverrideUrl)
        {
            int index = Add(uri);

            if (onWWWComp) AddonWWWCompletes(index, SaveWWWFileToPersistent);
            if (needCheckCrc) AddonCrcChecks(index, CrcCheck.CheckUriCrc);
            if (onOverrideUrl) AddonOverrideUrls(index, OverrideRequestUrlByCrc);
        }

        /// <summary>
        /// 添加uri
        /// </summary>
        /// <param name="uri"></param>
        public void Add(string uri, Action<CRequest, Array> onWWWComplete, Func<CRequest, bool> onCrcCheck)
        {
            int index = Add(uri);
            if (onWWWComplete != null) AddonWWWCompletes(index, onWWWComplete);
            if (onCrcCheck != null) AddonCrcChecks(index, CrcCheck.CheckUriCrc);
        }

        public void Add(string uri, Action<CRequest, Array> onWWWComplete, Func<CRequest, bool> onCrcCheck, Func<CRequest, string> onOverrideUrl)
        {
            int index = Add(uri);
            if (onWWWComplete != null) AddonWWWCompletes(index, onWWWComplete);
            if (onCrcCheck != null) AddonCrcChecks(index, CrcCheck.CheckUriCrc);
            if (onOverrideUrl != null) AddonOverrideUrls(index, onOverrideUrl);
        }

        public bool CheckUriCrc(CRequest req)
        {
            Func<CRequest, bool> act = null;
            if (onCrcChecks != null && onCrcChecks.TryGetValue(req.index, out act))
            {
                return act(req);
            }
            return true;
        }

        internal void OnWWWComplete(CRequest req, byte[] bytes)
        {
            Action<CRequest, Array> act = null;
            if (onWWWCompletes != null && onWWWCompletes.TryGetValue(req.index, out act))
            {
                act(req, bytes);
            }
        }

        internal string OnOverrideUrl(CRequest req)
        {
            Func<CRequest, string> act = null;
            if (onOverrideUrls != null && onOverrideUrls.TryGetValue(req.index, out act))
            {
                return act(req);
            }
            return req.url;
        }

        internal string this[int index]
        {
            get
            {
                if (uris.Count > index && index >= 0)
                {
                    return uris[index];
                }
                else
                    return string.Empty;
            }
        }

        public string GetUri(int index)
        {
            return this[index];
        }

        public void Clear()
        {
            uris.Clear();
            if (onWWWCompletes != null) onWWWCompletes.Clear();
            if (onCrcChecks != null) onCrcChecks.Clear();
            if (onOverrideUrls != null) onOverrideUrls.Clear();
        }

        #region static
        /// <summary>
        /// 设置crequest next index处的url
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckAndSetNextUriGroup(CRequest req)
        {
            if (req.uris == null) return false;

            int count = req.uris.count;
            int index = req.index + 1;
            // if (index >= count) index = 0;
            if (count > index && index >= 0)
            {
                req.index = index;
                req.url = string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测CRequest index 处的url crc校验，默认返回true
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static bool CheckRequestCurrentIndexCrc(CRequest req)
        {
            if (req.uris != null)
                return req.uris.CheckUriCrc(req);
            else
                return true;
        }

        /// <summary>
        /// Check WWW Complete event
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static void CheckWWWComplete(CRequest req, WWW www)
        {
            if (req.uris != null) req.uris.OnWWWComplete(req, www.bytes);
        }

        public static void CheckWWWComplete(CRequest req, UnityWebRequest www)
        {
            if (req.uris != null) req.uris.OnWWWComplete(req, www.downloadHandler.data);
        }

        public static void SaveWWWFileToPersistent(CRequest req, Array www)
        {
            string saveName = req.assetBundleName;
            FileHelper.SavePersistentFile(www, saveName);
        }

        public static string OverrideRequestUrlByCrc(CRequest req)
        {
            string url = req.url;
            ABInfo abInfo = ManifestManager.GetABInfo(req.assetBundleName);
            if (abInfo != null)
            {
                bool appCrc = HugulaSetting.instance != null ? HugulaSetting.instance.appendCrcToFile : false;
                if (abInfo.crc32 > 0 && appCrc)
                {
                    url = CUtils.InsertAssetBundleName(url, "_" + abInfo.crc32.ToString());
                }
            }

            return url;
        }


        static UriGroup _uriList;
        /// <summary>
        /// The URI list.
        /// </summary>
        public static UriGroup uriList
        {
            get
            {
                if (_uriList == null)
                {
                    _uriList = new UriGroup();
                    _uriList.Add(CUtils.GetRealPersistentDataPath(), true);
#if !UNITY_EDITOR && (SEVENZIP || HUGULA_COMPRESS_STREAMINGASSETS)
                    _uriList.Add(CUtils.realUncompressStreamingAssetsPath);
#else
                    _uriList.Add(CUtils.GetRealStreamingAssetsPath());
#endif
                }
                return _uriList;
            }
            set
            {
                _uriList = value;
            }
        }

        #endregion
    }
}