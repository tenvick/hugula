---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: moban_view.lua
--data:2016..
--author:pu
--desc:view 
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local LuaHelper=LuaHelper
local CSNameSpace = CSNameSpace

local view=class(Asset,function(self,item_obj)
    Asset._ctor(self,"uipublic.u3d",{"Alert"})
    self.item_obj=item_obj
end)

function view:on_asset_load(key,asset)
    self.item_obj:register_property_changed(self.databind,self)

    -- print("alertTips ................. is loaded")
    self.alert_refer = LuaHelper.GetComponent(self.items["Alert"],"Hugula.ReferGameObjects") 
    self.tips_label=alert_refer:Get(0)

    self:set_text(self.item_obj:get_text())
end

function view:set_text(text)
    if text~= nil then
        self.tips_label.text = text
    end
end

function view:dispose()
    self.item_obj:register_property_changed(self.databind,nil)
    self._base.dispose(self)
 end

function view:databind(item_obj,property_name)
    if property_name == item_obj.set_text then
        self:set_text(item_obj:get_text())
    end
end

return view