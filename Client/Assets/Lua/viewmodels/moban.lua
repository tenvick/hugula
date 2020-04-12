------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
---@class VMBase vm
---@class moban
local moban = VMBase()

moban.views = {
    View(moban, {find_path = "/Logo"}, "views.moban_view1"), --- asset以key以"find_path"开头表示使用GameObject.Find("moban")
    View(moban, {asset_name="assetName",res_path="assetbundle.u3d"}), ---加载prefab
    View(moban, {scene_name="scene_name",res_path="assetbundle.u3d"}), ---加载场景
    View(moban, "moban.moban_view") ---关联视图，资源在视图中设置asset_name=xxx和res_path=xxx.u3d 属性
}

----------------------------------申明属性名用于绑定--------------
local property_enable_slider = "enable_slider"
local property_btntext = "btntext"
-------------------------------------------------
---绑定属性
moban.enable_slider = true
moban.btntext = "拖动滑动条"

---当vmbase的属性被设置的时候回调次方法
function moban:on_property_set(property)
    if property == "value" and moban.slider1_value > 0.5 then
        -- Logger.Log("value ",moban.slider1_value)
        ---设置属性改变写法1
        moban:set_property(property_enable_slider, (not moban.enable_slider)) ---改变source属性的值并通知view
        ---设置属性改变写法2
        moban.btn_interactable = (not moban.enable_slider)
        moban:property_changed(property_btn_interactable) ---通知view source属性改变

        moban:set_property(property_btntext, "现在可以点击我了")
    end
end

function moban:on_active()
    print("moban:on_active")
end

function moban:on_deactive()
    print("moban:on_deactive")
end

---点击事件
moban.on_btn_click = {
    CanExecute = function(self,arg)
        return true
    end,
    Execute = function(self,arg)
        -- VMState.push(VMGroup.welcome)
    end
}

return moban
