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

require("common.constants") --定义的常量
require("core.databinding.ilist_table") --可以装换成ilist的table
require("core.databinding.notify_object") ---带通知功能的table集合
require("core.databinding.notify_table") ---带通知功能的普通luatable
require("core.databinding.binding_expression") ---表达式与寻值

require("models.model")

require("core.mvvm.view_base")
require("core.mvvm.vm_state")
require("core.mvvm.vm_base")

require("converter_register")

--net
DIS_TYPE=require("net.lua_dispatcher_type") --派发事件类型
LuaDispatcher=require("net.lua_dispatcher") --lua 派发器
require("net.protoc_init") --加载描述文件
Rpc = require("net.rpc") --rpc
Rpc:init()
Net=require("net.net") --网络处理