------------------------------------------------
--  Copyright © 2013-2024   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local pairs = pairs
local ipairs = ipairs
local string_format = string.format
local table_insert = table.insert
local table_remove = table.remove
local setmetatable = setmetatable
local class = class

-- 定义双向链表节点
local Node = {}
Node.__index = Node

function Node:new(key, value)
    local node = {
        key = key,
        value = value,
        prev = nil,
        next = nil
    }
    setmetatable(node, Node)
    return node
end

local lru_cache =
    class(
        function(self, capacity)
            self._capacity = capacity or 10
            self._size = 0
            self.map = {}       -- 缓存节点
            self.node_pool = {} -- 节点池
        end
    )

---从池子里面获取节点
local _get_empty_node = function(self, k, v)
    local node_pool = self.node_pool
    if #node_pool > 0 then
        local node = table_remove(node_pool, 1)
        node.key = k
        node.value = v
        return node
    else
        return Node:new(k, v)
    end
end

local _release_node = function(self, node)
    node.key = nil
    node.value = nil
    node.prev = nil
    node.next = nil
    local node_pool = self.node_pool
    table_insert(node_pool, #node_pool + 1, node)
end


---添加链表尾部
---@overload fun(key:string,val:any)
---@return void
local _add_to_tail = function(self, node)
    if self.head == nil then
        self.head = node
        self.tail = node
    else
        --将node添加到tail后面
        self.tail.next = node
        node.prev = self.tail
        self.tail = node
    end
    self._size = self._size + 1
end

---删除节点
---@overload fun(key:string)
---@return void
local _remove_node = function(self, node)

    if node == nil then
        return
    end

    local head = self.head
    if self.head == node and self.tail == node then --如果只有一个节点
        self.head = nil
        self.tail = nil
    elseif self.head == node then  --如果是头部节点
        self.head = head.next
        self.head.prev = nil
        head.next = nil
    elseif self.tail == node then --如果是尾部节点
        self.tail = node.prev
        self.tail.next = nil
        node.prev = nil
    else
        node.prev.next = node.next
        node.next.prev = node.prev
    end
    self._size = self._size - 1
end

---删除头部节点
---@overload fun()
---@return any 返回缓存的值
local _remove_head = function(self)
    if self.head == nil then
        return nil
    else
        local head = self.head
        if self.head == self.tail then
            self.head = nil
            self.tail = nil
        else
            self.head = head.next
            self.head.prev = nil
            head.next = nil
        end
        self._size = self._size - 1
        return head
    end
end

---将节点移动到尾部
---@overload fun(key:string)
---@return void
local _move_to_tail = function(self, node)
    if self.tail == node then
        return
    end

    if self.head == node then
        self.head = node.next
        self.head.prev = nil
    else
        node.prev.next = node.next
        node.next.prev = node.prev
    end

    node.prev = self.tail
    node.next = nil
    self.tail.next = node
    self.tail = node
end

---获取缓存的值
---@overload fun(key:string)
---@return any 返回缓存的值
local get = function(self, key)
    local node = self.map[key]
    if not node then
        return nil
    end
    _move_to_tail(self, node)
    return node.value
end

---设置缓存的值
---@overload fun(key:string,val:any)
---@return void
local put = function(self, key, value)
    local node = self.map[key]
    local rem_key, rem_val
    if node then
        node.key = key
        node.value = value
        _move_to_tail(self, node)
    else
        node = _get_empty_node(self, key, value) --Node:new(key, value)
        self.map[key] = node
        _add_to_tail(self, node)
        if self._size > self._capacity then
            local tail = _remove_head(self)
            if tail then
                rem_key = tail.key
                rem_val = tail.value
                self.map[rem_key] = nil
                _release_node(self, tail)
            end
        end
    end

    return rem_key, rem_val
end

local remove = function(self, key)
    local node = self.map[key]
    if node then
        _remove_node(self, node)
        self.map[key] = nil
        return node.key, node.value
    end
    return nil
end

local count = function(self)
    return self._size
end

local function debug_lru(self, str)
    local str = str and str.. " lru_cache:\n" or "lru_cache:\n"
    --遍历链表head到tail
    str = str .. "list:" .. self:count() .. "\n"
    local head = self.head
    while head do
        str = str .. string_format("%s=%s", head.key, head.value) .. " ,"
        head = head.next
    end

    str = str .. "\n map:"

    for k, v in pairs(self.map) do
        str = str .. string_format("%s=%s", k, v.value) .. " ,"
    end
    Logger.Log(str)
end

lru_cache.Get = get
lru_cache.Put = put
lru_cache.Remove = remove
-- lru_cache.RemoveAll = remove_all
lru_cache.GetCount = count
lru_cache.Debug = debug_lru
---LRUCache 实现
---@class LRUCache
---@field Get fun(self:LRUCache,key:string):any   绑定数据
---@field Put fun(self:LRUCache,key:string,value:any):any 设置gameobject可见性
---@field Remove fun(self:LRUCache,key:string):any 设置gameobject可见性
---@field GetCount fun(self:LRUCache):number 设置gameobject可见性
---@field Debug fun(self:LRUCache,str:string):void 设置gameobject可见性
LRUCache = lru_cache
