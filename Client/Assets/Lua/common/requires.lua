------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
require("core.logger")
require("core.tools")
require("core.structure")
require("core.unity3d")

require("core.lru_cache") --
require("common.constants") --定义的常量
require("core.databinding.get_set_obj") --get set 实现
require("core.databinding.ilist_table") --可以装换成ilist的table
require("core.databinding.notify_object") ---带通知功能的table集合
require("core.databinding.notify_table") ---带通知功能的普通luatable
require("core.databinding.binding_expression") ---表达式与寻值

require("net.lua_dispatcher_type") --派发事件类型
require("net.lua_dispatcher")

models = require("models.models_manager") --全局模型管理

require("core.mvvm.view_base")
require("core.mvvm.vm_state")
require("core.mvvm.vm_base")

require("converter_register")

GlobalDispatcher = CS.Hugula.Framework.GlobalDispatcher
DispatcherEvent = CS.Hugula.Framework.DispatcherEvent
--net
require("net.lua_dispatcher_type") --派发事件类型
require("net.lua_dispatcher") --lua 派发器
-- require("net.net_api_list") --
Rpc = require("net.rpc") --rpc
Rpc:init()
Net=require("net.net") --网络处理