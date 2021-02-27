// Copyright (c) 2020 hugula
// direct https://github.com/Hugulor/Hugula
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Hugula.Utils;
using Hugula;
using HugulaEditor;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace HugulaEditor
{
    public class LuaMenuItems
    {
        [MenuItem("Hugula/Export Lua [Assets\\Lua *.lua] %l", false, 12)]
        public static void Export()
        {
            LuaEditorTool.DoExportLua("Assets/Lua");
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
            AssetDatabase.DeleteAsset(OutLuaBytesPath);
            GetLuaBytesResourcesPath();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var files = Directory.GetFiles(rootPath, "*.lua", SearchOption.AllDirectories);
            var dests = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                string xfile = files[i].Remove(0, rootPath.Length + 1);
                xfile = "lua_" + xfile.Replace("\\", "+").Replace("/", "+");
                string file = files[i].Replace("\\", "/");
                string dest = OutLuaBytesPath + "/" + CUtils.GetRightFileName(xfile);
                string destName = dest.Substring(0, dest.Length - 3) + "bytes";
                dests[i] = destName;
                System.IO.File.Copy(file, destName, true);

                sb.AppendFormat("\r\n {0}   ({1}) ", file, destName);

            }

            string tmpPath = EditorUtils.GetAssetTmpPath();
            EditorUtils.CheckDirectory(tmpPath);
            string outPath = Path.Combine(tmpPath, "lua_export_log.txt");
            Debug.Log("write to path=" + outPath);
            using (StreamWriter sr = new StreamWriter(outPath, false))
            {
                sr.Write(sb.ToString());
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            //addressables
            var setting = AASEditorUtility.LoadAASSetting();
            var groupSchama = AASEditorUtility.DefaltGroupSchema[0];
            var group = AASEditorUtility.FindGroup(LUA_GROUP_NAME, groupSchama);//setting.FindGroup(LUA_GROUP_NAME);
            AASEditorUtility.ClearGroup(LUA_GROUP_NAME); //清空
            foreach (var str in dests)
            {
                var guid = AssetDatabase.AssetPathToGUID(str); //获得GUID
                var entry = setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
                entry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(str));
                entry.SetLabel("lua_script", true);
            }
        }


        public static string GetLuaBytesResourcesPath()
        {
            string luapath = "Assets/" + Common.LUACFOLDER + "/lua_bundle";
            DirectoryInfo p = new DirectoryInfo(luapath);
            if (!p.Exists) p.Create();
            return luapath;
        }
    }
}