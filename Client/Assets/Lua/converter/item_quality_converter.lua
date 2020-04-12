------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local string = string
local tostring = tostring
local ValueConverterRegister = CS.Hugula.Databinding.ValueConverterRegister.instance
---
---转换器
--- 实现接口
--- IConverter {
---   convert=function(source,...):any
---   convert_back=function(val,):any
---}
local sprite_pre = "common_hero_rim"

---string 与 quality 的相互装换
local string_to_quality_icon = {
    Convert = function(source_value, type)
        return sprite_pre..source_value
    end,
    ConvertBack = function(target_value, type)
        return target_value
    end
}


ValueConverterRegister:AddConverter("StringToQualityIcon", string_to_quality_icon)

