------------------------------------------------
--  Copyright © 2013-2024   Hugula mvvm framework
--  discription bag
--  author Administrator
--  date 2024-10-24 19:44:10
------------------------------------------------
local View = View
local VMState = VMState
local NotifyTable = NotifyTable
local NotifyObject = NotifyObject
--lua
local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
local CS = CS
local ValueConverterRegister = CS.Hugula.Databinding.ValueConverterRegister.instance

---@class bag:VMBase
---@type bag
local bag = VMBase()
bag.views = {
    View(bag, { key = "bag" }) ---加载prefab
}
--------------------    定义变量    --------------------
local property_selected_trigger = "selected_trigger"
local equ_type = { 1, 2, 3, 4, 5 }
local tab_que_cate = { { 1, 2, 3, 5, 11, 12, 13, 14, 15, 21, 22, 23, 24, 25, 31, 32, 33, 34, 35, 438 },
    { 6,   7,   8,   9,   10,  16,  17,  18,  19,  20, 26, 27, 28, 29, 30, 36, 37, 38, 39, 40, },
    { 587, 588, 589, 590, 591 },
    { 291, 292, 293, 294, 295, 296 },
    { 301, 302, 306, 308, 311, 314, 319, 320, 438, 440 }
}

local tab_left_txt = { "武器", "装备", "药水", "资源", "其他" }
local function get_equip_icon(cateid)
    return string.format("icon_equip_%04d", cateid)
end

--模拟背包项数据
local function create_item(i, cate)
    local it = {}
    it.name = "item " .. i
    it.cate = cate -- equ_type[i % 5 + 1]
    local cate_arr = tab_que_cate[it.cate]
    it.equip_id = cate_arr[math.random(1, #cate_arr)]
    it.icon = get_equip_icon(it.equip_id)
    it.quality = tostring(math.random(0, 10))
    it.count = tostring(math.random(1, 10))
    it.selected = false
    it.index = i
    return it
end

--模拟100条背包数据
local function create_test_bag_data()
    local datas = {}
    for i = 0, 199 do
        local cate = i % 5 + 1

        table.insert(datas, create_item(i,equ_type[cate]))
    end
    return datas
end

--模拟tab数据
local function create_tab_left()
    local its = {}
    local len = #tab_left_txt
    for i = 1, len do
        local it = {}
        it.tab_txt = tab_left_txt[i]
        it.tab_idx = i
        table.insert(its, it)
    end
    return its
end

bag.test_bag_data = create_test_bag_data() ---初始化背包数据
bag.test_tab_data = create_tab_left()      ---初始化tab页面数据
--------------------    模板生成 绑定属性    --------------------
----  bag begin  ----

local bag_items = NotifyTable()
bag.items = bag_items
----  items begin  ----
--items_item.icon = ""
bag_items.name = ""
bag_items.scroll_to_index = 0
--items_item.quality = ""
----  items end  --

-- local bag_selected_item = NotifyObject()
bag.selected_item = nil
----  selected_item begin  ----
----  selected_item end  --

local bag_tab_left = NotifyTable()
bag.tab_left = bag_tab_left
----  tab_left begin  ----
--tab_left_item.tab_txt = ""
----  tab_left end  --


bag.selected_trigger = true
bag.text = {}
bag.tab_idx = 0
bag.sel_idx = 0 --选中的tab页面
bag.lsv_selected_idx = -1
----  bag end  --


-------------------    模板生成 方法    --------------------

function bag:add_items(data) bag.items:Add(data) end

function bag:update_items(data)
    local index = bag.items:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then bag.items:set_Item(index, data) end
end

function bag:remove_items(data)
    local index = bag.items:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then bag.items:RemoveAt(index) end
end

function bag:clear_items() bag.items:Clear() end

function bag:add_tab_left(data) bag.tab_left:Add(data) end

function bag:update_tab_left(data)
    local index = bag.tab_left:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then bag.tab_left:set_Item(index, data) end
end

function bag:remove_tab_left(data)
    local index = bag.tab_left:FindIndex(function(i, v) return v.eid == data.eid end)
    if index ~= nil then bag.tab_left:RemoveAt(index) end
end

function bag:clear_tab_left() bag.tab_left:Clear() end

-------------------    模板生成 事件响应    --------------------
bag.btn_go = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        local i = tonumber(arg.text)
        print("btn_go", arg,arg.text)
        bag_items:SetProperty("scroll_to_index", i,true)
        -- bag.property.sel_idx = i --改变选中的tab页面
        DelayFrame(
            function()
                bag.property.lsv_selected_idx = i --选中
            end,
            24
        )
        -- bag:SetProperty("lsv_selected_idx",i,true) --强制刷新
    end
}
bag.on_add_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        bag:buy_item()
    end
}
bag.on_use_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        print("on_use_click", arg.selectedIndex)
        bag:use_item(arg.selectedIndex,1)
    end
}
bag.on_item_context = function(bc, item)
    if item == nil then return end
    -- declare
    local _count = bc["count"] --Text
    local _name = bc["name"]   --Text

    -- fill
    _count.text = item.count
    _name.text = item.name
