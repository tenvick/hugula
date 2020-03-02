------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--	asset 
--	author pu
------------------------------------------------
--- 日志打印
---@class Logger
Logger = {}

function Logger.Log(...)
	local tab = {...}
	table.insert(tab,debug.traceback("",2))
	print(unpack(tab))
end

function Logger.LogError(msg)
	error(msg)
end
