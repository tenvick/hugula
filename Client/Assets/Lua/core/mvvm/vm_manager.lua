------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local type = type
local ipairs = ipairs
local pairs = pairs
local require = require
-- local string_match = string.match
local table = table
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding
local lua_distribute = lua_distribute
local DIS_TYPE = DIS_TYPE
local xpcall = xpcall
local debug = debug
local string_format = string.format

-- local table_insert = table.insert
-- local table_indexof = table.indexof
-- local table_remove_item = table.remove_item
-- local table_remove_at = table.remove
-- local profiler = require("perf.profiler")

local CS = CS
local GameObject = CS.UnityEngine.GameObject
local Hugula = CS.Hugula
local ResLoader = Hugula.ResLoader
local ProfilerFactory = Hugula.Profiler.ProfilerFactory
local VMConfig = require("vm_config")[1]
local VMGenerate = require("core.mvvm.vm_generate")
local Timer = Hugula.Framework.Timer
local TLogger = CS.TLogger

local LoadSceneMode = {
    --CS.UnityEngine.SceneManagement.LoadSceneMode
    Single = 0,
    Additive = 1
}
local BindingUtility = Hugula.Databinding.BindingUtility

local vm_manager = {}

local function error_hander(h)
    TLogger.LogError(string_format("lua:%s \r\n %s", h, debug.traceback()))
end

local function safe_call(f, arg1)
    local status, re1, re2 = xpcall(f, error_hander, arg1)
    return re1, re2
end

--------------------state 相关----------------------------

local function _binding_msg(vm)
    local listener = vm.msg
    for k, v in pairs(listener) do
        lua_binding(k, v)
    end
end

local function _unbinding_msg(vm)
    local listener = vm.msg
    for k, v in pairs(listener) do
        lua_unbinding(k, v)
    end
end

------------------------------------------------

local function deactive(self, vm_name, view_active)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if view_active == nil then
        view_active = true
    end
    if curr_vm.is_res_ready == true and curr_vm.is_active then --已经加载过并且已经激活
        local profiler = ProfilerFactory.GetAndStartProfiler("vm_mamanger.deactive:", curr_vm.name)
        safe_call(curr_vm.on_deactive, curr_vm)
        _unbinding_msg(curr_vm)
        curr_vm.is_active = false --标记状态
        if view_active then
            local views = curr_vm.views
            if views then
                for k, v in ipairs(views) do
                    v:set_active(false)
                end
            end
        end

        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "deactive", name = vm_name})
        if profiler then
            profiler:Stop()
        end
    else
        -- Logger.Log("make vm_manager.deactive is_active=false", vm_name)
        curr_vm.is_active = false --标记状态
    end
end

--- 销毁vm关联的所有view的资源
---@overload fun(vm_name:string)
---@param vm_name string
local function destroy(self, vm_name)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    -- Logger.Log("vm_manager.destroy ", vm_name)
    if curr_vm.is_res_ready == true then --如果资源加载完成
        local profiler = ProfilerFactory.GetAndStartProfiler("vm_mamanger.destroy:", curr_vm.name)
        curr_vm.is_active = false
        curr_vm.is_res_ready = false
        curr_vm._is_destory = nil
        safe_call(curr_vm.on_deactive, curr_vm)
        _unbinding_msg(curr_vm)
        safe_call(curr_vm.on_destroy, curr_vm)
        safe_call(curr_vm.clear, curr_vm)
        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "destroy", name = vm_name})

        if curr_vm._need_destructor then --如果是标记了析构
            local destructor = curr_vm.destructor
            if destructor then
                destructor(curr_vm)
            end
        end

        if profiler then
            profiler:Stop()
        end
    else
        -- Logger.Log("mark vm_manager.destroy _is_destory=true", vm_name)
        curr_vm._is_destory = true --标记销毁
    end
end

