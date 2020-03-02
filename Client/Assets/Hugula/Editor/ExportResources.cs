// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Hugula.Cryptograph;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace Hugula.Editor {
    public class ExportResources {

        public const string ConfigPath = EditorCommon.ConfigPath; //"Assets/Hugula/Config";

        #region osx lua

#if UNITY_EDITOR_OSX && (UNITY_ANDROID || UNITY_IPHONE)
        public static string OutLuaPath = CurrentRootFolder + "Assets/" + Common.LUACFOLDER + "/osx";
#elif UNITY_EDITOR_OSX && UNITY_STANDALONE_WIN
        public static string OutLuaPath = CurrentRootFolder + "Assets/" + Common.LUACFOLDER + "/win";
#elif UNITY_EDITOR_WIN
        public static string OutLuaPath = CurrentRootFolder + "Assets/" + Common.LUACFOLDER + "/win";
#else //默认平台
        public static string OutLuaPath = CurrentRootFolder + "Assets/" + Common.LUACFOLDER + "/osx";
#endif

        //lua bytes 输出目录
        // public static string OutLuaBytesPath = //"Assets/" + Common.LUACFOLDER + "/Resources";

#if UNITY_ANDROID
        public static string LuaTmpPath = "Assets/Tmp/" + Common.LUA_TMP_FOLDER; //"/Tmp/" + Common.LUACFOLDER + "/";
#endif

        public static string CurrentRootFolder {
            get {
                string dataPath = Application.dataPath;
                dataPath = dataPath.Replace ("Assets", "");
                return dataPath;
            }
        }

        #endregion

        #region update
        /// <summary>
        /// Builds the asset bundles update A.
        /// </summary>
        public static void buildAssetBundlesUpdateAB () {
            EditorUtility.DisplayProgressBar ("Generate FileList", "loading bundle manifest", 1 / 2);
            AssetDatabase.Refresh ();
            string readPath = EditorUtils.GetFileStreamingOutAssetsPath (); // 读取Streaming目录
            var u3dList = EditorUtils.getAllChildFiles (readPath, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
            List<string> assets = new List<string> ();
            foreach (var s in u3dList) {
                string ab = EditorUtils.GetAssetPath (s); //s.Replace(readPath, "").Replace("/", "").Replace("\\", "");
                assets.Add (ab);
            }

            readPath = new System.IO.DirectoryInfo (EditorUtils.GetLuaBytesResourcesPath ()).FullName; // 读取lua 目录
            u3dList = EditorUtils.getAllChildFiles (readPath, @"\.bytes$", null);
            foreach (var s in u3dList) {
                string ab = EditorUtils.GetAssetPath (s); //s.Replace(readPath, "").Replace("/", "").Replace("\\", "");
                assets.Add (ab);
            }

            EditorUtility.ClearProgressBar ();
            CUtils.DebugCastTime ("Time Generate FileList End");
            Debug.Log ("all assetbundle count = " + assets.Count);
            BuildScript.GenerateAssetBundlesUpdateFile (assets.ToArray ());
            CUtils.DebugCastTime ("Time GenerateAssetBundlesUpdateFile End");
        }

        #endregion

        #region export

        public static void doExportLua (string rootPath) {
            EditorUtils.CheckstreamingAssetsPath ();
            string OutLuaBytesPath = EditorUtils.GetLuaBytesResourcesPath ();
            AssetDatabase.DeleteAsset (OutLuaBytesPath);
            EditorUtils.GetLuaBytesResourcesPath ();

            System.Text.StringBuilder sb = new System.Text.StringBuilder ();

            var files = Directory.GetFiles (rootPath, "*.lua", SearchOption.AllDirectories);
            var dests = new string[files.Length];
            var dests64 = new string[files.Length];

            for (int i = 0; i < files.Length; i++) {
                string xfile = files[i].Remove (0, rootPath.Length + 1);
                xfile = xfile.Replace ("\\", "+").Replace ("/", "+");
                string file = files[i].Replace ("\\", "/");
                string dest = OutLuaBytesPath + "/" + CUtils.GetRightFileName (xfile);
                string destName = dest.Substring (0, dest.Length - 3) + "bytes";

                System.IO.File.Copy (file, dest, true);

                sb.AppendFormat ("\r\n {0}   ({1}) ", file, destName);

            }

            string tmpPath = EditorUtils.GetAssetTmpPath ();
            EditorUtils.CheckDirectory (tmpPath);
            string outPath = Path.Combine (tmpPath, "lua_export_log.txt");
            Debug.Log ("write to path=" + outPath);
            using (StreamWriter sr = new StreamWriter (outPath, false)) {
                sr.Write (sb.ToString ());
            }

            EditorUtility.ClearProgressBar ();
        }

        public static void exportLua () {
            // var childrens = AssetDatabase.GetAllAssetPaths().Where(p =>
            //    (p.StartsWith("Assets/Lua")
            //    || p.StartsWith("Assets/Config"))
            //    && (p.EndsWith(".lua"))
            //    ).ToArray();
            doExportLua ("Assets/Lua");

        }

        public static void exportConfig () {
            var files = AssetDatabase.GetAllAssetPaths ().Where (p =>
                p.StartsWith ("Assets/Config") || !p.StartsWith ("Assets/Config/Lan") &&
                p.EndsWith (".csv")
            ).ToArray ();

            EditorUtils.CheckstreamingAssetsPath ();

            if (files.Length > 0) {
                string cname = CUtils.GetRightFileName (Common.CONFIG_CSV_NAME);
                BuildScript.BuildABs (files.ToArray (), null, cname, SplitPackage.DefaultBuildAssetBundleOptions);
                Debug.Log (" Config export " + cname);
            }

        }

        public static void exportLanguage () {
            var files = AssetDatabase.GetAllAssetPaths ().Where (p =>
                p.StartsWith ("Assets/Config/Lan") &&
                p.EndsWith (".csv")
            ).ToArray ();

            EditorUtils.CheckstreamingAssetsPath ();
            // BuildScript.ch
            foreach (string abPath in files) {
                string name = CUtils.GetAssetName (abPath);
                string abName = CUtils.GetRightFileName (name + Common.CHECK_ASSETBUNDLE_SUFFIX);
                Hugula.BytesAsset bytes = (Hugula.BytesAsset) ScriptableObject.CreateInstance (typeof (Hugula.BytesAsset));
                bytes.bytes = File.ReadAllBytes (abPath);
                string bytesPath = string.Format ("Assets/Tmp/{0}.asset", name);
                AssetDatabase.CreateAsset (bytes, bytesPath);
                BuildScript.BuildABs (new string[] { bytesPath }, null, abName, SplitPackage.DefaultBuildAssetBundleOptions);
                Debug.Log (name + " " + abName + " export");
            }
        }

        public static void exportPublish () {
            exportLua ();
            CUtils.DebugCastTime ("Time exportLua End");
            exportLanguage ();
            //exportConfig();
            BuildScript.BuildAssetBundles (); //导出资源
            // CleanAssetbundle.Clean();        //清理多余的资源
            CUtils.DebugCastTime ("Time BuildAssetBundles End");
            buildAssetBundlesUpdateAB (); //更新列表和版本号码
            CUtils.DebugCastTime ("Time buildAssetBundlesUpdateAB End");
        }

        #endregion

    }
}