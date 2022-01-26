------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table_insert = table.insert
local table_remove = table.remove
local table_clear = table.clear
local table_remove_item = table.remove_item
local table_indexof = table.indexof
local unpack = table.unpack
local ipairs = ipairs
local pairs = pairs
local type = type
local require = require

local lua_distribute = lua_distribute
local DIS_TYPE = DIS_TYPE
local set_target_context = BindingExpression.set_target_context
local VM_GC_TYPE = VM_GC_TYPE
local VMConfig, _VMGroup = unpack(require("vm_config"))
local VMManager = require("core.mvvm.vm_manager")
local VMGenerate = require("core.mvvm.vm_generate")

local TYPE_TABLE = "table"
---
---     下面是一个典型的vm stack示例：{}表示根m_state:状态,""表示追加状态, 根状态来控制stack上的UI显示。
------------------------------------------------------
---      7   {lobby,chatui,...}      root---返回大厅                (隐藏6)
---      6   {"loading"}                ---返回大厅                 (隐藏5,4)
---      5   "battle_results"           ---战斗结算界面             (不隐藏任)
---      5   "battle_setting"           ---战斗设置界面             (不隐藏任何,关闭后出栈)
---      4   {battle,battle_ui}      root---战斗场景                (隐藏3)
---      3   {battle_loading}        root---战斗匹配与资源预加载     (隐藏2,1)
---      3   "alert_tips"               ---领取奖励提示             (模态界面关闭之后自动出栈)
---      2   "daily_rewards"            ---日常奖励弹窗             (不隐藏任何)
---      1   {lobby,chatui,...}      root---大厅                    (自动隐藏 0)
---      0   {loading}                  ---加载界面
------------------------------------------------------
local ON_STATE_CHANGING = "on_state_changing"
local DISTYPE_STATE_CHANGED = "on_state_changed"

---@type VMState
local vm_state = {}
local _stack = {} --记录栈
local _root_index = 0 --当前group root的stack索引

local function debug_stack()
    local str, item = ""
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == "string" then
            str = str .. "\r\n" .. item
        elseif type(item) == TYPE_TABLE then
            local tb_str = ""
            for k, v in ipairs(item) do
                tb_str = tb_str .. "," .. v
            end
            str = str .. "\r\n{" .. tb_str .. "}"
        end
    end
    Logger.Log("debug_stack", str)
end

local function call_func(self, vm_name, fun_name, ...)
    -- Logger.Log("call_func",vm_name,fun_name,...)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if curr_vm then
        local fun = curr_vm[fun_name]
        if fun ~= nil then
            return fun(curr_vm, ...)
        end
    end
end

local function call_rawget_func(self, vm_name, fun_name, ...)
    -- Logger.Log("call_func",vm_name,fun_name,...)
    local curr_vm = VMGenerate:rawget(vm_name) --获取vm实例
    if curr_vm then
        local fun = curr_vm[fun_name]
        if fun ~= nil then
            return fun(curr_vm, ...)
        end
    end
end

local function _call_group_func(self, group, fun_name, ...)
    for k, vm_name in ipairs(group) do
        -- if k ~= "log_enable" then
        local curr_vm = VMGenerate:rawget(vm_name) --获取vm实例
        if curr_vm then
            local fun = curr_vm[fun_name]
            if fun ~= nil then
                fun(curr_vm, ...)
            end
        end
    end
end

local function call_top_func(self, fun_name, ...)
    _call_group_func(self, self.top_group, fun_name, ...)
end

local function call_last_top_func(self, fun_name, ...)
    local last = self.last_group
    if last then
        _call_group_func(self, last, fun_name, ...)
    end
end

local function get_member(self, vm_name, member_name)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if curr_vm then
        if member_name == "." or member_name == nil then
            return curr_vm
        else
            local fun = curr_vm[member_name]
            return fun
        end
    end
    return nil
end

local function set_top_group(self, vm_group)
    self.last_group = self.top_group --上一次记录
    self.top_group = vm_group --记录当前顶
    self.last_group_name = self.top_group_name
    self.top_group_name = vm_group.name
    -- Logger.Log("last_group_name ", self.last_group_name, "top_group_name ", self.top_group_name)
end

