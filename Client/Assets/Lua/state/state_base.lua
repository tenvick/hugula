------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------

local function sortFn(a,b) 
    return tonumber(a.priority) > tonumber(b.priority) 
end

StateBase=class(function(self,item_objects,log_enable)
        
        self._item_list={} --所有项
        self.original = {} --原始item

        if item_objects then 
            for k,v in ipairs(item_objects) do
                self.original[v] = false
            end
        end

        self.will_sort = false
        self.method = nil
        if log_enable == nil then  -- 是否启用日志记录用于返回
            self.log_enable = true 
        else
            self.log_enable = log_enable 
        end
    end)

local StateBase = StateBase

function StateBase:check_initialize( ... ) --初始化
    if self.initialize ~= true then
        local original = self.original
        local item_obj = nil
        for k,v in pairs(original) do
            if v == false then
                item_obj = LuaItemManager:get_item_obejct(k)
                if item_obj then 
                    original[k] = item_obj 
                    table.insert(self._item_list,item_obj)
                end
            end
        end
        table.sort(self._item_list,sortFn)
        self.initialize = true
    end
end

function StateBase:is_original_item(item) --是否是原始项目
    local original = self.original
    local key = ""
     if type(key) == "table" then
        key = item._key or ""
    elseif type(key) == "string" then
        key = item
    end
    local k = original[key]
    return k
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
end

function StateBase:is_all_loaded()
    local item_list = self._item_list
    for k,v in ipairs(item_list) do
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
            print(tostring(obj).." is exist in current state "..self.name)
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

function StateBase:on_focus(previous_state)
    self:check_initialize()
    local item_list = self._item_list
    local _len = #item_list
    for k,v in ipairs(item_list) do
        if k <= _len then --确保新加入的不会被执行
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

function StateBase:on_blur(new_state)
    local itemobj = nil
    local on_blured = {}
    for i=#self._item_list,1,-1 do
        itemobj=self._item_list[i]
        if itemobj and on_blured[itemobj] ~= true then 
            on_blured[itemobj] = true 
            itemobj:on_blur(new_state) 
            if itemobj.on_blured then itemobj:on_blured(new_state) end
        end
    end
    on_blured = nil
 end

function StateBase:clear( ... )
    for k,v in ipairs(self._item_list) do
        v:clear()
    end
end

function StateBase:on_event(funName,...)
    local fn,v = nil,nil
    local item = self._item_list
    local len = #item
    for k,v in ipairs(self._item_list) do
        if k <= len then --确保新加入的item不会被执行
            fn = v[funName]
            if v.active and fn then 
                if fn(v,...) then break end
            end
        end
    end

    self:check_sort()
end

function StateBase:__tostring()
    local str = ""
    if self._item_list then 
        for k,v in ipairs(self._item_list)  do
            str = str .. tostring(v)
        end
    end
    return string.format("StateBase(%s) {%s} ", tostring(self._item_list),tostring(str))
end
