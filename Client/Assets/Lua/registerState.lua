------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local StateManager = StateManager
local LuaItemManager = LuaItemManager
local StateBase = StateBase
StateManager:setStateTransform(LuaItemManager:getItemObject("transform"))

-- StateManager.hall = StateBase({LuaItemManager:getItemObject("hall")})
StateManager.welcome = StateBase({LuaItemManager:getItemObject("welcome")})
StateManager.tetris=StateBase({LuaItemManager:getItemObject("tetris")},"tetris")
