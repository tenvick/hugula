文档 https://github.com/tenvick/hugula/wiki
更新日志:
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
