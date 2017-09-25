------------------------------------------------
--  Copyright © 2013-2014   Hugula: Arpg game Engine
--   
--  author pu
------------------------------------------------
local transform = LuaItemManager:get_item_object("transform")
transform.priority = 100
transform.log_enable = false
transform.assets=
{
  Asset("loadingui.u3d")
}

function transform:on_focus( ... )
    if self:check_assets_loaded() then 
        self:show()  
        self:on_showed()     
    else
        self.asset_loader:load(self.assets)  
    end
end

function transform:dispose( ... ) --override dispose need't dispose on reload

end

function transform:on_click( ... )
	return true
end

function transform:on_assets_load(items)
  self:show()
end
