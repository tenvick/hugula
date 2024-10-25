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

local obj_to_bool = {
    Convert = function(self, source_value, type)
        if source_value == nil then
            return false
        else
            return true
        end
    end,
    ConvertBack = function(self, target_value, type)
        return target_value
    end
}

-- converter.number_to_string= number_to_string

ValueConverterRegister:AddConverter("StringToNumber", string_to_number)
ValueConverterRegister:AddConverter("NumberToString", number_to_string)
ValueConverterRegister:AddConverter("ObjToBool", obj_to_bool)

---------------------得到当前点击ui--------------------------

local EventSystem = CS.UnityEngine.EventSystems.EventSystem
local get_firsts_gameobject_convert = {
    Convert = function(self, target_value, type)
        local s_obj = EventSystem.current.currentSelectedGameObject
        return s_obj.transform
    end,
    ConvertBack = function(self, source_value, type)
    end
}

ValueConverterRegister:AddConverter("FirstSelectedGameObject", get_firsts_gameobject_convert)