------------------------------------------------
---检查view_base的所有资源是否都加载完成
---@overload fun(view_base:VMBase)
---@param view_base VMBase
local function check_vm_base_all_done(vm_base, view)
    local views = vm_base.views
    if views then
        for k, v in ipairs(views) do
            if v:is_initialized() == false then
                return false
            end
        end
    end
    local vvm_name = vm_base.name .. "-" .. view.name
    local p_root_name = "--ResL.1_2_0_InsAsync.onComp.check_all_done:" .. vvm_name
    local profiler = ProfilerFactory.GetAndStartProfiler(p_root_name, "", "-ResL.1_2_InsAsync.onComp:" .. view.name)
    vm_base._isloading = false
    vm_base.is_res_ready = true
    local v_profiler =
        ProfilerFactory.GetAndStartProfiler(
        "---ResL.1_2_0_1_InsAsync.onComp.check_all_done.on_active:",
        vvm_name,
        p_root_name
    )
    local is_active = vm_base.is_active --当前的状态
    -- Logger.Log("vm_manager.check_vm_base_all_done. vm_base.is_active = ", is_active, vm_base)
    if vm_base._is_push == true then
        -- safe_call(vm_base.on_push, vm_base)
    else
        safe_call(vm_base.on_back, vm_base)
    end

    _binding_msg(vm_base)

    if views then
        for k, view_base in ipairs(views) do
            view_base:set_active(is_active)
        end
    end
    safe_call(vm_base.on_assets_load, vm_base)

    safe_call(vm_base.on_active, vm_base)

    if v_profiler then
        v_profiler:Stop()
    end

    -- Logger.Log("check_vm_base_all_done", vm_base)
    if views then
        local _auto_context = vm_base.auto_context

        for k, view_base in ipairs(views) do
            if _auto_context then
                local vi_profiler =
                    ProfilerFactory.GetAndStartProfiler(
                    "---ResL.1_2_0_2_InsAsync.onComp.check_all_done.set_context:",
                    vm_base.name .. "-" .. view_base.name,
                    p_root_name
                )
                view_base:set_child_context(vm_base)
                if vi_profiler then
                    vi_profiler:Stop()
                end
            end
        end
    end

    v_profiler =
        ProfilerFactory.GetAndStartProfiler(
        "---ResL.1_2_0_3_InsAsync.onComp.check_all_done.context_after:",
        vvm_name,
        p_root_name
    )
    --on_state_changed
    if vm_base._is_group == true then
        vm_manager._vm_state:_check_on_state_changed(vm_base)
    end

    if is_active then
        lua_distribute(DIS_TYPE.DIALOG_OPEN_UI, vm_base.name) --触发界面打开消息
    end

    if vm_base._is_destory then --如果标记了销毁
        destroy(vm_manager, vm_base.name)
    elseif is_active == false then --非激活状态需要执行deactive逻辑
        vm_base.is_active = true --强行设置为true确保正确的deactive流程
        deactive(vm_manager, vm_base.name)
    end
    if v_profiler then
        v_profiler:Stop()
    end

    if profiler then
        profiler:Stop()
    end
    return true
end

---设置view的BindableContainer,并标记资源完成
---@overload fun(gobj:GameObject,view_base:ViewBase)
---@param gobj GameObject
---@param view_base ViewBase
local function set_view_child(gobj, view_base)
    local bindable_container = BindingUtility.GetBindableObject(gobj)
    view_base:set_child(bindable_container or gobj)
end

---初始化视图和viewmode并设置他们的关系
---@overload fun(view_base:ViewBase,viewmodel:VMBase)
---@param view_base ViewBase
---@param viewmodel VMBase
local function init_view(view_base, viewmodel)
    -- body
    local on_asset_load = view_base.on_asset_load
    if on_asset_load then
        on_asset_load(view_base, view_base._child)
    end
    -- view_base:set_active(true)
    view_base:initialized() --标记初始化

    local vm_base = view_base._vm_base or viewmodel

    -- local is_active = vm_base.is_active --设置当前view状态
    -- view_base:set_active(is_active)

    check_vm_base_all_done(vm_base, view_base)
end

---资源加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_res_comp(data, view_base)
    -- profiler.start()
    if view_base:has_child() ~= true then
        -- local inst = GameObject.Instantiate(data)
        set_view_child(data, view_base)
    end

    init_view(view_base)
    -- profiler.stop()
end

---资源预加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_pre_load_comp(data, view_base)
    if view_base:has_child() ~= true then
        -- data:SetActive(false)
        -- local inst = GameObject.Instantiate(data)
        set_view_child(data, view_base)
        data:SetActive(false)
    end
end

---场景加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_scene_comp(data, view_base)
    if view_base:has_child() ~= true then
        -- set_view_child(data, view_base)
        view_base:set_child(data)
    end

    init_view(view_base)
    -- Logger.Log(string.format("on_scene_comp : %s", view_base))
end

---场景预加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_pre_scene_comp(data, view_base)
    if view_base:has_child() ~= true then
        -- set_view_child(data, view_base)
        view_base:set_child(data)
    end
    -- Logger.Log(string.format("on_pre_scene_comp : %s", view_base))
