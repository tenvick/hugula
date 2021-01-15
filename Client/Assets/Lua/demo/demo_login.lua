------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local require = require
local View = View
local VMState = VMState
local VMGroup = VMGroup

local DIS_TYPE = DIS_TYPE
local lua_binding = lua_binding
local lua_unbinding = lua_unbinding
local Rpc = Rpc

local login_model = require("models.login_model")

local CUtils = CS.Hugula.Utils.CUtils
local NetworkProxy = CS.Hugula.Net.NetworkProxy
---@class login:VMBase
---@type login
local login = VMBase()
login.views = {
    View(login, {key = "login"}) ---加载prefab
}

----------------------------------申明属性名用于绑定--------------
local property_user_name = "user_name"
local property_password = "password"
local property_login_tips = "login_tips"
local property_group_index = "group_index"
local property_is_connected = "is_connected"
-------------------------------------------------
login.is_connected = false
login.group_index = 0
login.password = "" --双向绑定
login._user_name = CS.UnityEngine.SystemInfo.deviceUniqueIdentifier

------------------------------------------------
local server_list = NotifyTable() ---数据变更功能

server_list:InsertRange(
    {
        {
            id = 101,
            server_name = "杨聪"
        },
        {
            id = 201,
            server_name = "于江波"
        }
    }
)

login.server_list = server_list
------------------------------------------------
function login.user_name(data) --双向绑定
    if data ~= nil then
        login._user_name = data
    end
    return login._user_name
end

------------------自动消息监听-------------------
--监听lua_binding(DIS_TYPE.LOGIN_PACKET_ACK
function login.msg.login_packet_act(code)
    -- local ip = data.ip
    -- local port = data.port
    Logger.Log("on_login_act", code)
    login.property.login_tips = string.format("code = %s", code)
    login.property.group_index = 1
    VMState:push("loading", "RenderDemo") -- scroll_rect_table
end

--相当于监听 lua_binding(DIS_TYPE.NET_CONNECT_SUCCESS)
function login.msg.net_connect_success()
    login.property.is_connected =true
end

------------------------------------------------
function login:on_push_arg(arg)
end

--push到stack上时候调用
function login:on_push()
end

--从stack里返回激活调用
function login:on_back()
end

--view激活时候调用
function login:on_active()
    local ips = "10.23.0.41" --"10.23.7.63"
    local port = 20002
    local WARNET_PING_PORT = 5000 --端口
    local isEncode = false
    CS.Hugula.Net.NetWork.main:Connect(ips, ips, port, isEncode)

    -- NetworkProxy.main:Connect(ips,port,WARNET_PING_PORT,WARNET_IS_ENCODE)

    -- lua_binding(DIS_TYPE.NET_CONNECT_SUCCESS, login.on_connection)
    -- lua_binding(DIS_TYPE.LOGIN_PACKET_ACK, login.on_login_act)
end

--view失活调用
function login:on_deactive()
    -- lua_unbinding(DIS_TYPE.NET_CONNECT_SUCCESS, login.on_connection)
    -- lua_unbinding(DIS_TYPE.LOGIN_PACKET_ACK, login.on_login_act)
end

-- --在销毁的时候调用此函数
-- function login:on_destroy()
--     print("login:on_deactive")
-- end

--选中服务器
login.on_server_select = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        -- index = arg.selectedIndex
        Logger.Log("Execute ", arg, arg.context.id)
        login.selected_server_index = arg.context.id
    end
}

---点击事件
login.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        local username = login.user_name()
        local pwd = login.pwd
        Logger.Log(" ", username, pwd, CUtils.platform)
        if login.selected_server_index ~= nil then
            login_model.send_join_game(login.selected_server_index, username)
        end
        -- Rpc:Msg_LoginReq({account = username, password = pwd, platform = CUtils.platform})
    end
}

---点击事件
login.on_back_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        VMState:back()
    end
}

login_model:listen_connect()

return login
