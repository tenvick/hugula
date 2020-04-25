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
---@class VM_GC_TYPE
---@field NEVER 永远不销毁
---@field ALWAYS 总是销毁
---@field AUTO  系统判断是否回收
---@field STATE_CHANGED 切换vm_state的时候回收
---@field MANUAL 手动回收
VM_GC_TYPE = vm_gc_type