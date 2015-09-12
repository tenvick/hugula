--[[
-- 时间cd函数 2013-05-10编写
--]]--

local Timer = {}
Timer.__index = Timer

local function new()
    return setmetatable({functions = {}}, Timer)
end
	-- 每次减少一个
-- 间隔,这个timer效率不高
function Timer:update(dt)
    local to_remove = {}
	-- 更新时间
	for func, delay in pairs(self.functions) do
        delay = delay - dt	-- 每次减少一个
        if delay <= 0 then
            to_remove[#to_remove+1] = func
        end
        self.functions[func] = delay
    end
    --小于或等于0的时候删除并调用
    for _,func in ipairs(to_remove) do
        self.functions[func] = nil
        func(func)
    end
end

-- 多少秒之后调用 
function Timer:add(delay, func)
    assert(not self.functions[func], "Function already scheduled to run.")
    self.functions[func] = delay
    return func
end


--高级货，每隔多少秒调用一次.
function Timer:add_periodic(delay, func, count)
    local count = count or math.huge -- exploit below: math.huge - 1 = math.huge

-- 把(构造调用)函数添加到timer中,每次调用再构造一次
    return self:add(delay, function(f)
	--	debug_print("add timer delay : " .. delay);
        if func(func) == false then return end
        count = count - 1
        if count > 0 then
            self:add(delay, f)
        end
    end)
end

function Timer:cancel(func)
--	for i, v in pairs(self.functions) do
--		print("function i " .. tostring(i))
--		print("function v " .. v)
--		print("function funct " .. tostring(func))
--	end
    self.functions[func] = nil
end


function Timer:clear()
    self.functions = {}
end

-- 返回一个新的table,以免外面操作错误影响里面self.functions(暂时无用)
function Timer:func_list()
    local list = {}
	-- 更新时间
	for func, delay in pairs(self.functions) do
		list[func] = delay;
    end
	return list;
end

local function inter_polator(length, func)
    local t = 0
    return function(dt, ...)
        t = t + dt
        return t <= length and func((t-dt)/length, ...) or nil
    end
end

local function Oscillator(length, func)
    local t = 0
    return function(dt, ...)
        t = (t + dt) % length
        return func(t/length, ...)
    end
end

-- default timer
local default = new()

-- the module
return setmetatable({
    new = new,
    update = function(...) return default:update(...) end,
    add = function(...) return default:add(...) end,
    add_periodic = function(...) return default:add_periodic(...) end,
    cancel = function(...) return default:cancel(...) end,
    clear = function(...) return default:clear(...) end,
    inter_polator = inter_polator,
    Oscillator = Oscillator
}, {__call = new})

