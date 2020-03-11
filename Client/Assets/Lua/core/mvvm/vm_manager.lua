------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local type = type
local ipairs = ipairs
local pairs = pairs
local string_match = string.match
local table_insert = table.insert

local profiler = require("perf.profiler")

local GameObject = CS.UnityEngine.GameObject
local Hugula = CS.Hugula
local ResourcesLoader = Hugula.Loader.ResourcesLoader
local VMgenerate = VMgenerate
local VMConfig = VMConfig
local Binding = Binding
local Binder = Binder

ResourcesLoader.Initialize()

---检查view_base的所有资源是否都加载完成
---@overload fun(view_base:VMBase)
---@param view_base VMBase
local function check_vm_base_all_done(vm_base)
    local views = vm_base.views
    if views then
        for k, v in ipairs(views) do
            if v.isdone ~= true then
                return false
            end
        end
    end
    vm_base.is_res_ready = true
    vm_base.is_active = true
    vm_base:on_active()
    return true
end

---初始化视图和viewmode并设置他们的关系
---@overload fun(gobj:GameObject,view_base:ViewBase)
---@param gobj GameObject
---@param view_base ViewBase
local function init_view_wm(gobj, view_base)
    local bindable_container = gobj:GetComponent("Hugula.Databinding.BindableContainer")
    view_base:add_child(bindable_container)
    ---@class ViewBase
    local vm_base = view_base._vm_base
    local on_asset_load = view_base.on_asset_load
    if on_asset_load then
        on_asset_load(view_base, gobj.name, bindable_container)
    end
    view_base.isdone = true ---标记资源加载完成
    view_base:set_children_context(vm_base)
    check_vm_base_all_done(vm_base)
end

---资源加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_res_comp(data, view_base)
    -- profiler.start()
    local inst = GameObject.Instantiate(data)
    init_view_wm(inst, view_base)
    -- Logger.Log(string.format("init_view_wm:%s\r\n",inst.name), profiler.report())
    -- profiler.stop()
end

---利用find查找view的gameobject root
---@overload fun(name:string,view_base:ViewBase)
---@param name string
---@param vm_base VMBase
---@param view_base ViewBase
local function find_gameobject(name, view_base)
    -- profiler.start()
    local find_name = name
    local inst = GameObject.Find(find_name)
    init_view_wm(inst, view_base)
    -- Logger.Log("init_view_wm\r\n",profiler.report())
    -- profiler.stop()
end

--- 将vm关联的所有view 状态设置为参数enable的值
---@overload fun(vm_name:string,enable:boolean)
---@param vm_name string
---@param enable boolean
local function active_view(vm_name, enable)
    -- Logger.Log("active_view,",vm_name)
    ---@class VMBase
    local curr_vm = VMgenerate[vm_name] --获取vm实例
    if curr_vm.is_res_ready == true then --已经加载过
        local views = curr_vm.views
        if views then
            for k, v in ipairs(views) do
                v:set_active(enable)
            end
        end

        curr_vm.is_active = enable
        if enable then
            curr_vm:on_active()
        else
            curr_vm:on_deactive()
        end
    end
end

---加载配置的UI
---@overload fun(res_path:string,res_name:string,view_base:ViewBase)
---@param res_path string
---@param res_name string
---@param view_base ViewBase
local function load_resource(res_path, res_name, view_base)
    local vm_name = view_base._vm_base.name
    local vm_config = VMConfig[vm_name]
    local async = vm_config.async

    if async == true then ---异步加载
        ResourcesLoader.LoadAssetAsync(res_path, res_name, nil, on_res_comp, nil, view_base)
    else
        local gobj = ResourcesLoader.LoadAsset(res_path, res_name, nil)
        on_res_comp(gobj, view_base) ---同步步加载
    end
end

--- 开始加载或者激活vm关联的view
---@overload fun(vm_name:string)
---@param vm_name string
local function load(vm_name)
    local curr_vm = VMgenerate[vm_name] --获取vm实例
    if curr_vm.is_res_ready == true then --已经加载过
        active_view(vm_name, true)
    else
        -- Logger.Log(vm_name)
        local views = curr_vm.views
        if views then
            local find_path, res_name, res_path
            for k, v in ipairs(views) do
                find_path, res_name, res_path = v.find_path, v.asset_name, v.assetbundle
                if v.find_path ~= nil then
                    find_gameobject(v.find_path, v)
                elseif res_name ~= nil and res_path ~= nil then
                    load_resource(res_path, res_name, v)
                end
            end
        end
    end
end

---失活隐藏vm
---当vm为string单个
---vm table 时候多个
---@overload fun(vm_name:any)
---@param vm_name any
local function deactive(vm_name)
    if type(vm_name) == "string" then --单个
        active_view(vm_name, false)
    else
        for k, v in ipairs(vm_name) do --多个
            active_view(v, false)
        end
    end
end

---激活vm 加载资源或者激活ui
---当vm为string单个
---vm table 时候多个
---@overload fun(vm_name:any)
---@param vm_name any
local function active(vm_name)
    if vm_name == nil then
        error("vm_name is nil")
    end
    if type(vm_name) == "string" then --单个
        load(vm_name)
    else
        for k, v in ipairs(vm_name) do --多个
            load(v)
        end
    end
end

local vm_manager = {}
vm_manager.active = active
vm_manager.deactive = deactive

---vm的激活与失活管理
---@class VMManager
---@field active function
---@field deactive function
VMManager = vm_manager
