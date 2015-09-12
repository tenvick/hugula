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
    self.assetLoader = self:addComponent("assetLoader")
    self.assetsLoaded=false
    self.priority=0
end)

local StateManager = StateManager
local ItemObject = LuaItemManager.ItemObject
LuaItemManager.items = {}
function LuaItemManager:getItemObject(objectName)
    local obj=self.items[objectName]
    if(obj == nil) then
         print(objectName .. " is not registered ")
        return nil
    end

    if obj.m_require==nil then 
      obj.m_require=true require(obj.m_luaPath) 
      if obj.initialize~=nil then obj:initialize() end
    end
    return  obj
end

function LuaItemManager:registerItemObject(objectName, luaPath, instanceNow)

    assert(self.items[objectName] == nil)

    local newClass   = ItemObject(objectName)

    newClass.m_objectName  = objectName
    newClass.m_luaPath = luaPath
    self.items[objectName] = newClass
    if instanceNow then 
        newClass.m_require=true  
        require(luaPath) 
        if newClass.initialize~=nil then newClass:initialize() end
    end
end

function ItemObject:show( ... )
   local assets = self.assets
   assert(assets~=nil)
   for k,v in ipairs(assets) do
        v:show()
    end
end

function ItemObject:onShowed( ... )

end

function ItemObject:hide( ... )
   local assets = self.assets
   assert(assets~=nil)
   for k,v in ipairs(assets) do
        v:hide()
    end
end

function ItemObject:clear( ... )
   local assets = self.assets
   self.assetsLoaded =false
   assert(assets~=nil)
   for k,v in ipairs(assets) do
        v:clear()
    end
end

function ItemObject:onFocus( ... )
    if self.assetsLoaded then 
        self:show()  
        self:onShowed()     
    else
        if self.assets and #self.assets>=1 then --如果没有资源
            StateManager:checkShowTransform()
            self.assetLoader:load(self.assets)  
        else
            self.assetsLoaded = true
            self:show()  
            self:onShowed() 
        end
    end
    -- self:setActive(true)
end

function ItemObject:onHide()
end

function ItemObject:onBlur( ... )
    self:hide()
    self:onHide()
    -- self:setActive(false)
end

function ItemObject:addToState(state)
  if state == nil then
    StateManager:getCurrentState():addItem(self)
    self:onFocus(StateManager:getCurrentState())
  else
    state:addItem(self)
  end
end

function ItemObject:removeFromState(state)
  if state == nil then
     StateManager:getCurrentState():removeItem(self)
     self:onBlur(StateManager:getCurrentState())
  else
     state:removeItem(self)
  end
end

function ItemObject:__tostring()
    return string.format("ItemObject.name = %s ", self.name)
end
