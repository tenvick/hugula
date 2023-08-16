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
        [MenuItem("Hugula/Export Protobuf [Assets\\proto *.proto]", false, 14)]
        public static void Export()
        {
            LuaProtobufEditorTool.DoExport();
            LuaProtobufEditorTool.GenerateApiList();
        }

        // [MenuItem("Hugula/Lua Protobuf/1.Generate Lua Api List", false, 101)]
        // [MenuItem("Assets/Lua Protobuf/1.Generate Lua Api List", false, 201)]
        // public static void GenerateApiList()
        // {
        //     LuaProtobufEditorTool.GenerateApiList();
        // }
    }

    public class LuaProtobufEditorTool
    {
        // const string LUA_PROTO_GROUP_NAME = "proto_";

        readonly static string PROTO_PATH = Application.dataPath + "/proto";
        readonly static string PROTO_OUT_PATH = Application.dataPath + "/proto/bytes";

        public static void DoExport()
        {

            //copy
            EditorUtils.CheckstreamingAssetsPath();
            Directory.CreateDirectory(PROTO_OUT_PATH);
            foreach (string deleteFile in Directory.GetFileSystemEntries(PROTO_OUT_PATH))
            {
                File.Delete(deleteFile);
            }

            var files = Directory.GetFiles(PROTO_PATH, "*.proto", SearchOption.TopDirectoryOnly);

            var dests = new string[files.Length];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                string xfile = System.IO.Path.GetFileName(files[i]);//.Remove(0, rootPath.Length + 1);
                string file = files[i].Replace("\\", "/");
                string dest = PROTO_OUT_PATH + "/" + xfile;//.Replace(".", ".");
                string destName = dest + ".bytes";
                ExportProto(file, destName);
                dests[i] = "Assets/proto/bytes/" + xfile + ".bytes";
                sb.AppendFormat("\r\n {0}   ({1}) ", file, destName);
            }

            EditorUtils.WriteToTmpFile("protobuf_export_log.txt", sb.ToString());
            Debug.Log(sb.ToString());

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            //addressables
            var setting = AASEditorUtility.LoadAASSetting();
            var groupSchama = AASEditorUtility.DefaltGroupSchema[0];
            var group = AASEditorUtility.FindGroup(Common.PROTO_GROUP_NAME, groupSchama);//setting.FindGroup(LUA_GROUP_NAME);
            var labels = setting.GetLabels();
            if (!labels.Contains(Common.PROTO_GROUP_NAME))
            {
                setting.AddLabel(Common.PROTO_GROUP_NAME);
            }

            var packing = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            if (packing == null)
            {
                packing = group.AddSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            }

            packing.InternalIdNamingMode = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.AssetNamingMode.Filename;
            packing.BundleNaming = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundleNamingStyle.NoHash;
            packing.InternalBundleIdMode = UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema.BundleInternalIdMode.GroupGuid;
            packing.UseAssetBundleCrc = false;

            foreach (var str in dests)
            {
                var guid = AssetDatabase.AssetPathToGUID(str); //获得GUID
                var entry = setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
                Debug.Log($"guid:{guid},str:{str},entry:{entry}");
                entry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(str));
                entry.SetLabel(Common.PROTO_GROUP_NAME, true);
            }

            AssetDatabase.SaveAssets();
        }


        public static void GenerateApiList()
        {
            LoadPb();
            luaEnv.DoString("require('build_protobuf')");
            DisposeLua();
        }

        // public static void GenerateProtocInit()
        // {
        //     luaEnv.DoString("require('build_protobuf_init')");
        //     DisposeLua();
        // }

        private static void LoadPb()
        {
            string PROTO_PATH = Application.dataPath + "/proto";
            var files = Directory.GetFiles(PROTO_PATH, "*.proto", SearchOption.TopDirectoryOnly);
            LuaFunction loadFile = luaEnv.DoString(
                $@"
                local pc = require('protoc').new()
                pc.experimental_allow_proto3_optional = true
                pc:addpath('{PROTO_PATH}')
                return function (txt)
                    return pc:loadfile(txt)
                end"
            )[0] as LuaFunction;
            foreach (var pbFile in files)
            {
                loadFile.Func<string, string>(Path.GetFileName(pbFile));
            }
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
        static byte[] Loader(ref string name,ref int length)
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

        static void ExportProto(string source, string dest)
        {
            var argStr = string.Format("-o {0} {1} --proto_path={2}", dest, source, PROTO_PATH);
            // Debug.Log(argStr);
            System.Diagnostics.Process p = new System.Diagnostics.Process();

#if UNITY_EDITOR_WIN
            p.StartInfo.FileName = Application.dataPath + "/../tools/proto/protoc.exe";
#elif UNITY_EDITOR_OSX
            p.StartInfo.FileName = "protoc";
#endif
            p.StartInfo.Arguments = argStr;
            p.StartInfo.WorkingDirectory = PROTO_PATH;
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
                Debug.Log(source + " 导出成功!");
            }
            p.Close();
        }

    }
}