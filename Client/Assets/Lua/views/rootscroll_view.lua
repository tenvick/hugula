---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: rootscroll_view.lua
--data:2020.2.22
--author:pu
--desc:大数据滚动列表示例 
--===============================================================================================--
---------------------------------------------------------------------------------------------------
local rootscroll_view= ViewBase()
---指定了资源
rootscroll_view.asset_name = "scroll_rect"
rootscroll_view.res_path = "scroll_rect.u3d"

function rootscroll_view:on_asset_load(key,asset)
	Logger.Log(key,asset)
end

return rootscroll_view