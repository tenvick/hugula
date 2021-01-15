local require = require
local pairs = pairs

--lua
local DIS_TYPE = DIS_TYPE
local Logger = Logger
local lua_distribute = lua_distribute
local Rpc = Rpc
local Model = require("models.model")
local client_rpc = {}

function client_rpc.Msg_LoginAck(data)
    Logger.Log("-----client_rpc.Msg_LoginAck-----")
    Logger.LogTable(data)
    Model.login_data = data
    LuaDispatcher:distribute(DIS_TYPE.LOGIN_PACKET_ACK,data)
end


return client_rpc
