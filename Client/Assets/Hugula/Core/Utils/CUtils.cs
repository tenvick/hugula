// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.IO;
using Hugula.Cryptograph;
using UnityEngine;

namespace Hugula.Utils
{

    public static class CUtils
    {
#if HUGULA_RELEASE
        public const bool isRelease = true;
#else
        public const bool isRelease = false;
#endif
#if HUGULA_NO_LOG
        public static bool printLog = false;
#else
        public static bool printLog = true;
#endif

#if UNITY_EDITOR
        public const bool isEditor = true;
#else
        public const bool isEditor = false;

#endif

        /// <summary>
        /// string EndWith
        /// </summary>
        /// <param name="str">原始串</param>
        /// <param name="value">比较值</param>
        /// <returns></returns>
        public static bool EndWith(string str, string value)
        {
            var vlen = value.Length;
            var sLen = str.Length;
            if (vlen > sLen) return false;
            // str[0].Equals(value[0])
            int i = 0;
            while (i < vlen)
            {
                i++;
                if (!value[vlen - i].Equals(str[sLen - i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// string StartWith
        /// </summary>
        /// <param name="str">原始串</param>
        /// <param name="value">比较值</param>
        /// <returns></returns>
        public static bool StartWith(string str, string value)
        {
            if(str == null) return false;
            var vlen = value.Length;
            var sLen = str.Length;
            if (vlen > sLen) return false;
            int i = 0;
            while (i < vlen)
            {
                if (!value[i].Equals(str[i]))
                    return false;
                i++;
            }

            return true;
        }

        /// <summary>
        ///  获取当前gameobject的全路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetGameObjectFullPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            path = path.Substring(1);
            return path;
        }

        /// <summary>
        /// Gets the assetname of the URL file.
        /// </summary>
        /// <returns>
        /// The URL file name.
        /// </returns>
        /// <param name='url'>
        /// URL.
        /// </param>
        public static string GetAssetName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string fname = string.Empty;
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName(url, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);
            // Debug.LogFormat("lastFileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}", lastFileIndex, fileLen, lastDotIndex, suffixLen, url.Length);
            if (fileLen == 0) return string.Empty;
            fname = url.Substring(lastFileIndex, fileLen);
            return fname;
        }

        /// <summary>
        /// Gets the assetname of the URL file.
        /// </summary>
        /// <returns>
        /// The URL file name.
        /// </returns>
        /// <param name='url'>
        /// URL.
        /// </param>
        public static string GetFileName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string fname = string.Empty;
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName(url, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);
            // Debug.LogFormat("lastFileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}", lastFileIndex, fileLen, lastDotIndex, suffixLen, url.Length);
            if (fileLen == 0) return string.Empty;
            fname = url.Substring(lastFileIndex, fileLen+suffixLen);
            return fname;
        }

        /// <summary>
        /// AssetBundleName
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAssetBundleName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            int idxEnd = url.IndexOf('?');
            int idxBegin = url.IndexOf(platformFloder); // 

            if (idxBegin == -1)
            {
                idxBegin = 0;
            }
            else
                idxBegin = idxBegin + platformFloder.Length + 1;

            if (idxBegin >= url.Length) idxBegin = 0;
            if (idxEnd == -1) idxEnd = url.Length;
            int m_len = idxEnd - idxBegin;

            string re = string.Empty;
            if (m_len == 0) return re;

            re = url.Substring(idxBegin, idxEnd - idxBegin);
            return re;
        }

        /// <summary>
        /// assetbundle suffix
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>       
        public static string GetSuffix(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            string fname = string.Empty;
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName(url, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);

            if (suffixLen > 1)
            {
                suffixLen = suffixLen - 1;
                fname = url.Substring(lastDotIndex + 1, suffixLen);
            }
            return fname;
        }

        /// <summary>
        /// assetbundle Variants base name
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>  
        public static string GetBaseName(string assetbundleVariants)
        {
            if (string.IsNullOrEmpty(assetbundleVariants)) return string.Empty;
            string baseName = assetbundleVariants;
            int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            AnalysePathName(assetbundleVariants, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);

            if (suffixLen > 1)
            {
                suffixLen = suffixLen - 1;
                string suffix = assetbundleVariants.Substring(lastDotIndex + 1, suffixLen);

                if (suffix.Equals(Common.ASSETBUNDLE_SUFFIX))
                {
                    baseName = assetbundleVariants.Substring(0, lastFileIndex + fileLen + 4);
                }
                else
                {
                    baseName = assetbundleVariants.Substring(0, lastFileIndex + lastDotIndex - lastFileIndex);
                }
            }
            return baseName;
        }

        /// <summary>
        /// check www url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CheckWWWUrl(string url)
        {
            if (url.IndexOf("://") == -1)
            {
                url = string.Format("file:///{0}", url);
            }
            return url;
        }

        /// <summary>
        /// check full url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CheckFullUrl(string url)
        {
            return url.IndexOf("://") >=0 || url.IndexOf(":\\\\") >=0;
        }

        /// <summary>
        /// check Resolve host error
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsResolveHostError(string error)
        {
            error = error.ToLower();
            if ((error.Contains("resolve") && error.Contains("host")) ||
                error.Contains("nameresolutionfailure"))
            { //dns error
                return true;
            }
            return false;
        }

        /// <summary>
        /// check is https 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsHttps(string url)
        {
            return url.ToLower().StartsWith("https");
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
        [System.Obsolete("please use UriGroup")]
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
        [System.Obsolete("please use UriGroup")]
        public static string GetAssetFullPath(string assetPath)
        {
            string name = GetRightFileName(assetPath);
            string path = GetFileFullPath(GetAssetPath(name));
            return path;
        }

        /// <summary>
        /// GetAssetOriginalFullPath
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        [System.Obsolete("please use UriGroup")]
        public static string GetAssetOriginalFullPath(string assetPath)
        {
            string path = GetFileFullPath(GetAssetPath(assetPath));
            return path;
        }

        /// <summary>
        /// insert some worlds before suffix
        /// </summary>
        /// <param name="assetbundleName"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        public static string InsertAssetBundleName(string assetbundleName, string insert)
        {
            int fileIndex, fileLen, dotIndex, suffixLen;
            AnalysePathName(assetbundleName, out fileIndex, out fileLen, out dotIndex, out suffixLen);
            // Debug.LogFormat("fileIndex{0} fileLen{1} dotIndex{2} suffixLen{3} len{4}",fileIndex,fileLen,dotIndex,suffixLen,assetbundleName.Length);
            string fname = string.Empty;
            _textSB.Length = 0;
            _textSB.Append(assetbundleName);
            int firstDont = fileIndex + fileLen;
            if (firstDont > 0)
                _textSB.Insert(firstDont, insert);
            else
                _textSB.Insert(assetbundleName.Length, insert);

            fname = _textSB.ToString();

            return fname;
        }
        /// <summary>
        /// Gets the UD key.
        /// </summary>
        /// <returns>The UD key.</returns>
        /// <param name="url">url.</param>
        public static string GetUDKey(string url)
        {
            string udKey = CryptographHelper.CrypfString(url);
            return udKey;
        }

        public static string GetRealStreamingAssetsPath()
        {
            return realStreamingAssetsPath;
        }

        public static string GetRealPersistentDataPath()
        {
            return realPersistentDataPath;
        }

        /// <summary>
        /// add platformFloder  to asset path
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAssetPath(string name)
        {
            string path = platformFloder;
            if (!string.IsNullOrEmpty(name))
                path = PathCombine(platformFloder, name);
            return path;
        }

        /// <summary>
        /// the name manifest
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformFolderForAssetBundles()
        {
            return platform;
        }

        /// <summary>
        /// get Md5 or normal filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetRightFileName(string fileName)
        {
            // #if !HUGULA_COMMON_ASSETBUNDLE
            // if (string.IsNullOrEmpty(fileName)) return string.Empty;
            // int lastFileIndex, lastDotIndex, fileLen, suffixLen;
            // AnalysePathName(fileName, out lastFileIndex, out fileLen, out lastDotIndex, out suffixLen);

            // string fname = fileName.Substring(lastFileIndex, fileLen);
            // string md5 = string.Empty;
            // md5 = CryptographHelper.Md5String(fname);
            // _textSB.Length = 0;
            // _textSB.Append(fileName);
            // if (fileLen > 0) _textSB.Replace(fname, md5, lastFileIndex, fileLen);
            // fname = _textSB.ToString();
            // return fname;
            // #else
            return fileName;
            // #endif
        }

        /// <summary>
        /// get Persistent assetbundle name  
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPersistentBundleFileName(string filename)
        {
            return $"_{filename}";
        }

        public static bool currPersistentExist = false;

        static System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

        /// <summary>
        /// convert time to timeline
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// Milliseconds Timestamp
        /// </summary>
        public static double msTimestamp
        {
            get
            {
                return (System.DateTime.Now - startTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int Timeline()
        {
            return ConvertDateTimeInt(DateTime.Now);
        }

        /// <summary>
        /// Path Combine
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2)
        {

            if (path2.Length == 0)
            {
                return path1;
            }
            if (path1.Length == 0)
            {
                return path2;
            }

            string path = string.Empty;
            char c2 = path2[0];
            char c = path1[path1.Length - 1];

            if (c2 == '\\' || c2 == '/' || c2 == ':')
            {
                path2 = path2.Substring(1);
            }

            if (c != '\\' && c != '/' && c != ':')
            {
                path = path1 + "/" + path2;
            }
            else
                path = path1 + path2;

            return path;
        }

        /// <summary>
        /// Get Android AssetBundle.LoadFrom Path
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string GetAndroidABLoadPath(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // path = path.Replace(Common.JAR_FILE,"").Replace("!/assets","!assets");
#endif
            return path;
        }

        public static string bundleIdentifier
        {
            get
            {
#if UNITY_2017 || UNITY_5_6_OR_NEWER
                return Application.identifier;
#else
                return Application.bundleIdentifier;
#endif
            }

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
        internal static void AnalysePathName(string pathName, out int fileIndex, out int fileLen, out int dotIndex, out int suffixLen)
        {
            int len = pathName.Length;
            fileIndex = 0; //the last / or \ position
            int firstDotIndex = len; //the first dot position
            int lastDotIndex = len;//the last dot
            int questIndex = len; //the ? position

            int i = len - 1;
            char cha;
            while (i >= 0)
            {
                cha = pathName[i];
                if (cha == '/' || cha == '\\')
                {
                    fileIndex = i + 1;
                    break;
                }

                if (cha == '?')
                    questIndex = i;

                if (cha == '.')
                {
                    firstDotIndex = i;
                    if (lastDotIndex == len) lastDotIndex = i;
                }

                i--;
            }

            if (firstDotIndex > questIndex) firstDotIndex = questIndex;
            dotIndex = lastDotIndex;
            suffixLen = questIndex - lastDotIndex;
            fileLen = firstDotIndex - fileIndex;
            // Debug.LogFormat("fileInde={0},questIndex={1},dotIndex={2},len={3},suffixLen={4},fileLen={5}", fileIndex, questIndex, dotIndex, len, suffixLen, fileLen);
        }
        #endregion

        #region Platform info
#if UNITY_IOS
        public const string platform = "ios";
#elif UNITY_ANDROID
        public const string platform = "android";
#elif UNITY_FACEBOOK || USE_GAMEROOM
        public const string platform = "gameroom";
#elif UNITY_METRO
        public const string platform = "metroplayer";
#elif UNITY_OSX || UNITY_STANDALONE_OSX
        public const string platform = "osx";//standaloneosxintel
#else
        public const string platform = "win";// standalonewindows
#endif

        // #if HUGULA_COMMON_ASSETBUNDLE
        //         /// <summary>
        //         /// platform
        //         /// </summary>
        //         public const string platformFloder = platform;
        // #else
        private static string _platformFloder;
        /// <summary>
        /// platform Floder name
        /// </summary>
        public static string platformFloder
        {
            get
            {
                if (string.IsNullOrEmpty(_platformFloder))
                    _platformFloder = CUtils.GetRightFileName(platform);

                return _platformFloder;
            }
        }
        // #endif

        private static string _realPersistentDataPath;
        /// <summary>
        /// real persistentData Path
        /// </summary>
        public static string realPersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_realPersistentDataPath))
                    _realPersistentDataPath = PathCombine(Application.persistentDataPath, platformFloder);
                return _realPersistentDataPath;
            }
            set
            {
                _realPersistentDataPath = null;
            }
        }

        private static string _realStreamingAssetsPath;
        /// <summary>
        /// real streamingAssets path
        /// </summary>
        public static string realStreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_realStreamingAssetsPath))
                    _realStreamingAssetsPath = PathCombine(Application.streamingAssetsPath, platformFloder);
                return _realStreamingAssetsPath;
            }
            set
            {
                _realStreamingAssetsPath = null;
            }
        }

        public static string _androidFileStreamingAssetsPath;
        /// <summary>
        /// android streamingAssets path
        /// </summary>
        public static string androidFileStreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_androidFileStreamingAssetsPath))
                    _androidFileStreamingAssetsPath = PathCombine(Application.dataPath + "!assets", platformFloder);
                return _androidFileStreamingAssetsPath;
            }
        }

        private static string _uncompressStreamingAssetsPath;
        /// <summary>
        /// uncompress streamingAssets path
        /// </summary>
        public static string uncompressStreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_uncompressStreamingAssetsPath))
                {
                    _uncompressStreamingAssetsPath = PathCombine(Application.persistentDataPath, "local");
                }
                return _uncompressStreamingAssetsPath;
            }
        }

        private static string _realUncompressStreamingAssetsPath;

        /// <summary>
        /// real uncompress streamingAssets path
        /// </summary>
        public static string realUncompressStreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(_realUncompressStreamingAssetsPath))
                {
                    _realUncompressStreamingAssetsPath = PathCombine(uncompressStreamingAssetsPath, platformFloder);
                }
                return _realUncompressStreamingAssetsPath;
            }
        }

        private static string _dataPath;

        /// <summary>
        /// Application.dataPath
        /// </summary>
        public static string dataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_dataPath))
                    _dataPath = Application.dataPath;
                return _dataPath;
            }
        }

        #endregion

        private static System.Text.StringBuilder _textSB = new System.Text.StringBuilder();
        //private static System.DateTime _last_time = System.DateTime.Now;
        //private static readonly System.DateTime _begin_time = System.DateTime.Now;
        //编辑器下使用
        public static double DebugCastTime(string tips)
        {
            int all_ds = 0;
#if UNITY_EDITOR
            var now = Timeline();
            var _last_time = UnityEditor.EditorPrefs.GetInt("CUtils_D_last_time", now);
            var cast = now - _last_time;
            var begin =  UnityEditor.EditorPrefs.GetInt("CUtils_D_begin_time", now);
            if(begin == now) //如果没有设置
            {
                UnityEditor.EditorPrefs.SetInt("CUtils_D_begin_time", now);
            }
            all_ds = now - UnityEditor.EditorPrefs.GetInt("CUtils_D_begin_time", now);
            UnityEditor.EditorPrefs.SetInt("CUtils_D_last_time", now);

            Debug.LogFormat("Cast Time \"{0}\" Cast({1} s) Time({2}) runtime({3} s),frame={4}", tips, cast,now, all_ds, Time.frameCount);
#endif
            return all_ds;
        }

#if UNITY_EDITOR
        internal static void ClearCastTime()
        {
            UnityEditor.EditorPrefs.DeleteKey("CUtils_D_last_time");
            UnityEditor.EditorPrefs.DeleteKey("CUtils_D_begin_time");
            Debug.Log(" delete EditorPrefs Key (CUtils_D_last_time,CUtils_D_begin_time)  ");
        }

#endif
    }
}