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
local math_min = math.min

local class = class
local GetSetObject = GetSetObject
local IListTable = IListTable
local CS = CS
local Object = CS.System.Object
local Specialized = CS.System.Collections.Specialized
local BindingUtility = CS.Hugula.Databinding.CollectionChangedEventArgsUtility

local PropertyChangedEventHandlerEvent = CS.Hugula.Databinding.PropertyChangedEventHandlerEvent
local NotifyCollectionChangedEventHandlerEvent = CS.Hugula.Databinding.NotifyCollectionChangedEventHandlerEvent
--- 为了与C#一致，这里的索引都是从0开始
---集合改变通知事件
---@class NotifyCollectionChangedAction
---@field Add string
---@field Remove string
---@field Reset string
---@field Replace string
local NotifyCollectionChangedAction = Specialized.NotifyCollectionChangedAction

local notify_table =
    class(
    function(self, items)
        ---集合改变事件监听
        self.CollectionChanged =  NotifyCollectionChangedEventHandlerEvent()
        ---属性改变监听
        self.PropertyChanged = PropertyChangedEventHandlerEvent()
        ---
        self.items = items or {}
        self.Count = #self.items
        self.IsReadOnly = false
        self.IsFixedSize = false
        self.property = GetSetObject(self) --设置property getset

    end
)

---属性改变的时候通知绑定对象
---@overload fun(property_name:string)
---@return void
local function on_property_changed(self, property_name)
    self.PropertyChanged:Invoke(self,property_name)
end

---获取table或者list的长度
---@overload fun(property_name:string)
---@return void
local function get_list_length(range)
    local is_table = type(range) == "table"
    local size = 0
    if is_table then
        size = #range - 1
    else
        size = range.Count or range.Length
        size = size - 1
    end

    return size, is_table
end

---属性改变的时候通知绑定对象
---@overload fun(...)
---@param ... any
---@return void
local function on_collection_changed(self, arg)
    -- local changed = self.CollectionChanged
    -- for i = 1, #changed do
    --     changed[i](self, arg)
    -- end
    self.CollectionChanged:Invoke(self,arg)
    BindingUtility.Release(arg)
end

---改变属性
---@overload fun(property_name:string,value:any)
---@return void
local function set_property(self, property_name, value, force)
    local old = self[property_name]
    if old ~= value or force then
        self[property_name] = value
        on_property_changed(self, property_name)
    end
end

local add_property_changed = function(self, delegate)
    self.PropertyChanged:Add(delegate)
end

local remove_property_changed = function(self, delegate)
    self.PropertyChanged:Remove(delegate)
end

local function property_changed(self, op, delegate)
    if op == "+" then
        add_property_changed(self, delegate)
    else
        remove_property_changed(self, delegate)
    end
end

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

    on_property_changed(self, string_format("Item[%s]", index - 1))
    -- OnCollectionChanged (new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Replace, item, originalItem, index));
    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsNewItemOldItemIndex(
            NotifyCollectionChangedAction.Replace,
            item,
            old,
            index - 1
        )
    )
end

local add_collection_changed = function(self, delegate)
    self.CollectionChanged:Add(delegate)
end

local remove_collection_changed = function(self, delegate)
    self.CollectionChanged:Remove(delegate)
end

local function collection_changed(self, op, delegate)
    if op == "+" then
        add_collection_changed(self, delegate)
    else
        remove_collection_changed(self, delegate)
    end
end

