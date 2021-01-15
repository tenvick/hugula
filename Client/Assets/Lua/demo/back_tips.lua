------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup

---@class back_tips:VMBase
---@type back_tipsVMBase
local back_tips = VMBase()
--back_tips 以LuaModule挂接在GameObject上不需要指定views
back_tips.views = {
    View(back_tips, {key = "back_tips"}) ---加载prefab
}

function back_tips:on_push()
    Logger.Log("back_tips:on_push()")
end

---点击返回按钮
back_tips.on_btn_back = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        -- Logger.Log(" on back")
        VMState:back()
    end
}
return back_tips
