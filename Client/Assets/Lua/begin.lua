------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
require("common.requires")
local VMState = VMState
local VMGroup = VMGroup

-- -------------------------------------------------------------------------------
local GameObject = CS.UnityEngine.GameObject
-- local BindContainer = GameObject.Find("/Canvas"):GetComponent("Hugula.Databinding.BindableContainer")
-- local ButtonBinder = BindContainer.children[1]
-- local data = NotifyObject()

-- data.text = " this is lua text"
-- data.onclick = {
--     can_execute = function(self, arg)
--         return true
--     end,
--     execute = function(self, arg)
--         Logger.Log("dropped_cmd", table.tojson(self), "arg=", arg, ".")
--     end
-- }

-- data.cmd_parameter = "data aaaaa  "
-- local _value = 0.4
-- data.slider_value = function(value)
--     Logger.Log("slider_value=", value)
--     return _value
-- end
-- data.input_text = function(arg)
--     Logger.Log(arg)
--     return "input text" .. (arg or "")
-- end

-- Logger.Log(data, data["onclick"])
-- ButtonBinder.context = data
-- BindContainer.context = data
-- _value = _value + 0.5
-- data:property_changed("slider_value")
VMState.push(VMGroup.welcome)

-- local util = require 'xlua.util'
-- util.print_func_ref_by_csharp()
-- VMState.push(VMGroup.source_demo)
-- require("test_notify_collection_changed")
-- require()

-- local Proxy=Proxy
-- local NetMsgHelper = NetMsgHelper
-- local NetAPIList = NetAPIList

-- -- require("netGame")

-- local function on_state_change(state) --资源回收
-- 	if state == StateManager.welcome then --当切换到welcome状态时候
-- 		StateManager:auto_dispose_items() --回收标记的item_object
-- 		unload_unused_assets()
-- 	end
-- end

-- local function on_state_changing(state)
--     -- gc
-- end

-- local function update()
--     local cmp
--     local len
--     len = #UPDATECOMPONENTS
--     local ostime=os.clock()
--     for i=1,len do
--         cmp=UPDATECOMPONENTS[i]
--         if cmp.enable then    cmp:on_update(ostime) end
--     end
-- end

-- pLua.updateFn=update

-- StateManager:input_disable() --锁定输入
-- StateManager:set_current_state(StateManager.welcome)
-- StateManager:register_state_change(on_state_change,true)
-- StateManager:register_state_changing(on_state_changing,true)

-- --load config
-- -- require("common.load_csv")

-- delay(function( ... )
-- 	-- print(lua_localization("level_name_001")) --language key
-- 	-- print_table(Model.getUnit(200001)) --read config
-- 	-- Loader:clearSharedAB()
--     print("hot res... 9999")
-- end,0.5)

-- local slider = CS.UnityEngine.GameObject.Find("/Logo/Slider1"):GetComponent("UnityEngine.UI.Slider")
-- local s1 = Binder.Slider()
-- Logger.Log(s1)
-- s1.value = 0.56
-- print(s1.value)
-- t1.text

-- s1.value = 0.57
-- Logger.Log(t1.text)
-- s1.value = 0.58
-- Logger.Log(t1.text)

-- t1.text = 0.59 ---??
-- Logger.Log(s1.value)
