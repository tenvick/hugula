------------------------------------------------
--  Copyright © 2013-2020   Hugula game Engine
--   
--  author pu
------------------------------------------------
local require = require
local Rpc = Rpc
local DIS_TYPE = DIS_TYPE
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding


local login_model={}

function login_model.on_connection()
    local model = require("models.model")
    local data = model.login_data
    Rpc:Msg_LoginGateReq({token=data.token,id=data.id,utc=data.utc})
    Logger.Log("send login_model.Msg_LoginGateReq")
    Logger.LogTable({token=data.token,id=data.id,utc=data.utc})
end


function login_model.listen_connect()
    lua_binding(DIS_TYPE.NET_CONNECT_SUCCESS,login_model.on_connection)
end

function login_model.remove_listen_connect()
    lua_unbinding(DIS_TYPE.NET_CONNECT_SUCCESS,login_model.on_connection)
end


return login_model