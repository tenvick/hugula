------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local ViewBase = ViewBase
-- local LuaHelper=LuaHelper


local view= ViewBase()
view.asset_name = "welcome"
view.res_path = "welcome.u3d"

function view:on_asset_load(key,asset)
	Logger.Log(key,asset) 
end

return view