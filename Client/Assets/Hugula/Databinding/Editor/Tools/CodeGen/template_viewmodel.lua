------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription {name}
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

---@class {name}:VMBase
---@type {name}
local {name} = VMBase()
{name}.views = {
    View({name}, {key = "{name}"}) ---加载prefab
}

--------------------    绑定属性    --------------------
{property}

--------------------    消息处理    --------------------
{message}

-------------------     公共方法    --------------------
{method}

-------------------     事件响应    --------------------
{command}

--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function {name}:on_push_arg(arg)
end

--push到stack上时候调用
function {name}:on_push()
end

--从stack里返回激活调用
function {name}:on_back()
end

--view激活时候调用
function {name}:on_active()
    Logger.Log("{name}:on_active")
end

--view失活调用
function {name}:on_deactive()
    Logger.Log("{name}:on_deactive")
end

-- --状态切换之前
-- function {name}:on_state_changing()
-- end

-- --状态切换完成后
-- function {name}:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function {name}:on_destroy()
--     print("{name}:on_deactive")
-- end

--初始化方法只调用一次
-- function {name}:initialize()
--     -- body
-- end

return {name}
