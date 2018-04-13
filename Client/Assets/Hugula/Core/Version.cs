// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

namespace Hugula
{
    /// <summary>
    /// 版本号
    /// </summary>
    public class Version
    {

        public const uint VERSION = 0x000502;

    }
}
/**
 * 
 * 预编译命名说明
 * //HUGULA_COMMON_ASSETBUNDLE --     使用未加密的名字构建 assetbundle 需运行菜单ssetBundles/Generate/AssetBundle Update File更新ab名字
 * HUGULA_RELEASE                   release模式
 * HUGULA_NO_LOG                    取消log打印
 * HUGULA_SPLITE_ASSETBUNDLE        启用分离assetbundle功能。
 * HUGULA_COMPRESS_STREAMINGASSETS  压缩streamingAssets为一个zip包
 * HUGULA_APPEND_CRC                更新文件带crc后缀    
 *
 * 编辑器辅助
 * HUGULA_ASSETBUNDLE_LOG           记录assetbundle加载日志用与包体
 * HUGULA_LOADER_DEBUG              加载模块打印调试
 * HUGULA_PROFILER_DEBUG            profiler debug 信息
 * HUGULA_CACHE_DEBUG               引用计数与销毁debug信息
 * 
 * 
 * 
**/