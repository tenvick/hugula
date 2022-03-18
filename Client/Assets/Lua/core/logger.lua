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

function logger.LogSys(msg)
    TLogger.LogSys(msg .. debug.traceback("", 2))
end 

function logger.LogError(msg)
    TLogger.LogError(msg .. debug.traceback("", 2))
end

function logger.LogErrorFormat(format, ...)
    local msg = string_format(format, ...)
    TLogger.LogError(msg .. debug.traceback("", 2))
end

Logger = logger