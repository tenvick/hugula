using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace PSDUINewImporter
{
    //------------------------------------------------------------------------------
    // class definition
    //------------------------------------------------------------------------------
    public class PSDImportMenu : Editor
    {
        [MenuItem("QuickTool/2. PSDNew Generate Ugui", false, 2)]
        static public void ImportPSD()
        {
            var key="2. PSDNew Generate Ugui Path Key";
            var lastPath = EditorPrefs.GetString(key,Application.dataPath);
            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", lastPath, "xml");
            var f = new FileInfo(inputFile);
            if (f.Exists)
            {
                EditorPrefs.SetString(key,f.DirectoryName);
                PSDComponentImportCtrl import = new PSDComponentImportCtrl(inputFile);
                import.BeginDrawUILayers();
            }

            GC.Collect();
        }

        [MenuItem("Assets/2. PSDNew Generate Ugui", false, 2)]
        static public void ImportSelectedPSD()
        {

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(assPath))
                {
                    var import = new PSDComponentImportCtrl(assPath);
                    import.BeginDrawUILayers();//
                }
            }
            GC.Collect();
        }

    }
}