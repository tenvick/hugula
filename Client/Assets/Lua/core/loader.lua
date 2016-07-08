------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
Loader={}
local Request=LRequest
local AssetBundle = UnityEngine.AssetBundle
local WWW = UnityEngine.WWW
local CacheManager = CacheManager

local CUtils = CUtils

local Loader=Loader 
Loader.multipleLoader= LResLoader.instance

local function dispatch_complete(req)
	if req.onCompleteFn then req.onCompleteFn(req) end
end

local function load_by_url7(url,assetName,assetType,compFn,cache,endFn,head)
	local req=Request(url,assetName,assetType)

	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 

	Loader.multipleLoader:LoadReq(req)
end

local function load_by_url5(url,compFn,cache,endFn,head)	
	local req=Request(url)	

	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 
	
	Loader.multipleLoader:LoadReq(req) 
end

local function load_by_req( req,cache )
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
			if l1>0 then req=Request(v[1]) else req = Request("") end
			if l1>1 then req.onCompleteFn=v[2] end
			if l1>2 then req.onEndFn=v[3] end
			if l1>3 then req.head=v[4] end
			if l1>4 then req.async=v[5] end

			if v.url then req.url = v.url end
			if v.assetType then req.assetType = v.assetType end
			if v.onCompleteFn then req.onCompleteFn = v.onCompleteFn end
			if v.onEndFn then req.onEndFn = v.onEndFn end
			if v.head then req.head = v.head end
			if v.async ~= nil then req.async = v.async end

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

--load_by_url5(url,compFn,cache,endFn,head)
--load_by_table( {url,compFn,endFn,head},cache)
function Loader:get_resource(...)	
	local a,b,c,d,e,f,g=...	
	--url,onComplete
	if type(a)=="string" and type(b)=="string" then 
		load_by_url7(a,b,c,d,e,f,g)
    elseif type(a)=="string" then
		load_by_url5(a,b,c,d,e)
	elseif type(a)=="userdata" then
		load_by_req(a,b)
	elseif type(a) == "table" then
		load_by_table(a,b)
	end
end

local function on_shared_complete(req)

end

function Loader:set_onall_complete_fn(compFn)
	self.multipleLoader.onAllCompleteFn=compFn
end

function Loader:set_onprogress_fn(progFn)
	self.multipleLoader.onProgressFn=progFn
end

function Loader:refresh_assetbundle_manifest(onReady)

    local url = CUtils.GetAssetFullPath(CUtils.GetPlatformFolderForAssetBundles())
    local  function onCompleteFn (req1)
        local data=req1.data
        LResLoader.assetBundleManifest=data
        if onReady then onReady() end
    end

    self:get_resource(url,"assetbundlemanifest","UnityEngine.AssetBundleManifest",onCompleteFn)
end

Loader.multipleLoader.onSharedCompleteFn=on_shared_complete