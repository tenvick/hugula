------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local io = io

local serpent = require("serpent")
local pb = require "pb"
local protoc = require "protoc"
local p = protoc.new()

p:loadfile("_define.proto") --定义的消息ID namespace = id

local path = CS.UnityEngine.Application.dataPath .. "/Lua/net/net_api_list.lua"
print(path)
local file, err = io.open(path, "w+")
if err then
    print(err)
end
file:write(
    [[
-----------------this is generate by hugula lua protobuf tool -------------------------
-------------------------do not change--------------------------------------
local net_api_list = {
]]
)

local pack_name = "netpack"
local propto_msg_name = "def.MsgID"

for name, number, type in pb.fields(propto_msg_name) do
    -- print(name, number, type)
    file:write(
        string.format(
            [[
    %s={
        code = %d,
        type = "%s.%s",
    },   
    ]],
            name,
            number,
            pack_name,
            name
        )
    )
end

file:write([[}
    return net_api_list
]])

-- file:write([[}
-- net_api_list.code_msg_type = {

-- ]])

-- for name, number, type in pb.fields(propto_msg_name) do
--     -- print(name, number, type)
--     file:write(string.format([[
--     [%d]=net_api_list.%s.type,
--     ]], number, name))
-- end

-- file:write(
--     [[}

-- function net_api_list:get_struct_from_type(msg_type)
--     return self.code_msg_type[msg_type]
-- end

-- return net_api_list
-- ]]
-- )

print("-----------generate sueccess (Lua/net/net_api_list.lua)!----------------------")
