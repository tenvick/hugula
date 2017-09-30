// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
// using CustomLuaClass = SLua.CustomLuaClass;
// using DoNotToLua = SLua.DoNotToLua;

namespace Hugula.Utils
{
    /// <summary>
    /// 一些公用常量
    /// </summary>
    [SLua.CustomLuaClass]
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
        [SLua.DoNotToLua]
        public const string LUACFOLDER = "LuaBytes";

        public const string LUA_BUNDLE_NAME_X86="luabytesAsset32";
        public const string LUA_BUNDLE_NAME_X64="luabytesAsset64";

        /// <summary>
        /// lua 编译零时目录
        /// </summary>
        [SLua.DoNotToLua]
        public const string LUA_TMP_FOLDER = "PW";

        /// <summary>
        /// crc32 file list
        /// </summary>
        public const string CRC32_FILELIST_NAME = "filemanifest.u3d";

        /// <summary>
        /// 
        /// </summary>
        public const string CRC32_VER_FILENAME = "crc32_ver.u";

        /// <summary>
        ///  配置
        /// </summary>
        public const string CONFIG_CSV_NAME = "font1.u3d";

        /// <summary>
        /// 语言包特殊前缀
        /// </summary>
        [SLua.DoNotToLua]
        public const string LANGUAGE_PREFIX = "lan+";

        /// <summary>
        /// The Http STAR.
        /// </summary>
        public const string HTTP_STRING = "http://";

        /// <summary>
        /// The Https STAR.
        /// </summary>
        public const string HTTPS_STRING = "https://";

        /// <summary>
        /// 首包路径
        /// </summary>
        [SLua.DoNotToLua]
        public const string FirstOutPath = "FirstPackage";

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