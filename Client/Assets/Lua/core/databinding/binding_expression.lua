------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
-- local table = table
-- local string_sub = string.sub
local string_format = string.format
local tonumber = tonumber
local type = type
-- local setmetatable = setmetatable

-- local class = class
local CS = CS
-- local BindingPathPart = BindingPathPart
-- local BindingMode = CS.Hugula.Databinding.BindingMode
local BindingUtility = CS.Hugula.Databinding.BindingUtility

local context_property = "context"

--- set target.context
---@overload fun(bindable_object:BindableObject,context:any)
---@param bindable_object BindableObject
---@param context any
local function set_target_context(bindable_object, context)
    --check type
    if context.CollectionChanged then ---check INotifyTable:IList,INotifyPropertyChanged,INotifyCollectionChanged
        BindingUtility.SetContextByINotifyTable(bindable_object, context)
        return
    elseif context.get_Item then ---chekc IList
        BindingUtility.SetContextByIList(bindable_object, context)
        return
    elseif context.PropertyChanged then --check INotifyPropertyChanged
        BindingUtility.SetContextByINotifyPropertyChanged(bindable_object, context)
        return
    end
    bindable_object[context_property] = context
end

-- local function tostring(self)
--     return "BindingExpression()"
-- end

local val = nil
local val_type = nil

local function update_target_unpack(target, property, source, path, is_self, is_method, is_indexer, format, converter)
    local current = source --last_part.source
    if current == nil then
        -- Logger.Log(
        --     string.format(
        --         "set_target_value target=%s,property=%s,current=%s,path=%s,converter=%s,val=%s,ConvertVal=%s",
        --         target,
        --         property,
        --         current,
        --         path,
        --         converter,
        --         val,
        --         converter and converter:Convert(val, val_type) or ""
        --     )
        -- )
        return
    end

    if is_indexer == true then
        path = tonumber(path)
    end

    if is_self then
        val = current
    elseif is_method then
        val = current[path]()
    else
        val = current[path]
    end

    if format ~= "" then
        val = string_format(format, val)
    end

    if converter ~= nil then
        val = converter:Convert(val, val_type)
    end

    if property == context_property and type(val) == "table" then ---如果是context是lua table
        set_target_context(target, val)
    else
        target[property] = val
    end
end

-- (target, property, source, path, is_self, is_method, is_indexer, format, converter)
local function update_source_unpack(target, property, source, path, is_self, is_method, is_indexer, format, converter)
    if is_indexer == true then
        path = tonumber(path)
    end

    local val = target[property]
    local current = source
    if converter then
        val = converter:ConvertBack(val)
    end
    -- Logger.Log(
    --     string.format(
    --         "update_source target=%s,property=%s,current=%s,source=%s,path=%s,val=%s",
    --         target,
    --         property,
    --         current,
    --         source,
    --         part,
    --         val
    --     )
    -- )

    if is_method then
        current[path](val)
    else
        current[path] = val
    end

    -- if source.on_property_set then --触发改变
    --     source:on_property_set(path)
    -- end
end

---invoke source property
---@overload fun(source:any, part:BindingPathPart, needSubscribe:boolean):any
---@param source any
---@param part BindingPathPart
---@param needSubscribe boolean
---@return any
local function get_property_unpack(source, part, property, is_self, is_method, is_index, needSubscribe)
    if is_self then
        return source
    end

    if is_index then
        property = tonumber(property)
    end
    local val = nil
    if is_method then
        val = source[property]()
    else
        val = source[property]
    end
    if needSubscribe and val and val.PropertyChanged then
        part:Subscribe(val)
    end
    return val
end

local binding_expression = {}

binding_expression.set_target_context = set_target_context
binding_expression.get_property_unpack = get_property_unpack
binding_expression.update_target_unpack = update_target_unpack
binding_expression.update_source_unpack = update_source_unpack
-- binding_expression
---绑定信息
---@class BindingExpression
---@overload fun():BindingExpression
---@return BindingExpression
---@field set_target_context function
---@field get_property_unpack function
---@field update_target_unpack function
---@field update_source_unpack function
BindingExpression = binding_expression

CS.Hugula.Databinding.ExpressionUtility.instance:Initialize(
    get_property_unpack,
    update_target_unpack,
    update_source_unpack
)
