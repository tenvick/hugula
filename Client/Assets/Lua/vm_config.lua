------------------------------------------------
--  Copyright © 2013-2022   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local string_format = string.format
local setmetatable = setmetatable
local rawget = rawget
local rawset = rawset
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

--VMGroup item的meta table
-- 用于vmgroup 配置的append table 管理
local vm_group_item_mt = {}
vm_group_item_mt.__index = function(t, k)
    if k == "__append" then
        local __ap = {}
        rawset(t, "__append", __ap)
        return __ap
    else
        return rawget(vm_group_item_mt, k)
    end
end

vm_group_item_mt.get_append_items = function(t)
    return t.__append
end

vm_group_item_mt.append_item = function(t, item)
    local _append = t.__append
    _append[item] = true
end

vm_group_item_mt.contains_append_item = function(t, item)
    local _append = t.__append
    return _append[item]
end

vm_group_item_mt.remove_append_item = function(t, item)
    local _append = t.__append
    _append[item] = nil
end

vm_group_item_mt.clear_append = function(t, item)
    local _append = t.__append
    for key, value in pairs(_append) do
        _append[key] = nil
    end
end

--用于设置VMGroup的配置metatable
local _vm_group_get_mt = {}
_vm_group_get_mt.__index = function(t, k)
    local group_conf = vm_group[k] --(vm_group, k)
    if group_conf then
        setmetatable(group_conf, vm_group_item_mt)
        rawset(group_conf, "name", k)
        rawset(t, k, group_conf)
        return group_conf
    else
        return k
    end
end

local _VMGroupGet = setmetatable({}, _vm_group_get_mt)

-- 用于获取_VMGroup配置索引的key
local vmgroup_name = {}
vmgroup_name.__index = function(t, k)
    return k
end
local vm_g = setmetatable({}, vmgroup_name)

---@type vm_group
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
vm_config.demo_click_tips = {vm = "demo.demo_click_tips"}

----------------------------------------------------------------------------------demo end-----
vm_config.ui_login = {vm = "viewmodels.ui_login", gc_type = VM_GC_TYPE.ALWAYS} 

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

vm_group.ui_login = {"ui_login","back_tips"}

-- VMGroupSource = vm_group
return {vm_config, _VMGroupGet}
