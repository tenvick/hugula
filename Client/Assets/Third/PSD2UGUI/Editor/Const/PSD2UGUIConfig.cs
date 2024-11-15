using PSDUINewImporter;
using UnityEngine;
namespace PSDUINewImporter
{

    [CreateAssetMenu(fileName = "PSD2UGUINewConfig", menuName = "Creat PSD2NewUGUIConfig Asset")]
    public class PSD2UGUIConfig : ScriptableObject
    {
        [Header("图片搜索根目录")]
        public string m_rootImagePath;//= PSDImporterConst.Globle_BASE_FOLDER;

        [Header("图片默认导入目录")]
        public string m_defautImagePath;//

        [Header("字体资源路径 (TMPro asset)")]
        public string m_fontAssetPath;// = PSDImporterConst.FONT_FOLDER;

        [Header("组件模板加载路径")]
        public string m_psduiTemplatePath;// = PSDImporterConst.PSDUI_PATH;

        [Header("用户模板模板加载路径")]
        public string m_psduiCustomTemplatePath;// = PSDImporterConst.PSDUI_CONSTOM_PATH;
        [Header("用户字体模板加载路径")]
        public string m_psdFontCustomTemplatePath;// = PSDImporterConst.PSDUI_CONSTOM_FONT_PATH;
        [Header("是否启用设置图片")]
        public bool m_SettingImage=true;
        
        public void CloneTo(PSD2UGUIConfig m_config)
        {
            m_config.m_rootImagePath = m_rootImagePath;
            m_config.m_defautImagePath = m_defautImagePath;
            m_config.m_fontAssetPath = m_fontAssetPath;
            m_config.m_psduiTemplatePath = m_psduiTemplatePath;
            m_config.m_psduiCustomTemplatePath = m_psduiCustomTemplatePath;
            m_config.m_psdFontCustomTemplatePath = m_psdFontCustomTemplatePath;
            m_config.m_SettingImage = m_SettingImage;

        }

        public override string ToString()
        {
            return $"\rrootImagePath={m_rootImagePath},\nfontAssetPath={m_fontAssetPath},\npsduiTemplatePath={m_psduiTemplatePath},\npsduiCustomTemplatePath={m_psduiCustomTemplatePath},\npsdFontCustomTemplatePath={m_psdFontCustomTemplatePath} ";
        }
    }
}