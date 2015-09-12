local os=os
-- local pLua=PLua.instance
local UPDATECOMPONENTS=UPDATECOMPONENTS
local LuaObject=LuaObject
local StateManager=StateManager
local Net=Net
local Msg=Msg

-------------------------------------------------------------------------------

local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList


Net.onConnectionFn=function(net)
	print("game onConnection  ")
end

Net.onIntervalFn=function(net)
	luaGC()
end

Net.onAppPauseFn=function(bl)
--	print("onApplicationPause ="..tostring(bl).." isConnected="..tostring(Net.isConnected))
--	print("pingMsg onAppPause  "..CUtils.getDateTime())
	if bl==false then
		if Net.isConnected==false then Net:ReConnect() end
	end
end

Net.onReConnectFn=function(net)
	print("onReConnectFn")
	--delay(showNetworkInfo,2,"waiting reconnection")
end

Net.onMessageReceiveFn=function(m)
	local ty=m:get_Type()
	-- print(m:Debug())
	-- print(ty)
	local dataStruct=NetAPIList:getDataStructFromMsgType("MSGTYPE"..tostring(ty))
	print(" onMessageReceive  type="..tostring(ty).." Length="..tostring(m:ToArray().Length).." dataStruct ="..dataStruct)
	local model=NetProtocalPaser:parseMessage(m,ty)
	Proxy:distribute(ty,model)
end

Net.onConnectionCloseFn=function(net)
	print("onConnectionClose")
	--showTips("你的网络已断开！点击确定重新连接。",onReConnect)
	-- Net:ReConnect()
	local hall = LuaItemManager:getItemObject("hall")
	hall:toHall()
end

Net.onConnectionTimeoutFn=function(net)

	print("Connection time out")
	--showTips("网络连接超时,点击确定重新连接。",onReConnect)
	--showNetworkInfo("connection time out")
	-- Net:ReConnect()
end

Net.onReConnectFn=function()
	-- body
	print("onReConnect")
	--Net:ReConnect()
end

