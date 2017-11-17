------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	asset 
--	author pu
------------------------------------------------
local CUtils=CUtils
local LuaHelper=LuaHelper
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

function Asset:add_child(asset)
	if 	self.children == nil then self.children = {} end
	local index = #self.children
	table.insert(self.children,asset)
	asset.parent = self
	asset.index = index --position
end

function Asset:is_loaded()
	if self.root == nil then return false end
	if self.children then
		for k,v in pairs(self.children) do
			if v:is_loaded() == false then return false end
		end
	end
	return true
end

--清理引用
function Asset:clear()
	self.root = nil
	self.refer = nil
	table.clear(self.items)
	self.ui_parent_joint = nil
	self.ui_joint = nil
end

--销毁
function Asset:dispose()
	if self.children then
		for k,v in pairs(self.children) do
			v:dispose()
		end
	end

	if self.root then
		LuaHelper.Destroy(self.root)
		-- print("Asset.dispose",self.key,self.asset_name)
	end
	self.ui_parent_joint = nil
	self.ui_joint = nil
	self.root = nil
	self.refer = nil
	if self.is_clone ~= true then GAMEOBJECT_ATLAS[self.key] = nil	end
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
	if  self.names and self.root then
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
	
	if asse.parent == nil then
		asse.items = {}
		for k,v in pairs(self.items) do
			asse.items[k]=v
		end
		asse.refer = self.refer
		asse.root = self.root
		asse.ui_parent_joint = self.ui_parent_joint
		self.is_clone = nil
	else
		local root = LuaHelper.Instantiate(self.root)
		root.name = self.root.name
		local eachFn =function(i,obj)
			asse.items[obj.name]=obj
		end
		LuaHelper.ForeachChild(root,eachFn)
		asse.is_clone = true
		asse.root = root
	end
	return asse
end

function Asset:__tostring()
    return string.format("asset.key = %s ,url =%s ", self.key or "",self.url or "")
end