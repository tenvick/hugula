###Hugula 游戏框架 for slua

Hugula是一个基于unity3d+lua技术的全LUA免费开源游戏框架，能方便实现代码动态更新。
特点：全lua逻辑，基于状态模式，包含引用计数，对象池，自动资源回收等功能，能极大提高开发速度。
加入crc校验。 
[帮助文档](https://github.com/tenvick/hugula/wiki)。
qq群：19324776。
 
##本框架需要以下条件

1)unity3d 版本为 5.0以上 （5.1以上需要自行清理lua wrap file，后重新make）

2)slua项目 地址 https://github.com/pangweiwei/slua

3)如需使用tools辅助功能 需要配置python2.7 lua5.1环境

##框架目录

Assets

-Config               （存放xxx.csv 配置文件 使用菜单 Hugula/export config [Assets\Config]导出）

-CustomerResource     （存放美术资源）

-Hugula               （核心代码）

-Scene                （场景 begin为开始场景）

-Slua                 （Slua插件）

-Tmp                  （编译lua文件临时存放目录）

-Lua                  （lua脚本）


##运行发布

1. 一键发布 Hugula -> Build For Publish (首次运行或者发布时候)
1. 导出资源

 2.1  AssetBundles/Build AssetBundles 导出assetbundle资源

 2.2  AssetBundles/Generate/AssetBundle Update File  导出更新差异包和版本号等信息 
 
 2.3  AssetBundles/Generate/AssetBundle Md5Mapping  真实名字Md5映射值表

1. 导出其他 

 3.1 Hugula/export lua [Assets\Lua]          打包编译脚本

 3.2 Hugula/export config [Assets\Config]    打包配置
 
 3.3 Hugula/AES                              加密相关菜单 
 
1. 导出slua接口

 4.1 Slua/All/Make   导出slua所需要的wrap类。

 4.2 如果打开有报错 Slua/All/Clear	清理已经生成的wrap类

### lua架构图

![](Client/Assets/Doc/hugula lua framework.png)

##框架案例
百妖叛乱(卡牌) http://pan.baidu.com/s/1hssBiCG 
俄罗斯方块小游戏 http://pan.baidu.com/s/1o6L4E86
