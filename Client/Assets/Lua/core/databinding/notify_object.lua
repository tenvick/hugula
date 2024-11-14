------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local pairs = pairs
local class = class
local string_format = string.format
local Object = CS.System.Object
local GetSetObject = GetSetObject
local PropertyChangedEventHandlerEvent = CS.Hugula.Databinding.PropertyChangedEventHandlerEvent

--实现public interface INotifyPropertyChanged {
-- PropertyChangedEventHandlerEvent PropertyChanged;
-- }
---这个要通知到C#端
local notify_object =
    class(
    function(self, get_set)
        ---属性改变事件监听
        self.PropertyChanged = PropertyChangedEventHandlerEvent.Get()
        -- if get_set == true then
        self.property = GetSetObject(self) --设置property getset
        -- end
    end
)

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

---属性改变的时候通知绑定对象
---@overload fun(property_name:string)
---@return void
local function on_Property_changed(self, property_name)
    self.PropertyChanged:Invoke(self,property_name)
end

---改变属性
---@overload fun(property_name:string,value:any)
---@return void
local function set_property(self, property_name, value, force_refresh)
    if force_refresh or self[property_name] ~= value then
        self[property_name] = value
        on_Property_changed(self, property_name)
    end
end

local function tostring(self)
    return string_format("NotifyObject(%s)", self.PropertyChanged)
end

local function _gc(self)
    local propertyChanged = self and self.PropertyChanged
    if propertyChanged then
        PropertyChangedEventHandlerEvent.Release(propertyChanged)
    end
end

---INotifyPropertyChanged接口实现
-- notify_object.PropertyChanged = property_changed
notify_object.add_PropertyChanged = add_property_changed
notify_object.remove_PropertyChanged = remove_property_changed
---改变属性
notify_object.OnPropertyChanged = on_Property_changed
notify_object.SetProperty = set_property
-- notify_object.__tostring = tostring
notify_object.__gc = _gc

---属性改变监听类 接口INotifyPropertyChanged的lua实现
---@class NotifyObject
---@module Assets\Lua\core\databinding\notify_object.lua
---@overload fun():NotifyObject
---@return NotifyObject
---@field PropertyChanged fun(self:table, op:string, delegate:fun())
---@field add_PropertyChanged fun(self:table, delegate:fun())
---@field remove_PropertyChanged fun(self:table, delegate:fun())
---@field OnPropertyChanged fun(self:table, property_name:string)
---@field SetProperty fun(self:table, property_name:string, value:any, force_refresh:boolean)
NotifyObject = notify_object
