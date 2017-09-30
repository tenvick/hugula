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
#if UNITY_IPHONE
    public static string luajit32Path=CurrentRootFolder+"tools/luaTools/luajit2.1";
	public static string luajit64Path=CurrentRootFolder+"tools/luaTools/luajit64";
#elif UNITY_ANDROID && UNITY_EDITOR_OSX
	public static string luajit32Path=CurrentRootFolder+"tools/luaTools/luajit2.1";
    public static string luajit64Path="";
#elif UNITY_ANDROID && UNITY_EDITOR_WIN
        public static string luajit32Path = CurrentRootFolder + "tools/luaTools/win/210/luajit.exe";
        public static string luajit64Path = "";
#elif UNITY_STANDALONE_WIN && UNITY_EDITOR_WIN //pc版本
    public static string luajit32Path = CurrentRootFolder+"tools/luaTools/win/204/luajit.exe";
    public static string luajit64Path="";
#elif UNITY_STANDALONE_WIN && UNITY_EDITOR_OSX //pc版本
	public static string luajit32Path=CurrentRootFolder+"tools/luaTools/luajit2.04";
    public static string luajit64Path = "";
#elif UNITY_STANDALONE_OSX
    public static string luajit32Path=CurrentRootFolder+"tools/luaTools/luac";
    public static string luajit64Path = "";
#else
    public static string luajit32Path = "";
    public static string luajit64Path = "";
#endif

#if UNITY_EDITOR_WIN //win
    public static string luaWorkingPath = CurrentRootFolder + "tools/luaTools/win";
    public static string luacPath = CurrentRootFolder + "tools/luaTools/win/204/luajit.exe";
#elif UNITY_EDITOR_OSX && UNITY_IPHONE //iOS on mac
    public static string luaWorkingPath = CurrentRootFolder+"tools/luaTools";
    public static string luacPath = "";
#elif UNITY_STANDALONE_WIN && UNITY_EDITOR_OSX //win on mac
    public static string luaWorkingPath = CurrentRootFolder+"tools/luaTools";
    public static string luacPath = CurrentRootFolder+"tools/luaTools/luajit2.04";
#else // mac 
    public static string luaWorkingPath = CurrentRootFolder+"tools/luaTools";
    public static string luacPath = CurrentRootFolder+"tools/luaTools/luac";
