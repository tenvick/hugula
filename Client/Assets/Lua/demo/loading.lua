------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local pairs = pairs
local require = require
local table_insert = table.insert

local View = View
local VMState = VMState
local VMGroup = VMGroup
local VMManager = require("core.mvvm.vm_manager")

local Manager = Manager
local DispatcherEnum = DispatcherEnum
local CS = CS
local ResLoader = CS.Hugula.ResLoader
---@class VMBase vm
---@class loading
local loading = VMBase()

loading.views = {
    View(loading, {key = "loading"}) ---加载prefab
}

----------------------------------申明属性名用于绑定--------------
local property_slider_value = "slider_value"
local property_loading_text = "loading_text"

----------------------绑定属性-----------------------------------
loading.slider_value = 0
loading.loading_text = "loading..."

--------------------------预加载加载的模块------------------------
local pre_loading = {"asset_loader", "scene_loader"}

local function on_progress(loadingEvent)
    -- body
    loading:SetProperty(property_slider_value, loadingEvent.current / loadingEvent.total)
    -- Logger.Log(loadingEvent.current, loadingEvent.total)
end

local function all_complete()
    Logger.Log("all_complete")
    VMState:push(VMGroup.gamescene) --预加载完成后跳转到场景
    ResLoader.OnGroupProgress = nil
    ResLoader.OnGroupComplete = nil
end

local function load_async()
    ResLoader.OnGroupProgress = on_progress
    ResLoader.OnGroupComplete = all_complete

    ResLoader.BeginMarkGroup()
    for k, v in pairs(pre_loading) do
        Logger.Log(k,v)
        VMManager:pre_load(v)
    end
    ResLoader.EndMarkGroup()
end

------------------------------------------------------------------

---当vmbase的属性被设置的时候回调次方法
function loading:on_property_set(property)

end

function loading:on_push_arg(arg)
    --接受loading信息
    if arg ~= nil then
        VMState:call_func("scene_loader","on_push_arg",{scene_loader = arg})
	--VMGenerate.scene_loader:on_push_arg({scene_loader = arg})
    end

    --读取数据信息
    local assets = {}
    table_insert(assets,{key="Player1"})
    -- VMGenerate.asset_loader:on_push_arg({asset_loader=assets})
    VMState:call_func("asset_loader","on_push_arg",{asset_loader=assets})
end

function loading:on_active()
    load_async()
end

function loading:on_deactive()
end

---点击事件
loading.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        -- VMState:push(VMGroup.welcome)
    end
}

return loading
