// The MIT License (MIT)

// Copyright 2015 Siney/Pangweiwei siney@yeah.net
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SLua
{
    using System.Collections.Generic;
    using System;

    public class CustomExport
    {
        public static void OnGetAssemblyToGenerateExtensionMethod(out List<string> list) {
            list = new List<string> {
                "Assembly-CSharp",
            };
        }

        public static void OnAddCustomClass(LuaCodeGen.ExportGenericDelegate add)
        {
			// below lines only used for demostrate how to add custom class to export, can be delete on your app

            add(typeof(System.Func<int>), null);
            add(typeof(System.Action<int, string>), null);
            add(typeof(System.Action<int, Dictionary<int, object>>), null);
            add(typeof(List<int>), "ListInt");
            add(typeof(Dictionary<int, string>), "DictIntStr");
            add(typeof(string), "String");
			add(typeof(System.Func<Hugula.Loader.CRequest,bool>),null);
            // add your custom class here
            // add( type, typename)
            // type is what you want to export
            // typename used for simplify generic type name or rename, like List<int> named to "ListInt", if not a generic type keep typename as null or rename as new type name
        }

        public static void OnAddCustomAssembly(ref List<string> list)
        {
            // add your custom assembly here
            // you can build a dll for 3rd library like ngui titled assembly name "NGUI", put it in Assets folder
            // add its name into list, slua will generate all exported interface automatically for you

            //list.Add("NGUI");
        }

        public static HashSet<string> OnAddCustomNamespace()
        {
            return new HashSet<string>
            {
                //"NLuaTest.Mock"
            };
        }

        // if uselist return a white list, don't check noUseList(black list) again
        /// <summary>
        /// 枚举要写成 namespace.classname+enum
        /// event namespace.classname+event
        /// </summary>
        /// <param name="list"></param>
        public static void OnGetUseList(out List<string> list)
        {
            list = new List<string>
            {
                "UnityEngine.GameObject",
                "UnityEngine.WWW",
                "UnityEngine.WWWForm",
                "UnityEngine.Vector4",
                "UnityEngine.Vector3",
                "UnityEngine.Vector2",
#region ugui
                "UnityEngine.UI.VerticalLayoutGroup",
                "UnityEngine.UI.ToggleGroup",
                "UnityEngine.UI.Toggle+ToggleTransition",
                "UnityEngine.UI.Toggle+ToggleEvent",
                "UnityEngine.UI.Toggle",
                "UnityEngine.UI.Text",
                "UnityEngine.UI.SpriteState",
                "UnityEngine.UI.Slider+SliderEvent",
                "UnityEngine.UI.Slider+Direction",
                "UnityEngine.UI.Slider",
                "UnityEngine.UI.Shadow",
                "UnityEngine.UI.Selectable+Transition",
                "UnityEngine.UI.Selectable",
                "UnityEngine.UI.ScrollRect+ScrollRectEvent",
                "UnityEngine.UI.ScrollRect+ScrollbarVisibility",
                "UnityEngine.UI.ScrollRect+MovementType",
                "UnityEngine.UI.ScrollRect",
                "UnityEngine.UI.Scrollbar+ScrollEvent",
                "UnityEngine.UI.Scrollbar+Direction",
                "UnityEngine.UI.Scrollbar",
                "UnityEngine.UI.RectMask2D",
                "UnityEngine.UI.RawImage",
                "UnityEngine.UI.PositionAsUV1",
                "UnityEngine.UI.Outline",
                "UnityEngine.UI.Navigation+Mode",
                "UnityEngine.UI.Navigation",
                "UnityEngine.UI.MaskUtilities",
                "UnityEngine.UI.MaskableGraphic+CullStateChangedEvent",
                "UnityEngine.UI.MaskableGraphic",
                "UnityEngine.UI.Mask",
                "UnityEngine.UI.LayoutUtility",
                "UnityEngine.UI.LayoutRebuilder",
                "UnityEngine.UI.LayoutGroup",
                "UnityEngine.UI.LayoutElement",
                "UnityEngine.UI.InputField+SubmitEvent",
                "UnityEngine.UI.InputField+OnChangeEvent",
                "UnityEngine.UI.InputField+LineType",
                "UnityEngine.UI.InputField+InputType",
                "UnityEngine.UI.InputField+ContentType",
                "UnityEngine.UI.InputField+CharacterValidation",
                "UnityEngine.UI.InputField",
                "UnityEngine.UI.Image.Type",
                "UnityEngine.UI.Image+OriginVertical",
                "UnityEngine.UI.Image+OriginHorizontal",
                "UnityEngine.UI.Image+Origin360",
                "UnityEngine.UI.Image+Origin180",
                "UnityEngine.UI.Image+Origin90",
                "UnityEngine.UI.Image+FillMethod",
                "UnityEngine.UI.Image",
                "UnityEngine.UI.HorizontalOrVerticalLayoutGroup",
                "UnityEngine.UI.HorizontalLayoutGroup",
                "UnityEngine.UI.GridLayoutGroup+Corner",
                "UnityEngine.UI.GridLayoutGroup+Constraint",
                "UnityEngine.UI.GridLayoutGroup+Axis",
                "UnityEngine.UI.GridLayoutGroup",
                "UnityEngine.UI.GraphicRegistry",
                "UnityEngine.UI.GraphicRaycaster+BlockingObjects",
                "UnityEngine.UI.GraphicRaycaster",
                "UnityEngine.UI.Graphic",
                "UnityEngine.UI.FontUpdateTracker",
                "UnityEngine.UI.FontData",
                "UnityEngine.UI.Dropdown+OptionDataList",
                "UnityEngine.UI.Dropdown+OptionData",
                "UnityEngine.UI.Dropdown+DropdownEvent",
                "UnityEngine.UI.Dropdown",
                "UnityEngine.UI.DefaultControls+Resources",
                "UnityEngine.UI.DefaultControls",
                "UnityEngine.UI.ContentSizeFitter+FitMode",
                "UnityEngine.UI.ContentSizeFitter",
                "UnityEngine.UI.ColorBlock",
                "UnityEngine.UI.Clipping",
                "UnityEngine.UI.ClipperRegistry",
                "UnityEngine.UI.CanvasUpdateRegistry",
                "UnityEngine.UI.CanvasUpdate",
                "UnityEngine.UI.CanvasScaler+Unit",
                "UnityEngine.UI.CanvasScaler+ScreenMatchMode",
                "UnityEngine.UI.CanvasScaler+ScaleMode",
                "UnityEngine.UI.CanvasScaler",
                "UnityEngine.UI.Button+ButtonClickedEvent",
                "UnityEngine.UI.Button",
                "UnityEngine.UI.BaseMeshEffect",
                "UnityEngine.UI.AspectRatioFitter+AspectMode",
                "UnityEngine.UI.AspectRatioFitter",
                "UnityEngine.UI.AnimationTriggers",
                //event
                "UnityEngine.EventSystems.UIBehaviour",
                "UnityEngine.EventSystems.RaycastResult",
                "UnityEngine.EventSystems.PointerEventData+InputButton",
                "UnityEngine.EventSystems.PointerEventData+FramePressState",
                "UnityEngine.EventSystems.PointerEventData",
                "UnityEngine.EventSystems.EventSystem",
                "UnityEngine.EventSystems.EventTrigger",
                "UnityEngine.EventSystems.BaseEventData",
                "UnityEngine.EventSystems.AxisEventData",
                "UnityEngine.EventSystems.AbstractEventData",
                "UnityEngine.Events.UnityEventCallState",
                "UnityEngine.Events.UnityEventBase",
                "UnityEngine.Events.UnityEvent",
                "UnityEngine.Events.PersistentListenerMode",
                "UnityEngine.Event",
                "UnityEngine.EventSystems.ExecuteEvents",

#endregion
                "UnityEngine.TransparencySortMode",
                "UnityEngine.Transform",
                "UnityEngine.Time",
                "UnityEngine.Texture3D",
                "UnityEngine.Texture2D",
                "UnityEngine.Texture",
                "UnityEngine.TextMesh",
                "UnityEngine.TextGenerationSettings",
                "UnityEngine.TextAsset",
                "UnityEngine.TextAnchor",
                "UnityEngine.TexGenMode",
                "UnityEngine.SystemLanguage",
                "UnityEngine.SystemInfo",
                "UnityEngine.Sprites.DataUtility",
                "UnityEngine.SpriteRenderer",
                "UnityEngine.SpritePackingRotation",
                "UnityEngine.SpritePackingMode",
                "UnityEngine.SpriteMeshType",
                "UnityEngine.SpriteAlignment",
                "UnityEngine.Sprite",
                "UnityEngine.Space",
                "UnityEngine.Skybox",
                "UnityEngine.SkinnedMeshRenderer",
                "UnityEngine.Shader",
                "UnityEngine.SendMessageOptions",
                "UnityEngine.Security",
                "UnityEngine.ScriptableObject",
                "UnityEngine.ScreenOrientation",
                "UnityEngine.Screen",
                "UnityEngine.ScaleMode",
                "UnityEngine.RuntimePlatform",
                "UnityEngine.RuntimeAnimatorController",
                "UnityEngine.Resources",
                "UnityEngine.ResourceRequest",
                "UnityEngine.Resolution",
                "UnityEngine.RenderTextureFormat",
                "UnityEngine.RenderTextureReadWrite",
                "UnityEngine.RenderTexture",
                "UnityEngine.RenderSettings",
                "UnityEngine.RenderMode",
                "UnityEngine.Renderer",
                "UnityEngine.RenderBuffer",
                "UnityEngine.ReflectionProbe",
                "UnityEngine.RectTransformUtility",
                "UnityEngine.RectTransform+Edge",
                "UnityEngine.RectTransform+Axis",
                "UnityEngine.RectTransform",
                "UnityEngine.RectOffset",
                "UnityEngine.Rect",
                "UnityEngine.RaycastHit2D",
                "UnityEngine.RaycastHit",
                "UnityEngine.Random",
                "UnityEngine.QueueMode",
                "UnityEngine.Quaternion",
                "UnityEngine.QualitySettings",
                "UnityEngine.Projector",
                "UnityEngine.Profiler",
                "UnityEngine.PlayMode",
                "UnityEngine.PlayerPrefs",
                "UnityEngine.ParticleSystemSortMode",
                "UnityEngine.ParticleSystemSimulationSpace",
                "UnityEngine.ParticleSystem",
                "UnityEngine.Object",
                "UnityEngine.MonoBehaviour",
                "UnityEngine.MeshRenderer",
                "UnityEngine.MeshFilter",
                "UnityEngine.MeshCollider",
                "UnityEngine.Mesh",
                "UnityEngine.Matrix4x4",
                "UnityEngine.Mathf",
                "UnityEngine.Material",
                "UnityEngine.LineRenderer",
                "UnityEngine.LightmapSettings",
                "UnityEngine.LightmapsMode",
                "UnityEngine.LightmapData",
                // "UnityEngine.Light",
                "UnityEngine.LayerMask",
                "UnityEngine.KeyCode",
                "UnityEngine.Input",
                "UnityEngine.Graphics",
                "UnityEngine.Gradient",
                "UnityEngine.GameObject",
                "UnityEngine.Font",
                "UnityEngine.FogMode",
                "UnityEngine.FilterMode",
                "UnityEngine.DeviceType",
                "UnityEngine.DeviceOrientation",
                "UnityEngine.Debug",
                "UnityEngine.Component",
                "UnityEngine.ColorUtility",
                "UnityEngine.ColorSpace",
                "UnityEngine.Color32",
                "UnityEngine.Color",
                "UnityEngine.Collider2D",
                "UnityEngine.Collider",
                "UnityEngine.CapsuleCollider",
                "UnityEngine.CanvasRenderer",
                "UnityEngine.CanvasGroup",
                "UnityEngine.Canvas",
                "UnityEngine.CameraType",
                "UnityEngine.CameraClearFlags",
                "UnityEngine.Camera",
                "UnityEngine.BoxCollider2D",
                "UnityEngine.BoxCollider",
                "UnityEngine.Bounds",
                "UnityEngine.BoundingSphere",
                "UnityEngine.BillboardRenderer",
                "UnityEngine.BillboardAsset",
                "UnityEngine.Behaviour",
                "UnityEngine.AudioVelocityUpdateMode",
                "UnityEngine.AudioType",
                "UnityEngine.AudioSpeakerMode",
                "UnityEngine.AudioSourceCurveType",
                "UnityEngine.AudioSource",
                "UnityEngine.AudioSettings",
                "UnityEngine.AudioRolloffMode",
                "UnityEngine.AudioListener",
                "UnityEngine.AudioClip",
                "UnityEngine.AsyncOperation",
                "UnityEngine.AssetBundleRequest",
                "UnityEngine.AssetBundleManifest",
                "UnityEngine.AssetBundle",
                "UnityEngine.ApplicationSandboxType",
                "UnityEngine.ApplicationInstallMode",
                "UnityEngine.Application",
                "UnityEngine.Animator",
                "UnityEngine.Animation",
                "UnityEngine.AnimationBlendMode",
                "UnityEngine.AnimationClip",
                "UnityEngine.AnimationClipPair",
                "UnityEngine.AnimationCullingType",
                "UnityEngine.AnimationCurve",
                "UnityEngine.AnimationEvent",
                "UnityEngine.AnimationPlayMode",
                "UnityEngine.AnimationState",
                "UnityEngine.NetworkReachability",
                "UnityEngine.WaitForEndOfFrame",
                "UnityEngine.WaitForSeconds",
            };
        }

        public static List<string> FunctionFilterList = new List<string>()
        {
            "UIWidget.showHandles",
            "UIWidget.showHandlesWithMoveTool",
            
            "UnityEngine.MonoBehaviour.get_runInEditMode",
            "UnityEngine.MonoBehaviour.set_useGUILayout",
            "UnityEngine.MonoBehaviour.get_runInEditMode",
            "UnityEngine.MonoBehaviour.set_useGUILayout",
            "UnityEngine.MonoBehaviour.runInEditMode",
            "UnityEngine.MonoBehaviour.useGUILayout",
        };
        // black list if white list not given
        public static void OnGetNoUseList(out List<string> list)
        {
            list = new List<string>
        {      
            "HideInInspector",
            "ExecuteInEditMode",
            "AddComponentMenu",
            "ContextMenu",
            "RequireComponent",
            "DisallowMultipleComponent",
            "SerializeField",
            "AssemblyIsEditorAssembly",
            "Attribute", 
            "Types",
            "UnitySurrogateSelector",
            "TrackedReference",
            "TypeInferenceRules",
            "FFTWindow",
            "RPC",
            "Network",
            "MasterServer",
            "BitStream",
            "HostData",
            "ConnectionTesterStatus",
            "GUI",
            "EventType",
            "EventModifiers",
            "FontStyle",
            "TextAlignment",
            "TextEditor",
            "TextEditorDblClickSnapping",
            "TextGenerator",
            "TextClipping",
            "Gizmos",
             "ADBannerView",
            "ADInterstitialAd",            
            "Android",
            "Tizen",
            "jvalue",
            "iPhone",
            "iOS",
			"Windows",
            "CalendarIdentifier",
            "CalendarUnit",
            "CalendarUnit",
            "ClusterInput",
            "FullScreenMovieControlMode",
            "FullScreenMovieScalingMode",
            "Handheld",
            "LocalNotification",
            "NotificationServices",
            "RemoteNotificationType",      
            "RemoteNotification",
            "SamsungTV",
            "TextureCompressionQuality",
            "TouchScreenKeyboardType",
            "TouchScreenKeyboard",
            "MovieTexture",
            "UnityEngineInternal",
            "Terrain",                            
            "Tree",
            "SplatPrototype",
            "DetailPrototype",
            "DetailRenderMode",
            "MeshSubsetCombineUtility",
            "AOT",
            "Social",
            "Enumerator",       
            "SendMouseEvents",               
            "Cursor",
            "Flash",
            "ActionScript",
            "OnRequestRebuild",
            "Ping",
            "ShaderVariantCollection",
            "SimpleJson.Reflection",
            "CoroutineTween",
            "GraphicRebuildTracker",
            "Advertisements",
            "UnityEditor",
			"WSA",
			"EventProvider",
			"Apple",
			"ClusterInput",
			"Motion",
			"UnityEngine.UI.ReflectionMethodsCache",
			"Remote"
        };
        }
    }
}