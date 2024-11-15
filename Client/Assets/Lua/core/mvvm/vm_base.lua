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
local PropertyChangedEventHandlerEvent = CS.Hugula.Databinding.PropertyChangedEventHandlerEvent
local empty_tab = {}
local Timer = Timer
local vm_base =
    class(
    function(self)
        ---属性改变事件监听
        self.PropertyChanged = PropertyChangedEventHandlerEvent()
        self.msg = {} --消息监听
        self.property = GetSetObject(self) --设置property getset
        self.is_active = false ---是否激活。
        self.is_res_ready = false ---资源是否准备好
        self.auto_context = true ---资源完成自动设置context
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
    self.PropertyChanged:Invoke(self, property_name)
end

---改变属性
---@overload fun(property_name:string,value:any)
---@return void
local function set_property(self, property_name, value,force)
    local old = self[property_name]
    if force or old ~= value then
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

local function on_assets_load()
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

local function add_timer(self, timer_id)
    local timers = self._timers
    if timers == nil then
        timers = {}
        self._timers = timers
    end
    timers[timer_id] = timer_id
    return timer_id
end

local function start_timer(self, delay, cycle, func, arg)
    local t = Timer.Add(delay, cycle, func, arg)
    local timers = self._timers
    if timers == nil then
        timers = {}
        self._timers = timers
    end
    timers[t] = t
    return t
end

local function start_delay(self, func, time, arg)
    local t = Timer.Delay(func, time, arg)
    local timers = self._timers
    if timers == nil then
        timers = {}
        self._timers = timers
    end
    timers[t] = t
    return t
end

local function start_delayframe(self, action, frame, arg)
    local t = Timer.DelayFrame(action, frame, arg)
    local timers = self._timers
    if timers == nil then
        timers = {}
        self._timers = timers
    end
    timers[t] = t
    return t
end

local function stop_timer(self, timer_id)
    local timers = self._timers
    if timers then
        local t = timers[timer_id]
        if t then
            Timer.Remove(t)
            timers[t] = nil
        end
    end
end

local function stop_all_timer(self)
    local timers = self._timers
    if timers then
        for t, v in pairs(timers) do
            Timer.Remove(t)
            timers[t] = nil
        end
    end
end

--判断是否已经打开
local function is_open(self)
    return self.is_res_ready and self.is_active
end

---view销毁的时候
---@overload fun()
local function on_destroy(self)
    -- body
end

---绑定view的context到当前VMBase,如果view已经绑定过了相同content则不再绑定
---@overload fun()
local function bind_views(self)
    local views = self.views or empty_tab
    for k, view_base in ipairs(views) do
        if view_base._context ~= self then
            view_base:set_child_context(self)
        end
    end
end

---绑定view的context到当前VMBase
---@overload fun()
local function unbind_views(self)
    local views = self.views or empty_tab
    for k, view_base in ipairs(views) do
        view_base:set_child_context(nil)
    end
end

local function set_views_active(self,bl)
    local views = self.views or empty_tab
    for k, view_base in ipairs(views) do
        view_base:set_active(bl)
    end
end

local function set_views_context(self)
    local _auto_context = self.auto_context
    if _auto_context then
        local views = self.views or empty_tab
        for k, v in ipairs(views) do
            v:set_child_context(self)
        end
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
    self.PropertyChanged:Clear()
    self.is_res_ready = false
end

---注销的时候
---@overload fun()
local function dispose(self)
    -- body
    self.PropertyChanged:Clear()
    self.PropertyChanged = nil
    self._push_arg = nil
end

local function tostring(self)
    return string_format("VMBase(name=%s,ready=%s,active=%s,_destory_step=%s)", self._require_name, self.is_res_ready,self.is_active,self._destory_step)
end

---注销的时候
---@overload fun()
local function debug_property_changed(self)
    local changed = self.PropertyChanged.Count
    Logger.Log(string.format("debug_property_changed(%s) (%s) ", changed, tostring(self)))
    -- Logger.LogTable(changed)
end

---INotifyPropertyChanged接口实现
-- vm_base.PropertyChanged = property_changed
vm_base.add_PropertyChanged = add_property_changed
vm_base.remove_PropertyChanged = remove_property_changed
---改变属性
vm_base.OnPropertyChanged = on_Property_changed
vm_base.SetProperty = set_property
--栈相关事件
vm_base.on_push_arg = on_push_arg
vm_base.on_push = on_push
vm_base.on_back = on_back
vm_base.on_assets_load = on_assets_load
vm_base.on_active = on_active
vm_base.on_deactive = on_deactive
vm_base.add_timer = add_timer
vm_base.start_timer = start_timer
vm_base.start_delay = start_delay
vm_base.start_delayframe = start_delayframe
vm_base.stop_timer = stop_timer
vm_base.stop_all_timer = stop_all_timer
vm_base.on_destroy = on_destroy
vm_base.clear = clear
vm_base.dispose = dispose
vm_base.set_views_context = set_views_context
vm_base.bind_views = bind_views
vm_base.unbind_views = unbind_views
vm_base.set_views_active = set_views_active
vm_base.is_open = is_open
vm_base.debug_property_changed = debug_property_changed
vm_base.__tostring = tostring
---所有视图模型的基类
---@class VMBase
---@field OnPropertyChanged fun(self:VMBase, property_name:string)
---@field SetProperty fun(self:VMBase, property_name:string, value:any)
---@field on_push_arg fun(arg:any) 入栈资源加载之前调用 由VMState:push() 触发
---@field on_back fun(self:VMBase) 从回退栈激活资源加载完成后调用在on_active之前
---@field on_assets_load fun(self:VMBase) 所有资源加载完成
---@field on_active fun(self:table)
---@field add_timer  fun(self:VMBase,timer_id:int) 添加定时器id到生命周期中
---@field start_timer fun(self:VMBase,delay:number, cycle:int, func:function, arg:any) 添加定时器到生命周期中
---@field start_delay fun(self:VMBase,action:function, time:number,arg:any)
---@field start_delayframe fun(self:VMBase,action:function, frame:int, arg:any) 延时固定帧执行
---@field stop_timer  fun(self:VMBase,timer_id:int)
---@field stop_all_timer  fun(self:VMBase)
---@field on_deactive fun(self:VMBase)
---@field clear fun(self:VMBase) 清理view
---@field set_views_context fun(self:VMBase) 绑定数据
---@field bind_views fun(self:VMBase) 绑定数据
---@field unbind_views fun(self:VMBase) 解绑数据
---@field set_views_active fun(self:VMBase,active:boolean) 设置gameobject可见性
---@field is_open fun(self:VMBase):boolean 是否开启
---@field dispose fun(self:VMBase)
---@field is_active boolean
---@field is_res_ready boolean
VMBase = vm_base
