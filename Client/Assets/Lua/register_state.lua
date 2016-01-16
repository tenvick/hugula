------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local StateManager = StateManager
local LuaItemManager = LuaItemManager
local StateBase = StateBase
StateManager:set_state_transform(LuaItemManager:get_item_obejct("transform"))

-- StateManager.hall = StateBase({LuaItemManager:get_item_obejct("hall")})
StateManager.welcome = StateBase({LuaItemManager:get_item_obejct("welcome")})
StateManager.tetris=StateBase({LuaItemManager:get_item_obejct("tetris")},"tetris")
