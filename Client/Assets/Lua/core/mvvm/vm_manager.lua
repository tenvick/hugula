------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local type = type
local ipairs = ipairs
local pairs = pairs
local require = require
local unpack = unpack
local table = table
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding
local lua_distribute = lua_distribute
local DIS_TYPE = DIS_TYPE
local xpcall = xpcall
local debug = debug
local string_format = string.format

local CS = CS
local GameObject = CS.UnityEngine.GameObject
local Hugula = CS.Hugula
local ResLoader = Hugula.ResLoader
local ProfilerFactory = Hugula.Profiler.ProfilerFactory
local VMConfig = require("vm_config")[1]
local VMGenerate = require("core.mvvm.vm_generate")
local Timer = Hugula.Framework.Timer
local TLogger = CS.TLogger
local is_release = Hugula.Utils.CUtils.isRelease
local NeedProfileDump = not ProfilerFactory.DoNotProfile
local view_load_count = 0
--CS.UnityEngine.SceneManagement.LoadSceneMode
local LoadSceneMode = {
    Single = 0,
    Additive = 1
}
local BindingUtility = Hugula.Databinding.BindingUtility

local vm_manager = {}

local function error_hander(h)
    TLogger.LogError(string_format("lua:%s \r\n %s", h, debug.traceback("", 2)))
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
local empty_tab = {}

local function set_views_index(curr_vm)
    local views = curr_vm.views or empty_tab
    for k, view_base in ipairs(views) do
        view_base._active_index = view_load_count
        view_load_count = view_load_count + 1
    end
end

local function deactive(self, vm_name, view_active)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if view_active == nil then
        view_active = true
    end
    if curr_vm.is_res_ready == true and curr_vm.is_active then --已经加载过并且已经激活
        local p_name = nil
        local profiler
        local profiler1
        if NeedProfileDump then
            p_name = "vm_mamanger.deactive:" .. (curr_vm._require_name or "")
            profiler = ProfilerFactory.GetAndStartProfiler(p_name, nil, nil, true)
            profiler1 = ProfilerFactory.GetAndStartProfiler(p_name .. ":1.on_deactive()", nil, p_name, true)
        end

        safe_call(curr_vm.on_deactive, curr_vm)
        _unbinding_msg(curr_vm)
        curr_vm:stop_all_timer()
        curr_vm.is_active = false --标记状态

        if NeedProfileDump then
            if profiler1 then
                profiler1:Stop()
            end
            profiler1 = ProfilerFactory.GetAndStartProfiler(p_name .. ":2.set_views_active(false)", nil, p_name, true)
        end

        if view_active then
            curr_vm:set_views_active(false)
        end

        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "deactive", name = vm_name})

        if NeedProfileDump then
            if profiler1 then
                profiler1:Stop()
            end
            if profiler then
                profiler:Stop()
            end
        end
    else
        curr_vm.is_active = false --标记状态
    end
end

