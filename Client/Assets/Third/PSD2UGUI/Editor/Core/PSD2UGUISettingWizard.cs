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

        bool m_Modify = false;
        private void OnGUI()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("字体资源路径:", m_config.m_fontAssetPath);
            if (GUILayout.Button("字体资源(TMPro.asset)路径"))
            {
            lableFont:
                string _path = EditorUtility.OpenFolderPanel("字体资源路径", m_config.m_fontAssetPath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, null, out _path)) //目录选择错误
                {
                    goto lableFont;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_fontAssetPath = _path;
                    m_Modify = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("图片搜索根目录:", m_config.m_rootImagePath);
            if (GUILayout.Button("图片搜索根目录"))
            {
            labelRootImage:
                string _path = EditorUtility.OpenFolderPanel("图片搜索根目录", m_config.m_rootImagePath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, null, out _path)) //目录选择错误
                {
                    goto labelRootImage;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_rootImagePath = _path;
                    m_Modify = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("图片默认导入目录:", m_config.m_defautImagePath);
            if (GUILayout.Button("图片默认导入目录"))
            {
            labelDefaultImage:
                string _path = EditorUtility.OpenFolderPanel("图片默认导入目录", m_config.m_defautImagePath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, m_config.m_rootImagePath, out _path)) //目录选择错误
                {
                    goto labelDefaultImage;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_defautImagePath = _path;
                    m_Modify = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("系统组件加载路径:", m_config.m_psduiTemplatePath);
            if (GUILayout.Button("系统组件加载路径"))
            {
            labelSystemComp:
                string _path = EditorUtility.OpenFolderPanel("系统组件加载路径", m_config.m_psduiTemplatePath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, null, out _path)) //目录选择错误
                {
                    goto labelSystemComp;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_psduiTemplatePath = _path;
                    m_Modify = true;

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("自定义模板加载路径:", m_config.m_psduiCustomTemplatePath);
            if (GUILayout.Button("自定义模板加载路径"))
            {
            lableTemplate:
                string _path = EditorUtility.OpenFolderPanel("自定义模板加载路径", m_config.m_psduiTemplatePath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, null, out _path)) //目录选择错误
                {
                    goto lableTemplate;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_psduiCustomTemplatePath = _path;
                    m_Modify = true;

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("自定义字体模板加载路径:", m_config.m_psdFontCustomTemplatePath);
            if (GUILayout.Button("自定义字体模板加载路径"))
            {
            labelTextTemplate:
                string _path = EditorUtility.OpenFolderPanel("自定义字体模板加载路径", m_config.m_psdFontCustomTemplatePath, string.Empty).Replace('\\', '/');

                if (!CheckValue(_path, null, out _path)) //目录选择错误
                {
                    goto labelTextTemplate;
                }

                if (!string.IsNullOrEmpty(_path))
                {
                    m_config.m_psdFontCustomTemplatePath = _path;
                    m_Modify = true;

                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("是否设置图片:");
            m_config.m_SettingImage = EditorGUILayout.Toggle(m_config.m_SettingImage);

            EditorGUILayout.EndHorizontal();

            var tips = m_Modify ? "(有变更）保存" : "保存";
            if (GUILayout.Button(tips))
            {
                m_Modify = false;
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


        static bool CheckValue(string path, string checkFolderPath, out string outPath)
        {
            string _path2 = Application.dataPath.Replace('\\', '/');
            path = path.Replace('\\', '/');
            checkFolderPath = checkFolderPath?.Replace('\\', '/');
            if (!path.Contains("Assets") || (!string.IsNullOrEmpty(checkFolderPath) && !path.Contains(checkFolderPath)))
            {
                var t = string.IsNullOrEmpty(checkFolderPath)?"Unity Assets":checkFolderPath;
                Debug.LogError($"必须选择{t}路径下的文件夹! \r\n 当前选择:{path}");
                outPath = string.Empty;
                if (EditorUtility.DisplayDialog("配置路径错误", $"必须选择{t}路径下的文件夹! \r\n当前选择:{path}", "确认"))
                    return true;
                return false;
            }

            int _index = path.IndexOf("/Assets", StringComparison.Ordinal);

            outPath = path.Substring(_index + 1, path.Length - _index - 1);

            return true;
        }


    }
}