require("helperFunction")
io.output("../../Assets/Lua/net/netProtocalPaser.lua")

io.input("NetProtocalParserCode.lua")
local preCode = io.read("*all")
io.write(preCode)

-- function isEndStruct(line)
--     return line == "==="
-- end

function genParseFunction(startLine)
    local structName    = string.match(startLine, "(%S+)=")
    local nextLine      = getNextValidLine()
    
    print("parseProtocal structName" , structName)

    io.write("function NetProtocalPaser:parse",structName, "(msg, key, parentTable) \n")
    io.write("\tif(key ~= nil) then\n")
    io.write("\t\tparentTable[key] = {}\n")
    io.write("\t\tparentTable = parentTable[key]\n")
    io.write("\tend\n")

    while(not isEndStruct(nextLine)) do
        local valueName, valueType ,z= string.match(nextLine, "(%S+)%s+(%S+)")--"(%S+)%s+(%S+)")
        local isarr=string.find(nextLine,"array")              

        if  isarr ~=nil  then
            --local arrayKeyType = string.match(nextLine, "%S+%s+%S+%s+%$?(%S+)")
            --arrayKeyType=arrayKeyType:gsub("^%l", string.upper)
            io.write("\tself:parseArray(msg,","\'", valueName,"\'", ", parentTable,","'", valueType, "'", ")\n")
        else
            io.write("\tself:parse", valueType, "(msg,", "\'", valueName, "\'", ", parentTable)\n")
        end
        nextLine = getNextValidLine()
    end
    io.write("end\n\n")
end

function genFormatMessageFunction(startLine)
    local structName    = string.match(startLine, "(%S+)=")
    local nextLine      = getNextValidLine()

    io.write("function NetProtocalPaser:format",structName, "(msg, content) \n")
    while(not isEndStruct(nextLine)) do

        local valueName, valueType= string.match(nextLine, "(%S+)%s+(%S+)")    -- "(%S+)%s+(%S+)")
        local isarr=string.find(nextLine,"array")

        if isarr ~=nil then
            --local arrayKeyType = string.match(nextLine, "%S+%s+%S+%s+%$?(%S+)")
             --arrayKeyType=arrayKeyType:gsub("^%l", string.upper)
            io.write("\tself:formatArray(msg,","content.", valueName,",","'", valueType, "'", ")\n")
        else
            io.write("\tif(content.", valueName, " == nil) then print('formatNetMessage Error: ",valueName, " is nil' ) end \n")
            io.write("\tself:format", valueType, "(msg,", "content.", valueName, ")\n")
        end
        nextLine = getNextValidLine()
    end
    io.write("end\n\n")
end

io.input("protocal/protocal.txt")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    genParseFunction(nextLine)
    nextLine = getNextValidLine()
end

io.input("protocal/protocal.txt")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    genFormatMessageFunction(nextLine)
    nextLine = getNextValidLine()
end

