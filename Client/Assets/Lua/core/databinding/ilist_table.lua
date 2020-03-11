------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local type = type
local table_insert = table.insert
local table_remove = table.remove
local table_remove_item = table.remove_item
local string_format = string.format
local ipairs = ipairs
local pairs = pairs
local class = class

local ilist_table =
    class(
    function(self, items)
        self.items =  items or {}
        self.Count = #self.items
        self.IsReadOnly = false
        self.IsFixedSize = false
    end
)

local get_item = function(self, index)
    return self.items[index + 1]
end

local function set_count(self)
    self.Count = #self.items
end

---设置index索引处的元素item
---@overload fun(index:int,item:any):void
---@param index int
---@param item any
local function set_item(self, index, item)
    if index == nil then
        error("Argument index is nil")
    end
    local items = self.items
    local size = #items
    index = index + 1
    if index < 1 or index > size then
        error("Argument index Out of Range")
    end

    local old = items[index]
    items[index] = item
end

---从索引处删除count个元素 count默认为1
---@overload fun(index:index,count:int):void
---@param index index
---@param count int
local function remove_at(self, index, count)
    if count == nil then
        count = 1
    end

    index = index + 1
    local items = self.items
    local size = #items

    local end_idx = index + count - 1

    if end_idx > size then
        end_idx = size
    end

    local changedItems = {}
    local item
    for i = end_idx, index, -1 do
        item = table_remove(items, i)
        table_insert(changedItems, item)
    end

    set_count(self)
end

local function indexof(self, item)
    local idx = table.indexof(self.items, item)
    if idx == nil then
        return -1
    else
        return idx
    end
end

local function contains(self, item)
    local idx = table.indexof(self.items, item)
    if idx ~= nil then
        return true
    else
        return false
    end
end

---在索引index插入一个元素,index默认为size。
---@overload fun(index:int,item:any):void
---@param index int
---@param item any
local function insert_item(self, index, item)
    if item == nil then
        error("Argument item is nil")
    end

    table_insert(self.items, index + 1, item)

    set_count(self)
end

local function add(self, item)
    if item == nil then
        error("Argument item is nil")
    end

    insert_item(self, #self.items, item)
end

---交换old_index和new_index的数据
---@overload fun(old_index:int,new_index:int):void
---@param old_index int
---@param new_index int
local function move_item(self, old_index, new_index)
    if old_index == nil or new_index == nil then
        error("Argument old_index or new_index is nil")
    end

    local items = self.items
    local size = #items
    old_index = old_index + 1
    new_index = new_index + 1
    if old_index < 1 or old_index > size then
        error("Argument old_index Out of Range")
    end
    if new_index < 1 or new_index > size then
        error("Argument new_index Out of Range")
    end

    local old = items[old_index]
    local new = items[new_index]
    items[old_index] = new
    items[new_index] = old

    set_count(self)
end

---删除index处的元素
---@overload fun(index:int):void
---@param index int
local function remove_item(self, index)
    if index == nil then
        error("Argument index is nil")
    end
    local items = self.items
    local size = #items
    index = index + 1
    if index < 1 or index > size then
        error("Argument index Out of Range")
    end

    local old = table_remove(items, index)

    set_count(self)
end

local function clear(self)
    table.clear(self.items)
    set_count(self)
end

local function tostring(self)
    return string_format("IListTable(%s)", self.items)
end

------IList
ilist_table.set_Item = set_item --object this[int]
ilist_table.get_Item = get_item --this[int] = object
ilist_table.Add = add --int Add(object value);
ilist_table.Clear = clear --void Clear();
ilist_table.Contains = contains --bool Contains(object value);
ilist_table.IndexOf = indexof --int IndexOf(object value);
ilist_table.Insert = insert_item --void Insert(int index, object value);
ilist_table.Remove = remove_item --void Remove(object value);
ilist_table.RemoveAt = remove_at --void RemoveAt(int index);
ilist_table.__tostring = tostring

---Ilist的lua实现,在列表绑定不需要监听改变的时候可以使用这个数据结构
---@class IListTable
---@class IListTable
---@module Assets\Lua\core\databinding\ilist_table.lua
---@overload fun():IListTable
---@return NotifyTable
---@field Add   function
---@field Clear function
---@field Contains function
---@field IndexOf function
---@field RemoveAt function
---@field Insert function
---@field Remove function
---@field set_Item function
---@field get_Item function
IListTable = ilist_table
