NetMsgHelper={}
function NetMsgHelper:makept_pkid(id) 
	local t = {}
	t["id"]=id
	return t
end

function NetMsgHelper:makept_pkids(ids) 
	local t = {}
	t["ids"]=ids
	return t
end

function NetMsgHelper:makept_int(i) 
	local t = {}
	t["i"]=i
	return t
end

function NetMsgHelper:makept_code(api,code) 
	local t = {}
	t["api"]=api
	t["code"]=code
	return t
end

function NetMsgHelper:makept_login_session(session) 
	local t = {}
	t["session"]=session
	return t
end

function NetMsgHelper:makept_login_req(name,session) 
	local t = {}
	t["name"]=name
	t["session"]=session
	return t
end

function NetMsgHelper:makept_room_list(pt_login_req) 
	local t = {}
	t["list"]=pt_login_req
	return t
end

function NetMsgHelper:makept_chat_send(name,context,session) 
	local t = {}
	t["name"]=name
	t["context"]=context
	t["session"]=session
	return t
end

