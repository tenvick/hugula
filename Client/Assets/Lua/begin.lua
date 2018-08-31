------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------

require("core.loader")
require("core.unity3d")
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
local Application= UnityEngine.Application
local RuntimePlatform= UnityEngine.RuntimePlatform
local PlayerPrefs = UnityEngine.PlayerPrefs
local Net=Net
local Msg=Msg
local LuaHelper = LuaHelper
local delay = delay
local pLua = PLua.instance
local CrcCheck = Hugula.Update.CrcCheck
local Common = Hugula.Utils.Common
local UriGroup = Hugula.Loader.UriGroup
local CUtils= Hugula.Utils.CUtils
-------------------------------------------------------------------------------

local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList

-- require("netGame")

local function on_state_change(state) --资源回收
	if state == StateManager.welcome then --当切换到welcome状态时候
		StateManager:auto_dispose_items() --回收标记的item_object
		unload_unused_assets()
	end
end

local function on_state_changing(state)
    -- gc
end

local function update()
    local cmp
    local len
    len = #UPDATECOMPONENTS
    local ostime=os.clock()
    for i=1,len do
        cmp=UPDATECOMPONENTS[i]
        if cmp.enable then    cmp:on_update(ostime) end
    end
end

pLua.updateFn=update

StateManager:input_disable() --锁定输入
StateManager:set_current_state(StateManager.welcome)
StateManager:register_state_change(on_state_change,true)
StateManager:register_state_changing(on_state_changing,true)

--load config
-- require("common.load_csv")

delay(function( ... )
	-- print(lua_localization("level_name_001")) --language key
	-- print_table(Model.getUnit(200001)) --read config
	-- Loader:clearSharedAB() 
    print("hot res... 9999")
end,0.5)