--- 销毁vm关联的所有view的资源
---@overload fun(vm_name:string)
---@param vm_name string
local function destroy(self, vm_name)
    local curr_vm = VMGenerate[vm_name] --获取vm实例

    if curr_vm.is_res_ready == true then --如果资源加载完成
        local p_name, profiler, profiler1
        if NeedProfileDump then
            p_name = "vm_mamanger.destroy:" .. curr_vm._require_name
            profiler = ProfilerFactory.GetAndStartProfiler(p_name, nil, nil, true)
            profiler1 = ProfilerFactory.GetAndStartProfiler(p_name .. ":1.on_deactive() on_destroy()", nil, p_name, true)
        end
        curr_vm.is_active = false
        curr_vm.is_res_ready = false
        curr_vm._is_destory = nil
        safe_call(curr_vm.on_deactive, curr_vm)
        _unbinding_msg(curr_vm)
        curr_vm:stop_all_timer()
        safe_call(curr_vm.on_destroy, curr_vm)

        if NeedProfileDump then
            if profiler1 then
                profiler1:Stop()
            end
            profiler1 = ProfilerFactory.GetAndStartProfiler(p_name .. ":2.gameobject.destory clear", nil, p_name, true)
        end

        safe_call(curr_vm.clear, curr_vm)
        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "destroy", name = vm_name})

        if curr_vm._need_destructor then --如果是标记了析构
            local destructor = curr_vm.destructor
            if destructor then
                destructor(curr_vm)
            end
        end

        if NeedProfileDump then
            if profiler1 then
                profiler1:Stop()
            end

            if profiler then
                profiler:Stop()
            end
        end
    else
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

    local vvm_name, parent_name, p_root_name, profiler1, profiler2
    if NeedProfileDump then
        vvm_name = vm_base._require_name .. "-" .. view.name
        parent_name = "InstantiateAsync.onComp:" .. view.name
        p_root_name = "check_vm_base_all_done." .. vvm_name
        profiler1 = ProfilerFactory.GetAndStartProfiler(p_root_name .. ":1.set_views_active:", nil, parent_name, true)
    end

    vm_base._isloading = false
    vm_base.is_res_ready = true
    local is_active = vm_base.is_active --当前的状态

    if vm_base._is_push == true then
        -- safe_call(vm_base.on_push, vm_base)
    else
        safe_call(vm_base.on_back, vm_base)
    end

    _binding_msg(vm_base)

    vm_base:set_views_active(is_active)

    safe_call(vm_base.on_assets_load, vm_base)

    if NeedProfileDump then
        if profiler1 then
            profiler1:Stop()
        end
        profiler1 = ProfilerFactory.GetAndStartProfiler(p_root_name .. ":2.on_active:", nil, parent_name, true)
    end

    safe_call(vm_base.on_active, vm_base)

    if NeedProfileDump then
        if profiler1 then
            profiler1:Stop()
        end
        profiler1 = ProfilerFactory.GetAndStartProfiler(p_root_name .. ":3.set_views_context:", nil, parent_name, true)
    end
    vm_base:set_views_context()
    if NeedProfileDump and profiler1 then
        profiler1:Stop()
        profiler1 = nil
    end

    --transition
    local auto_transition = vm_base.auto_transition
    if auto_transition ~= false then
        local do_transition = vm_base.do_transition
        if do_transition then
            do_transition(vm_base)
        end
    end

    --on_state_changed
    if vm_base._is_group == true then
        if NeedProfileDump and not is_release then
            profiler2 =
                ProfilerFactory.GetAndStartProfiler(
                p_root_name .. ":4._check_on_state_changed:",
                nil,
                parent_name,
                true
            )
        end

        vm_manager._vm_state:_check_on_state_changed(vm_base)

        if NeedProfileDump and not is_release then
            if profiler2 then
                profiler2:Stop()
            end
            profiler2 =
                ProfilerFactory.GetAndStartProfiler(p_root_name .. ":5._check_transition:", nil, parent_name, true)
        end

        --check transition
        local _curr_group = vm_base._curr_group
        if _curr_group and _curr_group.transition then
            local need_close = true
            local curr_vm
            for k, v in ipairs(_curr_group) do --多个
                curr_vm = VMGenerate[v]
                if curr_vm.is_res_ready ~= true then
                    need_close = false
                    break
                end
            end

            if need_close then
                local transition = VMGenerate[_curr_group.transition]
                local on_transition_done = transition.on_transition_done
                if on_transition_done then
                    on_transition_done(transition, _curr_group)
                else
                    deactive(vm_manager, _curr_group.transition)
                end
            end
        end

        if NeedProfileDump and not is_release then
            if profiler2 then
                profiler2:Stop()
            end
        end
    end

    if is_active then
        if NeedProfileDump then
            profiler1 =
                ProfilerFactory.GetAndStartProfiler(
                p_root_name .. ":6.after_set_context:DIALOG_OPEN_UI:",
                nil,
                parent_name,
                true
            )
        end

        lua_distribute(DIS_TYPE.DIALOG_OPEN_UI, vm_base._require_name) --触发界面打开消息

        if NeedProfileDump and profiler1 then
            profiler1:Stop()
        end
    end

    if NeedProfileDump and not is_release then
        profiler2 = ProfilerFactory.GetAndStartProfiler(p_root_name .. ":7.end_set_context:", nil, parent_name, false)
    end

    if vm_base._is_destory then --如果标记了销毁
        destroy(vm_manager, vm_base._require_name)
    elseif is_active == false then --非激活状态需要执行deactive逻辑
        vm_base.is_active = true --强行设置为true确保正确的deactive流程
        deactive(vm_manager, vm_base._require_name)
    end

    if NeedProfileDump and not is_release and profiler2 then
        profiler2:Stop()
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
    view_base:initialized() --标记初始化

    local vm_base = view_base._vm_base or viewmodel

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
        local vvm_name, parent_name, p_root_name, profiler
        if NeedProfileDump then
            vvm_name = view_base.name .. "-" .. data.name
            parent_name = "InstantiateAsync.onPreLoadComp:" .. data.name
            p_root_name = "on_pre_load_comp:" .. vvm_name
            profiler =
                ProfilerFactory.GetAndStartProfiler(p_root_name .. ":1.set_views_active:", nil, parent_name, true)
        end
        
        set_view_child(data, view_base)
        data:SetActive(false)

        if NeedProfileDump and profiler then
            profiler:Stop()
        end
    end
