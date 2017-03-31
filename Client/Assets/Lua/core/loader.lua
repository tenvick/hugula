------------------------------------------------
--  Copyright © 2013-2016   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
local Hugula = Hugula
Loader={}
local Request = Hugula.Loader.LRequest
local LRequest = Hugula.Loader.LRequest --内存池
local CacheManager = Hugula.Loader.CacheManager
local LResLoader = Hugula.Loader.LResLoader
local CUtils = Hugula.Utils.CUtils
local Common = Hugula.Utils.Common
local LuaHelper = Hugula.Utils.LuaHelper

local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application

local Loader = Loader 
local u3d_pattern = ".+%."..Common.ASSETBUNDLE_SUFFIX.."$"
local u_pattern = ".+%.u$"
local u3d_is_md5_patter = "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u3d$"
local u_is_md5_patter =  "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u$"
--warnning assetBundle 原始名字最好以  aa_bb 命名并且长度<=30
Loader.multipleLoader= LResLoader.instance

local function dispatch_complete(req)
	if req.onCompleteFn then req.onCompleteFn(req) end
end

local function create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
	if assetName == nil then assetName = CUtils.GetAssetName(url) end
	local req = LRequest.Get() -- Request(url,assetName,assetType)
	if string.match(url,u3d_pattern) and  string.match(url,u3d_is_md5_patter) == nil then --以u3d结尾
		-- req.isLoadFromCacheOrDownload = true --从缓存加载
	  	url = CUtils.GetRightFileName(url)  --md5编码
	elseif string.match(url,u_pattern) and  string.match(url,u_is_md5_patter) == nil then
	  	url = CUtils.GetRightFileName(url)  --md5编码
	end--判断加密
	req.relativeUrl = url
	req.assetName = assetName
	if type(assetType)=="string" then 
		assetType = LuaHelper.GetClassType(assetType) 
	end
	
	if assetType ~= nil then req.assetType = assetType end
	
	if compFn then req.onCompleteFn=compFn end
	if endFn then req.onEndFn=endFn end
	if head ~= nil then req.head=head end 
	if isLoadFromCacheOrDownload ~= nil then req.isLoadFromCacheOrDownload = isLoadFromCacheOrDownload end
	local uri = url
	if uris then 
		req.uris = uris 
		uri = uris:GetUri(0)
	end
	if string.lower(string.sub(uri,1,4))=="http" then req.isNormal = false end --默认检测第一个uri如果是 http开头 非普通方式加载

	if Application.platform == RuntimePlatform.IPhonePlayer then
		req.async = false
	elseif  async ~= nil then
	 	req.async = async
	else
		req.async = true
	end

	-- print("create_req_url6 url=",req.url,"assetName=",assetName,"isNormal=",req.isNormal,"req.uris=",req.uris)
	return req
end