end

local function on_res_end(data, view_base)
    Logger.Log(string.format("on_res_end : %s", view_base))
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
    set_view_child(inst, view_base)
    init_view(view_base)
    -- Logger.Log("init_view_wm\r\n",profiler.report())
    -- profiler.stop()
end

--- 将vm关联的所有view激活
---@overload fun(vm_name:string)
---@param vm_name string
local function active_view(self, vm_name)
    ---@type VMBase
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    -- Logger.Log("vm_manager.active_view", curr_vm.is_res_ready, curr_vm, curr_vm.is_active)
    if curr_vm.is_res_ready == true and curr_vm.is_active == false then --已经加载过并且未被激活
        local profiler = ProfilerFactory.GetAndStartProfiler("vm_mamanger.active_view ", vm_name)
        -- Logger.Log("real vm_manager.active_view", curr_vm.is_res_ready, curr_vm, curr_vm.is_active)

        curr_vm.is_active = true

        local views = curr_vm.views

        if curr_vm._is_push == true then
            -- safe_call(curr_vm.on_push, curr_vm)
        else
            safe_call(curr_vm.on_back, curr_vm)
        end

        _binding_msg(curr_vm)

        local _auto_context = curr_vm.auto_context
        if views then
            for k, v in ipairs(views) do
                v:set_active(true)
            end
        end

        safe_call(curr_vm.on_active, curr_vm)

        if views then
            for k, v in ipairs(views) do
                if _auto_context and not v:has_context() then --是否需要对view重新设置viewmodel
                    v:set_child_context(curr_vm)
                end
            end
        end

        --on_state_changed 没有资源的模块也需要检测state changed 事件
        -- if (views == nil or #views == 0) and curr_vm._is_group == true then
        if curr_vm._is_group == true then
            vm_manager._vm_state:_check_on_state_changed(curr_vm)
        end

        lua_distribute(DIS_TYPE.DIALOG_OPEN_UI, vm_name) --触发界面打开消息
        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "active_view", name = vm_name})
        if profiler then
            profiler:Stop()
        end
    end
end

---加载配置的资源
---@overload fun(res_name:string,view_base:ViewBase,on_asset_comp:function,is_pre_load:boolean)
---@param res_name string
---@param view_base ViewBase
---@param on_asset_comp function
---@param is_pre_load boolean
local function load_resource(res_name, view_base, on_asset_comp, is_pre_load)
    local vm_name = view_base._vm_base.name
    local vm_config = VMConfig[vm_name]
    local async = vm_config.async
    local has_child = view_base:has_child()

    if is_pre_load == true then
        async = is_pre_load
        if has_child then
            return
        end --
    end

    if on_asset_comp == nil then
        on_asset_comp = on_res_comp
    end

    if has_child then --and is_pre_load ~= true then --如果已经加载资源
        init_view(view_base)
    else
        if async ~= false then ---异步加载
            ResLoader.InstantiateAsync(res_name, on_asset_comp, nil, view_base)
        else
            local gobj = ResLoader.Instantiate(res_name)
            on_asset_comp(gobj, view_base) ---同步步加载
        end
    end
end

---加载场景
---@overload fun(scene_name:string,view_base:ViewBase,on_asset_comp:function,is_pre_load:boolean)
---@param scene_name string
---@param view_base ViewBase
---@param on_asset_comp function
---@param is_pre_load boolean
local function load_scene(scene_name, view_base, on_asset_comp, is_pre_load) --LoadScene
    local load_scene_mode
    if view_base.load_scene_mode ~= nil then
        load_scene_mode = view_base.load_scene_mode
    else
        load_scene_mode = LoadSceneMode.Additive
    end

    if on_asset_comp == nil then
        on_asset_comp = on_scene_comp
    end

    local allow_scene_activation = true
    if view_base.allow_scene_activation ~= nil then
        allow_scene_activation = view_base.allow_scene_activation
    end

    if is_pre_load == true then --强制设置scene active 一般用于预加载
        allow_scene_activation = false
        if view_base:has_child() then
            return
        end --
    end

    ResLoader.LoadSceneAsync(scene_name, on_asset_comp, on_res_end, view_base, allow_scene_activation, load_scene_mode)
end

