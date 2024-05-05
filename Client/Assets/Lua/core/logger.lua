------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--	asset
--	author pu
------------------------------------------------
local CS = CS
local CUtils = CS.Hugula.Utils.CUtils
local serpent = require("serpent")

local line = serpent.line--rapidjson.encode--serpent.line
local block = serpent.block --rapidjson.encode--serpent.block
local debug = debug
local unpack = unpack or table.unpack
local string_format = string.format
local table_insert = table.insert
local raw_print = print
local print = raw_print
local error = error
local type = type
local string = string
local unpack = unpack
local MAX_DEPTH = 8
local TLogger = CS.TLogger
local Application = CS.UnityEngine.Application
local RuntimePlatform = CS.UnityEngine.RuntimePlatform
local Debug = CS.UnityEngine.Debug

--- 日志打印
---@class Logger
local logger = {}
logger.raw_print = raw_print

function logger.Log(...)
    if CUtils.printLog then
        local tab = {...}
        table_insert(tab, debug.traceback("", 2))
        print(unpack(tab))
    end
end

function logger.LogTable(tb)
    if CUtils.printLog then
        print(serpent.block(tb))
    end
end

--- 兼容测试时候开启，正式发布时候关闭
function logger.Log2(...)
    if CUtils.printLog or (CUtils.isRelease == false and CUtils.printLog == false ) then
        local msg = line({...}, {numformat = "%s", comment = false, maxlevel = MAX_DEPTH})
        local str = msg.. debug.traceback("", 2)
        TLogger.LogSys(str)
    end
end

function logger.LogFormat(format, ...)
    if CUtils.printLog then
        local msg = string_format(format, ...)
        TLogger.Log(msg .. debug.traceback("", 2))
    end
end


function logger.LogWarning(...)
    if CUtils.printLog or (CUtils.isRelease == false and CUtils.printLog == false ) then
        local msg = line({...}, {numformat = "%s", comment = false, maxlevel = MAX_DEPTH})
        Debug.LogWarning(msg .. debug.traceback("", 2))
    end
end

function logger.LogWarningFormat(format, ...)
    if CUtils.printLog or (CUtils.isRelease == false and CUtils.printLog == false )  then
        local msg = string.format(format, ...)
        Debug.LogWarning(msg .. debug.traceback("", 2))
    end
end


function logger.LogSys(...)
    local msg = serpent.line({...}, {numformat = "%s", comment = false, maxlevel = MAX_DEPTH})
    if CUtils.printLog then
        msg =  msg.. debug.traceback("", 2) 
    end
    TLogger.LogSys(msg)
end

function logger.LogSysError(...)
    local msg = serpent.line({...}, {numformat = "%s", comment = false, maxlevel = MAX_DEPTH})
    local str = msg .. debug.traceback("", 2)
    TLogger.LogError(str)
end

function logger.LogError(...)
    if CUtils.printLog then
        local msg = serpent.line({...}, {numformat = "%s", comment = false, maxlevel = MAX_DEPTH})
        local str = string_format("%s\n%s", msg, debug.traceback("", 2))
        TLogger.LogError(str)
    end
end

function logger.LogErrorFormat(format, ...)
    if CUtils.printLog then
        local msg = string_format(format, ...)
        TLogger.LogError(msg .. debug.traceback("", 2))
    end
end

Logger = logger
