// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

using Hugula.Utils;
using Hugula.Cryptograph;
using Debug = UnityEngine.Debug;
namespace Hugula.Editor
{
    public class ExportResources
    {

        public const string ConfigPath = EditorCommon.ConfigPath;//"Assets/Hugula/Config";

        #region osx lua

#if UNITY_EDITOR_OSX && (UNITY_ANDROID || UNITY_IPHONE)
    public static string OutLuaPath = CurrentRootFolder+"Assets/" + Common.LUACFOLDER + "/osx";
#elif UNITY_EDITOR_OSX && UNITY_STANDALONE_WIN
    public static string OutLuaPath = CurrentRootFolder+"Assets/" + Common.LUACFOLDER + "/win";
#elif UNITY_EDITOR_WIN
        public static string OutLuaPath = CurrentRootFolder + "Assets/" + Common.LUACFOLDER + "/win";
#else //默认平台
    public static string OutLuaPath = CurrentRootFolder+"Assets/" + Common.LUACFOLDER + "/osx";
#endif

        //lua bytes 输出目录
        // public static string OutLuaBytesPath = //"Assets/" + Common.LUACFOLDER + "/Resources";

#if UNITY_ANDROID
        public static string LuaTmpPath = "Assets/Tmp/" + Common.LUA_TMP_FOLDER;//"/Tmp/" + Common.LUACFOLDER + "/";
#endif

        public static string CurrentRootFolder
        {
            get
            {
                string dataPath = Application.dataPath;
                dataPath = dataPath.Replace("Assets", "");
                return dataPath;
            }
        }

        #endregion

        #region update
        /// <summary>
        /// Builds the asset bundles update A.
        /// </summary>
        public static void buildAssetBundlesUpdateAB()
        {
            EditorUtility.DisplayProgressBar("Generate FileList", "loading bundle manifest", 1 / 2);
            AssetDatabase.Refresh();
            string readPath = EditorUtils.GetFileStreamingOutAssetsPath();// 读取Streaming目录
            var u3dList = EditorUtils.getAllChildFiles(readPath, @"\.meta$|\.manifest$|\.DS_Store$|\.u$", null, false);
            List<string> assets = new List<string>();
            foreach (var s in u3dList)
            {
                string ab = EditorUtils.GetAssetPath(s); //s.Replace(readPath, "").Replace("/", "").Replace("\\", "");
                assets.Add(ab);
            }

            readPath = new System.IO.DirectoryInfo(EditorUtils.GetLuaBytesResourcesPath()).FullName;// 读取lua 目录
            u3dList = EditorUtils.getAllChildFiles(readPath, @"\.bytes$", null);
            foreach (var s in u3dList)
            {
                string ab = EditorUtils.GetAssetPath(s); //s.Replace(readPath, "").Replace("/", "").Replace("\\", "");
                assets.Add(ab);
            }

            EditorUtility.ClearProgressBar();
            CUtils.DebugCastTime("Time Generate FileList End");
            Debug.Log("all assetbundle count = " + assets.Count);
            BuildScript.GenerateAssetBundlesUpdateFile(assets.ToArray());
            CUtils.DebugCastTime("Time GenerateAssetBundlesUpdateFile End");
        }

        #endregion


        #region export

        public static void doExportLua(string rootPath)
        {
            EditorUtils.CheckstreamingAssetsPath();
            string OutLuaBytesPath = EditorUtils.GetLuaBytesResourcesPath();
            AssetDatabase.DeleteAsset(OutLuaBytesPath);
            EditorUtils.GetLuaBytesResourcesPath();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            var files = Directory.GetFiles(rootPath, "*.lua", SearchOption.AllDirectories);
            var dests = new string[files.Length];
            var dests64 = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string xfile = files[i].Remove(0, rootPath.Length+1);
                xfile = xfile.Replace("\\", "+").Replace("/","+");
                string xfile64 = CUtils.InsertAssetBundleName(xfile,"_64");

                string file = files[i].Replace("\\", "/");
                string dest = OutLuaBytesPath + "/" + CUtils.GetRightFileName(xfile);
                string destName = dest.Substring(0, dest.Length - 3) + "bytes";

                string dest64 = OutLuaBytesPath + "/" + CUtils.GetRightFileName(xfile64);
                string destName64 = dest64.Substring(0, dest64.Length - 3) + "bytes";


                files[i] = file;
                dests[i] = destName;
                dests64[i] = destName64;
                sb.AppendLine("[\"" + xfile + "\"]");
                sb.Append(" = {path = \"" + CUtils.GetRightFileName(xfile) + "\", ");
                sb.Append("out path64 = \"" + CUtils.GetRightFileName(xfile64) + "\"},");

                UnityEngine.Debug.Log(xfile + ":" + destName+" 64="+destName64);
            }

