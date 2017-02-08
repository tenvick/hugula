------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local LuaItemManager = LuaItemManager
local StateManager = StateManager
--global
LuaItemManager:register_item_object("transform","state/state_transform",true)
LuaItemManager:register_item_object("alert_tips","viewmodels/alert_tips",true)


--itemobject
LuaItemManager:register_item_object("welcome","viewmodels/welcome",false)
LuaItemManager:register_item_object("tetris","viewmodels/tetris",false,true)-- 标记释放
LuaItemManager:register_item_object("scroll_rect_table","viewmodels/scroll_rect_table",false,true)-- 标记释放
LuaItemManager:register_item_object("load_scene","viewmodels/load_scene",false,true)-- 标记释放
LuaItemManager:register_item_object("load_extends","viewmodels/load_extends",false,true)-- 标记释放
