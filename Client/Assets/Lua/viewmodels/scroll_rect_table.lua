---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: scroll_rect_table.lua
--data:2016.3.19
--author:pu
--desc:大数据滚动列表
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local scroll_rect_table = LuaItemManager:get_item_object("scroll_rect_table")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
scroll_rect_table.assets=
{
    View("rootscroll_view",scroll_rect_table)
}

------------------private-----------------
local scroll_table

local function create_tmp_data()
	local datas={}
	for i=0,1000 do
		local it ={}
		it.name="name "..i
		it.title ="title"..i
		table.insert(datas,it)
	end
	return datas
end
------------------public------------------
function scroll_rect_table:set_scroll_data(data)
	self.scroll_data = data
	self:raise_property_changed(self.set_scroll_data)
end

function scroll_rect_table:scroll_to(i)
	self.scroll_to_index = i
	self:raise_property_changed(self.scroll_to)
end

function scroll_rect_table:remove_data_at(data)
	self.remove_data = data
	self:raise_property_changed(self.remove_data_at)
end

function scroll_rect_table:add_data_at(data)
	self.add_data = data
	self:raise_property_changed(self.add_data_at)
end

--列表点击事件 Button绑定CEventReceive.OnCustomerEvent
function scroll_rect_table:on_customer(obj,arg)
	local name =obj.name
	print("scroll_rect_table:on_customer"..name)
   if name == "click me!" then
		self:scroll_to(10+math.random(1000))
	elseif name == "delete me!" then
		print(arg)
		local data = arg.data
		self:remove_data_at(data)
	elseif name == "click me back!" then
		self:scroll_to(0)
	end
end

function scroll_rect_table:on_showed()
	self:set_scroll_data(create_tmp_data())
end

--点击事件
function scroll_rect_table:on_click(obj,arg)
	local cmd = obj.name
	if cmd == "Back" then
		StateManager:set_current_state(StateManager.welcome)
	elseif cmd == "BtnADD" then
		local it ={}
		it.name="name "..math.random()*1000
		it.title ="title"..math.random()*1000
		self:add_data_at(it)
	end
end
