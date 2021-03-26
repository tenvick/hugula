------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription
--  author
--  data
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
--lua
local DIS_TYPE = DIS_TYPE
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
local CS = CS
local GlobalDispatcher = GlobalDispatcher
local DispatcherEvent = DispatcherEvent

---@class demo_subui1:VMBase
---@type demo_subui1
local demo_subui1 = VMBase()
demo_subui1.views = {
    View(demo_subui1, {key = "demo_subui1"}) ---加载prefab
}

--------------------    绑定属性    --------------------
demo_subui1.title = ""
demo_subui1.icon = ""
demo_subui1.date = os.date()
demo_subui1.detail = ""

local function create_str(len)
    local str = ""
    local rand = 0
    for i = 1, len do
        -- math.randomseed(i)
        if i == 1 then
            rand = string.char(math.random(0, 25) + 65)
        elseif math.random(1, 4) <= 3 then
            rand = string.char(math.random(0, 25) + 97)
        elseif math.random(1, 4) <= 3 then
            rand = " "
        else
            rand = " ." .. string.char(math.random(0, 25) + 65)
        end
        str = str .. rand
    end
    return str
end
--------------------    消息处理    --------------------

-------------------     公共方法    --------------------

function demo_subui1:refresh_data(data)
    local property = self.property
    property.title = data.title
    property.icon = data.icon
    property.date = data.date
    local clen = math.random(50, 300)
    local detail = create_str(clen)
    property.detail = detail
end
--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function demo_subui1:on_push_arg(arg)
    self:refresh_data(arg)
end

--push到stack上时候调用
function demo_subui1:on_push()
end

--从stack里返回激活调用
function demo_subui1:on_back()
end

--view激活时候调用
function demo_subui1:on_active()
    Logger.Log("demo_subui1:on_active")
end

--view失活调用
function demo_subui1:on_deactive()
    Logger.Log("demo_subui1:on_deactive")
end

-- --状态切换之前
-- function demo_subui1:on_state_changing()
-- end

-- --状态切换完成后
-- function demo_subui1:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function demo_subui1:on_destroy()
--     print("demo_subui1:on_deactive")
-- end

--初始化方法只调用一次
-- function demo_subui1:initialize()
--     -- body
-- end
--------------------    事件响应    --------------------
demo_subui1.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        -- VMState:push(VMGroup.welcome)
    end
}

return demo_subui1
