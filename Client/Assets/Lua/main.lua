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
collectgarbage("setpause", 110)
collectgarbage("setstepmul", 300)

json = require "lib.json"
local Hugula = Hugula
local RuntimePlatform = UnityEngine.RuntimePlatform
local Application = UnityEngine.Application
local GameObject = UnityEngine.GameObject
local ManifestManager = Hugula.Loader.ManifestManager
local AssetBundle = UnityEngine.AssetBundle
local LoaderType = Hugula.Loader.LoaderType
local WebHeaderCollection = System.Net.WebHeaderCollection

local CodeVersion = Hugula.CodeVersion
local CODE_VERSION = CodeVersion.CODE_VERSION
local APP_VERSION = CodeVersion.APP_VERSION

local CUtils = Hugula.Utils.CUtils
local LuaHelper = Hugula.Utils.LuaHelper
local FileHelper = Hugula.Utils.FileHelper
local Common = Hugula.Utils.Common
local PLua = Hugula.PLua
local CRequest = Hugula.Loader.CRequest
local ResourcesLoader = Hugula.Loader.ResourcesLoader
local MessageBox = Hugula.UI.MessageBox

local delay = PLua.Delay
local stop_delay = PLua.StopDelay
local Localization = Hugula.Localization --
local HugulaSetting = Hugula.HugulaSetting.instance
local BackGroundDownload = Hugula.Update.BackGroundDownload
ResVersion = {code = CODE_VERSION, crc32 = 0, time = os.time(), version = APP_VERSION}

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
function string.concat(...)
    local arg = {...}
    local ctab = {}
    for k,v in pairs(arg) do
        if v == nil then v = "" end
        ctab[k] = tostring(v)
    end
    local s = table.concat(ctab, "")
    return s
end

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
    if CUtils.printLog then
        print(...)
    end
end

function run_times(...)
    if CUtils.printLog==false then return end
    local msg = string.concat(...)
    CUtils.DebugCastTime(msg)
end

-- ----------------------------------------------------------
-- --local fristView
local MAX_STEP = 6
local server_ver
local server_manifest
-- local DEBUG_UPDATE = false
-- --- global-----------------------------------
cdn_hosts = {}
-- ---------------------------local function ----------------
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
            if (k == "code" or k == "crc32" or k == "version") then
                ResVersion["svr_"..k] = v
            end
            ResVersion[k] = v
        end
    end
end

local function decode_ver_str(str)
    local ok, t = pcall(json.decode,json, str)
    if not ok then
        return {code = CODE_VERSION, crc32 = 0, time = os.time(), version = APP_VERSION}
    end
    return t
end

local function get_ver_url()
    local ver_url --版本地址
    local udid = UnityEngine.SystemInfo.deviceUniqueIdentifier --
    local hosts = string.split(http_ver_hosts, ",")
    local group = {}
    for k, v in pairs(hosts) do
        if string.match(v, "%?") then
            ver_url = string.format(v, APP_VERSION, udid, CUtils.platform, os.time())--http server
        else
            ver_url = string.format(v, CUtils.platform,"ver.txt")
        end
        table.insert(group,ver_url)
        dprint(ver_url)
    end
    return group
end

local obj_coroutine = PLua.coroutine
local main_view = {binder={}}
local main_viewmodel = {property_changed={},property={}}

local function per_str(step,per)
    local p = (step + per) / MAX_STEP
    return p
end

local function enter_game(manifest)
    local function to_begin(...)
        BackGroundDownload.instance.alwaysDownLoad = false --close always download
        BackGroundDownload.instance.loadingCount = 2
        -- if _progressbar_slider then _progressbar_slider.gameObject:SetActive(false) end
        -- if _progressbar_txt then _progressbar_txt.gameObject:SetActive(false) end
        run_times("require('begin')")
        require("begin")
    end
    
    local function load_manifest(...)
        if manifest then --如果有更新需要刷新
            ManifestManager.LoadUpdateFileManifest(to_begin)
        else
            to_begin()
        end
    end
    
    main_viewmodel:set_slider_propgress(lua_localization("main_enter_game"), 5, 1) --刷新脚本。"
    cdn_hosts = ResVersion.cdn_host or {}
    delay(load_manifest, 0.01)
