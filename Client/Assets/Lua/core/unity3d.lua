local CS = CS
local UnityEngine = CS.UnityEngine
local Hugula = CS.Hugula
local manager = Hugula.Framework.Manager
local EnterLua = CS.Hugula.EnterLua
local timer = Hugula.Framework.Timer.instance --初始化timer

---------------
Delay = EnterLua.Delay
DelayFrame = EnterLua.DelayFrame
StopDelay = EnterLua.StopDelay
Timer = Hugula.Framework.Timer
local timer = Timer
local tins = timer.instance
Delay = timer.Delay
DelayFrame = timer.DelayFrame
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

ExpressionUtility.instance:Initialize()

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
