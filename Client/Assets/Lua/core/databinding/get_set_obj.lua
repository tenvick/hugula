------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discript 为viewmodel实现set与get
--  author pu
------------------------------------------------
local setmetatable = setmetatable

local mt = {}

mt.__index = function(tb, k)
    local self = tb.self
    return self[k]
end

mt.__newindex = function(tb, k, v)
    local self = tb.self
    self:SetProperty(k, v)
end

mt.__call = function(tab, ...)
    local obj = {}
    local self = ...
    obj.self = self
    setmetatable(obj, mt)
    return obj
end

local get_set_object = {}
setmetatable(get_set_object, mt)
---属性改变监听类 接口INotifyPropertyChanged的lua实现
---@class GetSetObject
---@module Assets\Lua\core\databinding\get_set_object.lua
GetSetObject = get_set_object
