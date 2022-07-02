---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: moban_view.lua
--data:2020..
--author:
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------
local moban_view = ViewBase()
---加载资源
moban_view.key = "scroll_rect"  ---指定资源名
--加载场景
moban_view.scene_name = "myscene" --表示加载场景
moban_view.load_scene_mode = CS.UnityEngine.SceneManagement.LoadSceneMode.Additive  --LoadSceneMode.Single
moban_view.allow_scene_activation = true --激活场景

function moban_view:on_asset_load(key,asset) ---逻辑实现，一般情况不需要代码。
	Logger.Log(key,asset)
end

return moban_view