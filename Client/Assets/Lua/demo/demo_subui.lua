---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: demo_subui.lua
--data:2020.2.17
--author:pu
--desc:
--===============================================================================================--
---------------------------------------------------------------------------------------------------
local View = View
local NotifyTable = NotifyTable
local VMState = VMState
local VMGroup = VMGroup
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color

---@class demo_subui:VMBase
---@type demo_subui
local demo_subui = VMBase()

--UI资源
demo_subui.views = {
    View(demo_subui, { key = "demo_subui" }) --
}
----------------------------------申明属性名用于绑定--------------
local property_mail_list = "mail_list"

--------------------------------绑定属性-----------------
local mail_list = NotifyTable() ---数据变更功能

local function create_item(i)
    local it = {}
    it.title = "mail " .. i
    it.icon = string.format("icon_equip_%04d", i % 39 + 1)
    it.date = tostring(os.date())
    it.index = i
    return it
end

local function create_tmp_data()
    local datas = {}
    for i = 1, 20 do
        table.insert(datas, create_item(i))
    end
    return datas
end
mail_list:InsertRange(create_tmp_data()) ---初始化数据

-- ---按钮点击事件
-- mail_list.on_btn_click = {
--     CanExecute = function(self, arg)
--         return true
--     end,
--     Execute = function(self, arg)
--         Logger.Log("we will go to state :", self, "arg=", arg)
--         VMState:push(VMGroup[arg.state], arg.arg) -- scroll_rect_table
--     end
-- }

local index, last_index = 0, -1

demo_subui.mail_list = mail_list
demo_subui.select_item = nil
------------------------------------------------------
---列表选中
demo_subui.on_item_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        index = arg.selectedIndex

        local item = mail_list:get_Item(index)
        demo_subui.select_item = item
        Logger.Log("on_item_select", arg.selectedIndex, item, item.title)
        VMState:push_item("demo_subui1", item)
        -- if last_index >= 0 and index ~= last_index then
        --     local last_item = mail_list.items[last_index + 1]
        --     last_item.click_enable = false
        --     last_item.name = btn_tips1
        --     mail_list:set_Item(last_index, last_item) --更新数据
        -- end
        -- if index ~= last_index then
        --     item.name = btn_tips2
        --     item.click_enable = true
        --     -- end
        --     mail_list:set_Item(index, item) --更新数据
        -- end

        -- last_index = index
    end
}

demo_subui.btn_sct_del = {
    CanExecute = function(self, arg)
        return demo_subui.select_item ~= nil
    end,
    Execute = function(self, arg)
        local re =  mail_list:Remove(demo_subui.select_item)
        Logger.Log("del", demo_subui.select_item, demo_subui.select_item.title, "arg=", arg.selectedIndex,"del=",re)
    end
}

------------------public------------------
function demo_subui:on_active()
    Logger.Log("demo_subui. active mail_list=", demo_subui.mail_list)
end

function demo_subui:on_deactive()
    Logger.Log("demo_subui. active mail_list=", demo_subui.mail_list)
end

function demo_subui:on_destroy()
end

function demo_subui:on_property_set(property)
    Logger.Log("demo_subui. on_property_set", property)
end

return demo_subui
