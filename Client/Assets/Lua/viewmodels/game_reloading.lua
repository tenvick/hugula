------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription 游戏重启
--  author  pu
--  date    2021/6/15
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
local require = require
local release_require = release_require
--lua
-- local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
-- local Rpc = Rpc
--C#
local Hugula = CS.Hugula

local EnterLua = Hugula.EnterLua
local LuaHelper = Hugula.Utils.LuaHelper
local ResLoader = Hugula.ResLoader
local BackGroundDownload = Hugula.ResUpdate.BackGroundDownload
local NetWork = Hugula.Net.NetWork

---@class game_reloading:VMBase
---@type game_reloading
local game_reloading = VMBase()
game_reloading.views = {}
game_reloading.log_enable = false
--------------------    绑定属性    --------------------

--------------------    消息处理    --------------------

local function re_loading(...)
    Logger.LogSys("game_reloading:re_loading() ")
    EnterLua.MarkStopAllCoroutines()

    Logger.LogSys("release lua_dispatcher and global_cs_dis_bridge ")
    release_require("net.lua_dispatcher")
    release_require("general.global_cs_dis_bridge")
    LuaHelper.Destroy(BackGroundDownload.instance.gameObject)
    EnterLua.ReOpen(0) --直接开始begin
end

-------------------     公共方法    --------------------

--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function game_reloading:on_push_arg(arg)
    CS.Hugula.Framework.Timer:Clear()
    LuaHelper.Destroy(NetWork.main.gameObject)
end

--push到stack上时候调用
function game_reloading:on_push()
end

--从stack里返回激活调用
function game_reloading:on_back()
end

--view激活时候调用
function game_reloading:on_active()
    Logger.LogSys("game_reloading:on_active")
    Delay(re_loading, 0.01)
end

--view失活调用
function game_reloading:on_deactive()
    -- Logger.Log("game_reloading:on_deactive")
end

-- --状态切换之前
-- function game_reloading:on_state_changing()
-- end

-- --状态切换完成后
-- function game_reloading:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function game_reloading:on_destroy()
--     print("game_reloading:on_deactive")
-- end

--初始化方法只调用一次
-- function game_reloading:initialize()
--     -- body
-- end
--------------------    事件响应    --------------------

return game_reloading
