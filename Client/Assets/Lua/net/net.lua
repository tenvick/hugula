local pairs = pairs
local type = type
local setmetatable = setmetatable
local require = require
local error = error
local assert = assert
local pb = require "pb"
local string_format = string.format
local LuaDispatcher = LuaDispatcher
local Rpc = Rpc
local DIS_TYPE = DIS_TYPE
local Logger = Logger

local net = {}
net.is_connected = false

--- 与服务器连接成功触发此事件
function net.on_connect()
    -- net.is_connected = true
    Logger.Log("Lua Net.onConnect")
    LuaDispatcher:distribute(DIS_TYPE.NET_CONNECT_SUCCESS)
end

--- 应该弹出提示框让玩家主动点击重连
function net.on_disconnect()
    -- if not net.is_connected then
    --     return
    -- end

    -- net.is_connected = false
    Logger.Log("Lua Net.onDisconnect")
    LuaDispatcher:distribute(DIS_TYPE.NET_DISCONNECT)
end

--- 开始尝试重连的时候触发这个事件，可以用于重置数据
function net.on_reconnect()
    -- net.is_connected = true
    Logger.Log("onReconnect")
end

function net.on_connect_error()
    -- if not net.is_connected then
    --     return
    -- end

    -- net.is_connected = false
    Logger.Log("onConnectError")
    LuaDispatcher:distribute(DIS_TYPE.NET_CONNECT_ERROR)
end

function net.on_unkown_error()
    Logger.Log("onUnkownError")
end

local NetEvent = {
    UNKOWN = 50,
    CONNECT = 51,
    DISCONNECT = 52,
    RECONNECT = 53,
    CONNECT_ERROR = 54,
    RECEIVE_ERROR = 55,
    SEND_ERROR = 56
}

local netEventHandler = {
    [NetEvent.UNKOWN] = net.on_unkown_error,
    [NetEvent.CONNECT] = net.on_connect,
    [NetEvent.DISCONNECT] = net.on_disconnect,
    [NetEvent.RECONNECT] = net.on_reconnect,
    [NetEvent.CONNECT_ERROR] = net.on_connect_error,
    [NetEvent.RECEIVE_ERROR] = net.on_disconnect,
    [NetEvent.SEND_ERROR] = net.on_disconnect
}

function net.net_handler(code, bytes)
    if code < 100 then
        local func = netEventHandler[code]
        if func then
            func()
            return true
        end
        return
    end

    local dealed, pname = Rpc:parse_rpc(code, bytes)
    if dealed then
    -- print("<color=green>Server: protocol(%s) processed</color>", pname)
    end
    return dealed
end

return net
