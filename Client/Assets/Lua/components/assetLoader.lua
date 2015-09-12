------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
--Gameobject 资源集合
local LuaHelper=LuaHelper
local Loader=Loader
local CUtils=CUtils
local Asset = Asset
local StateManager = StateManager
local GAMEOBJECT_ATLAS = GAMEOBJECT_ATLAS
--local SetReqDataFromData=toluacs.CHighway.SetReqDataFromData

local AssetLoader=class(function(self,luaObj)
	self.items={}
	self.luaObj=luaObj
	self.assets = nil
	self.enable = true
	self.loadCount = 0
	self.loadCurr = 0
end)

-- AssetLoader.loadCount=0
-- AssetLoader.loadCurr=0

function AssetLoader:onAssetLoaded(key,asset)
	self.assets[key]=asset
	self.loadCurr=self.loadCurr+1
    asset:show()
	self.luaObj:sendMessage("onAssetLoad",key,asset)
	-- print(string.format("AssetLoader.name=%s  loadCount %s loadCurr %s",self.luaObj.name,self.loadCount,self.loadCurr))
	if self.loadCurr >= self.loadCount then
		self.luaObj.assetsLoaded = true
		self.luaObj:sendMessage("onAssetsLoad",self.assets)
		if self.luaObj.onShowed then self.luaObj:onShowed() end
		if StateManager then StateManager:checkHideTransform() end --StateManager:onItemObjectAssetsLoaded(self.luaObj) end
	end
end

function AssetLoader:loadAssets(assets)
	local req = nil
	local reqs = {}
	local url = "" local key=""
	local asset = nil

	local onReqLoaded=function(req)
		-- print(req.key)
		local ass = req.head
		local baseAsset=Asset(ass.url)
		baseAsset.base = true
		local key = ass.key  --CUtils.GetKeyURLFileName(ass.url)

		if (GAMEOBJECT_ATLAS[key]~=nil) then
			baseAsset=GAMEOBJECT_ATLAS[key]
		else
			local main=req.data --.assetBundle.mainAsset
			LuaHelper.RefreshShader(req.assetBundle)
			local root=LuaHelper.Instantiate(main)
			root.name=main.name
			baseAsset.root=root
			local eachFn =function(i,obj)
				baseAsset.items[obj.name]=obj
			end

			LuaHelper.ForeachChild(root,eachFn)
			GAMEOBJECT_ATLAS[key]=baseAsset
		end
		baseAsset:copyTo(ass)
		Loader:clearItem(req.key)
		self:onAssetLoaded(key,ass)
	end

	local onErr=function(req) end

	for k,v in ipairs(assets) do
		key = v.key 
		local asst=GAMEOBJECT_ATLAS[key]
		if asst then
			asst:copyTo(v)
			--asst:show()
			self:onAssetLoaded(key,v)
		else
			req={v.fullUrl,onReqLoaded,onErr,v}
			table.insert(reqs,req)
		end
	end

    if #reqs>0 then	Loader:getResource(reqs) end
end

function AssetLoader:clear()
	--local at = GAMEOBJECT_ATLAS[self.name]
	for k,v in pairs(self.assets) do
		v:clear()		
	end	
	--GAMEOBJECT_ATLAS[self.name]=nil
	--if at then LuaHelper.Destroy(at.root) end
	self.items=nil
	self.url=nil
	self.name=nil
	self.luaObj.assetsLoaded = false
--    unloadUnusedAssets()
end

function AssetLoader:load(asts)
	self.loadCurr = 0
	self.assets = {}
	if asts~=nil then
		self.loadCount = #asts
		self:loadAssets(asts)
	end
end

function clearAssets()
	for k,v in pairs(GAMEOBJECT_ATLAS) do
		-- print(k.." is Destroy ")
		if v then LuaHelper.Destroy(v.root) end
	end
	GAMEOBJECT_ATLAS={}
end

return AssetLoader