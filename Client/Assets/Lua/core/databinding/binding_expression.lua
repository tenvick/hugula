------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local table = table
local string_sub = string.sub
local string_format = string.format
local tonumber = tonumber
local ipairs = ipairs
local setmetatable = setmetatable

local class = class
local CS = CS
local BindingPathPart = BindingPathPart
local BindingMode = CS.Hugula.Databinding.BindingMode
local BindingUtility = CS.Hugula.Databinding.BindingUtility

local context_property = "context"

--- set target.context
---@overload fun(bindable_object:BindableObject,context:any)
---@param bindable_object BindableObject
---@param context any
local function set_target_context(bindable_object, context)
    if context and context.CollectionChanged and context.get_Item ~= nil then ---check INotifyTable:IList,INotifyPropertyChanged,INotifyCollectionChanged
        BindingUtility.SetContextByINotifyTable(bindable_object, context)
    elseif context and context.get_Item then ---chekc IList
        BindingUtility.SetContextByIList(bindable_object, context)
    elseif context and context.PropertyChanged then --check INotifyPropertyChanged
        BindingUtility.SetContextByINotifyPropertyChanged(bindable_object, context)
    else
        bindable_object[context_property] = context
    end
end

local function tostring(self)
    return "BindingExpression()"
end

local function update_target(target, property, source, part, format, converter)
    local val = nil
    local val_type = nil

    local format = format
    local property = property
    local path = part.path
    local is_method = part.isMethod
    local current = part.source
    if part.isIndexer == true then
        path = tonumber(path)
    end

    if part.isSelf then
        val = current
    elseif is_method then
        val = current[path]()
    else
        val = current[path]
    end

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

    -- val_type = val.GetType()
    if format ~= "" then
        val = string_format(format, val)
    end

    if converter ~= nil then
        val = converter:Convert(val, val_type)
    end

    if target == nil then
        Logger.LogErrorFormat(
            "binding error ,target is nil ,info(property=%s,source=%s,%s.%s=%s ) ",
            property,
            source,
            current,
            path,
            val
        )
        return
    end
    -- Logger.Log("val=", val)
    if property == context_property and val ~= nil then ---如果是设置的context
        set_target_context(target, val)
    else
        -- end
        -- local old = target[property]
        -- if old ~= val then
        target[property] = val
    end
end

local function update_source(target, property, source, part, format, converter)
    local is_index = part.isIndexer
    local path = part.path
    local property = property
    local is_method = part.isMethod
    local format = format
    if is_index == true then
        path = tonumber(path)
    end

    local val = target[property]
    local current = part.source
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
    if part.isSelf then
        source = val
    elseif is_method then
        current[path](val)
    else
        current[path] = val
    end

    if source.on_property_set then --触发改变
        source:on_property_set(path)
    end
end

--- apply actual
---@overload fun(binding:Binding)
---@param sourceObject object
---@param target BindableObject
---@param property string
---@param _parts List<BindingPathPart>
---@param needsGetter bool
---@param needsSetter bool
---@param needSubscribe bool
local function apply_actual_by_lua(binding, source)
    local current = source
    local needSubscribe = binding.needSubscribe
    local part = nil
    local _parts = binding.parts

    for i = 0, _parts.Count - 1 do
        part = _parts[i]
        part:SetSource(current)
        if not part.isSelf and current ~= nil then
            if i < _parts.Count - 1 then
                if part.isMethod then
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
    binding:SetLastPart()
    local mode = binding.mode
    if mode == BindingMode.OneWay or BindingMode.TwoWay then
        update_target(binding.target, binding.propertyName, source, part, binding.format, binding.convert)
    elseif mode == BindingMode.OneWayToSource then
        update_source(binding.target, binding.propertyName, source, part, binding.format, binding.convert)
    end
end

---invoke source property
---@overload fun(source:any, part:BindingPathPart, needSubscribe:boolean):any
---@param source any
---@param part BindingPathPart
---@param needSubscribe boolean
---@return any
local function get_property(source, part, needSubscribe)
    local property = part.path
    local is_method = part.isMethod
    if part.isIndexer then
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
binding_expression.get_property = get_property
binding_expression.apply_actual_by_lua = apply_actual_by_lua
binding_expression.update_target = update_target
binding_expression.update_source = update_source
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
