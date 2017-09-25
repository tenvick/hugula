------------------------------------------------
--  Copyright © 2015-2016   Hugula: Arpg game Engine
--  热更新
--  author pu
--	从Ver_android or Ver_ios 中的cdn_host下载资源包。
-- 	1热更新流程
--	.1 对比本地版本号 streaming 和 persistent 的版本文件。
--	.2 加载文件列表 分别加载streaming 和 persistent的文件列表
--	.3 加载版本文件 对比最新的本地版本号
--	.4 如果不一致下载更新列表。
--	.5 对比 本地文件列表 查找变化的文件。
--	.6 下载变化文件列表。
--	.7 下载完成进入游戏。
--
--	2注意事项
--	.3 BackGroundDownload 是网络加载模块，用于从网络加载URL并保存到persistent目录。
--	.4 ResVersion 版本信息，除了基本版本信息外，还可读取VerExtends_android or VerExtends_ios里面的配置字段，可以用于一些配置。
--	.5 cdn_hosts 用于从网络加载资源的url列表
------------------------------------------------
require("core.loader")
collectgarbage("setpause", 110)
collectgarbage("setstepmul", 300)

json = require "lib.json"
local Hugula = Hugula
local RuntimePlatform = UnityEngine.RuntimePlatform
local Application = UnityEngine.Application
local WWW = UnityEngine.WWW
local GameObject = UnityEngine.GameObject
local ManifestManager = Hugula.Loader.ManifestManager
local AssetBundle = UnityEngine.AssetBundle

local CodeVersion = Hugula.CodeVersion
local CODE_VERSION = CodeVersion.CODE_VERSION
local APP_VERSION = CodeVersion.APP_VERSION

local CUtils = Hugula.Utils.CUtils
local LuaHelper = Hugula.Utils.LuaHelper
local FileHelper = Hugula.Utils.FileHelper
local Common = Hugula.Utils.Common
local PLua = Hugula.PLua
local UriGroup = Hugula.Loader.UriGroup
local CRequest = Hugula.Loader.CRequest
local MessageBox = Hugula.UI.MessageBox
local Loader = Loader

local delay = PLua.Delay
local stop_delay = PLua.StopDelay
local Localization = Hugula.Localization --
local HugulaSetting = Hugula.HugulaSetting.instance
local BackGroundDownload = Hugula.Update.BackGroundDownload
ResVersion = {code = CODE_VERSION, crc32 = 0, time = os.time(), version = APP_VERSION}

local _progressbar_txt, _progressbar_slider
local FRIST_VIEW = "/Logo"
local VERSION_FILE_NAME = Common.CRC32_VER_FILENAME
local VERSION_TEMP_FILE_NAME = CUtils.GetAssetName(VERSION_FILE_NAME) .. ".tmp"
local UPDATED_LIST_NAME = Common.CRC32_FILELIST_NAME
local UPDATED_TEMP_LIST_NAME = CUtils.GetAssetName(UPDATED_LIST_NAME) .. ".tmp"
local DOWANLOAD_TEMP_FILE = "downloaded.tmp"
local update_list_crc_key = CUtils.GetRightFileName(UPDATED_LIST_NAME)
-- #HUGULA_RELASE symbols control
local http_ver_hosts = HugulaSetting.httpVerHost
--------------------------------ver url fromat--------------------------
function string.split(str, delimiter)
    if str == nil or str == "" or delimiter == nil then
        return nil
    end
    
    local results = {}
    for match in (str .. delimiter):gmatch("(.-)" .. delimiter) do
        table.insert(results, match)
    end
    return results
end

function lua_localization(key, ...)--本地化
    local val = Localization.Get(key)
    if ... then
        val = string.format(val, ...)
    end
    return val
end

local function dprint(...)
    if not CUtils.isRelease then
        print(...)
    end
end

function run_times(arg)
    CUtils.DebugCastTime(arg or debug.traceback())
end

----------------------------------------------------------
--local fristView
local local_file, server_file = {}, {}
local backgroud_loader
local local_version, server_ver
local main_update = {}
local MAX_STEP = 6
local DEBUG_UPDATE = false
local server_manifest
local json_pattern = "^{.*}$"
--- global-----------------------------------
cdn_hosts = {}
---------------------------local function ----------------
local function check_platform()
    local re = Application.platform == RuntimePlatform.OSXEditor or Application.platform == RuntimePlatform.WindowsEditor or Application.platform == RuntimePlatform.WindowsPlayer --for test
    return re
