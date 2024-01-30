文档 https://github.com/tenvick/hugula/wiki
更新日志:
2024.1.30
1 memoryinfo内存查询插件。
2 PoolManager根据内存情况制定释放策略。
3 lua 5.4.6 库文件上传。
4 android streamingasset.jar 工程上传。 

2023.8.16
1 com.hugula.value-string@0.0.1 无gc string 包引入(谨慎使用)
2 lua require gc alloc 优化 XLUA_USE_SPAN 开启。
3 lua 通用字节码开启(android)。

2023.8.11 
1 transition过渡效果重构(ui_effect_loading.lua)，有无viewviewmodel都流程统一。
2 BindableContainer 属性筛选，MulitTargetsBinder多目标绑定组件。
3 gc 优化，Profiler支持时间排序。

2023.2.14
1 模板自动生成(字体与用户模板) ui_login.psd

2023.2.9
1 psd导出工具支持字体与自定义模板。
2 psd导出参考图功能。
3 xml导出更新界面功能。

2022.11.28
1 增量更新支持(.1 aas增量工具生成group, .2 Hugula/Update a Previous Build(produce smaller content updates)菜单生成更新文件)。
2 Addressables 升级到1.21.1 解决增量更新bug。

2022.11.10
1 BindableContainer半绑定的模式支持(直接Get组件设置值，eg:bag)。

2022.9.14
1 分包工具优化与新的分包资源上传。
2 timer profiler 优化。
3 profiler log按照时间输出。
4 webdownload 重连修改为异步方式。
5 测试脚本。

2022.8.2
1 lua取消 zip方式打包。
2 zip分包支持优先级配置。
3 图集映射战斗刷新。

2022.7.22
1 lua zip模式，#USE_LUA_ZIP开启
2 proto优化 启动阶段统一加载
3 #PROFILER_DUMP统一控制开启

2022.7.2
1 增加提示更新功能 文档(hot res update.docx)。
2 远端ver.json加载失败也能进入游戏。
3 GameInitialize统一控制启动流程。

2022.6.17
1 增量更新支持(lua)。
2 运行阶段，发布阶段打点分析功能。
3 激活界面传入开启索引可以用来设置界面层级。


2022.5.26
1 扩展包资源标记功能。
2 状态切换时候自定义loading功能。
3 缺失包重写地址功能增加以address为key。
4 修复zip下载太快，没有解压bug。
5 TLogger 输出日志到控制台。
6 数据绑定目标不匹配提示,多选编辑支持。

2022.5.10
1 热更新暂停功能。
2 address依赖zip包下载与缺失资源替换功能。 文档hot res update.docx 示例welcome.lua 155-157 ,player_load.lua
3 profiler打点优化。

2022.4.14
1 多次热更新文件读取与重定向优化。
2 编辑器模式下resNumber key优化，VersionConfig.cs命名规范。

2022.4.8
1 fast包下载问题修复，构建时候自动判断fast包状态。
2 热跟新关键日志开启。
3 TLogger放入文件夹,BindableContainer报错打印全路径。

2022.4.6
1 LoopVerticalScrollRect控件滚动到指定索引。

2022.4.1
1 扩展包(zip)加入校验版本号判断。
2 ui流程打点分析工具细化。
3 日志功能完善。
4 BindableContainer 编辑器模式删除报错处理

2022.3.20
1 绑定列表编辑功能优化扩展支持属性排序与筛选。
2 HotUpdate热更新流程注释。

2022.3.18
1 支持分包加载的热更新功能

2022.2.20
1 NotifyTable支持property。
2 LoopVerticalScrollRect支持动态高度，loading状态等。
3 聊天demo支持lv滚动控件新特性。

2022.1.26
1 lua notify优化
2 profiler输出格式优化，打点增加。
3 框架稳定性优化。
4 xlua 更新。


2021.11.27
1 数据绑定引入对象池与引用清空。
2 资源延时回收。
3 重启时候清理单例与各种Manager。
4 vm栈管理与异步加载相关bug修复。
5 lua require优化。
6 models.models_manager 统一模型管理。



