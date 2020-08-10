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
local type = type
local require = require

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
---@class VMState
---
local vm_state = {}
local _stack = {} --记录栈
local _root_index = 0 --当前group root的stack索引
local _item_index = {} --当前item的最新stack索引

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
    Logger.Log(str)
end

local function call_func(self, vm_name, fun_name, arg)
    -- Logger.Log("call_func",vm_name,fun_name,arg)
    local curr_vm = VMGenerate[vm_name] --获取vm实例
    if curr_vm then
        local fun = curr_vm[fun_name]
        if fun ~= nil then
            fun(curr_vm, arg)
        end
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

    if vm_gc == VM_GC_TYPE.AUTO then --内存不够的时候回收
        --todo 检查内存
        VMManager:deactive_view(vm_name)
    elseif vm_gc == VM_GC_TYPE.ALWAYS then
        VMManager:destory_view(vm_name)
    elseif vm_gc == VM_GC_TYPE.NEVER then
        VMManager:deactive_view(vm_name)
    elseif vm_gc == VM_GC_TYPE.STATE_CHANGED then
        if is_state_change == true then
            VMManager:destory_view(vm_name)
        else
            VMManager:deactive_view(vm_name)
        end
    else --只执行vm的on_deactive方法自己隐藏或者回收 vm_gc ==  VM_GC_TYPE.MANUAL
        local curr_vm = VMGenerate[vm_name] --获取vm实例
        curr_vm.is_active = false
        curr_vm:on_deactive()
    end
end

---失活当前栈顶以下的state
---@overload fun()
local function hide_group(curr)
    local item
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == TYPE_TABLE then --如果是root group
            local log_enable = item.log_enable
            for k, v in ipairs(item) do
                if table_indexof(curr, v) == nil then --马上要使用的item不需要回收
                    strategy_view_gc(v, true) --在group里不需要对item做栈操作
                end
            end

            if log_enable == false then --如果不需要记录root log栈
                table_remove(_stack,i)
                -- debug_stack() 
            end
            return
        else
            if table_indexof(curr, item) == nil then
                strategy_view_gc(item, true,i) --需要判断出stack
            end
        end
    end
end

---入栈单个模块
---@overload fun(vm_name:string,arg:any)
---@param vm_config.name string
local function push_item(self, vm_name, arg)
    local curr_index = _item_index[vm_name]
    if curr_index ~= nil and curr_index > _root_index then
        table_remove(_stack, curr_index) --移除
    end
    table_insert(_stack, vm_name) --- 进入显示stack
    _item_index[vm_name] = #_stack --更新新的索引
    VMManager:active(vm_name, arg, true) ---激活组
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
        _item_index[item] = nil
        table_remove(_stack, del)
        strategy_view_gc(item, false)
        -- debug_stack()
        return true
    end

    return false
end

---入栈group
---@overload fun(vm_group_name:string,arg:any)
---@param vm_group_name string
local function push(self, vm_group_name, arg)
    local vm_group = _VMGroup[vm_group_name]
    local tp = type(vm_group)
    if tp == TYPE_TABLE then ---如果是加入的是root 需要隐藏到上一个root的所有栈内容
        VMManager:_call_on_state_changed(vm_group) --调用on_state_changed方法
        hide_group(vm_group)
    end
    table_insert(_stack, vm_group) --- 进入显示stack
    _root_index = #_stack
    VMManager:active(vm_group, arg, true) ---激活组
    -- debug_stack()
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
    strategy_view_gc(vm_name,false)
end


local remove, active = {}, {}

--移除最顶上的项目
local function remove_pop()
    local curr = table_remove(_stack, #_stack) ---移除最顶上的

    if type(curr) == TYPE_TABLE then --如果当前最顶上的是vm group 则要找到下一个vm group
        table_clear(active)
        local item
        --激活项
        for i = #_stack, 1, -1 do
            item = _stack[i]
            if type(item) == TYPE_TABLE then
                for k, v in ipairs(item) do
                    table_insert(active, v)
                end
                _root_index = i --记录root
                break
            else
                table_insert(active, item)
            end
        end

        --deactive
        for k, v in ipairs(curr) do
            if table_indexof(active, v) == nil then --在激活名单的不用激活
                strategy_view_gc(v, true)
            end
        end

        --active
        for k, v in ipairs(active) do
            VMManager:load(v) --重新激活
        end
    else
        strategy_view_gc(curr, false)
    end
end

--- 返回上个状态
---@overload fun()
---
local function back(self)
    local item
    table_clear(remove)
    table_clear(active)
    for i = #_stack, _root_index, -1 do
        item = _stack[i]
        if i == _root_index then
            for k, v in ipairs(item) do
                table_insert(remove, v)
            end
        else
            table_insert(remove, item)
        end
        table_remove(_stack, i)
        _item_index[item] = nil --更新的索引
    end

    --激活项
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if type(item) == TYPE_TABLE then
            for k, v in ipairs(item) do
                table_insert(active, v)
            end
            _root_index = i --记录root
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
end

vm_state.get_member = get_member
vm_state.call_func = call_func
vm_state.push = push
vm_state.push_item = push_item
vm_state.popup_item = popup_item
vm_state.back = back
vm_state.remove_pop = remove_pop
vm_state.active = active_vm --激活当前viewmodel 不入栈
vm_state.deactive = deactive_vm --失活当前viewmodel与上面配对。
--- view model 的显示隐藏管理
---@class VMState
---@field push function
---@field push_item   function
---@field popup_item function
---@field back  function
---@field remove_pop function
---@field active function
---@field deactive function
VMState = vm_state