end

local function insert_assetBundle_name(assetbundleName, insert)
    local append = HugulaSetting.appendCrcToFile
    if append then
        return CUtils.InsertAssetBundleName(assetbundleName, insert)
    else
        return assetbundleName
    end
end


local function set_resversion(ver)
    if ver then
        for k, v in pairs(ver) do
            ResVersion[k] = v
        end
    end
end

local function set_resversion_var(ver)
    if ver then
        for k, v in pairs(ver) do
            if not (k == "code" or k == "crc32" or k == "version") then
                ResVersion[k] = v
            end
        end
    end
end

local function decode_ver_str(str)
    if string.match(str, json_pattern) then
        local ver = json:decode(str)
        return ver
    else
        return {code = CODE_VERSION, crc32 = 0, time = os.time(), version = APP_VERSION}
    end
end

local function get_update_uri_group(hosts, on_www_comp, on_crc_check)
    local group = UriGroup()
    for k, v in pairs(hosts) do
        if on_www_comp == nil then
            group:Add(v)
        else
            group:Add(v, on_www_comp, on_crc_check)
        end
    end
    return group
end

local function get_ver_url()
    local ver_url --版本地址
    local udid = UnityEngine.SystemInfo.deviceUniqueIdentifier --
    local hosts = string.split(http_ver_hosts, ",")
    local group = {}
    for k, v in pairs(hosts) do
        if string.match(v, "%?") then
            ver_url = string.format(v, APP_VERSION, udid, CUtils.platform, os.time())--
        else
            ver_url = string.format(v, CUtils.platform, "v" .. CODE_VERSION, CUtils.GetRightFileName(VERSION_FILE_NAME))
        end
        table.insert(group,ver_url)
        dprint(ver_url)
    end
    return group
end

local function set_progress_txt(text, step, per)
    if _progressbar_txt then _progressbar_txt.text = text end
    if _progressbar_slider and per and step then
        local p = (step + per) / MAX_STEP
        _progressbar_slider.value = p
    end
end

local function enter_game(manifest)
    local function to_begin(...)
        BackGroundDownload.instance.alwaysDownLoad = false --close always download
        BackGroundDownload.instance.loadingCount = 2
        if _progressbar_slider then _progressbar_slider.gameObject:SetActive(false) end
        if _progressbar_txt then _progressbar_txt.gameObject:SetActive(false) end
        require("begin")
    end
    
    local function load_manifest(...)
        set_progress_txt(lua_localization("main_enter_game"), 5, 1)--)进入游戏......"
        if manifest then --如果有更新需要刷新
            ManifestManager.LoadUpdateFileManifest(to_begin)
        else
            to_begin()
        end
    end
    
    set_progress_txt(lua_localization("main_enter_game"), 5, 1)--刷新脚本。"--main_enter_game main_refresh_script
    
    cdn_hosts = ResVersion.cdn_host or {}
    delay(load_manifest, 0.01)
end

local function one_file_down(loading_event_arg)
    local m = 1024 * 1024
    local loaded_s = string.format("%.2f", loading_event_arg.current / m)
    local loaded_t = string.format("%.2f", loading_event_arg.total / m)
    set_progress_txt(lua_localization("main_downloading_tips", loaded_s, loaded_t), 4, loading_event_arg.current / loading_event_arg.total)
end

local function all_file_down(is_error)
    print("all files have been download and the error is ", is_error)
    if is_error then
        local tips = lua_localization("main_download_fail")
        set_progress_txt(tips)--"文件下载失败请重启游戏。")
        local function close()
            MessageBox.Destroy()
        end
        MessageBox.Show(tips, "", close)
    else
        set_resversion(server_ver)
        if server_manifest then
            ManifestManager.updateFileManifest = server_manifest
            ManifestManager.fileManifest:AppendFileManifest(ManifestManager.updateFileManifest);
        end
        server_manifest = nil
        if ManifestManager.CheckFirstLoad() then ManifestManager.FinishFirstLoad() end
        
        FileHelper.DeletePersistentFile(CUtils.GetRightFileName(UPDATED_LIST_NAME))--删除旧文件
        FileHelper.DeletePersistentFile(CUtils.GetRightFileName(VERSION_FILE_NAME))--删除旧文件
        FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME), CUtils.GetRightFileName(UPDATED_LIST_NAME))
        FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME), CUtils.GetRightFileName(VERSION_FILE_NAME))
        set_progress_txt(lua_localization("main_download_complete"), 4, 1)--"更新完毕，进入游戏！"
        local loader_key = "core.loader"
        package.loaded[loader_key] = nil
        package.preload[loader_key] = nil
        print("all_file_update_down")
        enter_game(true)
    end

