using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEditorInternal;


namespace HugulaEditor.Databinding
{
    public class ConvertBindingPathPartToConfigTools
    {
        private static string prefabReferenceFilePath = "Assets/Tmp/ConvertBindingPathPartToConfigTools_{0}.txt";

        [MenuItem("Hugula/Convert BindingPathPart to BindingPathPartConfig")]
        [MenuItem("Assets/Hugula/Convert BindingPathPart to BindingPathPartConfig")]
        public static void ConvertSelectedToConfig()
        {

            //目录修改为右键选中的文件夹
            string[] selectedGUIDs = Selection.assetGUIDs;
            if (selectedGUIDs.Length == 0)
            {
                Debug.LogError("请先选择一个文件夹");
                return;
            }

            List<string> prefabReps = new List<string>();
            List<string> allGuids = new List<string>();
            string selectedFolder = string.Empty;
            var  sb = new System.Text.StringBuilder();

            string selectedPath = AssetDatabase.GUIDToAssetPath(selectedGUIDs[0]);
            foreach (var guid in selectedGUIDs)
            {
                var path1 = AssetDatabase.GUIDToAssetPath(guid);
                if (Directory.Exists(path1))
                {
                    selectedFolder += "_" + Path.GetFileNameWithoutExtension(path1);
                    //获取path1文件夹包括子文件夹下的所有prefab
                    allGuids.AddRange(AssetDatabase.FindAssets("t:prefab", new string[] { path1 }));
                }
                else if (File.Exists(path1))
                {
                    allGuids.Add(guid);
                    if (selectedFolder == string.Empty)
                    {
                        selectedFolder = Path.GetDirectoryName(path1);
                        selectedFolder = Path.GetFileNameWithoutExtension(selectedFolder);
                    }
                }
            }

            try
            {

                string[] allPrefabGUIDs = allGuids.ToArray(); //AssetDatabase.FindAssets("t:Prefab", new[] { selectFolderPath });

                float total = allPrefabGUIDs.Length;
                for (int i = 0; i < allPrefabGUIDs.Length; i++)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(allPrefabGUIDs[i]);
                    string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                    if (EditorUtility.DisplayCancelableProgressBar($"检测prefab：{prefabName}", $"处理中：{prefabPath}", (float)i / total))
                    {
                        break;
                    }
                    try
                    {

                        var bindableObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);//AssetDatabase.LoadAssetAtPath<BindableObject>(prefabPath);
                        if (bindableObject != null)
                        {
                            //获取所有bindableObject
                            var bindableObjects = bindableObject.GetComponentsInChildren<BindableObject>(true);
                            prefabReps.Add($"   prefab:{prefabPath}  bindableObjects:{bindableObjects.Length}");
                            foreach (var bindable in bindableObjects)
                            {
                                var bindings = bindable.GetBindings();
                                if (bindings != null)
                                {
                                    foreach (var binding in bindings)
                                    {
                                        binding.ParsePathToConfig(binding.path);
                                        EditorUtility.SetDirty(bindable);
                                        var parts = (BindingPathPartConfig[])binding.GetField("partConfigs");
                                        sb.Clear();
                                        foreach (var part in parts)
                                        {
                                            sb.Append($"\n                        path:{part}");
                                        }

                                        prefabReps.Add($"               binding:{binding}  path:{(parts).Length}   {sb.ToString()}\r\n  ");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        prefabReps.Add($"   prefab:{prefabPath}  convert error:{e}");
                    }
                }
                var path = string.Format(prefabReferenceFilePath, selectedFolder);

                prefabReps.Add("\r\n");
                prefabReps.Add("\r\n");
                EditorUtils.WriteToTmpFile(path, prefabReps.ToArray());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}