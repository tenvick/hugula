------------------------------------------------
--  Copyright © 2013-2020   Hugula: Arpg game Engine
--
--  author pu
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup

local DIS_TYPE = DIS_TYPE
local LuaDispatcher = LuaDispatcher
local Rpc = Rpc

local CUtils = CS.Hugula.Utils.CUtils

---@class VMBase vm
---@class login
local login = VMBase()
login.views = {
    View(login, {asset_name = "login", res_path = "login.u3d"}), ---加载prefab
}

----------------------------------申明属性名用于绑定--------------
local property_user_name = "user_name"
local property_password = "password"
local property_login_tips="login_tips"
local property_group_index = "group_index"
-------------------------------------------------
login.group_index = 0
login.password = "" --双向绑定
login._user_name = CS.UnityEngine.SystemInfo.deviceUniqueIdentifier
function login.user_name(data) --双向绑定
    if data ~= nil then
        login._user_name = data
    end
    return login._user_name
end

function login.on_login_act(data)
    local ip = data.ip
    local port = data.port
    Logger.Log("on_login_act",ip,port)
    login:SetProperty(property_login_tips,string.format("登录成功 ip:%s,port:%d",ip,port))
    login:SetProperty(property_group_index,1)
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
    LuaDispatcher:binding(DIS_TYPE.LOGIN_PACKET_ACK, login.on_login_act)
end


--view失活调用
function login:on_deactive()
    LuaDispatcher:unbinding(DIS_TYPE.LOGIN_PACKET_ACK, login.on_login_act)
end

-- --在销毁的时候调用此函数
-- function login:on_destroy()
--     print("login:on_deactive")
-- end

---点击事件
login.on_btn_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        local username = login.user_name()
        local pwd = login.pwd
        Logger.Log(" ", username, pwd, CUtils.platform)
        Rpc:Msg_LoginReq({account = username, password = pwd, platform = CUtils.platform})
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
return login
