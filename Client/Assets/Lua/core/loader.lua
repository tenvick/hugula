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
-- local Common = Hugula.Utils.Common
local LuaHelper = Hugula.Utils.LuaHelper
local Object = UnityEngine.Object

-- local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application
-- local Shader = UnityEngine.Shader

local Loader = Loader 
-- local u3d_pattern = ".+%."..Common.ASSETBUNDLE_SUFFIX.."$"
-- local u_pattern = ".+%.u$"
-- local u3d_is_md5_patter = "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u3d$"
-- local u_is_md5_patter =  "[%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l][%d%l]_?%d*%.u$"
--warnning assetBundle 原始名字最好以  aa_bb 命名并且长度<=30
-- Loader.default_async = Application.platform ~= RuntimePlatform.IPhonePlayer

-- local function check_md5(url)
-- 	if string.match(url,u3d_pattern) and  string.match(url,u3d_is_md5_patter) == nil then --以u3d结尾
-- 	  	url = CUtils.GetRightFileName(url)  --md5编码
-- 	elseif string.match(url,u_pattern) and  string.match(url,u_is_md5_patter) == nil then
-- 	  	url = CUtils.GetRightFileName(url)  --md5编码
-- 	end--判断加密
-- 	return url
-- end

local function create_req_url(assetbundle_name,asset_name,asset_type,comp_fn,end_fn)
	if assetName == nil then assetName = CUtils.GetAssetName(assetbundle_name) end
	assetbundle_name = CUtils.GetRightFileName(assetbundle_name)--check_md5(assetbundle_name)
	if type(asset_type)=="string" then 
		asset_type = LuaHelper.GetClassType(asset_type) 
	end
	if asset_type == nil then asset_type = Object end
	local req = CRequest.Create(assetbundle_name,asset_name,asset_type,comp_fn,end_fn)
	return req
end

local function load_by_url(assetbundle_name,assetName,assetType,compFn,endFn)
	local req = create_req_url(assetbundle_name,assetName,assetType,compFn,endFn)
	ResourcesLoader.LoadAsset(req)
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
			local url,assetName,assetType,compFn,endFn,async
			if l1>0 then url = v[1]  end
			if l1>1 then assetName=v[2] end
			if v[3] then assetType=v[3] end
			if v[4] then compFn=v[4] end
			if v[5] then endFn=v[5] end

			if v.assetbundleName then url = v.assetbundleName end
			if v.assetName then assetName = v.assetName end
			if v.assetType then assetType = v.assetType end
			if v.OnComplete then compFn = v.OnComplete end
			if v.OnEnd then endFn = v.OnEnd end

			local req = create_req_url(url,assetName,assetType,compFn,endFn)
			table.insert(arrList,req)
		elseif typen=="userdata" then
			table.insert(arrList,v)
		end
	end
	ResourcesLoader.LoadLuaTable(arrList,group_fn,progress_fn,0)
end

function Loader:clear(key)
	CacheManager.Unload(key)
end

function Loader:check_ab_is_done(ab_name) --判断存在 1 存在
	local md5 = CUtils.GetRightFileName(ab_name)
	return ManifestManager.CheckABIsDone(md5)
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

-- load_by_url(assetBundleName,assetName,assetType,compFn,endFn)
-- load_by_url(assetBundleName,compFn,endFn)
--load_by_table( {assetBundleName,assetName,assetType,compFn,endFn},group_fn,progress_fn)
function Loader:get_resource(...)	
	local a,b,c,d,e,f,g,h,i = ...	
	--url,onComplete
	local t_a = type(a)
	if t_a=="string" and type(b)~="function" then 
		load_by_url(a,b,c,d,e)
	elseif t_a=="string" and type(b) == "function" then
		load_by_url(a,nil,nil,b,c)
	elseif t_a=="userdata" then
		ResourcesLoader.LoadAsset(a)
	elseif t_a == "table" then
		load_by_table(a,b,c)
	else
		error(" loader.lua line 163  this is no overloaded function for args")
	end
end

function Loader:get_http_data(...)
	local a,head,typ,on_comp,on_end = ...
	local t_a = type(a) 
	if t_a=="userdata" then
		ResourcesLoader.UnityWebRequest(a)
	else
		ResourcesLoader.UnityWebRequest(a,head,typ,on_comp,on_end)
	end
end

-- function Loader:get_web_data(...)
-- 	local a,head,typ,on_comp,on_end = ...
-- 	local t_a = type(a)
-- 	if t_a=="userdata" then
-- 		ResourcesLoader.WWWRequest(a)
-- 	else
-- 		ResourcesLoader.WWWRequest(a,head,typ,on_comp,on_end)
-- 	end
-- end

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

function Loader:refresh_assetbundle_manifest(on_ready)
	ManifestManager.LoadFileManifest(on_ready)
end

function Loader:set_background_loading_priority(thread_priority)
	Application.backgroundLoadingPriority = thread_priority
end

-- local function on_ab_complete(req,ab)
-- 	if not HUGULA_RELEASE then
-- 	--	TLogger.AddUpLogInfo(string.format("%s	%s	%d" ,req.key,os.date("%c",os.time()),UnityEngine.Time.frameCount))
-- 	end
-- 	if req.isShared and not LuaHelper.IsNull(ab) and Application.platform == RuntimePlatform.IPhonePlayer then --and Application.platform == RuntimePlatform.IPhonePlayer
-- 		ab:LoadAllAssets()
-- 	end
-- end

--
-- Loader:set_on_assetbundle_comp_fn(on_ab_complete)

-- print("require loader:"..os.time())