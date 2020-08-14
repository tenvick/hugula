--模拟服务器处理消息
local DIS_TYPE = DIS_TYPE
local LuaDispatcher = LuaDispatcher
local Logger = Logger
local api_list = require("net.net_api_list")

local server_rpc = {}

function server_rpc.Msg_Ping(data)
    Logger.Log("-----server_rpc.Msg_Ping-----")
    Logger.LogTable(data)
end

function server_rpc.Msg_HandShakeREQ(data)
    Logger.Log("-----server_rpc.Msg_HandShakeREQ-----")
    Logger.LogTable(data)
end

function server_rpc.Msg_LoginReq(data)
    Logger.Log("-----server_rpc.Msg_LoginReq-----")
    Logger.LogTable(data)
    Rpc:simulation_client_rpc(api_list.Msg_LoginAck.code,{code=1,token="adsfasdfasfasdfasdf",utc=os.time(),id=2,ip="192.168.1.159",9001})
end

return server_rpc
