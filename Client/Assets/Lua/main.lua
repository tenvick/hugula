 ------------------------------------------------
--  Copyright © 2015-2016   Hugula: Arpg game Engine
--  检查资源
--  author pu
------------------------------------------------
require("core.loader")
require_bytes = SluaCMD.require_bytes
json = require "lib.json"

local Hugula = Hugula
local RuntimePlatform= UnityEngine.RuntimePlatform
local Application= UnityEngine.Application
local WWW = UnityEngine.WWW
local GameObject = UnityEngine.GameObject
local LRequestPool = Hugula.Loader.LRequestPool --内存池

local CODE_VERSION = Hugula.CodeVersion.CODE_VERSION
local CUtils= Hugula.Utils.CUtils
local LuaHelper=Hugula.Utils.LuaHelper
local FileHelper=Hugula.Utils.FileHelper
local Common = Hugula.Utils.Common
local PLua = Hugula.PLua
local Download = Hugula.Update.Download 
local CrcCheck = Hugula.Update.CrcCheck
local UriGroup = Hugula.Loader.UriGroup
local Loader = Loader
local require_bytes = require_bytes
local delay = PLua.Delay
local stop_delay = PLua.StopDelay

ResVersion = {code=1,crc32=0,time=1}

local _progressbar_txt;
local FRIST_VIEW = "Logo"
local VERSION_FILE_NAME = Common.CRC32_VER_FILENAME 
local VERSION_TEMP_FILE_NAME = CUtils.GetAssetName(VERSION_FILE_NAME)..".tmp"
local UPDATED_LIST_NAME = Common.CRC32_FILELIST_NAME 
local UPDATED_TEMP_LIST_NAME =  CUtils.GetAssetName(UPDATED_LIST_NAME)..".tmp"
local DOWANLOAD_TEMP_FILE = "downloaded.tmp"
local folder = CUtils.GetAssetPath("")
local update_list_crc_key = CUtils.GetRightFileName(UPDATED_LIST_NAME)
local http_url = ""
http_url = "http://192.168.103.200:8055/"
local ver_host = {http_url..CUtils.platform.."/v"..CODE_VERSION.."/"}--,"http://192.168.100.114/"..CUtils.GetAssetPath("").."/"} --更新列表

--local fristView
local all,loaded = 0,0
local local_file,server_file = {},{}
local loaded_file
local local_version,server_ver
local loaded_err = false
--- global
CRC_FILELIST = {}

function CRC_FILELIST.get_item(key)
	local val = nil
	for k,v in pairs(CRC_FILELIST) do
		if type(v) == "table" then
			val = v[key] 
			if val then return val end
		end
	end
	return val
end

local function print_time(times)
	print(os.date("%c",times))
end

local function set_resversion(ver)
	if ver then
		for k,v in pairs(ver) do
			ResVersion[k] = v
		end
	end
end

local function get_update_uri_group(hosts, on_www_comp,on_crc_check)
	local group = UriGroup()
	for k,v in pairs(hosts) do
		if on_www_comp == nil then
			group:Add(v)
		else
			group:Add(v,on_www_comp,on_crc_check)
		end
	end
	return group
end

local function set_progress_txt(text)
 	if _progressbar_txt then _progressbar_txt.text = text end
 	print(text)
end

local function add_file(crc_tb )
	local item
	for k,v in pairs(crc_tb) do
		item = CRC_FILELIST[k]
		if item == nil  then item = {} CRC_FILELIST[k] = item end
		for k1,v1 in pairs(v) do	
			item[k1] = v1
			-- print("add_file",k1,v1)
		end		
	end
end

local function add_crc(crc_tb)
	for k,v in pairs(crc_tb) do
		for k1,v1 in pairs(v) do
			CrcCheck.Add(k1,v1)
			-- print(k1,v1)
		end
	end
end

--检测扩展文件夹
local function check_extends_folder(path)
	local i = string.find(path,"/")
	if i ~= nil then
		return FileHelper.PersistentFileExists(path)
	end	
	return true
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

	local  function refresh_lua( ... )
		PLua.instance:LoadBundle(load_manifest)
	end

	cdn_hosts = ResVersion.cdn_host -- or host
	delay(refresh_lua,0.2)
	
end