---根据策略判断回收view
---@overload fun(vm_name:string,is_state_change:boolean,stack_index:int)
---@param vm_name string
---@param is_state_change boolean
---@return void
local function strategy_view_gc(vm_name, is_state_change, stack_index)
    local config_item = VMConfig[vm_name]
    local vm_gc = VM_GC_TYPE.AUTO
    if config_item.gc_type ~= nil then
        vm_gc = config_item.gc_type
    end

    if config_item.log_enable == false and stack_index ~= nil then
        table_remove(_stack, stack_index)
    end

    if is_state_change then --
        call_rawget_func(nil, vm_name, ON_STATE_CHANGING)
    end

    if vm_gc == VM_GC_TYPE.AUTO then --内存不够的时候回收
        --todo 检查内存
        VMManager:deactive(vm_name)
    elseif vm_gc == VM_GC_TYPE.ALWAYS then
        VMManager:destroy(vm_name)
    elseif vm_gc == VM_GC_TYPE.NEVER then
        VMManager:deactive(vm_name)
    elseif vm_gc == VM_GC_TYPE.STATE_CHANGED then
        if is_state_change == true then
            VMManager:destroy(vm_name)
        else
            VMManager:deactive(vm_name)
        end
    elseif vm_gc == VM_GC_TYPE.MANUAL then --只执行vm的on_deactive方法自己隐藏或者回收 vm_gc ==  VM_GC_TYPE.MANUAL
        VMManager:deactive(vm_name, false) --不隐藏ui
    elseif vm_gc == VM_GC_TYPE.RELEASE then
        VMManager:release(vm_name)
    end
end

local function _check_on_state_changed(self, vm_base)
    local top = self.top_group
    if top then
        local check_isin_top = false
        for k, v in ipairs(top) do
            local curr_vm = VMGenerate[v] --获取vm实例
            if curr_vm.is_res_ready ~= true or curr_vm.is_active == false then
                return false
            end

            if curr_vm == vm_base then
                check_isin_top = true
            end
        end

        if check_isin_top then
            local last_group_name = self.last_group_name
            local top_group_name = self.top_group_name
            call_last_top_func(self, DISTYPE_STATE_CHANGED, last_group_name, top_group_name)
            call_top_func(self, DISTYPE_STATE_CHANGED, last_group_name, top_group_name)
            lua_distribute(DIS_TYPE.ON_STATE_CHANGED, last_group_name, top_group_name) --状态改变完成后触发
        end
    end
end

---失活当前栈顶以下的state
---@overload fun()
local function hide_group(curr)
    local item
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == TYPE_TABLE then --如果是 group
            local log_enable = item.log_enable
            item:clear_append() --清理附加
            for k, v in ipairs(item) do
                if table_indexof(curr, v) == nil then --马上要使用的item不需要回收
                    strategy_view_gc(v, true) --在group里不需要对item做栈操作
                end
            end

            if log_enable == false then --如果不需要记录root log栈
                table_remove(_stack, i)
            -- debug_stack()
            end
            return
        else
            if table_indexof(curr, item) == nil then
                strategy_view_gc(item, true, i) --需要判断出stack
            end
        end
    end
end

---追加单个模块到当前group与group一起返回
---@overload fun(vm_name:string,arg:any)
---@param vm_config.name string
local function append_item(self, vm_name, arg)
    for i = #_stack, _root_index, -1 do
        if _stack[i] == vm_name then
            table_remove(_stack, i) --移除
            break
        end
    end

    table_insert(_stack, vm_name) --- 进入显示stack
    self.top_group:append_item(vm_name)
    VMManager:active(vm_name, arg, true, false) ---激活组
    -- debug_stack()
end

---入栈单个模块
---@overload fun(vm_name:string,arg:any)
---@param vm_config.name string
local function push_item(self, vm_name, arg)
    for i = #_stack, _root_index, -1 do
        if _stack[i] == vm_name then
            table_remove(_stack, i) --移除
            break
        end
    end

    table_insert(_stack, vm_name) --- 进入显示stack
    VMManager:active(vm_name, arg, true, false) ---激活组
    -- debug_stack()
end

---移除view model追加项目,只有追加项目才需要手动移除
---@overload fun(vm:string)
---@param vm string
---@return boolean
local function popup_item(self, vm)
    if type(vm) ~= "string" then
        return false
    end

    --- 寻找当前 vm位置
    local len, del = #_stack, 0
    local item
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if item == vm then
            del = i
            break
        elseif type(item) == TYPE_TABLE then --如果遇到root不需要移除vm
            return false
        end
    end

    if del > 0 then
        table_remove(_stack, del)
        strategy_view_gc(vm, false)
        self.top_group:remove_append_item(vm)
        -- debug_stack()
        return true
    end

    return false
