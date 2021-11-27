local rawset = rawset
local rawget = rawget
local setmetatable = setmetatable
local lower = string.lower

local dispatcher_type = {}
-- 返回key，用于分发网络消息
local mt = {}
mt.__index = function(tb, key)
    rawset(tb, key, lower(key))
    return rawget(tb, key)
end
setmetatable(dispatcher_type, mt)
---消息类型提示
---@class DIS_TYPE
------------------------网络消息------------------------
---@field NET_DISCONNECT function 网络断开
---@field NET_RECONNECT function 网络重连
---@field NET_CONNECT_ERROR function 网络链接错误
---@field NET_CONNECT_SUCCESS function 网络链接成功
    
---@field     RPC_LOGIN_PACKET_ACK function 登录确认信息
---@field     RPC_NET_CERTIFY function 验证成功后回调

------------------------------------------------
---@field ON_STATE_CHANGED function on_state_changed状态切完成后
---@field ON_STATE_CHANGING function on_state_changing状态切换前
---------------------------------------------------
DIS_TYPE = dispatcher_type
-- return dispatcher_type
