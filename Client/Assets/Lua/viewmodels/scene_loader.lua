------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local type = type
local string_lower = string.lower
local table_insert = table.insert
local View = View
local VMState = VMState
local VMGroup = VMGroup
---@class VMBase vm
---@class scene_loader
local scene_loader = VMBase()

scene_loader.views = {}
scene_loader.auto_context = false

-------------------------------------------------

function scene_loader:on_push_arg(arg)
    if arg ~= nil and arg.scene_loader then
        local args = arg.scene_loader
        local scene_name, res_path = "", ""
        if type(args) == "string" then
            scene_name = args
            res_path = string_lower(scene_name) .. ".u3d"
        else
            scene_name = args.scene_name
            res_path = args.res_path
        end
        self:clear()
        --
        local views = scene_loader.views
        local view1
        if (#views > 0) then
            view1 = views[1]
            view1.scene_name = scene_name
            view1.res_path = res_path
            -- Logger.Log("on_push_arg ", scene_name, res_path)
        else
            view1 = View(self, {scene_name = scene_name, res_path = res_path}) --,load_scene_mode = CS.UnityEngine.SceneManagement.LoadSceneMode.Single})
            table_insert(views, view1)
            -- Logger.Log("on_push_arg View(", scene_name, res_path)
        end
    end
end

function scene_loader:on_active()
end

function scene_loader:on_deactive()
end

return scene_loader
