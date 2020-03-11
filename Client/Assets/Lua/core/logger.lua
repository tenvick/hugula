------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--	asset 
--	author pu
------------------------------------------------
--- 日志打印
---@class Logger
Logger = {}
local Debug = CS.UnityEngine.Debug
function Logger.Log(...)
	local tab = {...}
	table.insert(tab,debug.traceback("",2))
	print(unpack(tab))
end

function Logger.LogFormat(format,...)
	local msg = string.format(format,...)
	Debug.Log(msg ..debug.traceback("",2) )
end

function Logger.LogError(msg)
	Debug.LogError(msg ..debug.traceback("",2) )
end

function Logger.LogErrorFormat(format,...)
	local msg = string.format(format,...)
	Debug.LogError(msg ..debug.traceback("",2) )
end