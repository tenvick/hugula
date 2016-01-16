---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
--data:2015.4.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local welcome = LuaItemManager:get_item_obejct("welcome")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
welcome.assets=
{
     Asset("welcome.u3d")
}

------------------private-----------------


------------------public------------------
-- 资源加载完成时候调用方法
function welcome:onAssetsLoad(items)
	local fristView = LuaHelper.Find("Frist")
	if fristView then LuaHelper.Destroy(fristView) end
	-- Loader:clearSharedAB()
end


--点击事件
function welcome:on_click(obj,arg)
	local cmd =obj.name
    print("welcome  click"..cmd)
    if cmd == "BtnStart" then
    	StateManager:set_current_state(StateManager.tetris)
    end
end


--显示时候调用
function welcome:onShowed( ... )

end

--初始化函数只会调用一次
function welcome:initialize()

end