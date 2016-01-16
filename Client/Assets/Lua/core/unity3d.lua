------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
-- luanet.load_assembly("UnityEngine.UI")
-- import "UnityEngine"
delay = PLua.Delay
stop_delay = PLua.StopDelay

if unpack==nil then unpack=table.unpack end

--print =function(...)

--end


function tojson(tbl,indent)
  assert(tal==nil)
	if not indent then indent = 0 end

	local tab=string.rep("	",indent)
	local havetable=false
	local str="{"
	local sp=""
    if tbl then
	     for k, v in pairs(tbl) do
	        if type(v) == "table" then
	    	    havetable=true
	    	    if(indenct==0) then
	    		    str=str..sp.."\r\n	"..tostring(k)..":"..tojson(v,indent+1)
	    	    else
	    		    str=str..sp.."\r\n"..tab..tostring(k)..":"..tojson(v,indent+1)
	   		    end
	        else
	    	    str=str..sp..tostring(k)..":"..tostring(v)
	        end
		    sp=";"
	     end
     end

	if(havetable) then	   	str=str.."\r\n"..tab.."}"	else	   	str=str.."}"	end

   return str
end


function get_angle(posFrom,posTo)  
    local tmpx=posTo.x - posFrom.x
    local tmpy=posTo.y - posFrom.y
    local angle= math.atan2(tmpy,tmpx)*(180/math.pi)
    return angle
end

function print_table(tbl)	print(tojson(tbl)) end

function string.split(s, delimiter)
    result = {};
    for match in (s..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
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

function lua_gc()
  collectgarbage("collect")
  c=collectgarbage("count")
  print(" gc end ="..tostring(c).." ")
end

LuaStack = {}
LuaStack.__index = LuaStack
local _mt = {}
_mt.__call = function(tb,size)
    return LuaStack.new(size)
end

setmetatable(LuaStack,_mt)

function LuaStack.new(size)
    if size == nil then size = 32 end
    local o = {size = size}
    setmetatable(o,LuaStack)
    return o
end

function LuaStack:push(v)
    table.insert(self,v)
    if #self > self.size then table.remove(self,1) end -- 确保长度
end

function LuaStack:get(i) --获取 i 位置 0 栈顶， 
    local top = #self
    i = top + i
    if i > 0 and #self >= i then
        return self[i]
    else
        return nil
    end
end

function LuaStack:pop(i) --从顶部取出 i 个 
    local top = #self
    local re = {}
    while (i>0) do
        local r = table.remove(self,#self)
        i = i - 1
        table.insert(re,r)
    end
    return re
end


