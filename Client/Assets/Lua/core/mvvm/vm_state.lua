------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table_insert = table.insert
local table_remove = table.remove
local type = type

local VMManager = VMManager

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

---失活当前栈顶以下的state
---@overload fun()
local function hide_group()
    local len = #_stack - 1
    local item

    for i = len, 1, -1 do
        item = _stack[i]
        VMManager.deactive(item)
        if type(item) == "table" then
            return
        end
    end
end

---入栈 单个或者group
---@overload fun(vm_group:any)
---@param vm_group any
local function push(vm_group)
    VMManager.active(vm_group) ---激活组
    table_insert(_stack, vm_group) --- 进入显示stack
    if type(vm_group) == "table" then ---如果是加入的是root 需要隐藏到上一个root的所有栈内容
        hide_group()
    end
end

---移除view model追加项目,只有追加项目才需要手动移除
---@overload fun(vm:string)
---@param vm string
---@return boolean
local function remove_item(vm)
    if type(vm) ~= "string" then return false end   

    --- 寻找当前 vm位置
    local len,del = #_stack,0
    local item
    for i = #_stack, 1, -1 do
        item = _stack[i]
        if item == vm then
            del = i
            break
        elseif type(item) == "table" then --如果遇到root不需要移除vm item因为
            return false
        end
    end

    if del > 0 then
        table_remove(_stack,del)
        VMManager.deactive(item) ---
        return true
    end

    return false
end


--- 返回上个状态
---@overload fun()
---
local function back()
    local len = #_stack
    local curr = _stack[len]
    table_remove(_stack, len) ---移除最顶上的
    VMManager.deactive(curr) --
    if type(curr) == "table" then --如果当前最顶上的是vm group 则要找到下一个vm group
        local item
        for i = #_stack, 1, -1 do
            item = _stack[i]
            Logger.Log("active",item)
            VMManager.active(item) --重新激活
            if type(item) == "table" then
                return
            end
        end
    end
end

vm_state.push = push
vm_state.remove_item = remove_item
vm_state.back = back

--- view model 的显示隐藏管理
---@class VMState
---@field push function
---@field remove_item   function
---@field back  function
VMState = vm_state