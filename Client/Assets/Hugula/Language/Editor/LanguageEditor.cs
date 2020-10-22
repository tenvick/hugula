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
           public static void ExportLanguage()
        {
            var files = AssetDatabase.GetAllAssetPaths().Where(p =>
              p.StartsWith("Assets/Config/Lan") &&
              p.EndsWith(".csv")
            ).ToArray();

            EditorUtils.CheckstreamingAssetsPath();
            EditorUtils.CheckDirectory("Assets/Tmp/");
            //check dir
            
            // BuildScript.ch
            foreach (string abPath in files)
            {
                string name = CUtils.GetAssetName(abPath);
                string abName = CUtils.GetRightFileName(name + Common.CHECK_ASSETBUNDLE_SUFFIX);
                Hugula.BytesAsset bytes = (Hugula.BytesAsset)ScriptableObject.CreateInstance(typeof(Hugula.BytesAsset));
                bytes.bytes = File.ReadAllBytes(abPath);
                string bytesPath = string.Format("Assets/Tmp/{0}.asset", name);

                AssetDatabase.CreateAsset(bytes, bytesPath);
                // BuildScript.BuildABs(new string[] { bytesPath }, null, abName, SplitPackage.DefaultBuildAssetBundleOptions);
                Debug.Log(name + " " + abName + " export");
            }
        }
}

}