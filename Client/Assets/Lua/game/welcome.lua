---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
--data:2015.4.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local welcome = LuaItemManager:getItemObject("welcome")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local getValue = getValue --多国语言

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
function welcome:onClick(obj,arg)
	local cmd =obj.name
    print("you are click"..cmd)
    if cmd == "BtnStart" then
    	StateManager:setCurrentState(StateManager.tetris)
    end
end


--显示时候调用
function welcome:onShowed( ... )

end

--初始化函数只会调用一次
function welcome:initialize()

end