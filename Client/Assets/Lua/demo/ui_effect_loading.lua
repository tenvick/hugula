------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription ui_effect_loading
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

---@class ui_effect_loading:VMBase
---@type ui_effect_loading
local ui_effect_loading = VMBase()
ui_effect_loading.views = {
    View(ui_effect_loading, {key = "ui_effect_loading"}) ---加载prefab
}

--
--------------------    绑定属性    --------------------
----  ui_effect_loading  ----
ui_effect_loading.alpha=0
----  ui_effect_loading end  --
---
ui_effect_loading.auto_transition = false --手动控制切换效果 
ui_effect_loading.always_transition = true --不管资源是否加载完成都执行界面切换

--------------------    消息处理    --------------------


-------------------     公共方法    --------------------


-------------------     事件响应    --------------------

--当transition资源加载完成时候接管 transistion操作
-- function ui_effect_loading:on_transition_done(group)
--     local transition = group and group.transition
--     if transition then
--         ui_effect_loading:start_delay(function() 
--         VMState:deactive(transition)  --手动隐藏loading界面
--         end,0.2)
--     end
-- end

function ui_effect_loading:_base_set_views_active(bl)
    local views = self.views or {}
    for k, view_base in ipairs(views) do
        view_base:set_active(bl)
    end
end

--override
function ui_effect_loading:set_views_active(bl)
    Logger.Log("ui_effect_loading:set_views_active",bl)
    if bl == true then
        ui_effect_loading:_base_set_views_active(bl)
    else
        local property = ui_effect_loading.property

        ui_effect_loading:start_timer(0.05,-1,function()
            property.alpha = ui_effect_loading.alpha -0.1 --CS.UnityEngine.Time.deltaTime*.2
        -- Logger.Log("property.alpha",property.alpha,bl)

            if property.alpha <= 0 then
                property.alpha = 0
                ui_effect_loading:stop_all_timer()
                ui_effect_loading:_base_set_views_active(false) --延时关闭
            end
        end)
     
    end
end
--------------------    生命周期    --------------------

--VMState:push(vm_name,arg) push过来的arg，此时view资源可能还没准备好
function ui_effect_loading:on_push_arg(arg)
    ui_effect_loading.alpha=0
end

--从stack里返回激活调用
function ui_effect_loading:on_back()
end

--view资源全部加载完成时候调用
function ui_effect_loading:on_assets_load()
    Logger.Log("ui_effect_loading:on_assets_load")
end

--view激活时候调用
function ui_effect_loading:on_active()
    Logger.Log("ui_effect_loading:on_active")
    local property = ui_effect_loading.property
    ui_effect_loading:start_timer(0.05,-1,function()

        property.alpha = ui_effect_loading.alpha + 0.1
        -- Logger.Log("property.alpha",property.alpha)
        if property.alpha >= 1 then
            ui_effect_loading:stop_all_timer()
            --真正开始切换
            ui_effect_loading:do_transition()
        end
    end,nil)

end

--view失活调用
function ui_effect_loading:on_deactive()
    Logger.Log("ui_effect_loading:on_deactive")
end

-- --状态切换之前
-- function ui_effect_loading:on_state_changing()
-- end

-- --状态切换完成后
-- function ui_effect_loading:on_state_changed(last_group_name)
-- end

-- --在销毁的时候调用此函数
-- function ui_effect_loading:on_destroy()
--     print("ui_effect_loading:on_deactive")
-- end

--初始化方法只调用一次
-- function ui_effect_loading:initialize()
--     -- body
-- end

return ui_effect_loading

--[[

vm_config.ui_effect_loading = {vm = "viewmodels.ui_effect_loading", gc_type = VM_GC_TYPE.ALWAYS} 

vm_group.ui_effect_loading = {"ui_effect_loading"}

---]]