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
function LuaItemManager:get_item_obejct(objectName)
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

function LuaItemManager:register_item_object(objectName, luaPath, instance_now)

    assert(self.items[objectName] == nil)
    local new_class   = ItemObject(objectName)
    new_class._key  = objectName
    new_class._lua_path = luaPath
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

function ItemObject:clear( ... )
   local assets = self.assets
   assert(assets ~= nil)
   for k,v in ipairs(assets) do
        v:dispose()
    end
    self.is_call_assets_loaded = nil
end

function ItemObject:check_assets_loaded() --检测资源是否加载完成
    local assets = self.assets
    if assets and #assets >= 1 then
        for k,v in ipairs(assets) do
          if v and v.root == nil then return false end --如果为空没有加载完成
        end
      if  self.is_call_assets_loaded ~= true then return false end --如果还没有加载过
    end
    return true --加载完成
end

function ItemObject:on_focus(...)
    if self:check_assets_loaded() then 
        self:show()  
        self:send_message("on_showed")
        self:call_event("on_showed")
    else
        StateManager:check_show_transform()
        self.asset_loader:load(self.assets)  
    end
end

-- function ItemObject:on_back() --返回时候自己实现调用

-- end

function ItemObject:on_hide()
  
end

function ItemObject:on_blur( state )
    self:send_message("on_hide",state) --开始隐藏
    self:hide()
    -- self:send_message("on_hided",state) --隐藏完成
end

function ItemObject:add_to_state(state)
  if state == nil then
    StateManager:get_current_state():add_item(self)
    self:on_focus(StateManager:get_current_state())
    if self.log_enable then StateManager:record_state() end
  else
    state:add_item(self)
  end
end

function ItemObject:remove_from_state(state)
  if state == nil then
    local current_state = StateManager:get_current_state()
    local removed = current_state:remove_item(self)
    if removed then --如果从当前状态移除成功
        self:on_blur(current_state)
        if self.log_enable then StateManager:record_state() end
    end
  else
     state:remove_item(self)
  end
end

function ItemObject:__tostring()
    return string.format("ItemObject(%s) ", self.name)
end
