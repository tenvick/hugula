------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
---
---
---------------------定义的常量----------------------
local vm_gc_type = {}
vm_gc_type.NEVER = 1 --永远不销毁
vm_gc_type.ALWAYS = 2 --总是销毁
vm_gc_type.AUTO = 3 --系统判断是否销毁，当1 内存不足时候销毁。
vm_gc_type.STATE_CHANGED = 4 --切换vm_state的时候销毁
vm_gc_type.MANUAL = 5 --手动销毁，此时只会调用vm_model.on_deactive不会隐藏或者销毁view
vm_gc_type.RELEASE = 6 --彻底清理模块包括lua vm
vm_gc_type.MARK_STATE_CHANGED = 7 --切换到标记的状态组时候销毁
vm_gc_type.CUSTOM_GC = 8 --自定义回收函数 _on_custom_gc(self,is_state_change,is_mark_group) 返回 true 表示回收
---@class VM_GC_TYPE
---@field NEVER 永远不销毁
---@field ALWAYS 总是销毁
---@field AUTO  系统判断是否回收
---@field STATE_CHANGED 切换状态组的时候回收
---@field MANUAL 手动回收执行on_deactive不隐藏ui
---@field RELEASE 彻底释放模块 包括lua viewmodel和view资源
---@field MARK_STATE_CHANGED 切换到标记的状态组时候回收
VM_GC_TYPE = vm_gc_type

local vm_mark_type = {}
vm_mark_type.ONLY_BACK = 1 --只作为返回标记
vm_mark_type.HIDDEN_SCENES = 2 --push_item时候会隐藏场景 hidden scenes  失活时候会显示场景

--@class VM_ITEM_TYPE
---@field ONLY_MARK 只作为返回标记
---@field HIDDEN_SCENES push_item时候会隐藏场景
VM_MARK_TYPE = vm_mark_type