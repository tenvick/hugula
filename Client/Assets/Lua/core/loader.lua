------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
-- import "UnityEngine"
Loader={}
local Resources = Resources
local CTransport=CTransport
local LMultipleLoader= LHighway-- luanet.import_type("LHighway") --toLuaCS.LMultipleLoader-- local LHighway = luanet.LHighway.instance LMultipleLoader
LMultipleLoader=LMultipleLoader.instance
local Request=LRequest
local AssetBundle = UnityEngine.AssetBundle
local WWW = UnityEngine.WWW
--local to1=luanet.LuaHelper
--local to2=luanet.CUtils
local SetReqDataFromData=CHighway.SetReqDataFromData

local LuaHelper = LuaHelper
local delay = delay
local CUtils = CUtils

local Loader=Loader 
Loader.multipleLoader=LMultipleLoader
Loader.resdic={} --cache dic
Loader.shareCache ={}
LMultipleLoader.cache=Loader.resdic

-- printTable(getmetatable(LMultipleLoader))
local function dispatchComplete(req)
	if req.onCompleteFn then req.onCompleteFn(req) end
end

local function loadByUrl7(url,assetName,assetType,compFn,cache,endFn,head)
	local req=Request(url,assetName,assetType)

	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 
	if cache==nil or cache==true then req.cache=true end
	local key=req.key
	local cacheData=Loader.resdic[key]
	if cacheData~=nil then SetReqDataFromData(req,cacheData) dispatchComplete(req)
	else Loader.multipleLoader:LoadReq(req) 
	end
end

local function loadByUrl5(url,compFn,cache,endFn,head)
	local req=Request(url)
--    req.assetName=req.key
	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head~=nil then req.head=head end 
	if cache==nil or cache==true then req.cache=true end
	local key=req.key
	local cacheData=Loader.resdic[key]
	if cacheData~=nil then SetReqDataFromData(req,cacheData) dispatchComplete(req)
	else Loader.multipleLoader:LoadReq(req) 
	end
end

local function loadByReq( req,cache )
	if cache==nil or cache==true then req.cache=true end
	local key=req.key
	local cacheData=Loader.resdic[key]
	if cacheData~=nil then SetReqDataFromData(req,cacheData) dispatchComplete(req)
	else Loader.multipleLoader:LoadReq(req)
	end 
end

local function loadByTable(tb,cache)
	local arrList={} --ArrayList()
	local len=#tb
	local key = ""
	-- printTable(tb)
	--for i=1,len do
	for k,v in pairs(tb) do
--		local v = tb[i]
--        local arg=unpack(v)
--        print(arg)
--        Loader:getResource(arg)
		local l1=#v
		local req=Request(v[1])
		key=req.key
		if l1>1 then req.onCompleteFn=v[2] end
		if l1>2 then req.onEndFn=v[3] end
		if l1>3 then req.head=v[4] end
		if cache==nil or cache==true then req.cache=true end
		local cacheData=Loader.resdic[key]
		if cacheData~=nil then SetReqDataFromData(req,cacheData) dispatchComplete(req)
		else table.insert(arrList,req)--arrList:Add(req)
		end
	end
	Loader.multipleLoader:LoadLuaTable(arrList)
end

function Loader:clearItem(key)
	local assetBundle =	self.resdic[key]
	if assetBundle then	assetBundle:Unload(false) end
	self.resdic[key]=nil 
end

function Loader:clear(key)
	if key then 
		self:clearItem(key)
	elseif (key==true) then
		for k, v in pairs(self.resdic) do
			self:clearItem(k)
		end
	end 
--    unloadUnusedAssets()
end

function Loader:clearSharedAB()
    local share = Loader.shareCache
    for k,v in pairs(share) do
        v:Unload(false) 
        -- print("clearSharedAB "..k)
    end
    Loader.shareCache = {}
end

function Loader:unload(url)
	if url then
		local key=CUtils.getURLFullFileName(url)
		self:clearItem(key)
--        unloadUnusedAssets()
	end
end

--loadByUrl(url,compFn,cache,endFn,head)
--loadByTable( {url,compFn,endFn,head},cache)
function Loader:getResource(...)
	local a,b,c,d,e,f,g=...
	--print("Loader:getResource type=" ..type(a))
	--url,onComplete
	if type(a)=="string" and type(b)=="string" then 
		loadByUrl7(a,b,c,d,e,f,g)
    elseif type(a)=="string" then
		loadByUrl5(a,b,c,d,e)
	elseif type(a)=="userdata" then
		loadByReq(a,b)
	elseif type(a) == "table" then
		loadByTable(a,b)
	end
	--print("getResource  ed...."..tostring(a))
end

local function onCache( key,www)
	-- local key=req.key
	-- local www=req.data
	Loader.resdic[key]=www
end

local function onSharedComplete(req)
--	local typeName = req:GetType().Name
    local key=req.key
    local data=req.assetBundle
     Loader.resdic[key]= data
     Loader.shareCache[key] = data
     data:LoadAllAssets()
end

function Loader:setOnAllCompelteFn(compFn)
	self.multipleLoader.onAllCompleteFn=compFn
end

function Loader:setOnProgressFn(progFn)
	self.multipleLoader.onProgressFn=progFn
end

function Loader:RefreshAssetBundleManifest()

    local url = CUtils.GetAssetFullPath(CUtils.GetPlatformFolderForAssetBundles())
    local  function onCompleteFn (req1)
--        print(req1.key.." on comp ")
        local data=req1.data
        CTransport.m_AssetBundleManifest=data
        req1.assetBundle:Unload(false)
    end

    self:getResource(url,"assetbundlemanifest","UnityEngine.AssetBundleManifest",onCompleteFn)
end

-- print(Loader.multipleLoader.onCacheFn)
-- Loader.multipleLoader.onCacheFn=onCache
Loader.multipleLoader.onSharedCompleteFn=onSharedComplete