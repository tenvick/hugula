// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Cryptograph;
using Hugula.Loader;

namespace Hugula.Utils
{
    [SLua.CustomLuaClass]
    public class CUtils
    {


        /// <summary>
        /// Gets the name of the URL file.
        /// </summary>
        /// <returns>
        /// The URL file name.
        /// </returns>
        /// <param name='url'>
        /// URL.
        /// </param>
        public static string GetURLFileName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string re = "";
            int len = url.Length - 1;
            char[] arr = url.ToCharArray();
            while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
                len = len - 1;
            //int sub = (url.Length - 1) - len;
            //int end=url.Length-sub;

            re = url.Substring(len + 1);
            int last = re.LastIndexOf(".");
            if (last == -1) last = re.Length;
            string cut = re.Substring(0, last);
            return cut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetKeyURLFileName(string url)
        {
            //Debug.Log(url);
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string re = "";
            int len = url.Length - 1;
            char[] arr = url.ToCharArray();
            while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
                len = len - 1;
            //int sub = (url.Length - 1) - len;
            //int end=url.Length-sub;

            re = url.Substring(len + 1);
            int last = re.LastIndexOf(".");
            if (last == -1) last = re.Length;
            string cut = re.Substring(0, last);
            cut = cut.Replace('.', '_');
            return cut;
        }

        public static string GetURLFullFileName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string re = "";
            int len = url.Length - 1;
            char[] arr = url.ToCharArray();
            while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
                len = len - 1;

            re = url.Substring(len + 1);
            return re;
        }

        /// <summary>
        /// Gets the file full path for www
        /// form Application.dataPath
        /// </summary>
        /// <returns>
        /// The file full path.
        /// </returns>
        /// <param name='absolutePath'>
        /// Absolute path.
        /// </param>
        public static string GetFileFullPath(string absolutePath)
        {
            string path = "";

            path = Application.persistentDataPath + "/" + absolutePath;
            currPersistentExist = File.Exists(path);
            if (!currPersistentExist)
                path = Application.streamingAssetsPath + "/" + absolutePath;

            if (path.IndexOf("://") == -1)
            {
                path = "file://" + path;
            }

            return path;
        }

        /// <summary>
        /// get assetBunld full path
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetFullPath(string assetPath)
        {
            string name = GetFileName(assetPath);
            string path = GetFileFullPath(GetAssetPath(name));
            return path;
        }

        /// <summary>
        /// 得到原始的名字
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetOriginalFullPath(string assetPath)
        {
            string path = GetFileFullPath(GetAssetPath(assetPath));
            return path;
        }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <returns>The URI.</returns>
        /// <param name="uris">Uris.</param>
        /// <param name="index">Index.</param>
        public static string GetUri(string[] uris, int index)
        {
            string uri = "";
            if (uris.Length > index && index >= 0)
            {
                uri = uris[index];
            }
            return uri;
        }

        /// <summary>
        /// Gets the UD key.
        /// </summary>
        /// <returns>The UD key.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="relativeUrl">Relative URL.</param>
        public static string GetUDKey(string uri, string relativeUrl)
        {
            string url = Path.Combine(uri, relativeUrl);
            string udKey = CryptographHelper.CrypfString(url);
            return udKey;
        }

        /// <summary>
        /// Sets the request URI.
        /// </summary>
        /// <returns><c>true</c>, if request URI was set, <c>false</c> otherwise.</returns>
        /// <param name="req">Req.</param>
        /// <param name="index">Index.</param>
        public static bool SetRequestUri(CRequest req, int index)
        {
            string uri = GetUri(req.uris, index);
            if (!string.IsNullOrEmpty(uri))
            {
                req.uri = uri;
                req.index = index;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断第0个资源是否存在
        /// </summary>
        /// <returns><c>true</c>, if exists was check0ed, <c>false</c> otherwise.</returns>
        /// <param name="req">Req.</param>
        public static void CheckUri0Exists(CRequest req)
        {
            if (req.index == 0 && !req.url.StartsWith(Common.HTTP_STRING) && !File.Exists(req.url))
            { //如果没有更新
                SetRequestUri(req, 1);
            }
        }


        public static string GetRealStreamingAssetsPath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, GetAssetPath(""));
            return path;
        }

        public static string GetRealPersistentDataPath()
        {
            string path = Path.Combine(Application.persistentDataPath, GetAssetPath(""));
            return path;
        }

        /// <summary>
        /// 得到资源的相对路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAssetPath(string name)
        {
            string Platform = "";
#if UNITY_IOS
			Platform="iOS";
#elif UNITY_ANDROID
            Platform = "Android";
#elif UNITY_WEBPLAYER
            Platform ="WebPlayer";
#elif UNITY_WP8
			Platform="WP8Player";
#elif UNITY_METRO
            Platform = "MetroPlayer";
#elif UNITY_OSX || UNITY_STANDALONE_OSX
		    Platform = "StandaloneOSXIntel";
#else
            Platform = "StandaloneWindows";
#endif

#if BUILD_COMMON_ASSETBUNDLE 
            string path = Path.Combine(Platform, name);
#else
            string path = Path.Combine(CryptographHelper.Md5String(Platform), name);
#endif
            return path;
        }

        /// <summary>
        /// 得到当前平台的manifest名字
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformFolderForAssetBundles()
        {
            string manifest = string.Empty;
#if UNITY_IOS
			manifest = "iOS";
#elif UNITY_ANDROID
            manifest = "Android";
#elif UNITY_WEBPLAYER
            manifest = "WebPlayer";
#elif UNITY_WP8
			manifest = "WP8Player";
#elif UNITY_METRO
            manifest = "MetroPlayer";
#elif UNITY_OSX 
		    manifest = "OSX";
#elif UNITY_STANDALONE_OSX
		    manifest =  "StandaloneOSXIntel";
#else
            manifest = "StandaloneWindows";
#endif
            //#if BUILD_COMMON_ASSETBUNDLE 
            return manifest;
            //#else
            //			return CryptographHelper.Md5String(manifest);
            //#endif

        }

        /// <summary>
        /// 得到加密或者没有加密的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileName(string fileName)
        {
#if BUILD_COMMON_ASSETBUNDLE 
        return fileName;
#else

            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            string re = "";
            string fname = "";

            int len = fileName.Length - 1;
            char[] arr = fileName.ToCharArray();
            while (len >= 0 && arr[len] != '/' && arr[len] != '\\')
                len = len - 1;


            re = fileName.Substring(len + 1);
            int last = re.LastIndexOf(".");
            if (last == -1)
            {
                last = re.Length;
                fname = CryptographHelper.Md5String(re.Substring(0, last));
            }
            else
            {
                fname = CryptographHelper.Md5String(re.Substring(0, last));
                string end = re.Substring(last, re.Length - last);
                fname = fname + end;
            }

            return fname;
#endif
        }

        public static bool currPersistentExist = false;

        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}