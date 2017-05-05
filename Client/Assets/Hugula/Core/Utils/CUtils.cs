// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.IO;
using Hugula.Cryptograph;
using UnityEngine;

namespace Hugula.Utils {
    [SLua.CustomLuaClass]
    public static class CUtils {
#if HUGULA_RELEASE
        public const bool isRelease = true;
#else
        public const bool isRelease = false;
#endif
        /// <summary>
        /// Gets the assetname of the URL file.
        /// </summary>
        /// <returns>
        /// The URL file name.
        /// </returns>
        /// <param name='url'>
        /// URL.
        /// </param>
        public static string GetAssetName (string url) {
            if (string.IsNullOrEmpty (url)) return string.Empty;
            string fname = "";
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName (url, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);
            // Debug.LogFormat("lastFileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}", lastFileIndex, fileLen, lastDotIndex, suffixLen, url.Length);
            fname = url.Substring (lastFileIndex, fileLen);
            return fname;
        }

        /// <summary>
        /// AssetBundleName
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAssetBundleName (string url) {
            if (string.IsNullOrEmpty (url)) return string.Empty;
            int idxEnd = url.IndexOf ('?');
            int idxBegin = url.IndexOf (platformFloder); // 

            if (idxBegin == -1) {
                // idxBegin = url.LastIndexOf ("/")+1;
                // int idx1 = url.LastIndexOf ("\\")+1;
                // if (idx1 > idxBegin) idxBegin = idx1;
                idxBegin = 0;
            } else
                idxBegin = idxBegin + platformFloder.Length + 1;

            if (idxBegin >= url.Length) idxBegin = 0;
            if (idxEnd == -1) idxEnd = url.Length;

            string re = url.Substring (idxBegin, idxEnd - idxBegin);
            return re;
        }

        /// <summary>
        /// assetbundle suffix
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>       
        public static string GetSuffix (string url) {
            if (string.IsNullOrEmpty (url)) return string.Empty;
            string fname = string.Empty;
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName (url, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);
            // Debug.LogFormat("lastFileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}", lastFileIndex, fileLen, lastDotIndex, suffixLen, url.Length);
            if (suffixLen > 1) {
                suffixLen = suffixLen - 1;
                fname = url.Substring (lastDotIndex + 1, suffixLen);
            }
            return fname;
        }

        /// <summary>
        /// check www url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CheckWWWUrl (string url) {
            if (url.IndexOf ("://") == -1) url = string.Format ("file:///{0}", url);
            return url;
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
        [System.Obsolete ("please use UriGroup")]
        public static string GetFileFullPath (string absolutePath) {
            string path = "";

            path = Application.persistentDataPath + "/" + absolutePath;
            currPersistentExist = File.Exists (path);
            if (!currPersistentExist)
                path = Application.streamingAssetsPath + "/" + absolutePath;

            if (path.IndexOf ("://") == -1) {
                path = "file://" + path;
            }

            return path;
        }

        /// <summary>
        /// get assetBunld full path
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        [System.Obsolete ("please use UriGroup")]
        public static string GetAssetFullPath (string assetPath) {
            string name = GetRightFileName (assetPath);
            string path = GetFileFullPath (GetAssetPath (name));
            return path;
        }

        /// <summary>
        /// GetAssetOriginalFullPath
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        [System.Obsolete ("please use UriGroup")]
        public static string GetAssetOriginalFullPath (string assetPath) {
            string path = GetFileFullPath (GetAssetPath (assetPath));
            return path;
        }

        /// <summary>
        /// insert some worlds before suffix
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        public static string InsertAssetBundleName (string assetbundleName, string insert) {
            int fileIndex, fileLen, dotIndex, suffixLen;
            AnalysePathName (assetbundleName, out fileIndex, out fileLen, out dotIndex, out suffixLen);
            // Debug.LogFormat("fileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}",fileIndex,fileLen,dotIndex,suffixLen,assetbundleName.Length);
            string fname = string.Empty;
            _textSB.Length = 0;
            _textSB.Append (assetbundleName);
            if (dotIndex > 0)
                _textSB.Insert (dotIndex, insert);
            else
                _textSB.Insert (assetbundleName.Length, insert);

            fname = _textSB.ToString ();

            return fname;
        }
        /// <summary>
        /// Gets the UD key.
        /// </summary>
        /// <returns>The UD key.</returns>
        /// <param name="url">url.</param>
        public static string GetUDKey (string url) {
            string udKey = CryptographHelper.CrypfString (url);
            return udKey;
        }

        public static string GetRealStreamingAssetsPath () {
            return realStreamingAssetsPath;
        }

        public static string GetRealPersistentDataPath () {
            return realPersistentDataPath;
        }

        /// <summary>
        /// add platformFloder  to asset path
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAssetPath (string name) {
            string path = platformFloder;
            if (!string.IsNullOrEmpty (name))
                path = PathCombine (platformFloder, name);
            return path;
        }

        /// <summary>
        /// the name manifest
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformFolderForAssetBundles () {
            return platform;
        }

        /// <summary>
        /// get Md5 or normal filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetRightFileName (string fileName) {
            if (string.IsNullOrEmpty (fileName)) return string.Empty;

#if !HUGULA_COMMON_ASSETBUNDLE
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName (fileName, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);

