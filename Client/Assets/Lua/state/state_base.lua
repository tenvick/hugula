------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local StateManager = StateManager
local function sortFn(a,b) 
    return tonumber(a.priority) > tonumber(b.priority) 
end

StateBase=class(function(self,item_objects,arg1,arg2)
        
        self._item_list={} --所有项
        self._original = {} --原始item
        self._on_blured ={} --标记移除
        self.name = ""

        if item_objects and #item_objects > 0 then 
            self.name = item_objects[1]
            for k,v in ipairs(item_objects) do
                self._original[v] = true
            end
        end

        self.will_sort = false
        self.method = nil

        if type(arg1) == "string" then
            self._transform_original = arg1
        end

        local log_enable = true
        if type(arg1) == "boolean" then
            log_enable = arg1
        elseif  type(arg2) == "boolean" then
            log_enable = arg2
        end

        self.log_enable = log_enable

    end)

local StateBase = StateBase

function StateBase:check_initialize( ... ) --初始化
    if self.initialize ~= true then
        local original = self._original
        local item_obj = nil
        for k,v in pairs(original) do
            if v == true then
                item_obj = LuaItemManager:get_item_object(k)
                if item_obj then 
                    original[k] = item_obj 
                    table.insert(self._item_list,item_obj)
                end
            end
        end
        table.sort(self._item_list,sortFn)

        if self._transform_original then
            self._transform = LuaItemManager:get_item_object(self._transform_original)
        end
        self.initialize = true
    end
end

--显示切换UI
function StateBase:show_transform(state_manager)
    local transform = self._transform
    if transform then
        transform._state_manager = state_manager
        transform:on_focus()
        return true
    else
        return  false
    end
end

--隐藏切换效果
function StateBase:hide_transform()
    if self._transform then
        self._transform:on_blur()
        return true
    else
        return  false
    end

end

function StateBase:is_original_item(item) --是否是原始项目
    local original = self._original
    local key = item
     if type(key) == "table" then
        key = item._key or ""
    elseif type(key) == "string" then
        key = item
    end
    local k = original[key]
    return k ~= nil
end

function StateBase:contains_item(key) --当前状态是否包含item
    if type(key) == "table" then
        for k,v in ipairs(self._item_list) do
            if v == key then return true end
        end
    elseif type(key) == "string" then
        for k,v in ipairs(self._item_list) do
            if v and v._key == key then return true end
        end
    end
    return false
end

function StateBase:is_all_loaded()
    local item_list = self._original
    for k,v in pairs(item_list) do     
        if v == true then return false end  -- true has not initialize 
        if v:check_assets_loaded() == false then return false end
    end
    return true
end

function StateBase:get_all_items() --获取当前状态所有项
    return self._item_list
end

function StateBase:add_item(obj)
    for i, v in ipairs(self._item_list) do
        if v == obj  then
            print(tostring(obj).." is exist in current state ")
           return
        end
    end
    table.insert(self._item_list, obj)
    self.will_sort = true
end

function StateBase:check_sort()
    if self.will_sort then
        table.sort(self._item_list,sortFn)
        self.will_sort = false
    end
end

function StateBase:remove_item(obj)
    for i, v in ipairs(self._item_list) do
        if v == obj and not self:is_original_item(obj)  then --原始项目不能移除
            table.remove(self._item_list,i)
            if(obj.onremove_from_state~=nil) then
                obj:onremove_from_state(self)
            end
            return true
        end
    end
    return false
end

function StateBase:on_focusing(previous_state) --获取焦点之前
     local item_list = self._item_list
    local _len = #item_list --print(_len)
    for k,v in ipairs(self._item_list) do
        if k <= _len and (previous_state == nil or not previous_state:is_original_item(v)) or v.forcecall then --确保新加入的不会被执行,前一个状态的item不需要 focus
            if v.on_focusing then v:on_focusing(previous_state) end
        end
    end
end

function StateBase:on_focus(previous_state)
    -- self:check_initialize()
    local item_list = self._item_list
    local _len = #item_list --print(_len)
    for k,v in ipairs(self._item_list) do
        if k <= _len and (previous_state == nil or not previous_state:is_original_item(v)) or v.forcecall then --确保新加入的不会被执行,前一个状态的item不需要 focus
            v:on_focus(previous_state)
            if v.on_focused then v:on_focused(previous_state) end
        end
    end
    self:check_sort()
end

function StateBase:on_back(new_state)
    local itemobj = nil
    for i=#self._item_list,1,-1 do
        itemobj=self._item_list[i]
        if itemobj and itemobj.on_back then
            itemobj:on_back()
        end
    end

 end

function StateBase:on_bluring(new_state) --失去焦点之前
    local itemobj = nil
    local on_blured = self._on_blured
    for i=#self._item_list,1,-1 do
        itemobj=self._item_list[i]
        if itemobj and on_blured[itemobj] ~= true and (new_state == nil or not new_state:is_original_item(itemobj))  or itemobj.forcecall then --如果新状态包涵当前item不需要失去焦点
            on_blured[itemobj] = true 
            if itemobj.on_bluring then itemobj:on_bluring(new_state) end
        end
    end
    table.clear(on_blured)
 end

function StateBase:on_blur(new_state)
    local itemobj = nil
    local on_blured = self._on_blured
    local item_list = self._item_list

    for i=#item_list,1,-1 do
        itemobj=item_list[i]
        if itemobj and on_blured[itemobj] ~= true and (new_state == nil or not new_state:is_original_item(itemobj))  or itemobj.forcecall then --如果新状态包涵当前item不需要失去焦点
            on_blured[itemobj] = true 
            itemobj:on_blur(new_state) 
            if not itemobj.log_enable and not self:is_original_item(itemobj) then table.remove(self._item_list,i)  end
            if itemobj.on_blured then itemobj:on_blured(new_state) end
        end
    end
    -- self:hide_transform()
    table.clear(on_blured)
 end

function StateBase:dispose( ... )
    for k,v in ipairs(self._item_list) do
        v:dispose()
    end
end

function StateBase:on_event(fun_name,...)
    self:on_filter_event(nil,fun_name,...)
end

function StateBase:on_filter_event(prev_state,fun_name,...)
    local fn,v = nil,nil
    local item = self._item_list
    local len = #item 
    for k,v in ipairs(item) do
        if k <= len and (prev_state == nil or not prev_state:is_original_item(v)) and StateManager:is_in_current_state(v)  or v.forcecall then --确保新加入的item不会被执行
            fn = v[fun_name]
            if v.active and fn then 
                if fn(v,...) then break end
            end
        end
    end
end

function StateBase:__tostring()
    local str,len = "",0
    if self._item_list then 
        for k,v in ipairs(self._item_list)  do
            str = str .. tostring(v)
        end
        len = #self._item_list
    end
    return string.format("StateBase(%s)len(%d) {%s} ", tostring(self._item_list),len,str)
end
