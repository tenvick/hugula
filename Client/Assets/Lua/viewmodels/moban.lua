------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription
--  author
--  date
------------------------------------------------
local View = View   --View资源加载器
local VMState = VMState --ViewModel 栈和状态机
local VMGroup = VMGroup --ViewModel 组
--lua
local DIS_TYPE = DIS_TYPE
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
local CS = CS
local GlobalDispatcher = GlobalDispatcher
local DispatcherEvent = DispatcherEvent

---@class moban:VMBase
---@type moban
local moban = VMBase() --VMBase是带通知功能的NotifyObject
--------------------    关联界面    --------------------
moban.views = {
    View(moban, {key = "assetName"}) ---加载prefab
}

--------------------    绑定属性    --------------------
-- local items = NotifyTable() --带通知的table，数据集合变更时候通知view更新
-- moban.items =items

--------------------    消息处理    --------------------
-- moban.msg[DIS_TYPE.NET_CONNECT_SUCCESS] = function()
--     -- body
-- end
-------------------     公共方法    --------------------

--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function moban:on_push_arg(arg)
end

--从stack里返回激活调用
function moban:on_back()
end

--view资源加载完成后调用
function moban:on_assets_load()

end

--view激活时候调用
function moban:on_active()
    Logger.Log("moban:on_active")
end

--view失活调用
function moban:on_deactive()
    Logger.Log("moban:on_deactive")
end

-- --状态切换之前
-- function moban:on_state_changing()
-- end

-- --状态切换完成后
-- function moban:on_state_changed(last_group_name,top_group_name)
-- end

-- --在销毁的时候调用此函数
-- function moban:on_destroy()
--     print("moban:on_deactive")
-- end

--初始化方法只调用一次
-- function moban:initialize()
--     -- body
-- end
--------------------    事件响应    --------------------
moban.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        -- VMState:push(VMGroup.welcome)
    end
}

return moban