            string fname = fileName.Substring (lastFileIndex, fileLen);
            string md5 = string.Empty;
            md5 = CryptographHelper.Md5String (fname);
            _textSB.Length = 0;
            _textSB.Append (fileName);
            if (fileLen > 0) _textSB.Replace (fname, md5, lastFileIndex, fileLen);
            fname = _textSB.ToString ();
            return fname;
#else
            return fileName;
#endif

        }

        public static bool currPersistentExist = false;

        /// <summary>
        /// convert time to timeline
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt (System.DateTime time) {
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (1970, 1, 1));
            return (int) (time - startTime).TotalSeconds;
        }

        /// <summary>
        /// Path Combine
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombine (string path1, string path2) {
            if (path2.Length == 0) {
                return path1;
            }
            if (path1.Length == 0) {
                return path2;
            }
            char c2 = path2[0];
            if (c2 == '\\' && c2 == '/' && c2 == ':') {
                path2 = path2.Substring (1);
            }
            char c = path1[path1.Length - 1];
            if (c != '\\' && c != '/' && c != ':') {
                return path1 + "/" + path2;
            }
            return path1 + path2;
            // return Path.Combine(path1, path2);
        }

        #region private

        /// <summary>
        /// Analyse file Path Name
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="fileIndex"></param>
        /// <param name="fileLen"></param>
        /// <param name="dotIndex"></param>
        /// <param name="suffixLen"></param>
        internal static void AnalysePathName (string pathName, out int fileIndex, out int fileLen, out int dotIndex, out int suffixLen) {
            int len = pathName.Length;
            fileIndex = 0; //the last / or \ position
            dotIndex = len; //the last dot position
            int questIndex = len; //the ? position

            var arr = pathName.ToCharArray ();
            int i = len - 1;
            char cha;
            while (i >= 0) {
                cha = arr[i];
                if (cha == '/' || cha == '\\') {
                    fileIndex = i + 1;
                    break;
                }

                if (cha == '?')
                    questIndex = i;

                if (cha == '.') {
                    dotIndex = i;
                }

                i--;
            }

            if (dotIndex > questIndex) dotIndex = questIndex;

            suffixLen = questIndex - dotIndex;
            fileLen = dotIndex - fileIndex;
            // Debug.LogFormat("fileInde={0},questIndex={1},dotIndex={2},len={3},suffixLen={4},fileLen={5}", fileIndex, questIndex, dotIndex, len, suffixLen, fileLen);
        }
        #endregion

        #region Platform info
#if UNITY_IOS
        public const string platform = "ios";
#elif UNITY_ANDROID
        public const string platform = "android";
#elif UNITY_METRO
        public const string platform = "metroplayer";
#elif UNITY_OSX || UNITY_STANDALONE_OSX
        public const string platform = "osx";//standaloneosxintel
#else
        public const string platform = "win";// standalonewindows
#endif

#if HUGULA_COMMON_ASSETBUNDLE
        /// <summary>
        /// platform
        /// </summary>
        public const string platformFloder = platform;
#else
        private static string _platformFloder;
        /// <summary>
        /// platform Floder name
        /// </summary>
        public static string platformFloder {
            get {
                if (string.IsNullOrEmpty (_platformFloder))
                    _platformFloder = CryptographHelper.Md5String (platform);

                return _platformFloder;
            }
        }
#endif

        private static string _realPersistentDataPath;
        /// <summary>
        /// real persistentData Path
        /// </summary>
        public static string realPersistentDataPath {
            get {
                if (string.IsNullOrEmpty (_realPersistentDataPath))
                    _realPersistentDataPath = PathCombine (Application.persistentDataPath, platformFloder);
                return _realPersistentDataPath;
            }
        }

        private static string _realStreamingAssetsPath;
        /// <summary>
        /// real streamingAssets path
        /// </summary>
        public static string realStreamingAssetsPath {
            get {
                if (string.IsNullOrEmpty (_realStreamingAssetsPath))
                    _realStreamingAssetsPath = PathCombine (Application.streamingAssetsPath, platformFloder);
                return _realStreamingAssetsPath;
            }
        }

        public static string _androidFileStreamingAssetsPath;
        /// <summary>
        /// android streamingAssets path
        /// </summary>
        public static string androidFileStreamingAssetsPath {
            get {
                if (string.IsNullOrEmpty (_androidFileStreamingAssetsPath))
                    _androidFileStreamingAssetsPath = PathCombine (Application.dataPath + "!assets", platformFloder);
                return _androidFileStreamingAssetsPath;
            }
        }

        #endregion
        private static System.Text.StringBuilder _textSB = new System.Text.StringBuilder (2048);
        private static System.DateTime _last_time = System.DateTime.Now;
        private static System.DateTime _begin_time = System.DateTime.Now;
        public static double DebugCastTime (string tips) {

            var ds = System.DateTime.Now - _last_time;
            var all_ds = System.DateTime.Now - _begin_time;
            double cast = ds.TotalSeconds;
            _last_time = System.DateTime.Now;
#if HUGULA_RELEASE
           return cast;
#else
            if (!string.IsNullOrEmpty (tips))
                Debug.LogFormat (tips + " Cast({0})s runtime({1})s  ", cast, all_ds.TotalSeconds);
            return cast;
#endif
           
        }
    }
}