end

---场景加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_scene_comp(data, view_base)
    if view_base:has_child() ~= true then
        view_base:set_child(data)
    end

    init_view(view_base)
end

---场景预加载完成
---@overload fun(data:GameObject,view_base:ViewBase)
---@param data GameObject
---@param view_base ViewBase
local function on_pre_scene_comp(data, view_base)
    if view_base:has_child() ~= true then
        view_base:set_child(data)
    end
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
    local find_name = name
    local inst = GameObject.Find(find_name)
    set_view_child(inst, view_base)
    init_view(view_base)
end

--- 将vm关联的所有view激活
---@overload fun(curr_vm:VMBase)
---@param curr_vm VMBase
local function active_view(self, curr_vm)
    ---@type VMBase
    if curr_vm.is_res_ready == true and curr_vm.is_active == false then --已经加载过并且未被激活
        local vm_name = curr_vm._require_name
        local p_name, profiler, profiler1, profiler2

        if NeedProfileDump then
            p_name = "vm_mamanger.active_view:" .. (vm_name or "")
            profiler = ProfilerFactory.GetAndStartProfiler(p_name, nil, nil, true)
        end

        curr_vm.is_active = true

        if curr_vm._is_push == true then
            -- safe_call(curr_vm.on_push, curr_vm)
        else
            safe_call(curr_vm.on_back, curr_vm)
        end

        _binding_msg(curr_vm)

        set_views_index(curr_vm) --设置view激活索引

        curr_vm:set_views_active(true)

        if NeedProfileDump then
            profiler1 = ProfilerFactory.GetAndStartProfiler(p_name .. ".1.on_active", nil, p_name, true)
        end
        safe_call(curr_vm.on_active, curr_vm)
        if NeedProfileDump then
            if profiler1 then
                profiler1:Stop()
            end

            profiler2 = ProfilerFactory.GetAndStartProfiler(p_name .. ".2.set_views_context", nil, p_name, true)
        end
        curr_vm:set_views_context()
        if NeedProfileDump and profiler2 then
            profiler2:Stop()
        end

        --transition
        local auto_transition = curr_vm.auto_transition
        if auto_transition ~= false then
            local do_transition = curr_vm.do_transition
            if do_transition then
                do_transition(curr_vm)
            end
        end

        --on_state_changed 没有资源的模块也需要检测state changed 事件
        if curr_vm._is_group == true then
            vm_manager._vm_state:_check_on_state_changed(curr_vm)

            --check transition
            local _curr_group = curr_vm._curr_group
            if _curr_group and _curr_group.transition then
                local need_close = true
                local curr_vm
                for k, v in ipairs(_curr_group) do --多个
                    curr_vm = VMGenerate[v]
                    if curr_vm.is_res_ready ~= true then
                        need_close = false
                        break
                    end
                end

                if need_close then
                    local transition = VMGenerate[_curr_group.transition]
                    local on_transition_done = transition.on_transition_done
                    if on_transition_done then
                        on_transition_done(transition, _curr_group)
                    else
                        deactive(vm_manager, _curr_group.transition)
                    end
                end
            end
        end

        lua_distribute(DIS_TYPE.DIALOG_OPEN_UI, vm_name) --触发界面打开消息
        lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "active_view", name = vm_name})

        if NeedProfileDump and profiler then
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
    local vm_name = view_base._vm_base._require_name
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
local function load(self, curr_vm, arg, is_push, curr_group)
    if not is_push then --is back
        arg = curr_vm._push_arg
    end
    curr_vm:on_push_arg(arg) --有参数
    curr_vm._push_arg = arg
    curr_vm._is_push = is_push --是否是push到栈上的
    curr_vm._is_group = curr_group ~= nil --是否是组
    curr_vm._curr_group = curr_group
    local views = curr_vm.views

    if views == nil or #views == 0 then
        curr_vm.is_res_ready = true
    end
    if curr_vm.is_res_ready == true then --已经加载过
        active_view(self, curr_vm)
    else
        curr_vm.is_active = true --需要重新加载，设置本身为激活状态
        curr_vm._is_destory = nil --清理销毁标记
        if views and curr_vm._isloading ~= true then
            curr_vm._isloading = true

            local find_path, res_name, scene_name
            for k, v in ipairs(views) do
                -- active index
                v._active_index = view_load_count
                view_load_count = view_load_count + 1
                --
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
local function active(self, vm_name, arg, is_push)
    if vm_name == nil then
        error("VMManager.active vm_name is nil")
    end

    load(self, VMGenerate[vm_name], arg, is_push)

    lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "active", name = vm_name})
