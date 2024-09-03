local table_insert = table.insert
local table_remove = table.remove
local pairs = pairs
local ipairs = ipairs
local type = type
local assert = assert
local xpcall = xpcall
local unpack = unpack
local string_format = string.format
local debug = debug

local lua_dispatcher = {}
local tables = {} --存储所有函数
local errorTables = {}
local TLogger = CS.TLogger

local function error_hander(h)
    Debug.LogError(string_format("lua:%s \r\n %s", h, debug.traceback()))
end

local function safe_call(f, ...)
    return xpcall(f, error_hander, ...)
end

function lua_dispatcher:binding(api, fun)
    -- assert(fun ~= nil, api .. ":lua_dispatcher:binding fun is null")
    local msgTable = tables[api]
    if msgTable == nil then
        msgTable = {}
        tables[api] = msgTable
    end

    -- for k, v in ipairs(msgTable) do
    --     if v == fun then
    --         Logger.LogErrorFormat("重复绑定： %s %s", api, fun)
    --         return
    --     end --同一个函数只能绑定一次。
    -- end
    table_insert(msgTable, fun)
end

--只绑定一个全局响应函数
function lua_dispatcher:bindingOne(api, fun)
    local msgTable = tables[api]
    if msgTable == nil then
        msgTable = {}
        tables[api] = msgTable
    end

    while (#msgTable > 0) do
        table_remove(msgTable, 1)
    end

    table_insert(msgTable, fun)
end

function lua_dispatcher:unbinding(api, fun)
    local msgTable = tables[api]
    if msgTable and fun ~= nil then
        -- msgTable[fun] = nil
        -- local len=#msgTable local funitem local reindex
        -- for i=1,len do	funitem=msgTable[i]	if funitem==fun then reindex=i end		end
        -- if reindex then table.remove(msgTable,reindex) end --msgTable[msgTable]=nil
        -- 这里没有remove，因为在事件分发时的callback回调中，调用unbinding会有问题

        for k, v in ipairs(msgTable) do
            if v == fun then
                table_remove(msgTable, k)
                break
            end
        end
    end
end

function lua_dispatcher:distribute(api, ...)
    self:callHandle(api, ...)
end

function lua_dispatcher:callHandle(api, ...)
    local funTable = tables[api]
    if funTable then
        local len = #funTable
        local i = 1
        while (i <= len) do
            safe_call(funTable[i], ...)
            --     --check delete
            if len > #funTable then
                len = #funTable
            else
                i = i + 1
            end
        end
    end
end

function lua_dispatcher:bindingError(code, fun)
    self.errorTables[code .. ""] = fun
end

function lua_dispatcher:unbindingError(code, fun)
    self.errorTables[code .. ""] = nil
end

function lua_dispatcher:clear_all()
    table.clear(tables)
end

lua_dispatcher.destructor = lua_dispatcher.clear_all

function lua_binding(api, fun)
    lua_dispatcher:binding(api, fun)
end

function lua_unbinding(api, fun)
    lua_dispatcher:unbinding(api, fun)
end

local local_utils
function lua_distribute(api, ...)
    local funTable = tables[api]

    if funTable then
        local len = #funTable
        if len > 0 then
            if local_utils == nil then local_utils = utils end
            local_utils.begin_sample("lua_distribute: ", api, len, ...)

            local i = 1
            while (i <= len) do
                safe_call(funTable[i], ...)
                --     --check delete
                if len > #funTable then
                    len = #funTable
                else
                    i = i + 1
                end
            end

            local_utils.end_sample()
        end
    end
end

--关闭
function lua_dispatcher:close()
    -- Net:Close()
end

return lua_dispatcher
