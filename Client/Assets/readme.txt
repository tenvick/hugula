文档 https://github.com/tenvick/hugula/wiki
更新日志:
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
