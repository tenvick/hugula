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

        /// <summary>
        /// export assetbundle suffix
        /// </summary>
        public const string ASSETBUNDLE_SUFFIX = "u3d";

        public const string CHECK_ASSETBUNDLE_SUFFIX = "." + ASSETBUNDLE_SUFFIX;

        public const string DOT_BYTES = ".bytes";

        /// <summary>
        /// the lua out path
        /// </summary>
        public const string LUACFOLDER = "lua_bundle";
        /// <summary>
        /// the lua bundle name
        /// </summary>
        public const string LUA_BUNDLE_NAME="asset_lu_.bundle";
        public const string PROTO_BUNDLE_NAME="asset_pr_.bundle";
        public const int BUNDLE_OFF_SET=16;

        /// <summary>
        /// lua 编译零时目录
        /// </summary>
        
        public const string LUA_TMP_FOLDER = "PW";

        /// <summary>
        /// streamingAsset下面的所有folderManifest文件列表打成的bunlde 名字
        /// </summary>
        public const string STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME = "streaming_all"+CHECK_ASSETBUNDLE_SUFFIX;

        /// <summary>
        /// 
        /// </summary>
        public const string CRC32_VER_FILENAME = "version.json";

        /// <summary>
        ///  配置
        /// </summary>
        public const string CONFIG_CSV_NAME = "font1.u3d";

        /// <summary>
        ///  PackingType
        /// 需要排除的folder name
        /// </summary>
        public const string FOLDER_STREAMING_NAME =  "streaming";

        /// <summary>
        /// 语言包特殊前缀
        /// </summary>
        
        public const string LANGUAGE_PREFIX = "lan+";

        public const string  LUA_PREFIX = "lua_";


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

        /// <summary>
        /// is web mode
        /// </summary>
#if HUGULA_WEB_MODE
            public const bool IS_WEB_MODE = true;
#else
        public const bool IS_WEB_MODE = false;
#endif
    }

}