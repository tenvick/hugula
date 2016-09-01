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
local LRequestPool = Hugula.Loader.LRequestPool --内存池

local AssetLoader=class(function(self,lua_obj)
	self.lua_obj=lua_obj
	self.assets = nil
	self.enable = true
	self._load_count = 0
	self._load_curr = 0
end)

local function create_request(v,on_req_loaded,on_err)
	local r = LRequestPool.Get()--Request(v.full_url)
	r.relativeUrl = v.full_url
	r.onCompleteFn = on_req_loaded
	r.onEndFn = on_err
	r.head = v
	r.async = true

	if v:is_a(Asset) then
		r.assetName = CUtils.GetAssetName(v.url)
	else
		-- print("load scene ",v.scene_name)
		r.assetName = v.scene_name
		r.isAdditive = v.is_additive
		r.assetType = AssetBundleScene --加载场景类型
	end

	return r
end

function AssetLoader:on_asset_loaded(key,asset)
	self.assets[key]=asset
	self._load_curr=self._load_curr+1
    asset:show()
	self.lua_obj:send_message("on_asset_load",key,asset)
	-- print(string.format("AssetLoader.name=%s  _load_count %s _load_curr %s ,key %s",self.lua_obj.name,self._load_count,self._load_curr,key))
	if self._load_curr >= self._load_count then
		self.lua_obj.is_call_assets_loaded = true
		self.lua_obj:send_message("on_assets_load",self.assets)
		-- print(string.format("AssetLoader.name=%s  ",asset.root))
		self.lua_obj:send_message("on_showed")
		self.lua_obj:call_event("on_showed")
		if StateManager and StateManager:get_current_state():is_all_loaded() then 
			StateManager:check_hide_transform()
			StateManager:call_all_item_method()
		end 
	end
end

function AssetLoader:on_asset_loaded_error(key,asset)
	self.assets[key]=asset
	self._load_curr=self._load_curr+1
	if self._load_curr >= self._load_count then
		self.lua_obj:send_message("on_assets_load",self.assets)
		self.lua_obj:send_message("on_showed")
		self.lua_obj:call_event("on_showed")
		if StateManager and StateManager:get_current_state():is_all_loaded() then 
			StateManager:check_hide_transform()
			StateManager:call_all_item_method()
		end 
	end
end

function AssetLoader:load_assets(assets)
	local req = nil
	local reqs = {}
	local url = "" local key=""
	local asset = nil

	local on_req_loaded=function(req)
		local ass = req.head
		local key = ass.key 

		if ass:is_a(Asset) then
			local base_asset=Asset(ass.url)
			base_asset.base = true

			if (GAMEOBJECT_ATLAS[key]~=nil) then
				base_asset=GAMEOBJECT_ATLAS[key]
			else
				-- print("on_req_loaded assetName=",req.assetName,"key=",req.key,req.assetBundleName)
				local main=req.data 
				local root=LuaHelper.Instantiate(main)
				root.name=main.name
				base_asset.root=root
				local eachFn =function(i,obj)
					base_asset.items[obj.name]=obj
				end
				LuaHelper.ForeachChild(root,eachFn)
				GAMEOBJECT_ATLAS[key]=base_asset
			end
			base_asset:copy_to(ass)
		else
			ass.root = ass
		end
		self:on_asset_loaded(key,ass)
	end

	local on_err = function(req) 
		local ass = req.head
		local key = ass.key
		self:on_asset_loaded_error(key,ass)
	end

	for k,v in ipairs(assets) do
		key = v.key 
		local asst=GAMEOBJECT_ATLAS[key] --print(key,asst)
		if asst then
			asst:copy_to(v)
			self:on_asset_loaded(key,v)
		else
			local r = create_request(v,on_req_loaded,on_err)
			table.insert(reqs,r)
		end
	end

    if #reqs>0 then	Loader:get_resource(reqs) end
end

function AssetLoader:clear()
	--local at = GAMEOBJECT_ATLAS[self.name]
	for k,v in pairs(self.assets) do
		v:clear()		
	end	
	--GAMEOBJECT_ATLAS[self.name]=nil
	--if at then LuaHelper.Destroy(at.root) end
--    unload_unused_assets()
end

function AssetLoader:load(asts)
	self._load_curr = 0
	self.assets = {}
	self._load_count = #asts
	self:load_assets(asts)

end

function clear_assets()
	for k,v in pairs(GAMEOBJECT_ATLAS) do
		-- print(k.." is Destroy ")
		if v then v:dispose() end
	end
	GAMEOBJECT_ATLAS={}
end

return AssetLoader