end

---入栈group
---@overload fun(vm_group_name:string,arg:any)
---@param vm_group_name string
local function push(self, vm_group_name, arg)
    if self.top_group_name == vm_group_name then --如果与当前相同不需要push
        return
    end
    local vm_group = _VMGroup[vm_group_name]
    -- vm_group.name = vm_group_name
    local tp = type(vm_group)
    if tp == TYPE_TABLE then ---如果是加入的是root 需要隐藏到上一个root的所有栈内容
        hide_group(vm_group)
    end
    table_insert(_stack, vm_group) --- 进入显示stack
    _root_index = #_stack

    set_top_group(self, vm_group)
    VMManager:active(vm_group, arg, true, true) ---激活组
    -- debug_stack()
end

---获取顶部的group
---@overload fun():luatable
---@param vm_group_name string
local function top_group_is(self, vm_group_name)
    return self.top_group == _VMGroup[vm_group_name]
end

---激活active不放入栈
---@overload fun(vm_name:string,arg:any)
---@param vm_name string
local function active_vm(self, vm_name, arg)
    VMManager:active(vm_name, arg, false) ---激活组
end

---失活
---@overload fun(vm_name:string,arg:any)
---@param vm_name string
local function deactive_vm(self, vm_name)
    strategy_view_gc(vm_name, false)
end

---设置root组，返回的时候到这里就停止
---@overload fun(self:VMState,root:{Group})
---@param vm_name string
local function set_root(self, root)
    self._root = _VMGroup[root]
end

local remove, active = {}, {}

--- 返回上个组状态
---@overload fun()
---
local function back(self)
    local item
    local group_changed = false
    table_clear(remove)
    table_clear(active)
    for i = #_stack, _root_index, -1 do
        item = _stack[i]
        if item == self._root then --如果是根模块不需要移除
            return true
        end
        if i == _root_index then
            for k, v in ipairs(item) do
                table_insert(remove, v)
            end
        else
            table_insert(remove, item)
        end
        table_remove(_stack, i)
    end

    --激活项
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == TYPE_TABLE then
            for k, v in ipairs(item) do
                table_insert(active, v)
            end
            _root_index = i --记录root
            set_top_group(self, item)
            group_changed = true
            break
        else
            table_insert(active, item)
        end
    end

    for k, v in ipairs(remove) do
        if table_indexof(active, v) == nil then --在激活名单不需要deactive
            strategy_view_gc(v, true)
        end
    end

    for k, v in ipairs(active) do
        VMManager:load(v) --重新激活
    end

    -- debug_stack()
    if group_changed then
        lua_distribute(DIS_TYPE.ON_STATE_CHANGED, self.last_group_name, self.top_group_name, true) --状态改变完成后触发
    end
    return false
end

--- 弹出顶上模块或者组
---@overload fun()
---
local function popup_top(self)
    local item
    table_clear(remove)
    table_clear(active)

    local log_enable
    --hide
    local group_changed = false
    local top_group = self.top_group
    local append_items = {}
    for i = #_stack, 1, -1 do --寻找可以弹出的模块
        item = _stack[i]
        if type(item) == TYPE_TABLE then
            if item == self._root then --如果是根模块不需要移除
                return true
            end
            for k, v in ipairs(item) do
                table_insert(remove, v)
                -- Logger.Log("移除当前组模块", k, v)
            end

            for k, v in ipairs(append_items) do
                table_insert(remove, _stack[v])
                -- Logger.Log("移除追加模块", _stack[v], v)
                table_remove(_stack, v) --
            end

            item:clear_append()
            group_changed = true
            table_remove(_stack, i) --移除当前栈
            break
        elseif (not top_group:contains_append_item(item)) then --非追加项目需要弹出
            -- log_enable = VMConfig[item].log_enable
            table_insert(remove, item)
            table_remove(_stack, i) --移除当前栈
            break
        else
            table_insert(append_items, i)
        end
    end

    -- --激活项
    if group_changed == true then --如果上一个root被隐藏了
        for i = #_stack, 1, -1 do
            item = _stack[i]
            if type(item) == TYPE_TABLE then --一直要寻找到下一个root
                for k, v in ipairs(item) do
                    table_insert(active, v)
                end
                _root_index = i --记录root
                set_top_group(self, item)
                break
            else
                table_insert(active, item)
            end
        end
    end

    for k, v in ipairs(remove) do
        if table_indexof(active, v) == nil then --在激活名单不需要deactive
            strategy_view_gc(v, true)
        end
    end

    for k, v in ipairs(active) do
        VMManager:load(v) --重新激活
    end

    -- debug_stack()
    if group_changed then
        lua_distribute(DIS_TYPE.ON_STATE_CHANGED, self.last_group_name, self.top_group_name, true) --状态改变完成后触发
    end
    return false
