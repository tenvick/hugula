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

local progressBarTxt;
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
local function languageInit()
	local lan=PlayerPrefs.GetString("Language","")
	-- if lan=="" then lan=Application.systemLanguage:ToString() end
	Localization.language="chinese" --"Chinese"
	print(Application.systemLanguage.."current language is "..Localization.language)
end 

local function enterGame()
     languageInit()
 	 require("begin")
end

local function setProgressTxt(text)
 	if progressBarTxt then progressBarTxt.text = text end
end

local function onProgress(loader,arg)
	-- body
end

local function addLoadedFile(key)
	local da = FileHelper.ReadUTF8File(UPDATED_LIST_TMEP)
	if da==nil then da = "{}" end
	local jsda=json:decode(da)
	jsda[key]=true
	da = json:encode(jsda)
	FileHelper.PersistentUTF8File(da,UPDATED_LIST_TMEP)
end

local function seveVersion()
	FileHelper.PersistentUTF8File(update_id,VERSION_FILE_NAME)	
end

local function onAllUpdateResComp()
	setProgressTxt("wait start...")
    Loader:setOnAllCompelteFn(nil)
	Loader:setOnProgressFn(nil)
	seveVersion()
	FileHelper.DeleteFile(UPDATED_LIST_TMEP)
	-- enterGame(true)
	PLua:LoadBundle(enterGame) --reload LuaBind
end

local function onUpdateItemComp(req)
	local bytes=req.data --get_data().bytes
	if(bytes~=nil) then
		FileHelper.UnZipFile(bytes,Application.persistentDataPath)
		addLoadedFile(req.key)
	end
	loaded = loaded+1
	setProgressTxt(string.format("%s %s/%s","开始从网络加载资源包请不要关闭程序。",loaded,all))

	if all<=loaded then onAllUpdateResComp() end
end

local function  onUpdateResComp(req)
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
						table.insert(reqs,{itemURl,onUpdateItemComp,assetType="System.Byte[]"})
					end
				end
				all=#reqs
				if all>0  then
					loaded = 0 
					setProgressTxt(string.format("%s %s/%s","开始从网络加载资源包请不要关闭程序。",loaded,all))
					Loader:getResource(reqs)
					Loader:setOnProgressFn(onProgress)
				else
					onAllUpdateResComp()
				end
			else
			 	enterGame()
			end
		else
			enterGame()
		end
	end
end

local function checkRes()

	if(Application.platform==RuntimePlatform.OSXEditor) then
		enterGame()
	elseif(Application.platform==RuntimePlatform.WindowsEditor) then
		enterGame()
	else
		enterGame()
		--[[
		 local url=string.format(resourceURL,tonumber(ResVersion),WWW.EscapeURL(Application.platform:ToString()),"0.2")
		 local req=Request(url)
		 req.onCompleteFn=onUpdateResComp
         req.assetType="System.String"
		 local function onErr( req )
		 	print("checkRes on erro")
		 	enterGame()
		 end
		 print("begin checkRes "..url)
		 req.onEndFn=onErr
	     Loader:getResource(req,false)
	     ]]
	end
end

local function checkVerion()
	local function onURLComp(req )	ResVersion=req.data[0] checkRes() end
	local function onURLErComp(req )   checkRes() end
	
	local verPath=CUtils.GetFileFullPath(VERSION_FILE_NAME);
	 print("checkVerion . verPath"..verPath)
	local req=Request(verPath)
    req.assetType ="System.String"
	req.onCompleteFn=onURLComp
	req.onEndFn=onURLErComp
  	--print("request create "..tostring(req))
  	--print(Loader)
    Loader:getResource(req,false)
    --print("checkVerion . Loader:getResource called  "..verPath)
end

local function loadFrist()

    Loader:RefreshAssetBundleManifest()
	local function onLoadComp(r)
		local www=r.data
		 print(r.url.." loaded ")
		--print(www)
        local fristView = LuaHelper.Instantiate(r.data)
        progressBarTxt = LuaHelper.GetComponentInChildren(fristView,"UnityEngine.UI.Text")
        -- progressBarTxt.text="check resource  ..."
        r.assetBundle:Unload(false)
        fristView.name = "Frist"
        checkVerion()
    end

	Application.targetFrameRate=30
   
--    checkVerion()
	 local url = CUtils.GetAssetFullPath(FRIST_VIEW)
	 print("begin"..url)
	 Loader:getResource(url,onLoadComp,false)
end


loadFrist()
