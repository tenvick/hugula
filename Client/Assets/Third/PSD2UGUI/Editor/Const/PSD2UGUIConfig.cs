using PSDUINewImporter;
using UnityEngine;
namespace PSDUINewImporter {

    [CreateAssetMenu (fileName = "PSD2UGUIConfig", menuName = "Creat PSD2NewUGUIConfig Asset")]
    public class PSD2UGUIConfig : ScriptableObject {
        [Header ("通用图集路径")]
        public string m_commonAtlasPath = PSDImporterConst.Globle_BASE_FOLDER;

    [Space(10)]
    [Header("图集名")]
    public string m_commonAtlasName = PSDImporterConst.Globle_FOLDER_NAME;

    [Header("字体资源路径")]
    public string m_fontPath = PSDImporterConst.FONT_FOLDER;

    [Header("静态字体资源路径")]
    public string m_staticFontPath = PSDImporterConst.FONT_STATIC_FOLDER;

    [Header("资源模板加载路径")]
    public string m_psduiTemplatePath = PSDImporterConst.PSDUI_PATH;

    public void CloneTo(PSD2UGUIConfig  m_config)
    {
        m_config.m_commonAtlasPath = m_commonAtlasPath;
        m_config.m_commonAtlasName = m_commonAtlasName;
        m_config.m_fontPath = m_fontPath;
        m_config.m_staticFontPath = m_staticFontPath;
        m_config.m_psduiTemplatePath = m_psduiTemplatePath;
    }

        public override string ToString () {
            return string.Format ("m_commonAtlasPath={0},\nm_commonAtlasName={1},\nm_fontPath={2},\nm_staticFontPath={3},\nm_psduiTemplatePath={4} ", m_commonAtlasPath, m_commonAtlasName, m_fontPath, m_staticFontPath, m_psduiTemplatePath);
        }
    }
}