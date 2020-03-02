------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local oneway_path_demo = {}

----------------------------------申明属性名--------------
local property_text1 = "text1"
local property_goods = "goods"
local property_name = "name"
local property_color = "color"
-------------------------------------------------

---绑定属性 
---text1 绑定 text1
---text2 绑定 goods.name
---text3 绑定 bind_text4(1)
---text4 绑定 goods.color[2]


oneway_path_demo.text1 = "path=text1,普通路径"
---数组与多路径支持双向
local goods = NotifyObject()
goods.name = "path=goods.name,pen"
goods.color =  {"path=goods.color[1],red", "path=goods.color[2],yellow(原始属性)", "path=goods.color[3], blue"}
oneway_path_demo.goods = goods

---可以绑定的方法
---@overload fun(arg:any):any
---@param arg any
---@return any
function oneway_path_demo.bind_text4(arg)
    -- Logger.Log("bind_fun4=",arg)
    return "path=bind_fun4(1) 绑定方法(1)表示参数"
end

function oneway_path_demo:on_property_set(property)
    -- Logger.Log(" oneway_path_demo.on_property_set",property)
    goods:OnPropertyChanged(property_color)
end
return oneway_path_demo
