---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome_view.lua
--data:2016..
--author:pu
--desc:view 
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local LuaHelper=LuaHelper

local view=class(Asset,function(self,item_obj)
    Asset._ctor(self, "welcome.u3d")
    self.item_obj=item_obj
end)

function view:on_asset_load(key,asset)
    self.item_obj:register_property_changed(self.databind,self)
    
    local fristView = LuaHelper.Find("Logo")
	if fristView then LuaHelper.Destroy(fristView) end
	local refer = LuaHelper.GetComponent(self.root,"Hugula.ReferGameObjects") 
	self.content_rect_table = refer:Get(1)

	self.content_rect_table.onItemRender=function(scroll_rect_item,index,dataItem)
		scroll_rect_item.gameObject:SetActive(true)
		scroll_rect_item:Get(1).text = dataItem.title --title
		scroll_rect_item:Get(2).name = dataItem.name --button
		scroll_rect_item.name = dataItem.name
	end

end

--资源加载完成后显示的时候调用
function view:show_eg_list(data)
	self.content_rect_table.data = data
	self.content_rect_table:Refresh(-1,-1) --显示列表

end

function view:dispose()
    self.item_obj:register_property_changed(self.databind,nil)
    self._base.dispose(self)
 end

function view:databind(item_obj,property_name)
    if property_name == "eg_data" then
        self:show_eg_list(item_obj:get_eg_data())
    end
end

return view