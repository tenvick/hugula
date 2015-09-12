------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaItemManager = LuaItemManager
local Resources = UnityEngine.Resources
StateManager =
{
    m_currentGameState  =   nil,
    m_objectCount=0 ,
    m_autoShowLoading=true,
    m_objectLoaded=0,
    m_transform = nil
}

function StateManager:getCurrentState()
    return self.m_currentGameState
end

function StateManager:setStateTransform(transform)
    self.m_transform = transform
end

function StateManager:showTransform()
    -- print("show Transform")
    -- print(self.m_transform)
    if self.m_transform then self.m_transform:onFocus() end
end

--设置Loading慕的显示模式
function StateManager:setLoading(bl)
    -- body
    if bl then self.m_autoShowLoading = true
    else self.m_autoShowLoading = false end
end

function StateManager:hideTransform()
    if self.m_transform then self.m_transform:hide() end
end

function StateManager:checkShowTransform( ... )
    if self.m_autoShowLoading then self:showTransform() end
end

function StateManager:checkHideTransform( ... )
    if self.m_autoShowLoading then self:hideTransform() end
end

function StateManager:setCurrentState(newState,callFocues)
    assert(newState ~= nil)
    if(newState == self.m_currentGameState)   then
        print("setCurrent State: "..tostring(newState).." is same as currentState"..tostring(self.m_currentGameState))
        return
    end

    if(self.m_currentGameState ~= nil) then
        self.m_currentGameState:onBlur(newState)
    end

    local previousState = self.m_currentGameState
    self.m_currentGameState = newState
    --self.m_objectCount=newState:getItemCount()
    --self.m_objectLoaded=0    
    if callFocues==nil then callFocues=true end
    --if self.m_autoShowLoading and callFocues then self:showTransform() end
    if callFocues then newState:onFocus(previousState) end

    unloadUnusedAssets()
end
