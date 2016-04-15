------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaItemManager = LuaItemManager
local Resources = UnityEngine.Resources
-------------------------------private function------------------------------------
--状态与日志对比查找出不同项目
--返回值 {change bool,add table，remove table}
local function get_diff_log(curr_state,curr_log) 
    local change = false
    local items = curr_state:get_all_items() --当前的所有项目
    local add,remove = {},{}
    -- print("get_diff_log")
    if curr_log == nil or curr_log == false then --如果没有记录
        change = true
        return change,add,remove
    end
    -- print(curr_log)
    local check_dup = {}
    local log_state = curr_log[1]
    local check_state = curr_state

    if log_state ~= curr_state and log_state ~= nil then --如果不同状态
        check_state = log_state
        items = log_state:get_all_items()
        change = true
    end

    for k,v in ipairs(items) do
        -- print(v,v.log_enable,check_state:is_original_item(v))
        if v.log_enable and not check_state:is_original_item(v) then --不是原始项目并且可以log
            if check_dup[v] == nil then 
                check_dup[v] = -1 -- -1 表示需要删除
            elseif check_dup[v] == 1 then --1 表示log表中存在
                check_dup[v] = 0 --不需要改变
            end
            -- print("need ",v,check_dup[v])
        end

        for ke,va in ipairs(curr_log) do
            if ke > 1 then --第一个是状态不需要判断
                if check_dup[va] == nil then --如果没有默认为1
                    check_dup[va] = 1 -- 1表示增加
                end

                if va.log_enable and check_dup[va] == -1 then
                    check_dup[va] = 0 --不需要改变
                end    
            end
        end
    end --end for

    for k,v in pairs(check_dup) do
        if v == 1 then
            change = true
            table.insert(add,k)
        elseif v == -1 then
            change = true
            table.insert(remove,k)
        end
    end -- end for

    return change,add,remove

end
-------------------------------------------------------------------
StateManager =
{
    _current_game_state = nil,
    _auto_show_loading = true,
    _transform = nil,
    _log_state = LuaStack(8), --状态日志
    _input_enable = true --输入控制
}

--锁定输入可以输入
function StateManager:input_disable()
    self._input_enable = false
end

--可以输入
function StateManager:input_enable()
    self._input_enable = true
end

function StateManager:get_input_enable()
    return self._input_enable
end
--得到当前状态
function StateManager:get_current_state()
    return self._current_game_state
end

--设置统一场景切换效果
function StateManager:set_state_transform(transform)
    self._transform = transform
end

--显示切换UI
function StateManager:show_transform()
    if self._transform then self._transform:on_focus() end
end

--设置切换效果的显示模式
function StateManager:set_loading(bl)
    if bl then self._auto_show_loading = true
    else self._auto_show_loading = false end
end

--隐藏切换效果
function StateManager:hide_transform()
    if self._transform then self._transform:hide() end
end

--检测显示切换效果
function StateManager:check_show_transform( ... )
    if self._auto_show_loading then self:show_transform() end
end

--检测隐藏切换效果
function StateManager:check_hide_transform( ... )
    if self._auto_show_loading then self:hide_transform() end
end

--调用设置的itemobject方法
function StateManager:call_all_item_method( )
    local curr_state = self._current_game_state
    if curr_state.method then
        curr_state:on_event(curr_state.method,unpack(curr_state.args))
        curr_state.method = nil
        curr_state.args = nil
    end
end

--改变状态
function StateManager:set_current_state(new_state,method,...)
    assert(new_state ~= nil)
    if(new_state == self._current_game_state)   then
        print("setCurrent State: "..tostring(new_state).." is same as currentState"..tostring(self._current_game_state))
        return
    end
    -- print("new_state",new_state)
    if(self._current_game_state ~= nil) then
        self._current_game_state:on_blur(new_state)
    end

    local previousState = self._current_game_state
    self._current_game_state = new_state

    new_state.method=method
    new_state.args={...}

    new_state:on_focus(previousState)

    if new_state:is_all_loaded() then self:call_all_item_method() end

    unload_unused_assets()

    self:record_state() --记录状态用于返回
end

function StateManager:record_state() --记录状态用于返回
    local curr_state = self._current_game_state --当前状态
    if curr_state.log_enable == false then 
        self._log_state:push(false) --空位置
        return nil 
    end

    -- if self._record_enable == false and force_record == nil then return nil end --如果不能记录同时force_record没有值 直接返回

    local curr_log =  self._log_state:get(0) --get_log(self._log_state,-1) --上一个状态
    local need_record = get_diff_log(curr_state,curr_log) --是否需要记录
    
    if need_record  then --可以记录
        local items = curr_state:get_all_items() --得到所有item
        local log_state = {}
        table.insert(log_state,curr_state) --第一个位置为当前状态
        for k,v in ipairs(items) do
            if v.log_enable and not curr_state:is_original_item(v) then --log_enable == true 
                table.insert(log_state,v) --当前项
            end
        end
        self._log_state:push(log_state)
    end

end

function StateManager:go_back(index) --返回
    assert(type(index) == "number"," StateManager:go_back index is not number")
    if index == nil then index = -1 end
    if index > 0  then index = -1 end

    local back_log = self._log_state:get(index) --上一个状态
    while back_log == false do
        self._log_state:pop(1) --删除顶上第一个
        back_log = self._log_state:get(index) --上一个状态
    end
    self._log_state:pop(1) --删除顶上第一个

    if back_log == nil then return nil end 
    local new_state = back_log[1] --pos 1 is state
    assert(new_state ~= nil,"返回的状态不能为空！")

    --添加状态
    local change,add,remove = get_diff_log(self._current_game_state,back_log)
    if new_state == self._current_game_state then --如果是当前状态
        local curr = self._current_game_state
        for k,v in ipairs(add) do 
            curr:add_item(v)
            v:on_focus(curr)
            if v.onback then v:on_back() end
        end

        for k,v in ipairs(remove) do 
            curr:remove_item(v)
            v:on_blur(curr)
            -- v:on_back()
        end
    else

        for k,v in ipairs(add) do 
            new_state:add_item(v)
        end

        for k,v in ipairs(remove) do 
            new_state:remove_item(v)
            v:on_blur(self._current_game_state)
        end

        if  self._current_game_state ~= nil then
            self._current_game_state:on_blur(new_state)
        end
    
        local previous_state = self._current_game_state
        self._current_game_state = new_state
        new_state:on_focus(previous_state)
        new_state:on_back(previous_state)
    end

end