--- 开始加载或者激活vm关联的view
---@overload fun(vm_name:string,arg:any)
---@param vm_name string
---@param arg any
local function load(self, vm_name, arg, is_push, is_group)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if not is_push then --is back
        arg = curr_vm._push_arg
    end
    curr_vm:on_push_arg(arg) --有参数
    curr_vm._push_arg = arg
    curr_vm._is_push = is_push --是否是push到栈上的
    curr_vm._is_group = is_group --是否是组
    local views = curr_vm.views

    if views == nil or #views == 0 then
        curr_vm.is_res_ready = true
    end
    -- Logger.Log("vm_mamanger.load ", curr_vm, curr_vm.is_active, "_isloading=", curr_vm._isloading)
    if curr_vm.is_res_ready == true then --已经加载过
        active_view(self, vm_name)
    else
        curr_vm.is_active = true --需要重新加载，设置本身为激活状态
        curr_vm._is_destory = nil --清理销毁标记
        if views and curr_vm._isloading ~= true then
            curr_vm._isloading = true
            local find_path, res_name, scene_name
            for k, v in ipairs(views) do
                find_path, res_name, scene_name = v.find_path, v.key, v.scene_name
                if res_name ~= nil then
                    load_resource(res_name, v)
                elseif scene_name ~= nil then
                    load_scene(scene_name, v)
                elseif v.find_path ~= nil then
                    find_gameobject(v.find_path, v)
                end
            end
        end
    end
end

---激活vm 加载资源或者激活ui
---当vm为string单个
---vm table 时候多个
---@overload fun(vm_name:string,arg:any)
---@param vm_name string
---@param arg any
---@param is_group bool
local function active(self, vm_name, arg, is_push, is_group)
    if vm_name == nil then
        error("VMManager.active vm_name is nil")
    end
    -- Logger.Log("vm_manager.active(", vm_name.name or vm_name)
    if type(vm_name) == "string" then --单个
        load(self, vm_name, arg, is_push)
    else
        for k, v in ipairs(vm_name) do --多个
            load(self, v, arg, is_push, is_group)
        end
    end
    lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "active", name = vm_name})
end

---预加载vm资源
---当vm为string单个
---@overload fun(vm_name:string)
---@param vm_name string
local function pre_load(self, vm_name)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    local views = curr_vm.views
    if views and not curr_vm.is_res_ready then
        local res_name, scene_name
        for k, v in ipairs(views) do
            res_name, scene_name = v.key, v.scene_name
            if scene_name ~= nil then
                load_scene(scene_name, v, on_pre_scene_comp, true)
            elseif res_name ~= nil then
                load_resource(res_name, v, on_pre_load_comp, true)
            end
            Logger.Log("pre_load", vm_name, k, v)
        end
    end
end

---热重载 vm
---当vm为string单个
---@overload fun(vm_name:string)
---@param vm_name string
local function re_load(self, vm_name)
    local curr_vm = VMGenerate[vm_name] --获取旧vm实例
    local views = curr_vm.views
    local is_res_ready = curr_vm.is_res_ready
    local arg = curr_vm._push_arg --保存变量
    local is_active = curr_vm.is_active
    local _is_push = curr_vm._is_push

    if is_active ~= true then
        deactive(self, vm_name)
    end

    VMGenerate:reload_vm(vm_name, true) --热重载模块
    curr_vm = VMGenerate[vm_name] --新的模块
    curr_vm:on_push_arg(arg)
    curr_vm._push_arg = arg
    curr_vm._is_push = _is_push --是否是push到栈上的
    curr_vm.is_active = is_active

    if views == nil or #views == 0 then
        is_res_ready = true
        curr_vm.is_res_ready = true
    end
    if views and #views > 0 and is_res_ready then --如果资源已经加载需要重新刷新
        -- local res_name, scene_name
        local new_views = curr_vm.views
        for k, v in ipairs(new_views) do
            v:set_child(views[k]._child) -- set view BindableContainer
            init_view(v)
        end

        --clear
        for k, v in ipairs(views) do
            v._child = nil
            v._vm_base = nil
            v._initialized = false
        end
    elseif is_res_ready then
        -- Logger.Log(" re_load no view", vm_name)
        active_view(self, vm_name)
    end
end

local function release(self, vm_name)
    VMGenerate:release_vm(vm_name) --彻底释放
end

vm_manager.re_load = re_load
vm_manager.pre_load = pre_load
vm_manager.load = load
vm_manager.active = active
vm_manager.destroy = destroy
vm_manager.deactive = deactive
vm_manager.release = release

---vm的激活与失活管理
---@class VMManager
---@field active function
---@field pre_load function
---@field load function
---@field destroy function
---@field deactive function
-- VMManager = vm_manager
return vm_manager