end

function main_view:init()
    local ui_logo = LuaHelper.Find(FRIST_VIEW)
    local refer = ui_logo:GetComponent(Hugula.ReferGameObjects)
    if refer then
        local  _progressbar_txt = refer:Get("Text")--ui_logo:GetComponentInChildren(UnityEngine.UI.Text, true)
        local _progressbar_slider = refer:Get("Progress")--ui_logo:GetComponentInChildren(UnityEngine.UI.Slider, true)
        if _progressbar_slider then
            _progressbar_slider.gameObject:SetActive(true)
            self:binding(_progressbar_slider,"value","slider_value")
            -- self:binding(_progressbar_txt.gameObject,"SetActive","progress_enable")
        end
        if _progressbar_txt then
            self:binding(_progressbar_txt,"text","progress_txt")
            -- self:binding(_progressbar_txt.gameObject,"SetActive","progress_enable")
        end
    end
 end

 function main_view:binding(target,method,field_path)
    local binder = self.binder
    if binder[field_path] == nil then binder[field_path] = {} end
    local binder_item = binder[field_path]
    table.insert(binder_item,{target,method})
 end

  function main_view:on_value_change(field_path,value)
    local binder_item = self.binder[field_path]
    if binder_item then
        for k,v in ipairs(binder_item) do
            print(v[1],v[2],value)
            v[1][tostring(v[2])] = value
        end
    end
  end

--view model

function main_viewmodel:set_property(propertyName,value) --设置属性
    local property =self.property
    if  property[propertyName] == value or not propertyName then return false end
        print(value)
        property[propertyName] = value
        self:raise_property_changed(propertyName,value)
    return true
end

function main_viewmodel:register_property_changed(view) --注册视图
    table.insert(self.property_changed,view)
end

function main_viewmodel:raise_property_changed(propertyName,value) --触发视图改变
    local property_changed = self.property_changed
    for k,v in ipairs(property_changed) do
        if v then v:on_value_change(propertyName,value) end
    end
end

main_viewmodel.slider_value = "slider_value"
main_viewmodel.progress_txt = "progress_txt"

function main_viewmodel:set_slider_propgress(txt,step,per)
    self:set_property(self.progress_txt,txt)
    self:set_property(self.slider_value,per_str(step, per))
end

function main_viewmodel:set_progress_txt(txt)
    self:set_property(self.progress_txt,txt)
end

function main_viewmodel:compare_local_version()--对比本地版本号

        local function load_streaming()
            local ver_file_name = CUtils.GetRightFileName(VERSION_FILE_NAME)
            local uri = CUtils.GetRealStreamingAssetsPath()
            local url = CUtils.PathCombine(uri, ver_file_name)
            print("begin load",url)
            local op = ResourcesLoader.UnityWebRequestCoroutine(url, nil, String)
            coroutine.yield(op)
            local data = op.data
            print(data)
            if ManifestManager.needClearCache then
                local tips = lua_localization("main_clear_cache")
                self:set_progress_txt(tips) --清理旧的缓存。")
                print(tips)
                FileHelper.DeletePersistentFile(CUtils.GetRightFileName(UPDATED_LIST_NAME))--删除旧文件
                -- FileHelper.DeletePersistentFile(CUtils.GetRightFileName(VERSION_FILE_NAME))--删除旧文件
                print("delete " .. UPDATED_LIST_NAME)
            end

            main_viewmodel:load_server_verion()
        end

        obj_coroutine:StartCoroutine(load_streaming)
end