#endif

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

        private static Process CreateProcess(string Arguments, string FileName)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = Arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            // startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = luaWorkingPath;
            startInfo.FileName = FileName;
            process.StartInfo = startInfo;
            return process;
        }

        public static void doExportLua(string[] childrens)
        {
            EditorUtils.CheckstreamingAssetsPath();

            string info = "luac";
            string title = "build lua";
            EditorUtility.DisplayProgressBar(title, info, 0);

            var checkChildrens = AssetDatabase.GetAllAssetPaths().Where(p =>
                (p.StartsWith("Assets/Lua")
                || p.StartsWith("Assets/Config"))
                && (p.EndsWith(".lua"))
                ).ToArray();
            string path = "Assets/Lua/"; //lua path
            string path1 = "Assets/Config/"; //config path
            string root = CurrentRootFolder;//Application.dataPath.Replace("Assets", "");

            string crypName = "", crypEditorName = "", fileName = "", outfilePath = "", arg = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //refresh directory
            if (checkChildrens.Length == childrens.Length) EditorUtils.DirectoryDelete(OutLuaPath);
            EditorUtils.CheckDirectory(OutLuaPath);

            float allLen = childrens.Length;
            float i = 0;

            Debug.Log("luajit32Path:" + luajit32Path);
            Debug.Log("luajit64Path:" + luajit64Path);
            Debug.Log("luacPath:" + luacPath);

            string OutLuaBytesPath = EditorUtils.GetLuaBytesResourcesPath();
            string luabytesParentPath = OutLuaBytesPath.Substring(0, OutLuaBytesPath.LastIndexOf("/"));
            string streamingAssetsPath = Path.Combine(CurrentRootFolder, OutLuaBytesPath); //Path.Combine(CurrentRootFolder, LuaTmpPath);
            EditorUtils.DirectoryDelete(luabytesParentPath);
            EditorUtils.CheckDirectory(luabytesParentPath);
            EditorUtils.CheckDirectory(streamingAssetsPath);
            Debug.Log(streamingAssetsPath);
            List<System.Diagnostics.Process> listPc = new List<System.Diagnostics.Process>();

            List<string> luabytesAssets32 = new List<string>();
            List<string> luabytesAssets64 = new List<string>();

            foreach (string file in childrens)
            {
                string filePath = Path.Combine(root, file);
                fileName = CUtils.GetAssetName(filePath);
                crypName = file.Replace(path, "").Replace(path1, "").Replace(".lua", ".bytes").Replace("\\", "+").Replace("/", "+");
                crypEditorName = file.Replace(path, "").Replace(path1, "").Replace(".lua", "." + Common.ASSETBUNDLE_SUFFIX).Replace("\\", "+").Replace("/", "+");
                if (!string.IsNullOrEmpty(luajit32Path))// luajit32
                {
                    string override_name = CUtils.GetRightFileName(crypName);
                    string override_lua = streamingAssetsPath + "/" + override_name;
                    arg = "-b " + filePath + " " + override_lua; //for jit
                    // Debug.Log(arg);
                    listPc.Add(CreateProcess(arg, luajit32Path));
                    luabytesAssets32.Add(Path.Combine(OutLuaBytesPath, override_name));
                    sb.AppendLine("[\"" + crypName + "\"] = { name = \"" + override_name + "\", path = \"" + file + "\", out path = \"" + override_lua + "\"},");
                }
                if (!string.IsNullOrEmpty(luajit64Path)) //luajit64
                {
                    string crypName_64 = CUtils.InsertAssetBundleName(crypName, "_64");
                    string override_name = CUtils.GetRightFileName(crypName_64);
                    string override_lua = streamingAssetsPath + "/" + override_name;
                    arg = "-b " + filePath + " " + override_lua; //for jit
                    //  Debug.Log(arg);
                    listPc.Add(CreateProcess(arg, luajit64Path));
                    luabytesAssets64.Add(Path.Combine(OutLuaBytesPath, override_name));
                    sb.AppendLine("[\"" + crypName_64 + "\"] = { name = \"" + override_name + "\", path = \"" + file + "\", out path = \"" + override_lua + "\"},");
                }
                if (!string.IsNullOrEmpty(luacPath)) //for editor
                {
                    string override_name = CUtils.GetRightFileName(crypEditorName); //CUtils.GetRightFileName(CUtils.InsertAssetBundleName(crypName,"_64"));
                    string override_lua = OutLuaPath + "/" + override_name;
#if UNITY_EDITOR_OSX  && !UNITY_STANDALONE_WIN  
                    arg="-o "+override_lua+" "+filePath; //for lua
#else
                    arg = "-b " + filePath + " " + override_lua; //for jit
#endif
                    // Debug.Log(arg);
                    listPc.Add(CreateProcess(arg, luacPath));
                    sb.AppendLine("[\"" + crypEditorName + "(editor)\"] = { name = \"" + override_name + "\", path = \"" + file + "\", out path = \"" + override_lua + "\"},");
                }
                i++;
                // EditorUtility.DisplayProgressBar(title, info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
            }

            //compile lua
            int total = listPc.Count;
            int workThreadCount = System.Environment.ProcessorCount * 2 + 2;
            int batchCount = (int)System.Math.Ceiling(total / (float)workThreadCount);
            for (int batchIndex = 0; batchIndex < batchCount; ++batchIndex)
            {
                int processIndex;
                int offset = batchIndex * workThreadCount;
                for (processIndex = 0; processIndex < workThreadCount; ++processIndex)
                {
                    int fileIndex = offset + processIndex;
                    if (fileIndex >= total)
                        break;
                    var ps = listPc[fileIndex];
                    ps.Start();
                }

                bool fail = false;
                fileName = null;
                string arguments = null;
                for (int j = offset; j < offset + processIndex; ++j)
                {
                    var ps = listPc[j];
                    ps.WaitForExit();

                    EditorUtility.DisplayProgressBar(title, info + "=>" + j.ToString() + "/" + total.ToString(), j / total);

                    if (ps.ExitCode != 0 && !fail)
                    {
                        fail = true;
                        fileName = ps.StartInfo.FileName;
                        arguments = ps.StartInfo.Arguments;
                    }
                    ps.Dispose();
                }

                if (fail)
                {
                    throw new System.Exception(string.Format("Luajit Compile Fail.FileName={0},Arg={1}", fileName, arguments));
                }
            }

            Debug.Log("lua:" + path + "files=" + childrens.Length + " completed");
            System.Threading.Thread.Sleep(100);

           // AssetDatabase.Refresh();
            //all luabytes in one asset
            // var luaBytesAsset = ScriptableObject.CreateInstance<LuaBytesAsset>();
            // foreach (var file in luabytesAssets32)
            // {
            //     var bytes = File.ReadAllBytes(file);
            //     var fn = CUtils.GetAssetName(file);
            //     luaBytesAsset.GenerateBytes(bytes, fn);
            //     Debug.LogFormat("lua 32 bytes name ={0},len={1}", fn, bytes.Length);
            // }

            // string luaAssetPath = Path.Combine(luabytesParentPath, Common.LUA_BUNDLE_NAME_X86 + ".asset");
            // AssetDatabase.DeleteAsset(luaAssetPath);
            // AssetDatabase.CreateAsset(luaBytesAsset, luaAssetPath);
            // Debug.LogFormat("lua32:{0}", luaAssetPath);

            // if (luabytesAssets64.Count > 0)
            // {
            //     var luaBytesAsset64 = ScriptableObject.CreateInstance<LuaBytesAsset>();
            //     foreach (var file in luabytesAssets64)
            //     {
            //         var bytes = File.ReadAllBytes(file);
            //         var fn = CUtils.GetAssetName(file);
            //         luaBytesAsset.GenerateBytes(bytes, fn);
            //         Debug.LogFormat("lua 64 bytes name ={0},len={1}", fn, bytes.Length);
            //     }

            //     luaAssetPath = Path.Combine(luabytesParentPath, Common.LUA_BUNDLE_NAME_X64 + ".asset");
            //     AssetDatabase.DeleteAsset(luaAssetPath);
            //     AssetDatabase.CreateAsset(luaBytesAsset64, luaAssetPath);
            //     Debug.LogFormat("lua64:{0}", luaAssetPath);
            // }

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
            var childrens = AssetDatabase.GetAllAssetPaths().Where(p =>
               (p.StartsWith("Assets/Lua")
               || p.StartsWith("Assets/Config"))
               && (p.EndsWith(".lua"))
               ).ToArray();
            doExportLua(childrens);
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