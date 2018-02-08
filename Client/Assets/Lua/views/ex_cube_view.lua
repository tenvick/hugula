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
    Asset._ctor(self, "ex_cube.u3d")
    self.item_obj=item_obj
end)

function view:on_asset_load(key,asset)
    self.item_obj:register_property_changed(self.databind,self)

end

function view:dispose()
    self.item_obj:register_property_changed(self.databind,nil)
    self._base.dispose(self)
 end

function view:databind(item_obj,property_name)
    if property_name == item_obj.on_hide then
        self:dispose()
    end
end

return view