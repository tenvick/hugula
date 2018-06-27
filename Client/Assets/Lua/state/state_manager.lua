------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaItemManager = LuaItemManager
local Resources = UnityEngine.Resources
local HugulaProfiler = Hugula.Pool.HugulaProfiler
-------------------------------private function------------------------------------
--状态与日志对比查找出不同项目
local function check_need_log(curr_state,curr_log)  

    if not curr_state.log_enable then return false end 
    
    if curr_log == nil or curr_log == false  then --如果没有记录
        return true
    end

    local change = false
    local items = curr_state:get_all_items() --当前的所有项目
    local check_dup = {}
    local count = 0
    for k,v in ipairs(items) do
        if v.log_enable then  
            change = true
            if not curr_state:is_original_item(v) then --不是原始项目需要对比
                check_dup[v] = true
                count = count + 1
            end
        end
    end

    if change == false and count == 0 then return false end --如果没有需要记录的项目直接返回

    local log_count = #curr_log - 1

    for ke,va in ipairs(curr_log) do
        if ke > 1 then --第一个是状态不需要判断
            if check_dup[va] == nil then --如果没有
                change = true
                break
            end
        end
    end

    if chage then
        return true 
    elseif log_count ~= count then
        return true
    else
        local log_state = curr_log[1]
        return log_state~=curr_state --如果不相同可以记录
    end
end

--返回值 {change bool,add table，remove table}
local function get_diff_log(curr_state,curr_log) 
    local change = false
    local items = curr_state:get_all_items() --当前的所有项目
    local add,remove = {},{}
    if curr_log == nil or curr_log == false then --如果没有记录
        change = true
        return change,add,remove
    end

    local check_dup = {}
    local log_state = curr_log[1]
    local check_state = curr_state

    if log_state ~= curr_state and log_state ~= nil then --如果不同状态
        check_state = log_state
        items = log_state:get_all_items()
        change = true
    end

    for k,v in ipairs(items) do
        if v.log_enable and not check_state:is_original_item(v) then --不是原始项目并且可以log
            if check_dup[v] == nil then 
                check_dup[v] = -1 -- -1 表示需要删除
            elseif check_dup[v] == 1 then --1 表示log表中存在
                check_dup[v] = 0 --不需要改变
            end
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

local function unload_unused_assets()
    -- collectgarbage("collect")
