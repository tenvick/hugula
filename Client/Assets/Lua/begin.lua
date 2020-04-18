------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
require("common.requires")
local VMState = VMState
local VMGroup = VMGroup

-- -------------------------------------------------------------------------------
local GameObject = CS.UnityEngine.GameObject
VMState:push(VMGroup.welcome)


-- local function on_state_change(state) --资源回收
-- 	if state == StateManager.welcome then --当切换到welcome状态时候
-- 		StateManager:auto_dispose_items() --回收标记的item_object
-- 		unload_unused_assets()
-- 	end
-- end
