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

