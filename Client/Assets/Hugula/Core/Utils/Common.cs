// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

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

        /// <summary>
        /// the lua out path
        /// </summary>
        [SLua.DoNotToLua]
        public const string LUACFOLDER = "PW";

        /// <summary>
        /// the lua suffix
        /// </summary>
        [SLua.DoNotToLua]
        public const string LUA_LC_SUFFIX = "bytes";

        /// <summary>
        /// 
        /// </summary>
        public const string LUA_ASSETBUNDLE_FILENAME = "lua.u3d";

        /// <summary>
        /// crc32 file list
        /// </summary>
        public const string CRC32_FILELIST_NAME = "file_crc32.u3d";

        /// <summary>
        /// 
        /// </summary>
        public const string CRC32_VER_FILENAME = "ver.u3d";

       /// <summary>
       ///  配置
       /// </summary>
        public const string CONFIG_CSV_NAME = "config.u3d";

        /// <summary>
        /// 语言包目录
        /// </summary>
        [SLua.DoNotToLua]
        public const string LANGUAGE_FLODER = "Lan";

        /// <summary>
        /// 语言包后缀
        /// </summary>
        [SLua.DoNotToLua]
        public const string LANGUAGE_SUFFIX = "lan";

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
        /// is web mode
        /// </summary>
        #if HUGULA_WEB_MODE 
            public const bool IS_WEB_MODE = true;
        #else
            public const bool IS_WEB_MODE = false;
        #endif
    }

}