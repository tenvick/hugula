------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
---@class VMBase vm
---@class binding_demo
local binding_demo = VMBase()

----------------------------------申明属性名用于绑定--------------
local property_enable_slider = "enable_slider"
local property_btntext = "btntext"
local property_slider1_value = "slider1_value"
local property_btn_interactable = "btn_interactable"
-------------------------------------------------
binding_demo.views = {
    View(binding_demo, {asset_name="binding_demo",res_path="binding_demo.u3d"}) ---使用默认view视图
}

----界面PanelOneWayVM 和界面PanelTwoWayVM 共同使用vm
binding_demo.oneway_path_demo = require("viewmodels.binding_demo.path_binding_demo")
-----界面PanelFormat不需要代码
-----界面PanelTwoWayComponent不需要代码
-----界面PanelFormat不需要代码

--- 界面 PanelInteractableComponent
binding_demo.enable_slider = true
binding_demo.btntext = "拖动滑动条"
binding_demo.slider1_value = 0 ---双向绑定
binding_demo.btn_interactable = false
---重载方法
function binding_demo:on_property_set(property)
    if property == property_slider1_value then

        if binding_demo.slider1_value > 0.5 then
            ---viewmodel属性通知view 写法2
            binding_demo.btn_interactable = true
            binding_demo:OnPropertyChanged(property_btn_interactable) ---通知view source属性改变
            ---viewmodel属性通知view 写法1
            binding_demo:SetProperty(property_btntext, "现在可以点击我了")
        else
            ---viewmodel属性通知view 写法1
            binding_demo:SetProperty(property_btn_interactable, false) ---改变btn_interactable属性的值并通知view
            binding_demo:SetProperty(property_btntext, "拖动试试。")
        end
    end
end

---点击事件
binding_demo.on_btn_click = {
    CanExecute = function(self, arg)
        -- Logger.Log("onbtn1click,CanExecute", table.tojson(self), arg)
        return true
    end,
    Execute = function(self, arg)
        ---viewmodel属性通知view 写法1
        binding_demo:SetProperty(property_enable_slider, (not binding_demo.enable_slider)) ---改变source属性的值并通知view

        -- binding_demo:set_property(property_enable_slider, (not binding_demo.enable_slider)) ---通知改变
        if binding_demo.slider1_value > 0.9 then
        -- VMState:push(VMGroup.welcome)
        end
    end
}

return binding_demo
