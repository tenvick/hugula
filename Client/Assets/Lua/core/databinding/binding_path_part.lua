------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local string_match = string.match
local string_format = string.format
local type = type
local class = class
local weak_table = weak_table

local self_path = "."

---绑定路径
---源对象值查找和更新
---@class BindingPathPart
local binding_path_part =
    class(
    function(self, binding, path)
        self.binding = binding
        self.path = path
        self._weak_ref = weak_table()
        if path == self_path then
            self.is_self = true
        end

        self.handler = function(sender, args)
            -- Logger.Log(string.format("self=%s,_weak_ref=%s,sender=%s,args=%s", self, self._weak_ref, sender, args))
            -- self:on_property_changed(sender, args)
            local _weak_ref = self._weak_ref
            local listener = _weak_ref["listener"]
            if listener then
                listener(self, sender, args)
            else
                self:unsubscribe()
                Logger.LogError("listener is nil")
            end
        end
    end
)

--- 获取源source的属性
---@overload fun(source:table)
---@param source table
---@return any
local function get_source_val(self, source)
    local val
    if self.is_method == true then
        -- Logger.Log(string_format("get_source_val (source:%s,path:%s())", source, self.path))
        val = source[self.path]()
    else
        -- Logger.Log(string_format("get_source_val(source:%s,path:%s)=%s", source, self.path, val))
        val = source[self.path]
    end
    return val
end

--- 获取源source的属性
---@overload fun(source:table)
---@param source table
---@param val   any
---@return void
local function set_source_val(self, source, val)
    if self.is_method == true then
        local method_args = self.method_args
        if method_args ~= nil and method_args >= 1 then
            source[self.path](val)
        else
            source[self.path]()
        end
    else
        source[self.path] = val
    end
end

--- wm属性改变的时候触发表达式更新
---@overload fun(self:table,property:string)
---@param sender table
---@param property string
local function property_changed(self, sender, property)
    local part = self.next_part or self
    -- Logger.Log("property changed:", self, sender, property,part.path)
    if property and part then
        if
            property == part.path or
                (part.is_indexer and property == string_format("%s[%s]", part.index_name, part.path))
         then --如果匹配 执行绑定函数
            -- Logger.Log("part.binding:Apply(true):", self, sender, property)
            part.binding:Apply(true)
        end
    end
end

--- 取消订阅source改变监听
---@overload fun(self:BindingPathPart)
---@param self BindingPathPart
local function unsubscribe(self)
    local _weak_ref = self._weak_ref
    local source = _weak_ref["source"]
    if source ~= nil and source.PropertyChanged then
        -- Logger.Log("unsubscribe", source, self.path, self.handler)
        source:PropertyChanged("-", self.handler)
    end
    _weak_ref["source"] = nil
    _weak_ref["listener"] = nil
end

--- 取消订阅source改变监听
---@overload fun(self:BindingPathPart,source:INotifyPropertyChanged)
---@param self BindingPathPart
---@param source INotifyPropertyChanged
local function subscribe(self, source)
    if source then
        local _weak_ref = self._weak_ref
        if _weak_ref["source"] ~= source then
            unsubscribe(self)
            _weak_ref["source"] = source
            _weak_ref["listener"] = property_changed
            source:PropertyChanged("+", self.handler)
            -- Logger.Log("subscribe", source, self.path, self.handler)
        end
    end
    -- body
end

local function dispose(self)
    self.handler = nil
    self._weak_ref = nil
end

local function tostring(self)
    -- body
    return string_format("BindingPathPart(path=%s,binding=%s)", self.path, self.binding)
end

binding_path_part.get_source_val = get_source_val
binding_path_part.set_source_val = set_source_val
binding_path_part.subscribe = subscribe
binding_path_part.unsubscribe = unsubscribe
binding_path_part.property_changed = property_changed
binding_path_part.__tostring = tostring

---绑定路径
---源对象值查找和更新
---@class BindingPathPart
---@module binding_path_part.lua
---@field get_source_val function
---@field set_source_val function
---@field subscribe function
---@field unsubscribe function
---@field property_changed function
---@field next_part BindingPathPart
---@field tostring function
BindingPathPart = binding_path_part
