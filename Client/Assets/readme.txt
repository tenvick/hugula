文档 https://github.com/tenvick/hugula/wiki
更新日志:
2018.8.31
1 加载模块重构 精简operation

2018.7.16
1 main.lua MVVM重构。 
2 slua 2018.7.16 版本更新。
3 lua导出bytes修改。

2018.7.13
 1 加载模块简化。

2018.4.21
 1 main.lua函数同步错误处理。
 2 热更新加载错误队列不移除。
 
2018.4.13
 1 协程加载bug修复（MessageBox.Show）
 2 热更新流程优化，断线后提示并加载。
 3 热更新后，安装新包读取缓存资源bug修复。
 4 assetbundle 释放以framcount来控制。

2018.2.8
1 httpdns 支持。
2 热更新文件规范为一种模式。
3 仅支持md5方式构建assetbundle.
4 断点续传的webdowload。
5 热更新优化。
6 更多unity版本支持5.4-2017.x。
7 完善的自动构建shell脚本.

2017.11.20 
1 lua状态切换与异步加载导致的一些bug修复。

2017.11.17
1 启动流程恢复为frist场景加载开始场景，方便开始场景制作。
2 极端情况下ab计数bug修复。
3 PrefabPool池里对象被清理处理。
4 重启功能，场景demo里。

2017.9.25
1 unity 2017 支持。
2 启动流程简化，只保留frist场景。
3 assetbundle variant 支持。
4 onlyInclude 仅包含特定资源[HugulaExtends/Add OnlyInclusion Files]添加的文件,其余文件放入后台加载
5 网络备用多域名功能支持。

2017.8.5
1 二次下载功能(启动时候自动下载[HugulaExtends/Add First Load Files]添加的文件)
2 使用预编译命令控制和打开功能，详细Version.cs。
3 BackGroundDownload 备用域名支持 ver_xxx.txt cdn_host字段配置。
4 自动构建脚本。
5 新增文件不打入FirstPakcage BUG修复,启动界面语言包读取bug修复。

2017.7.22
1 加载模块重构，代码更清醒扩展更方便，减少加载时gcalloc，支持协程和回调两种方式加载。
2 丢弃assetbundleManifest,使用自己的FileManifest,通过ManifestManager来访问依赖项目。
3 BackGroundDownload 热更后台加载模块引入。
4 HUGULA_PROFILER_DEBUG 编译命令增加用于profile调试。
5 MessageBox 系统提示框功能。
6 PrefabPool 回收优化。　

2017.5.5
1 zip file EDITOR 功能
2 HUGULA_RELEASE 编译命令增加。
3 热更新流程优化，优先读取服务器信息ver.txt。
4 PrefabPool.Add 支持原始asset
5 OBB 模式资源读取路径错误修改。
6 unity Editor 下面Simulation模式资源依赖加载问题修复。
7 lua  StateManager 快速切换可能界面卡死bug修复。
8 lua  StateManager:record() 日志记录bug修复。

20174.4.19
1 优化FirstPackage组织方式（OneResFolder,VerResFolder）。
2 更新文件是否带crc后缀 开关功能。appendCrcToFile(Hugula/Setting)。
3 修复删除旧文件路径报错问题。

2017.4.7
1 更新包目录组织方式， EditorSetting里面设置。
2 自动发布脚本补全。

2017.4.6
1 分包机制支持，将扩展文件夹或者扩展文件分离出来单独下载（文档稍后补上）。
2 每一个强制更新版本对应一个首包文件。

2017.3.31
1 完整的多国语言方案。
2 自动发布脚本tools里面。
3 扩展文件夹剔除功能，设置ab名字忽略特定后缀。

2017.3.24
1 ab延时释放策略。
2 新的版本号规则。
3 模拟assetbundle模式 (AssetBundles/Simulation Mode) 直接运行。
4 lua 发布流程优化，首保luaytes放Resources目录。
5 CResloader 代码整理。

2017.3.10
1 CRequest 默认urigroup规则更改。
2 FirstPackage copy assetbundle crc错误校验日志 Assets/Tmp 下面。
3 ScrollRectTable onItemDispose事件增加。
4 CrcCheck 无用代码去除。
2017.3.3
1 依赖项目加载失败也可完成资源complete。
2 热更新后依赖项目资源路径读取问题。
3 bytes转BytesAsset对象功能。

2017.2.22
1 slua IOS原版libslua.a编译报错问题解决。
2 Cache类分离。
3 加载debuglog优化。
4 选中lua文件导出。

2017.2.7
1 Gstring 去除。
2 item_object 标记销毁功能，方便控制模块的资源释放。
3 lua导出优化。
4 CacheManager 增加释放assetbundle的unload(false)功能。
4 CRequest urigroup依赖去除。

2017.1.4
1 CacheData CacheManager访问控制,。
2 ScrollRectTable控件易用性优化，支持editor预览。
2017.1.2
1 lua bytes支持,lua单文件热更支持。
2 本地assetbundle 以loadfromfile加载。
3 被依赖项目提前加载导致assetbundle requst不能完成complete事件bug repair
4 启动场景优化，不在与lua启动绑定，方便扩展。
5 lua StateManager:record_pop(i) 移除记录日志功能