2021.5.30
1 psd to ugui 命名空间修改。
2 增加Slider控件导入支持。
3 psd to ugui 导入坐标以localPosition计算。

2021.5.23
1 psdtougui插件整合，完善ui导出到模板生成全流程。
2 psd to ugui 文档与示例。
3 BindableObject 绑定清理功能。
4 TextMeshPro 插件导入。

2021.3.26
1 子模块示例demo。
2 滚动列表LoopScrollRect编辑器预览功能。
3 Profiler功能。
4 BindableContainer显示自身可绑定属性。
6 F6刷新所有当前激活lua模块代码。

2021.3.13
1 code gen代码模板生成工具与教程

2021.2.27
1 databinding graph 工具。
2 编辑器热重载，打开界面F5刷新代码(无需关闭界面直接刷新)。
3 编辑器拖拽Binder类匹配修改为class name匹配。
4 Hugula教程doc更新。

2021.1.16
1 音效资源asset包支持。
2 集合绑定控件强制刷新内容。
3 BindableContainer容器编辑器删除bug修复。
4 msg自动绑定机制。 
5 viewmodel.property.property_name = value 设置替代 viewmodel:SetProperty("property_name",value)

2020.11.14
1 基于ass的atlas生成工具与热更新打包脚本。
2 demo资源与代码独立成单独文件夹。

2020.8.14
1 lua protobuf 生成工具(Hugula/Lua Protobuf ...)导出工具(Hugula/Export Lua Protobuf)
2 登录示例，客户端模拟服务器发送消息。

2020.8.12
1 lua protobuf 集成。

2020.8.10
1 AudioManager声音停止后再播放bug修复，CoolDown时间格式显示错误修复，LoopScrollRect滚动条报错处理。
2 vm_state增加不入栈，激活和失活vm模块功能(自己管理vm状态)。
3 AssetbundleMappingManger 异步加载计数默认为1。

2020.7.25
1 Binder在属性面板暴露Target 属性,PopUpComponentsAttribute Drawer类型修改为UnityEngine.object,部分C#代码警告消除。
2 VMState 增加 onremove_pop()移除栈顶对象，debug_stack()测试方法，暴露vm_group配置为VMGroupSource。
3 viewmodel 引入on_push和on_back事件方法,on_state_changed(group)增加vm_group参数。


2020.7.14
1 LoopVericalScrollRect控件删除数据清空索引，模板变更刷新样式，默认参数传递优化。
2 AuioManager同一个音乐不重复播放。
3 CollectionViewBinder context null 判断。

2020.7.5
1 VMManager,VMConfig 全局变量优化。
2 模块间调用,统一为VMState:call_fun(viewmodel_name,function_name,arg)

2020.6.24
1 xlua v2.1.15版本同步。
2 AssetbundleMappingManger 变量名修改，列表为空时候移除list。
3 CustomBinding 示例加入。

2020.6.20
1 xlua auto wrap 预编译命令XLUA_AUTOWRAP开启。
2 binding对象convert刷新，设置context为null后下次自动刷新。
3 VMStateHelper增加get_member方法。用于无GC执行luafunction
4 框架方面:viewmodel增加on_state_changed方法。当VMState.push()时候触发

2020.6.14
1 同步加载assetbundle时候取消异步加载done事件。
2 VMManager 先执行on_deactive然后在设置view.gameobject active为false
3 BindableObject xlua方法黑名单增加，默认显示所有BindableObject的可序列化属性。
4 新的fps类，支持采样分析。

2020.6.6
1 Bindalbe对象编辑界面优化。
2 LoopScrollRect 滚动条支持。

2020.5.31
1 AudioManager模块，声音分音乐和音效，音效需要利用工具Hugula/Audio/Create(Refresh) AudioClip Asset打包成集合。
2 assetbundle与asset映射查找模块，加载资源再也不需要关心ab了。
3 AtlasManger图集sprite加载管理模块。
4 LoopScrollRect selectedIndex触发选中功能。
5 ReferCountInfo 查看assetbundle数量筛选优化。

