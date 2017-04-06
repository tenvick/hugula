function string.split(s, delimiter)
    result = {};
    for match in (s..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

function table.clear(tb)
  for k,v in pairs(tb) do
    tb[k] = nil
  end
end

function math.randomseed1(i)
    math.randomseed(tostring(os.time()+tonumber(i)):reverse():sub(1, 6)) 
end

--常用数据结构

--[[
 正的索引指的是栈上的绝对位置（从一开始）；负的索引则指从栈顶开始的偏移量。 
 更详细的说明一下，如果堆栈有 n 个元素， 那么索引 1 表示第一个元素（也就是
 最先被压入堆栈的元素） 而索引 n 则指最后一个元素； 索引 -1 也是指最后一个
 元素（即栈顶的元素）， 索引 -n 是指第一个元素。如果索引在 1 到栈顶之间（
 也就是，1 ≤ abs(index) ≤ top） 我们就说这是个有效的索引。
--]]
LuaStack = {}
LuaStack.__index = LuaStack
local _mt = {}
_mt.__call = function(tb,size)
	return LuaStack.new(size)
end

setmetatable(LuaStack,_mt)

function LuaStack.new(size)
	if size == nil then size = 16 end
	local o = {data={},size = size}
	setmetatable(o,LuaStack)
	return o
end

function LuaStack:push(v)
	table.insert(self.data,v)
	local top = self:get_top()
	if top > self.size then table.remove(self.data,1) end -- 确保长度
end

function LuaStack:get(i) --获取 i 位置 -1 栈顶， 
	local top = self:get_top() --长度
	local pos = 0 --位置
	if i < 0 then --从顶部取值
		pos = top + i + 1 
	elseif i > 0 then
		pos = i
	end
    if pos > 0 and pos <= top then
        return self.data[pos]
    else
        return nil
    end
end

function LuaStack:get_top() --得到长度 
	local top = #self.data
  	return top
end

--从堆栈中移除出 n 个元素 返回最后一个移除的元素
function LuaStack:pop(i) --从顶部取出第 i 个 
	local top,re = 0,nil
    while (i>0) do
    	top = self:get_top()
    	if top <= 0 then break end
    	re = table.remove(self.data,top)
    	i = i - 1
    end
    return re
end

------------------------------------------quadtree------------------------------------
--矩形min.xy max.xy
local function get_rect_rang(topx,topy,width,height)
	return topx , topy - height+1,topx + width-1,topy 
	-- return cx - half_width,cy - half_height,cx + half_width,cy + half_height
end

--两个矩形的交集
--第一个矩形左下角bottom_x1,bottom_y1，右上角top_x1,top_y1，第二个左下bottom_x2,bottom_y2,右上top_x2,top_y2
--1 当top_x1<bottom_x2 or top_x2<bottom_x1 or top_y2<bottom_y1 or bottom_y2>top_y1 没有交集
--2 除去1的情况后交集为  (min(top_y2,top_y1)-max(bottom_y1,bottom_y2))*(min(top_x2,top_x1)-max(bottom_x1,bottom_x2))
local function get_cross_rect(bottom_x1,bottom_y1,top_x1,top_y1,bottom_x2,bottom_y2,top_x2,top_y2)
	local re = {}
	if top_x1<bottom_x2 or top_x2<bottom_x1 or top_y2<bottom_y1 or bottom_y2>top_y1 then
		return re
	else
		local bx,ex,by,ey =math.max(bottom_x1,bottom_x2),math.min(top_x2,top_x1),math.max(bottom_y1,bottom_y2),math.min(top_y2,top_y1)
		for x = bx,ex do
			for y = by,ey do
				local key = string.format("%d_%d",x,y)
				re[key] = true
			end
		end
	end

	return re
end

-- 按照位置存储对象
QuadTree = {}
QuadTree.__index = QuadTree
local _mt = {}
_mt.__call = function(tb,w,h,lv,off_x,off_y)
	return QuadTree.new(w,h,lv,off_x,off_y)
end
--
setmetatable(QuadTree,_mt)

--注意 off_x轴 -1向左
--		off_y轴 +1向上
-- 0，0点在左下角
--w格子宽度(level) h格子高度(level) lv级别(世界坐标/level 4的n次方) off_x(相对于w) ,off_y (相对于h) 
function QuadTree.new(w,h,lv,off_x,off_y)
	if w == nil then w = 8 end
	if h == nil then h = 8 end
	if lv == nil then lv = 4 end
	if off_x == nil then off_x = 0 end
	if off_y == nil then off_y = 0 end

	local o = {width = w,height = h,data = {},level = lv,offset_y=off_y,offset_x = off_x}
	o.half_width,o.half_height = math.floor(w/2),math.floor(h/2)
	setmetatable(o,QuadTree)
	return o
end

function QuadTree:set_size(w,h)
	self.width = w
	self.height = h
end

--x,y 世界坐标 v对象
function QuadTree:push(x,y,v)
	local level = self.level
	local tx,ty = math.floor(x/level),math.floor(y/level)
	local key = string.format("%d_%d",tx,ty)
	local data = self.data
	if data[key] == nil then data[key] = {} end
	local leaf = data[key]
	local i = table.index_of(leaf,v)
	if i == -1  then
		table.insert(leaf,v)
	end
end

--x,y 世界坐标 v对象
function QuadTree:remove(x,y,v)
	local level = self.level
	local tx,ty = math.floor(x/level),math.floor(y/level)
	local key = string.format("%d_%d",tx,ty)
	local data = self.data
	if data[key] == nil then data[key] = {} end
	local leaf = data[key]
	local i = table.index_of(leaf,v)
	if i>-1 then
		table.remove(leaf,i)
	end
end

--返回 x/level,y/level 坐标周围radius半径的所有对象
function QuadTree:get_objects(x,y,radius)
	local level = self.level
	local tx,ty = math.floor(x/level),math.floor(y/level)

	local ret = {} --数据
	local key
	local data = self.data
	local leaf
	for i = tx - radius,tx + radius do
		for j = ty - radius,ty + radius do
			key = string.format("%d_%d",i,j)
			leaf = data[key]
			if leaf then
				for k,v in ipairs(leaf) do
					table.insert(ret,v)
				end
			end
		end
	end

	return ret
end

function QuadTree:clear()
	table.clear(self.data)
end

function QuadTree:offset(off_x,off_y) --相对于中心点偏移
	self.offset_x = off_x
	self.offset_y = off_y
end

function QuadTree:_move_rect(x,y) --目标矩形
	-- 求矩形的坐标min.xy max.xy
	self.bottom_left_x,self.bottom_left_y,self.top_right_x,self.top_right_y = get_rect_rang(x,y,self.width,self.height)
end

function QuadTree:__tostring()
	if self.bottom_left_x and self.bottom_left_y and self.top_right_x and self.top_right_y then
		return string.format("quadTree(%s)x[%d:%d],y[%d:%d];",tostring(self.data),self.bottom_left_x,self.top_right_x,self.bottom_left_y,self.top_right_y)
	else
		return  string.format("quadTree(%s)",tostring(self.data))
	end
end

function QuadTree:check_in_visible(x,y)
	local level = self.level
	local tx,ty = math.floor(x/level),math.floor(y/level) --四*四个格子为一组
	local x1,y1 = self.bottom_left_x,self.bottom_left_y
	local x2,y2 = self.top_right_x,self.top_right_y
	-- print("check_in_visible",tx,ty)
	if x1 == nil or y1 == nil or x2 == nil or y2 == nil then return false end
	local re = x1 <= tx and x2 >= tx and y1 <= ty and y2 >= ty
	return re
end
	

function QuadTree:get_change(x1,y1,x2,y2) --return {remove,add}
	local remove,add = {},{}
	if x1 == nil then x1 = self._last_x or -x2-100 end --如果没有
	if y1 == nil then y1 = self._last_y or -y2-100 end

	local level = self.level
	local hf_width,hf_height = self.half_width,self.half_height
	local c_x1,c_y1 = math.floor((x1+self.offset_x)/level)-hf_width,math.floor((y1+self.offset_y)/level)+hf_height --第一个top点
	local c_x2,c_y2 = math.floor((x2+self.offset_x)/level)-hf_width,math.floor((y2+self.offset_y)/level)+hf_height--第二个top点
	self:_move_rect(c_x2,c_y2) --移动区域中心点

	-- 求矩形的坐标min.xy max.xy
	local bottom_left_x,bottom_left_y,top_right_x,top_right_y = get_rect_rang(c_x1,c_y1,self.width,self.height)
	local cross = get_cross_rect(bottom_left_x,bottom_left_y,top_right_x,top_right_y,self.bottom_left_x,self.bottom_left_y,self.top_right_x,self.top_right_y)
	
	self._last_x = x2
	self._last_y = y2

	--remove
	local bx,ex,by,ey = bottom_left_x,top_right_x,bottom_left_y,top_right_y
	local leaf
	local data = self.data
	-- print(string.format("source sizex(%d,%d),sizey(%d,%d)",bx,ex,by,ey))
	for x = bx,ex do
		for y = by,ey do
			local key = string.format("%d_%d",x,y)
			-- re[key] = true
			leaf = data[key]
			if not cross[key] and leaf then --
				-- print("remove "..key)
				for k,v in pairs(leaf) do
					table.insert(remove,v)
				end
			else
				-- print("keep "..key)
			end
		end
	end

	--add
	bx,ex,by,ey = self.bottom_left_x,self.top_right_x,self.bottom_left_y,self.top_right_y
	-- print(string.format("target sizex(%d,%d),sizey(%d,%d)",bx,ex,by,ey))
	for x = bx,ex do
		for y = by,ey do
			local key = string.format("%d_%d",x,y)
			-- re[key] = true
			leaf = data[key]
			if not cross[key] and leaf then --
				for k,v in pairs(leaf) do
					table.insert(add,v)
				end
			end
		end
	end

	return remove,add
end


------------------------------------------end quadtree------------------------------------