------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local tonumber = tonumber
local tostring = tostring
local ValueConverterRegister = CS.Hugula.Databinding.ValueConverterRegister.instance
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
    Convert = function(self, source_value, type)
        if source_value == nil then
            return 0
        else
            return tonumber(source_value)
        end
    end,
    ConvertBack = function(self, target_value, type)
        if target_value == nil then
            return ""
        else
            return tostring(target_value)
        end
    end
}

local number_to_string = {
    Convert = function(self, target_value, type)
        if target_value == nil then
            return ""
        else
            return tostring(target_value)
        end
    end,
    ConvertBack = function(self, source_value, type)
        if source_value == nil then
            return 0
        else
            return tonumber(source_value)
        end
    end
}

-- converter.number_to_string= number_to_string

ValueConverterRegister:AddConverter("StringToNumber", string_to_number)
ValueConverterRegister:AddConverter("NumberToString", number_to_string)

