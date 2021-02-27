------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--  discription 热更新lua
--  author pu
------------------------------------------------
--C#
local CS = CS
local GlobalDispatcher = GlobalDispatcher
local DispatcherEvent = DispatcherEvent

local hot_reload = {}

function hot_reload.listenf5()
    VMState:_reload_top()
end

function hot_reload:initialize()
    local GlobalDispatcher = GlobalDispatcher.instance
    GlobalDispatcher:AddListener(DispatcherEvent.F5, hot_reload.listenf5)
end

hot_reload:initialize()

return hot_reload
