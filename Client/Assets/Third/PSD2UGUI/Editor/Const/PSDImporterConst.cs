using UnityEditor;
using UnityEngine;
using System.Linq;

namespace PSDUINewImporter
{
    public class PSDImporterConst
    {
        public const string __CONFIG_PATH = "Assets/Third/PSD2UGUI/PSD2UGUINewConfig.asset";

        public const string BASE_FOLDER = "UI/";
        public const string PNG_SUFFIX = ".png";

        /// <summary>
        /// 图片文件夹
        /// </summary>
        public static string Globle_BASE_FOLDER = "Assets/CustomRes/Demo/UI/";

        public static string Globle_PSD_FOLDER = "CustomRes/psd";
  

        /// <summary>
        /// 字体资源路径路径
        /// </summary>
        public static string FONT_FOLDER = "Assets/CustomRes/Font/";

        public const string FONT_ASSET_SUFIX = ".asset";
        /// <summary>
        /// 系统组件模板加载路径
        /// </summary>
        public static string PSDUI_PATH = "Assets/Third/PSD2UGUI/Template/UI/";

        /// <summary>
        /// 用户自定义模板加载路径(包括自定义组件模板)
        /// </summary>
        public static string PSDUI_CONSTOM_PATH = "Assets/CustomRes/Demo/UI/components";

        /// <summary>
        /// 字体模板
        /// </summary>
        public static string PSDUI_CONSTOM_FONT_PATH = "Assets/CustomRes/Demo/UI/components";

        /// <summary>
        /// 设置图片
        /// </summary>
        public static bool PSDUI_SETTING_TEXTRUE = true;


        public const string PSDUI_SUFFIX = ".prefab";

        public static string ASSET_PATH_EMPTY => GetAssetPath("Empty");
        public static string ASSET_PATH_BINDABLECONTAINER => GetAssetPath("BinableContainer");
        public static string ASSET_PATH_LOOP_SCROLLVIEW => GetAssetPath("LoopScrollView");

        public static string ASSET_PATH_BUTTON => GetAssetPath("Button");

        public static string ASSET_PATH_TOGGLE => GetAssetPath("Toggle");
        public static string ASSET_PATH_CANVAS => GetAssetPath("Canvas");
        public static string ASSET_PATH_EVENTSYSTEM => GetAssetPath("EventSystem");
        public static string ASSET_PATH_GRID => GetAssetPath("Grid");
        public static string ASSET_PATH_IMAGE => GetAssetPath("Image");
        public static string ASSET_PATH_RAWIMAGE => GetAssetPath("RawImage");
        public static string ASSET_PATH_HALFIMAGE => GetAssetPath("HalfImage");
        public static string ASSET_PATH_SCROLLVIEW => GetAssetPath("ScrollView");
        public static string ASSET_PATH_SLIDER => GetAssetPath("Slider");
        public static string ASSET_PATH_TEXT => GetAssetPath("Text");
        public static string ASSET_PATH_SCROLLBAR => GetAssetPath("Scrollbar");
        public static string ASSET_PATH_GROUP_V => GetAssetPath("VerticalGroup");
        public static string ASSET_PATH_GROUP_H => GetAssetPath("HorizontalGroup");
        public static string ASSET_PATH_INPUTFIELD => GetAssetPath("InputField");
        public static string ASSET_PATH_TMP_INPUTFIELD => GetAssetPath("TMP_InputField");
        public static string ASSET_PATH_LAYOUTELEMENT => GetAssetPath("LayoutElement");
        public static string ASSET_PATH_TAB => GetAssetPath("Tab");
        public static string ASSET_PATH_TABGROUP => GetAssetPath("TabGroup");

        internal static string GetAssetPath(string componentName)
        {
            return System.IO.Path.Combine(PSDUI_PATH , componentName + PSDUI_SUFFIX);
        }

        /// <summary>
        /// 获取自定义模板prefab
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetAttachedPrefab(string name)
        {
            return System.IO.Path.Combine(PSDUI_CONSTOM_PATH , name + PSDUI_SUFFIX);
        }

        /// <summary>
        /// 查找指定目录下的prefab文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string SearAttachedPrefab(string folder,string name)
        {
            var files = System.IO.Directory.GetFiles(folder, name+PSDUI_SUFFIX, System.IO.SearchOption.AllDirectories)
                    .Where(f => 
                    name ==  System.IO.Path.GetFileNameWithoutExtension(f)
                    );

            var Enumerator = files.GetEnumerator();
            if(Enumerator.MoveNext())
                return Enumerator.Current;
            return string.Empty;
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
                Globle_BASE_FOLDER = _config.m_rootImagePath;
                FONT_FOLDER = _config.m_fontAssetPath;
                PSDUI_PATH = _config.m_psduiTemplatePath;
                PSDUI_CONSTOM_PATH = _config.m_psduiCustomTemplatePath;
                PSDUI_CONSTOM_FONT_PATH = _config.m_psdFontCustomTemplatePath;
                PSDUI_SETTING_TEXTRUE = _config.m_SettingImage;
                Debug.Log($"Load config. {_config.ToString()}");
            }

            // Debug.LogError(_config.m_staticFontPath);
        }
    }
}