end
bag.on_item_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        local index = arg.selectedIndex
        --播放动画
        bag:OnPropertyChanged(property_selected_trigger)
        bag.lsv_selected_idx = index
        -- 显示详细
        local item_data = bag.items:get_Item(index)
        print("on_item_select", index, item_data and item_data.name)
        --刷新详细数据
        bag.property.selected_item = item_data
        -- DelayFrame(
        --     function()
        --         print("selected_item", item_data.name)
        --     end,
        --     24
        -- )
    end
}
bag.on_select_render = function(bc, item)

    if item == nil then return end
    -- declare
    local _Icon = bc["Icon"]     --ImageBinder
    local _Border = bc["Border"] --ImageBinder
    local _Text = bc["Text"]     --Text
    local _nameText = bc["nameText"] --Text

    local convert = ValueConverterRegister:Get("StringToSprite")
    -- fill
    _Icon.spriteName = item.icon
    _Border.sprite = convert:Convert(item.quality)
    _Text.text = item.count
    _nameText.text = item.name

    print("on_select_render", item.index,item.name)
end
bag.on_tab_sel = {
    CanExecute = function(self, arg)
        print("on_tab_sel.CanExecute", arg.selectedIndex,arg.lastSelectedIndex)
        if (arg.selectedIndex == 2) then 
            print("on_tab_sel.你不能选择", arg.selectedIndex,arg.lastSelectedIndex)
            return false 
        end
        return true
    end,
    Execute = function(self, arg)
        print("on_tab_sel.Execute", arg.selectedIndex,arg.lastSelectedIndex)

        bag:sel_tab_item(arg.selectedIndex+1)
    end
}


--------------------    自定义  消息处理    --------------------


-------------------     自定义 方法   --------------------
bag.tab_left:InsertRange(bag.test_tab_data) ---初始化tab页面数据

function bag:filter_bag_data_by_cate(cate)
    local test_bag_data = bag.test_bag_data
    local filter_data = {}
    for i = 1, #test_bag_data do
        local item = test_bag_data[i]
        if item.cate == cate then
            table.insert(filter_data, item)
        end
    end

    bag.items:Clear()
    bag.items:InsertRange(filter_data)
end

function bag:sel_tab_item(idx)
    bag.property.tab_idx = idx --缓存索引
    bag.property.lsv_selected_idx = -1 --清除选中
    bag.property.selected_item = nil

    bag:filter_bag_data_by_cate(idx) --过滤数据
end

function bag:use_item(idx,count)
    if (idx < 0) then return end
    local item = bag.items:get_Item(idx)
    item.count = item.count - count
    print("use_item",idx,count,item.count)
    if item.count <= 0 then
        bag.items:RemoveAt(idx)
        local idx = table.indexof(bag.test_bag_data,item)
        if idx ~= nil then
            table.remove(bag.test_bag_data, idx)
        end

        bag.property.lsv_selected_idx = -1
        -- bag:SetProperty("lsv_selected_idx",-1,true) --强制刷新
        bag.property.selected_item = nil
    else
        bag.items:set_Item(idx, item) --更新数据
        bag:OnPropertyChanged("selected_item")-- --强制刷新
    end    
end

function bag:buy_item(...)
    local size = #bag.items.items + math.random(1, 5)
    local item = create_item(size,equ_type[ bag.tab_idx])
    table.insert(bag.test_bag_data,item )
     bag.items:Add(item)
end

--------------------    生命周期    --------------------
function bag:on_push_arg(arg)

end

--view资源全部加载完成时候调用
function bag:on_assets_load()
    -- Logger.Log("bag:on_assets_load")
    -- bag:sel_tab_item(1)
end

function bag:on_active()
    Logger.Log("bag:on_active")
end

function bag:on_deactive()
    VMState:popup_item("demo_click_tips")
end

-- --在销毁的时候调用此函数
-- function bag:on_destroy()
--     print("bag:on_deactive")
-- end

--初始化方法只调用一次
-- function bag:initialize()
--     -- body
-- end

return bag

--[[

vm_config.bag = {vm = "viewmodels.bag", gc_type = VM_GC_TYPE.ALWAYS}
vm_group.bag = {"bag"}

---]]
