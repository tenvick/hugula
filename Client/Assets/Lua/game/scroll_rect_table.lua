---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: scroll_rect_table.lua
--data:2016.3.19
--author:pu
--desc:大数据滚动列表
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local scroll_rect_table = LuaItemManager:get_item_obejct("scroll_rect_table")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
scroll_rect_table.assets=
{
    Asset("rootscroll.u3d")
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
-- 资源加载完成时候调用方法
function scroll_rect_table:on_assets_load(items)
	local refer = LuaHelper.GetComponent(self.assets[1].root,"Hugula.ReferGameObjects") 
	scroll_table = refer:Get(1)
	scroll_table.onItemRender=function(scroll_rect_item,index,dataItem)
		scroll_rect_item.data = {index,dataItem}
		scroll_rect_item.gameObject:SetActive(true)
		local monos = scroll_rect_item.monos
		scroll_rect_item:Get(1).text = dataItem.title
		scroll_rect_item:Get(2).text = dataItem.name
		scroll_rect_item.name = dataItem.name
		local btn = monos[4]
		if index % 30 == 0 then
			btn.name = "click me!"
			monos[2].text = "ScrollTo 93"
		elseif index % 39 == 1 then
			btn.name = "delete me!"
			monos[2].text = "点我删除!"
		elseif index % 100 == 0 or index % 90 == 0 then
			btn.name = "click me back!"
			monos[2].text = "点我返回 0 !"
		else
			btn.name = "click "..index
		end
	end
end


--列表点击事件 Button绑定CEventReceive.OnCustomerEvent
function scroll_rect_table:on_customer(obj,arg)
	local name =obj.name
	print("scroll_rect_table:on_customer"..name)
   if name == "click me!" then
		scroll_table:ScrollTo(10+math.random(100))
	elseif name == "delete me!" then
		print(arg)
		local data = arg.data
		local i = scroll_table:RemoveDataAt(data[1])
		-- local i = scroll_table:RemoveDataAt(math.random(#scroll_table.data)) --随机删除
		-- scroll_table:Refresh(scroll_table.headIndex,-1)
		scroll_table:Refresh(i,-1)
		print(" you are remove "..i)
	elseif name == "click me back!" then
		scroll_table:ScrollTo(0)
	end
end

--点击事件
function scroll_rect_table:on_click(obj,arg)
	local cmd = obj.name
	if cmd == "Back" then
		StateManager:set_current_state(StateManager.welcome)
	end
end

--每次显示时候调用
function scroll_rect_table:on_showed( ... )
	scroll_table.data= create_tmp_data()
	scroll_table:Refresh(-1,-1)
end

--初始化函数只会调用一次
function scroll_rect_table:initialize()

end