function main_viewmodel:load_server_verion()--加载服务器版本号
    local ver_group = get_ver_url()
    local max_try_times, curr_time = 1, 1
    local load_server_ver,ver_str
    max_try_times = #ver_group

    for k, v in pairs(ver_group) do
        -- ResourcesLoader.UnityWebRequest(v, nil, String, on_req_comp, on_err)
        -- ResourcesLoader.UnityWebRequest(v, nil, String, on_req_comp, on_err)
    end

    local function on_msg_click()
        MessageBox.Destroy()
        enter_game() --or main_viewmodel:load_server_verion()
    end

    load_server_ver = function()
        local url = ver_group[curr_time]
        run_times("start load server version info ... " ,curr_time,url)
        local op = ResourcesLoader.HttpWebRequestCoroutine(url, nil, String)
        coroutine.yield(op)
        ver_str = op.data
        if ver_str == nil then
            curr_time = curr_time + 1
            run_times("load_server_ver on erro,retry later ",url)
            if curr_time >= max_try_times then
                MessageBox.Show(lua_localization("main_web_server_error"), "", "", on_msg_click)
                self:set_slider_propgress(lua_localization("main_web_server_error"), 3, 0)
            end
            return
        end

        server_ver = decode_ver_str(ver_str)
        run_times(string.format("load server ver.txt compelete,the server version is %s", server_ver.version))
        
        local sub_version = CodeVersion.Subtract(server_ver.version, ManifestManager.localVersion)
        self.need_server_filelist = sub_version > 0 --是否需要加载服务器效验列表
        print("need update server list",self.need_server_filelist)
        -- if  sub_version >= 0 then
            set_resversion(server_ver)
        -- end

        BackGroundDownload.instance.hosts = ResVersion.cdn_host
        
        if check_platform() then --for test &
            print("editor mode, just enter game directly")
            if ManifestManager.fileManifest == nil then
                enter_game(true)
            else
                enter_game()
            end
            return
        end
        
        FileHelper.SavePersistentFile(ver_str, CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME))--临时文件
        
       if CODE_VERSION < server_ver.code then --如果本地代码版本号不一致
            local new_app_tips = lua_localization("main_download_new_app")
            local function open_url()
                Application.OpenURL(server_ver.update_url)
            end
            MessageBox.Show(new_app_tips, "", "", open_url)--版本提示
        elseif self.need_server_filelist or ManifestManager.CheckFirstLoad() then --server_ver.version > local_version.version 或者需要做首包加载
            run_times("server version is newer than client,begin load server file list")
            self:load_server_file_list()
        else
            print("compare version complete, enter game")
            enter_game()
        end
    end
    obj_coroutine:StartCoroutine(load_server_ver)
end

