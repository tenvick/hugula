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
        [MenuItem("QuickTool/1. PSD2UGUIResCopyTool", false,1)]
        static void Init()
        {
            var window = EditorWindow.GetWindow<PSD2UGUIResCopyTool>("Image & XML Res Copy tool");
            window.Show();
        }

        Dictionary<string, string> baseFolderFiles;
        string textInfo = string.Empty;
        StringBuilder sb = new StringBuilder();
        string lastSourcePath = string.Empty;
        string lastTargetPath = string.Empty;
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
                string _path = EditorUtility.OpenFolderPanel("原始资源目录", lastSourcePath, string.Empty).Replace('\\', '/');

                if(string.IsNullOrEmpty(_path))
                {
                    EditorUtility.DisplayDialog("警告", "必须选择原始目录", "确定");
                    return;
                }
                lastSourcePath = _path;
                if(string.IsNullOrEmpty(lastTargetPath))
                {
                    lastTargetPath = PSDImporterConst.Globle_BASE_FOLDER;
                }
                string _path2 = EditorUtility.OpenFolderPanel("选择工程内部图片存放目录", lastTargetPath, string.Empty).Replace('\\', '/');

                if(!_path2.Contains(PSDImporterConst.Globle_BASE_FOLDER))
                {
                    EditorUtility.DisplayDialog("警告", $"目标目录必须在{PSDImporterConst.Globle_BASE_FOLDER}下", "确定");
                    return;
                }

                lastTargetPath = _path2;
                AssetDatabase.Refresh();
                sb.Clear();
                baseFolderFiles = ForeachFolderAndCachName(PSDImporterConst.Globle_BASE_FOLDER);
                errSb.Clear();
                CopyFolder(_path, _path2);
                AssetDatabase.Refresh();

                sb.Append(errSb);
                textInfo = sb.ToString();
                HugulaEditor.EditorUtils.WriteToTmpFile("PSD2UGUIResImport_Log.txt",sb.ToString());
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
        Vector2 ScrollPos;
        StringBuilder errSb = new StringBuilder();
        void CopyFolder(string source,string target)
        {
            Debug.Log($"Copy foler {source} to {target}");
            sb.AppendLine($"开始导入 \r\n原始文件夹：{source} \r\n目标文件夹：{target} \r\n {System.DateTime.Now.ToString()} \r\n ");

            var files= FilterFiles(source,new string[]{ "png","xml"},SearchOption.AllDirectories);

            for(int i=0;i<files.Length;i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file);
                var extension = Path.GetExtension(fileName);
                if(!baseFolderFiles.TryGetValue(fileName,out var sourPath)  || extension == ".xml" )//如果是xml需要覆盖
                {
                    var targetPath = Path.Combine(target, fileName);
                    if(File.Exists(targetPath) && extension == ".xml")
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

        Dictionary<string,string> ForeachFolderAndCachName(string folder)
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

        public string[] FilterFiles(string path,string[] exts, SearchOption searchOption = SearchOption.AllDirectories )
        {
            return
                Directory
                .EnumerateFiles(path, "*.*", searchOption)
                .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase))).ToArray();
        }
    }

}