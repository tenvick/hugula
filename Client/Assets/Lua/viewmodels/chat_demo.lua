------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
local NotifyTable = NotifyTable
local math = math
local string = string
local tostring = tostring
local table_insert = table.insert
local os = os
---@class VMBase vm
---@class chat_demo
local chat_demo = VMBase()

----------------------------------申明属性名用于绑定--------------
local property_chat_input_txt = "chat_input_txt"
local property_chat_btn_send = "chat_btn_send"
-------------------------------------------------
chat_demo.views = {
    View(chat_demo, {asset_name = "chat_demo", res_path = "chat_demo.u3d"}) ---直接加载资源使用默认视图
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

local gInt = 0

local function create_talk_data(i)
    local d = {}
    d.type = math.random(0, 3)
    if d.type == 3 then ---表示分割线
        d.chat_content = tostring(os.date())
    else
        d.user_name = string.format("ap_%s_%s", create_str(3), i)
        local tick = os.time()
        local clen = math.random(50, 300)
        d.chat_content = create_str(clen)
        d.chat_time = tostring(os.date("%H:%M:%S"))
        d.id = gInt
        if d.type == 2 then
            d.name = "system"
        end
        gInt = gInt + 1
    end
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

-----------------------------------------------------
-------------------------绑定属性---------------------
-----------------------------------------------------
chat_demo.chat_data = chat_data

---模板类型查找函数
function chat_data.on_get_item_template_type(comp, index)
    local d = chat_data.items[index + 1]
    -- Logger.Log("on_get_item_template_type idx=%s,type=%s ",index,d.type)
    return d.type or 0
end

chat_data.dropped_cmd = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log("dropped_cmd", table.tojson(self), "arg=",arg,".")
        if arg.y > 0 then
            add_chat_data(8, 10,0)
        end
    end
}

chat_data.on_system_click =  {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log("dropped_cmd", table.tojson(self), "arg=",arg,".")
        -- if arg.y > 0 then
            add_chat_data(1, 1)
        -- end
    end
}

---属性直接绑定方法 有参数的时候表示设置值，没有的时候表示获取值
function chat_demo.chat_input_txt(arg)
    -- Logger.Log("chat_input_txt", arg)
    return ""
end

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
        Logger.Log("chat_btn_send", table.tojson(self), "arg=",arg,".")
        add_my_chat(arg)
        chat_demo:OnPropertyChanged(property_chat_input_txt) ---情况聊天内容
    end
}

function chat_demo:on_active()
    chat_demo.user_name = "on_active " .. create_str(6)
    add_chat_data(10, 50) --随机显示聊天记录
end

return chat_demo
