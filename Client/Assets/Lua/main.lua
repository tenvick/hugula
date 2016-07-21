 ------------------------------------------------
--  Copyright © 2015-2016   Hugula: Arpg game Engine
--  检查资源
--  author pu
------------------------------------------------
require("core.loader")
json = require "lib.json"

local Hugula = Hugula
local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application
local WWW = UnityEngine.WWW
local GameObject = UnityEngine.GameObject
local Request=LRequest 

local CodeVersion = Hugula.CodeVersion
local CUtils= Hugula.Utils.CUtils
local LuaHelper=Hugula.Utils.LuaHelper
local FileHelper=Hugula.Utils.FileHelper
local PLua = Hugula.PLua
local Download = Hugula.Update.Download 
local Loader = Loader

ResVersion = {code=1,crc32=0}

local _progressbar_txt;
local FRIST_VIEW = "Logo"
local VERSION_FILE_NAME = "crc32_ver.u3d"
local VERSION_TEMP_FILE_NAME = "crc32_ver.tmp"
local UPDATED_LIST_NAME = "crc32_file.u3d"
local UPDATED_TEMP_LIST_NAME = "crc32_file.tmp"
local DOWANLOAD_TEMP_FILE = "downloaded.tmp"

local host = {"http://192.168.100.14/"..CUtils.GetAssetPath("").."/","http://192.168.100.114/"..CUtils.GetAssetPath("").."/"} --更新列表

--local fristView
local all,loaded = 0,0
local local_file,server_file,loaded_file
local local_version

local function set_progress_txt(text)
 	if _progressbar_txt then _progressbar_txt.text = text end
end

local function enterGame(need_reload)

	local function to_begin( ... )
		require("begin")
	end

	local function load_manifest( ... )
		set_progress_txt("进入游戏。")
	 	Loader:refresh_assetbundle_manifest(to_begin)
	end

	if need_reload == true then
		set_progress_txt("刷新脚本。")
		PLua.instance:LoadBundle(load_manifest)
	else
		load_manifest()
	end
end