local function load_by_url6(url,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
	local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
	Loader.multipleLoader:LoadReq(req)
end

local function load_by_req( req )
	Loader.multipleLoader:LoadReq(req)
end

local function load_by_table(tb,group_fn,progress_fn)
	local arrList={} --ArrayList()
	local len=#tb
	local key = ""
	local typen = ""
	for k,v in ipairs(tb) do
		typen = type(v)
		if typen=="table" then
			local l1=#v
			local req 
			local url,assetName,assetType,compFn,endFn,uris,head,async,isLoadFromCacheOrDownload
			if l1>0 then url = v[1]  end
			if l1>1 then assetName=v[2] end
			if l1>2 then assetType=v[3] end
			if l1>3 then compFn=v[4] end
			if l1>4 then endFn=v[5] end
			if l1>5 then head=v[6] end
			if l1>6 then uris=v[7] end
			if l1>7 then async=v[8] end
			if l1>8 then isLoadFromCacheOrDownload = v[9] end

			if v.url then url = v.url end
			if v.assetName then assetName = v.assetName end
			if v.assetType then assetType = v.assetType end
			if v.onCompleteFn then compFn = v.onCompleteFn end
			if v.onEndFn then endFn = v.onEndFn end
			if v.uris then uris = v.uris end
			if v.head ~= nil then head = v.head end
			if v.async ~= nil then async = v.async end
			if v.isLoadFromCacheOrDownload ~= nil then isLoadFromCacheOrDownload = v.isLoadFromCacheOrDownload end

			local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
			table.insert(arrList,req)
		elseif typen=="userdata" then
			table.insert(arrList,v)
		end
	end
	Loader.multipleLoader:LoadLuaTable(arrList,group_fn,progress_fn) 
end

function Loader:clear(key)
	-- local t = type(key)
	--if t == "string" then 
	CacheManager.Unload(key)
	--end 
--    unload_unused_assets()
end

function Loader:stop_url(url)
	self.multipleLoader:StopURL(url)
end

function Loader:stop_req(req)
	self.multipleLoader:StopReq(req)
end

function Loader:unload(url)
	if url then
		local key=CUtils.GetAssetBundleName(url)
		self:clear(key)
--        unload_unused_assets()
	end
end

function Loader:unload_cache_false(assetbundle_name)
	return CacheManager.UnloadCacheFalse(assetbundle_name)
end

function Loader:unload_dependencies_cache_false(assetbundle_name)
	CacheManager.UnloadDependenciesCacheFalse(assetbundle_name)
end

-- load_by_url6(assetBundleName,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
-- load_by_url4(assetBundleName,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload)
--load_by_table( {assetBundleName,assetName,assetType,compFn,endFn,head,uris,async,isLoadFromCacheOrDownload},group_fn,progress_fn)
function Loader:get_resource(...)	
	local a,b,c,d,e,f,g,h,i = ...	
	--url,onComplete
	local t_a = type(a)
	if t_a=="string" and type(b)~="function" then 
		load_by_url6(a,b,c,d,e,f,g,h,i)
	elseif t_a=="string" and type(b) == "function" then
		load_by_url6(a,nil,nil,b,c,d,e,f,g)
	elseif t_a=="userdata" then
		load_by_req(a,b)
	elseif t_a == "table" then
		load_by_table(a,b,c)
	else
		error(" loader.lua line 116  this is no overloaded function for args")
	end
end

function Loader:set_onall_complete_fn(fn)
	self.multipleLoader.onAllCompleteFn = fn
end

function Loader:set_onprogress_fn(fn)
	self.multipleLoader.onProgressFn = fn
end

function Loader:set_on_assetbundle_comp_fn(fn)
	self.multipleLoader.onAssetBundleCompleteFn = fn
end

function Loader:set_on_assetbundle_err_fn(fn)
	self.multipleLoader.onAssetBundleErrFn = fn
end

function Loader:set_active_variants(vars)
	LResLoader.ActiveVariants = vars --{"sd"}
end

function Loader:set_maxloading(max)
	LResLoader.maxLoading = max --{"sd"}
end

function Loader:set_uris(uris)
	LResLoader.uriList = uris
end

function Loader:refresh_assetbundle_manifest(onReady)

    local url = CUtils.GetPlatformFolderForAssetBundles()
	url = CUtils.GetRightFileName(url)
    local  function on_complete_fn (req1)
        local data=req1.data
        LResLoader.assetBundleManifest=data
		print("manifest is done"..req1.url)
        if onReady then onReady() end
		CacheManager.UnloadCacheFalse(url) --清理缓存
    end

	local function on_erro_fn(req1)
		print("manifest is error"..req1.url)
		if onReady then onReady() end
	end

	self:clear(url) --清理旧的缓存
	
    self:get_resource(url,"assetbundlemanifest",UnityEngine.AssetBundleManifest,on_complete_fn,on_erro_fn,nil,LResLoader.uriList)
end


local function on_shared_complete(req)
	-- local deps = LResLoader.assetBundleManifest:GetAllDependencies(req.assetBundleName)
	-- print("<color=red>on_shared_complete "..req.assetBundleName.." "..req.assetName.."</color>")
	-- if deps.Length == 0 then
	local ab = req.data
	if Application.platform == RuntimePlatform.OSXEditor or Application.platform == RuntimePlatform.WindowsEditor or Application.platform == RuntimePlatform.WindowsPlayer then --for test
		
	elseif ab then
		-- LuaHelper.RefreshShader(ab)
		ab:LoadAllAssets()
	else
		print("on_shared_complete data is nil req(",req.assetBundleName,req.assetName)
	end
end

Loader.multipleLoader.onSharedCompleteFn = on_shared_complete

-- print("require loader:"..os.time())