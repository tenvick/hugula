------------------- NetProtocalPaser---------------
local NetAPIList = NetAPIList
NetProtocalPaser = {}
function NetProtocalPaser:parsestring(msg, key, parentTable)
    parentTable[key]    =   msg:ReadString()
end

function NetProtocalPaser:parseinteger(msg, key, parentTable)
    parentTable[key]    =   msg:ReadInt()
end

function NetProtocalPaser:parsefloat(msg, key, parentTable)
    parentTable[key]    =   msg:ReadFloat()
end

function NetProtocalPaser:parseshort(msg, key, parentTable)
    parentTable[key]    =   msg:ReadShort()
end

function NetProtocalPaser:parsebyte(msg, key, parentTable)
    parentTable[key]    =   msg:ReadByte()
end

function NetProtocalPaser:parseboolean(msg, key, parentTable)
    parentTable[key]    =   msg:ReadBoolean()
end

function NetProtocalPaser:parsepkid(msg,key,parentTable)
    parentTable[key]    =   msg:ReadString()
end

function NetProtocalPaser:parsechar(msg,key,parentTable)
    parentTable[key]    =   msg:ReadChar()
end

function NetProtocalPaser:parseushort(msg,key,parentTable)
    parentTable[key]    =   msg:ReadUShort()
end

function NetProtocalPaser:parseuint(msg,key,parentTable)
    parentTable[key]    =   msg:ReadUInt()
end

function NetProtocalPaser:parseulong(msg,key,parentTable)
    parentTable[key]    =   msg:ReadULong()
end


function NetProtocalPaser:formatstring(msg, value)
    msg:WriteString(value)
end

function NetProtocalPaser:formatinteger(msg, value)
    msg:WriteInt(value)
end

function NetProtocalPaser:formatshort(msg, value)
    msg:WriteShort(value)
end

function NetProtocalPaser:formatfloat(msg, value)
    msg:WriteFloat(value)
end

function NetProtocalPaser:formatbyte(msg, value)
     msg:WriteByte(value)
end

function NetProtocalPaser:formatboolean(msg, value)
    msg:WriteBoolean(value)
end

function NetProtocalPaser:formatchar(msg, value)
    msg:WriteChar(value)
end

function NetProtocalPaser:formatushort(msg, value)
    msg:WriteUShort(value)
end

function NetProtocalPaser:formatuint(msg, value)
    msg:WriteUInt(value)
end

function NetProtocalPaser:formatulong(msg, value)
    msg:WriteULong(value)
end

function NetProtocalPaser:formatArray(msg, value, valueType)
    local funcName = "format" .. valueType
    local func = self[funcName]
    local arrayLen = #value
    msg:WriteUShort(arrayLen)
    for i=1, arrayLen, 1 do
        local v = value[i]
        func(self,msg,v)
    end
end

function NetProtocalPaser:formatpkid(msg,value)
     msg:WriteString(value)
end

function NetProtocalPaser:parseArray(msg, key, parentTable, valueType)
    local funcName = "parse" .. valueType
    local func = self[funcName]
    parentTable[key]= {}
    local arrayLen = msg:ReadUShort()
    for i=1, arrayLen, 1 do
        local tempT = {}
        func(self,msg,"tempK", tempT)
        table.insert(parentTable[key],tempT.tempK)
    end
end


function NetProtocalPaser:parseMessage(msg,msgType)
    local result    = {}
    msgType = "MSGTYPE" .. msgType
    local dataStructName = NetAPIList:getDataStructFromMsgType(msgType)

    if(dataStructName ~= "null") then
        local funcName = "parse"..dataStructName
        local func = NetProtocalPaser[funcName]
        func(self, msg,nil,result)
    end
    return result
end

function NetProtocalPaser:formatMessage(msg,msgType, content)
    msg:set_Type(msgType)
    msgType = "MSGTYPE" .. msgType
    local dataStructName = NetAPIList:getDataStructFromMsgType(msgType)
    if(dataStructName ~= "null") then
        local funcName = "format"..dataStructName
        --print("---- formatMessage func name--- " .. funcName)
        local func = NetProtocalPaser[funcName]
        func(self, msg, content)
    end
end

function NetProtocalPaser:parsept_pkid(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parsepkid(msg,'id', parentTable)
end

function NetProtocalPaser:parsept_pkids(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parseArray(msg,'ids', parentTable,'pkid')
end

function NetProtocalPaser:parsept_int(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parseinteger(msg,'i', parentTable)
end

function NetProtocalPaser:parsept_code(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parseinteger(msg,'api', parentTable)
	self:parseinteger(msg,'code', parentTable)
end

function NetProtocalPaser:parsept_login_session(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parseinteger(msg,'session', parentTable)
end

function NetProtocalPaser:parsept_login_req(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parsestring(msg,'name', parentTable)
	self:parseinteger(msg,'session', parentTable)
end

function NetProtocalPaser:parsept_room_list(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parseArray(msg,'list', parentTable,'pt_login_req')
end

function NetProtocalPaser:parsept_chat_send(msg, key, parentTable) 
	if(key ~= nil) then
		parentTable[key] = {}
		parentTable = parentTable[key]
	end
	self:parsestring(msg,'name', parentTable)
	self:parsestring(msg,'context', parentTable)
	self:parseinteger(msg,'session', parentTable)
end

function NetProtocalPaser:formatpt_pkid(msg, content) 
	if(content.id == nil) then print('formatNetMessage Error: id is nil' ) end 
	self:formatpkid(msg,content.id)
end

function NetProtocalPaser:formatpt_pkids(msg, content) 
	self:formatArray(msg,content.ids,'pkid')
end

function NetProtocalPaser:formatpt_int(msg, content) 
	if(content.i == nil) then print('formatNetMessage Error: i is nil' ) end 
	self:formatinteger(msg,content.i)
end

function NetProtocalPaser:formatpt_code(msg, content) 
	if(content.api == nil) then print('formatNetMessage Error: api is nil' ) end 
	self:formatinteger(msg,content.api)
	if(content.code == nil) then print('formatNetMessage Error: code is nil' ) end 
	self:formatinteger(msg,content.code)
end

function NetProtocalPaser:formatpt_login_session(msg, content) 
	if(content.session == nil) then print('formatNetMessage Error: session is nil' ) end 
	self:formatinteger(msg,content.session)
end

function NetProtocalPaser:formatpt_login_req(msg, content) 
	if(content.name == nil) then print('formatNetMessage Error: name is nil' ) end 
	self:formatstring(msg,content.name)
	if(content.session == nil) then print('formatNetMessage Error: session is nil' ) end 
	self:formatinteger(msg,content.session)
end

function NetProtocalPaser:formatpt_room_list(msg, content) 
	self:formatArray(msg,content.list,'pt_login_req')
end

function NetProtocalPaser:formatpt_chat_send(msg, content) 
	if(content.name == nil) then print('formatNetMessage Error: name is nil' ) end 
	self:formatstring(msg,content.name)
	if(content.context == nil) then print('formatNetMessage Error: context is nil' ) end 
	self:formatstring(msg,content.context)
	if(content.session == nil) then print('formatNetMessage Error: session is nil' ) end 
	self:formatinteger(msg,content.session)
end