end
-------------------------------------------------------------------
StateManager =
{
    _current_game_state = nil,
    _auto_show_loading = true,
    _transform = nil,
    _log_state = LuaStack(512), --状态日志
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
    return self._new_state or self._current_game_state
end

function StateManager:get_last_state()
    return self._last_game_state
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
    if self._transform then self._transform:on_blur() end
end

--检测显示切换效果
function StateManager:check_show_transform( state)
    local need_load =  state ~= nil and state:is_all_loaded() == false

    if need_load and state:show_transform(self) then
        -- do nothing
    elseif self._auto_show_loading and need_load then
        self:show_transform()
        self:real_change_to_state()
    else
        self:real_change_to_state()
    end
end

--检测隐藏切换效果
function StateManager:check_hide_transform(curr_state)
    curr_state = curr_state or self._current_game_state
    if curr_state then curr_state:hide_transform() end
    if self._auto_show_loading then self:hide_transform() end
end

--调用设置的itemobject方法
function StateManager:call_all_item_method( )
    local curr_state = self._current_game_state

    self:input_enable()

    local previous_state = curr_state._on_state_showed_flag

    if previous_state and previous_state[1] then
         curr_state:on_filter_event(previous_state[2],"on_state_showed",previous_state[2]) 
         curr_state._on_state_showed_flag = nil
    end--当前状态所有item显示完毕

    local ty = type(curr_state.method)
    if ty == "string" then
        curr_state:on_event(curr_state.method,unpack(curr_state.args))
    elseif ty == "function" then
        curr_state.method(unpack(curr_state.args))
    end
    curr_state.method = nil
    curr_state.args = nil

    self:check_hide_transform(curr_state)
end

--注册状态改变事件
function StateManager:register_state_change(on_state_change,flag)
    if self._on_state_change == nil then self._on_state_change = {} end
    self._on_state_change[on_state_change] = flag
end

function StateManager:register_state_changing(on_state_changing,flag)
    if self._on_state_changing == nil then self._on_state_changing = {} end
    self._on_state_changing[on_state_changing] = flag
end

function StateManager:call_on_state_changing(curr_state,new_state) --call when sate changing
    local _on_state_changing = self._on_state_changing
    if _on_state_changing then
        for k,v in pairs(_on_state_changing) do
            k(curr_state,new_state) 
        end
    end
end

function StateManager:call_on_state_change(new_state) --call when sate change
    local _on_state_change = self._on_state_change
    if _on_state_change then
        for k,v in pairs(_on_state_change) do
            k(new_state) 
        end
    end
end

--标记回收
function StateManager:mark_dispose_flag(item_obj,item_gc_type)
    if self.mark_dispose_items == nil then self.mark_dispose_items = {} end
    self.mark_dispose_items[item_obj] = item_gc_type
end

--回收标记的item_object
function StateManager:auto_dispose_items(item_gc_type) -- dispose marked item_object 
    if self.mark_dispose_items ~= nil then 
       local mem_warning = HugulaProfiler.IsMemoryWarning
    --    print("mem_warning=",mem_warning)
        for k,v in pairs(self.mark_dispose_items) do
            if item_gc_type == nil or ( v >= item_gc_type or mem_warning) or v == 0 then
                if k.dispose and  self:is_in_current_state(k) == false then --self._current_game_state:contains_item(k) == false then
                    k:dispose() 
                end
                self.mark_dispose_items[k] = nil
            end
        end
    end
end


function StateManager:_state_on_blur()
    local new_state = self._new_state
    self._last_game_state = self._current_game_state

    if self._current_game_state ~= nil and new_state then
        self._current_game_state:on_blur(new_state)
        unload_unused_assets()
    end

end

function StateManager:_state_on_focus()
    local new_state = self._new_state
    local is_back = self.is_back
    self._new_state = nil
    if new_state then 
        local previous_state = self._last_game_state
        self._current_game_state = new_state
        new_state._on_state_showed_flag = {true,previous_state}
        new_state:on_focus(previous_state)
        if is_back then new_state:on_back(previous_state) end
        if new_state:is_all_loaded() then self:call_all_item_method() end

        if not is_back then self:record_state() end--记录状态用于返回   
        self:call_on_state_change(new_state) --state change event
        self.is_back = false
    end
end

-- --真正开始改变
function StateManager:real_change_to_state() --real change state
    self:_state_on_blur()
    self:call_on_state_changing(self._last_game_state,self._new_state)
    self:_state_on_focus()
end

function StateManager:is_in_current_state(item)
    local curr = self:get_current_state()
    return curr:contains_item(item)
end

--设置新的状态
function StateManager:set_current_state(new_state,method,...)
    assert(new_state ~= nil) 
    if new_state == self._current_game_state or self._new_state  then
        print("setCurrent State: "..tostring(new_state).." is fail ")
        return
    end

    local go_transform = nil
    new_state:check_initialize()

    if method == true then
        go_transform = true
    else
        new_state.method = method
        new_state.args={...}
    end

    local previous_state = self._current_game_state
    self._last_game_state = previous_state --self._current_game_state

    if previous_state then previous_state:on_bluring(new_state) end
    new_state:on_focusing(previous_state)

    self:input_disable() --lock input
    self._new_state = new_state

    self:check_show_transform(new_state,go_transform)

    
end

--删除日志记录
function StateManager:record_pop(i)  --从顶部删除    到第 i 个
    if type(i) == "number" then
        self._log_state:pop(i)
    end
end

--记录状态用于返回
function StateManager:record_state() 
    local curr_state = self._current_game_state --当前状态
    if curr_state.log_enable == false then 
        local top = self._log_state:get(-1) --顶部
        if top ~= false and top ~= nil then
            self._log_state:push(false) --空位置
        end
        return false 
    end

    local curr_log =  self._log_state:get(-1) --栈顶
    while curr_log == false do --如果栈顶为false
        self._log_state:pop(1) --删除栈顶
        curr_log = self._log_state:get(-1) --返回新的栈顶
    end

    local need_record = check_need_log(curr_state,curr_log) --是否需要记录

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

    return need_record
end

-- 注 -1表示栈顶 所以默认为-2
function StateManager:go_back(index) --返回
    if index == nil then index = 2 end --默认-2
    index = -math.abs(index) --取绝对值
    if index >= -1 then return false end --为当前状态
    local back_log = self._log_state:get(index) --返回的状态 --self._log_state:get(index) --上一个状态
    while back_log == false do
        self._log_state:pop(1) --删除顶上第一个
        back_log = self._log_state:get(index) --返回的状态
    end
    local pop_len = math.abs(index)-1
    if back_log == nil then return false end 
    local new_state = back_log[1] --pos 1 is state
    assert(new_state ~= nil,"返回的状态不能为空！")
    --添加状态
    local change,add,remove = get_diff_log(self._current_game_state,back_log)

    if change then
        self._log_state:pop(pop_len) --多余状态
    else
        return false
    end

    if new_state == self._current_game_state then --如果是当前状态
        local curr = self._current_game_state
        local last = self._last_game_state

        for k,v in ipairs(remove) do 
            curr:remove_item(v)
            v:on_blur(curr)
        end

        for k,v in ipairs(add) do 
            curr:add_item(v)
            v:on_focus(last)
            if v.on_focused then v:on_focused(last) end
            if v.onback then v:on_back() end
        end
           
        self:call_on_state_change(new_state)

    else

        local previous_state = self._current_game_state
        if previous_state then previous_state:on_bluring(new_state) end
        new_state:on_focusing(previous_state)

        self:input_disable() --lock
        for k,v in ipairs(add) do 
            new_state:add_item(v)
        end

        for k,v in ipairs(remove) do 
            new_state:remove_item(v)
            v:on_blur(self._current_game_state)
        end

        self._new_state = new_state

        self.is_back = true
        self:check_show_transform(new_state,true)

    end
    return true 
end