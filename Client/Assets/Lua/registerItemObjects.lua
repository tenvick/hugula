------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaItemManager = LuaItemManager
local StateManager = StateManager
--global
LuaItemManager:registerItemObject("transform","state/stateTransform",true)
LuaItemManager:registerItemObject("alertTips","game/alertTips",true)


--itemobject
-- LuaItemManager:registerItemObject("hall","game/hall",false)
LuaItemManager:registerItemObject("welcome","game/welcome",false)
LuaItemManager:registerItemObject("tetris","game/tetris",false)