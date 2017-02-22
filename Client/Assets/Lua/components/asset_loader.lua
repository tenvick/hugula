------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
--Gameobject 资源集合
local LuaHelper=LuaHelper
local GAMEOBJECT_ATLAS = GAMEOBJECT_ATLAS
local CUtils=CUtils
local AssetBundleScene = AssetBundleScene
local LRequestPool = Hugula.Loader.LRequestPool --内存池
local UIJoint = UIJoint
local UIParentJoint = UIParentJoint

local Loader=Loader
local Asset = Asset
local StateManager = StateManager
local send_message = send_message

local AssetLoader=class(function(self,lua_obj)
	self.lua_obj=lua_obj
	self.assets = nil
	self.enable = true
	self._load_count = 0
	self._load_curr = 0
end)

local function joint_child(ui_parent_joint,index,ui_joint)
	local len,v = ui_parent_joint.Length-1,nil
	for i=0,len do
		if i == index then
			v = ui_parent_joint[i+1]
			v:AddChild(ui_joint.transform,ui_joint.order)
			break
		end
	end
end

local function add_to_parent_asset(ass)
	if ass.parent ~= nil then 
		ass.ui_joint = ass.root:GetComponent(UIJoint)
		local ui_parent_joint = ass.parent.ui_parent_joint
		if ui_parent_joint and ass.ui_joint  then joint_child(ui_parent_joint,ass.index,ass.ui_joint) end
	end
end

local function add_child_asset(ass)
	local ui_parent_joint = ass.ui_parent_joint
	if ass.children and ui_parent_joint then
		for k,v in pairs(ass.children) do
			if v.ui_joint then joint_child(ui_parent_joint,v.index,v.ui_joint) end
		end
	end
end


local function create_request(v,on_req_loaded,on_err)
	local r = LRequestPool.Get()
	r.relativeUrl = v.assetbundle_url
	r.onCompleteFn = on_req_loaded
	r.onEndFn = on_err
	r.head = v
	r.async = true
	r.assetName = v.asset_name

	if v:is_a(AssetScene) then
		-- print("load scene ",v.scene_name)
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
	send_message(asset,"on_asset_load",key,asset)
	-- print(string.format("AssetLoader.name=%s  _load_count %s _load_curr %s ,key %s",self.lua_obj.name,self._load_count,self._load_curr,key))
	if self._load_curr >= self._load_count then
		self.lua_obj.is_loading = nil
		self.lua_obj.is_call_assets_loaded = true
		self.lua_obj:send_message("on_assets_load",self.assets)
		-- print(string.format("AssetLoader.name=%s  ",asset.root))
		self.lua_obj:send_message("on_showed")
		self.lua_obj:call_event("on_showed")
		if StateManager and StateManager:get_current_state():is_all_loaded() then 
			-- StateManager:check_hide_transform(self.lua_obj._transform)
			StateManager:call_all_item_method()
		end 
	end
end

function AssetLoader:on_asset_loaded_error(key,asset)
	self.assets[key]=asset
	self._load_curr=self._load_curr+1
	if self._load_curr >= self._load_count then
		self.lua_obj.is_loading = nil
		self.lua_obj:send_message("on_assets_load",self.assets)
		self.lua_obj:send_message("on_showed")
		self.lua_obj:call_event("on_showed")
		if StateManager and StateManager:get_current_state():is_all_loaded() then 
			-- StateManager:check_hide_transform(self.lua_obj._transform)
			StateManager:call_all_item_method()
		end 
	end
end

function AssetLoader:load_assets(assets)
	self.lua_obj.is_loading = true
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
				-- Loader:unload_cache_false(req.keyHashCode)
				-- print("unload_cache_false"..req.assetName)
				base_asset.root=root
				local eachFn =function(i,obj)
					base_asset.items[obj.name]=obj
				end
				LuaHelper.ForeachChild(root,eachFn)

				if ass.children then  
					base_asset.ui_parent_joint =  root:GetComponentsInChildren(UIParentJoint)
					-- print("ass.children",ass.asset_name,base_asset.ui_parent_joint)
				end

				if ass.parent then
					root:SetActive(false)
				end

				GAMEOBJECT_ATLAS[key]=base_asset
			end

			base_asset:copy_to(ass)
			add_child_asset(ass)
			add_to_parent_asset(ass)
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
			add_to_parent_asset(v)
			self:on_asset_loaded(key,v)
		else
			local r = create_request(v,on_req_loaded,on_err)
			table.insert(reqs,r)
		end
	end

    if #reqs>0 then
    	Loader:get_resource(reqs,self._onall_complete,self._on_progress) 
    else
    	if self._onall_complete then self._onall_complete() end
    end

	self._onall_complete = nil
	self._on_progress = nil
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

function AssetLoader:load(asts,onall_complete,on_progress)

	self.assets = {}
	local all_assets = {}
	for k,v in ipairs(asts) do
		table.insert(all_assets,v)
		if v.children then 
			for k1,v1 in ipairs(v.children) do table.insert(all_assets,v1)  end
		end
	end
	if self.lua_obj.is_loading then print("warring something is loading ") end
	self._load_curr = 0
	self._load_count = #all_assets
	self._on_progress = on_progress
	self._onall_complete = onall_complete
	self:load_assets(all_assets)

end

function clear_assets()
	for k,v in pairs(GAMEOBJECT_ATLAS) do
		-- print(k.." is Destroy ")
		if v then v:dispose() end
	end
	GAMEOBJECT_ATLAS={}
end

return AssetLoader