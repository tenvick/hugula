------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup

---@class binding_demo:VMBase
---@type binding_demo
local binding_demo = VMBase()

----------------------------------申明属性名用于绑定--------------

-------------------------------------------------
binding_demo.views = {
    View(binding_demo, {key = "binding_demo", res_path = "binding_demo.u3d"}) ---使用默认view视图
}

----界面PanelOneWayVM 和界面PanelTwoWayVM 共同使用vm
binding_demo.oneway_path_demo = require("demo.binding_demo.path_binding_demo")
-----界面PanelFormat不需要代码
-----界面PanelTwoWayComponent不需要代码
-----界面PanelFormat不需要代码

--- 界面 PanelInteractableComponent
binding_demo.enable_slider = true
binding_demo.btntext = "拖动滑动条"
binding_demo.slider1_value = 0 ---双向绑定
binding_demo.btn_interactable = false

binding_demo.on_slider_value = {
    Execute = function(self, arg)
        ---viewmodel属性通知view 写法1
        if binding_demo.slider1_value > 0.5 then
            binding_demo.property.btn_interactable = true
            ---viewmodel属性通知view 写法1
            binding_demo:SetProperty("btntext", "现在可以点击我了")
        else
            ---viewmodel属性通知view 写法1
            binding_demo.property.btn_interactable = false
            binding_demo.property.btntext = "拖动试试。"
        end
    end
}

---点击事件
binding_demo.on_btn_click = {
    CanExecute = function(self, arg)
        -- Logger.Log("onbtn1click,CanExecute", table.tojson(self), arg)
        return true
    end,
    Execute = function(self, arg)
        ---viewmodel属性通知view 写法1
        binding_demo.property.enable_slider = (not binding_demo.enable_slider)
        if binding_demo.slider1_value > 0.9 then
        -- VMState:push(VMGroup.welcome)
        end
    end
}

return binding_demo
