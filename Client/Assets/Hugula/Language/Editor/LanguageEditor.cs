// Copyright (c) 2020 hugula
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
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace HugulaEditor
{

    public class LanMenuItems
    {
        [MenuItem("Hugula/Export Language [Assets\\Config\\Lan *.csv]", false, 15)]
        public static void Export()
        {
            LanguageEditorTools.ExportLanguage();
        }
    }

    public class LanguageEditorTools
    {
        public const string LAN_GROUP_NAME = "lan_bundle";

        public static void ExportLanguage()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
              p.StartsWith("Assets/Config/Lan") &&
              p.EndsWith(".csv")
            ).ToArray();

            // EditorUtils.CheckstreamingAssetsPath();
            EditorUtils.CheckDirectory("Assets/LuaBytes/lan_bundle");
            var dests = new List<string>();

            foreach (string abPath in files)
            {
                string name = CUtils.GetAssetName(abPath);
                string abName = CUtils.GetRightFileName(name + Common.CHECK_ASSETBUNDLE_SUFFIX);
                Hugula.BytesAsset bytes = (Hugula.BytesAsset)ScriptableObject.CreateInstance(typeof(Hugula.BytesAsset));
                bytes.bytes = File.ReadAllBytes(abPath);
                string bytesPath = string.Format("Assets/LuaBytes/lan_bundle/{0}.asset", name);
                dests.Add(bytesPath);
                AssetDatabase.CreateAsset(bytes, bytesPath);

                // BuildScript.BuildABs(new string[] { bytesPath }, null, abName, SplitPackage.DefaultBuildAssetBundleOptions);
                // Debug.Log(name + " " + abName + " export");
            }

            AssetDatabase.Refresh();

            var setting = AASEditorUtility.LoadAASSetting();
            var group = AASEditorUtility.FindGroup(LAN_GROUP_NAME, AASEditorUtility.DefaltGroupSchema[0]);//setting.FindGroup(LUA_GROUP_NAME);

            foreach (var str in dests)
            {
                var guid = AssetDatabase.AssetPathToGUID(str); //获得GUID

                var entry = setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
                entry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(str));
                entry.SetLabel("lan_csv", true);
            }

        }
    }

}