end

main_update.load_server_file_list = function(load_first)--版本差异化对比
        
        local function on_www_comp(req, bytes)
            dprint("on www comp " .. req.assetName)
            FileHelper.SavePersistentFile(bytes, CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME))--保存server端临时文件
        end
        
        local function on_server_comp(req)
            set_progress_txt(lua_localization("main_compare_crc_list"))--校验列表对比中。")
            local ab = req.data
            server_manifest = ab:LoadAllAssets()[1]
            ab:Unload(false)
            if not CUtils.isRelease then
                print(server_manifest)
                print(ManifestManager.fileManifest)
            end
            dprint("load first", load_first)
            Loader:unload_cache_false(req.key)
            local change = 0
            
            if load_first then
                change = BackGroundDownload.instance:AddFirstManifestTask(ManifestManager.fileManifest, server_manifest, one_file_down, all_file_down)
            else
                change = BackGroundDownload.instance:AddDiffManifestTask(ManifestManager.fileManifest, server_manifest, one_file_down, all_file_down)
            end
            dprint("need load file size:", change)
            if change > 0 then
                local function begin_load()
                    BackGroundDownload.instance.alwaysDownLoad = true
                    BackGroundDownload.instance.loadingCount = 4
                    BackGroundDownload.instance:Begin()
                    MessageBox.Destroy()
                end
                local tips = lua_localization("main_download_from_webserver", string.format("%.2f", change / 1048576))
                set_progress_txt(tips, 4, 0.01)--开始从服务器加载新的资源。
                MessageBox.Show(tips, "", "", begin_load)--版本提示
            else
                all_file_down()
            end
        end
        
        local function on_server_err(req)
            print("load_server_file_list error :", req.url)
            enter_game()
        end
        
        local function load_server(...)
            set_progress_txt(lua_localization("main_web_server_crc_list"))--加载服务器校验列表。")
            local crc = tostring(server_ver.crc32)
            local asset_name = CUtils.GetAssetName(UPDATED_LIST_NAME)
            local assetbundle_name = CUtils.GetRightFileName(asset_name .. Common.CHECK_ASSETBUNDLE_SUFFIX)
            local file_name = insert_assetBundle_name(assetbundle_name, "_" .. crc)
            print("load web server crc " .. server_ver.cdn_host[1] .. "/" .. file_name)
            local req = CRequest.Create(file_name, asset_name, AssetBundle, on_server_comp, on_server_err, nil, true)
            req.uris = get_update_uri_group(server_ver.cdn_host, on_www_comp, nil)
            Loader:get_www_data(req)
        end
        
        load_server()
end