function main_viewmodel:load_server_file_list()--版本差异化对比

    local function on_progress_event(loading_event_arg)
        local m = 1024 * 1024
        local loaded_s = string.format("%.2f", loading_event_arg.current / m)
        local loaded_t = string.format("%.2f", loading_event_arg.total / m)
        local kbs = string.format("%.2f kb/s",BackGroundDownload.BytesReceivedPerSecond/1024)
        -- print("bytes = kb/s",BackGroundDownload.BytesReceivedPerSecond/1024)
        local str = lua_localization("main_downloading_tips", loaded_s, loaded_t,kbs)
        self:set_slider_propgress(str, 4, loading_event_arg.current / loading_event_arg.total)
    end
    
    local function load_update_filelist(server_manifest) --开始加载更新文件
        local change = 0
        local need_first = ManifestManager.CheckFirstLoad()
        if need_first then
            change = BackGroundDownload.instance:AddFirstManifestTask(ManifestManager.fileManifest, server_manifest, on_progress_event, self.on_background_complete)
        else
            change = BackGroundDownload.instance:AddDiffManifestTask(ManifestManager.fileManifest, server_manifest, on_progress_event, self.on_background_complete)
        end
        print("need load file size:", change)
        if change > 0 then
            local function begin_load()
                BackGroundDownload.instance.alwaysDownLoad = true
                BackGroundDownload.instance.loadingCount = 4
                BackGroundDownload.instance:Begin()
                MessageBox.Destroy()
            end

            local tips = lua_localization("main_download_from_webserver", string.format("%.2f", change / 1048576))
            self:set_slider_propgress(tips, 4, 0.01)--开始从服务器加载新的资源。
            local is_wifi = Application.internetReachability == UnityEngine.NetworkReachability.ReachableViaLocalAreaNetwork
            print("is_wifi=",is_wifi)
            dprint(Application.internetReachability)
            if  need_first and is_wifi  then
                begin_load()
            else
                MessageBox.Show(tips, "", "", begin_load)--版本提示
            end
        else
            self.on_background_complete()
        end
    end
        
    local function on_server_comp(req) --服务端文件列表加载完毕
        run_times("on_server_file list compelete ",req.url)
        self:set_progress_txt(lua_localization("main_compare_crc_list"))--校验列表对比中。"
        local bytes = req.data
        FileHelper.SavePersistentFile(bytes, CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME))--保存server端临时文件
        local ab = LuaHelper.LoadFromMemory(bytes)
        server_manifest = ab:LoadAllAssets()[1]
        ab:Unload(false)
        print("server file list id down")
        if not CUtils.isRelease then
            print(server_manifest)
            print(ManifestManager.fileManifest)
        end
        -- CacheManager.UnloadCacheFalse(req.key)
        load_update_filelist(server_manifest)
    end
    
    local load_server

    local function on_server_err(req)
        run_times("load_server_file_list error :",req.url)
        local tips=lua_localization("main_web_server_error")
        MessageBox.Show(tips, "", "", function()
            load_server()
            -- enter_game()
        end)--版本提示
    end
    
    load_server=function(...)
        if self.need_server_filelist then
            self:set_progress_txt(lua_localization("main_web_server_crc_list"))--加载服务器校验列表。")
            local crc = tostring(server_ver.crc32)
            local asset_name = CUtils.GetAssetName(UPDATED_LIST_NAME)
            local assetbundle_name = CUtils.GetRightFileName(asset_name .. Common.CHECK_ASSETBUNDLE_SUFFIX)
            local file_name = insert_assetBundle_name(assetbundle_name, "_" .. crc)
            local url = CUtils.PathCombine(server_ver.cdn_host[1],file_name) --server_ver.cdn_host[1] .. "/" .. file_name
            self.cdn_file_url = url

            local req = CRequest.Create(url, asset_name, LoaderType.Typeof_Bytes, on_server_comp, on_server_err)
            run_times("begin load server ver "..url)
            ResourcesLoader.UnityWebRequest(req)
        else
            run_times("begin load update file list files !")
            load_update_filelist()
        end
    end
    
    load_server()
end

main_viewmodel.on_background_complete=function(is_error)
    local self = main_viewmodel
    print("all files have been download and the error is ", is_error)
    if is_error then
        local tips = lua_localization("main_download_fail")
        self:set_progress_txt(tips)--"文件下载失败请重启游戏。")
        local function reload()
            print("begin reload")
            BackGroundDownload.instance:ReloadError()    
            BackGroundDownload.instance:Begin()
            MessageBox.Destroy()
        end
        MessageBox.Show(tips, "", "",reload)
        -- self:load_server_file_list()
    else
        -- set_resversion(server_ver)
        if server_manifest then
            print("set update file manifest")
            ManifestManager.SetUpdateFileManifest(server_manifest)
        end
        server_manifest = nil
        if ManifestManager.CheckFirstLoad() then ManifestManager.FinishFirstLoad() end
        
        FileHelper.DeletePersistentFile(CUtils.GetRightFileName(UPDATED_LIST_NAME))--删除旧文件
        FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(UPDATED_TEMP_LIST_NAME), CUtils.GetRightFileName(UPDATED_LIST_NAME))
        
        if not self.need_server_filelist then 
            FileHelper.DeletePersistentFile(CUtils.GetRightFileName(VERSION_FILE_NAME))--删除旧文件
            FileHelper.ChangePersistentFileName(CUtils.GetRightFileName(VERSION_TEMP_FILE_NAME), CUtils.GetRightFileName(VERSION_FILE_NAME))
        end
        
        self:set_slider_propgress(lua_localization("main_download_complete"), 4, 1)--"更新完毕，进入游戏！"
        print("all_file_update_down")
        enter_game(true)
    end
end

main_view:init()
main_viewmodel:register_property_changed(main_view)



function main()
    main_viewmodel:load_server_verion()
end


