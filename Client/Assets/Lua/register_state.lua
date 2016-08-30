------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local StateManager = StateManager
local LuaItemManager = LuaItemManager
local StateBase = StateBase
StateManager:set_state_transform(LuaItemManager:get_item_obejct("transform"))

-- StateManager.hall = StateBase({"hall")})
StateManager.welcome = StateBase({"welcome"})
StateManager.tetris=StateBase({"tetris"})
StateManager.scroll_rect_table=StateBase({"scroll_rect_table"})
StateManager.load_scene = StateBase({"load_scene"})
StateManager.load_extends = StateBase({"load_extends"})