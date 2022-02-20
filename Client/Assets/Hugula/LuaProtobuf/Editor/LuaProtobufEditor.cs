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
using HugulaEditor;
using XLua;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace HugulaEditor
{
    public class LuaProtobufMenuItems
    {
        [MenuItem("Hugula/Export Lua Protobuf [Assets\\Lua\\proto *.proto]", false, 14)]
        public static void Export()
        {
            LuaProtobufEditorTool.DoExport();
        }

        [MenuItem("Hugula/Lua Protobuf/1.Generate Lua Api List", false, 101)]
        [MenuItem("Assets/Lua Protobuf/1.Generate Lua Api List", false, 201)]
        public static void GenerateApiList()
        {
            LuaProtobufEditorTool.GenerateApiList();

        }

        [MenuItem("Hugula/Lua Protobuf/2.Generate Lua protoc_init.lua", false, 102)]
        [MenuItem("Assets/Lua Protobuf/2.Generate Lua protoc_init.lua", false, 202)]
        public static void GenerateProtocInit()
        {
            LuaProtobufEditorTool.GenerateProtocInit();
        }
    }

    public class LuaProtobufEditorTool
    {
        const string LUA_PROTO_GROUP_NAME = "lua_proto";
        public static void DoExport()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
                 p.StartsWith("Assets/Lua/proto")
                  && (p.EndsWith(".proto") || p.EndsWith(".pb"))
               ).ToArray();

            //copy
            EditorUtils.CheckstreamingAssetsPath();
            string OutLuaProtobufPath = GetLuaProtobufResourcesPath();
            AssetDatabase.DeleteAsset(OutLuaProtobufPath);
            GetLuaProtobufResourcesPath();
            var dests = new string[files.Length];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                string xfile = System.IO.Path.GetFileName(files[i]);//.Remove(0, rootPath.Length + 1);
                string file = files[i].Replace("\\", "/");
                string dest = OutLuaProtobufPath + "/" + CUtils.GetRightFileName(xfile);//.Replace(".", ".");
                string destName = dest + ".bytes";
                dests[i] = destName;
                Debug.LogFormat("Copy({0},{1})", file, destName);
                System.IO.File.Copy(file, destName, true);
                sb.AppendFormat("\r\n {0}   ({1}) ", file, destName);
            }

            string tmpPath = EditorUtils.GetAssetTmpPath();
            EditorUtils.CheckDirectory(tmpPath);
            string outPath = Path.Combine(tmpPath, "lua_protobuf_export_log.txt");
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
            var group = AASEditorUtility.FindGroup(LUA_PROTO_GROUP_NAME, groupSchama);//setting.FindGroup(LUA_GROUP_NAME);

             //sync load
            var packing = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            if (packing == null)
            {
                packing = group.AddSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            }

            packing.InternalIdNamingMode = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.AssetNamingMode.Filename;
            packing.BundleNaming = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            packing.InternalBundleIdMode = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundleInternalIdMode.GroupGuid;

            var abProviderType = packing.AssetBundleProviderType;
            var assetProviderType = packing.BundledAssetProviderType;

            var syncABPType = typeof(SyncBundleProvider);
            var syncAssetPType = typeof(SyncBundledAssetProvider);

            if (!syncABPType.Equals(abProviderType.Value))
            {
                var targetType = new UnityEngine.ResourceManagement.Util.SerializedType();
                targetType.Value = syncABPType;
                EditorUtils.SetFieldValue(packing, "m_AssetBundleProviderType", targetType);
                Debug.Log($"set  AssetBundleProviderType={packing.AssetBundleProviderType.Value}");
            }

            if (!syncAssetPType.Equals(assetProviderType.Value))
            {
                var targetType = new UnityEngine.ResourceManagement.Util.SerializedType();
                targetType.Value = syncAssetPType;
                EditorUtils.SetFieldValue(packing, "m_BundledAssetProviderType", targetType);
                Debug.Log($"set  BundledAssetProviderType={targetType.Value} ,target={syncAssetPType}");
            }


            foreach (var str in dests)
            {
                var guid = AssetDatabase.AssetPathToGUID(str); //获得GUID
                var entry = setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
                entry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(str));
                // entry.SetLabel("lua_protobuf", true);
            }
        }

        public static void GenerateApiList()
        {
            luaEnv.DoString("require('build_protobuf')");
            DisposeLua();
        }

        public static void GenerateProtocInit()
        {
            luaEnv.DoString("require('build_protobuf_init')");
            DisposeLua();
        }

        public static string GetLuaProtobufResourcesPath()
        {
            string luapath = "Assets/LuaBytes/lua_proto";
            DirectoryInfo p = new DirectoryInfo(luapath);
            if (!p.Exists) p.Create();
            return luapath;
        }

        private static LuaEnv m_LuaEnv;

        public static LuaEnv luaEnv
        {
            get
            {
                if (m_LuaEnv == null)
                {
                    m_LuaEnv = new LuaEnv();
                    m_LuaEnv.AddLoader(Loader);
                }
                return m_LuaEnv;
            }
        }


        public static void DisposeLua()
        {
            if (m_LuaEnv != null) m_LuaEnv.Dispose();
            m_LuaEnv = null;
        }
        /// <summary>
        ///  loader
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static byte[] Loader(ref string name)
        {
            byte[] str = null;
            string name1 = name.Replace('.', '/');
            string path = Application.dataPath + "/Hugula/LuaProtobuf/Editor/" + name1 + ".lua";
            if (File.Exists(path))
            {
                str = File.ReadAllBytes(path);
            }
            else
            {
                path = Application.dataPath + "/Lua/" + name1 + ".lua";
                str = File.ReadAllBytes(path);
                // Debug.LogErrorFormat("the file({0},{1}) did't exist. ", name, path);
            }

            return str;
        }
    }
}