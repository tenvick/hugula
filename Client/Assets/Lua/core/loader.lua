------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
Loader={}
local Resources = Resources
local CTransport=CTransport
local Request=LRequest
local AssetBundle = UnityEngine.AssetBundle
local WWW = UnityEngine.WWW
local CacheManager = CacheManager
local SetReqDataFromData= CacheManager.SetRequestDataFromCache 

local LightHelper = LightHelper
local LuaHelper = LuaHelper
local delay = delay
local CUtils = CUtils

local Loader=Loader 
Loader.multipleLoader=LHighway.instance
-- Loader.resdic={} --cache dic
Loader.shareCache ={}

-- print_table(getmetatable(LHighway))
local function dispatch_complete(req)
	if req.onCompleteFn then req.onCompleteFn(req) end
end

local function load_by_url7(url,assetName,assetType,compFn,cache,endFn,head)
	local req=Request(url,assetName,assetType)

	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 
	-- if cache==nil or cache==true then req.cache=true end
	local key=req.key
	if SetReqDataFromData(req) then dispatch_complete(req)
	else Loader.multipleLoader:LoadReq(req) 
	end
end

local function load_by_url5(url,compFn,cache,endFn,head)	
	local req=Request(url)	
--    req.assetName=req.key
	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 
	-- if cache==nil or cache==true then req.cache=true end
	local key=req.key
	if SetReqDataFromData(req) then dispatch_complete(req)
	else Loader.multipleLoader:LoadReq(req) 
	end
end

local function load_by_req( req,cache )
	-- if cache==nil or cache==true then req.cache=true end
	-- local key=req.key
	-- local cacheData=Loader.resdic[key]
	if SetReqDataFromData(req) then dispatch_complete(req)
	else Loader.multipleLoader:LoadReq(req)
	end 
end

local function load_by_table(tb,cache)
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
			if v.url then req.url = v.url end
			if v.assetType then req.assetType = v.assetType end
			if v.onCompleteFn then req.onCompleteFn = v.onCompleteFn end
			if v.onEndFn then req.onEndFn = v.onEndFn end
			if v.head then req.head = v.head end
			key=req.key
			-- if cache==nil or cache==true then req.cache=true end
			if SetReqDataFromData(req) then dispatch_complete(req)
			else table.insert(arrList,req)--arrList:Add(req)
			end
		elseif typen=="userdata" then
			table.insert(arrList,v)
		end
	end
	local multipleLoader = Loader.multipleLoader
	multipleLoader.pushGroup = true --作为一组加载
	Loader.multipleLoader:LoadLuaTable(arrList) 
	multipleLoader.pushGroup = false
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
    local key=req.key
    local data=req.data --.assetBundle
	-- print(key.." Shared is laoded")
   	 -- LuaHelper.RefreshShader(data)
     local res = data:LoadAllAssets()
     --temp for lightmap
     -- local i = string.match(key,"lightmap%-(%d+)_comp_light")
     -- print(i,key)
     -- if i then
     -- 	LightHelper.SetLightMapSetting(tonumber(i),res[1],res[1])
     -- 	print("lightmap ",i,key,"settinged")
     -- end     
end

function Loader:set_onall_complete_fn(compFn)
	self.multipleLoader.onAllCompleteFn=compFn
end

function Loader:set_onprogress_fn(progFn)
	self.multipleLoader.onProgressFn=progFn
	self.multipleLoader:InitProgressState()
end

function Loader:refresh_assetbundle_manifest(onReady)

    local url = CUtils.GetAssetFullPath(CUtils.GetPlatformFolderForAssetBundles())
    local  function onCompleteFn (req1)
--        print(req1.key.." on comp ")
        local data=req1.data
        CTransport.m_AssetBundleManifest=data
        -- CacheManager.ClearCache(req1.key)
        if onReady then onReady() end
    end

    self:get_resource(url,"assetbundlemanifest","UnityEngine.AssetBundleManifest",onCompleteFn)
end

Loader.multipleLoader.onSharedCompleteFn=on_shared_complete