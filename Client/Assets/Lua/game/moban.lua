---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: moban.lua
--data:2014.xx.xx
--author:xxx
--desc:xx功能
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local youname = LuaItemManager:getItemObject("youname")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local getValue = getValue --多国语言

--UI资源
youname.assets=
{
    -- Asset("youresource.u3d"),
    -- Asset("youresource1.u3d",{"yourItemName"})
}

------------------private-----------------


------------------public------------------
-- 资源加载完成时候调用方法
function youname:onAssetsLoad(items)
	-- local ReferScript = LuaHelper.GetComponent(self.assets[1].root,"ReferGameObjects") 
	-- local ReferScript1 = LuaHelper.GetComponent(self.assets[2].items["yourItemName"],"ReferGameObjects")
end


--点击事件
function youname:onClick(obj,arg)
	local cmd =obj.name
	
end

--每次显示时候调用
function youname:onShowed( ... )

end

--初始化函数只会调用一次
function youname:initialize()

end