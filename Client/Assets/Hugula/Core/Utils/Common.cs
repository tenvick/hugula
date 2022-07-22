// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

namespace Hugula.Utils
{
    /// <summary>
    /// 一些公用常量
    /// </summary>

    public class Common
    {

        static Common()
        {
            _STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME = null;
            _RES_VER_FOLDER = null;
        }

        /// <summary>
        /// export assetbundle suffix
        /// </summary>
        public const string ASSETBUNDLE_SUFFIX = "u3d";

        public const string CHECK_ASSETBUNDLE_SUFFIX = "." + ASSETBUNDLE_SUFFIX;

        public const string DOT_BYTES = ".bytes";

        /// <summary>
        /// the lua out path
        /// </summary>
#if USE_LUA_ZIP
        public const string LUACFOLDER = "lua_bundle";

#else
        public const string LUACFOLDER = "Assets/lua_bundle";
#endif
        /// <summary>
        /// the lua bundle name
        /// </summary>
#if USE_LUA_ZIP
        public const string LUA_BUNDLE_NAME = "z_lua_.bundle";
#else
        public const string LUA_BUNDLE_NAME = "a_lua_.bundle";
#endif


        public const string PROTO_GROUP_NAME = "asset_pb";
        public const int BUNDLE_OFF_SET = 16;
        public const string BUNDLE_OFF_STR = "123";

        static string _STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME;

        /// <summary>
        /// streamingAsset下面的所有folderManifest文件列表打成的bunlde 名字
        /// </summary>
        public static string STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME //= "streaming_all"+CHECK_ASSETBUNDLE_SUFFIX;
        {
            get
            {
                if (string.IsNullOrEmpty(_STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME))
                {
                    _STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME = $"list_v{CodeVersion.CODE_VERSION}{CHECK_ASSETBUNDLE_SUFFIX}";
                }
                return _STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME;
            }
            set
            {
                _STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME = string.Empty;
            }
        }

        static string _RES_VER_FOLDER;

        /// <summary>
        /// 当前版本资源目录
        /// </summary>
        public static string RES_VER_FOLDER
        {
            get
            {
                if (string.IsNullOrEmpty(_RES_VER_FOLDER))
                {
                    _RES_VER_FOLDER = $"v{CodeVersion.CODE_VERSION}";
                }

                return _RES_VER_FOLDER;
            }
            set
            {
                _RES_VER_FOLDER = string.Empty;
            }
        }

        /// <summary>
        /// 版本文件
        /// </summary>
        public const string CRC32_VER_FILENAME = "ver.json";

        /// <summary>
        ///  PackingType
        /// 需要排除的folder name
        /// </summary>
        public const string FOLDER_STREAMING_NAME = "streaming";

        /// <summary>
        /// 语言包特殊前缀
        /// </summary>

        public const string LANGUAGE_PREFIX = "lan+";

        public const string LUA_PREFIX = "lua_";


        /// <summary>
        /// The Http STAR.
        /// </summary>
        public const string HTTP_STRING = "http://";

        /// <summary>
        /// The Https STAR.
        /// </summary>
        public const string HTTPS_STRING = "https://";

        /// <summary>
        /// jar前坠
        /// </summary>
        public const string JAR_FILE = "jar:file://";

    }

}