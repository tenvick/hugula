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

            try
            {
                AssetDatabase.StartAssetEditing();

            for (int i = 0; i < files.Length; i++)
            {
                string xfile = files[i].Remove(0, rootPath.Length + 1);
                xfile = xfile.Replace("\\", ".").Replace("/", ".");
                string file = files[i].Replace("\\", "/");
                string dest = OutLuaBytesPath + "/" + CUtils.GetRightFileName(xfile);
#if USE_LUA_ZIP
                string destName = dest.Substring(0, dest.Length - 4);// + "bytes";
#else
                string destName = dest.Substring(0, dest.Length - 3) + "bytes";
#endif
                dests[i] = destName;
                System.IO.File.Copy(file, destName, true);
                // CopyEncrypt(file,destName);

                sb.AppendFormat("\r\n {0}   ({1}) ", file, destName);

            }

            string tmpPath = EditorUtils.GetAssetTmpPath();
            EditorUtils.CheckDirectory(tmpPath);

            EditorUtils.WriteToTmpFile("lua_export_log.txt",sb.ToString());

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
            File.WriteAllBytes(destName,bytes);
            return true;
        }

        public static string GetLuaBytesResourcesPath()
        {
            string luapath = $"{Common.LUACFOLDER}";
            DirectoryInfo p = new DirectoryInfo(luapath);
            if (!p.Exists) p.Create();
            return luapath;
        }
        
    }
}