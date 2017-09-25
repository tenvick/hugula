---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: load_extends.lua
--data:2016.8.30
--author:pu
--desc:加载扩展包
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local load_extends = LuaItemManager:get_item_object("load_extends")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
load_extends.assets=
{
    View("ex_ui_bottom_view",load_extends),
    View("ex_cube_view",load_extends)
}

------------------private-----------------


------------------public------------------

--点击事件
function load_extends:on_click(obj,arg)
	local cmd =obj.name
	if "Button" == cmd then
		StateManager:go_back()
	end
end

function load_extends:on_hide( ... )
	self:raise_property_changed(self.on_hide)
end
