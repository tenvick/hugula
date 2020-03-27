--常用数据结构
local table = table
local string = string
local math = math
local math_max = math.max
local math_min = math.min
local setmetatable = setmetatable
local pairs = pairs
------------------------------------------quadtree------------------------------------
---矩形min.xy max.xy
---@overload fun(topx:number,topy:number,width:number,height:number)
---@param topx number
---@param topy number
---@param width number
---@param height number
---@return number
local function get_rect_rang(topx, topy, width, height)
    return topx, topy - height + 1, topx + width - 1, topy
end

---两个矩形的交集
---第一个矩形左下角bottom_x1,bottom_y1，右上角top_x1,top_y1，第二个左下bottom_x2,bottom_y2,右上top_x2,top_y2
---1 当top_x1<bottom_x2 or top_x2<bottom_x1 or top_y2<bottom_y1 or bottom_y2>top_y1 没有交集
---2 除去1的情况后交集为  (min(top_y2,top_y1)-max(bottom_y1,bottom_y2))*(min(top_x2,top_x1)-max(bottom_x1,bottom_x2))
local function get_cross_rect(bottom_x1, bottom_y1, top_x1, top_y1, bottom_x2, bottom_y2, top_x2, top_y2)
    local re = {}
    if top_x1 < bottom_x2 or top_x2 < bottom_x1 or top_y2 < bottom_y1 or bottom_y2 > top_y1 then
        return re
    else
        local bx, ex, by, ey =
            math_max(bottom_x1, bottom_x2),
            math_min(top_x2, top_x1),
            math_max(bottom_y1, bottom_y2),
            math_min(top_y2, top_y1)
        for x = bx, ex do
            for y = by, ey do
                local key = string.format("%d_%d", x, y)
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
_mt.__call = function(tb, w, h, lv, off_x, off_y)
    return QuadTree.new(w, h, lv, off_x, off_y)
end
--
setmetatable(QuadTree, _mt)

---注意 off_x轴 -1向左
---		off_y轴 +1向上
--- 0，0点在左下角
---w格子宽度(level) h格子高度(level) lv级别(世界坐标/level 4的n次方) off_x(相对于w) ,off_y (相对于h)
function QuadTree.new(w, h, lv, off_x, off_y)
    if w == nil then
        w = 8
    end
    if h == nil then
        h = 8
    end
    if lv == nil then
        lv = 4
    end
    if off_x == nil then
        off_x = 0
    end
    if off_y == nil then
        off_y = 0
    end

    local o = {width = w, height = h, data = {}, level = lv, offset_y = off_y, offset_x = off_x}
    o.half_width, o.half_height = math.floor(w / 2), math.floor(h / 2)
    setmetatable(o, QuadTree)
    return o
end

function QuadTree:set_size(w, h)
    self.width = w
    self.height = h
end

--x,y 世界坐标 v对象
function QuadTree:push(x, y, v)
    local level = self.level
    local tx, ty = math.floor(x / level), math.floor(y / level)
    local key = string.format("%d_%d", tx, ty)
    local data = self.data
    if data[key] == nil then
        data[key] = {}
    end
    local leaf = data[key]
    local i = table.index_of(leaf, v)
    if i == -1 then
        table.insert(leaf, v)
    end
end

--x,y 世界坐标 v对象
function QuadTree:remove(x, y, v)
    local level = self.level
    local tx, ty = math.floor(x / level), math.floor(y / level)
    local key = string.format("%d_%d", tx, ty)
    local data = self.data
    if data[key] == nil then
        data[key] = {}
    end
    local leaf = data[key]
    local i = table.index_of(leaf, v)
    if i > -1 then
        table.remove(leaf, i)
    end
end

--返回 x/level,y/level 坐标周围radius半径的所有对象
function QuadTree:get_objects(x, y, radius)
    local level = self.level
    local tx, ty = math.floor(x / level), math.floor(y / level)

    local ret = {} --数据
    local key
    local data = self.data
    local leaf
    for i = tx - radius, tx + radius do
        for j = ty - radius, ty + radius do
            key = string.format("%d_%d", i, j)
            leaf = data[key]
            if leaf then
                for k, v in ipairs(leaf) do
                    table.insert(ret, v)
                end
            end
        end
    end

    return ret
end

function QuadTree:clear()
    table.clear(self.data)
end

function QuadTree:offset(off_x, off_y) --相对于中心点偏移
    self.offset_x = off_x
    self.offset_y = off_y
end

function QuadTree:_move_rect(x, y) --目标矩形
    -- 求矩形的坐标min.xy max.xy
    self.bottom_left_x, self.bottom_left_y, self.top_right_x, self.top_right_y =
        get_rect_rang(x, y, self.width, self.height)
end

