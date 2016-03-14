------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
require("core.structure")
require("net.netMsgHelper")
require("net.netAPIList")
require("net.netProtocalPaser")
require("net.proxy")
require("const.requires")
require("register_item_objects")
require("register_state")
require("ui_input")

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

StateManager:set_current_state(StateManager.welcome)

-- require("netGame")

local function update()
	local cmp
	local len
	len = #UPDATECOMPONENTS
	local ostime=os.clock()
	for i=1,len do
		cmp=UPDATECOMPONENTS[i]
		if cmp.enable then	cmp:on_update(ostime) end
	end
end

pLua.updateFn=update


--load config
require("game.common.load_csv")

delay(function( ... )
	-- print(get_value("level_name_001")) --language key
	-- print_table(Model.getUnit(200001)) --read config
	-- Loader:clearSharedAB() 
end,0.5)