require("helperFunction")
io.input("protocal/api.txt")
io.output("../../Assets/Lua/net/netAPIList.lua")

local baseIsUselessLine = isUselessLine
function isUselessLine(line)
    if(baseIsUselessLine(line)) then
        return true
    else
        if(string.sub(line, 1, 7) == "module:") then
            return true
        else
            return false
        end
    end
end

function parseAPI(startLine)
    local secondLine    = getNextValidLine()
    local thirdLine     = getNextValidLine()
    local forthLine     = getNextValidLine()

    local apiCode       = string.match(startLine, ":(%S+)")
    local apiName       = string.match(secondLine, "name:(%S+)")
    local apiDataStruct = string.match(thirdLine, "payload:(%S+)")
    local comment       = string.match(forthLine, "desc:(%S+)")
    if(apiCode == nil or apiName == nil or apiDataStruct == nil) then
        print("error api:")
        print(startLine)
        print(secondLine)
        print(thirdLine)
        print(forthLine)
    end


    io.write("\n")
    io.write("\t-- ", tostring(comment), " \n")
    io.write("\t", tostring(apiName), " = \n")
    io.write("\t{ \n")
    io.write("\t\tCode = ", tostring(apiCode), ",\n")
    io.write("\t\tDataStruct= ", "\'", tostring(apiDataStruct), "\'", ",\n")
    io.write("\t}, \n")
end

function generateIndex()
    io.flush()
    dofile("../../Assets/Lua/net/netApiList.lua")

    io.write("\n")
    io.write("NetAPIList.CodeToMsgType = { \n")
    for i, v in pairs(NetAPIList) do
        io.write("\t","MSGTYPE",v.Code,"\t=\tNetAPIList.",i,".DataStruct,\n")
    end
    io.write("} \n")


    io.write("\n")
    io.write("function NetAPIList:getDataStructFromMsgType(msgType)\n")
    io.write("\t return self.CodeToMsgType[msgType]\n")
    io.write("end\n")
end

io.write("------------------- API type ---------------\n")
io.write("NetAPIList= { \n")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    parseAPI(nextLine)
    nextLine = getNextValidLine()
end

io.write("} \n")
generateIndex()








