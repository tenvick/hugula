local pairs = pairs
local ipairs = ipairs
local type = type
local print = print
local unpack = unpack
local setmetatable = setmetatable
local require = require
local error = error
local assert = assert
local pb = require "pb"
local string_format = string.format
local string_gmatch = string.gmatch
local net_api_list = require("net.net_api_list")
local table_remove = table.remove
local lower = string.lower
local DIS_TYPE = DIS_TYPE --require("net.lua_dispatcher_type")
local lua_distribute = lua_distribute
local client_rpc = require("net.client_rpc")

local SIMULATION_SERVER = false

local function make_rpc(rpc, name, ...)
    local rf = rpc.remoteF[name]
    if not rf then
        error(string_format("can't find remote function named %s", name))
        return nil
    end

    local code_type = rf.type
    local code = rf.code

    local bytes = assert(pb.encode(code_type, ...))

    return code, bytes
end

local function parse_rpc(rpc, code, bytes)
    local rf = rpc.localF[code]
    if not rf then
        error(string_format("rfid(%d) not found in LocalF", rfid))
        return false
    end

    local lf = client_rpc[rf.name]
    if lf then
        local data2 = assert(pb.decode(rf.type, bytes))
        lf(data2)
        
        local key = "rpc_" .. rf.name
        lua_distribute(lower(key), unpack(data2))

    end
    return true, rf.name
end

--模拟服务端处理,call server_rpc.lua 方法
local function simulation_remote_rpc(rpc, code, bytes)
    local rf = rpc.localF[code]
    if not rf then
        error(string_format("rfid(%d) not found in LocalF", code))
        return false
    end
    local remote_rpc = require("net.server_rpc")

    local lf = remote_rpc[rf.name]
    if lf then
        local data2 = assert(pb.decode(rf.type, bytes))
        lf(data2)
    end
    return true, rf.name
end

--模拟发送
local function simulation_client_rpc(rpc, code, data)
    local rf = rpc.localF[code]
    if not rf then
        error(string_format("rfid(%d) not found in LocalF", code))
        return false
    end

    local bytes = assert(pb.encode(rf.type, data))
    parse_rpc(rpc, code, bytes)
end

local function parse_protocol(rpc)
    local rpcS = {}
    local rpcC = {}

    -- parse server
    -- parse client
    for k, v in pairs(net_api_list) do
        -- if type(v) == "table" then
        local rf = {}
        rf.type = v.type
        rf.code = v.code
        rf.name = k
        rpcC[rf.code] = rf
        rpcS[k] = rf
        -- end
    end

    rpc.remoteF = rpcS
    rpc.localF = rpcC
end

local function init(rpc)
    parse_protocol(rpc)
    -- print("--------------------- init rpc ----------------------")
    -- Logger.LogTable(rpc.remoteF)
    -- Logger.LogTable(rpc.localF)
end

local function debug_list_all_rpc(rpc)
    local log = "<color=yellow>===========remote============\n"
    for name, rf in pairs(rpc.remoteF) do
        log = string_format("%s  %d %s %s\n", log, name, rf.type, rf.name)
    end
    local client_rpc = require("net.client_rpc")
    log = string_format("%s\n===========local==========\n", log)
    for code, rf in pairs(rpc.localF) do
        log = string_format("%s %s %d %s\n", log, code, rf.type, client_rpc[rf.name])
    end
    print("%s</color>", log)
end

local main_net = CS.Hugula.Net.NetWork.main
local mt = {
    __index = function(table, key)
        return function(rpc, ...)
            local code, bytes = rpc:make_rpc(key, ...)
            if SIMULATION_SERVER then
                Logger.Log("data:", pb.tohex(bytes))
                Logger.Log(
                    string.format("<color=#40ad23> send to SIMULATION_SERVER key=%s,code=%d,time=%d </color> ", key, code, os.time())
                )    
                simulation_remote_rpc(rpc, code, bytes)
            else
                
                main_net:SendBytes(code, bytes)
            end
        end
    end
}

local function reload(rpc)
    main_net = CS.Hugula.Net.NetWork.main
    package.loaded["net.net_api_list"] = nil
    net_api_list = require("net.net_api_list")

    init(rpc)
end

local function new()
    local ins = {}
    ins.init = init
    ins.make_rpc = make_rpc
    ins.parse_rpc = parse_rpc
    ins.simulation_client_rpc = simulation_client_rpc
    ins.debug_list_all_rpc = debug_list_all_rpc
    ins.reload = reload
    setmetatable(ins, mt)
    return ins
end

return new()
