------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
unpack = unpack or table.unpack
local tostring = tostring
local table = table
local assert = assert
local pairs = pairs
local ipairs = ipairs
local type = type
local string = string
local math = math
local os = os
local collectgarbage = collectgarbage
local rawget = rawget
local rawset = rawset
local setmetatable = setmetatable
local getmetatable = getmetatable
local tonumber = tonumber

function math.lerp(min, max, t)
    --   y = y0 + α(y1 - y0)
    local v = min + t * (max - min)
    return v
end

function string.trim(s)
    return (s:gsub("^%s*(.-)%s*$", "%1"))
end

function string.concat(...)
    local arg = {...}
    local ctab = {}
    for k, v in pairs(arg) do
        if v == nil then
            v = ""
        end
        ctab[k] = tostring(v)
    end
    local s = table.concat(ctab, "")
    return s
end

local function tojson(tbl, indent)
    assert(tbl ~= nil)
    if not indent then
        indent = 0
    end

    local tab = string.rep("  ", indent)
    local havetable = false
    local str = "{"
    local sp = ""
    if tbl then
        for k, v in pairs(tbl) do
            if type(v) == "table" then
                havetable = true
                if (indent == 0) then
                    str = str .. sp .. "\r\n  " .. tostring(k) .. ":" .. tojson(v, indent + 1)
                else
                    str = str .. sp .. "\r\n" .. tab .. tostring(k) .. ":" .. tojson(v, indent + 1)
                end
            else
                str = str .. sp .. tostring(k) .. ":" .. tostring(v)
            end
            sp = ";"
        end
    end

    if (havetable) then
        str = str .. "\r\n" .. tab .. "}"
    else
        str = str .. "}"
    end

    return str
end

table.tojson = tojson

---查找table中的指定项的索引
---@overload fun(tb:table,item:any):int
---@param tb table
---@param item any
---@return int
function table.indexof(table,item)
    local idx
    for k, v in ipairs(table) do
        if v == item then
            idx = k
            break
        end
    end
    return idx
end

---移除table中的指定项
---@overload fun(tb:table,item:any):int
---@param tb table
---@param item any
---@return int
function table.remove_item(tb, item)
    local idx = table.indexof(tb,item)
    if idx ~= nil then
        table.remove(tb, idx)
        return idx
    end
    return nil
end

function table.clear(tb)
    for k, v in pairs(tb) do
        tb[k] = nil
    end
end

function table.clone(source, target)
    for k, v in pairs(source) do
        target[k] = v
    end
end

function table.get_size(tab)
    if not tab then
        return 0
    end
    local i = 0
    for k, v in pairs(tab) do
        i = i + 1
    end
    return i
end

function table.has_value(tb)
    return type(tb) == "table" and next(tb) ~= nil
end

function string.split(s, delimiter)
    local result = {}
    for match in (s .. delimiter):gmatch("(.-)" .. delimiter) do
        table.insert(result, match)
    end
    return result
end

function math.randomseed1(i)
    math.randomseed(tostring(os.time() + tonumber(i)):reverse():sub(1, 6))
end

function lua_gc()
    collectgarbage("collect")
    local c = collectgarbage("count")
    -- print(" gc end ="..tostring(c).." ")
end

function send_message(obj, method, ...)
    local fn = obj[method]
    if type(fn) == "function" then
        fn(obj, ...)
    end
end

---创建weak table
function weak_table()
    local mt = {__mode = "v"}
    local tab = {}
    setmetatable(tab, mt)
    return tab
end

---创建get set功能的class
function class_property(warp_class, ...)
    local mt = {}
    mt.__index = function(t, k)
        local prop = rawget(t, "_wrap")
        local raw = prop[k]
        if raw then
            return raw
        else
            -- end
            -- local get_ = "get_" .. k
            -- raw = prop[get_]
            -- if raw then
            --     return raw(prop, k)
            -- else
            -- Logger.Log("getvalue:",k)
            return prop:_get_value(k)
        end
    end

    mt.__newindex = function(t, k, v)
        if k == "_wrap" then
            rawset(t, k, v)
            return
        end
        local prop = rawget(t, "_wrap")
        -- local raw = prop[k]
        -- if raw then
        --     return raw(t, k, v)
        -- else
            -- local set_ = "set_" .. k
            -- set_ = prop[set_]
            -- if set_ then
            --     set_(prop, v)
            -- else
            prop:_set_value(k, v)
        -- end
    end

    mt.__tostring = function(t)
        local prop = rawget(t, "_wrap")
        return tostring(prop)
    end

    local mt_call = {}
    mt_call.__call = function(class_tbl, ...)
        local obj = {}
        local wrap = warp_class(...)
        obj._wrap = wrap
        -- obj.__tostring = wrap.__tostring
        setmetatable(obj, mt)
        return obj
    end

    local c = {}
    setmetatable(c, mt_call)
    return c
end

function class(base, _ctor)
    local c = {} -- a new class instance
    if not _ctor and type(base) == "function" then
        _ctor = base
        base = nil
    elseif type(base) == "table" then
        -- our new class is a shallow copy of the base class!
        for i, v in pairs(base) do
            c[i] = v
        end
        c._base = base
    end

    -- the class will be the metatable for all its objects,
    -- and they will look up their methods in it.
    c.__index = c

    -- expose a constructor which can be called by <classname>(<args>)
    local mt = {}

    mt.__call = function(class_tbl, ...)
        local obj = {}
        setmetatable(obj, c)

        if _ctor then
            _ctor(obj, ...)
        end
        return obj
    end

    c._ctor = _ctor
    c.is_a = function(self, klass)
        local m = getmetatable(self)
        while m do
            if m == klass then
                return true
            end
            m = m._base
        end
        return false
    end
    setmetatable(c, mt)
    return c
end
