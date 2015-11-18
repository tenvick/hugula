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

Request=LRequest --luanet.import_type("LRequest")

local LocalizationMy = Localization

--获取语言包内容
function getValue(key)
    return LocalizationMy.Get(key)
end

--释放没使用的资源
function unloadUnusedAssets()
    luaGC()
    Resources.UnloadUnusedAssets()
end

-----------------------global-----------------------------
GAMEOBJECT_ATLAS={} --resource cach table
UPDATECOMPONENTS={} --all update fun components
----------------------require-----------------------------
require("core.luaObject")
require("core.asset")

require("state.stateManager")
require("state.itemObject")
require("state.stateBase")

require("game.model.model")
