------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
-- delay = PLua.Delay
-- stop_delay = PLua.StopDelay
Vector3 = UnityEngine.Vector3
local Resources = UnityEngine.Resources
local LuaTimer = LuaTimer
if unpack == nil then unpack = table.unpack end
-- local json = require "cjson"
if Hugula ~= nil then
    local Hugula = Hugula
    CUtils = Hugula.Utils.CUtils
    LuaHelper = Hugula.Utils.LuaHelper
    FileHelper = Hugula.Utils.FileHelper
    Version = Hugula.Utils.Version
    
    CacheManager = Hugula.Loader.CacheManager
    PrefabPool = Hugula.Pool.PrefabPool
    
    PLua = Hugula.PLua
    ReferGameObjects = Hugula.ReferGameObjects
    ScrollRectItem = Hugula.UGUIExtend.ScrollRectItem
    ScrollRectTable = Hugula.UGUIExtend.ScrollRectTable
    UGUIEvent = Hugula.UGUIExtend.UGUIEvent
    UGUIEventLuaTrigger = Hugula.UGUIExtend.UGUIEventLuaTrigger
end

local ins = PrefabPool.instance
--PrefabPool资源自动回收
--当内存达到阈值的时候触发回收，回收按照PrefabCacheType从0开始每段回收
--PrefabPool.gcDeltaTimeConfig = 10 两次GC检测间隔时间
--资源放入缓存使用的类型,与回收密切相关
PrefabCacheType =
    {
        segment0 = 0, --最先被回收
        segment1 = 1, --第一个内存阈值时候回收 (PrefabPool.threshold1 = 150)
        segment2 = 2,
        segment3 = 3, --第二个内存阈值的时候回收(PrefabPool.threshold1 = 180)
        segment4 = 4,
        segment5 = 5,
        segment6 = 6, --第三个阈值的时候回收 (PrefabPool.threshold1 = 200)
        segment7 = 7, --手动调用回收PrefabPool.GCCollect(PrefabCacheType.segment7)
        segment8 = 8 --永远不回收 TODO:自动收缩
    }
--item_object的 gc type类型
ItemGCType =
    {
        segment0 = 0, --标记回收                    关闭立即回收
        segment1 = 1, --一代 用于标记自己做回收策略     低端 lod=1
        segment2 = 2, --二代 用于标记自己做回收策略      中端 lod=2
        segment3 = 3, --三代  用于标记自己做回收策略     高端  lod=3
        segment4 = 4
    }

UnityEngine.ThreadPriority =
{
    Low = 0,
    BelowNormal = 1,
    Normal = 2,
    High = 4
}

function string.concat(...)
    local arg = {...}
    local ctab = {}
    for k,v in pairs(arg) do
        if v == nil then v = "" end
        ctab[k] = tostring(v)
    end
    local s = table.concat(ctab, "")
    return s
end

local gprint = print
function print(...)
    if CUtils.printLog==false then return end
  local arg={...}
  table.insert(arg,"\r\n\r\n"..debug.traceback().."\r\n\r\n")
  gprint(unpack(arg))
end

local gdebug = UnityEngine.Debug
function log_warning(...)
    local str = string.concat(...)
    gdebug.LogWarning(str)
end

function tojson(tbl, indent)
    assert(tal == nil)
    if not indent then indent = 0 end

    local tab=string.rep("  ",indent)
    local havetable=false
    local str="{"
    local sp=""
    if tbl then
        for k, v in pairs(tbl) do
            if type(v) == "table" then
                havetable=true
                if(indenct==0) then
                    str=str..sp.."\r\n  "..tostring(k)..":"..tojson(v,indent+1)
                else
                    str=str..sp.."\r\n"..tab..tostring(k)..":"..tojson(v,indent+1)
                end
            else
                str=str..sp..tostring(k)..":"..tostring(v)
            end
            sp=";"
        end
    end

    if(havetable) then      str=str.."\r\n"..tab.."}"   else        str=str.."}"    end

    return str
end

function print_table(tbl)
    if CUtils.printLog == false then return end
    print(tojson(tbl))
end

function math.randomseed1(i)
    math.randomseed(tostring(os.time()+tonumber(i)):reverse():sub(1, 6)) 
end

function table.get_elem_size(tab)
    if not tab then return 0 end
    local i = 0
    for k,v in pairs(tab) do
        i = i + 1
    end
    return i
end

function lua_gc()
  collectgarbage("collect")
  local c=collectgarbage("count")
  -- print(" gc end ="..tostring(c).." ")
end

--释放没有使用的资源 (mesh,texture)
function unload_unused_assets()
    Resources.UnloadUnusedAssets()
end

function send_message(obj,method,...)
    local fn = obj[method]
    if type(fn) == "function" then fn(obj,...) end
end

function class(base, _ctor)
    local c = {}    -- a new class instance
    if not _ctor and type(base) == 'function' then
        _ctor = base
        base = nil
    elseif type(base) == 'table' then
    -- our new class is a shallow copy of the base class!
        for i,v in pairs(base) do
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
        setmetatable(obj,c)

        if _ctor then
            _ctor(obj,...)
        end
        return obj
    end

    c._ctor = _ctor
    c.is_a = function(self, klass)
        local m = getmetatable(self)
        while m do
            if m == klass then return true end
            m = m._base
        end
        return false
    end
    setmetatable(c, mt)
    return c
end

--用timer实现delay 函数
-- function delay(fun,delay_time,...)
--     local arg = {...}
--     local function temp_delay(id)
--       fun(unpack(arg))
--     end
--     local id = LuaTimer.Add(delay_time*1000,temp_delay)
--     return id
-- end

-- function stop_delay(id)
--   if type(id) == "number" then LuaTimer.Delete(id) end
-- end

delay = PLua.Delay
stop_delay = PLua.StopDelay
multiple_requires = PLua.MultipleRequires --(string[] requires,luafunc ) //eof is the group split sign