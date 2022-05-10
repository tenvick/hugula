------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription player_load
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
local CS = CS
-- local GlobalDispatcher = GlobalDispatcher
-- local DispatcherEvent = DispatcherEvent
local FileManifestManager = CS.Hugula.ResUpdate.FileManifestManager
local BackGroundDownload = CS.Hugula.ResUpdate.BackGroundDownload
local ResLoader = CS.Hugula.ResLoader
---@class player_load:VMBase
---@type player_load
local player_load = VMBase()
player_load.views = {
    View(player_load, {key = "player_load"}) ---加载prefab
}

--------------------    绑定属性    --------------------
----  player_load  ----
----  player_load end  --

--------------------    消息处理    --------------------

-------------------     公共方法    --------------------

-------------------     事件响应    --------------------
player_load.on_load_click = {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        if not FileManifestManager.CheckAddressIsDown(arg) then
            local zip_name = arg
            CS.Hugula.UI.MessageBox.Show(
                zip_name .. " 没有下载",
                arg.title,
                "确定下载",
                function()
                    CS.Hugula.UI.MessageBox.Destroy()
                    Logger.Log(zip_name)
                    local size = BackGroundDownload.instance:AddZipFolderByAddress(zip_name)
                    if size <= 0 then
                        Logger.Log("没有找到相关资源")
                    else
                        BackGroundDownload.instance:Begin()
                        Logger.Log("开启下载")
                    end
                end
            )
        end
    end
}

player_load.on_show_hero= {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)
        Logger.Log("on_show_hero:",arg)
        ResLoader.InstantiateAsync(arg,function(gobj,user)  
            Logger.Log(gobj,user)
            gobj.transform.position = CS.UnityEngine.Vector3(math.random(-14,3),0,math.random(-8,5))
        end,nil)
    end
}


--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function player_load:on_push_arg(arg)
end

--从stack里返回激活调用
function player_load:on_back()
end

--view资源全部加载完成时候调用
function player_load:on_assets_load()
    Logger.Log("player_load:on_assets_load")
end

--view激活时候调用
function player_load:on_active()
    Logger.Log("player_load:on_active")
end

--view失活调用
function player_load:on_deactive()
    Logger.Log("player_load:on_deactive")
end

-- --状态切换之前
-- function player_load:on_state_changing()
-- end

-- --状态切换完成后
-- function player_load:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function player_load:on_destroy()
--     print("player_load:on_deactive")
-- end

--初始化方法只调用一次
-- function player_load:initialize()
--     -- body
-- end

return player_load

--[[

vm_config.player_load = {vm = "viewmodels.player_load", gc_type = VM_GC_TYPE.ALWAYS} 

vm_group.player_load = {"player_load"}

---]]
