------------------------------------------------
--  Copyright © 2016-2017   Hugula: Arpg game Engine
--	view 
--	author pu
------------------------------------------------
--view(view_name,item_name or item_obj,{sub_view_name,sub_item_name or sub_item_obj},...)
View = function( view_name,item_object,...)
	local tyn =type(item_object)
	if tyn == "string" then	item_object = LuaItemManager:get_item_object(item_object) end
	local parent = require("views."..view_name)(item_object)
	--find children
	local children = {...}
	local child_name,child_item_obj,child_view
	for k,v in ipairs(children) do
		child_name =v[1]
		child_item_obj = v[2]
		child_view = require("views.subviews."..v[1])
		if type(child_item_obj) == "string" then child_item_obj = LuaItemManager:get_item_clone_object(child_item_obj) end
		if child_item_obj == nil then child_item_obj = item_object end
		child_view = child_view(child_item_obj)
		parent:add_child(child_view)
	end

	return parent
end
