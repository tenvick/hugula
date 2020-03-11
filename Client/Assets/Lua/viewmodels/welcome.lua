---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
--data:2020.2.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------
local View = View
local NotifyTable = NotifyTable
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color
---@class VMBase vm
---@class welcome
local welcome = VMBase()

--UI资源
welcome.views = {
    View(welcome, "views.welcome_view")
}
----------------------------------申明属性名用于绑定--------------
local property_eg_data = "eg_data"

--------------------------------绑定属性-----------------
welcome.eg_data = require("viewmodels.submodels.eg_list_model") ---模块化演示

------------------public------------------
function welcome:on_active()
    Logger.Log("welcome. active eg_data=",welcome.eg_data)
end

function welcome:on_property_set(property)
    Logger.Log("welcome. on_property_set", property)
end

return welcome
