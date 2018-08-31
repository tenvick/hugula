------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
--Gameobject 资源集合
local RuntimePlatform = UnityEngine.RuntimePlatform
local Application = UnityEngine.Application
local Object = UnityEngine.Object
local LuaHelper = LuaHelper
local LogicHelper = Hugula.LogicHelper
local GAMEOBJECT_ATLAS = GAMEOBJECT_ATLAS
local CUtils = CUtils
local AssetBundleScene = AssetBundleScene
local CRequest = Hugula.Loader.CRequest --内存池
local UIJoint = UIJoint
local UIParentJoint = UIParentJoint
local ManifestManager = Hugula.Loader.ManifestManager
local Time = UnityEngine.Time

local Loader = Loader
local Asset = Asset
local StateManager = StateManager
local send_message = send_message
local delay = delay
local AssetLoader = class(function(self, lua_obj)
	self.lua_obj = lua_obj
	self.assets = nil
	self.enable = true
	self._load_count = 0
	self._load_curr = 0
end)

local function joint_child(ui_parent_joint, index, ui_joint)
	local len, v = ui_parent_joint.Length - 1, nil
	for i = 0, len do
		if i == index then
			v = ui_parent_joint[i + 1]
			v:AddChild(ui_joint.transform, ui_joint.order)
			break
		end
	end
end

local function add_to_parent_asset(ass)
	if ass.parent ~= nil then
		ass.ui_joint = ass.root:GetComponent(UIJoint)
		local ui_parent_joint = ass.parent.ui_parent_joint
		if ui_parent_joint and ass.ui_joint then joint_child(ui_parent_joint, ass.index, ass.ui_joint) end
	end
end

local function add_child_asset(ass)
	local ui_parent_joint = ass.ui_parent_joint
	if ass.children and ui_parent_joint then
		for k, v in pairs(ass.children) do
			if v.ui_joint then joint_child(ui_parent_joint, v.index, v.ui_joint) end
		end
	end
end


local function create_request(v, on_req_loaded, on_err)
	local req
	if v:is_a(AssetScene) then
		 req = CRequest.Create(v.assetbundle_url,v.asset_name,AssetBundleScene,on_req_loaded,on_err)
		 if v.is_additive == true then req.isAdditive = true end
	else
		-- print("create request..........",v.asset_name)
		req = CRequest.Create(v.assetbundle_url,v.asset_name,Object,on_req_loaded,on_err)
	end
	req.uploadData = v
	return req
end

function AssetLoader:on_asset_loaded(key, asset)
	self.assets[key] = asset
	self._load_curr = self._load_curr + 1
	asset:show()
	local lua_obj = self.lua_obj

	lua_obj:send_message("on_asset_load", key, asset)
	send_message(asset, "on_asset_load", key, asset)

	-- print("on_asset_load (",asset.asset_name,") delta time = ",os.clock()-asset._start_time,Time.frameCount)
	-- print(string.format("AssetLoader.name=%s,_load_count=%s,_load_curr=%s,key=%s,self.is_on_blur=%s",self.lua_obj.name,self._load_count,self._load_curr,key,tostring(is_on_blur)))
	if self._load_curr >= self._load_count then
		lua_obj.is_loading = nil
		lua_obj.is_call_assets_loaded = true
		lua_obj:send_message("on_assets_load", self.assets)
		-- print("on_assets_load (",lua_obj,") delta time = ",os.clock()-lua_obj._start_time,Time.frameCount)
		local call_showed = function()
			if not lua_obj.is_on_blur then
				lua_obj:send_message("on_showed")
				lua_obj:call_event("on_showed")
			else
				lua_obj:auto_mark_dispose() --标记回收
			end
			-- print("on_showed (",lua_obj,") delta time = ",os.clock()-lua_obj._start_time,Time.frameCount)
			if StateManager:is_in_current_state(lua_obj) and StateManager:get_current_state():is_all_loaded() then
				StateManager:call_all_item_method()
			end
		end
			
		delay(call_showed,0.01)	
		
	end
	
	if lua_obj.is_on_blur then
		asset:hide()
		-- print(string.format("%s on_blur  asset:hide() ",asset.url))
	end
end

