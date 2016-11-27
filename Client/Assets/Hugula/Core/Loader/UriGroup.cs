// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System;
using Hugula.Utils;
using Hugula.Update;

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

        internal void OnWWWComplete(CRequest req, WWW www)
        {
            Action<CRequest, Array> act = null;
            if (onWWWCompletes != null && onWWWCompletes.TryGetValue(req.index, out act))
            {
                act(req, www.bytes);
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

        /// <summary>
        /// 获取当前索引的uri
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetUri(int index)
        {
            string uri = "";
            if (uris.Count > index && index >= 0)
            {
                uri = uris[index];
            }
            return uri;
        }

        /// <summary>
        /// 设置req index处的uri
        /// </summary>
        /// <param name="req"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetNextUri(CRequest req)
        {
            int index = req.index + 1;
            if (index >= count) index = 0;
            string uri = GetUri(index);
            if (!string.IsNullOrEmpty(uri))
            {
                req.index = index;
                req.uri = uri;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            uris.Clear();
            if (onWWWCompletes != null) onWWWCompletes.Clear();
            if (onCrcChecks != null) onCrcChecks.Clear();
            if (onOverrideUrls != null) onOverrideUrls.Clear();
        }


        #region static
        public static void SaveWWWFileToPersistent(CRequest req, Array www)
        {
            string saveName = req.assetBundleName;
            FileHelper.SavePersistentFile(www, saveName);
        }

        public static string OverrideRequestUrlByCrc(CRequest req)
        {
            string url = req.url;
            uint crc = Hugula.Update.CrcCheck.GetCrc(req.assetBundleName);
			if(crc>0)
			{
            	url = CUtils.InsertAssetBundleName(url, "_" + crc.ToString());
			}
            return url;
        }

        #endregion
    }
}