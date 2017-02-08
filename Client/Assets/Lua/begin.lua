------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
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

local function add_black_crc( ... )
	local black = CRC_FILELIST.black
	if black then
		for k1,v1 in pairs(black) do
			CrcCheck.Add(k1,v1[1])
		end
	end
end
local function check_mode()
	if Common.IS_WEB_MODE then
		local uris = UriGroup()
		uris:Add(CUtils.GetRealPersistentDataPath(),true);
		for k,v in pairs(cdn_hosts) do
			uris:Add(v,false,true,true)
		end
		Loader:set_uris(uris)
	elseif cdn_hosts then
		-- local uris = Hugula.Loader.LResLoader.uriList
		-- for k,v in pairs(cdn_hosts) do
		-- 	uris:Add(v,false,true,true)
		-- end
	end

	if Application.platform == RuntimePlatform.WindowsPlayer then --如果是windows平台删除缓存记录
		Hugula.Utils.FileHelper.DeletePersistentDirectory(nil)
	end
end

local function on_state_change(state) --资源回收
	if state == StateManager.welcome then --当切换到welcome状态时候
		StateManager:auto_dispose_items() --回收标记的item_object
		unload_unused_assets()
	end
end

add_black_crc()
check_mode()

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

--load config
require("common.load_csv")

delay(function( ... )
	-- print(get_value("level_name_001")) --language key
	-- print_table(Model.getUnit(200001)) --read config
	-- Loader:clearSharedAB() 
end,0.5)