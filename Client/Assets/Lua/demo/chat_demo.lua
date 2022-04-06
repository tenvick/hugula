------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
local NotifyTable = NotifyTable
local NotifyObject = NotifyObject
local math = math
local string = string
local tostring = tostring
local table_insert = table.insert
local os = os

---@class chat_demo:VMBase
---@type chat_demo
local chat_demo = VMBase()

----------------------------------申明属性名用于绑定--------------
local property_chat_input_txt = "chat_input_txt"
-------------------------------------------------
chat_demo.views = {
    View(chat_demo, {key = "chat_demo"}) ---直接加载资源使用默认视图
}
local chat_data = NotifyTable()

local function create_str(len)
    local str = ""
    local rand = 0
    for i = 1, len do
        -- math.randomseed(i)
        if i == 1 then
            rand = string.char(math.random(0, 25) + 65)
        elseif math.random(1, 4) <= 3 then
            rand = string.char(math.random(0, 25) + 97)
        elseif math.random(1, 4) <= 3 then
            rand = " "
        else
            rand = " ."..string.char(math.random(0, 25) + 65)
        end
        str = str .. rand
    end
    return str
end

local function create_bag_item(i)
    local it = {}
    it.name = "item " .. i
    it.icon = string.format("icon_equip_%04d", i % 39 + 1)
    it.quality = tostring(math.random(0, 10))
    it.count = tostring(math.random(1, 5))
    it.selected = false
    it.idx = i
    return it
end

local function create_tmp_data()
    local datas = {}
    local count = math.random(3, 16)
    for i = 1, count do
        table.insert(datas, create_bag_item(i))
    end
    return datas
end


local gInt = 0

local function create_talk_data(i)
    local d = NotifyObject()
    d.type = math.random(0, 4)
    if d.type == 3 then ---表示分割线
        d.chat_content = tostring(os.date())
    elseif d.type == 4 then
        d.tips = "bag："..create_str(math.random(40, 60))
        d.bag_data = NotifyTable() --点击后生成数据
    else
        d.user_name = string.format("%s_%s", create_str(3), i)
        local tick = os.time()
        local clen = math.random(6, 300)
        d.chat_content = create_str(clen)
        d.chat_time = tostring(os.date("%H:%M:%S"))
        if d.type == 2 then
            d.name = "system"
        end
    end
    d.id = gInt

    gInt = gInt + 1
    return d
end

local function add_chat_data(min, max,index)
    local count = math.random(min, max)
    local range = {}
    for i = 0, count do
        local d = create_talk_data(i)
        table_insert(range, d)
    end
    chat_data:InsertRange(index,range)
end

local function add_my_chat(input)
    local d = create_talk_data(0)
    d.user_name = chat_demo.user_name
    d.chat_content = input
    d.type = 1
    chat_data:InsertRange({d})
end

local function add_chat_tips(tips)
    local d = {}
    d.chat_content = tips
    d.type = 3
    chat_data:InsertRange({d})
end


local function add_chat_random_data()
    chat_data.property.loading_data = false --取消加载状态
    add_chat_data(8, 10,0)
end
-----------------------------------------------------
-------------------------绑定属性---------------------
-----------------------------------------------------
chat_demo.chat_data = chat_data

chat_data.goto_index = -1
chat_data.loading_data = false --加载状态

---属性直接绑定方法 有参数的时候表示设置值，没有的时候表示获取值
function chat_demo.chat_input_txt(arg)
    -- Logger.Log("chat_input_txt", arg)
    return ""
end

---模板类型查找函数
function chat_data.on_get_item_template_type(comp, index)
    local d = chat_data.items[index + 1]
    return d.type or 0
end

chat_data.dropped_cmd = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        if arg.y > 0 then --下拉插入数据
            chat_data.property.loading_data = true --标记为加载状态
            Delay(add_chat_random_data,math.random(1, 3))
        end
    end
}


chat_data.on_system_click =  { 
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        
        local clen = math.random(150, 600)
        arg.property.chat_content = create_str(clen) --系统按钮点自适应内容高度
    end
}

--点击背包响应事件
chat_data.on_bag_click =  {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        arg.bag_data:InsertRange(create_tmp_data()) --绑定背包数据
    end
}

chat_demo.on_goto_click = 
{
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        print("跳转",arg)
        chat_data.property.goto_index = tonumber(arg)
    end
}

---发送按钮
chat_demo.chat_btn_send = {
    CanExecute = function(self, arg)
        if arg == "" then
            add_chat_tips("聊天内容不能为空！")
            return false
        else
            return true
        end
    end,
    Execute = function(self, arg)
        add_my_chat(arg) --显示自定义发送数据
        chat_demo:OnPropertyChanged(property_chat_input_txt) ---清空聊天内容
        print("hot update test 111")

    end
}

function chat_demo:on_active()
    chat_demo.user_name = "on_active " .. create_str(6)
    add_chat_data(5, 15) --随机显示聊天记录
end

return chat_demo