---从索引处插入一个table数组
---索引是从0开始
---@overload fun(index:int,range:table):void
---@param range table
---@param index int
local function insert_range(self, index, range)
    local items = self.items
    local count = #items
    if type(index) == "table" or type(index) == "userdata" then
        range = index
        index = count
    end

    if index == nil then
        index = count
    end

    index = index + 1 --luatable从1开始
    if index >= count then
        index = count + 1
    end

    local size, is_table = get_list_length(range)

    local j = 0
    for i = 0, size do
        if is_table then
            j = i + 1
        else
            j = i
        end
        table_insert(items, index + i, range[j])
    end

    set_count(self)

    if is_table then
        range = IListTable(range)
    end

    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsChangedItemsStartingIndex(
            NotifyCollectionChangedAction.Add,
            range,
            index - 1
        )
    )
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
    count = #changedItems

    if count == 1 then
        on_property_changed(self, string_format("Item[%d]", index - 1))
        on_collection_changed(
            self,
            BindingUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Remove, item, index - 1)
            -- {action = NotifyCollectionChangedAction.Remove, changedItem = item, index = index - 1}
        )
    else
        -- on_collection_changed(self, {action = NotifyCollectionChangedAction.Remove, changedItems = changedItems})
        on_property_changed(self, string_format("Item[%d]", index - 1))
        on_collection_changed(
            self,
            BindingUtility.CreateCollectionArgsChangedItems(
                NotifyCollectionChangedAction.Remove,
                IListTable(changedItems)
            )
        )
    end
end

---删除range中的元素
---@overload fun(range:table):void
---@param range table
local function remove_range(self, range)
    local items = self.items
    local size, is_table = get_list_length(range)

    local removes = {}
    -- local indexs = {}
    local min_idx = size

    local j = 0
    for i = 0, size do
        if is_table then
            j = i + 1
        else
            j = i
        end
        local v = range[j]
        local del_idx = table_remove_item(items, v)
        if del_idx ~= nil and del_idx > 0 then
            -- table_insert(indexs, del_idx)
            min_idx = math_min(min_idx, del_idx)
            table_insert(removes, v)
        end
    end

    set_count(self)

    if #removes > 0 then
        on_collection_changed(
            self,
            BindingUtility.CreateCollectionArgsChangedItems(
                NotifyCollectionChangedAction.Remove,
                IListTable(removes),
                min_idx - 1
            )
            -- {
            --     action = NotifyCollectionChangedAction.Remove,
            --     changedItems = removes
            -- }
        )
    end
end

---从start_index 替换range中的元素
---@overload fun(range:table,start_index:int):void
---@param range table
---@param start_index int
local function replace_range(self, range, start_index)
    if range == nil then
        error("Argument items is Null")
    end

    local items = self.items
    local size = #items
    local rep_count, is_table = get_list_length(range)
    if start_index < 0 or start_index + rep_count > size then
        error("Argument start_index Out Of Range)")
    end

    start_index = start_index + 1
    -- local rep_count = #range

    local old_items = {}
    -- for i = 1, rep_count do
    local j = 0
    for i = 0, rep_count do
        if is_table then
            j = i + 1
        else
            j = i
        end
        table_insert(old_items, items[i + start_index])
        items[i + start_index] = range[j]
    end

    set_count(self)

    if is_table then
        range = IListTable(range)
    end

    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsNewItemsOldItemsStartingIndex(
            NotifyCollectionChangedAction.Replace,
            range,
            IListTable(old_items),
            start_index - 1
        )
        -- {
        --     action = NotifyCollectionChangedAction.Replace,
        --     newItems = range,
        --     oldItems = old_items,
        --     startingIndex = start_index - 1
        -- }
    )
end

local function indexof(self, item)
    local idx = table.indexof(self.items, item)
    if idx == nil then
        return -1
    else
        return idx
    end
end

local function find_index(self, filter_fun)
    local items = self.items
    for i, v in ipairs(items) do
        if filter_fun(i, v) then
            return i - 1
        end
    end
    return nil
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

    on_property_changed(self, string_format("items[%d]", index))
    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Add, item, index)
        --  {action = NotifyCollectionChangedAction.Add, changedItem = item, index = index}
    )
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

    on_property_changed(self, string_format("items[%d]", new_index - 1))
    on_property_changed(self, string_format("items[%d]", old_index - 1))

    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsChangedItemIndexOldIndex(
            NotifyCollectionChangedAction.Move,
            new,
            new_index - 1,
            old_index - 1
        )

        -- {
        --     action = NotifyCollectionChangedAction.Move,
        --     changedItem = new,
        --     index = new_index - 1,
        --     oldIndex = old_index - 1
        -- }
    )
