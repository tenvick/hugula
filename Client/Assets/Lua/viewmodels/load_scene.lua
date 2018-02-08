---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: load_scene.lua
--data:2016.08.29
--author:pu
--desc:动态加载一个新的场景
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local load_scene = LuaItemManager:get_item_object("load_scene")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

local PrefabPool = Hugula.Pool.PrefabPool
local BackGroundDownload = Hugula.Update.BackGroundDownload
local UGUIEvent = Hugula.UGUIExtend.UGUIEvent
local ResourcesLoader = Hugula.Loader.ResourcesLoader
local PLua = Hugula.PLua--UI资源

load_scene.assets=
{
    View("loadscene_view",load_scene) 
}

------------------private-----------------
local function re_start_game()
	local svp = LuaHelper.Find("LuaSvrProxy")
    LuaHelper.Destroy(svp)
    LuaHelper.Destroy(PrefabPool.instance)
    UGUIEvent.RemoveAllEvents()
    LuaHelper.Destroy(ResourcesLoader.instance.gameObject)
	BackGroundDownload.Dispose()
	--重启之前清理资源
	PLua.ReStart(0.5)
end

------------------public------------------

--点击事件
function load_scene:on_click(obj,arg)
	local cmd =obj.name
	print(cmd)
	if cmd == "Btn_back" then
		StateManager:go_back()
	elseif cmd == "Btn_restart" then
		re_start_game()
	end
end

--每次显示时候调用
function load_scene:on_showed( ... )

end

--初始化函数只会调用一次
function load_scene:initialize()

end