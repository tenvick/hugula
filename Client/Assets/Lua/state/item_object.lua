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
    self.assets_loaded = false
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
    new_class._object_name  = objectName
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
   self.assets_loaded = false
   assert(assets ~= nil)
   for k,v in ipairs(assets) do
        v:dispose()
    end
end

function ItemObject:on_focus( ... )
    if self.assets_loaded then 
        self:show()  
        self:send_message("on_showed")
        self:call_event("on_showed")
    else
        if self.assets and #self.assets >= 1 then --如果没有资源
            StateManager:check_show_transform()
            self.asset_loader:load(self.assets)  
        else
            self.assets_loaded = true
            self:show()  
            self:send_message("on_showed")
            self:call_event("on_showed")
        end
    end
end

-- function ItemObject:on_back() --返回时候自己实现调用

-- end

function ItemObject:on_hide()
end

function ItemObject:on_blur( ... )
    self:send_message("on_hide")
    self:hide()
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