end

---删除index处的元素
---@overload fun(index:int):void
---@param index int
local function remove_item(self, obj)
    if obj == nil then
        error("Argument index is nil")
    end
    local items = self.items
    local size = #items
    local index = table.indexof(items, obj)
    -- index = index + 1
    if index == nil then
        error(" Argument index Out of Range " .. tostring(self))
    end

    local old = table_remove(items, index)

    set_count(self)

    on_property_changed(self, string_format("items[%d]", index - 1))
    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Remove, old, index - 1)
        --  {action = NotifyCollectionChangedAction.Remove, changedItem = old, index = index - 1}
    )
end

local function clear(self)
    table.clear(self.items)
    set_count(self)
    on_property_changed(self, "Item[]")

    on_collection_changed(
        self,
        BindingUtility.CreateCollectionArgsChangedItems(NotifyCollectionChangedAction.Reset)
        -- {action = NotifyCollectionChangedAction.Reset}
    )
end

local function tostring(self)
    return string_format("NotifyTable(%s)", self.items)
end

------INotifyPropertyChanged----
-- notify_table.PropertyChanged = property_changed
notify_table.add_PropertyChanged = add_property_changed
notify_table.remove_PropertyChanged = remove_property_changed
------INotifyCollectionChanged-----
-- notify_table.CollectionChanged = collection_changed
notify_table.add_CollectionChanged = add_collection_changed
notify_table.remove_CollectionChanged = remove_collection_changed

------IList
notify_table.set_Item = set_item --object this[int]
notify_table.get_Item = get_item --this[int] = object
notify_table.Add = add --int Add(object value);
notify_table.Clear = clear --void Clear();
notify_table.Contains = contains --bool Contains(object value);
notify_table.IndexOf = indexof --int IndexOf(object value);
notify_table.Insert = insert_item --void Insert(int index, object value);
notify_table.Remove = remove_item --void Remove(object value);
notify_table.RemoveAt = remove_at --void RemoveAt(int index);
notify_table.FindIndex = find_index --- filter_fun(int,item) return index
-- notify_table.Count = Count --int Count { get; }
---集合改变
-- notify_table.OnCollectionChanged = on_collection_changed
notify_table.InsertRange = insert_range
notify_table.RemoveRange = remove_range
notify_table.ReplaceRange = replace_range
notify_table.MoveItem = move_item
notify_table.Move = move_item
---改变属性
notify_table.OnPropertyChanged = on_property_changed
notify_table.SetProperty = set_property
notify_table.__tostring = tostring

---带通知功能的列表的数据集合,实现了INotifyCollectionChanged, INotifyPropertyChanged。
---
---@class NotifyTable
---@class INotifyTable
---@module notify_table.lua
---@overload fun():NotifyTable
---@return NotifyTable
---@field Add   fun(self:table, item:table)
---@field Clear fun(self:table)
---@field Contains  fun(self:table, item:table)
---@field IndexOf fun(self:table, item:table)
---@field FindIndex fun(self:table, filter_fun:fun(i:number, v:table):number):number
---@field InsertRange fun(self:table, index:number, range:table)
---@field RemoveAt fun(self:table, index:table, count:table)
---@field RemoveRange fun(self:table, range:table)
---@field ReplaceRange fun(self:table, range:table, start_index:number)
---@field Insert fun(self:table, index:number, item:table)
---@field MoveItem fun(self:table, old_index:number, new_index:number)
---@field Move fun(self:table, old_index:number, new_index:number)
---@field Remove fun(self:table, obj:table)
---@field set_Item fun(self:table, index:number, item:table)
---@field get_Item fun(self:table, index:number):table
---@field CollectionChanged fun(self:table, op:string, delegate:fun())
---@field OnPropertyChanged fun(self:table, property_name:string)
---@field SetProperty fun(self:table, property_name:string, value:any)
NotifyTable = notify_table
