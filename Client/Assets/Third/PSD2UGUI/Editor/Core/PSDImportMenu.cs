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
            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Path.Combine(Application.dataPath, PSDImporterConst.Globle_PSD_FOLDER), "xml");

            if (!string.IsNullOrEmpty(inputFile))
            {
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