function QuadTree:__tostring()
    if self.bottom_left_x and self.bottom_left_y and self.top_right_x and self.top_right_y then
        return string.format(
            "quadTree(%s)x[%d:%d],y[%d:%d];",
            tostring(self.data),
            self.bottom_left_x,
            self.top_right_x,
            self.bottom_left_y,
            self.top_right_y
        )
    else
        return string.format("quadTree(%s)", tostring(self.data))
    end
end

function QuadTree:check_in_visible(x, y)
    local level = self.level
    local tx, ty = math.floor(x / level), math.floor(y / level) --四*四个格子为一组
    local x1, y1 = self.bottom_left_x, self.bottom_left_y
    local x2, y2 = self.top_right_x, self.top_right_y
    -- print("check_in_visible",tx,ty)
    if x1 == nil or y1 == nil or x2 == nil or y2 == nil then
        return false
    end
    local re = x1 <= tx and x2 >= tx and y1 <= ty and y2 >= ty
    return re
end

function QuadTree:get_change(x1, y1, x2, y2) --return {remove,add}
    local remove, add = {}, {}
    if x1 == nil then
        x1 = self._last_x or -x2 - 100
    end --如果没有
    if y1 == nil then
        y1 = self._last_y or -y2 - 100
    end

    local level = self.level
    local hf_width, hf_height = self.half_width, self.half_height
    local c_x1, c_y1 =
        math.floor((x1 + self.offset_x) / level) - hf_width,
        math.floor((y1 + self.offset_y) / level) + hf_height --第一个top点
    local c_x2, c_y2 =
        math.floor((x2 + self.offset_x) / level) - hf_width,
        math.floor((y2 + self.offset_y) / level) + hf_height
    --第二个top点
    self:_move_rect(c_x2, c_y2) --移动区域中心点

    -- 求矩形的坐标min.xy max.xy
    local bottom_left_x, bottom_left_y, top_right_x, top_right_y = get_rect_rang(c_x1, c_y1, self.width, self.height)
    local cross =
        get_cross_rect(
        bottom_left_x,
        bottom_left_y,
        top_right_x,
        top_right_y,
        self.bottom_left_x,
        self.bottom_left_y,
        self.top_right_x,
        self.top_right_y
    )

    self._last_x = x2
    self._last_y = y2

    --remove
    local bx, ex, by, ey = bottom_left_x, top_right_x, bottom_left_y, top_right_y
    local leaf
    local data = self.data
    -- print(string.format("source sizex(%d,%d),sizey(%d,%d)",bx,ex,by,ey))
    for x = bx, ex do
        for y = by, ey do
            local key = string.format("%d_%d", x, y)
            -- re[key] = true
            leaf = data[key]
            if not cross[key] and leaf then --
                -- print("remove "..key)
                for k, v in pairs(leaf) do
                    table.insert(remove, v)
                end
            else
                -- print("keep "..key)
            end
        end
    end

    --add
    bx, ex, by, ey = self.bottom_left_x, self.top_right_x, self.bottom_left_y, self.top_right_y
    -- print(string.format("target sizex(%d,%d),sizey(%d,%d)",bx,ex,by,ey))
    for x = bx, ex do
        for y = by, ey do
            local key = string.format("%d_%d", x, y)
            -- re[key] = true
            leaf = data[key]
            if not cross[key] and leaf then --
                for k, v in pairs(leaf) do
                    table.insert(add, v)
                end
            end
        end
    end

    return remove, add
end

------------------------------------------end quadtree------------------------------------
---

local function left(i)
    return 2 * i
end

local function right(i)
    return 2 * i + 1
end

local function parent(i)
    return math.floor(i / 2)
end
---------------------------------------------heap sort------------------------------------
--- 		parent(i)
---		leaf(2*i)  right(2*i)+1
---最大堆 parent > left or right
---@overload fun(table:table,i:int)
local function max_heap(heap_size, a, i)
    local i = math.floor(i)
    local l = left(i)
    local r = right(i)
    local size = heap_size
    local largest

    if l <= size and a[l] > a[i] then ---左值比较
        largest = l
    else
        largest = i
    end

    if r <= size and a[r] > a[largest] then
        largest = r
    end

    if largest ~= i then
        local ai = a[i]
        a[i] = a[largest]
        a[largest] = ai
        max_heap(heap_size, a, largest)
    end
end

