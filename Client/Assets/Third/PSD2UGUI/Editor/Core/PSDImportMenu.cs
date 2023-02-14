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
        [MenuItem("QuickTool/2. PSDNewImport ...", false, 2)]
        static public void ImportPSD()
        {
            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import" ,Path.Combine(Application.dataPath ,PSDImporterConst.Globle_PSD_FOLDER), "xml");

            if (!string.IsNullOrEmpty(inputFile))
            {
                PSDComponentImportCtrl import = new PSDComponentImportCtrl(inputFile);
               import.BeginDrawUILayers();
            }            

            GC.Collect();
        }
    }
}