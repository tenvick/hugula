using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;

namespace Hugula.Editor {
    //编辑器工具类
    public static class EditorUtils {

        public const string streamingPath = "Assets/StreamingAssets"; //打包assetbundle输出目录。
        public const string TmpPath = "Tmp/";
        public const string HugulaFolder = "HugulaFolder";

        //获取url的assetbundle name 
        static public string GetAssetBundleName (string url) {
            if (string.IsNullOrEmpty (url)) return string.Empty;
            int idxEnd = url.IndexOf ('?');
            int idxBegin = url.IndexOf (CUtils.platformFloder); // 

            if (idxBegin == -1) {
                idxBegin = url.LastIndexOf ("/") + 1;
                int idx1 = url.LastIndexOf ("\\") + 1;
                if (idx1 > idxBegin) idxBegin = idx1;
            } else
                idxBegin = idxBegin + CUtils.platformFloder.Length + 1;

            if (idxBegin >= url.Length) idxBegin = 0;
            if (idxEnd == -1) idxEnd = url.Length;

            string re = url.Substring (idxBegin, idxEnd - idxBegin);
            return re;
        }

        //获取label
        static public string GetLabelsByPath (string abPath) {
            return HugulaExtensionFolderEditor.GetLabelsByPath (abPath);
        }

        static public string GetGameObjectPathInScene (Transform obj, string path) {
            if (obj.parent == null) {
                if (string.IsNullOrEmpty (path)) path = obj.name;
                return path; //  +/+path
            } else {
                string re = string.Format ("{0}/{1}", obj.parent.name, obj.name);
                return GetGameObjectPathInScene (obj.transform.parent, re);
            }
        }

        public static string GetAssetTmpPath () {
            return Path.Combine (Application.dataPath, TmpPath);
        }

        public static string GetProjectTempPath () {
            return Path.Combine (Application.dataPath.Replace ("Assets", ""), "Temp/hugula");
        }

        public static string GetOutPutPath () {
            return Path.Combine (streamingPath, CUtils.GetAssetPath (""));
        }

        public static string GetAssetPath (string filePath) {
            string path = filePath.Replace (Application.dataPath + "/", "");
            return path; //Path.Combine("Assets",path);
        }

        public static string GetLuaBytesResourcesPath () {
            string luapath = "Assets/" + Common.LUACFOLDER + "/Resources/luac";
            DirectoryInfo p = new DirectoryInfo(luapath);
            if(!p.Exists) p.Create();
            return luapath;
        }

        public static string GetFileStreamingOutAssetsPath () {
            string dircAssert = Path.Combine (Application.streamingAssetsPath, CUtils.GetAssetPath (""));
            return dircAssert;
        }

        public static void DeleteStreamingOutPath () {
            DirectoryDelete (Application.streamingAssetsPath);
        }

        /// <summary>
        /// 检查输出目标
        /// </summary>
        public static void CheckstreamingAssetsPath () {
            string dircAssert = EditorUtils.GetFileStreamingOutAssetsPath ();
            if (!Directory.Exists (dircAssert)) {
                Directory.CreateDirectory (dircAssert);
            }

            Debug.Log (string.Format ("current BuildTarget ={0},path = {1} ", CUtils.platform, dircAssert));
        }

        public static void CheckDirectory (string fullPath) {
            if (!Directory.Exists (fullPath)) {
                Directory.CreateDirectory (fullPath);
            }
        }

        public static void DirectoryDelete (string path) {
            DirectoryInfo di = new DirectoryInfo (path);
            if (di.Exists) di.Delete (true);
        }

        public static List<string> getAllChildFiles (string path, string suffix = "lua", List<string> files = null, bool isMatch = true) {
            if (files == null) files = new List<string> ();
            if (!string.IsNullOrEmpty (path)) addFiles (path, suffix, files, isMatch);
            string[] dires = Directory.GetDirectories (path);
            foreach (string dirp in dires) {
                //            Debug.Log(dirp);
                getAllChildFiles (dirp, suffix, files, isMatch);
            }
            return files;
        }

        public static void addFiles (string direPath, string suffix, List<string> files, bool isMatch = true) {
            string[] fileMys = Directory.GetFiles (direPath);
            foreach (string f in fileMys) {
                if (System.Text.RegularExpressions.Regex.IsMatch (f, suffix) == isMatch) {
                    files.Add (f);
                }
            }
        }

        //判断追加crc到文件名，editor 专用。
        public static string InsertAssetBundleName (string assetbundleName, string insert) {
            var append = HugulaSetting.instance.appendCrcToFile;
            if (append) {
                var str = CUtils.InsertAssetBundleName (assetbundleName, insert);
                return str;
            } else {
                return assetbundleName;
            }

        }

        //select objects contains folder file
        public static UnityEngine.Object[] SelectObjects (params System.Type[] args) //System.Type[] filter = null)
        {
            if (args.Length == 0) args = new System.Type[] { typeof (UnityEngine.Object) };

            System.Func<Object, bool> checkType = (Object s) => {
                foreach (System.Type t in args) {
                    //  Debug.LogFormat("{0}={1},is={2}",s.GetType(),t,s.GetType().IsSubclassOf(t));
                    if (s.GetType ().IsSubclassOf (t) || s.GetType ().Equals (t)) return true;
                }
                return false;
            };

            List<UnityEngine.Object> re = new List<UnityEngine.Object> ();
            Object[] selection = UnityEditor.Selection.objects;
            string path = string.Empty;
            foreach (Object s in selection) {
                if (s is DefaultAsset && (path = AssetDatabase.GetAssetPath (s)) != null && Directory.Exists (path)) {
                    var import = AssetImporter.GetAtPath (path);
                    if (!string.IsNullOrEmpty (import.assetBundleName)) {
                        re.Add (s);
                    } else {
                        var allchildren = getAllChildFiles (path, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
                        foreach (var f in allchildren) {
                            var cpath = EditorUtils.GetAssetPath (f);
                            // Debug.Log(cpath);
                            var obj = AssetDatabase.LoadAssetAtPath (cpath, typeof (Object));
                            if (checkType (obj))
                                re.Add (obj);
                        }
                    }

                } else if (checkType (s)) {
                    re.Add (s);
                }
            }

            return re.ToArray ();
        }
    }

}