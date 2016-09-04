------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	asset 
--	author pu
------------------------------------------------
local CUtils=CUtils
local LuaHelper=LuaHelper
local CryptographHelper = CryptographHelper
local GAMEOBJECT_ATLAS = GAMEOBJECT_ATLAS
Asset = class(function(self,url,names)
    self.base = false
    self.url = url
    self.asset_name = CUtils.GetAssetName(url) --asset name
    self.assetbundle_url = CUtils.GetRightFileName(url) --real use url
    self.key = CUtils.GetAssetBundleName(self.assetbundle_url) --以assetbundle name为key
   -- print("Asset url=",url,"assetbundle_url=",self.assetbundle_url,"asset_name=",self.asset_name)
    self.names = names
    if names then 
    	local len =#names local name
    	for i=1,len do name=names[i] names[name]=name   end 
	end
    self.items = {}
    self.root = nil
    self.refer = nil --ReferGameObjects
end)

--清理引用
function Asset:clear()
	self.root = nil
	self.refer = nil
	table.clear(self.items)
end

--消耗
function Asset:dispose()
	if self.root then LuaHelper.Destroy(self.root) end
	self.root = nil
	self.refer = nil
	GAMEOBJECT_ATLAS[self.key]=nil
	table.clear(self.items)
end

function Asset:show(...)
	if self.names then
		local item
		for k,v in pairs(self.names) do
			item = self.items[v]
			if item then item:SetActive(true) end
		end
		if self.root.activeSelf==false then  self.root:SetActive(true) end
	else
		if self.root then self.root:SetActive(true) end
	end
end

function Asset:hide(...)
	if  self.names then
		local kill
		for k,v in pairs(self.names) do
			kill = self.items[v]
			if kill then kill:SetActive(false) end
		end
	else
		if self.root then self.root:SetActive(false) end
	end
end

--
function Asset:copy_to(asse)
	if asse.type == nil then asse.type = self.type end
	local names=asse.names
	if names then
		asse.items={}
		for k,v in pairs(names) do
			local ref=self.items[v]
			asse.items[v] = ref
		end
	else
		asse.items = {}
		for k,v in pairs(self.items) do
			asse.items[k]=v
		end
	end
	asse.refer = self.refer
	asse.root = self.root
	return asse
end

function Asset:__tostring()
    return string.format("asset.key = %s ,url =%s ", self.key,self.url)
end