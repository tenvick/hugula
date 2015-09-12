require("helperFunction")
io.input("protocal/error_code.txt")
io.output("../../Assets/Lua/net/alertDataList.lua")

function parseAlertData(startLine)
    local r = {}
    for i in string.gmatch(startLine, string.format('[^%s]+', '-')) do
        table.insert(r, i)
    end
    local alertCode = r[1]
    local alertKey = r[2]
    local alertDesc = r[3]
    local alertType = r[4] or 'Tip'
    if(alertCode == nil or alertKey == nil) then
        print("------------------invalid format-------")
        print(startLine)
        return
    end

    io.write("\n")
    io.write("\t\t-- ", alertDesc, " \n")
    io.write("\t\t", tostring(alertKey), " = 'LC_ALERT_", tostring(alertKey), "',\n")
end

function generateIndex(startLine)
    local r = {}
    for i in string.gmatch(startLine, string.format('[^%s]+', '-')) do
        table.insert(r, i)
    end
    local alertCode = r[1]
    local alertKey = r[2]
    local alertDesc = r[3]
    local alertType = r[4] or 'Tip'
    io.write(string.format("AlertDataList.CodeToKey[%s] = {\n\tkind = '%s',\n\ttext = '%s',\n}\n", tostring(alertCode), tostring(alertType), tostring(alertKey)))
end

io.write("------------------- AlertDataList ---------------\n")
io.write("AlertDataList= { \n")
io.write("\tDataList= { \n")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    parseAlertData(nextLine)
    nextLine = getNextValidLine()
end
io.write("\t},\n")
io.write("} \n")

io.flush()
io.input("protocal/error_code.txt")
io.write("AlertDataList.CodeToKey = { }\n")
local nextLine = getNextValidLine()
while(nextLine ~= nil) do
    generateIndex(nextLine)
    nextLine = getNextValidLine()
end

io.write("\nfunction AlertDataList:GetTextId(key)\n")
io.write("\treturn self.DataList[key]\n")
io.write("end\n")

io.write("\nfunction AlertDataList:GetTextIdFromCode(code)\n")
io.write("\treturn self:GetTextId(self.CodeToKey[code].text)\n")
io.write("end\n")

io.write("\nfunction AlertDataList:GetTypeFromCode(code)\n")
io.write("\treturn self.CodeToKey[code].kind\n")
io.write("end\n")

