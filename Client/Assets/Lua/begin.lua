------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
require("const.importClass")
require("net.netMsgHelper")
require("net.netAPIList")
require("net.netProtocalPaser")
require("net.proxy")
require("const.requires")
require("registerItemObjects")
require("registerState")
require("uiInput")

local os=os
local UPDATECOMPONENTS=UPDATECOMPONENTS
local LuaObject=LuaObject
local StateManager=StateManager
local Net=Net
local Msg=Msg
local LuaHelper = LuaHelper
local delay = delay
local pLua = PLua.instance
-------------------------------------------------------------------------------

local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList

StateManager:setCurrentState(StateManager.welcome)

-- require("netGame")

local function update()
	local cmp
	local len
	len = #UPDATECOMPONENTS
	local ostime=os.clock()
	for i=1,len do
		cmp=UPDATECOMPONENTS[i]
		if cmp.enable then	cmp:onUpdate(ostime) end
	end
end

pLua.updateFn=update


--load config
require("game.common.loadCSV")

delay(function( ... )
	print(getValue("level_name_001")) --language key
	printTable(Model.getUnit(200001)) --read config
	-- Loader:clearSharedAB() 
end,0.5)