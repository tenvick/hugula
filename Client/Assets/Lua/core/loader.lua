------------------------------------------------
--  Copyright © 2013-2016   Hugula: Arpg game Engine
--	loader loadresouce from assetbundl
--	author pu
------------------------------------------------
local Hugula = Hugula
Loader={}
local CRequest = Hugula.Loader.CRequest
local CacheManager = Hugula.Loader.CacheManager
local ResourcesLoader = Hugula.Loader.ResourcesLoader
local ManifestManager = Hugula.Loader.ManifestManager
local CUtils = Hugula.Utils.CUtils
local Common = Hugula.Utils.Common
local LuaHelper = Hugula.Utils.LuaHelper
local Object = UnityEngine.Object

local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application
local Shader = UnityEngine.Shader

local Loader = Loader 
local u3d_pattern = ".+%."..Common.ASSETBUNDLE_SUFFIX.."$"
local u_pattern = ".+%.u$"
local u3d_is_md5_patter = "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u3d$"
local u_is_md5_patter =  "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u$"
--warnning assetBundle 原始名字最好以  aa_bb 命名并且长度<=30
Loader.default_async = Application.platform ~= RuntimePlatform.IPhonePlayer

local function check_md5(url)
	if string.match(url,u3d_pattern) and  string.match(url,u3d_is_md5_patter) == nil then --以u3d结尾
	  	url = CUtils.GetRightFileName(url)  --md5编码
	elseif string.match(url,u_pattern) and  string.match(url,u_is_md5_patter) == nil then
	  	url = CUtils.GetRightFileName(url)  --md5编码
	end--判断加密
	return url
end

local function create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	if assetName == nil then assetName = CUtils.GetAssetName(url) end
	url = check_md5(url)
	if type(assetType)=="string" then 
		assetType = LuaHelper.GetClassType(assetType) 
	end
	if async == nil then async = Loader.default_async end
	if assetType == nil then assetType = Object end
	-- print("create_req_url6 url=",req.url,"assetName=",assetName,"req.type=",assetType)
	local req = CRequest.Create(url,assetName,assetType,compFn,endFn,head,async)
	return req
end

local function load_by_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
	-- Loader.multipleLoader:LoadReq(req)
	ResourcesLoader.LoadAsset(req,false)
end

local function load_by_req( req )
	-- Loader.multipleLoader:LoadReq(req)
	ResourcesLoader.LoadAsset(req,false)
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
			local url,assetName,assetType,compFn,endFn,uris,head,async
			if l1>0 then url = v[1]  end
			if l1>1 then assetName=v[2] end
			if v[3] then assetType=v[3] end
			if v[4] then compFn=v[4] end
			if v[5] then endFn=v[5] end
			if v[6]~=nil then head=v[6] end
			if v[7] then uris=v[7] end
			if v[8]~=nil then async=v[8] end

			if v.url then url = v.url end
			if v.assetName then assetName = v.assetName end
			if v.assetType then assetType = v.assetType end
			if v.OnComplete then compFn = v.OnComplete end
			if v.OnEnd then endFn = v.OnEnd end
			if v.uris then uris = v.uris end
			if v.head ~= nil then head = v.head end
			if v.async ~= nil then async = v.async end

			local req = create_req_url6(url,assetName,assetType,compFn,endFn,head,uris,async)
			table.insert(arrList,req)
		elseif typen=="userdata" then
			table.insert(arrList,v)
		end
	end
	-- Loader.multipleLoader:LoadLuaTable(arrList,group_fn,progress_fn) 
	ResourcesLoader.LoadLuaTable(arrList,group_fn,progress_fn,0)
end

function Loader:clear(key)
	CacheManager.Unload(key)
end

function Loader:check_ab_is_done(ab_name) --判断存在 1 存在
	local md5 = CUtils.GetRightFileName(ab_name)
	return ManifestManager.CheckABIsDone(md5)
end

function Loader:stop_url(url)
	-- self.multipleLoader:StopURL(url)
end

function Loader:stop_req(req)
	-- self.multipleLoader:StopReq(req)
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

-- load_by_url6(assetBundleName,assetName,assetType,compFn,endFn,head,uris,async)
-- load_by_url4(assetBundleName,compFn,endFn,head,uris,async)
--load_by_table( {assetBundleName,assetName,assetType,compFn,endFn,head,uris,async},group_fn,progress_fn)
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
		error(" loader.lua line 163  this is no overloaded function for args")
	end
end

--string url, object head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd
function Loader:get_http_data(...)
	local a,b,c,d,e = ...
	local t_a = type(a)
	if t_a=="userdata" then
		ResourcesLoader.WWWRequest(a,false)
	else
		ResourcesLoader.WWWRequest(a,b,c,d,e)
	end
end

--string url, object head, System.Type type, System.Action<CRequest> onComplete, System.Action<CRequest> onEnd
function Loader:get_www_data(...)
	local a,b,c,d,e,f = ...
	local t_a = type(a)
	if t_a=="userdata" then
		ResourcesLoader.WWWRequest(a,false)
	else
		ResourcesLoader.WWWRequest(a,b,c,d,e)
	end
end

function Loader:set_onall_complete_fn(fn)
	ResourcesLoader.OnAllComplete = fn
end

function Loader:set_onprogress_fn(fn)
	ResourcesLoader.OnProgress = fn
end

function Loader:set_on_assetbundle_comp_fn(fn)
	ResourcesLoader.OnAssetBundleComplete = fn
end

function Loader:set_on_assetbundle_err_fn(fn)
	ResourcesLoader.OnAssetBundleErr = fn
end

function Loader:set_active_variants(vars)
	ManifestManager.ActiveVariants = vars
end

function Loader:set_maxloading(max)
	ResourcesLoader.maxLoading = max
end

function Loader:set_uris(uris)

end

function Loader:refresh_assetbundle_manifest(on_ready)
	ManifestManager.LoadFileManifest(on_ready)
end

local function on_ab_complete(req,ab)

	if req.isShared and not LuaHelper.IsNull(ab) and Application.platform == RuntimePlatform.IPhonePlayer then --and Application.platform == RuntimePlatform.IPhonePlayer
		ab:LoadAllAssets()
	else
		-- print("on_shared_complete data is nil req(",req.assetBundleName,req.assetName)
	end
end

Loader:set_on_assetbundle_comp_fn(on_ab_complete)

-- print("require loader:"..os.time())