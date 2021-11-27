------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local pairs = pairs
local ipairs = ipairs
local string_format = string.format
local table_insert = table.insert
local table_remove = table.remove
local class = class
local Object = CS.System.Object
local GetSetObject = GetSetObject

local empty_tab = {}

local vm_base =
    class(
    function(self)
        ---属性改变事件监听
        self._property_changed = {}
        self.msg = {} --消息监听
        self.property = GetSetObject(self) --设置property getset
        self.is_active = false ---是否激活。
        self.is_res_ready = false ---资源是否准备好
        self.auto_context = true ---资源完成自动设置context
    end
)

local add_property_changed = function(self, delegate)
    if self._property_changed == nil then
        self._property_changed = {}
    end
    table_insert(self._property_changed, delegate)
    -- Logger.Log("object add_property_changed", delegate, tostring(self), #self._property_changed)
end

local remove_property_changed = function(self, delegate)
    local _property_changed = self._property_changed
    -- Logger.Log("object begin remove_property_changed ", delegate, tostring(self))
    -- Logger.LogTable(_property_changed)
    for i = 1, #_property_changed do
        if Object.Equals(_property_changed[i], delegate) then
            table_remove(_property_changed, i)
            -- Logger.Log("object remove_property_changed ", delegate, i, tostring(self))
            return
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
    local act
    for i = 1, #changed do
        act = changed[i]
        if act then
            -- Logger.Log(" for",i,act)
            act(self, property_name)
        end
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

local function on_push_arg(arg)
    -- body
end

local function on_push()
    -- body
end

local function on_back()
    -- body
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

---view销毁的时候
---@overload fun()
local function on_destroy(self)
    -- body
end

---绑定view的context到当前VMBase
---@overload fun()
local function bind_view(self)
    local views = self.views or empty_tab
    for k, view_base in ipairs(views) do
        view_base:set_child_context(vm_base)
    end
end

---绑定view的context到当前VMBase
---@overload fun()
local function unbind_view(self)
    local views = self.views or empty_tab
    for k, view_base in ipairs(views) do
        view_base:set_child_context(nil)
    end
end

---清理View的资源
---@overload fun()
local function clear(self)
    local views = self.views
    if views then
        for k, v in ipairs(views) do
            v:clear()
        end
    end
    table.clear(self._property_changed)
    self.is_res_ready = false
end

---注销的时候
---@overload fun()
local function dispose(self)
    -- body
    table.clear(self._property_changed)
    self._push_arg = nil
end

local function tostring(self)
    return string_format("VMBase(name=%s).views=%s ", self.name, self.views)
end

---注销的时候
---@overload fun()
local function debug_property_changed(self)
    local changed = self._property_changed
    Logger.Log(string.format("debug_property_changed(%s) (%s) ", #changed, tostring(self)))
    Logger.LogTable(changed)
end

---INotifyPropertyChanged接口实现
vm_base.PropertyChanged = property_changed
vm_base.add_PropertyChanged = add_property_changed
vm_base.remove_PropertyChanged = remove_property_changed
---改变属性
vm_base.OnPropertyChanged = on_Property_changed
vm_base.SetProperty = set_property
--栈相关事件
vm_base.on_push_arg = on_push_arg
vm_base.on_push = on_push
vm_base.on_back = on_back
vm_base.on_active = on_active
vm_base.on_deactive = on_deactive
vm_base.on_destroy = on_destroy
vm_base.clear = clear
vm_base.dispose = dispose

vm_base.debug_property_changed = debug_property_changed
vm_base.__tostring = tostring
---所有视图模型的基类
---@class VMBase
---@field OnPropertyChanged fun(self:table, property_name:string)
---@field SetProperty fun(self:table, property_name:string, value:any)
---@field on_push_arg fun(arg:any) 入栈资源加载之前调用 由VMState:push() 触发
---@field on_back fun() 从回退栈激活资源加载完成后调用在on_active之前
---@field on_active fun(self:table)
---@field on_deactive fun(self:table)
---@field clear fun(self:table) 清理view
---@field dispose fun(self:table)
---@field is_active boolean
---@field is_res_ready boolean
VMBase = vm_base
