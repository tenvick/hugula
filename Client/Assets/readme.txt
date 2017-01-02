文档 https://github.com/tenvick/hugula/wiki
更新日志:
2017.1.2
1 lua bytes支持,lua单文件热更支持。
2 本地assetbundle 以loadfromfile加载。
3 被依赖项目提前加载导致assetbundle requst不能完成complete事件bug repair
4 启动场景优化，不在与lua启动绑定，方便扩展。
5 lua StateManager:record_pop(i) 移除记录日志功能
