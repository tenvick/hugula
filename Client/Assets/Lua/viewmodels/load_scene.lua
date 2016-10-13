---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: load_scene.lua
--data:2016.08.29
--author:pu
--desc:动态加载一个新的场景
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local load_scene = LuaItemManager:get_item_obejct("load_scene")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
load_scene.assets=
{
    View("loadscene_view",load_scene) 
}

------------------private-----------------


------------------public------------------

--点击事件
function load_scene:on_click(obj,arg)
	local cmd =obj.name
	print(cmd)
	if cmd == "Btn_back" then
		StateManager:go_back()
	end
end

--每次显示时候调用
function load_scene:on_showed( ... )

end

--初始化函数只会调用一次
function load_scene:initialize()

end