------------------------------------------------
--  Copyright © 2013-2022   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local string_format = string.format
local setmetatable = setmetatable
local pairs = pairs
local VM_GC_TYPE = VM_GC_TYPE
------------------------------------------------------------------------

-------------------------------------------------------------------------
local mt = {}
mt.__index = function(t, k)
    error(string_format(" %s does't set key (%s) config  ", t.name, k))
end
---
local vm_config = {name = "ViewModelConfig file:  vm_config.lua"}
setmetatable(vm_config, mt)
--- view model的配置信息
---@class WindowConfig
---@field logo table
---@field scroll_rect_table table
-- VMConfig = vm_config

local vm_group = {name = "ViewModelGroup  file: vm_config.lua"}
setmetatable(vm_group, mt)
--- vm配置组，为了增加模块独立性，使用一组vm模块来切换功能。
---@class VMGroup
---@field welcome table
---@field source_demo table
---@field scroll_rect_table table
-- _VMGroup = vm_group

local vmgroup_name = {}
vmgroup_name.__index = function(t, k)
    return k
end
-- for k,v in pairs(vm_group) do
--     vmgroup_name[k] = k
-- end
local vm_g = {}
setmetatable(vm_g, vmgroup_name)
VMGroup = vm_g
--------------------------------------------------------------------------
-------------------------------viewmodel配置-------------------------------
---------------------------------------------------------------------------
--log_enable --为是否记录回退栈。

--------------------------demo-------------------------------------
vm_config.scene_loader = {vm = "demo.scene_loader", gc_type = VM_GC_TYPE.ALWAYS} ---  动态加载场景
vm_config.asset_loader = {vm = "demo.asset_loader", gc_type = VM_GC_TYPE.ALWAYS} ---  动态加载资源
vm_config.welcome = {vm = "demo.welcome"} ---  示例列表
vm_config.bag = {vm = "demo.bag", gc_type = VM_GC_TYPE.STATE_CHANGED} --- 背包
vm_config.back_tips = {vm = "demo.back_tips", gc_type = VM_GC_TYPE.NEVER} --luaModule
vm_config.binding_demo = {vm = "demo.binding_demo.binding_demo", gc_type = VM_GC_TYPE.STATE_CHANGED}
vm_config.chat_demo = {vm = "demo.chat_demo", gc_type = VM_GC_TYPE.STATE_CHANGED}
vm_config.demo_loading = {vm = "demo.demo_loading", gc_type = VM_GC_TYPE.STATE_CHANGED} ---  loading界面
vm_config.demo_login = {vm = "demo.demo_login"}
vm_config.demo_subui = {vm = "demo.demo_subui"}
vm_config.demo_subui1 = {vm = "demo.demo_subui1"}

----------------------------------------------------------------------------------demo end-----

--------------------------------------------------------------------------
------------------------viewmodel group 配置-------------------------------
---------------------------------------------------------------------------
--log_enable --为是否记录回退栈。

vm_group.welcome = {"welcome"}
vm_group.bag = {"bag","back_tips"}
vm_group.binding_demo = {"binding_demo","back_tips"}
vm_group.demo_subui = {"demo_subui", "back_tips"}
vm_group.chat_demo = {"chat_demo","back_tips"}
vm_group.demo_loading = {"demo_loading",log_enable = false} --loading不需要回退
vm_group.gamescene = {"asset_loader","scene_loader","back_tips"}
vm_group.demo_login = {"demo_login","back_tips"}


-- VMGroupSource = vm_group
return {vm_config,vm_group}