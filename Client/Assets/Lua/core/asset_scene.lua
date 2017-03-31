------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	asset 
--	author pu
------------------------------------------------
local CUtils=CUtils
local LuaHelper=LuaHelper
local CacheManager = Hugula.Loader.CacheManager
local GAMEOBJECT_ATLAS = GAMEOBJECT_ATLAS

AssetScene = class(function(self,url,scene_name,is_additive)
	self.base = false
    self.url = url
    self.is_additive = is_additive --(LoadSceneMode.Single,LoadSceneMode.Additive)
    self.asset_name = scene_name --asset name
    self.assetbundle_url = CUtils.GetRightFileName(url) --real use url
    self.key = CUtils.GetAssetBundleName(self.assetbundle_url) --以assetbundle name为key
    self.root = nil
end)

--清理引用
function AssetScene:clear()
	self.root = nil
	-- print("clear ",self.scene_name)
end

function AssetScene:is_loaded()
	if self.root == nil then return false end
	return true
end
--消耗
function AssetScene:dispose()
	CacheManager.Unload(self.key) --清理缓存
	self.root = nil
	GAMEOBJECT_ATLAS[self.key]=nil
end

function AssetScene:show(...)
	self.root = self
	-- print("scene show "..self.asset_name)
end

function AssetScene:hide(...)
	self:clear()
	LuaHelper.UnloadScene(self.asset_name)
end

--
function AssetScene:copy_to(asse)
    asse.root = self.root
	return asse
end

function AssetScene:__tostring()
    return string.format("AssetScene.key = %s ,url =%s ", self.key,self.url)
end