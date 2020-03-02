---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: scroll_rect_demo_table.lua
--data:2022.2.22
--author:pu
--desc:大数据滚动列表
--===============================================================================================--
---------------------------------------------------------------------------------------------------
local View = View
local NotifyTable = NotifyTable
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color
---@class VMBase vm
---@class welcome
local scroll_rect_demo = VMBase()

--UI资源
scroll_rect_demo.views = {
    View(scroll_rect_demo, "views.rootscroll_view")
}
--------------------------------------ui数据-------------------------------------
---
scroll_rect_demo.input_count = "1"
scroll_rect_demo.input_index = "1"
---
----------------------------------------列表数据-----------------------------
local scroll_data = NotifyTable()

scroll_data.scroll_to_index = nil
scroll_data.input_scroll_index = 0
---删除数据
scroll_data.on_item_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        -- Logger.Log(" delete ", arg)
        scroll_data:RemoveRange({arg})
    end
}

---滚动到索引
scroll_data.btn_scroll_to = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        scroll_data:SetProperty("scroll_to_index", scroll_data.input_scroll_index)
    end
}

local function create_tmp_data()
    local datas = {}
    for i = 1, 10 do
        local it = {}
        it.name = "del " .. i
        it.title = "title" .. i
        table.insert(datas, it)
    end
    return datas
end
scroll_data:InsertRange(create_tmp_data()) ---初始化数据

scroll_rect_demo.scroll_data = scroll_data

-- scroll_data:CollectionChanged(
--     "+",
--     function(sender, arg)
--         Logger.Log(
--             string.format(
--                 "eg_data:Action=%s,NewStartingIndex:%s,NewItems:%s,OldItems:%s,OldStartingIndex:%s",
--                 arg.Action,
--                 arg.NewStartingIndex,
--                 arg.NewItems,
--                 arg.OldItems,
--                 arg.OldStartingIndex
--             )
--         )
--     end
-- )
---------------------------------点击事件处理------------------------------------
---点击返回按钮
scroll_rect_demo.on_back = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log(" on back")
        VMState.back()
    end
}

---点击添加数据按钮
scroll_rect_demo.on_add_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(...)
        local size = #scroll_data.items + 1
        scroll_data:Add({name = "insert " .. size, title = "insert title" .. size})
    end
}

---点击插入数据
scroll_rect_demo.on_insert_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(...)
        local size = #scroll_data.items
        local index = tonumber(scroll_rect_demo.input_index or "1")
        local count = tonumber(scroll_rect_demo.input_count or "1")

        local range = {}
        for i = 1, count do
            table.insert(range, {name = " insert " .. (size + i), title = "insert title" .. (size + i)})
        end
        scroll_data:InsertRange(index, range)
        Logger.Log(string.format("insert_range(index=%s,#range=%s,items=%s)", index, #range, #scroll_data.items))
    end
}

return scroll_rect_demo
