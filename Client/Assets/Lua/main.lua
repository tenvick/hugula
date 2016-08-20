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
local LRequestPool = Hugula.Loader.LRequestPool --内存池

local CodeVersion = Hugula.CodeVersion
local CUtils= Hugula.Utils.CUtils
local LuaHelper=Hugula.Utils.LuaHelper
local FileHelper=Hugula.Utils.FileHelper
local Common = Hugula.Utils.Common
local PLua = Hugula.PLua
local Download = Hugula.Update.Download 
local CrcCheck = Hugula.Update.CrcCheck
local Loader = Loader

ResVersion = {code=1,crc32=0}

local _progressbar_txt;
local FRIST_VIEW = "Logo"
local VERSION_FILE_NAME = Common.CRC32_VER_FILENAME 
local VERSION_TEMP_FILE_NAME = CUtils.GetKeyURLFileName(VERSION_FILE_NAME)..".tmp"
local UPDATED_LIST_NAME = Common.CRC32_FILELIST_NAME 
local UPDATED_TEMP_LIST_NAME =  CUtils.GetKeyURLFileName(UPDATED_LIST_NAME)..".tmp"
local DOWANLOAD_TEMP_FILE = "downloaded.tmp"
local folder = CUtils.GetAssetPath("")
local version_crc_key = CUtils.GetFileName(CUtils.GetKeyURLFileName(VERSION_FILE_NAME))
local update_list_crc_key = CUtils.GetFileName(CUtils.GetKeyURLFileName(UPDATED_LIST_NAME))

local host = {"https://github.com/tenvick/hugula_resource/"..folder.."/"}--,"http://192.168.100.114/"..CUtils.GetAssetPath("").."/"} --更新列表

--local fristView
local all,loaded = 0,0
local local_file,server_file = {},{}
local loaded_file
local local_version,server_ver
local loaded_err = false

local function print_time(times)
	print(os.date("%c",times))
end

local function set_progress_txt(text)
 	if _progressbar_txt then _progressbar_txt.text = text end
 	print(text)
end

local function add_crc(crc_tb)
	for k,v in pairs(crc_tb) do
		CrcCheck.Add(k,v)
	end
end

local function enterGame()

	local function to_begin( ... )
		require("begin")
	end

	local function load_manifest( ... )
		set_progress_txt("进入游戏。")
	 	Loader:refresh_assetbundle_manifest(to_begin)
	end

	set_progress_txt("刷新脚本。")
	PLua.instance:LoadBundle(load_manifest)
	
end

