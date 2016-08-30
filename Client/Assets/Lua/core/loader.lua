------------------------------------------------
--  Copyright © 2013-2016   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
local Hugula = Hugula
Loader={}
local Request=Hugula.Loader.LRequest
local LRequestPool = Hugula.Loader.LRequestPool --内存池
local CacheManager = Hugula.Loader.CacheManager
local LResLoader = Hugula.Loader.LResLoader
local CUtils = Hugula.Utils.CUtils
local Loader=Loader 
local LuaHelper=Hugula.Utils.LuaHelper

Loader.multipleLoader= LResLoader.instance

local function dispatch_complete(req)
	if req.onCompleteFn then req.onCompleteFn(req) end
end

local function create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	local key = CUtils.GetKeyURLFileName(url) --获取key
	if assetName == nil then assetName = key end
	url = CUtils.GetFileName(url) --判断加密
	local req = LRequestPool.Get() -- Request(url,assetName,assetType)
	req.relativeUrl = url
	req.assetName = assetName
	if type(assetType)=="string" then 
		assetType = LuaHelper.GetClassType(assetType) 
	end

	if assetType ~= nil then req.assetType = assetType end
	
	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head ~= nil then req.head=head end 
	if uris then req.uris = uris end
	if async ~= nil then req.async = async end

	return req
end

local function load_by_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	Loader.multipleLoader:LoadReq(req)
end

local function load_by_req( req )
	Loader.multipleLoader:LoadReq(req)
end

local function load_by_table(tb,group_fn)
	local arrList={} --ArrayList()
	local len=#tb
	local key = ""
	local typen = ""
	for k,v in ipairs(tb) do
		typen = type(v)
		if typen=="table" then
			local l1=#v
			local req 
			local url,assetName,assetType,compFn,endFn,uris,head,async
			if l1>0 then url = v[1]  end
			if l1>1 then assetName=v[2] end
			if l1>2 then assetType=v[3] end
			if l1>3 then compFn=v[4] end
			if l1>4 then endFn=v[5] end
			if l1>5 then head=v[6] end
			if l1>6 then uris=v[7] end
			if l1>7 then async=v[8] end

			if v.url then url = v.url end
			if v.assetName then assetName = v.assetName end
			if v.assetType then assetType = v.assetType end
			if v.onCompleteFn then compFn = v.onCompleteFn end
			if v.onEndFn then endFn = v.onEndFn end
			if v.uris then uris = v.uris end
			if v.head ~= nil then head = v.head end
			if v.async ~= nil then async = v.async end

			local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
			table.insert(arrList,req)
		elseif typen=="userdata" then
			table.insert(arrList,v)
		end
	end
	Loader.multipleLoader:LoadLuaTable(arrList,group_fn) 
end

function Loader:clear(key)
	local t = type(key)
	if t == "string" then 
		CacheManager.ClearCache(key)
	end 
--    unload_unused_assets()
end

function Loader:unload(url)
	if url then
		local key=CUtils.getURLFullFileName(url)
		self:clear(key)
--        unload_unused_assets()
	end
end

-- load_by_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
-- load_by_url4(url,compFn,endFn,head,uris,async)
--load_by_table( {url,compFn,endFn,head},group_fn)
function Loader:get_resource(...)	
	local a,b,c,d,e,f,g,h = ...	
	--url,onComplete
	local t_a = type(a)
	if t_a=="string" and (type(b)=="string" or type(b)=="nil")then 
		load_by_url6(a,b,c,d,e,f,g,h)
	elseif t_a=="string" and type(b) == "function" then
		load_by_url6(a,nil,nil,b,c,d,e,f)
	elseif t_a=="userdata" then
		load_by_req(a,b)
	elseif t_a == "table" then
		load_by_table(a,b)
	else
		error(" loader.lua line 116  this is no overloaded function for args")
	end
end

local function on_shared_complete(req)
	-- local deps = LResLoader.assetBundleManifest:GetAllDependencies(req.assetBundleName)
	-- print(req.key.." on_shared_complete "..req.assetBundleName.." "..tostring(deps.Length).." "..req.relativeUrl)
	-- if deps.Length == 0 then
	local ab = req.data
	LuaHelper.RefreshShader(ab)
	ab:LoadAllAssets()
	-- end
end

function Loader:set_onall_complete_fn(compFn)
	self.multipleLoader.onAllCompleteFn=compFn
end

function Loader:set_onprogress_fn(progFn)
	self.multipleLoader.onProgressFn=progFn
end

function Loader:set_active_variants(vars)
	LResLoader.ActiveVariants = vars --{"sd"}
end

function Loader:refresh_assetbundle_manifest(onReady)

    local url = CUtils.GetPlatformFolderForAssetBundles()
    local  function onCompleteFn (req1)
        local data=req1.data
        LResLoader.assetBundleManifest=data
        if onReady then onReady() end
    end

    self:get_resource(url,"assetbundlemanifest",UnityEngine.AssetBundleManifest,onCompleteFn)
end

Loader.multipleLoader.onSharedCompleteFn=on_shared_complete