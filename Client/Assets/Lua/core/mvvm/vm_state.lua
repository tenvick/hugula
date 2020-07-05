------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table_insert = table.insert
local table_remove = table.remove
local table_remove_item = table.remove_item
local table_indexof = table.indexof
local unpack = table.unpack
local ipairs = ipairs
local type = type
local require = require

local VM_GC_TYPE = VM_GC_TYPE
local VMConfig,_VMGroup = unpack(require("vm_config"))
local VMManager = require("core.mvvm.vm_manager")
local VMGenerate = require("core.mvvm.vm_generate")

---
---     下面是一个典型的vm stack示例：{}表示根状态,""表示追加状态, 根状态来控制stack上的UI显示。
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
local _stack = {}

---根据策略判断回收view
---@overload fun(vm_name:string,is_popup:boolean,is_state_change:boolean)
---@param vm_name string
---@param is_popup boolean
local function strategy_view_gc(vm_name, is_popup, is_state_change)
    local config_item = VMConfig[vm_name]
    local vm_gc = VM_GC_TYPE.AUTO
    if config_item.gc_type ~= nil then
        vm_gc = config_item.gc_type
    end
    -- Logger.Log("strategy_view_gc",vm_name,vm_gc)
    if config_item.log_enable == true then --如果当前viewmodel模块不需要记录log栈
        table_remove_item(_stack, vm_name)
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
    local len = #_stack - 1
    local item

    for i = len, 1, -1 do
        item = _stack[i]
        if type(item) == "table" then --如果是group
            for k, v in ipairs(item) do
                -- Logger.Log("hide_group.table  ",table_indexof(curr, k),k)
                if table_indexof(curr, v) == nil then
                    strategy_view_gc(v, false, true)
                end
            end

            if item.log_enable == false then --如果不需要记录log栈
                table_remove(_stack, i) --从当前栈移除
            -- Logger.Log(" pop stack", item[1])
            end
            return
        else
            -- Logger.Log("hide_group.string ",table_indexof(curr, item),item)
            if table_indexof(curr, item) == nil then
                strategy_view_gc(item, false, true)
            end
        end
    end
end

---入栈单个模块
---@overload fun(vm_name:string,arg:any)
---@param vm_config.name string
local function push_item(self, vm_name, arg)
    table_insert(_stack, vm_name) --- 进入显示stack
    VMManager:active(vm_name, arg) ---激活组
    --todo item 互斥流程
end

---入栈group
---@overload fun(vm_group_name:string,arg:any)
---@param vm_group_name string
local function push(self, vm_group_name, arg)
    local vm_group = _VMGroup[vm_group_name]
    table_insert(_stack, vm_group) --- 进入显示stack
    if type(vm_group) == "table" then ---如果是加入的是root 需要隐藏到上一个root的所有栈内容
        VMManager:_call_on_state_changed() --调用on_state_changed方法
        hide_group(vm_group)
    end
    VMManager:active(vm_group, arg) ---激活组
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
        elseif type(item) == "table" then --如果遇到root不需要移除vm
            return false
        end
    end

    if del > 0 then
        table_remove(_stack, del)
        strategy_view_gc(item, true, false)
        return true
    end

    return false
end

--- 返回上个状态
---@overload fun()
---
local function back(self)
    local len = #_stack
    local curr = _stack[len]
    table_remove(_stack, len) ---移除最顶上的

    if type(curr) == "table" then --如果当前最顶上的是vm group 则要找到下一个vm group
        local active = {}

        local item
        --激活项
        for i = #_stack, 1, -1 do
            item = _stack[i]
            if type(item) == "table" then
                for k, v in ipairs(item) do
                    table_insert(active, v)
                end
                break
            else
                table_insert(active, item)
            end
        end

        --deactive
        for k, v in ipairs(curr) do
            if table_indexof(active, v) == nil then --在激活名单的不用激活
                strategy_view_gc(v, true, true)
            end
        end

        --active
        for k, v in ipairs(active) do
            VMManager:load(v) --重新激活
        end
    else
        strategy_view_gc(curr, true, false)
    end
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

vm_state.get_member = get_member
vm_state.call_func = call_func
vm_state.push = push
vm_state.push_item = push_item
vm_state.popup_item = popup_item
vm_state.back = back

--- view model 的显示隐藏管理
---@class VMState
---@field push function
---@field push_item   function
---@field popup_item function
---@field back  function
VMState = vm_state