main_update.load_server_verion = function(load_first)--加载服务器版本号
    main_update.on_first_complete = false
    local begin_fun
    local max_try_times, curr_time = 2, 0
    
    local function on_msg_click()
        MessageBox.Destroy()
        enter_game()
    end
    
    local function on_err(req)
        curr_time = curr_time + 1
        print("load_server_ver on erro,retry later " .. req.key, req.udKey, req.url, req.assetName, req.assetBundleName)
        if curr_time >= max_try_times then
            set_progress_txt(lua_localization("main_web_server_error"), 3, 0)--"加载服务器版本信息。"
            MessageBox.Show(lua_localization("main_web_server_error"), "", "", on_msg_click)
        end
    end
    
    local function on_comp(ver_str)
        server_ver = decode_ver_str(ver_str)
        print(string.format("load server comp,the server version is %s", server_ver.version))
        dprint(ver_str)
        if server_ver.version and CodeVersion.Subtract(server_ver.version, local_version.version) >= 0 then -- server_ver.version >= local_version.version then --如果服务器版本号>=本地
            set_resversion_var(server_ver)
        end
        BackGroundDownload.instance.hosts = ResVersion.cdn_host
        
        if check_platform() then --for test
            print("editor mode, just enter game directly")
            if ManifestManager.fileManifest == nil then
                enter_game(true)
            else
                enter_game()
            end
            return
        end
        
        FileHelper.SavePersistentFile(ver_str, CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME))--临时文件
        
        if server_ver.code == -1 or (server_ver.code <= 13003 and server_ver.code > 13000) then --错误码判断
            local function on_sure_click()
                Application.Quit()
            end
            MessageBox.Show(lua_localization("main_code_error"), "", "", on_sure_click)--版本提示
        elseif load_first then --need update first
            main_update.load_server_file_list(load_first)
        elseif CODE_VERSION < server_ver.code then --如果本地代码版本号不一致
            local new_app_tips = lua_localization("main_download_new_app")
            set_progress_txt(new_app_tips)--"请更新app版本！")
            local function open_url()
                Application.OpenURL(server_ver.update_url)
            end
            MessageBox.Show(new_app_tips, "", "", open_url)--版本提示
        elseif CodeVersion.Subtract(server_ver.version, local_version.version) >= 0 then -- and server_ver.crc32 ~= local_version.crc32 then --服务器版本号大于等于当前版本号 --&& server_ver.crc32 ~= local_version.crc32
            dprint("server version is newer than client,begin load server file list")
            main_update.load_server_file_list()
        else
            dprint("compare version complete, enter game")
            enter_game()
        end
    end
    
    local function on_req_comp(req)
		print("ver comp:"..req.url)
        if main_update.on_first_complete == false then
            on_comp(req.data)
        end
        main_update.on_first_complete = true
    end
    
    begin_fun = function()
        main_update.on_first_complete = false
        set_progress_txt(lua_localization("main_web_server_ver"), 3, 0.5)--"加载服务器版本信息。"
        dprint(" need first load = ", need_first_load)
        local ver_group = get_ver_url()
        max_try_times = #ver_group
        for k, v in pairs(ver_group) do
            print("start load server version info ... " .. v)
            Loader:get_http_data(v, nil, String, on_req_comp, on_err)
        end
    end
    
    begin_fun()
end

