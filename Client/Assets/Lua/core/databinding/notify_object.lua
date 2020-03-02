------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local pairs = pairs
local class = class

local Object = CS.System.Object

--实现public interface INotifyPropertyChanged {
-- event PropertyChangedEventHandler PropertyChanged;
-- }
---这个要通知到C#端
local nofity_object =
    class(
    function(self)
        ---属性改变事件监听
        self._property_changed = {}
        self._items = {}
    end
)

local add_property_changed = function(self, delegate)
    if self._property_changed == nil then
        self._property_changed = {}
    end
    table.insert(self._property_changed, delegate)
    Logger.Log("object add_property_changed",#self._property_changed, delegate)
end

local remove_property_changed = function(self, delegate)
    local _property_changed = self._property_changed
    for i = 1, #_property_changed do
        if Object.Equals(_property_changed[i], delegate) then
            table.remove(_property_changed, i)
            Logger.Log("object remove_property_changed ",i, delegate)
            break
        end
    end
end

local function property_changed(self, op, delegate)
    if op == "+" then
        add_property_changed(self, delegate)
    else
        remove_property_changed(self, delegate)
    end
end

---属性改变的时候通知绑定对象
---@overload fun(property_name:string)
---@return void
local function on_Property_changed(self, property_name)
    local changed = self._property_changed
    Logger.Log(" on_Property_changed#=", #changed)
    for i = 1, #changed do
        changed[i](self, property_name)
    end
end

---改变属性
---@overload fun(property_name:string,value:any)
---@return void
local function set_property(self, property_name, value)
    local old = self[property_name]
    if old ~= value then
        self[property_name] = value
        on_Property_changed(self, property_name)
    end
end

---INotifyPropertyChanged接口实现
nofity_object.PropertyChanged = property_changed
nofity_object.add_PropertyChanged = add_property_changed
nofity_object.remove_PropertyChanged = remove_property_changed
---改变属性
nofity_object.OnPropertyChanged = on_Property_changed
nofity_object.SetProperty = set_property

---属性改变监听类
---@class NotifyObject
---@module nofity_object.lua
---@overload fun():NotifyObject
---@return NotifyObject
---@field PropertyChanged function
---@field add_PropertyChanged function
---@field remove_PropertyChanged function
---@field OnPropertyChanged function
---@field SetProperty function
NotifyObject = nofity_object