local function save_loaded_file(loaded_list)
	local context = json:encode(loaded_list)
	local old_list_context = FileHelper.SavePersistentFile(context,DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
end

local function save_loaded_file_one(url )
	local key = CUtils.GetKeyURLFileName(url)
	key = "m_"..key
	local crc = server_file[key]
	loaded_file[key] = crc
	save_loaded_file(loaded_file)
end

local function one_file_dow(url,bol)
	-- print(url," is down ",bol)
	if bol == false then
		print(url," download error ")
	else
		loaded = loaded + 1
		set_progress_txt(string.format("网络资源加载中(消耗流量) %d/%d 。",loaded,all))
		save_loaded_file_one(url)
	end
end

local function all_file_down(isdown)
	-- print("all file is down")

	FileHelper.DeletePersistentFile(CUtils.GetFileName(UPDATED_LIST_NAME)) --删除旧文件
	FileHelper.DeletePersistentFile(CUtils.GetFileName(VERSION_FILE_NAME)) --删除旧文件
	print("更新文件列表！")
	FileHelper.ChangePersistentFileName(CUtils.GetFileName(UPDATED_TEMP_LIST_NAME),CUtils.GetFileName(UPDATED_LIST_NAME))
	print("更新版本号！")
	FileHelper.ChangePersistentFileName(CUtils.GetFileName(VERSION_TEMP_FILE_NAME),CUtils.GetFileName(VERSION_FILE_NAME))
	FileHelper.DeletePersistentFile(DOWANLOAD_TEMP_FILE)--删除零时文件
	set_progress_txt("更新完毕，进入游戏！")
	enterGame(true)
	print("all_file_down")
	Download.Dispose()
end 

local function load_update_files(urls)
	local download = Download.instance
	set_progress_txt("开始从服务器加载新的资源。")
	loaded_file = {}
	save_loaded_file(loaded_file) 
	download:Init(host,2,one_file_dow,all_file_down)
	local file
	for k,v in pairs(urls) do
		file = v[1].."?"..v[2]
		print(file)
		download:Load(file,v[1])
	end
end


local function load_server_file_list() --版本差异化对比

	local function on_server_comp(req)
		set_progress_txt("校验文件对比中。")
		local bytes = req.data
	 	FileHelper.SavePersistentFile(bytes,CUtils.GetFileName(UPDATED_TEMP_LIST_NAME)) --保存server端临时文件
	 	local ab = LuaHelper.LoadFromMemory(bytes)
	 	local text_asset = ab:LoadAsset(req.assetName)
		PLua.instance:SetRequire("file1",text_asset)
		-- Loader:clear(req.key)
		server_file=require("file1")
		local old_list_context = FileHelper.ReadPersistentFile(DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
		local old_list = {}
		if old_list_context ~= nil then
			old_list = json:decode(old_list_context)
		end

		local item_url,crc_path
		local urls = {}
		for k,v in pairs(server_file) do 
			if v~=local_file[k]  then
				item_url =string.sub(k,3)..".u3d"
				local crc = old_list[k] --FileHelper.ComputeCrc32(crc_path) -- this is expensive
				if crc~=v then
					table.insert(urls,{item_url,v})
				end
			end
		end
		all = #urls
		print("need update file count ",all)
		if all>0 then
			load_update_files(urls)
		else
			enterGame()
		end
	end

	local function on_server_err(req) 
		enterGame()	
	end

	local function load_server( ... )
		set_progress_txt("加载服务器校验文件。")
		Loader:get_resource(UPDATED_LIST_NAME,nil,"System.Byte[]",on_server_comp,on_server_err,nil,host)
	end
	

	local function on_local_comp(req) 
		local bytes = req.data.bytes
		PLua.instance:SetRequire("file",req.data)
		Loader:clear(req.key)
		local_file = require("file")
		load_server()
	end

	local function on_local_err(req)
	  enterGame() 
	end

	set_progress_txt("读取本地校验文件。")
	Loader:get_resource(UPDATED_LIST_NAME,nil,"UnityEngine.TextAsset",on_local_comp,on_local_err)
end


local function load_server_verion() --加载服务器版本号

	 local function on_err( req )
	 	print("check_res on erro"..req.key,req.udKey,req.url,req.assetName,req.assetBundleName)
	 	enterGame()
	 end

	 local function on_comp( req )
	 	-- print(req.url,"is onComplete")
	 	local ver_str = req.data[1]
		print("server var ",ver_str)
	 	local server_ver = json:decode(ver_str)
	 	FileHelper.SavePersistentFile(ver_str,CUtils.GetFileName(VERSION_TEMP_FILE_NAME)) --临时文件

	 	ResVersion = server_ver

	 	if CodeVersion.CODE_VERSION ~= server_ver.code then --如果本地代码版本号不一致
  			set_progress_txt("请更新app版本！")
	 	elseif server_ver.crc32 ~= local_version.crc32 then
	 		load_server_file_list()
	 	else
	 		enterGame()
	 	end
	 end

	set_progress_txt("加载服务器信息。")

    Loader:get_resource(VERSION_FILE_NAME,nil,"System.String",on_comp,on_err,nil,host)
end

local function load_local_version() --加载本地版本号

	local function on_verjson_comp(req )	
		local ver_str = req.data[1] 
		print("local ver ",ver_str)
		local ver_json = json:decode(ver_str)
		local_version = ver_json
		ResVersion = ver_json
		if Application.platform == RuntimePlatform.OSXEditor or Application.platform == RuntimePlatform.WindowsEditor or Application.platform == RuntimePlatform.WindowsPlayer then --for test
			enterGame()
		else
			load_server_verion() 
		end
	end

	local function on_verjson_err(req )   
		enterGame()
	end

  	set_progress_txt("读取本地版本信息。")
    Loader:get_resource(VERSION_FILE_NAME,nil,"System.String",on_verjson_comp,on_verjson_err)
end

local function init_frist()

	local ui_logo = LuaHelper.Find(FRIST_VIEW)
	_progressbar_txt = LuaHelper.GetComponentInChildren(ui_logo,"UnityEngine.UI.Text")	

	set_progress_txt("初始化...")

	load_local_version()

end

init_frist()
