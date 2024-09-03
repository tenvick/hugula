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

------------------------------------
local groups = {}
local require = require
local table_insert = table.insert

local DelayFrame = DelayFrame

table_insert(groups, function()
    print("groups 1 require")  
    print(_VERSION)

    --require("common.requires")
end)

table_insert(groups, function()
    print("groups 2 require config")
    -- require("require_config")
end)

table_insert(groups, function()
    print("groups 3 设置")
    VMState:set_root(VMGroup.welcome,true) -- 设置根
 --标记回收组
    local vmManager = VMState:get_vm_manager()
    vmManager:mark_gc_group("welcome",true)

    VMState:push(VMGroup.welcome)
end)


--执行
for i, func in ipairs(groups) do DelayFrame(func, i) end


-- -------------------------------------------------------------------------------
local GameObject = CS.UnityEngine.GameObject
VMState:push(VMGroup.welcome)

local NetWork = CS.Hugula.Net.NetWork.main
Logger.LogSys("hot fix 2022.9.22 10:19")
-- NetWork:Connect("192.168.31.159",9091,false)
local serpent = require("serpent")

local rapidjson = require "rapidjson"
local t = rapidjson.decode('{"a":123}')
print(t.a)
t.a = 456
local s = rapidjson.encode(t)
print("json", s)
-- ------------------------------------
local lpeg = require "lpeg"
print(lpeg.match(lpeg.R "09", "123"))
-- ------------------------------------
local pb = require "pb"

local loginreq = {
    account = "aaa",
    password = "adsf",
    platform = "3"
}
local bytes = assert(pb.encode("netpack.Msg_LoginReq", loginreq))
print(pb.tohex(bytes))
local data2 = assert(pb.decode("netpack.Msg_LoginReq", bytes))
Logger.LogTable(data2)

loginreq={
    code = 1,
    token = "adfasdf",
    utc = os.time(),
    id = 12222222222,
    ip = "192.168.113.25",
    port = 25000025
}
local bytes = assert(pb.encode("netpack.Msg_LoginAck", loginreq))
print(pb.tohex(bytes))
local data2 = assert(pb.decode("netpack.Msg_LoginAck", bytes))
Logger.LogTable(data2)

local data = {
    name = "ilse",
    age = 18858756582,
    address = "上海",
    -- contacts = {
    --     {name = "alice", phonenumber = 13975126500},
    --     {name = "bob", phonenumber = 1398541250}
    -- }
}

local bytes = assert(pb.encode("netpack.Person", data))
print(pb.tohex(bytes))

local data2 = assert(pb.decode("netpack.Person", bytes))
print(data2.name)
print(data2.age)
print(data2.address)
-- print(data2.contacts[1].name)
-- print(data2.contacts[1].phonenumber)
-- print(data2.contacts[2].name)
-- print(data2.contacts[2].phonenumber)
-- ---------------------------------
local ffi = require("ffi")
ffi.cdef [[
    typedef struct {int fake_id;unsigned int len;} CSSHeader;
]]
ffi.cdef [[
    typedef struct {
        CSSHeader header;
        float x;
        float y;
        float z;
    } Vector3;
]]

local Vector3Native = ffi.typeof("Vector3 *")
local v = CS.UnityEngine.Vector3(1, 2, 3)
local vn = ffi.cast(Vector3Native, v)
print(vn)
if vn.header.fake_id == -1 then
    print("vector { ", vn.x, vn.y, vn.z, "}")
else
    print("please gen code")
end

ffi.cdef [[
    typedef struct lua_State lua_State;

    int parse_rpc(lua_State* L, unsigned int id, char* cur, int len);
]]
local parse_rpc = ffi.typeof("int parse_rpc(lua_State* L, unsigned int id, char* cur, int len)")
print(parse_rpc)
local lua_State = ffi.typeof("lua_State *")
print(lua_State)
-- print(lua_State.l_G)