end

local function active_group(self, vm_group, arg, is_push, vm_group)
    for k, v in ipairs(vm_group) do --多个
        load(self, VMGenerate[v], arg, is_push, vm_group)
    end

    lua_distribute(DIS_TYPE.ON_UI_STATE_CHANGE, {action = "active", name = vm_group.name})
end

--真正开始切换
local function do_transition(self)
    local _change_action = self._change_action
    if _change_action then
        _change_action()
    end
    self._on_do_transition(unpack(self._on_do_transition_arg))
    -- self.do_transition = nil --只能执行一次
end

--判断是否需要loading切换
local function _transition_active_group(self, vm_group, is_push, _change_action, arg)
    local transition = vm_group.transition
    if transition then --check show loading
        local curr_vm, views
        local transition_item = VMGenerate[transition]
        local always_transition = transition_item.always_transition
        local need_trans = false

        if always_transition ~= true then
            for k, v in ipairs(vm_group) do --多个
                curr_vm = VMGenerate[v]
                views = curr_vm.views
                if not (views == nil or #views == 0) and curr_vm.is_res_ready ~= true then
                    need_trans = true
                    break
                end
            end
        else
            need_trans = true
        end

        if need_trans then
            transition_item._on_do_transition = active_group
            transition_item._on_do_transition_arg = {self, vm_group, arg, is_push, vm_group}
            transition_item.do_transition = do_transition

            transition_item._change_action = _change_action

            load(self, transition_item, vm_group, true) --激活loading
            return
        end
    end

    if _change_action then
        _change_action()
    end
    active_group(self, vm_group, arg, is_push, vm_group)
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
        active_view(self, curr_vm)
    end
end

local function release(self, vm_name)
    VMGenerate:release_vm(vm_name) --彻底释放
end

vm_manager.re_load = re_load
vm_manager.pre_load = pre_load
vm_manager.load = load
vm_manager.active = active
vm_manager._transition_active_group = _transition_active_group
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
