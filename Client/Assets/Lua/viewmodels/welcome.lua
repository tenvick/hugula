---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
--data:2015.4.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------

local welcome = LuaItemManager:get_item_object("welcome")
local StateManager = StateManager
local delay = delay
local LuaHelper=LuaHelper
local CUtils = CUtils
local get_value = get_value --多国语言

--UI资源
welcome.assets=
{
     View("welcome_view",welcome)
}

------------------private-----------------
local eg_data = {
	{title="俄罗斯方块",name="tetris"},
	{title="（大数据）滚动列表",name="scroll_rect_table"},
	{title="动态加载场景",name="load_scene"},
	{title="扩展包资源加载",name="load_extends"}

}
------------------public------------------
function welcome:get_eg_data()
	return self.eg_data
end

function welcome:set_eg_data(val)
	self:set_property("eg_data",val) --设置属性触发属性改变事件
	print(self.eg_data)
end

--资源加载完成后显示的时候调用
function welcome:on_showed()
	self:set_eg_data(eg_data)
end

--列表点击事件 Button绑定CEventReceive.OnCustomerEvent
function welcome:on_customer(obj,arg)
	local cmd =obj.name
    print("welcome  click "..cmd)
    Loader:set_active_variants({"hd"})
    StateManager:set_current_state(StateManager[cmd]) --切换到对应状态
end

--初始化函数只会调用一次
function welcome:initialize()

end