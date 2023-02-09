using PSDUINewImporter;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PSDUINewImporter
{


    public class PSD2UGUISettingWizard : ScriptableWizard
    {
        [MenuItem("QuickTool/PSD2NewUGUISettingWizard")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<PSD2UGUISettingWizard>("Create Config", "Create");
        }

        private PSD2UGUIConfig m_config;

        private void OnEnable()
        {
            LoadOrCreateConfig();
        }

        private void LoadOrCreateConfig()
        {
            if (File.Exists(PSDImporterConst.__CONFIG_PATH))
            {
                m_config = AssetDatabase.LoadAssetAtPath<PSD2UGUIConfig>(PSDImporterConst.__CONFIG_PATH);
                Debug.Log("读取配置");
            }
            else
            {
                m_config = new PSD2UGUIConfig();
                AssetDatabase.CreateAsset(m_config, PSDImporterConst.__CONFIG_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("创建成功", "创建配置完成!", "确认");

                PSDImporterConst.LoadConfig();
            }
        }

        private void OnGUI()
        {
           
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("字体资源路径:", m_config.m_fontAssetPath);
            if (GUILayout.Button("字体资源(TMPro.asset)路径"))
            {
                string _path = EditorUtility.OpenFolderPanel("字体资源路径", m_config.m_fontAssetPath, string.Empty).Replace('\\', '/');

                _path = GetValue(_path);

                if (!string.IsNullOrEmpty(_path))
                    m_config.m_fontAssetPath = _path;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("ui图片资源根目录:", m_config.m_rootImagePath);
            if (GUILayout.Button("ui图片资源根目录"))
            {
                string _path = EditorUtility.OpenFolderPanel("ui图片资源根目录", m_config.m_rootImagePath, string.Empty).Replace('\\', '/');

                _path = GetValue(_path);
                if (!string.IsNullOrEmpty(_path))
                    m_config.m_rootImagePath = _path;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("系统组件加载路径:", m_config.m_psduiTemplatePath);
            if (GUILayout.Button("系统组件加载路径"))
            {
                string _path = EditorUtility.OpenFolderPanel("系统组件加载路径", m_config.m_psduiTemplatePath, string.Empty).Replace('\\', '/');

                _path = GetValue(_path);
                if (!string.IsNullOrEmpty(_path))
                    m_config.m_psduiTemplatePath = _path;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("自定义模板加载路径:", m_config.m_psduiCustomTemplatePath);
            if (GUILayout.Button("自定义模板加载路径"))
            {
                string _path = EditorUtility.OpenFolderPanel("自定义模板加载路径", m_config.m_psduiTemplatePath, string.Empty).Replace('\\', '/');

                _path = GetValue(_path);
                if (!string.IsNullOrEmpty(_path))
                    m_config.m_psduiCustomTemplatePath = _path;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("自定义字体模板加载路径:", m_config.m_psdFontCustomTemplatePath);
            if (GUILayout.Button("自定义字体模板加载路径"))
            {
                string _path = EditorUtility.OpenFolderPanel("自定义字体模板加载路径", m_config.m_psdFontCustomTemplatePath, string.Empty).Replace('\\', '/');

                _path = GetValue(_path);
                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_psdFontCustomTemplatePath = _path;

                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("保存"))
            {
                if (string.IsNullOrEmpty(m_config.m_rootImagePath) ||
                    string.IsNullOrEmpty(m_config.m_fontAssetPath) ||
                    string.IsNullOrEmpty(m_config.m_psduiTemplatePath) ||
                    string.IsNullOrEmpty(m_config.m_rootImagePath) ||
                    string.IsNullOrEmpty(m_config.m_psdFontCustomTemplatePath))
                {
                    ShowNotification(new GUIContent("配置路径不应该为空!"));
                    return;
                }
                var sav = AssetDatabase.LoadAssetAtPath<PSD2UGUIConfig>(PSDImporterConst.__CONFIG_PATH);
                m_config.CloneTo(sav);
                EditorUtility.SetDirty(sav);
                Debug.LogFormat(" save config = {0}", sav.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                PSDImporterConst.LoadConfig();
                ShowNotification(new GUIContent("保存配置成功!"));
            }
        }

        private static string GetValue(string _path)
        {
            string _path2 = Application.dataPath.Replace('\\', '/');

            if (!_path.Contains("Assets"))
            {
                Debug.LogError($"必须选择UnityAssets路径下的文件夹! 当前选择:{_path}");

                // EditorWindow.ShowNotification(new GUIContent("配置路径不应该为空!"));
                EditorUtility.DisplayDialog("配置路径错误", $"必须选择UnityAssets路径下的文件夹! 当前选择:{_path}", "确认");
                return string.Empty;
            }

            int _index = _path.IndexOf("/Assets", StringComparison.Ordinal);

            _path = _path.Substring(_index + 1, _path.Length - _index - 1);

            return _path;
        }

    }
}