2020.5.15
1 Timer功能延时执行n次，Timer.Add(delay,cycle,action<object>)。
2 NotifyTable 与ObservableCollection<T> FindIndex功能，方便查找索引。
3 BindableContainer 一键添加所有子BindableObject对象，Ctrl+g 打开开始场景。
4 NotifyTable 支持C# List对象插入替换删除。

2020.4.30
1 Hugula.Editor名字空间变更。
2 Binder对象修改为泛型方式。
3 CollectionViewBinder新的集合显示组件，需要自己定义布局。
4 log日志关闭功能。
5 NofityTable增加FindIndex(filter_fun(index,item) )函数方便数据索引查找
6 VMState初始化流程修改，以字段标识初始状态。
7 xlua Config去除黑名单添加，生成模板添加Lua Profiler功能。


2020.4.25
1 vm模块资源预加载功能,动态资源加载asset_loader,scene_loader功能。
2 Config目录.bytes文件打包功能。
3 图集打包assetbundle统一，资源整理。
4 依赖项引用ab计数bug修复。
5 资源释放策略目前支持释放类型有(NEVER,ALWAYS,AUTO,STATE_CHANGED,MANUAL)
6 group与item log_enable控制(loading界面在VMState:back()的时候不需要返回)。
7 GloabalDispatcher引入。
8 场景加载示例(带loading界面)。

2020.4.18
1 列表控件支持选中样式。
2 view支持延时关闭和销毁。
3 VMState:push("vm",arg)参数传递。
4 viewmodel.views 销毁流程增加。
5 Hugula.Manager名字空间修改为Hugula.Framework
6 GlobalDispathcer全局事件派发,VMStateHelper C#访问lua VMState辅助类。

2020.4.13
1 Delay和DelayFrame功能支持。
2 Binder target访问修改。
3 LoopScrollRect itemParameter传递优化。
4 AnimatorBinder,GroupSelector,CoolDown,LuaModule与LuaNotifyComponent,LuaComponent引入。
5 ui资源清理与图集生成。
6 bag示例。


2020.4.8 
1 C#Examples 滚动列表示例(loop_scroll_rect),Converter示例(string_texture_converter), 文本绑定与类型自动转换,控件之接双向绑定示例(textbinding)
2 EnterLua入口属性修改为可序列化属性。
3 清理废旧代码和注释。
4 CustomBinder引入，可以任意单向绑定Unity的Component对象属性。
5 Manager增加Add和Remove方法支持挂接的MonoBehaviour。

2020.4.7
1 Binding流程优化，引入IBinding接口,source修改为序列化引用。
2 Convert管理机制引入，可以同时支持C#和lua Converter。
3 引入Manager管理所有IManager对象，引入单例模式。
4 组件LoopScrollRect 易用性优化。
5 xlua某些属性黑名单不排除bug修复。
6 资源清理，其他细节优化。

2020.3.27
1 atlas图集引用与打包,全局sprite信息保存。
2 ImageBinder增加spriteName属性，可以直接绑定spriteName对应的sprite。
3 新增TextMeshProBinder，预编译命令USE_TMPro开启。
3 Binder销毁引用资源清理。

2020.3.11
1 BindingExpression和BindingPathPart移值到C#中实现。
2 真机导出lua文件后缀修改。
3 LoopScrollRect 单行或者单列适配pading问题修改。
4 IBindingExpression接口修改。


2020.3.2
hugula 0.5.1 alpaha版本发布
1 轻量级的数据绑定框架，支持单向双向绑定,支持C#对象与luatable的双向绑定,控件之间互相绑定。
2 支持绑定表达式表,string.format,convert，绑定表达式模块基于lua，扩展方便。
3 framework重新构建成mvvm模式，基于stack与group资源管理机制。slua替换为xlua,逻辑全lua，热更新无压力。
4 内置循环列表和自适应循环列表对大量数据显示无压力（聊天demo）。

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