local function save_loaded_file(loaded_list)
	local context = json:encode(loaded_list)
	local old_list_context = FileHelper.SavePersistentFile(context,DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
end

local function save_loaded_file_one(url )
	local key = CUtils.GetAssetBundleName(url)
	local crc = server_file[key]
	loaded_file[key] = crc
	save_loaded_file(loaded_file)
end

local function one_file_dow(url,bol,key)
	-- print(url," is down ",bol)
	if bol == false then
		loaded_err = true
		print(url," download error ",key)
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
		set_resversion(server_ver)
		FileHelper.DeletePersistentFile(CUtils.GetRightFileName(UPDATED_LIST_NAME)) --删除旧文件
		FileHelper.DeletePersistentFile(CUtils.GetRightFileName(VERSION_FILE_NAME)) --删除旧文件
		print("更新文件列表！")
		FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME),CUtils.GetRightFileName(UPDATED_LIST_NAME))
		print("更新版本号！")
		FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME),CUtils.GetRightFileName(VERSION_FILE_NAME))
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
	download:Init(server_ver.cdn_host,2,one_file_dow,all_file_down)
	local file,savefile,v1
	for k,v in pairs(urls) do
		v1 = v[1]
		file = CUtils.InsertAssetBundleName(v1,"_"..v[2]) --拼接
		savefile =  v1
		if v1 == folder then --如果是目录
			savefile = folder
			file = v1.."_"..v[2].."."..Common.ASSETBUNDLE_SUFFIX
			print(" load folder "..file)
		end
		print("begin load "..file.." save name "..savefile)
		download:Load(file,savefile,v1)
	end
end


local function load_server_file_list() --版本差异化对比

	local function on_www_comp( req,bytes )
		print("on www comp "..req.assetName)
	 	FileHelper.SavePersistentFile(bytes,CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME)) --保存server端临时文件
	end

	local function on_server_comp(req)
		set_progress_txt("校验列表对比中。")
		local text_asset = req.data
		server_file = require_bytes(text_asset)
		-- print(text_asset)
	 	Loader:clear(req.key)
		add_crc(server_file) --加入验证列表
		local old_list_context = FileHelper.ReadPersistentFile(DOWANLOAD_TEMP_FILE) --读取上次加载未完成列表
		local old_list = {}
		if old_list_context ~= nil then
			old_list = json:decode(old_list_context)
		end

		local urls = {}
		for k,v in pairs(server_file) do 
			for k1,v1 in pairs(v) do
				-- print(k1,v1,CRC_FILELIST.get_item(k1),check_extends_folder)
				if v1~=CRC_FILELIST.get_item(k1) and check_extends_folder(k1) then
					local crc = old_list[k1] --FileHelper.ComputeCrc32(crc_path) -- this is expensive
					if crc~=v then
						table.insert(urls,{k1,v1})
					end
				end
			end
		end

		add_file(server_file)

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
		set_progress_txt("加载服务器校验列表。")
		local crc = tostring(server_ver.crc32)
		local asset_name = CUtils.GetAssetName(UPDATED_LIST_NAME)
		local assetbundle_name = CUtils.GetRightFileName(asset_name)
		local file_name = assetbundle_name.."_"..crc..".u3d"
		print("load web server crc "..file_name)
		local req = LRequestPool.Get()
		req.relativeUrl = file_name
		req.onCompleteFn = on_server_comp
		req.onEndFn = on_server_err
		req.assetName = asset_name--LuaHelper.GetClassType("System.Byte[]")
		req.uris = get_update_uri_group(server_ver.cdn_host,on_www_comp,nil)
		Loader:get_resource(req)
	end
 
 	load_server()
end

