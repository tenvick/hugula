------------------------------------------------
--  Copyright © 2013-2021   Hugula mvvm framework
--  discription ui_effect_loading
--  author 
--  date
------------------------------------------------
local View = View
local VMState = VMState
local VMGroup = VMGroup
local table               = table
local type                = type
local string_format       = string.format
local math_floor          = math.floor
local ipairs              = ipairs
local math                = math
--lua
local Delay               = Delay
local DIS_TYPE = DIS_TYPE
-- local lua_binding = lua_binding
-- local lua_unbinding = lua_unbinding
local Rpc = Rpc
--C#
-- local CS = CS
-- local GlobalDispatcher = GlobalDispatcher
-- local DispatcherEvent = DispatcherEvent
local Timer = Timer
local ResLoader           = CS.Hugula.ResLoader
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

----------------------------------初始值设置--------------
local property_slider_value = "slider_value"
local property_map_loading_text = "map_loading_text"
ui_effect_loading.slider_value = 0
local loading_percent = 0
local real_per = 0
-------------------     公共方法    --------------------

function ui_effect_loading.on_progress(map_loadingEvent)
    -- body
    local per = map_loadingEvent.current / map_loadingEvent.total
    real_per = per
    if per <= loading_percent then per = loading_percent end
        ui_effect_loading.update_ui(per)
end

function ui_effect_loading.update_ui(percent)
    -- body
    Logger.Log("ui_inner_loading.update_ui ", percent)
    ui_effect_loading:SetProperty(property_slider_value, percent)
    ui_effect_loading:SetProperty("count",   string_format("loading %02d%%",math_floor(percent * 100)))
end

function ui_effect_loading:simulate_progress()
    -- body
    if loading_percent <= real_per then loading_percent = real_per end
    loading_percent = loading_percent + 0.034
    if loading_percent <= 0.9 then
        ui_effect_loading.update_ui(loading_percent)
    end
end

-------------------     事件响应    --------------------
------ITransition-------传输接口------------

local loading_groups_call = {}

local arg_arr = {}
function ui_effect_loading:RegisterActiveGroupFun(vm_group,begin_active_group)
    loading_groups_call[vm_group] = begin_active_group
    table.insert(arg_arr,vm_group)
end

function ui_effect_loading:DoActiveGroup(vm_group)
    if vm_group == nil  then 
        local c_group = arg_arr[1]
        table.remove(arg_arr,1)
        vm_group = c_group
    end

    if vm_group == nil then 
        Logger.LogWarning("ui_effect_transition:DoActiveGroup currGrop is nil")
        return 
    end

    local done =loading_groups_call[vm_group]
    if type(done) == "function" then
        done()
    end
    loading_groups_call[vm_group] = nil
end


function ui_effect_loading:ActiveGroupDone(group)
    -- Delay(function()
        VMState:deactive("ui_effect_loading")
    -- end, 0.2)
end

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
        local t = nil;
        t =  Timer.Add(0.05,100,function()
            property.alpha = ui_effect_loading.alpha -0.1 --CS.UnityEngine.Time.deltaTime*.2
        -- Logger.Log("property.alpha",property.alpha,bl)
            if property.alpha <= 0 then
                property.alpha = 0
                Timer.Remove(t)
                
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
    ResLoader.OnGroupProgress = ui_effect_loading.on_progress
    loading_percent = 0
    real_per = 0
    self:start_timer(0.05, 60, ui_effect_loading.simulate_progress, self)

    local property = ui_effect_loading.property
    local t = nil;
    t = Timer.Add(0.05,100,function()
        property.alpha = ui_effect_loading.alpha + 0.1
        Logger.Log("property.alpha",property.alpha)
        if property.alpha >= 1 then
            Timer.Remove(t)
            --真正开始切换
            ui_effect_loading:DoActiveGroup() --开始激活组
            ResLoader.EndMarkGroup()

        end
    end,nil)

end

--view失活调用
function ui_effect_loading:on_deactive()
    ResLoader.OnGroupProgress = nil
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