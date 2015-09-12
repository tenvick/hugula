------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
ServerMsgHelperr = {}
local ServerMsgHelperr = ServerMsgHelperr
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper

local TcpServer = TcpServer
local Net=Net
local Msg = Msg
local Proxy=Proxy
local NetMsgHelper = NetMsgHelper
local NetAPIList = NetAPIList
local NetProtocalPaser = NetProtocalPaser
--局部变量
local currTcpServer

ServerMsgHelperr.clients = {} --用户列表
--有新消息
local function newMsg(session,msg)
 ServerMsgHelperr:onMsgArrive(session,msg)
end

--有新用户连接
local function onClientConnect( session )
	print(session.id.." is connected")

    local cls = NetMsgHelper:makept_login_session(session.id)
	local msg = Proxy:getMsg(NetAPIList.login_session,cls)
    session:Send(msg)
end

local function onClientClose( session )
	print(session.id.." is close")
	local removeSessionId = session.id
	local clients = ServerMsgHelperr.clients
	local remove = -1
	for i,v in ipairs(clients) do
		if v.session == removeSessionId then
			remove = i
			break
		end
	end

	if remove>=0 then
		table.remove(clients,remove)
	end

	local cls = NetMsgHelper:makept_room_list(clients)
	local msg = Proxy:getMsg(NetAPIList.room_list,cls)
	currTcpServer:Broadcast(msg)
end

function ServerMsgHelperr:currTcpServer()
	return currTcpServer
end

function ServerMsgHelperr:onMsgArrive(session,msg)
	-- print(id)
	-- print(msg.Type)
	local ty =msg.Type
	-- print("on Server Msg "..ty)
	if ty == NetAPIList.login_req.Code then --如果有玩家登陆
		local model=NetProtocalPaser:parseMessage(msg,ty)
		-- printTable(model)
		table.insert(ServerMsgHelperr.clients,model)
		local cls = NetMsgHelper:makept_room_list(ServerMsgHelperr.clients)
		local msg = Proxy:getMsg(NetAPIList.room_list,cls)
		currTcpServer:Broadcast(msg)
	end
end

function ServerMsgHelperr:closeServer( ... )
	TcpServer.StopTcpServer()
end

function ServerMsgHelperr:createServer( ... )
  TcpServer.StartTcpServer()
  ServerMsgHelperr.clients = {}
  currTcpServer = TcpServer.currTcpServer
  currTcpServer.onClientConnectFn = onClientConnect
  currTcpServer.onMessageArriveFn = newMsg
  currTcpServer.onClientCloseFn = onClientClose
  currTcpServer.autoBroadcast = true
  local ip = TcpServer.GetLocalIP():ToString()
  ServerMsgHelperr:connect(ip)
end

function ServerMsgHelperr:connect(ip)
	Net:Connect(ip,TcpServer.port) --链接主机
end