local function load_server_verion() --加载服务器版本号

	 local function on_err( req )
	 	print("load_server_ver on erro"..req.key,req.udKey,req.url,req.assetName,req.assetBundleName)
	 	enterGame()
	 end

	 local function on_comp( req )
	 	-- print(req.url,"is onComplete",req.data.Length)
	 	local ver_str = req.data
		print("server var ",ver_str)
	 	server_ver = json:decode(ver_str)
	 	print_time(server_ver.time)

	 	FileHelper.SavePersistentFile(ver_str,CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME)) --临时文件

	 	if server_ver.time <=  local_version.time then --如果发布时间不对。
  			set_progress_txt("你的版本不需要更新！")
	 		enterGame()
	 	elseif CODE_VERSION ~= server_ver.code then --如果本地代码版本号不一致
  			set_progress_txt("请更新app版本！")
			 Application.OpenURL(server_ver.update_url)
	 	elseif server_ver.crc32 ~= local_version.crc32 then
	 		load_server_file_list()
	 	else
	 		enterGame()
	 	end
	 end

	set_progress_txt("加载服务器信息。")
    Loader:get_resource(VERSION_FILE_NAME,nil,String,on_comp,on_err,nil,get_update_uri_group(ver_host))
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
		local_file = require_bytes(text_asset)
		print(" on_persistent_file_list",text_asset)
		add_crc(local_file)
		add_file(local_file)
		Loader:clear(req.key)
		step.next_step()
	end

	step.on_persistent_error=function( req )
		print("本地没有校验文件"..req.url)
		step.next_step()
	end

	step.load_persistent_file=function ( ... )
		if CrcCheck.ContainsKey(update_list_crc_key) then
			set_progress_txt("读取本地校验文件。")
			local crc = CrcCheck.GetCrc(update_list_crc_key)
			print("persistent update file list"..tostring(crc))
		
			local group = UriGroup()
			group:Add(CUtils.GetRealPersistentDataPath(),true)
			Loader:get_resource(UPDATED_LIST_NAME,nil,UnityEngine.TextAsset,step.on_persistent_comp,step.on_persistent_error,nil,group)
		else
			print("本地没有校验文件")
			step.next_step()
		end
	end

    --
    local step1 = {}
    step1.on_streaming_comp = function( req )
    	local text_asset = req.data
    	print("on_streaming_filelist",text_asset)
		local s_crc32_file = require_bytes(text_asset)
		add_file(s_crc32_file)
		Loader:clear(req.key)
		step.load_persistent_file()
    end
    step1.on_streaming_error = function( req )
    	print("never happen!!! load error "..req.url)
    	step.load_persistent_file()
    end
    step1.load_streaming_file = function( ... )
    	local group = UriGroup()
		group:Add(CUtils.GetRealStreamingAssetsPath())
		Loader:get_resource(UPDATED_LIST_NAME,nil,UnityEngine.TextAsset,step1.on_streaming_comp,step1.on_streaming_error,nil,group)
    end

    step1.load_streaming_file()

end

local function compare_local_version() --对比本地版本号
	local step = {}
	step.key = CUtils.GetRightFileName(UPDATED_LIST_NAME)
	step.on_persistent_comp=function ( req )
		local ver_str = req.data
		print("local persistent ver ",ver_str)
		local ver_json = json:decode(ver_str)
		print_time(ver_json.time)
		step.persistent_version = ver_json
		CrcCheck.Add(update_list_crc_key,tonumber(ver_json.crc32))--本地验证列表的crc验证
		-- print("persistent ver crc",update_list_crc_key,"=",ver_json.crc32)
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
			set_resversion(local_version)
			load_local_file_list()
		elseif step.persistent_version ~= nil and step.streaming_version ~= nil then
			if step.persistent_version.time >= step.streaming_version.time then
				print("直接进入。")
				local_version = step.persistent_version
				set_resversion(local_version)
				load_local_file_list()			
			else
				set_progress_txt("清理旧的缓存。")
				print("清理旧的缓存。"..CUtils.GetRealPersistentDataPath())
				FileHelper.DeletePersistentDirectory(nil)
				CrcCheck.Clear() --清除校验列表
				package.loaded[step.key] = nil 
				package.preload[step.key] = nil
				print("delete lua"..step.key)
				local_version = step.streaming_version --当前版本
				set_resversion(local_version)
				load_local_file_list()
			end
		end
	end

	step.on_streaming_comp=function ( req )
		local ver_str = req.data 
		print("local streaming ver ",ver_str)
		local ver_json = json:decode(ver_str)
		print_time(ver_json.time)
		step.streaming_version = ver_json
		step.compare()
	end

	step.on_streaming_error=function ( req ) --never happen
		print("local streaming ver err ")
		--temp
		enterGame()
	end

	step.load_persistent=function(  )
		print("加载本地缓存版本信息。")
		local group = UriGroup()
		group:Add(CUtils.GetRealPersistentDataPath())
  		Loader:get_resource(VERSION_FILE_NAME,nil,String,step.on_persistent_comp,step.on_persistent_error,nil,group)
	end

	step.load_streaming=function(  )
		print("加载本地版本信息。") --
		local group = UriGroup()
		group:Add(CUtils.GetRealStreamingAssetsPath())
    	Loader:get_resource(VERSION_FILE_NAME,nil,String,step.on_streaming_comp,step.on_streaming_error,nil,group)
	end

  	set_progress_txt("对比本地版本信息。")
	step.load_streaming()
	step.load_persistent()

end

local function init_frist()

	print(Hugula.Utils.CUtils.GetRealPersistentDataPath())
	print(Hugula.Utils.CUtils.GetRealStreamingAssetsPath())
	print(UnityEngine.Application.version,Application.bundleIdentifier)
	local ui_logo = LuaHelper.Find(FRIST_VIEW)
	_progressbar_txt = LuaHelper.GetComponentInChildren(ui_logo,"UnityEngine.UI.Text")	
	set_progress_txt("初始化...")

	compare_local_version()

end

init_frist()
