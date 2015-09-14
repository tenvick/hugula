Hugula 游戏框架 for slua
======
Hugula是一个基于unity3d+slua技术的全LUA免费开源游戏框架，作者基于此框架已经开发过两款卡牌带arpg战斗游戏。
特点 全lua逻辑，基于状态模式，只需关心状态，无需关心UI隐藏和显示。
qq群：19324776。
 
本框架需要以下条件
======
1)unity3d 版本为 5.0以上 （5.1以上需要自行清理lua wrap file，后重新make）

2)slua项目 地址 https://github.com/pangweiwei/slua

3)如需使用tools辅助功能 需要配置python2.7 lua5.1环境

框架目录
======
Assets

-Config               （存放xxx.csv 配置文件 使用菜单 Hugula/export config [Assets\Config]导出）

-CustomerResource     （存放美术资源）

-Hugula               （核心代码）

-Lan                  （多国语言包 csv   使用菜单 Hugula/export language [Assets\Lan]导出 lua中可直接调用）

-Scene                （场景）

-Slua                 （Slua插件）

-Tmp                  （编译lua文件临时存放目录）

-Lua                  （lua脚本）


发布流程
======
1 导出assetbundle资源 菜单 AssetBundles/Build AssetBundles

2 导出其他

  2.1 Hugula/export lua [Assets\Lua]          打包编译脚本
  2.2 Hugula/export config [Assets\Config]    打包配置
  2.3 Hugula/export language [Assets\Lan]     打包语言包

俄罗斯方块小游戏
======
基于UGUI 
展示lua components的用法
是一种非典型的方式