main_update.compare_local_version = function()--对比本地版本号
        -- 二次更新前埋点

        local step = {}
        step.key = CUtils.GetRightFileName(UPDATED_LIST_NAME)
        step.on_persistent_comp = function(req)
            local ver_str = req.data
            local ver_json = decode_ver_str(ver_str)
            print("load persistent version info complete,the version is " .. ver_json.version)
            dprint(ver_str)
            step.persistent_version = ver_json
            step.compare()
        end
        
        step.on_persistent_error = function(req)
            step.persistent_error = true
            dprint("local verion persistent error and the req key is " .. req.key)
            step.compare()
        end
        
        step.compare = function()
            local need_first_load = ManifestManager.CheckFirstLoad()
            if step.persistent_error == true and step.streaming_version ~= nil then
                dprint("there is no persistent_version info,just use streaming_version:", step.streaming_version.version)
                local_version = step.streaming_version
                set_resversion(local_version)
                main_update.load_server_verion(need_first_load)
            elseif step.persistent_version ~= nil and step.streaming_version ~= nil then
                if CodeVersion.Subtract(step.persistent_version.version, step.streaming_version.version) >= 0 then
                    dprint("直接进入。%s > %s", step.persistent_version.version, step.streaming_version.version)
                    local_version = step.persistent_version
                    set_resversion(local_version)
                    main_update.load_server_verion(need_first_load)
                else
                    dprint("persistent_version is older than streming_version ,use streaming_version:" .. os.date("%c", step.streaming_version.time) .. ",clear all caches")
                    local tips = lua_localization("main_clear_cache")
                    set_progress_txt(lua_localization("main_clear_cache"))--清理旧的缓存。")
                    print(tips)
                    print(step.persistent_version.version)
                    print(step.streaming_version.version)
                    package.loaded["core.loader"] = nil
                    package.preload["core.loader"] = nil
                    local_version = step.streaming_version --当前版本
                    set_resversion(local_version)
                    
                    FileHelper.DeletePersistentFile(CUtils.GetRightFileName(UPDATED_LIST_NAME))--删除旧文件
                    FileHelper.DeletePersistentFile(CUtils.GetRightFileName(VERSION_FILE_NAME))--删除旧文件
                    print("delete " .. UPDATED_LIST_NAME)
                    print("delete " .. VERSION_FILE_NAME)
                    
                    local function load_server_ver()main_update.load_server_verion(need_first_load) end
                    
                    ManifestManager.CheckClearCacheFiles(nil, load_server_ver)
                end
            
            end
        
        end
        
        step.on_streaming_comp = function(req)
            local ver_str = req.data
            local ver_json = decode_ver_str(ver_str)
            print("load streaming_version info complete,the version is ", ver_json.version)
            step.streaming_version = ver_json
            dprint(ver_str)
            step.compare()
        end
        
        step.on_streaming_error = function(req)
            print("<color=#ffff00>streaming ver.txt does't exist use (Hugula/BUild For Bublish) build</color>")
            step.streaming_version = {code = CODE_VERSION, crc32 = 0, time = os.time(), version = APP_VERSION}
            if Application.platform == RuntimePlatform.OSXEditor or Application.platform == RuntimePlatform.WindowsEditor then
                local path = "Assets/Hugula/Config/Ver_" .. CUtils.platform .. ".txt"
                if CUtils.isRelease == false then
                    path = "Assets/Hugula/Config/Ver_" .. CUtils.platform .. "_dev.txt"
                end
                local ver = FileHelper.ReadText(path)
                print(path, ver)
                ver = string.format(ver, CODE_VERSION, CODE_VERSION, CODE_VERSION)
                local arr = ver:split('\n')
                ver = '"eidtor":true'
                for k, v in ipairs(arr) do
                    ver = ver .. "," .. v
                end
                ver = string.format("{%s}", ver)
                local ext = decode_ver_str(ver)
                for k, v in pairs(ext) do
                    step.streaming_version[k] = v
                end
                print("load streaming_version info complete,the version is " .. step.streaming_version.version)
            else
                print("streaming ver.txt does't exist")
            end
            step.compare()
        end
        
        local ver_file_name = CUtils.GetRightFileName(VERSION_FILE_NAME)
        
        step.load_persistent = function()
            if FileHelper.PersistentFileExists(ver_file_name) then
                local uri = CUtils.GetRealPersistentDataPath()
                local url = CUtils.PathCombine(uri, ver_file_name)
                dprint(url)
                Loader:get_www_data(url, nil, String, step.on_persistent_comp, step.on_persistent_error, nil)
            else
                step.on_persistent_error({key = ver_file_name})
            end
        end
        
        step.load_streaming = function()
            local uri = CUtils.GetRealStreamingAssetsPath()
            local url = CUtils.PathCombine(uri, ver_file_name)
            dprint(url)
            Loader:get_www_data(url, nil, String, step.on_streaming_comp, step.on_streaming_error, nil)
        end
        
        set_progress_txt(lua_localization("main_compare_local_ver"), 2, 0.2)--"对比本地版本信息。"
        dprint("start load local version info...")
        step.load_streaming()
        step.load_persistent()
end

local function init_step1()
    dprint(Hugula.Utils.CUtils.GetRealPersistentDataPath())
    dprint(Hugula.Utils.CUtils.GetRealStreamingAssetsPath())
    dprint(UnityEngine.Application.version) --Application.bundleIdentifier)
    local ui_logo = LuaHelper.Find(FRIST_VIEW)
    local refer = ui_logo:GetComponent(Hugula.ReferGameObjects)
    
    if refer then
        _progressbar_txt = refer:Get("Text")--ui_logo:GetComponentInChildren(UnityEngine.UI.Text, true)
        _progressbar_slider = refer:Get("Progress")--ui_logo:GetComponentInChildren(UnityEngine.UI.Slider, true)
        set_progress_txt(lua_localization("main_init"), 1, 1)
        if _progressbar_slider then
            _progressbar_slider.gameObject:SetActive(true)
        end
        refer:Get("Logo").depth = 10
        
        local music_login = refer:Get("login_music")
        local _sound_status = UnityEngine.PlayerPrefs.GetInt("playerPrefsMusic", 1)
        if _sound_status == 1 then
            music_login.enabled = true
        else
            music_login.enabled = false
        end
    end
    main_update.compare_local_version()

end

init_step1()
