require("helperFunction")
io.output("../../Assets/Lua/net/netMsgHelper.lua")


function genParseFunction(startLine)
    local structName    = string.match(startLine, "(%S+)=")
    local nextLine      = getNextValidLine()
    print("NetMsgHelper model name" , structName)

    local funStr = ""
    local parStr=""
    local sp=""

    while(not isEndStruct(nextLine)) do
        local valueName, valueType = string.match(nextLine, "(%S+)%s+(%S+)") --        local valueName, valueType ,z= string.match(nextLine, "(%S+)%s+(%S+)")--"(%S+)%s+(%S+)")

        local ueserType=string.match(valueType,"(%S%S_)")
        --print("userType="..ueserType)
        if ueserType ~= nil then
             parStr=parStr..sp..valueType
            funStr=funStr.."\tt[\""..valueName.."\"]="..valueType.."\n"

        else
            parStr=parStr..sp..valueName
            funStr=funStr.."\tt[\""..valueName.."\"]="..valueName.."\n"

        end
        sp=","

        nextLine = getNextValidLine()
    end

    io.write("function NetMsgHelper:make",structName, "(".. parStr..") \n")
    io.write("\tlocal t = {}\n")
    io.write(funStr)
    io.write("\treturn t\n")
    io.write("end\n\n")

end

io.input("protocal/protocal.txt")
io.write("NetMsgHelper={}\n")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    genParseFunction(nextLine)
    nextLine = getNextValidLine()
end
