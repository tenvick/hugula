---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: load_extends.lua
--data:2016.8.30
--author:pu
--desc:加载扩展包
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local load_extends = LuaItemManager:get_item_obejct("load_extends")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
load_extends.assets=
{
    Asset("extends/ex_ui_bottom.u3d"),
    Asset("extends/ex_cube.u3d")
}

------------------private-----------------


------------------public------------------
-- 资源加载完成时候调用方法
function load_extends:on_assets_load(items)
	-- local refer = LuaHelper.GetComponent(self.assets[1].root,"ReferGameObjects") 
	-- local refer1 = LuaHelper.GetComponent(self.assets[2].items["yourItemName"],"ReferGameObjects")
end


--点击事件
function load_extends:on_click(obj,arg)
	local cmd =obj.name
	if "Button" == cmd then
		StateManager:go_back()
	end
end

--每次显示时候调用
function load_extends:on_showed( ... )

end

function load_extends:on_hide( ... )
	self.assets[1]:dispose() --销毁资源
	self.assets[2]:dispose()
end

--初始化函数只会调用一次
function load_extends:initialize()

end