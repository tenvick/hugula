------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
LuaItemManager={}
local LuaObject =LuaObject
local LuaItemManager = LuaItemManager
LuaItemManager.ItemObject=class(LuaObject,function(self,name) --implement luaobject
    LuaObject._ctor(self, name)
    self.asset_loader = self:add_component("asset_loader")
    self.priority = 0
    self.log_enable = true -- 是否启用日志记录用于返回
end)

local StateManager = StateManager
local ItemObject = LuaItemManager.ItemObject
LuaItemManager.items = {}

function LuaItemManager:get_item_object(objectName)
    local obj=self.items[objectName]
    if obj == nil then
         print(objectName .. " is not registered ")
        return nil
    end
   
    if obj._require == nil then 
      obj._require = true require(obj._lua_path) 
      if obj.initialize ~= nil then obj:initialize() end
    end
    return  obj
end

--小心调用这个只用于subview关联
function LuaItemManager:get_item_clone_object(objectName)
    local obj=self.items[objectName]
    if obj == nil then
         print(objectName .. " is not registered ")
        return nil
    end
   
    require(obj._lua_path) 
    if obj.initialize ~= nil then obj:initialize() end
    return  obj
end

function LuaItemManager:register_item_object(objectName, luaPath, instance_now,auto_mark_dispose)

    assert(self.items[objectName] == nil)
    local new_class   = ItemObject(objectName)
    new_class._key  = objectName
    new_class._lua_path = luaPath
    new_class._auto_mark_dispose = auto_mark_dispose
    self.items[objectName] = new_class

    if instance_now then 
        new_class._require = true  
        require(luaPath) 
        if new_class.initialize ~= nil then new_class:initialize() end
    end
end

function ItemObject:show( ... )
   local assets = self.assets
   if assets ~= nil then
    for k,v in ipairs(assets) do  v:show()   end
   end
end

function ItemObject:on_showed( ... )

end

function ItemObject:hide( ... )
  local assets = self.assets
  if assets then
    for k,v in ipairs(assets) do  v:hide()  end
  end
end

function ItemObject:dispose( ... )
    local assets = self.assets
    if self.on_dispose then
        self:on_dispose() 
    end
    if assets then
        for k,v in ipairs(assets) do  v:dispose()   end
    end
    self.is_call_assets_loaded = nil
    self.is_disposed = true --标记销毁
    -- self.property_changed = nil
end

function ItemObject:check_assets_loaded() --检测资源是否加载完成
    local assets = self.assets
    if assets and #assets >= 1 then
        for k,v in ipairs(assets) do
          if v and v:is_loaded() == false then return false end --如果为空没有加载完成
        end
      if  self.is_call_assets_loaded ~= true then return false end --如果还没有加载过
    end
    return true --加载完成
end

function ItemObject:on_focus(...)
    self.is_on_blur = false
    self.is_disposed = false
    if self.is_loading then return end
    if self:check_assets_loaded() then 
        self:show()  
        self:send_message("on_showed")
        self:call_event("on_showed")
    else
        self.asset_loader:load(self.assets)  
    end
end

-- function ItemObject:on_back() --返回时候自己实现调用

-- end

function ItemObject:mark_dispose_flag(flag) -- dispose when  StateManager:auto_dispose_items() called
    self._auto_mark_dispose = flag
end

function ItemObject:on_hide()
  
end

function ItemObject:auto_mark_dispose()
    if self._auto_mark_dispose then 
        StateManager:mark_dispose_flag(self,self._auto_mark_dispose) --标记销毁
    end
end

function ItemObject:on_blur( state )
    if self:check_assets_loaded() then
        self:send_message("on_hide",state) --开始隐藏
        self:auto_mark_dispose()
    end
    self.is_on_blur = true --处于失去焦点状态
    self:hide()
    -- print(self.name,"ItemObject:on_blur",self.is_on_blur)
    -- self:send_message("on_hided",state) --隐藏完成
end

--注册 属性改变事件
function ItemObject:register_property_changed(func,view)
    if self.property_changed == nil then self.property_changed = {} end
    self.property_changed[func] = view
end

--mvvm property change 当属性改变的时候需要调用
function ItemObject:raise_property_changed(property_name)
    if self.property_changed ~= nil then
        local changed_tb = self.property_changed
        for k,v in pairs(changed_tb) do
            k(v,self, property_name)
        end
    end
end

function ItemObject:set_property(propertyName,value) --设置属性
    if  self[propertyName] == value or not propertyName then return false end
        print(value)
        self[propertyName] = value;
        self:raise_property_changed(propertyName);
    return true;
end

function ItemObject:add_to_state(state)
    local current_state = StateManager:get_current_state()
    if state == nil or state == current_state then
        current_state:add_item(self)
        if self.on_focusing then self:on_focusing(current_state) end
        local previous_state = self._current_state
        self:on_focus(previous_state)
        if self.on_focused then self:on_focused(previous_state) end
        if self.log_enable then StateManager:record_state() end
    else
        state:add_item(self)
    end
    
    local added_state = state or current_state 
    if self._current_state and self._current_state ~= added_state then self._current_state:remove_item(self) end
    self._current_state = added_state --record state
    -- print("add to state",tostring(added_state),tostring(self))
end

function ItemObject:remove_from_current()
    self:remove_from_state(StateManager._current_game_state)
end

function ItemObject:remove_from_state(state)
    local current_state = StateManager:get_current_state()

    if state == nil or state == current_state then 

        local removed = current_state:remove_item(self)
        if not removed and self._current_state then removed = self._current_state:remove_item(self) end
        
        if removed then --如果从当前状态移除成功        
            if self.on_bluring then self:on_bluring(new_state) end
            self:on_blur(current_state) 
            if self.on_blured then self:on_blured(new_state) end
            if self.log_enable then StateManager:record_state() end
        end
    else
        state:remove_item(self)
        if self._current_state then self._current_state:remove_item(self) end
    end
    self._current_state = nil
end

function ItemObject:__tostring()
    return string.format("ItemObject(%s) ", self._key)
end
