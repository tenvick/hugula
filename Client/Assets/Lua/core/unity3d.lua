------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
-- luanet.load_assembly("UnityEngine.UI")
import "UnityEngine"
delay = PLua.Delay
stopDelay = PLua.StopDelay

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


function getAngle(posFrom,posTo)  
    local tmpx=posTo.x - posFrom.x
    local tmpy=posTo.y - posFrom.y
    local angle= math.atan2(tmpy,tmpx)*(180/math.pi)
    return angle
end

function printTable(tbl)	print(tojson(tbl)) end

function string:split(s, delimiter)
    result = {};
    for match in (s..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

function split(s, delimiter)
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

function luaGC()
  -- local c=collectgarbage("count")
  -- print("begin gc ="..tostring(c).." ")
  collectgarbage("collect")
  c=collectgarbage("count")
  print(" gc end ="..tostring(c).." ")
end

function make_array (tp,tbl)
    local arr = tp[#tbl]
    for i,v in ipairs(tbl) do
        arr:SetValue(v,i-1)
    end
    return arr
end

--value type
-- require("core.Math")
-- require("core.Vector3")
-- require("core.Vector2")
-- require("core.Quaternion")
-- require("core.Vector4")
-- require("core.Raycast")
-- require("core.Color")
-- require("core.Touch")
-- require("core.Ray")