end

local function init_viewmodel(self, vm_name, container)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if curr_vm then
        -- BindingUtility.SetContextByINotifyTable(bindable_object, context)
        if curr_vm.auto_context then
            VMManager:active(vm_name, nil, false) ---激活组
        end
        set_target_context(container, curr_vm)
    end
end

local function destroy_viewmodel(self, vm_name)
    VMManager:destroy(vm_name)
end

local function get_vm_manager(self)
    return VMManager
end

local function _reload_top_one(self)
    local top = _stack[#_stack] -- top 栈顶

    local function do_reload(vm_name)
        Logger.Log("hot reload viewmodel：", vm_name)
        VMManager:re_load(vm_name)
    end

    if type(top) == "table" then
        for k, v in ipairs(top) do
            if type(v) == "string" then
                do_reload(v)
            end
        end
    else
        do_reload(top)
    end
end

local function _reload_top_active(self)
    -- local top = _stack[#_stack] -- top 栈顶

    local function do_reload(vm_name)
        Logger.Log("hot reload viewmodel：", vm_name)
        VMManager:re_load(vm_name)
    end

    --刷新当前所有激活的模块
    local item
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == TYPE_TABLE then --如果是root group
            for k, v in ipairs(item) do
                do_reload(v)
            end
            return
        else
            do_reload(item)
        end
    end
end

VMManager._vm_state = vm_state
vm_state.get_member = get_member
vm_state.call_func = call_func
vm_state.push = push
vm_state.append_item = append_item
vm_state.push_item = push_item
vm_state.popup_item = popup_item
vm_state.back = back
vm_state.back_group = back

vm_state.popup_top = popup_top
vm_state.set_root = set_root

vm_state.active = active_vm --激活当前viewmodel 不入栈
vm_state.deactive = deactive_vm --失活当前viewmodel与上面配对。
vm_state.init_viewmodel = init_viewmodel -- 初始化viewmodel并激活用于组件初始化
vm_state.destroy_viewmodel = destroy_viewmodel
vm_state.get_vm_manager = get_vm_manager --
vm_state._check_on_state_changed = _check_on_state_changed --检查当前顶上的group是否全部激活
vm_state._reload_top_one = _reload_top_one
vm_state._reload_top_active = _reload_top_active
vm_state.top_group_is = top_group_is
vm_state.debug_stack = debug_stack
--- view model 的显示隐藏管理
---@class VMState
---@field get_member fun(self:VMState, vm_name:string)
---@field call_func fun(self:VMState, vm_name:string, fun_name:string, arg:any)
---@field push fun(self:VMState, vm_group_name:string, arg:any)
---@field push_item   fun(self:VMState, vm_name:string, arg:any)
---@field popup_item fun(self:VMState, vm:string)
---@field back  fun(self:VMState) 返回上个组
---@field popup_top fun(self:VMState) 弹出最顶上的模块或者组
---@field active fun(self:VMState, vm_name:string, arg:any)
---@field deactive fun(self:VMState, vm_name:string)
---@field init_viewmodel fun(self:VMState, vm_name:string, container:BindableContainer)
---@field destroymodel fun(self:VMState, vm_name:string)
---@field get_vm_manager fun(self:VMState):VMManager
---@field _check_on_state_changed fun(self:VMState,vm_base:VMBase)
---@field _reload_top_one fun(self:VMState) reload 栈顶的一个模块
---@field _reload_top_active fun(self:VMState) reload 栈顶的所有模块

---@field top_group_is fun(self:VMState,vm_group_name:string) 判断顶部的group
VMState = vm_state
