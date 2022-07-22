------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local io = io

local serpent = require("serpent")
-- local pb = require "pb"
-- local protoc = require "protoc"
-- local p = protoc.new()

-- p:loadfile("msgid.proto")
local GetFileName = CS.System.IO.Path.GetFileName
local outfilepath = CS.UnityEngine.Application.dataPath .. "/Lua/net/protoc_init.lua"
local read_dir = CS.UnityEngine.Application.dataPath .. "/proto"

local fileMys = CS.System.IO.Directory.GetFiles (read_dir);

local proto_files = {}
for i =0,fileMys.Length-1 do
    local file = GetFileName(fileMys[i])
    -- print(file)
    if string.find(file, "%.proto$") then
        table.insert(proto_files, file)
    end
end
-- print(serpent.block(proto_files))
-- getLuaFileInfo:close()

-- print(outfilepath)
local file, err = io.open(outfilepath, "w+")
if err then print(err) end
file:write(
    [[
-----------------this is generate by hugula lua protobuf tool -------------------------
-------------------------do not change--------------------------------------
local pb = require("pb")
-- local protoc = require "protoc"
-- local p = protoc.new()
]]
)

for k,v in ipairs(proto_files) do
    file:write(string.format("p:loadfile(\"%s\")\n",v))
end

print("-----------generate sueccess (Lua/net/protoc_init.lua)!----------------------")
