--常用数据结构

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