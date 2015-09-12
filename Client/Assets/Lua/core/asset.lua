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
    self.url = url --CUtils.GetAssetFullPath(url)
    self.fullUrl=CUtils.GetAssetFullPath(url)
   -- print(url)
    self.key = CUtils.GetKeyURLFileName(url)
    self.names = names
    if names then 
    	local len =#names local name
    	for i=1,len do name=names[i] names[name]=name   end 
	end
    self.items = {}
    self.root = nil
end)

function Asset:clear()
	-- if self.type == SINGLE then
		print("clear"..self.root.name)
		if self.root then LuaHelper.Destroy(self.root) end
		self.root = nil
		GAMEOBJECT_ATLAS[self.key]=nil
		self.items={}
		-- self.names=nil
		--self.names=nil
	-- elseif self.items then
	-- 	for k,v in pairs(self.items) do
	-- 		LuaHelper.Destory(v)
	-- 	end
	-- 	self.items=nil
	-- end
end

function Asset:show(...)
	if self.names then
		for k,v in pairs(self.names) do
			self.items[v]:SetActive(true)	
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
function Asset:copyTo(asse)
	if asse.type == nil then asse.type = self.type end
	asse.key = self.key
	asse.url = self.url
	asse.fullUrl = self.fullUrl
	local names=asse.names
	if names then
		asse.items={}
		for k,v in pairs(names) do
			local ref=self.items[v]
			asse.items[v] = ref
		end
	else
		asse.items=self.items
	end
	asse.root = self.root
	return asse
end

function Asset:__tostring()
    return string.format("asset.key = %s ,url =%s ", self.key,self.url)
end