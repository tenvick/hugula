------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription 持久化服务
--  author pu
--  data    2020.11.18
------------------------------------------------
local require = require
local load = load
local pcall = pcall
local string = string
local json = require("common.json")

local CS = CS
local PlayerPrefs = CS.UnityEngine.PlayerPrefs
local LuaHelper = CS.Hugula.Utils.LuaHelper

local persistent = {}

function persistent.set_string(key, value)
    PlayerPrefs.SetString(key, value)
end

function persistent.get_string(key, default)
    return PlayerPrefs.GetString(key, default)
end

function persistent.set_float(key, value)
    PlayerPrefs.SetFloat(key, value)
end

function persistent.get_float(key, default)
    return PlayerPrefs.GetFloat(key, default or 0)
end

function persistent.save_local_luatable(file_name, tab)
    local str = json:encode(tab)
    LuaHelper.SaveLocalData(file_name, str)
    -- Logger.Log("=====save table====", file_name)
end

function persistent.load_local_luatable(file_name)
    local str = LuaHelper.LoadLocalData(file_name)
    if str and string.len(str) > 0 then
        local tab = json:decode(str)
        return tab
    end
    return nil
end

function persistent.save_local_data(file_name, tab, field_name)
    tab.__field = field_name
    local str = json:encode(tab)
    -- Logger.Log("persistent.save_local_data:", str)
    LuaHelper.SaveLocalData(file_name, str)
end

-----------------------------存储值到指定文件----------------------------------------
function persistent:load_local_data(file_name)
    local str = LuaHelper.LoadLocalData(file_name)
    if str and string.len(str) > 0 then
        local tab = json:decode(str)
        local __field = tab.__field
        tab.__field = nil
        persistent[__field] = tab
        return tab
    end
end

function persistent:load_data()
    self:load_local_data(self.fn_)
    if not self:check_version() then
        --清理缓存数据
        self.data = nil
        self.data_ = {}
    else
        --替换缓存数据
        self.data_ = self.data
        self.data = nil
    end
end

function persistent:save_data()
    -- Logger.Log("[persistent] save_data")
    self.data_.version = self.version_
    persistent.save_local_data(self.fn_, self.data_, "data")
end

function persistent:check_version()
    if self.data and self.data.version and self.data.version == self.version_ then
        return true
    end
    return false
end

---设置指定值
---@param key string 读取key
---@param value any 设置的值
---@param flag bool 是否立即保存
function persistent:set_val(key, value, flag)
    self.data_[key] = value
    if flag and flag == true then
        self:save_data()
    end
end

---设置指定值
---@param key string 读取key
---@return any 读取的值
function persistent:get_val(key)
    return self.data_[key]
end

function persistent:initialize(self)
    local self = persistent
    self.fn_ = "setting.dat"
    self.version_ = 100 --版本号
    self.data_ = {}

    self:load_data()
end

persistent:initialize()

return persistent
