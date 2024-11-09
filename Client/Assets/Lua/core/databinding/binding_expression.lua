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

local binding_expression = {}

binding_expression.set_target_context = set_target_context

---new BindingExpression
binding_expression.m_GetSourcePropertyInvoke = function(source, path, is_indexer)
    if is_indexer then
        path = tonumber(path)
    end
    local val = source[path]

    return val
end

binding_expression.m_GetSourceMethodInvoke = function(source, path, is_indexer)
    if is_indexer then
        path = tonumber(path)
    end
    local val = source[path]()

    return val
end

binding_expression.m_SetSourcePropertyInvoke = function(source, path, target, property, is_indexer, converter)
    if is_indexer == true then
        path = tonumber(path)
    end

    local val = target[property]

    if converter ~= nil then
        val = converter:ConvertBack(val, nil)
    end

    source[path] = val
end

binding_expression.m_SetSourceMethodInvoke = function(source, path, target, property, is_indexer, converter)
    if is_indexer == true then
        path = tonumber(path)
    end

    local val = target[property]

    if converter ~= nil then
        val = converter:ConvertBack(val, nil)
    end

    source[path](val)
end

binding_expression.m_SetTargetPropertyInvoke = function(source, path, target, property, is_indexer, is_self, format,
                                                        converter)
    if is_indexer then
        path = tonumber(path)
    end
    local val = nil
    if is_self then
        val = source
    else
        val = source[path]
    end

    if format ~= "" then
        val = string_format(format, val)
    end

    if converter then
        val = converter:Convert(val)
    end

    if property == context_property and type(val) == "table" then ---如果是context是lua table
        set_target_context(target, val)
    else
        target[property] = val
    end

end

binding_expression.m_SetTargetPropertyNoFormatInvoke = function(source, path, target, property, is_indexer, is_self,
                                                                format,
                                                                converter)
    if is_indexer then
        path = tonumber(path)
    end

    local val = nil
    if is_self then
        val = source
    else
        val = source[path]
    end

    if property == context_property and type(val) == "table" then ---如果是context是lua table
        set_target_context(target, val)
    else
        target[property] = val
    end


end

binding_expression.m_SetTargetMethodInvoke = function(source, path, target, property, is_indexer, is_self, format,
                                                      converter)
    if is_indexer then
        path = tonumber(path)
    end
    local val = nil
    if is_self then
        val = source
    else
        val = source[path]()
    end

    if format ~= "" then
        val = string_format(format, val)
    end

    if converter then
        val = converter:Convert(val)
    end

    if property == context_property and type(val) == "table" then ---如果是context是lua table
        set_target_context(target, val)
    else
        target[property] = val
    end

end

binding_expression.m_PartSubscribe = function(m_Current, part)
     if  m_Current and m_Current.PropertyChanged then
        part:Subscribe(m_Current)
    end   
end

-- binding_expression
---绑定信息
---@class BindingExpression
---@overload fun():BindingExpression
---@return BindingExpression
BindingExpression = binding_expression


CS.Hugula.Databinding.ExpressionUtility.instance:NewInitialize(
    binding_expression.m_GetSourcePropertyInvoke,
    binding_expression.m_GetSourceMethodInvoke,
    binding_expression.m_SetSourcePropertyInvoke,
    binding_expression.m_SetSourceMethodInvoke,
    binding_expression.m_SetTargetPropertyInvoke,
    binding_expression.m_SetTargetPropertyNoFormatInvoke,
    binding_expression.m_SetTargetMethodInvoke,
    binding_expression.m_PartSubscribe
)
