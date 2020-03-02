------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local pairs = pairs
local string_format = string.format
local class = class
local VMState = VMState
local Object = CS.System.Object

local vm_base =
    class(
    function(self)
        ---属性改变事件监听
        self._property_changed = {}
        self.is_active = false ---是否激活。
        self.is_res_ready = false ---资源是否准备好
    end
)

local add_property_changed = function(self, delegate)
    if self._property_changed == nil then
        self._property_changed = {}
    end
    table.insert(self._property_changed, delegate)
    -- Logger.Log("object add_property_changed", delegate)
end

local remove_property_changed = function(self, delegate)
    local _property_changed = self._property_changed
    for i = 1, #_property_changed do
        if Object.Equals(_property_changed[i], delegate) then
            table.remove(_property_changed, i)
            break
        end
    end
    -- Logger.Log("object remove_property_changed ", delegate)
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
---激活的时候
---@overload fun()
local function on_active(self)
    -- body
end

---失活的时候
---@overload fun()
local function on_deactive(self)
    -- body
end

---注销的时候
---@overload fun()
local function dispose(self)
    -- body
end

local on_back = {
    Execute = function(self, arg)
        VMState.back()
    end,
    CanExecute = function(...)
        return true
    end
}

local function tostring(self)
    return string_format("VMBase(name=%s).views=%s ", self.views, self.name)
end

---INotifyPropertyChanged接口实现
vm_base.PropertyChanged = property_changed
vm_base.add_PropertyChanged = add_property_changed
vm_base.remove_PropertyChanged = remove_property_changed
---改变属性
vm_base.OnPropertyChanged = on_Property_changed
vm_base.SetProperty = set_property

vm_base.on_active = on_active
vm_base.on_deactive = on_deactive
vm_base.dispose = dispose
vm_base.on_back = on_back
vm_base.__tostring = tostring
---所有视图模型的基类
---@class VMBase
---@field PropertyChanged function
---@field SetProperty function
---@field on_active function
---@field on_deactive function
---@field on_property_set function
---@field dispose function
---@field is_active boolean
---@field is_res_ready boolean
VMBase = vm_base
