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
---@class bag:VMBase
---@type bag
local bag = VMBase()
--UI资源
bag.views = {
    View(bag, {key = "bag"})
}
----------------------------------申明属性名用于绑定--------------
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
    it.index = i
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

function bag:on_deactive()
    -- lua_unbinding(DIS_TYPE.NET_CONNECT_SUCCESS, login.on_connection)
    -- lua_unbinding(DIS_TYPE.LOGIN_PACKET_ACK, login.on_login_act)
    --close tips
    VMState:popup_item("demo_click_tips")
end


---------------------------------点击事件处理------------------------------------
bag.selected_item = nil
bag.selected_trigger = "fadeIn"

bag.on_item_changed = {
    CanExecute = function(...)
        return true
    end,
    Execute = function(self, arg)
        local arg0 = arg[0]
        local arg1 = arg[1]

        local old_item = items:get_Item(arg0 - 1)
        local new_item = items:get_Item(arg1 - 1)
        if not old_item or not new_item then
            return
        end
        -- if not items:get_Item(arg0) or not items:get_Item(arg1) then return end
        old_item.index = arg1
        new_item.index = arg0
        items:Move(arg0 - 1, arg1 - 1)
        Logger.LogTable(items.items)
    end
}

bag.on_item_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        local index = arg.selectedIndex
        --播放动画
        bag:OnPropertyChanged(property_selected_trigger)

        -- 显示详细
        local item_data = items:get_Item(index)
        --刷新详细数据
        DelayFrame(
            function()
                bag.property.selected_item = item_data
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

---------------------------------渲染处理------------------------------------
function bag.on_item_context(bc,item_data)
    bc:Get(0).text = item_data.count
    --or 
    -- bc:Get("count").text =item_data.count
end


--显示选中的项
local valueConverterRegister = CS.Hugula.Databinding.ValueConverterRegister.instance
function bag.on_select_render(select, item)
    select:Get(0).spriteName = item.icon
    select:Get(1).sprite =  valueConverterRegister:Get("StringToSprite"):Convert(item.quality)
    select:Get(2).text = item.count
end

return bag
