------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
-- require("core.unity3d")
require("core.loader")
json=require("lib/json")
ResVersion = 0
luanet = _G
toluacs = _G
local resourceURL ="http://192.168.18.152:8345/api" --http://121.199.51.39:8080/client_update?v_id=%s&platform=%s&code=%s";

local _progressbar_txt;
local update_id="";
local FRIST_VIEW = "frist.u3d";
local VIDEO_NAME = "Loading.mp4";
local VERSION_FILE_NAME = "Ver.t";
-- local UPDATING_LIST_TMEP = "updating.tmp"
local UPDATED_LIST_TMEP = "updated.tmp"
local luanet = luanet
local RuntimePlatform= toluacs.UnityEngine.RuntimePlatform
local Application= toluacs.UnityEngine.Application
local PlayerPrefs = toluacs.UnityEngine.PlayerPrefs
local WWW = toluacs.UnityEngine.WWW
local Request=luanet.LRequest 

local CUtils=CUtils
local LuaHelper=LuaHelper
local FilLoadereHelper=FileHelper -- luanet.import_type("FileHelper")
local Localization = Localization
local delay = delay
local Loader = Loader

--local fristView
local all,loaded = 0,0

local function enterGame()
     -- languageInit()
 	 require("begin")
end

local function set_progress_txt(text)
 	if _progressbar_txt then _progressbar_txt.text = text end
end

local function on_progress(loader,arg)
	-- body
end

local function add_loaded_file(key)
	local da = FileHelper.ReadUTF8File(UPDATED_LIST_TMEP)
	if da==nil then da = "{}" end
	local jsda=json:decode(da)
	jsda[key]=true
	da = json:encode(jsda)
	FileHelper.PersistentUTF8File(da,UPDATED_LIST_TMEP)
end

local function save_version()
	FileHelper.PersistentUTF8File(update_id,VERSION_FILE_NAME)	
end

local function on_all_update_res_comp()
	set_progress_txt("wait start...")
    Loader:set_onall_complete_fn(nil)
	Loader:set_onprogress_fn(nil)
	save_version()
	FileHelper.DeleteFile(UPDATED_LIST_TMEP)
	-- enterGame(true)
	PLua:LoadBundle(enterGame) --reload LuaBind
end

local function on_update_item_comp(req)
	local bytes=req.data --get_data().bytes
	if(bytes~=nil) then
		FileHelper.UnZipFile(bytes,Application.persistentDataPath)
		add_loaded_file(req.key)
	end
	loaded = loaded+1
	set_progress_txt(string.format("%s %s/%s","开始从网络加载资源包请不要关闭程序。",loaded,all))

	if all<=loaded then on_all_update_res_comp() end
end

local function  on_update_res_comp(req)
    local www=req.data;
	if(www) then
		local txt=www[0]
        local res = json:decode(txt)
		if res["error"] then
			enterGame()
		elseif res["update_url"] then
			update_id = res["update_id"]
			if update_id> tonumber(ResVersion) then
				local loadtab = {}
				local old=FileHelper.ReadUTF8File(UPDATED_LIST_TMEP) --是否有下载完成的内容
				if old then loadtab=json:decode(old) end
				-- print(" id "..update_id)
				local upURL=res["update_url"]
				local reqs={}
				local len=#upURL
				local itemURl = ""
				local key=""
				for i=1,len do
					itemURl = upURL[i]
	 				key = CUtils.GetKeyURLFileName(itemURl)
					if not loadtab[key] then
						table.insert(reqs,{itemURl,on_update_item_comp,assetType="System.Byte[]"})
					end
				end
				all=#reqs
				if all>0  then
					loaded = 0 
					set_progress_txt(string.format("%s %s/%s","开始从网络加载资源包请不要关闭程序。",loaded,all))
					Loader:get_resource(reqs)
					Loader:setOnProgressFn(onProgress)
				else
					on_all_update_res_comp()
				end
			else
			 	enterGame()
			end
		else
			enterGame()
		end
	end
end

local function check_res()

	if(Application.platform==RuntimePlatform.OSXEditor) then
		enterGame()
	elseif(Application.platform==RuntimePlatform.WindowsEditor) then
		enterGame()
	else
		enterGame()
		--[[
		 local url=string.format(resourceURL,tonumber(ResVersion),WWW.EscapeURL(Application.platform:ToString()),"0.2")
		 local req=Request(url)
		 req.onCompleteFn=on_update_res_comp
         req.assetType="System.String"
		 local function onErr( req )
		 	print("check_res on erro")
		 	enterGame()
		 end
		 print("begin check_res "..url)
		 req.onEndFn=onErr
	     Loader:get_resource(req,false)
	     ]]
	end
end

local function check_version()
	local function onURLComp(req )	
		print(req.data[1] )
		ResVersion=req.data[1] 
		check_res() 
	end
	local function onURLErComp(req )   check_res() end
	
	local verPath=CUtils.GetFileFullPath(VERSION_FILE_NAME);
	 print("check_version . verPath"..verPath)
	local req=Request(verPath)
    req.assetType ="System.String"
	req.onCompleteFn=onURLComp
	req.onEndFn=onURLErComp
  	--print("request create "..tostring(req))
  	--print(Loader)
    Loader:get_resource(req,false)
    --print("check_version . Loader:get_resource called  "..verPath)
end

local function load_frist()

    Loader:refresh_assetbundle_manifest()
	local function onload_comp(r)
		local www=r.data
		 print(r.url.." loaded ")
		--print(www)
        local fristView = LuaHelper.Instantiate(r.data)
        _progressbar_txt = LuaHelper.GetComponentInChildren(fristView,"UnityEngine.UI.Text")
        -- _progressbar_txt.text="check resource  ..."
        r.assetBundle:Unload(false)
        --fristView.name = "Frist"
        UnityEngine.GameObject.Destroy(fristView)
        check_version()        
    end

	Application.targetFrameRate=60
   
--    check_version()
	 local url = CUtils.GetAssetFullPath(FRIST_VIEW)
	 print("begin"..url)
	 Loader:get_resource(url,onload_comp,false)
end


load_frist()
