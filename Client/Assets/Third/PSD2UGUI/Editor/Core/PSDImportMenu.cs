using System;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace PSDUIImporter
{
    //------------------------------------------------------------------------------
    // class definition
    //------------------------------------------------------------------------------
    public class PSDImportMenu : Editor
    {
        [MenuItem("QuickTool/PSDImport ...", false, 1)]
        static public void ImportPSD()
        {
            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Path.Combine(Application.dataPath, PSDImporterConst.Globle_PSD_FOLDER), "xml");

            if (!string.IsNullOrEmpty(inputFile) &&
                inputFile.StartsWith(Application.dataPath))
            {
                PSDComponentImportCtrl import = new PSDComponentImportCtrl(inputFile);
                import.BeginDrawUILayers();
            }

            GC.Collect();
        }
    }
}