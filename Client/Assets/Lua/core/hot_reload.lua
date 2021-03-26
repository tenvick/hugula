------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription 热更新lua
--  author pu
------------------------------------------------
--C#
local CS = CS
local GlobalDispatcher = GlobalDispatcher
local DispatcherEvent = DispatcherEvent
local VMState = VMState
local hot_reload = {}

function hot_reload.listenf5()
    VMState:_reload_top_one()
end

function hot_reload.listenf6()
    VMState:_reload_top_active()
end

function hot_reload:initialize()
    local GlobalDispatcher = GlobalDispatcher.instance
    GlobalDispatcher:AddListener(DispatcherEvent.F5, hot_reload.listenf5)
    GlobalDispatcher:AddListener(DispatcherEvent.F6, hot_reload.listenf6)
end

hot_reload:initialize()

return hot_reload
