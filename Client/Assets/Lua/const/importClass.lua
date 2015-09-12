------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
-- local luanet=luanet
-- UnityEngine=luanet.UnityEngine
local UnityEngine = UnityEngine
----------------------global-------------------------
GameObject=UnityEngine.GameObject
-- Vector3=UnityEngine.Vector3
-- Quaternion = UnityEngine.Quaternion
local Resources = UnityEngine.Resources

-----------------------init---------------------------

-- iTween = luanet.import_type("iTween")
LeanTweenType=LeanTweenType

------------------------------static 变量---------------------------
LeanTween=LeanTween
Random = UnityEngine.Random
CUtils=CUtils --luanet.import_type("CUtils") -- --LCUtils --
LuaHelper=LuaHelper --LLuaHelper --luanet.import_type("LuaHelper")
toluaiTween=iTween

-- PLua = luanet.import_type("PLua")
Net= LNet.instance -- luanet.import_type("LNet").instance
-- Msg=luanet.import_type("Msg")
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