            #if UNITY_ANDROID || UNITY_IPHONE
            SLua.LuajitGen.compileLuaJit(files, dests64, SLua.JITBUILDTYPE.GC64);
            SLua.LuajitGen.compileLuaJit(files, dests, SLua.JITBUILDTYPE.X86);
            #else
            SLua.LuajitGen.compileLuaJit(files, dests64, SLua.JITBUILDTYPE.X64);
            SLua.LuajitGen.compileLuaJit(files, dests, SLua.JITBUILDTYPE.X86);
            #endif

            //out md5 mapping file
            string tmpPath = EditorUtils.GetAssetTmpPath();
            EditorUtils.CheckDirectory(tmpPath);
            string outPath = Path.Combine(tmpPath, "lua_md5mapping.txt");
            Debug.Log("write to path=" + outPath);
            using (StreamWriter sr = new StreamWriter(outPath, false))
            {
                sr.Write(sb.ToString());
            }

            EditorUtility.ClearProgressBar();
        }

        public static void exportLua()
        {
            // var childrens = AssetDatabase.GetAllAssetPaths().Where(p =>
            //    (p.StartsWith("Assets/Lua")
            //    || p.StartsWith("Assets/Config"))
            //    && (p.EndsWith(".lua"))
            //    ).ToArray();
            doExportLua("Assets/Lua");

        }

        public static void exportConfig()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
             p.StartsWith("Assets/Config") || !p.StartsWith("Assets/Config/Lan")
             && p.EndsWith(".csv")
             ).ToArray();

            EditorUtils.CheckstreamingAssetsPath();

            if (files.Length > 0)
            {
                string cname = CUtils.GetRightFileName(Common.CONFIG_CSV_NAME);
                BuildScript.BuildABs(files.ToArray(), null, cname, SplitPackage.DefaultBuildAssetBundleOptions);
                Debug.Log(" Config export " + cname);
            }

        }

        public static void exportLanguage()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
                p.StartsWith("Assets/Config/Lan")
                && p.EndsWith(".csv")
            ).ToArray();

            EditorUtils.CheckstreamingAssetsPath();
            // BuildScript.ch
            foreach (string abPath in files)
            {
                string name = CUtils.GetAssetName(abPath);
                string abName = CUtils.GetRightFileName(name + Common.CHECK_ASSETBUNDLE_SUFFIX);
                Hugula.BytesAsset bytes = (Hugula.BytesAsset)ScriptableObject.CreateInstance(typeof(Hugula.BytesAsset));
                bytes.bytes = File.ReadAllBytes(abPath);
                string bytesPath = string.Format("Assets/Tmp/{0}.asset", name);
                AssetDatabase.CreateAsset(bytes, bytesPath);
                BuildScript.BuildABs(new string[] { bytesPath }, null, abName, SplitPackage.DefaultBuildAssetBundleOptions);
                Debug.Log(name + " " + abName + " export");
            }
        }

        public static void exportPublish()
        {
            exportLua();
            CUtils.DebugCastTime("Time exportLua End");
            exportLanguage();
            //exportConfig();
            BuildScript.BuildAssetBundles(); //导出资源
            // CleanAssetbundle.Clean();        //清理多余的资源
            CUtils.DebugCastTime("Time BuildAssetBundles End");
            buildAssetBundlesUpdateAB();//更新列表和版本号码
            CUtils.DebugCastTime("Time buildAssetBundlesUpdateAB End");
        }

        #endregion

    }
}