--- 构建最大堆，从数组中间开始构建
---@overload fun(a:table)
local function build_max_heap(self, a)
    local heap_size = #a
    local half = math.floor(#a / 2)
    for i = half, 1, -1 do
        max_heap(heap_size, a, i)
    end
end

---堆排序
---1先构建最大堆，这时候a[1]为最大值
---2因为a[1]为最大值，所以只需要替换a[i]为a[1]该元素就能放到正确的值，同时减小heap_size,剩余节点任然满足最大堆，一直循环直到size为heap_size -1
local heapsort = function(a)
    build_max_heap(nil, a)
    local heap_size = #a
    for i = #a, 2, -1 do
        local a1 = a[1]
        a[1] = a[i]
        a[i] = a1
        heap_size = heap_size - 1
        max_heap(heap_size, a, 1)
    end
end

local _heap = {}
_heap.heapsort = heapsort
_heap.max_heap = max_heap
_heap.build_max_heap = build_max_heap

_heap.__call = function(tb, a)
    local h = {}
    setmetatable(h, _heap)
    heapsort(a)
end

-- Heap = setmetatable({}, _heap)

-- local A = {4, 5, 6, 8, 9, 0, 1, 2, 3, 14, 15, 16, 7, 18, 19, 10}
-- local str = ""
-- local h1 = Heap(A)
-- Logger.Log("排序后")
-- for k, v in ipairs(A) do
--     str = str .. string.format("%s,", v)
-- end
-- Logger.Log(str)
---------------------------------------------quick sort------------------------------------
-----
---
--- A[p-------q--------------r] 数组a[p,q-1]<a[q] and a[q]<a[q+1,r]
---将小于A[r]的值放左边，大于的放右边 最后返回 q的位置
local function partition(A, p, r)
    local x = A[r]
    local i = p - 1
    for j = p, r - 1 do
        if A[j] <= x then
            i = i + 1
            local aj = A[j] --交换 i j的值
            A[j] = A[i]
            A[i] = aj
        end
    end
    A[r] = A[i + 1]
    A[(i + 1)] = x
    return i + 1
end

local function quick_sort(A, p, r)
    if p < r then
        local q = partition(A, p, r)
        quick_sort(A, p, q - 1)
        quick_sort(A, q + 1, r)
    end
end

-- QuickSort = quick_sort

-- local A = {4, 5, 6, 8, 9, 0, 1, 2, 3, 14, 15, 16, 7, 18, 19, 10}
-- local A = {1, 1, 1,1,1,1,1,1,1,1}
-- local str = ""
-- quick_sort(A, 1, #A)
-- Logger.Log("排序后")
-- for k, v in ipairs(A) do
--     str = str .. string.format("%s,", v)
-- end
-- Logger.Log(str)

----------------------------------------binary search tree--------------------------
---对于任意x节点，左子树的值不大于<= x的值，右子树的值不低于>=x的值。 Left(i)<=parent(i)<=right(i)

local function inorder_tree_walk(x)
    if x ~= nil then
        inorder_tree_walk(x.left)
        inorder_tree_walk(x.right)
        print(x.key)
    end
end

local function tree_search(x, k)
    if x == nil or k == x.key then
        return x
    end
    if k < x.key then
        return tree_search(x.left, k)
    else
        return tree_search(x.right, k)
    end
end

local function iterave_tree_search(x, k)
    local i = 1
    while x ~= nil and k ~= x.key do
        if k < x.key then
            x = x.left
        else
            x = x.right
        end
    end
    return x
end

local function tree_minmum(x)
    while x.left ~= nil do
        x = x.left
    end
    return x.key
end

local function tree_maxmum(x)
    while x.right ~= nil do
        x = x.right
    end
    return x.key
end

local function tree_insert(T, z)
    local y = nil
    local x = T.root

    while x ~= nil do ---遍历原始数组T找到z的查入点
        y = x
        if z.key < x.key then
            x = x.left
        else
            x = x.right
        end
    end

    z.p = y

    if y == nil then --如果T为nil
        T.root = z
    elseif z.key < y.key then
        y.left = z
    else
        y.right = z
    end
end

---以v为根的子树替换u为根的子树，最后v成为u的双亲的孩子。
local function transplant(T,u,v)
    if u.p == nil then
        T.root = v
    elseif u == u.p.left then
        u.p.left = v
    else
        u.p.right = v
    end
    if v ~= nil then
        v.p = u.p
    end
end

local function tree_delete(T,z)
    if z.left == nil then
        transplant(T,z,z.right)
    elseif z.right == nil then
        transplant(T,z,z.left)
    else
       local y = tree_minmum(z.right)
        if y.p ~= z then
            transplant(T,y,y.right)
            y.right = z.right
            y.right.p = y
        end
        transplant(T,z,y)
        y.left = z.left
        y.left.p = y
    end
end

local function build_binary_tree(A, z)
    for k, v in ipairs(z) do
        tree_insert(A, {key = v, left = nil, right = nil})
    end
end

local A = {4, 2, 5, 3, 1, 6, 8, 9}
local T = {}
-- build_binary_tree(T, A)

-- inorder_tree_walk(T.root)
-- print("min", tree_minmum(T.root))
-- print("max", tree_maxmum(T.root))
-- print("search 3=", iterave_tree_search(T.root,3).key)
