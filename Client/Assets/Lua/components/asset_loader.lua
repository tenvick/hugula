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

local AssetLoader=class(function(self,lua_obj)
	self.lua_obj=lua_obj
	self.assets = nil
	self.enable = true
	self._load_count = 0
	self._load_curr = 0
end)

-- AssetLoader._load_count=0
-- AssetLoader._load_curr=0

function AssetLoader:on_asset_loaded(key,asset)
	self.assets[key]=asset
	self._load_curr=self._load_curr+1
    asset:show()
	self.lua_obj:send_message("on_asset_load",key,asset)
	-- print(string.format("AssetLoader.name=%s  _load_count %s _load_curr %s",self.lua_obj.name,self._load_count,self._load_curr))
	if self._load_curr >= self._load_count then
		self.lua_obj.assets_loaded = true
		self.lua_obj:send_message("on_assets_load",self.assets)
		self.lua_obj:send_message("on_showed")
		-- if self.lua_obj.on_showed then self.lua_obj:on_showed() end
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
		self.lua_obj.assets_loaded = true
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
		local base_asset=Asset(ass.url)
		base_asset.base = true
		local key = ass.key 

		if (GAMEOBJECT_ATLAS[key]~=nil) then
			base_asset=GAMEOBJECT_ATLAS[key]
		else
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
		self:on_asset_loaded(key,ass)
	end

	local on_err = function(req) 
		local ass = req.head
		local key = ass.key
		self:on_asset_loaded_error(key,ass)
	end

	for k,v in ipairs(assets) do
		key = v.key 
		local asst=GAMEOBJECT_ATLAS[key]
		if asst then
			asst:copy_to(v)
			--asst:show()
			self:on_asset_loaded(key,v)
		else
			req={v.full_url,on_req_loaded,on_err,v}
			table.insert(reqs,req)
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
	self.lua_obj.assets_loaded = false
--    unload_unused_assets()
end

function AssetLoader:load(asts)
	self._load_curr = 0
	self.assets = {}
	if asts~=nil then
		self._load_count = #asts
		self:load_assets(asts)
	end
end

function clear_assets()
	for k,v in pairs(GAMEOBJECT_ATLAS) do
		-- print(k.." is Destroy ")
		if v then LuaHelper.Destroy(v.root) end
	end
	GAMEOBJECT_ATLAS={}
end

return AssetLoader