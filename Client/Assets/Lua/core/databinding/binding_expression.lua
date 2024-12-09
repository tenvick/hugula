------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local string_format = string.format
local tonumber = tonumber

local binding_expression = {}

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

binding_expression.m_SetSourcePropertyInvoke = function(source, target, path, property, is_indexer, converter)
    if is_indexer == true then
        path = tonumber(path)
    end

    local val = target[property]

    if converter ~= nil then
        val = converter:ConvertBack(val, nil)
    end

    source[path] = val
end

binding_expression.m_SetSourceMethodInvoke = function(source, target, path, property, is_indexer, converter)
    if is_indexer == true then
        path = tonumber(path)
    end

    local val = target[property]

    if converter ~= nil then
        val = converter:ConvertBack(val, nil)
    end

    source[path](val)
end

binding_expression.m_SetTargetPropertyInvoke = function(source, target, path, property, is_indexer, is_self,
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

    if converter then
        val = converter:Convert(val)
    end

    target[property] = val
end


binding_expression.m_SetTargetPropertyNoConvertInvoke = function(source, target, path, property, is_indexer, is_self,
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

    target[property] = val
end


binding_expression.m_SetTargetMethodInvoke = function(source, target, path, property, is_indexer, is_self,
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

    if converter then
        val = converter:Convert(val)
    end

    target[property] = val
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
    binding_expression.m_SetTargetPropertyNoConvertInvoke,
    binding_expression.m_SetTargetMethodInvoke
)