local function save_loaded_file(loaded_list)
	local context = json:encode(loaded_list)
	local old_list_context = FileHelper.SavePersistentFile(context,DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
end

local function save_loaded_file_one(url )
	local key = CUtils.GetKeyURLFileName(url)
	local crc = server_file[key]
	loaded_file[key] = crc
	save_loaded_file(loaded_file)
end

local function one_file_dow(url,bol)
	-- print(url," is down ",bol)
	if bol == false then
		loaded_err = true
		print(url," download error ")
	else
		loaded = loaded + 1
		set_progress_txt(string.format("网络资源加载中(消耗流量) %d/%d 。",loaded,all))
		save_loaded_file_one(url)
	end
end

local function all_file_down(isdown)
	-- print("all file is down")
	if loaded_err then
		set_progress_txt("文件下载失败请重启游戏。")
	else
		ResVersion = server_ver
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
	end

	Download.Dispose()
end 

local function load_update_files(urls)
	local download = Download.instance
	set_progress_txt("开始从服务器加载新的资源。")
	loaded_file = {}
	save_loaded_file(loaded_file) 
	download:Init(host,2,one_file_dow,all_file_down)
	local file,savefile,v1
	for k,v in pairs(urls) do
		v1 = v[1]
		file = v1.."_"..v[2]..".u3d"
		savefile =  v1..".u3d"
		if v1 == folder then
			savefile = v1
			print(" save file "..savefile)
		end
		download:Load(file,savefile)
	end
end


local function load_server_file_list() --版本差异化对比

	local function on_server_comp(req)
		set_progress_txt("校验文件对比中。")
		local bytes = req.data
	 	FileHelper.SavePersistentFile(bytes,CUtils.GetFileName(UPDATED_TEMP_LIST_NAME)) --保存server端临时文件
	 	local ab = LuaHelper.LoadFromMemory(bytes)
	 	local text_asset = ab:LoadAllAssets(UnityEngine.TextAsset)[1]
		PLua.instance:SetRequire("_file1",text_asset)
		ab:Unload(true) --释放ab
		-- Loader:clear(req.key)
		server_file=require("_file1")
		add_crc(server_file) --加入验证列表
		local old_list_context = FileHelper.ReadPersistentFile(DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
		local old_list = {}
		if old_list_context ~= nil then
			old_list = json:decode(old_list_context)
		end

		local urls = {}
		for k,v in pairs(server_file) do 
			if v~=local_file[k]  then
				local crc = old_list[k] --FileHelper.ComputeCrc32(crc_path) -- this is expensive
				if crc~=v then
					table.insert(urls,{k,v})
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
		print("on_server_err :",req.url)
		enterGame()	
	end

	local function load_server( ... )
		set_progress_txt("加载服务器校验文件。")
		local crc = tostring(server_ver.crc32)
		local asset_name = CUtils.GetFileName(CUtils.GetKeyURLFileName(UPDATED_LIST_NAME))
		local file_name = asset_name.."_"..crc..".u3d"
		print("load web server crc"..file_name)
		local req = LRequestPool.Get()
		req.relativeUrl = file_name
		req.onCompleteFn = on_server_comp
		req.onEndFn = on_server_err
		req.assetType = "System.Byte[]"
		req.uris = host
		Loader:get_resource(req)
	end
 
 	load_server()
end

local function load_server_verion() --加载服务器版本号

	 local function on_err( req )
	 	print("check_res on erro"..req.key,req.udKey,req.url,req.assetName,req.assetBundleName)
	 	enterGame()
	 end

	 local function on_comp( req )
	 	-- print(req.url,"is onComplete",req.data.Length)
	 	local ver_str = req.data[1]
		print("server var ",ver_str)
	 	server_ver = json:decode(ver_str)
	 	print_time(server_ver.time)

	 	FileHelper.SavePersistentFile(ver_str,CUtils.GetFileName(VERSION_TEMP_FILE_NAME)) --临时文件

	 	if server_ver.time <=  local_version.time then --如果发布时间不对。
  			set_progress_txt("你的版本不需要更新！")
	 		enterGame()
	 	elseif CodeVersion.CODE_VERSION < server_ver.code then --如果本地代码版本号不一致
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

local function load_local_file_list()

	local step = {}
	step.next_step=function( ... )
		if Application.platform == RuntimePlatform.OSXEditor or Application.platform == RuntimePlatform.WindowsEditor or Application.platform == RuntimePlatform.WindowsPlayer then --for test
			enterGame()
		else
			load_server_verion()
		end
	end

	step.on_persistent_comp=function( req )
		local text_asset = req.data
		PLua.instance:SetRequire("_crc32",text_asset)
		local_file = require("_crc32")
		print(" persistent crc32 file list")
		print(local_file)
		add_crc(local_file)
		Loader:clear(req.key)
		step.next_step()
	end

	step.on_persistent_error=function( req )
		print("本地没有校验文件"..req.url)
		step.next_step()
	end

	if CrcCheck.ContainsKey(update_list_crc_key) then
		set_progress_txt("读取本地校验文件。")
		local crc = CrcCheck.GetCrc(update_list_crc_key)
		print("persistent update file list"..tostring(crc))
		Loader:get_resource(UPDATED_LIST_NAME,nil,"UnityEngine.TextAsset",step.on_persistent_comp,step.on_persistent_error,nil,{CUtils.GetRealPersistentDataPath()})
	else
		print("本地没有校验文件")
		step.next_step()
	end
end

local function compare_local_version() --对比本地版本号
	local step = {}
	step.key = CUtils.GetKeyURLFileName(UPDATED_LIST_NAME)
	step.on_persistent_comp=function ( req )
		local ver_str = req.data[1] 
		print("local persistent ver ",ver_str)
		local ver_json = json.decode(ver_str)
		print_time(ver_json.time)
		step.persistent_version = ver_json
		CrcCheck.Add(update_list_crc_key,tonumber(ver_json.crc32))--本地验证列表的crc验证
		print("persistent ver crc",update_list_crc_key,"=",ver_json.crc32)
		step.compare()
	end

	step.on_persistent_error=function ( req )
		step.persistent_error = true
		print("local verion persistent erro ",req.key)
		step.compare()
	end

	step.compare=function(  )
		if step.persistent_error == true and  step.streaming_version ~= nil then
			print("没有缓存版本文件！")
			local_version = step.streaming_version
			ResVersion = local_version
			load_local_file_list()
		elseif step.persistent_version ~= nil and step.streaming_version ~= nil then
			if step.persistent_version.time >= step.streaming_version.time then
				print("直接进入。")
				local_version = step.persistent_version
				ResVersion = local_version
				load_local_file_list()			
			else
				set_progress_txt("清理旧的缓存。")
				print("清理旧的缓存。"..CUtils.GetRealPersistentDataPath())
				FileHelper.DeletePersistenDirectory()
				CrcCheck.Clear() --清除校验列表
				package.loaded[step.key] = nil 
				package.preload[step.key] = nil
				print("delete lua"..step.key)
				local_version = step.streaming_version --当前版本
				ResVersion = local_version
				load_local_file_list()
			end
		end
	end

	step.on_streaming_comp=function ( req )
		local ver_str = req.data[1] 
		print("local streaming ver ",ver_str)
		local ver_json = json:decode(ver_str)
		print_time(ver_json.time)
		step.streaming_version = ver_json
		step.load_persistent()
	end

	step.on_streaming_error=function ( req ) --never happen
		print("local streaming ver err ")
	end

	step.load_persistent=function(  )
		print("加载本地缓存版本信息。")
  		Loader:get_resource(VERSION_FILE_NAME,nil,"System.String",step.on_persistent_comp,step.on_persistent_error,nil,{CUtils.GetRealPersistentDataPath()})
	end

	step.load_streaming=function(  )
		print("加载本地版本信息。")
    	Loader:get_resource(VERSION_FILE_NAME,nil,"System.String",step.on_streaming_comp,step.on_streaming_error,nil,{CUtils.GetRealStreamingAssetsPath()})
	end

  	set_progress_txt("对比本地版本信息。")
	CrcCheck.Add(version_crc_key,0) --ver 不需要验证crc
	step.load_streaming()
end

local function init_frist()

	print(Hugula.Utils.CUtils.GetRealPersistentDataPath())
	print(Hugula.Utils.CUtils.GetRealStreamingAssetsPath())

	local ui_logo = LuaHelper.Find(FRIST_VIEW)
	_progressbar_txt = LuaHelper.GetComponentInChildren(ui_logo,"UnityEngine.UI.Text")	

	set_progress_txt("初始化...")

	compare_local_version()

end

init_frist()
