---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: bag_table.lua
--data:2022.2.22
--author:pu
--desc:背包
--===============================================================================================--
---------------------------------------------------------------------------------------------------
local View = View
local NotifyTable = NotifyTable
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color
---@class VMBase vm
---@class welcome
local bag = VMBase()
--UI资源
bag.views = {
    View(bag, {asset_name = "bag", res_path = "bag.u3d"})
}
----------------------------------申明属性名用于绑定--------------
local property_selected_item = "selected_item"
local property_selected_trigger = "selected_trigger"
-------------------------------------------------
--------------------------------------ui数据-------------------------------------
---
bag.input_count = "1"
bag.input_index = "1"
---
----------------------------------------列表数据-----------------------------
local items = NotifyTable()

items.scroll_to_index = nil
items.input_scroll_index = 0
---删除数据
items.on_item_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        -- Logger.Log(" delete ", arg)
        items:RemoveRange({arg})
    end
}

---滚动到索引
items.btn_scroll_to = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        items:SetProperty("scroll_to_index", items.input_scroll_index)
    end
}

local function get_equip_icon(i)
    return string.format("icon_equip_%04d", i % 39 + 1)
end

local function create_item(i)
    local it = {}
    it.name = "item " .. i
    it.icon = get_equip_icon(i)
    it.quality = tostring(math.random(0, 10))
    it.count = tostring(math.random(1, 5))
    it.selected = false
    return it
end

local function create_tmp_data()
    local datas = {}
    for i = 1, 60 do
        table.insert(datas, create_item(i))
    end
    return datas
end
items:InsertRange(create_tmp_data()) ---初始化数据

bag.items = items

-- items:CollectionChanged(
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
bag.selected_item = nil
bag.selected_trigger = "fadeIn"

local index, last_index

bag.on_item_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        index = arg.selectedIndex
        last_index = arg.lastSelectedIndex
        --播放动画
        bag:OnPropertyChanged(property_selected_trigger)

        --选中状态
        local item_data = items:get_Item(index)
        item_data.selected = true
        items:set_Item(index, item_data) --更新数据

        --取消上个选中
        if last_index >= 0 and last_index ~= index then
            local last_item = items:get_Item(last_index)
            last_item.selected = false
            items:set_Item(last_index, last_item) --更新数据
        end

        --刷新详细数据
        DelayFrame(
            function()
                bag:SetProperty(property_selected_item, item_data)
            end,
            24
        )
    end
}

---点击添加数据按钮
bag.on_add_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(...)
        local size = #items.items + 1
        items:Add(create_item(size))
    end
}

---点击插入数据
bag.on_insert_click = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(...)
        local size = #items.items
        local index = tonumber(bag.input_index or "1")
        local count = tonumber(bag.input_count or "1")

        local range = {}
        for i = 1, count do
            table.insert(range, create_tmp_data(size + i))
        end
        items:InsertRange(index, range)
        Logger.Log(string.format("insert_range(index=%s,#range=%s,items=%s)", index, #range, #items.items))
    end
}

return bag
