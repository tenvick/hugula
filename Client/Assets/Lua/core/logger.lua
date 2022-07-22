------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--	asset
--	author pu
------------------------------------------------
local CUtils = CS.Hugula.Utils.CUtils
local serpent = require("serpent")
local TLogger = CS.TLogger
local debug = debug
local unpack = unpack or table.unpack
local string_format = string.format
local table_insert = table.insert
local print = print
local MAX_DEPTH = 6
--- 日志打印
---@class Logger
local logger = {}
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

function logger.LogFormat(format, ...)
    if CUtils.printLog then
        local msg = string_format(format, ...)
        TLogger.Log(msg .. debug.traceback("", 2))
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
