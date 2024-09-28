
local path = package.path
-- print("path", path)
print(jit.version,jit.arch)


local function convert_units (input)
    if type(input) ~= "table" then
        return input
    else
        if input.unit == "cm" then
            return input.amount * 10
        else
            return input.amount
        end
    end
end

local mt = {
    __lt = function (self, other)
        -- print("self", self,other)
        return convert_units(self) < convert_units(other)
    end
}

local a = {
    amount = 1.2,
    unit = "cm"
}

local b = {
    amount = 14,
    unit = "mm"
}

setmetatable(a, mt)
setmetatable(b, mt)

print(a < b)

print (a < 13)


local ffi = require("ffi")

-- 定义 C 函数原型
ffi.cdef[[
    int printf(const char *fmt, ...);
]]

-- 调用 C 函数
ffi.C.printf("Hello, %s!\n", "world")

-- 定义 C 结构体
ffi.cdef[[
    typedef struct {
        int x;
        int y;
    } Point;
]]

-- 创建并使用 C 结构体
local point = ffi.new("Point")
point.x = 10
point.y = 20
print("Point:",type(point), type(point.x),point.x, point.y)


-- 定义一个 C 结构体，其中包含一个 long 类型的字段
ffi.cdef[[
typedef struct {
    long value;
} long_struct;
]]

local myLong = ffi.new("long", 987654321)
print("The long value is:", myLong - point.x,myLong <= point.x , myLong>=10,type(myLong))

-- print (a < myLong)
local int63 = 9223372036854775807
print(int63, type(int63))

print(myLong < int63)

print(10000<100)
print(myLong < a)