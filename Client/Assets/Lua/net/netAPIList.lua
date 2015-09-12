------------------- API type ---------------
NetAPIList= { 

	-- 待用加密key. 
	code_ack = 
	{ 
		Code = 10001,
		DataStruct= 'pt_code',
	}, 

	-- 心跳包 
	heartbeat_req = 
	{ 
		Code = 1,
		DataStruct= 'pt_int',
	}, 

	-- 链接成功后获取sessionID 
	login_session = 
	{ 
		Code = 10,
		DataStruct= 'pt_login_session',
	}, 

	-- 请求登陆 
	login_req = 
	{ 
		Code = 11,
		DataStruct= 'pt_login_req',
	}, 

	-- 房间用户列表 
	room_list = 
	{ 
		Code = 12,
		DataStruct= 'pt_room_list',
	}, 

	-- 发送聊天消息 
	chat_send = 
	{ 
		Code = 15,
		DataStruct= 'pt_chat_send',
	}, 
} 

NetAPIList.CodeToMsgType = { 
	MSGTYPE11	=	NetAPIList.login_req.DataStruct,
	MSGTYPE15	=	NetAPIList.chat_send.DataStruct,
	MSGTYPE10	=	NetAPIList.login_session.DataStruct,
	MSGTYPE12	=	NetAPIList.room_list.DataStruct,
	MSGTYPE1	=	NetAPIList.heartbeat_req.DataStruct,
	MSGTYPE10001	=	NetAPIList.code_ack.DataStruct,
} 

function NetAPIList:getDataStructFromMsgType(msgType)
	 return self.CodeToMsgType[msgType]
end
