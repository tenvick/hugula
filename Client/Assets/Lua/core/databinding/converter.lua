------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local tonumber = tonumber
local tostring = tostring
---
---转换器
--- 实现接口
--- IConverter {
---   convert=function(source,...):any
---   convert_back=function(val,):any
---}
---@class converter
local converter = {}

---string 与 number的相互装换
local string_to_number = {
    convert = function(source_value)
        if source_value == nil then
            return 0
        else
            return tonumber(source_value)
        end
    end,
    convert_back = function(target_value)
        if target_value == nil then
            return ""
        else
            return tostring(target_value)
        end
    end
}


local number_to_string = {
    convert = function(target_value)
        if target_value == nil then
            return ""
        else
            return tostring(target_value)
        end
    end,
    convert_back = function(source_value)
        if source_value == nil then
            return 0
        else
            return tonumber(source_value)
        end
    end
}


converter.number_to_string= number_to_string



---转换器
--- 实现接口
--- IConverter {
---   convert=function(source,...):any
---   convert_back=function(val,):any
---}
---@class Converter
---@field string_to_number table
Converter = converter