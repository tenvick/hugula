// Copyright (c) 2020 hugula
// direct https://github.com/Hugulor/Hugula
//
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Hugula;
using Hugula.Utils;
using HugulaEditor;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;
using HugulaEditor.Addressable;

namespace HugulaEditor
{
    public class LuaMenuItems
    {
        [MenuItem("Hugula/Export Lua [Assets\\Lua *.lua] %l", false, 12)]
        public static void Export()
        {
            LuaEditorTool.DoExportLua("Assets/Lua");
        }

        [MenuItem("Hugula/Build Export Lua [Assets\\Lua *.lua] ", false, 12)]
        public static void BuildExport()
        {
            var BuildLuaBundle = new HugulaEditor.ResUpdate.BuildLuaBundle();
            BuildLuaBundle.Run(null);
        }

        #region hugula debug
        const string kDebugLuaAssetBundlesMenu = "Hugula/Debug Lua";

        [MenuItem(kDebugLuaAssetBundlesMenu, false, 1)]
        public static void ToggleSimulateAssetBundle()
        {
            EnterLua.isDebug = !EnterLua.isDebug;
        }

        [MenuItem(kDebugLuaAssetBundlesMenu, true, 1)]
        public static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(kDebugLuaAssetBundlesMenu, EnterLua.isDebug);
            return true;
        }
        #endregion
    }


    public class LuaEditorTool
    {

        public const string LUA_GROUP_NAME = "lua_bundle";
        readonly static string LUA_ROOT_PATH = Application.dataPath + "/Lua";
        readonly static string LUA_OUT_PATH = Application.dataPath + "/lua_bundle";
        public static void DoExportLua(string rootPath)
        {
            EditorUtils.CheckstreamingAssetsPath();
            string OutLuaBytesPath = GetLuaBytesResourcesPath();
            // AssetDatabase.DeleteAsset(OutLuaBytesPath);
            HugulaEditor.EditorUtils.DirectoryDelete(OutLuaBytesPath);
            GetLuaBytesResourcesPath();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var files = Directory.GetFiles(rootPath, "*.lua", SearchOption.AllDirectories);
            var dests = new string[files.Length];
            var projectRoot = Application.dataPath.Replace("Assets", "");
            try
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < files.Length; i++)
                {
                    string xfile = files[i].Remove(0, rootPath.Length + 1);
                    xfile = xfile.Replace("\\", ".").Replace("/", ".");
                    string file = projectRoot + files[i].Replace("\\", "/");
                    string dest = LUA_OUT_PATH + "/" + xfile;
                    string destName = dest.Substring(0, dest.Length - 3) + "bytes";
                    dests[i] = destName;
                    LuacCMD(file, destName);
                    //System.IO.File.Copy(file, destName, true);
                    // CopyEncrypt(file,destName);

                    sb.AppendFormat("\r\n {0}   ({1}) ", file, destName);

                }

                string tmpPath = EditorUtils.GetAssetTmpPath();
                EditorUtils.CheckDirectory(tmpPath);

                EditorUtils.WriteToTmpFile("lua_export_log.txt", sb.ToString());

                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{ex.Message} \r\n {ex.StackTrace} ");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }



        }

        static bool CopyEncrypt(string file, string destName)
        {
            var bytes = Hugula.Cryptograph.CryptographHelper.EncryptDefault(File.ReadAllBytes(file));
            File.WriteAllBytes(destName, bytes);
            return true;
        }

        public static string GetLuaBytesResourcesPath()
        {
            string luapath = $"{Common.LUACFOLDER}";
            DirectoryInfo p = new DirectoryInfo(luapath);
            if (!p.Exists) p.Create();
            return luapath;
        }



        static void LuacCMD(string source, string dest)
        {
            var argStr = string.Format("-o {0} {1}", dest, source, LUA_ROOT_PATH);
            Debug.Log(argStr);
            System.Diagnostics.Process p = new System.Diagnostics.Process();

#if UNITY_EDITOR_WIN
            p.StartInfo.FileName = Application.dataPath + "/../tools/luaTools/win/535/luac.exe";
#elif UNITY_EDITOR_OSX
            p.StartInfo.FileName = Application.dataPath + "/../tools/luaTools/luac535;
#endif
            p.StartInfo.Arguments = argStr;
            p.StartInfo.WorkingDirectory = LUA_ROOT_PATH;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            string errorLog = p.StandardError.ReadToEnd();
            if (errorLog.Length > 0)
            {
                if (errorLog.ToLower().Contains("error"))
                {
                    Debug.LogError(errorLog);
                }
                else
                {
                    Debug.Log(errorLog);
                }
            }
            else
            {
                Debug.Log(dest + " 导出成功!");
            }
            p.Close();
        }
    }
}