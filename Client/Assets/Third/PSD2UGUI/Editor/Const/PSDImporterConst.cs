using UnityEditor;
using UnityEngine;

namespace PSDUINewImporter
{
    public class PSDImporterConst
    {
        public const string __CONFIG_PATH = "Assets/Third/PSD2UGUI/PSD2UGUIConfig.asset";

        public const string BASE_FOLDER = "UI/";
        public const string PNG_SUFFIX = ".png";

        /// <summary>
        /// 公用图片路径，按需修改
        /// </summary>
        public static string Globle_BASE_FOLDER = "Assets/CustomRes/ui_atlas/";

        public static string Globle_PSD_FOLDER = "CustomRes/psd";

        /// <summary>
        /// 图集文件名
        /// </summary>
        public static string Globle_FOLDER_NAME = "common";

        /// <summary>
        ///
        /// </summary>
        public const string RENDER = "Render";

        public const string NINE_SLICE = "9Slice";

        public const string IMAGE = "Image";

        /// <summary>
        /// 字体路径，按需修改
        /// </summary>
        public static string FONT_FOLDER = "Assets/CustomRes/Font/";

        public static string FONT_STATIC_FOLDER = "Assets/Third/Fonts";

        public const string FONT_SUFIX = ".ttf";

        public const string FONT_ASSET_SUFIX = ".asset";
        /// <summary>
        /// 修改资源模板加载路径,不能放在resources目录
        /// </summary>
        public static string PSDUI_PATH = "Assets/Third/PSD2UGUI/Template/UI/";

        public const string PSDUI_SUFFIX = ".prefab";

        public static string ASSET_PATH_EMPTY = GetAssetPath("Empty");
        public static string ASSET_PATH_BINDABLECONTAINER = GetAssetPath("BinableContainer");
        public static string ASSET_PATH_LOOP_SCROLLVIEW = GetAssetPath("LoopScrollView");

        public static string ASSET_PATH_BUTTON = GetAssetPath("Button");

        public static string ASSET_PATH_TOGGLE = GetAssetPath("Toggle");
        public static string ASSET_PATH_CANVAS = GetAssetPath("Canvas");
        public static string ASSET_PATH_EVENTSYSTEM = GetAssetPath("EventSystem");
        public static string ASSET_PATH_GRID = GetAssetPath("Grid");
        public static string ASSET_PATH_IMAGE = GetAssetPath("Image");
        public static string ASSET_PATH_RAWIMAGE = GetAssetPath("RawImage");
        public static string ASSET_PATH_HALFIMAGE = GetAssetPath("HalfImage");
        public static string ASSET_PATH_SCROLLVIEW = GetAssetPath("ScrollView");
        public static string ASSET_PATH_SLIDER = GetAssetPath("Slider");
        public static string ASSET_PATH_TEXT = GetAssetPath("Text");
        public static string ASSET_PATH_SCROLLBAR = GetAssetPath("Scrollbar");
        public static string ASSET_PATH_GROUP_V = GetAssetPath("VerticalGroup");
        public static string ASSET_PATH_GROUP_H = GetAssetPath("HorizontalGroup");
        public static string ASSET_PATH_INPUTFIELD = GetAssetPath("InputField");
        public static string ASSET_PATH_TMP_INPUTFIELD = GetAssetPath("TMP_InputField");
        public static string ASSET_PATH_LAYOUTELEMENT = GetAssetPath("LayoutElement");
        public static string ASSET_PATH_TAB = GetAssetPath("Tab");
        public static string ASSET_PATH_TABGROUP = GetAssetPath("TabGroup");

        internal static string GetAssetPath(string componentName)
        {
            return PSDUI_PATH + componentName + PSDUI_SUFFIX;
        }

        public PSDImporterConst()
        {
            LoadConfig();
        }

        /// <summary>
        /// 读取工具
        /// </summary>
        public static void LoadConfig()
        {
            // 重读资源路径
            PSD2UGUIConfig _config = AssetDatabase.LoadAssetAtPath<PSD2UGUIConfig>(__CONFIG_PATH);

            if (_config != null)
            {
                Globle_BASE_FOLDER = _config.m_commonAtlasPath;
                Globle_FOLDER_NAME = _config.m_commonAtlasName;
                FONT_FOLDER = _config.m_fontPath;
                FONT_STATIC_FOLDER = _config.m_staticFontPath;
                PSDUI_PATH = _config.m_psduiTemplatePath;
                Debug.Log("Load config.");
            }

            // Debug.LogError(_config.m_staticFontPath);
        }
    }
}