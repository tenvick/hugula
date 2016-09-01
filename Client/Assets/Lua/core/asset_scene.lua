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
    self.key = CUtils.GetAssetBundleName(url)
    self.is_additive = is_additive --(LoadSceneMode.Single,LoadSceneMode.Additive)
    self.scene_name = scene_name
    self.full_url = CUtils.GetRightFileName(url)
    self.root = nil
end)

--清理引用
function AssetScene:clear()
	self.root = nil
	-- print("clear ",self.scene_name)
end

--消耗
function AssetScene:dispose()
	CacheManager.ClearCache(self.key) --清理缓存
	self.root = nil
	GAMEOBJECT_ATLAS[self.key]=nil
end

function AssetScene:show(...)
	self.root = self
	-- print("scene show "..self.scene_name)
end

function AssetScene:hide(...)
	self:clear()
	LuaHelper.UnloadScene(self.scene_name)
end

--
function AssetScene:copy_to(asse)
	if asse.type == nil then asse.type = self.type end
	asse.key = self.key
	asse.url = self.url
	asse.full_url = self.full_url
	asse.root = self.root
	asse.is_additive = self.is_additive
    asse.scene_name = self.scene_name
	return asse
end

function AssetScene:__tostring()
    return string.format("AssetScene.key = %s ,url =%s ", self.key,self.url)
end