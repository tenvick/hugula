local CS = CS
local UnityEngine = CS.UnityEngine
local Hugula = CS.Hugula
local manager = Hugula.Framework.Manager
local EnterLua = CS.Hugula.EnterLua
local NeedProfileDump = not Hugula.Profiler.ProfilerFactory.DoNotProfile
---------------
Timer                 = Hugula.Framework.Timer
local timer           = Timer
local type            = type
local debug           = debug

if NeedProfileDump then
    local _Delay       = timer.Delay
    local _Add         = timer.Add
    local _DelayFrame  = timer.DelayFrame
    local SetTimerName = timer.SetTimerName

    Delay              = function(func, time, arg)
        local info = debug.getinfo(func)
        local name = info and info["source"] .. ":" .. info["linedefined"] or ""
        local id = _Delay(func, time, arg, name)
        SetTimerName(id, name)
        return id
    end

    DelayFrame         = function(func, frame, arg)
        local info = debug.getinfo(func)
        local name = info and info["source"] .. ":" .. info["linedefined"] or ""
        local id = _DelayFrame(func, frame, arg)
        SetTimerName(id, name)
        return id
    end


    timer.Add        = function(delay, arg2, arg3, arg4)
        local v1 = type(arg2) == "function"
        local name = ""
        local fun = v1 == true and arg2 or arg3
        local info = debug.getinfo(fun)
        name = info and info["source"] .. ":" .. info["linedefined"] or ""
        local id = _Add(delay, arg2, arg3, arg4)

        SetTimerName(id, name)

        return id
    end

    timer.Delay      = function(func, time, arg, name)
        local name = ""
        local info = debug.getinfo(func)
        name = info and info["source"] .. ":" .. info["linedefined"] or ""

        local id = _Delay(func, time, arg, name)
        SetTimerName(id, name)
        return id
    end

    timer.DelayFrame = function(action, frame, arg)
        local name = ""
        local info = debug.getinfo(action)
        name = info and info["source"] .. ":" .. info["linedefined"] or ""

        local id = _DelayFrame(action, frame, arg, name)
        SetTimerName(id, name)
        return id
    end
else
    ---属性改变的时候通知绑定对象
    ---@overload fun(function:function,time:float,arg:any)
    ---@return int
    Delay = timer.Delay
    ---属性改变的时候通知绑定对象
    ---@overload fun(function:function,frame:int,arg:any)
    ---@return int
    DelayFrame = timer.DelayFrame
end



---属性改变的时候通知绑定对象
---@overload fun(id:int)
---@return void
StopDelay = timer.StopDelay



Manager = manager

LoadSceneMode = {
    Single = 0,
    -- Adds the Scene to the current loaded Scenes.
    Additive = 1
}
---------------------unityEngine--------------------------------
local GameObject = UnityEngine.GameObject
local Transform = UnityEngine.Transform
local Animation = UnityEngine.Animation
local Animator = UnityEngine.Animator
local Application = UnityEngine.Application
local SystemInfo = UnityEngine.SystemInfo

--------------Hugula.Databinding------
local Databinding = Hugula.Databinding
local BindableContainer = Databinding.BindableContainer
local BindableObject = Databinding.BindableObject
local Binding = Databinding.Binding
local BindingPathPart = Databinding.BindingPathPart
local BindingUtility = Databinding.BindingUtility
local ExpressionUtility = Databinding.ExpressionUtility

local HugulaNotifyCollectionChangedEventArgs = Databinding.HugulaNotifyCollectionChangedEventArgs

-- ExpressionUtility.instance:Initialize()

local Binder = Hugula.Databinding.Binder
local AnimatorBinder = Binder.AnimatorBinder
local ButtonBinder = Binder.ButtonBinder
local CollectionChangedBinder = Binder.CollectionChangedBinder
local CollectionViewBinder = Binder.CollectionViewBinder
local CustomBinder = Binder.CustomBinder
local EventCommandBinder = Binder.EventCommandBinder
local EventExecuteBinder = Binder.EventExecuteBinder
local ImageBinder = Binder.ImageBinder
local InputFieldBinder = Binder.InputFieldBinder
local LoopScrollRectBinder = Binder.LoopScrollRectBinder
local LoopVerticalScrollRectBinder = Binder.LoopVerticalScrollRectBinder
local MaskableGraphicBinder = Binder.MaskableGraphicBinder
local PrefabBinder = Binder.PrefabBinder
local SelectableBinder = Binder.SelectableBinder
local SliderBinder = Binder.SliderBinder
local TextBinder = Binder.TextBinder
local TextMeshProUGUIBinder = Binder.TextMeshProUGUIBinder
local UIBehaviourBinder = Binder.UIBehaviourBinder

local UIComponents = Hugula.UIComponents
local CoolDown = UIComponents.CoolDown
local GroupSelector = UIComponents.GroupSelector
--------------hugula------------------
AudioManager = Hugula.Audio.AudioManager
local StopwatchProfiler = Hugula.Profiler.StopwatchProfiler
local CUtils = CS.Hugula.Utils.CUtils
local CodeVersion = CS.Hugula.CodeVersion
