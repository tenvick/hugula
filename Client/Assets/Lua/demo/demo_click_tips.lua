------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription demo_click_tips
--  author 
--  date
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
--lua
local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
local CS = CS
local GlobalDispatcher = GlobalDispatcher
local DispatcherEvent = DispatcherEvent
local UnityEngine = CS.UnityEngine

---@class demo_click_tips:VMBase
---@type demo_click_tips
local demo_click_tips = VMBase()
demo_click_tips.views = {
    View(demo_click_tips, {key = "demo_click_tips"}) ---加载prefab
}

--------------------    绑定属性    --------------------
----  demo_click_tips  ----
demo_click_tips.pos= UnityEngine.Vector3(0,0,0)
demo_click_tips.sprite=""
demo_click_tips.tips=""
----  demo_click_tips end  --


--------------------    消息处理    --------------------


-------------------     公共方法    --------------------
local function get_equip_icon(i)
    return string.format("icon_equip_%04d", i % 39 + 1)
end

-------------------     事件响应    --------------------


--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function demo_click_tips:on_push_arg(arg)
    Logger.Log(" select ",arg)
    local property = self.property
    property.tips =  arg.name
    property.sprite = arg.icon
    self:OnPropertyChanged("pos") --强制刷新位置
end

--push到stack上时候调用
function demo_click_tips:on_push()
end

--从stack里返回激活调用
function demo_click_tips:on_back()
end

--view激活时候调用
function demo_click_tips:on_active()
    Logger.Log("demo_click_tips:on_active")
end

--view失活调用
function demo_click_tips:on_deactive()
    Logger.Log("demo_click_tips:on_deactive")
end

-- --状态切换之前
-- function demo_click_tips:on_state_changing()
-- end

-- --状态切换完成后
-- function demo_click_tips:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function demo_click_tips:on_destroy()
--     print("demo_click_tips:on_deactive")
-- end

--初始化方法只调用一次
-- function demo_click_tips:initialize()
--     -- body
-- end

return demo_click_tips
