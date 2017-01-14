---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: rootscroll_view.lua
--data:2016..
--author:pu
--desc:view 
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local LuaHelper=LuaHelper
local CSNameSpace = CSNameSpace

local view=class(Asset,function(self,item_obj)
    Asset._ctor(self, "rootscroll.u3d")
    self.item_obj=item_obj
end)

function view:on_asset_load(key,asset)
    self.item_obj:register_property_changed(self.databind,self)

    local refer = LuaHelper.GetComponent(self.root,"Hugula.ReferGameObjects") 
	self.scroll_table = refer:Get(1)
	self.scroll_table.onItemRender=function(scroll_rect_item,index,dataItem)
		scroll_rect_item.data = {index,dataItem}
		scroll_rect_item.gameObject:SetActive(true)
		local monos = scroll_rect_item.monos
		scroll_rect_item:Get(1).text = dataItem.title
		scroll_rect_item:Get(2).text = dataItem.name
		scroll_rect_item.name = dataItem.name
		local btn = monos[4]
		if index % 9 == 0 then
			btn.name = "click me!"
			monos[2].text = "ScrollTo 93"
		elseif index % 10 == 0 then
			btn.name = "delete me!"
			monos[2].text = "点我删除!"
		elseif index % 30 == 0 then
			btn.name = "click me back!"
			monos[2].text = "点我返回 0 !"
		else
			btn.name = "click "..index
		end
	end
end

function view:show_scroll_list(data)
    self.scroll_table.data= data
	self.scroll_table:Refresh(-1,-1)
end

function view:scroll_to(i)
	print("scroll_to",i)
    self.scroll_table:ScrollTo(i)
end

function view:remove_data_at(data)
	print(data[1]-1)
    local i = self.scroll_table:RemoveDataAt(data[1]-1)
    self.scroll_table:Refresh(i-1,-1)
    print(" you are remove "..i)
end

function view:add_data_at( data )
	local i = self.scroll_table:InsertData(data,1000)
    self.scroll_table:Refresh(i-1,-1)
	  print(" you are add "..i)
end

function view:dispose()
    self.item_obj:register_property_changed(self.databind,nil)
    self._base.dispose(self)
 end

function view:databind(item_obj,property_name)
    if property_name == item_obj.scroll_to then
        self:scroll_to(item_obj.scroll_to_index)
    elseif property_name == item_obj.remove_data_at then
        self:remove_data_at(item_obj.remove_data)
    elseif property_name == item_obj.set_scroll_data then
        self:show_scroll_list(item_obj.scroll_data)
	elseif property_name == item_obj.add_data_at then
		self:add_data_at(item_obj.add_data)
    end
end

return view