------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local type = type
local ipairs = ipairs
local string_lower = string.lower
local table_insert = table.insert
local View = View
local VMState = VMState
local VMGroup = VMGroup
---@class VMBase vm
---@class asset_loader
local asset_loader = VMBase()

asset_loader.views = {}
asset_loader.auto_context = false
-------------------------------------------------
--加载的
function asset_loader:set_views(tab)
    -- self:clear()
    local views = {}

    for k, v in ipairs(tab) do
        local view1 = View(self, {key = v.key}) --, res_path = v.res_path})
        table_insert(views, view1)
        Logger.Log(v.key)
    end

    asset_loader.views = views
end


function asset_loader:on_push_arg(arg)
    if arg and arg.asset_loader then
        self:set_views(arg.asset_loader)
    end
end

function asset_loader:on_active()
end

function asset_loader:on_deactive()

end

function asset_loader:on_destroy()
    self:clear()

    Logger.Log(" asset_loader:on_destroy")
end

return asset_loader
