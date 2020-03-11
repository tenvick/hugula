------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
require("core.databinding.converter")
local table = table
local string_sub = string.sub
local string_format = string.format
local tonumber = tonumber
local ipairs = ipairs
local setmetatable = setmetatable

local class = class
local Converter = Converter
local BindingPathPart = BindingPathPart
local BindingUtility = CS.Hugula.Databinding.BindingUtility

local context_property = "context"

--- set target.context
---@overload fun(bindable_object:BindableObject,context:any)
---@param bindable_object BindableObject
---@param context any
local function set_target_context(bindable_object, context)
    if context.CollectionChanged and context.get_Item ~= nil then ---check INotifyTable:IList,INotifyPropertyChanged,INotifyCollectionChanged
        BindingUtility.SetContextByINotifyTable(bindable_object, context)
    elseif context.get_Item then ---chekc IList
        BindingUtility.SetContextByIList(bindable_object, context)
    elseif context.PropertyChanged then --check INotifyPropertyChanged
        BindingUtility.SetContextByINotifyPropertyChanged(bindable_object, context)
    else
        bindable_object[context_property] = context
    end
end


local function tostring(self)
    return "BindingExpression()"
end

--- set source property value
---@overload fun(target:BindableObject,source:any,current:any,part:BindingPathPart)
---@param target BindableObject
---@param source any
---@param current any
---@param part BindingPathPart
local function set_source_value(target, source, current, part)
    local expression = part.expression
    local path = part.path
    local source_property = path
    local property = expression.targetProperty
    local converter = expression.converter
    local is_index = part.isIndexer
    if is_index == true then
        path = tonumber(path)
        source_property = part.indexerName
    end

    local val = target[property]

    -- Logger.Log(
    --     string.format(
    --         "set_source_value target=%s,property=%s,current=%s,source=%s,path=%s,val=%s",
    --         target,
    --         property,
    --         current,
    --         source,
    --         path,
    --         val
    --     )
    -- )
    if converter then
        val = converter.convert_back(val)
    end

    if part.isSelf then
        source = val
    elseif part.isSetter then
        current[path](val)
    else
        current[path] = val
    end

    if source.on_property_set then --触发改变
        source:on_property_set(source_property)
    end
end

--- set target property value
---@overload fun(target:BindableObject,source:any,current:any,part:BindingPathPart)
---@param target BindableObject
---@param source any
---@param current any
---@param part BindingPathPart
local function set_target_value(target, source, current, part)
    local expression = part.expression
    local is_index = part.isIndexer
    local path = part.path
    local property = expression.targetProperty
    local format = expression.format
    local converter = expression.converter
    if is_index == true then
        path = tonumber(path)
    end


    local val = nil
    if part.isSelf then
        val = current
    elseif part.isSetter or part.isGetter then
        val = current[path]()
    else
        val = current[path]
    end

    -- Logger.Log(
    --     string.format(
    --         "set_target_value target=%s,property=%s,current=%s,path=%s,val=%s",
    --         target,
    --         property,
    --         current,
    --         path,
    --         val
    --     )
    -- )
    if format ~= "" then
        val = string_format(format, val)
    end

    if converter then
        val = converter.convert(val)
    end

    if property == context_property then ---如果是设置的context
        set_target_context(target, val)
    else
        local old = target[property]
        if old ~= val then
            target[property] = val
        end
    end
end

--- apply actual 
---@overload fun(sourceObject:object, target:BindableObject, property:string, _parts:List<BindingPathPart>,needsGetter:bool,needsSetter:bool,needSubscribe:bool)
---@param sourceObject object
---@param target BindableObject
---@param property string
---@param _parts List<BindingPathPart>
---@param needsGetter bool
---@param needsSetter bool
---@param needSubscribe bool
local function apply_actual_by_lua(
    sourceObject,
    target,
    property,
    _parts,
    needsGetter,
    needsSetter,
    needSubscribe)
    local current = sourceObject
    -- local mode
    local part = nil
    for i = 0, _parts.Count - 1 do
        part = _parts[i]

        if not part.isSelf and current ~= nil then
            if i < _parts.Count - 1 then
                if part.isSetter or part.isGetter then
                    current = current[part.path]()
                else
                    current = current[part.path]
                end
            end
        end

        -- Logger.LogErrorFormat(
        --     "ApplyActual.current=%s,property=%s,m_Path=%s,parts.Count=%s,part=%s,i=%s",
        --     current,
        --     property,
        --     "m_Path",
        --     _parts.Count,
        --     part,
        --     i
        -- )
        if not part.isSelf and current == nil then
            break
        end

        if part.nextPart ~= nil and needSubscribe and current.PropertyChanged then
            part:Subscribe(current)
        end
    end

    if part == nil then
        return
    end

    if needsGetter and current ~= nil then
        set_target_value(target, sourceObject, current, part)
    elseif needsSetter and current ~= nil then
        set_source_value(target, sourceObject, current, part)
    end
end

---invoke source method
---@overload fun(source:any, property:string, value:any):any
---@param source any
---@param property string
---@param value any
---@return any
local function invoke_method (source, property, value)
    local val = source[property]()
    return val
end

---invoke source property
---@overload fun(source:any, property:string, value:any):any
---@param source any
---@param property string
---@param value any
---@return any
local function get_property (source, property, value)
    local val = source[property]
    return val
end


local binding_expression = {}

binding_expression.set_target_context = set_target_context
binding_expression.set_source_value = set_source_value
binding_expression.set_target_value = set_target_value
binding_expression.invoke_method = invoke_method
binding_expression.get_property = get_property
binding_expression.apply_actual_by_lua = apply_actual_by_lua

-- binding_expression
---绑定信息
---@class BindingExpression
---@overload fun():BindingExpression
---@return BindingExpression
---@field set_target_context function
---@field set_source_value function
---@field set_target_value function
---@field invoke_source function
---@field get_property function
---@field apply_actual_by_lua function
BindingExpression = binding_expression

