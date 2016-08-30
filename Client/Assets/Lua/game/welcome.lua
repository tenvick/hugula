---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
--data:2015.4.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local welcome = LuaItemManager:get_item_obejct("welcome")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
welcome.assets=
{
     Asset("welcome.u3d")
}

------------------private-----------------
local content_rect_table --内容列表
local eg_data = {
	{title="俄罗斯方块",name="tetris"},
	{title="（大数据）滚动列表",name="scroll_rect_table"},
	{title="动态加载场景",name="load_scene"},
	{title="扩展包资源加载",name="load_extends"}

}
------------------public------------------
-- 资源加载完成时候调用方法
function welcome:on_assets_load(items)
	local fristView = LuaHelper.Find("Logo")
	if fristView then LuaHelper.Destroy(fristView) end
	local refer = LuaHelper.GetComponent(self.assets[1].root,"Hugula.ReferGameObjects") 
	content_rect_table = refer:Get(1)

	content_rect_table.onItemRender=function(scroll_rect_item,index,dataItem)
		scroll_rect_item.gameObject:SetActive(true)
		scroll_rect_item:Get(1).text = dataItem.title --title
		scroll_rect_item:Get(2).name = dataItem.name --button
		scroll_rect_item.name = dataItem.name
	end

	
end

--资源加载完成后显示的时候调用
function welcome:on_showed()

	content_rect_table.data = eg_data
	content_rect_table:Refresh(-1,-1) --显示列表

end

--列表点击事件 Button绑定CEventReceive.OnCustomerEvent
function welcome:on_customer(obj,arg)
	local cmd =obj.name
    print("welcome  click "..cmd)
    Loader:set_active_variants({"sd"})
    StateManager:set_current_state(StateManager[cmd]) --切换到对应状态
end

--点击事件
function welcome:on_click(obj,arg)
	
end


--显示时候调用
function welcome:onShowed( ... )

end

--初始化函数只会调用一次
function welcome:initialize()

end