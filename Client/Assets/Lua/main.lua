------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
require("core.unity3d")
require("core.loader")
json=require("lib/json")
ResVersion = 0
-- luanet = _G
-- toluacs = _G
local resourceURL ="http://192.168.18.152:8345/api" --http://121.199.51.39:8080/client_update?v_id=%s&platform=%s&code=%s";

local progressBarTxt;
local update_id="";
local FRIST_VIEW = "frist.u3d";
local VIDEO_NAME = "Loading.mp4";
local VERSION_FILE_NAME = "Ver.t";

local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application
local GameObject= UnityEngine.GameObject
local PlayerPrefs = UnityEngine.PlayerPrefs
local Request=LRequest --luanet.import_type("LRequest")

local CUtils=CUtils
local LuaHelper=LuaHelper
local FilLoadereHelper=FileHelper -- luanet.import_type("FileHelper")
local Localization = Localization
local delay = delay
local Loader = Loader

--local fristView

local function languageInit()
	local lan=PlayerPrefs.GetString("Language","")
	-- if lan=="" then lan=Application.systemLanguage:ToString() end
	Localization.language="Chinese" --"Chinese"
	print(Application.systemLanguage.."current language is "..Localization.language)
end 

local function enterGame()
     languageInit()

	require("begin")
--	if fristView then LuaHelper.Destroy(fristView) end
--	fristView = nil 

--	local function dsLog() 
--		print("dsLog")
--		local logo = LuaHelper.Find("Logo")
--		if logo then  LuaHelper.Destroy(logo) end 
--	end
--	delay(dsLog,1,nil)
end

local function onProgress(loader,arg)
	-- body
end

local function onUpdateItemComp(req)
	local bytes=req.data.bytes
	if(bytes~=nil) then
		FileHelper.UnZipFile(bytes,Application.persistentDataPath);
	end
end

local function onAllUpdateResComp(loader)
	--SetProgressTxt("wait start...");
    loader:setOnAllCompelteFn(nil)
	loader:setOnProgressFn(nil)
	seveVersion()
--	enterGame()
end

local function seveVersion()
	FileHelper.PersistentUTF8File(update_id,VERSION_FILE_NAME)	
end

local function  onUpdateResComp(req)
    local www=req.data;
	if(www) then
		local txt=www[0]
        local res = json:decode(txt)
		if res["error"] then
			enterGame()
		elseif res["update_url"] then
			local upURL=res["update_url"]
			local reqs={}
			local len=#upURL
			for i=1,len do
				table.insert(reqs,{upURL[i],onUpdateItemComp})
			end
			Loader:getResource(reqs)
			Loader:setOnAllCompelteFn(onAllUpdateResComp)
			Loader:setOnProgressFn(onProgress)
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
		 local url=string.format(resourceURL,tostring(ResVersion),Application.platform,"0.2.0");
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
