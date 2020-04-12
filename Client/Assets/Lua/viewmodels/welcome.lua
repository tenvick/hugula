---------------------------------------------------------------------------------------------------
--===============================================================================================--
--filename: welcome.lua
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
---@class VMBase vm
---@class welcome
local welcome = VMBase()

--UI资源
welcome.views = {
    View(welcome, {asset_name = "welcome", res_path = "welcome.u3d"}) --
}
----------------------------------申明属性名用于绑定--------------
local property_eg_data = "eg_data"

--------------------------------绑定属性-----------------
local btn_tips1 = "选中点击"
local btn_tips2 = "点击前往"
local eg_data = NotifyTable() ---数据变更功能
eg_data:InsertRange(
    {
        {
            id = 1,
            title = "背包",
            name = btn_tips1,
            state = "bag",
            bgcolor = Color.cyan,
            click_enable = false
        },
        {
            id = 2,
            title = "数据绑定示例（绑定属性和方法）",
            name = btn_tips1,
            state = "binding_demo",
            bgcolor = Color.green,
            click_enable = false
        },
        {id = 3, title = "聊天", name = btn_tips1, state = "chat_demo", bgcolor = Color.yellow, click_enable = false}
        -- {id = 4, title = "俄罗斯方块", name = btn_tips1, state = "tetris", bgcolor = Color.blue, click_enable = false}
    }
)
---按钮点击事件
eg_data.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log("we will go to state :", self, "arg=", arg)
        VMState.push(VMGroup[arg.state]) -- scroll_rect_table
    end
}

local index, last_index
local last_color
---列表选中
eg_data.on_item_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log("click",self,arg)
        index = arg.selectedIndex
        last_index = arg.lastSelectedIndex
        -- local item = eg_data.items[index + 1]
        local item = eg_data:get_Item(index)
        if last_index >= 0 and index ~= last_index then
            local last_item = eg_data.items[last_index + 1]
            last_item.bgcolor = last_color
            last_item.click_enable = false
            last_item.name = btn_tips1
            eg_data:set_Item(last_index, last_item) --更新数据
        end
        if index ~= last_index then
            last_color = item.bgcolor
            if item.id == 4 then
                item.name = "敬请期待"
            else
                item.name = btn_tips2
                item.click_enable = not item.click_enable
            end
            Logger.Log("click", index, last_index, #eg_data.items, item.bgcolor)
            item.bgcolor = Color.white
            eg_data:set_Item(index, item) --更新数据
        end
    end
}

---事件变更监听
eg_data:CollectionChanged(
    "+",
    function(sender, arg)
        -- Logger.Log(
        --     sender,
        --     string.format(
        --         "eg_data:Action=%s,NewStartingIndex:%s,NewItems:%s,OldItems:%s,OldStartingIndex:%s",
        --         arg.Action,
        --         arg.NewStartingIndex,
        --         arg.NewItems,
        --         arg.OldItems,
        --         arg.OldStartingIndex
        --     )
        -- )
    end
)

welcome.eg_data = eg_data 

------------------public------------------
function welcome:on_active()
    Logger.Log("welcome. active eg_data=", welcome.eg_data)
end

function welcome:on_property_set(property)
    Logger.Log("welcome. on_property_set", property)
end

return welcome
