// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Hugula.Update;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Hugula.Loader {

    /// <summary>
    /// loader 公共方法
    /// </summary>

    public class LoaderHelper {

        /// <summary>
        /// 目标引用减一
        /// </summary>
        /// <param name="hashcode"></param>
        /// <returns></returns>
        public static string OverrideRequestUrlByCrc (CRequest req) {
            string url = req.url;
            ABInfo abInfo = ManifestManager.GetABInfo (req.key);
            if (abInfo != null) {
                bool appCrc = HugulaSetting.instance != null ? HugulaSetting.instance.appendCrcToFile : false;
                if (abInfo.crc32 > 0 && appCrc) {
                    url = CUtils.InsertAssetBundleName (url, "_" + abInfo.crc32.ToString ());
                }
            }

            return url;
        }

        internal static HttpWebRequest SetupRequest (CRequest req, int timeout = 10000) {
            System.Uri uri = new System.Uri (req.url);
            HttpWebRequest webRequest = GetWebRequest (uri, timeout);
            // webRequest.Credentials = this.credentials;
            var headers = (WebHeaderCollection) req.webHeader;
            if (headers != null && headers.Count != 0 && webRequest != null) {
                HttpWebRequest httpWebRequest = (HttpWebRequest) webRequest;
                string text = headers["Expect"];
                string text2 = headers["Content-Type"];
                string text3 = headers["Accept"];
                string text4 = headers["Connection"];
                string text5 = headers["User-Agent"];
                string text6 = headers["Referer"];
                headers.Remove ("Expect");
                headers.Remove ("Content-Type");
                headers.Remove ("Accept");
                headers.Remove ("Connection");
                headers.Remove ("Referer");
                headers.Remove ("User-Agent");
                webRequest.Headers = headers;
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
            return webRequest;
        }

        protected static HttpWebRequest GetWebRequest (Uri address, int timeout = 10000) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create (address); // base.GetWebRequest (address);
            if (request != null) {
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
            }
            return request;
        }
    }

    /// <summary>
    /// loader 公共方法
    /// </summary>
    [SLua.CustomLuaClass]
    public class LoaderType {
        #region check type
        public static readonly Type Typeof_String = typeof (System.String);
        public static readonly Type Typeof_Bytes = typeof (System.Byte[]);
        public static readonly Type Typeof_AssetBundle = typeof (AssetBundle);
        public static readonly Type Typeof_ABScene = typeof (AssetBundleScene);
        public static readonly Type Typeof_ABAllAssets = typeof (UnityEngine.Object[]);
        public static readonly Type Typeof_AudioClip = typeof (AudioClip);
        public static readonly Type Typeof_Texture2D = typeof (Texture2D);
        // public static readonly Type Typeof_Texture = typeof(Texture);

        public static readonly Type Typeof_Object = typeof (UnityEngine.Object);
        #endregion
    }
}