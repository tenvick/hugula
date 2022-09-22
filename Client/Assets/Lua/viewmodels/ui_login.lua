------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription ui_login
--  author 
--  date
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
--lua
local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
-- local CS = CS
-- local GlobalDispatcher = GlobalDispatcher
-- local DispatcherEvent = DispatcherEvent
local FileManifestManager = CS.Hugula.ResUpdate.FileManifestManager
---@class ui_login:VMBase
---@type ui_login
local ui_login = VMBase()
ui_login.views = {
    View(ui_login, {key = "ui_login"}) ---加载prefab
}

--------------------    绑定属性    --------------------
----  ui_login  ----
ui_login.input_txt_Password=""
ui_login.input_txt_Username=""
ui_login.Remember_me_is_on=false
ui_login.on_invalid_enable=false
----  ui_login end  --

ui_login.version = FileManifestManager.localVersion.."("..FileManifestManager.localResNum..")"
print(ui_login.version)
--------------------    消息处理    --------------------


-------------------     公共方法    --------------------


-------------------     事件响应    --------------------
ui_login.on_click_Sign_In= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        -- ui_login.property.on_invalid_enable = math.random(1,4) == 1
        if ui_login.Remember_me_is_on then
            VMState:push(VMGroup.game_reloading)
        else
            Logger.Log("Remember_me_is_on is false ")
        end
    end
}

ui_login.on_click_INVALID_PASSWORD= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)

    end
}

ui_login.on_value_changed_Remember_me= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)

    end
}

ui_login.on_submit_Password= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        ui_login.property.on_invalid_enable = math.random(1,2) == 1
        Logger.Log("on_submit_Password on_invalid_enable ",ui_login.on_invalid_enable)
    end
}

ui_login.on_submit_Username= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)

    end
}

ui_login.on_click_orange_button= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        ui_login.property.on_invalid_enable = math.random(1,4) == 1
        Logger.Log("on_click_orange_button on_invalid_enable ",ui_login.on_invalid_enable)

    end
}



--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function ui_login:on_push_arg(arg)
end

--push到stack上时候调用
function ui_login:on_push()
end

--从stack里返回激活调用
function ui_login:on_back()
end

--view激活时候调用
function ui_login:on_active()
    Logger.Log("ui_login:on_active hot update 2022.9.22 12:24")
end

--view失活调用
function ui_login:on_deactive()
    Logger.Log("ui_login:on_deactive")
end

-- --状态切换之前
-- function ui_login:on_state_changing()
-- end

-- --状态切换完成后
-- function ui_login:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function ui_login:on_destroy()
--     print("ui_login:on_deactive")
-- end

--初始化方法只调用一次
-- function ui_login:initialize()
--     -- body
-- end

return ui_login

--[[

vm_config.ui_login = {vm = "viewmodels.ui_login", gc_type = VM_GC_TYPE.ALWAYS} 

vm_group.ui_login = {"ui_login"}

---]]