------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local ProfilerFactory        = CS.Hugula.Profiler.ProfilerFactory

local oneway_path_demo = {}

----------------------------------申明属性名--------------
local property_text1 = "text1"
local property_goods = "goods"
local property_name = "name"
local property_color = "color"
-------------------------------------------------
local BindingExpression = BindingExpression
---绑定属性
---text1 绑定 text1
---text2 绑定 goods.name
---text3 绑定 bind_text4(1)
---text4 绑定 goods.color[2]

oneway_path_demo.text1 = "path=text1,普通路径  lua hot 08 24 20:29"
---数组与多路径支持双向
local goods = NotifyObject()
goods.name = "path=goods.name,pen"
-- goods.color = NotifyTable()
-- goods.color:InsertRange({"path=goods.color[1],red", "path=goods.color[2],yellow(原始属性)", "path=goods.color[3], blue"})
goods.color = { "path=goods.color[1],red", "path=goods.color[2],yellow(原始属性)", "path=goods.color[3], blue" }
oneway_path_demo.goods = goods
goods.text3 = "goods.text3"
oneway_path_demo._bind_text4 = "path=bind_fun4() 绑定方法 表示参数"
---可绑定的方法 有参数的时候表示设置值，没有的时候获取值。
---解决lua没有get set的缺陷。
---@overload fun(arg:any):any
---@param arg any
---@return any
function oneway_path_demo.bind_text4(arg)
    if arg == nil then --get
        -- Logger.Log("_bind_text4 get=", oneway_path_demo._bind_text4)
        return oneway_path_demo._bind_text4
    else
        oneway_path_demo._bind_text4 = arg
        -- Logger.Log("_bind_text4 set=", arg)
    end
end

oneway_path_demo.on_input_changed = {
    CanExecute = function(self, arg)
        -- Logger.Log("onbtn1click,CanExecute", table.tojson(self), arg)
        return true
    end,
    Execute = function(self, arg)
        -- print(arg)
        -- print("oneway_path_demo", oneway_path_demo, " goods.", goods, "color", goods.color, "color[2]", goods.color[2],
        --     "arg", arg)
        goods:OnPropertyChanged(property_color)
        ---viewmodel属性通知view 写法1
        -- goods:OnPropertyChanged(property_color)
    end
}

oneway_path_demo.btn_test_1000 = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        print("oneway_path_demo.btn_test_1000", arg)
        local profiler = ProfilerFactory.GetAndStartProfiler("oneway_path_demo.btn_test_1000", nil, nil, true)

        for i = 0, 1000 do
            -- arg.text = goods.color[2]
            BindingExpression.m_SetTargetPropertyNoConvertInvoke(goods.color, "2", arg, "text", true, false, "", nil)
        end
        
        if profiler then
            profiler:Stop()
        end
    end
}



-- function oneway_path_demo:on_property_set(property)
--     goods:OnPropertyChanged(property_color)
-- end
print("init oneway_path_demo", oneway_path_demo, " goods.", goods, "color", goods.color, "color[2]", goods.color[2])
return oneway_path_demo
