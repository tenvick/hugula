------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
require("common.requires")
local VMState = VMState
local VMGroup = VMGroup
require("core.hot_reload")

-- -------------------------------------------------------------------------------
local GameObject = CS.UnityEngine.GameObject
VMState:push(VMGroup.welcome)

local NetWork = CS.Hugula.Net.NetWork.main
Logger.LogSys("hot fix 2022.7.2 09:03")
-- NetWork:Connect("192.168.31.159",9091,false)
-- local serpent = require("serpent")

-- local rapidjson = require "rapidjson"
-- local t = rapidjson.decode('{"a":123}')
-- print(t.a)
-- t.a = 456
-- local s = rapidjson.encode(t)
-- print("json", s)
-- -- ------------------------------------
-- local lpeg = require "lpeg"
-- print(lpeg.match(lpeg.R "09", "123"))
-- -- ------------------------------------
-- local pb = require "pb"
-- local protoc = require "protoc"

-- local p = protoc.new()

-- p:loadfile("commcode.proto")
-- p:loadfile("login.proto")
-- p:loadfile("comm.proto")
-- p:loadfile("message.proto")
-- p:loadfile("base.proto")
-- p:loadfile("Person.proto")
-- -- p:loadfile("userinfo")
-- -- for name, basename, type in pb.types() do
-- --     print(name, basename, type)
-- -- end

-- local loginreq = {
--     account = "aaa",
--     password = "adsf",
--     platform = "3"
-- }
-- local bytes = assert(pb.encode("netpack.MsgLoginReq", loginreq))
-- print(pb.tohex(bytes))
-- local data2 = assert(pb.decode("netpack.MsgLoginReq", bytes))
-- Logger.LogTable(data2)

-- loginreq={
--     code = 1,
--     token = "adfasdf",
--     id = 12222222222,
--     ipaddr = "192.168.113.25"
-- }
-- local bytes = assert(pb.encode("netpack.MsgLoginAck", loginreq))
-- print(pb.tohex(bytes))
-- local data2 = assert(pb.decode("netpack.MsgLoginAck", bytes))
-- Logger.LogTable(data2)

-- local data = {
--     name = "ilse",
--     age = 18,
--     contacts = {
--         {name = "alice", phonenumber = 12312341234},
--         {name = "bob", phonenumber = 45645674567}
--     }
-- }

-- local bytes = assert(pb.encode("Person", data))
-- print(pb.tohex(bytes))

-- local data2 = assert(pb.decode("Person", bytes))
-- print(data2.name)
-- print(data2.age)
-- print(data2.address)
-- print(data2.contacts[1].name)
-- print(data2.contacts[1].phonenumber)
-- print(data2.contacts[2].name)
-- print(data2.contacts[2].phonenumber)
-- -- ---------------------------------
-- local ffi = require("ffi")
-- ffi.cdef [[
--     typedef struct {int fake_id;unsigned int len;} CSSHeader;
-- ]]
-- ffi.cdef [[
--     typedef struct {
--         CSSHeader header;
--         float x;
--         float y;
--         float z;
--     } Vector3;
-- ]]

-- local Vector3Native = ffi.typeof("Vector3 *")
-- local v = CS.UnityEngine.Vector3(1, 2, 3)
-- local vn = ffi.cast(Vector3Native, v)
-- print(vn)
-- if vn.header.fake_id == -1 then
--     print("vector { ", vn.x, vn.y, vn.z, "}")
-- else
--     print("please gen code")
-- end
