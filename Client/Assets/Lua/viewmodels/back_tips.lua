------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
---@class VMBase vm
---@class back_tips
local back_tips = VMBase()
--back_tips 以LuaModule挂接在GameObject上不需要指定views
back_tips.views = {
    View(back_tips, {asset_name = "back_tips", res_path = "back_tips.u3d"}) ---加载prefab
}

---点击返回按钮
back_tips.on_back = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log(" on back")
        VMState.back()
    end
}
return back_tips
