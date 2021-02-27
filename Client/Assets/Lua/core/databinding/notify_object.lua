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

--实现public interface INotifyPropertyChanged {
-- event PropertyChangedEventHandler PropertyChanged;
-- }
---这个要通知到C#端
local notify_object =
    class(
    function(self, get_set)
        ---属性改变事件监听
        self._property_changed = {}
        -- if get_set == true then
        self.property = GetSetObject(self) --设置property getset
        -- end
    end
)

local add_property_changed = function(self, delegate)
    if self._property_changed == nil then
        self._property_changed = {}
    end
    table.insert(self._property_changed, delegate)
    -- Logger.LogErrorFormat("object add_property_changed=%s,%s;",#self._property_changed,delegate)
end

local remove_property_changed = function(self, delegate)
    -- Logger.LogErrorFormat("object remove_property_changed=%s,%s;",self,delegate)
    local _property_changed = self._property_changed
    for i = 1, #_property_changed do
        if Object.Equals(_property_changed[i], delegate) then
            table.remove(_property_changed, i)
            -- Logger.LogErrorFormat("object remove_property_changed=%s,%s; ",i, delegate)
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
    -- Logger.Log(" on_Property_changed#=", property_name, #changed)
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

local function tostring(self)
    return string_format("NotifyObject(%s)", self._property_changed)
end
---INotifyPropertyChanged接口实现
notify_object.PropertyChanged = property_changed
notify_object.add_PropertyChanged = add_property_changed
notify_object.remove_PropertyChanged = remove_property_changed
---改变属性
notify_object.OnPropertyChanged = on_Property_changed
notify_object.SetProperty = set_property
notify_object.__tostring = tostring

---属性改变监听类 接口INotifyPropertyChanged的lua实现
---@class NotifyObject
---@module Assets\Lua\core\databinding\notify_object.lua
---@overload fun():NotifyObject
---@return NotifyObject
---@field PropertyChanged fun(self:table, op:string, delegate:fun())
---@field add_PropertyChanged fun(self:table, delegate:fun())
---@field remove_PropertyChanged fun(self:table, delegate:fun())
---@field OnPropertyChanged fun(self:table, property_name:string)
---@field SetProperty fun(self:table, property_name:string, value:any)
NotifyObject = notify_object
