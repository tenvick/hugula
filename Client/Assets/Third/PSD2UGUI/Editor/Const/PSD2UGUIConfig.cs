using PSDUINewImporter;
using UnityEngine;
namespace PSDUINewImporter
{

    [CreateAssetMenu(fileName = "PSD2UGUIConfig", menuName = "Creat PSD2NewUGUIConfig Asset")]
    public class PSD2UGUIConfig : ScriptableObject
    {
        [Header("sprite图片根路径")]
        public string m_rootImagePath;//= PSDImporterConst.Globle_BASE_FOLDER;

        [Header("字体资源路径 (TMPro asset)")]
        public string m_fontAssetPath;// = PSDImporterConst.FONT_FOLDER;

        [Header("组件模板加载路径")]
        public string m_psduiTemplatePath;// = PSDImporterConst.PSDUI_PATH;

        [Header("用户模板模板加载路径")]
        public string m_psduiCustomTemplatePath;// = PSDImporterConst.PSDUI_CONSTOM_PATH;
        [Header("用户字体模板加载路径")]
        public string m_psdFontCustomTemplatePath;// = PSDImporterConst.PSDUI_CONSTOM_FONT_PATH;

        
        public void CloneTo(PSD2UGUIConfig m_config)
        {
            m_config.m_rootImagePath = m_rootImagePath;
            m_config.m_fontAssetPath = m_fontAssetPath;
            m_config.m_psduiTemplatePath = m_psduiTemplatePath;
            m_config.m_psduiCustomTemplatePath = m_psduiCustomTemplatePath;
            m_config.m_psdFontCustomTemplatePath = m_psdFontCustomTemplatePath;

        }

        public override string ToString()
        {
            return $"\rrootImagePath={m_rootImagePath},\nfontAssetPath={m_fontAssetPath},\npsduiTemplatePath={m_psduiTemplatePath},\npsduiCustomTemplatePath={m_psduiCustomTemplatePath},\npsdFontCustomTemplatePath={m_psdFontCustomTemplatePath} ";
        }
    }
}