------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------

local UnityEngine = UnityEngine
----------------------global-------------------------
GameObject=UnityEngine.GameObject
local Resources = UnityEngine.Resources

------------------------------static 变量---------------------------
LeanTween=LeanTween
LeanTweenType=LeanTweenType
Random = UnityEngine.Random
CUtils=CUtils --luanet.import_type("CUtils") -- --LCUtils --
LuaHelper=LuaHelper --LLuaHelper --luanet.import_type("LuaHelper")

local CRequest = Hugula.Loader.CRequest --内存池

Request = {}
Request.__index = Request

local request_meta = {}
request_meta.__call = function(tb,...)
	local url,assetName,assetType = ...
	local req = CRequest.Get()
	req.relativeUrl = url
	if assetName then req.assetName = assetName end
	if assetType then req.assetType = assetType end
	return req
end

setmetatable(Request,_mt)


-- --释放没使用的资源
-- function unload_unused_assets()
--     lua_gc()
--     Resources.UnloadUnusedAssets()
-- end

-----------------------global-----------------------------
GAMEOBJECT_ATLAS={} --resource cach table
UPDATECOMPONENTS={} --all update fun components
----------------------require-----------------------------
require("core.lua_object")
require("core.asset")
require("core.asset_scene")
require("core.view")

require("state.state_manager")
require("state.item_object")
require("state.state_base")

require("models.model")
