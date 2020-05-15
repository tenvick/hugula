local CS = CS
local UnityEngine = CS.UnityEngine
local Hugula = CS.Hugula
local manager = Hugula.Framework.Manager
local EnterLua = CS.EnterLua
local timer = Hugula.Framework.Timer.instance --≥ı ºªØtimer

---------------
Delay = EnterLua.Delay
DelayFrame = EnterLua.DelayFrame
StopDelay = EnterLua.StopDelay
Timer = Hugula.Framework.Timer
Manager = manager
---------------------unityEngine--------------------------------
local GameObject = UnityEngine.GameObject
local Transform = UnityEngine.Transform
local Animation = UnityEngine.Animation
local Animator = UnityEngine.Animator

--------------Hugula.Databinding------
local Databinding = Hugula.Databinding
local BindableContainer = Databinding.BindableContainer
local BindableObject = Databinding.BindableObject
local Binding = Databinding.Binding
local BindingPathPart = Databinding.BindingPathPart
local BindingUtility = Databinding.BindingUtility
local ExpressionUtility = Databinding.ExpressionUtility
local IBindingExpression = Databinding.IBindingExpression

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

