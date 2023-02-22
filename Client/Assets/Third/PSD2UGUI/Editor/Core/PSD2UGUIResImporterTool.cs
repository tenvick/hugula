using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace PSDUINewImporter
{

    public class PSD2UGUIResCopyTool : EditorWindow
    {

        [MenuItem("QuickTool/1. PSD2UGUIResCopyTool", false, 1)]
        static void Init()
        {
            var window = EditorWindow.GetWindow<PSD2UGUIResCopyTool>("Image & XML Res Copy tool");
            window.Show();
        }

        //右键方式导入资源
        [MenuItem("Assets/1. PSD2UGUIResCopyTool", false, 1)]
        static void CopyResFormOutside()
        {

            if (ChooseFolder(true))
            {
                foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
                {
                    var assPath = AssetDatabase.GetAssetPath(obj);
                    var objName = obj.name.ToLower();
                    var psdUI = (PSDUI)PSDImportUtility.DeserializeXml(assPath);
                    BeginCopy(psdUI);
                    AssetDatabase.Refresh();
                    if (PSDImporterConst.PSDUI_SETTING_TEXTRUE)
                    {
                        var psdCtrl = new PSDComponentImportCtrl();
                        psdCtrl.LoadLayers(psdUI);//设置图片
                    }
                }
            }

        }

        [MenuItem("Assets/2. PSDNew Generate Ugui", true, 2)]
        [MenuItem("Assets/1. PSD2UGUIResCopyTool", true, 1)]
        static bool ValidateXMLFile()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                if (Path.GetExtension(AssetDatabase.GetAssetPath(obj)) == ".xml")
                {
                    return true;
                }
            }
            return false;
        }

        static Dictionary<string, string> baseFolderFiles;
        string textInfo = string.Empty;
        static StringBuilder sb = new StringBuilder();
        static string lastSourcePath = string.Empty;
        static string lastTargetPath = string.Empty;

        #region window
        void OnEnable()
        {
            PSDImporterConst.LoadConfig();
        }

        void OnDisable()
        {

        }

        void OnGUI()
        {
            DrawToolbar();
        }

        private void DrawToolbar()
        {

            EditorGUILayout.BeginHorizontal();
            var toolbarHeight = GUILayout.Height(25);
            GUILayout.Space(25);

            GUILayout.Label("资源copy工具=>", toolbarHeight);
            if (GUILayout.Button("Choose Source Folder", EditorStyles.miniButton, GUILayout.Width(300), toolbarHeight))
            {
                if (ChooseFolder())
                {
                    BeginCopy();
                    textInfo = sb.ToString();
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15);
            if (!string.IsNullOrEmpty(textInfo))
            {
                ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.Height(position.height - 30));
                EditorGUILayout.TextArea(textInfo, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }

        }

        #endregion
        const string choosePath1Key = "PSD2ugui_LASET_choose_Path1";
        const string choosePath2Key = "PSD2ugui_LASET_choose_Path2";

        static string choosePath1, choosePath2;
        static bool ChooseFolder(bool defaultTargetPath = false)
        {
            PSDImporterConst.LoadConfig();
            PSDDirectoryUtility.ClearCache();
            lastSourcePath = EditorPrefs.GetString(choosePath1Key, PSDImporterConst.Globle_BASE_FOLDER);
        sourceFolderSelectBegin:
            string _path = EditorUtility.OpenFolderPanel("原始资源目录", lastSourcePath, string.Empty).Replace('\\', '/');

            if (string.IsNullOrEmpty(_path))
            {
                if (EditorUtility.DisplayDialog("警告", "必须选择原始目录", "确定"))
                    goto sourceFolderSelectBegin;
                else
                    return false;
            }

            EditorPrefs.SetString(choosePath1Key, _path);

            choosePath1 = _path;

            string _path2 = string.Empty;
            if (defaultTargetPath) //采用默认路径存放图片
            {
                _path2 =   PSDImporterConst.DEFAULT_IMAGE_PATH;
            }
            else
            {

                lastTargetPath = EditorPrefs.GetString(choosePath2Key, PSDImporterConst.Globle_BASE_FOLDER);
                if (!lastTargetPath.Contains(PSDImporterConst.Globle_BASE_FOLDER))
                {
                    lastTargetPath = PSDImporterConst.Globle_BASE_FOLDER;
                }

            targetFolderSelectPath:
                _path2 = EditorUtility.OpenFolderPanel("选择工程内部图片存放目录", lastTargetPath, string.Empty).Replace('\\', '/');

                if (!_path2.Contains(PSDImporterConst.Globle_BASE_FOLDER))
                {
                    if (EditorUtility.DisplayDialog("警告", $"目标目录必须在{PSDImporterConst.Globle_BASE_FOLDER}下", "确定"))
                        goto targetFolderSelectPath;
                    else
                        return false;
                }

                EditorPrefs.SetString(choosePath2Key, _path2);
            }

            AssetDatabase.Refresh();
            choosePath2 = _path2;

            sb.Clear();
            baseFolderFiles = ForeachFolderAndCachName(PSDImporterConst.Globle_BASE_FOLDER);
            errSb.Clear();

            return true;
        }


        static void BeginCopy()
        {
            CopyFolder(choosePath1, choosePath2);
            AssetDatabase.Refresh();
            sb.Append(errSb);
            HugulaEditor.EditorUtils.WriteToTmpFile("PSD2UGUIResImport_Log.txt", sb.ToString());
        }

        #region  xml文件方式导入图片
        ///以xml文件方式导入
        static void BeginCopy(PSDUI psdUI)
        {
            Layer layer = null;
            var len = psdUI.layers.Length;
            for (int layerIndex = 0; layerIndex < len; layerIndex++)
            {
                layer = psdUI.layers[layerIndex];
                ImportLayerImage(layer, choosePath1, choosePath2);
            }
            sb.Append(errSb);
            HugulaEditor.EditorUtils.WriteToTmpFile("PSD2UGUIResImport_selected_Log.txt", sb.ToString());
        }

        /// <summary>
        /// 导入图片
        /// </summary>
        /// <param name="layer"></param>
        static void ImportLayerImage(Layer layer, string sourcePath, string targetPath)
        {
            if (layer.type == ComponentType.Image || layer.type == "img")//如果是图片
            {
                //寻找目标
                var fileName = layer.name + ".png";
                if (!baseFolderFiles.TryGetValue(fileName, out var sourPath))//如果没有目标图片
                {
                    var found = PSDDirectoryUtility.FindFileInDirectory(sourcePath, fileName);
                    if (string.IsNullOrEmpty(found))
                    {
                        errSb.AppendLine($"\r\n        {fileName} 不存在于目录      {sourcePath}!");
                    }
                    else
                    {
                        var targetFile = Path.Combine(targetPath, fileName);
                        if (File.Exists(targetPath))
                        {
                            File.Delete(targetPath);
                            sb.AppendLine($"\r\n目标文件:{targetPath}存在,已经删除！");
                            Debug.LogWarning($"    目标文件:{targetPath}存在,已经删除！");
                        }
                        File.Copy(found, targetFile);
                        baseFolderFiles[fileName] = targetFile; //标记为已经导入
                        sb.AppendLine($"{fileName},    copy to:    {targetFile}");
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    ImportLayerImage(layer.layers[layerIndex], sourcePath, targetPath);
                }
            }
        }

        #endregion


        Vector2 ScrollPos;
        static StringBuilder errSb = new StringBuilder();
        static void CopyFolder(string source, string target)
        {
            Debug.Log($"Copy foler {source} to {target}");
            sb.AppendLine($"开始导入 \r\n原始文件夹：{source} \r\n目标文件夹：{target} \r\n {System.DateTime.Now.ToString()} \r\n ");

            var files = FilterFiles(source, new string[] { "png", "xml" }, SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file);
                var extension = Path.GetExtension(fileName);
                if (!baseFolderFiles.TryGetValue(fileName, out var sourPath) || extension == ".xml")//如果是xml需要覆盖
                {
                    var targetPath = Path.Combine(target, fileName);
                    if (File.Exists(targetPath) && extension == ".xml")
                    {
                        File.Delete(targetPath);
                        sb.AppendLine($"\r\n目标文件:{targetPath}存在,已经删除！");
                        Debug.LogWarning($"    目标文件:{targetPath}存在,已经删除！");
                    }
                    File.Copy(file, targetPath);
                    baseFolderFiles[fileName] = targetPath; //标记为已经导入
                    sb.AppendLine($"{fileName},    copy to:    {targetPath}");
                }
                else
                {
                    Debug.LogWarning($"{fileName}已经存在于{sourPath}本次不需要导入");
                    errSb.AppendLine($"\r\n        {fileName}已经存在于{sourPath}本次不需要导入!");
                }
            }
        }

        static Dictionary<string, string> ForeachFolderAndCachName(string folder)
        {
            folder = folder.Replace("\\", "/");
            Debug.Log(folder);
            var dic = new Dictionary<string, string>();
            var files = Directory.GetFiles(folder, "*.png", SearchOption.AllDirectories);
            //sb.AppendLine($" ForeachFolderAndCachName目录：{folder} 数量{files.Length}");
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file);
                dic[fileName] = file;
                // sb.AppendLine($" 原始文件：{fileName} 目录：{file}");
            }
            return dic;
        }

        static public string[] FilterFiles(string path, string[] exts, SearchOption searchOption = SearchOption.AllDirectories)
        {
            return
                Directory
                .EnumerateFiles(path, "*.*", searchOption)
                .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase))).ToArray();
        }


    }

}