function AssetLoader:on_asset_loaded_error(key, asset)
	self.assets[key] = asset
	self._load_curr = self._load_curr + 1
	if self._load_curr >= self._load_count then
		self.lua_obj.is_loading = nil
		self.lua_obj:send_message("on_assets_load", self.assets)
		
		if not self.is_on_blur then
			self.lua_obj:send_message("on_showed")
			self.lua_obj:call_event("on_showed")
		end
		
		if StateManager:is_in_current_state(self.lua_obj) and StateManager:get_current_state():is_all_loaded() then
			StateManager:call_all_item_method()
		end
	end
end

function AssetLoader:load_assets(assets)
	self.lua_obj.is_loading = true
	local req = nil
	local reqs = {}
	local url = "" local key = ""
	local asset = nil
	
	local on_req_loaded = function(req)
		local ass =  req.uploadData --req.userData
		local key = ass.key
		
		if ass:is_a(Asset) then
			local base_asset = Asset(ass.url)
			base_asset.base = true
			
			if(GAMEOBJECT_ATLAS[key] ~= nil) then
				base_asset = GAMEOBJECT_ATLAS[key]
			else

				LogicHelper.OnAssetsLoad(req,base_asset)
				-- local main = req.data
				-- local root = LuaHelper.Instantiate(main)
				-- root.name = main.name
				-- base_asset.root = root
				-- local eachFn = function(i, obj)
				-- 	base_asset.items[obj.name] = obj
				-- end
				-- LuaHelper.ForeachChild(root, eachFn)
				local root = base_asset.root

				if ass.children then
					base_asset.ui_parent_joint = root:GetComponentsInChildren(UIParentJoint)
					-- print("ass.children",ass.asset_name,base_asset.ui_parent_joint)
				end
				
				if ass.parent then
					root:SetActive(false)
				end
				
				GAMEOBJECT_ATLAS[key] = base_asset
			end
			
			base_asset:copy_to(ass)
			add_child_asset(ass)
			add_to_parent_asset(ass)
		else
			ass.root = ass
		end
		self:on_asset_loaded(key, ass)
	end
	
	local on_err = function(req)
		local ass =  req.uploadData--req.userData
		local key = ass.key
		self:on_asset_loaded_error(key, ass)
	end
	
	for k, v in ipairs(assets) do
		key = v.key
		if v.variant then v:set_url(ManifestManager.GetVariantName(v.url)) print("variant",v.url) end
		local asst = GAMEOBJECT_ATLAS[key] --print(key,asst)
		v._start_time = os.clock()
		if asst then
			asst:copy_to(v)
			add_to_parent_asset(v)
			self:on_asset_loaded(key, v)
		else
			local r = create_request(v, on_req_loaded, on_err)
			table.insert(reqs, r)
		end
	end
	
	if #reqs > 0 then
		Loader:get_resource(reqs, self._onall_complete, self._on_progress)
	else
		if self._onall_complete then self._onall_complete() end
	end
	
	self._onall_complete = nil
	self._on_progress = nil
end

function AssetLoader:clear()
	--local at = GAMEOBJECT_ATLAS[self.name]
	for k, v in pairs(self.assets) do
		v:clear()				
	end	
	--GAMEOBJECT_ATLAS[self.name]=nil
	--if at then LuaHelper.Destroy(at.root) end
	--    unload_unused_assets()
end

function AssetLoader:load(asts, onall_complete, on_progress)
	-- local dug_str = ""
	self.assets = {}
	local all_assets = {}
	self._load_count = 0 --#all_assets
	
	for k, v in ipairs(asts) do
		table.insert(all_assets, v)
		self._load_count = self._load_count + 1
		-- dug_str = dug_str .. v.asset_name .. ","
		if v.children then
			for k1, v1 in ipairs(v.children) do
				table.insert(all_assets, v1)
				self._load_count = self._load_count + 1
			end
		end
	end
	if self.lua_obj.is_loading then print("warring something is loading lua_obj=",self.lua_obj) end
	self.lua_obj._start_time = os.clock()
	self._load_curr = 0
	self._on_progress = on_progress
	self._onall_complete = onall_complete
	self:load_assets(all_assets)
	
end

function clear_assets()
	for k, v in pairs(GAMEOBJECT_ATLAS) do
		-- print(k.." is Destroy ")
		if v then v:dispose() end
	end
	GAMEOBJECT_ATLAS = {}
end

return AssetLoader 