-- package.loaded["core.structure"] = nil
-- require("general.requires")
unpack = unpack or table.unpack
require("common.sys_define")
require("core.logger")
require("core.unity3d")
require("core.tools")
require("net.lua_dispatcher_type") --派发事件类型
require("net.lua_dispatcher")
require("general.server_tools")
require("common.sys_define")
T = {}
require("general.resmng")
-- require("general.requires")
local CS = CS
local CameraUtils = CS.CDL2.CameraUtils

local lod_config = {4, 5, 18, 300, 300, 300, 300} --客户端可视范围数据
local serpent = require("serpent")
require("net.lua_dispatcher")
require("net.lua_dispatcher_type")
-- local profiler = require("perf.profiler")
local unit_helper = require("viewmodels.worldmap.unit_helper")
local unit_base = require("viewmodels.worldmap.unit_base")
local ProfilerFactory = CS.Hugula.Profiler.ProfilerFactory

local os = os

local CUtils = CS.Hugula.Utils.CUtils
local table_insert = table.insert
local list = {}

local profiler = ProfilerFactory.GetAndStartProfiler("add unit_name_comp_test 1000 ")

local function check_range(self)
    local lod = self.lod or CameraUtils.LodLevel
    local range = self.lod_range
    if lod >= range[1] and lod <= range[2] then
        return true
    else
        return false
    end
end

local on_became_visible = function(data)
    local need_show = check_range(data.unit_name_comp_data)
    local u = CUtils.realPersistentDataPath
end
--组件模式
for i = 1, 10000 do
    local unit = {}
    unit.data ={id = i,unit_name_comp_data={lod_range = {0, 1},lod=0}}
    unit.unit_name_comp_test = {on_became_visible = on_became_visible }
    table_insert(list, unit)
end
if profiler then
    profiler:Stop()
end

local profiler = ProfilerFactory.GetAndStartProfiler("call unit_name_comp_test 1000 ")

local data
for i = 1, 10000 do
    local unit = list[i]
    unit["unit_name_comp_test"].on_became_visible(unit.data)
    -- _components["unit_name_comp_test"]:on_became_visible()
    -- unit:call_comp_func("on_became_visible")
end

if profiler then
    profiler:Stop()
end

local profiler = ProfilerFactory.GetAndStartProfiler("data model add unit_name_comp_test 1000 ")

list = {}
--数据模式
for i = 1, 10000 do
    local unit = {}
    unit.data ={id = i}
    unit.unit_name_comp_data = {lod_range = {0, 1},lod=0}
    table_insert(list, unit)
end
if profiler then
    profiler:Stop()
end

local profiler = ProfilerFactory.GetAndStartProfiler("data model call unit_name_comp_test 1000 ")



for i = 1, 10000 do
    local unit = list[i]  
    on_became_visible(unit)
    -- local need_show = check_range(unit.unit_name_comp_data)
    -- local u = CUtils.realPersistentDataPath
end

if profiler then
    profiler:Stop()
end

ProfilerFactory.DumpProfilerInfo(0, true, true)

local TLogger = CS.TLogger
local string_format = string.format

local function error_hander(h)
    TLogger.LogError(string_format("lua:%s \r\n %s", h, debug.traceback()))
end

local function safe_call(f, arg1, ...)
    local status, ret,a = xpcall(f, error_hander, arg1, ...)
    return status, ret,a
end

print(safe_call(function() return 1,2,3 end))

-- require("core.disable_global")
--[[
local api = 6
a = function(b)
    print("call fun idx=", b)
    -- print("lua_unbinding", a, "id=", b)
    lua_unbinding(api, a)
end
-- lua_binding(api, a)

for i = 1, 8 do
    local f
    f = function()
        print("call fun idx=", i)
        if i % 2 == 0 then
            -- print("lua_unbinding f=", f, "id=", i)
            lua_unbinding(api, f)
        end
    end

    if i == 6 then
        lua_binding(api, a)
    end
    lua_binding(api, f)
end
print("lua_distribute----------------------", api)
lua_distribute(api, api)
print("----------------------lua_distribute", api)
lua_distribute(api, api)
]]
--
-- [[--
-- local debug = debug

-- local function get_upvalue(f, name)
--     local i = 1
--     local up_name, up_value
--     repeat
--         up_name, up_value = debug.getupvalue(f, i)
--         if up_name == name then
--             return up_value, up_name
--         end
--         i = i + 1
--     until not up_name
-- end

-- local function set_upvalue(f, name, value)
--     local i = 1
--     local up_name, up_value
--     repeat
--         up_name, up_value = debug.getupvalue(f, i)
--         if up_name == name then
--             debug.setupvalue(f, i, value)
--             return
--         end
--         i = i + 1
--     until not up_name
-- end

-- local move_corts = {}
-- move_corts[1] = "hello 1"
-- virtual_ety = {}
-- virtual_ety.stop_move = function(eid)
--     local h1 = 1
--     print("old", move_corts[eid])
-- end

-- local old_stop_move = virtual_ety.stop_move

-- function virtual_ety.stop_move(eid)
--     local h2 = 2
--     -- print("h2", h2)
--     old_stop_move(eid)
--     local move_corts = get_upvalue(old_stop_move, "move_corts")
--     move_corts[eid] = "hello 2 override"
--     print(move_corts[eid])
--     old_stop_move(eid)
--     set_upvalue(old_stop_move, "move_corts", {"set hello 2 2 2 "})
-- end

-- virtual_ety.stop_move(1)
-- old_stop_move(1)

-- local global = global
-- --测试client_gridtree
-- local client_gridtree = GridTree(4, 0, 0)
-- local size = lod_config[1]
-- global.x = 100
-- global.y = 100
-- local remove, add

-- local function print_remove_add()
--     local str = ""
--     for k, v in pairs(remove) do
--         str = str .. "\r\n" .. string.format("id=%s,val=%s ", k.id, v)
--     end
--     print("remove", table.get_size(remove), str)
--     str = ""
--     for k, v in pairs(add) do
--         str = str .. "\r\n" .. string.format("id=%s,val=%s ", k.id, v)
--     end
--     print("add", table.get_size(add), str)
-- end

-- client_gridtree:intersects(100, 100, size, size) --移动到 世界坐标100,100

-- client_gridtree:push(x, y, {id = 0}) --加入单位unit id=0
-- client_gridtree:push(x + 10, y + 10, {id = 1}) --加入单位unit id=1

-- print(client_gridtree:contains(x, y))
-- print(client_gridtree:contains(x + 10, y + 10))

-- print("move to ", x + 10, y + 10, size)
-- remove, add = client_gridtree:intersects(x + 10, y + 10, size, size) --移动到 世界坐标100,100
-- print_remove_add()
-- client_gridtree:contains(x + 10, y + 10)

-- size = lod_config[3]
-- print("size change", size)
-- remove, add = client_gridtree:intersects(x + 10, y + 10, size, size) --移动到 世界坐标100,100
-- print_remove_add()

-- size = lod_config[1]

-- print("move back", size)
-- remove, add = client_gridtree:intersects(x, y, size, size) --移动到 世界坐标100,100
-- print_remove_add()
