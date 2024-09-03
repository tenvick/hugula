using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XLua;

namespace XLua.Editor
{
    public static class HugulaXluaConfig
    {
        /***************如果你全lua编程，可以参考这份自动化配置***************/
        //--------------begin 纯lua编程配置参考----------------------------
        static List<string> exclude = new List<string> {
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
            //"OnRequestRebuild",
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
            "NativeLeakDetection",
            "NativeLeakDetectionMode",
            "WWWAudioExtensions",
            "UnityEngine.Experimental",
            "UnityEngine.CanvasRenderer",
            "Hugula.Loader.AssetOperationSimulation",
            "UnityEngine.InputRegistering",
            "UnityEngine.InputManagerEntry",
            "UnityEngine.LocationService",
            "UnityEngine.BuildCompression",
            "UnityEngine.AssetBundle",
            "UnityEngine.WheelCollider",
            "UnityEngine.LightProbes",
			"UnityEngine.ClusterSerialization",
            "UnityEngine.ClusterRendererModule",
            "UnityEngine.GamepadSpeakerOutputType",
            "UnityEngine.LightingSettings+Lightmapper",
            "UnityEngine.LightingSettings+Sampling",
            "UnityEngine.LightingSettings+FilterMode",
            "UnityEngine.LightingSettings+DenoiserType",
            "UnityEngine.LightingSettings+FilterType",
            "UnityEngine.AudioSource",
            "UnityEngine.LightingSettings",
            "UnityEngine.CloudStreaming",
            "Hugula.LuaBundleRead",
            "Hugula.StreamingLuaBytesRead",
            "Hugula.ZipLuaRead",
            "Hugula.AndroidLuaRead",
		
        };

        static bool isExcluded(Type type)
        {
            var fullName = type.FullName;
            for (int i = 0; i < exclude.Count; i++)
            {
                if (fullName.Contains(exclude[i]))
                {
                    return true;
                }
            }
            return false;
        }

        [BlackList]
        public static List<List<string>> HugulaBlackList = new List<List<string>>() {
            new List<string> () { "UnityEngine.GameObject", "networkView" },
            new List<string> () { "System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
            //new List<string> () { "UnityEngine.CanvasRenderer", "OnRequestRebuild" },
            //new List<string> () { "UnityEngine.CanvasRenderer", "OnRequestRebuild", "delegate" },
            new List<string> () { "UnityEngine.Canvas", "renderingDisplaySize"},

            new List<string> () { "Hugula.Loader.ResourcesLoader", "SimulateAssetBundleInEditor" },
            new List<string> () { "Hugula.Databinding.BindableObject", "AddBinding","Hugula.Databinding.Binding" },
            new List<string> () { "Hugula.Databinding.BindableObject", "GetBindings" },
            new List<string> () { "Hugula.Databinding.BindableObject", "GetBindingByName","System.String" },
            new List<string> () { "Hugula.Databinding.BindableObject", "RemoveBindingAt","System.Int32" },
             new List<string> () { "Hugula.ResUpdate.FileManifestManager", "AnalyzeAddressDependencies","System.String","System.Type","System.Collections.Generic.List<string>"},


            new List<string> () { "UnityEngine.AnimatorControllerParameter", "name" },
            new List<string> () { "UnityEngine.AudioSettings", "GetSpatializerPluginNames" },
            new List<string> () { "UnityEngine.AudioSettings", "SetSpatializerPluginName", "System.String" },
            new List<string> () { "UnityEngine.Caching", "SetNoBackupFlag", "UnityEngine.CachedAssetBundle" },
            new List<string> () { "UnityEngine.Caching", "SetNoBackupFlag", "System.String", "UnityEngine.Hash128" },
            new List<string> () { "UnityEngine.Caching", "ResetNoBackupFlag", "UnityEngine.CachedAssetBundle" },
            new List<string> () { "UnityEngine.Caching", "ResetNoBackupFlag", "System.String", "UnityEngine.Hash128" },
            new List<string> () { "UnityEngine.DrivenRectTransformTracker", "StopRecordingUndo" },
            new List<string> () { "UnityEngine.DrivenRectTransformTracker", "StartRecordingUndo" },
            new List<string> () { "UnityEngine.LightProbeGroup", "dering" },
            new List<string> () { "UnityEngine.LightProbeGroup", "probePositions" },
            new List<string> () { "UnityEngine.Light", "SetLightDirty" },
            new List<string> () { "UnityEngine.Light", "shadowRadius" },
            new List<string> () { "UnityEngine.Light", "shadowAngle" },
            new List<string> () { "UnityEngine.Input", "IsJoystickPreconfigured", "System.String" },
            new List<string> () { "UnityEngine.MeshRenderer", "receiveGI" },
            new List<string> () { "UnityEngine.ParticleSystemForceField", "FindAll" },
            new List<string> () { "UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame" },
            new List<string> () { "UnityEngine.Texture", "imageContentsHash" },
            new List<string> () { "UnityEngine.Texture", "imageContentsHash" },
            new List<string> () { "UnityEngine.UI.DefaultControls", "factory" },
            new List<string> () { "UnityEngine.UI.Graphic", "OnRebuildRequested" },
            new List<string> () { "UnityEngine.UI.Text", "OnRebuildRequested" },
            new List<string> () { "UnityEngine.MeshRenderer","scaleInLightmap","System.Int32"},
            new List<string> () { "UnityEngine.MeshRenderer","stitchLightmapSeams","System.Boolean"},
            new List<string> () { "UnityEngine.ParticleSystemRenderer","supportsMeshInstancing","System.Boolean"},
            new List<string> () {"System.Collections.Generic.Stack<Hugula.Profiler.StopwatchProfiler>","TryPeek","Hugula.Profiler.StopwatchProfiler"},
            new List<string> () { "UnityEngine.SystemInfo", "GetRenderTextureSupportedMSAASampleCount","UnityEngine.RenderTextureDescriptor"},
            new List<string>() {"UnityEngine.SystemInfo",""},
              new List<string> () { "System.Xml.XmlNodeList", "ItemOf" },
            new List<string> () { "UnityEngine.WWW", "movie" },
#if UNITY_WEBGL
            new List<string> () { "UnityEngine.WWW", "threadPriority" },
#endif
            new List<string> () { "UnityEngine.Texture2D", "alphaIsTransparency" },
            new List<string> () { "UnityEngine.Sprite", "isUsingPlaceholder" },


            new List<string> () { "UnityEngine.Security", "GetChainOfTrustValue" },
            new List<string> () { "UnityEngine.CanvasRenderer", "onRequestRebuild" },
            new List<string> () { "UnityEngine.Light", "areaSize" },
            new List<string> () { "UnityEngine.Light", "lightmapBakeType" },
            new List<string> () { "UnityEngine.WWW", "MovieTexture" },
            new List<string> () { "UnityEngine.WWW", "GetMovieTexture" },
            new List<string> () { "UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup" },
#if !UNITY_WEBPLAYER
            new List<string> () { "UnityEngine.Application", "ExternalEval" },
#endif
            new List<string> () { "UnityEngine.GameObject", "networkView" }, //4.6.2 not support
            new List<string> () { "UnityEngine.Component", "networkView" }, //4.6.2 not support
            new List<string> () { "UnityEngine.MonoBehaviour", "runInEditMode" },

            new List<string> () { "System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
            new List<string> () { "System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity" },

            new List<string> () { "System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
            new List<string> () { "System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "TryGetValue", "System.Security.AccessControl.DirectorySecurity" },

            new List<string> () { "Hugula.Loader.FileManifest", "WriteToFile","System.String" },
            new List<string> () { "Hugula.Loader.ManifestManager", "SimulateAssetBundleInEditor" },
            new List<string> () { "Hugula.ZipConfigs", "CreateInstance" },
            new List<string> () { "Hugula.ZipConfigs", "Delete" },

            new List<string> () { "Hugula.Loader.BackGroundDownload", "currentLoadingCount" },
            new List<string> () { "Hugula.Loader.BackGroundDownload", "loadingList" },
            new List<string> () { "Hugula.Loader.ABInfo", "EqualsDependencies","Hugula.Loader.ABInfo" },
            new List<string> () { "Hugula.Loader.ABInfo", "assetPath"},
            new List<string> () { "Hugula.Loader.CacheManager", "EditorCacheData"},
            new List<string> () { "Hugula.EnterLua", "isDebug"},
            new List<string> () { "Hugula.Audio.WwiseAudioManager", "isDebug"},

            //hashset报错处理
            new List<string> () { "System.Collections.Generic.HashSet`1[System.Int32]", "TryGetValue", "System.Int32","System.Int32&" },
            new List<string> () { "System.Collections.Generic.HashSet`1[System.Int32]", ".ctor", "System.Int32"},
            new List<string> () { "System.Collections.Generic.HashSet`1[System.Int32]", ".ctor", "System.Int32","System.Collections.Generic.IEqualityComparer`1[System.Int32]"},

            new List<string> () { "UnityEngine.Application", "MemoryUsageChangedCallback","UnityEngine.ApplicationMemoryUsageChange" },
            new List<string>() {"UnityEngine.ClusterSerialization", "SaveTimeManagerState","RestoreTimeManagerState","SaveInputManagerState","RestoreInputManagerState","SaveClusterInputState","RestoreClusterInputState"},

            //2022.3 win发布报错处理
            new List<string>() {"UnityEngine.Material","RevertAllPropertyOverrides"},
            new List<string>() {"UnityEngine.Material","IsPropertyOverriden","System.String"},
            new List<string>() {"UnityEngine.Material","IsPropertyOverriden","System.Int32"},
            new List<string>() {"UnityEngine.Material","IsPropertyLocked","System.String"},
            new List<string>() {"UnityEngine.Material","IsPropertyLocked","System.Int32"},
            new List<string>() {"UnityEngine.Material","IsPropertyLockedByAncestor","System.String"},
            new List<string>() {"UnityEngine.Material","IsPropertyLockedByAncestor","System.Int32"},
            new List<string>() {"UnityEngine.Material","SetPropertyLock","System.Int32","System.Boolean"},
            new List<string>() {"UnityEngine.Material","SetPropertyLock","System.String","System.Boolean"},
            new List<string>() {"UnityEngine.Material","ApplyPropertyOverride","UnityEngine.Material","System.String","System.Boolean"},
            new List<string>() {"UnityEngine.Material","ApplyPropertyOverride","UnityEngine.Material","System.Int32","System.Boolean"},
            new List<string>() {"UnityEngine.Material","ApplyPropertyOverride","UnityEngine.Material","UnityEngine.MaterialSerializedProperty","System.Boolean"},
            new List<string>() {"UnityEngine.Material","RevertPropertyOverride","System.Int32"},
            new List<string>() {"UnityEngine.Material","RevertPropertyOverride","System.String"},
            new List<string>() {"UnityEngine.Material","IsChildOf","UnityEngine.Material"},
            new List<string>() {"UnityEngine.Material","parent"},
            new List<string>() {"UnityEngine.Material","isVariant"},
            new List<string>() {"UnityEngine.Material","shaderKeywords"},

            new List<string>() {"UnityEngine.QualitySettings","IsPlatformIncluded","System.String","System.Int32"},
            new List<string>() {"UnityEngine.QualitySettings","GetActiveQualityLevelsForPlatform","System.String"},
            new List<string>() {"UnityEngine.QualitySettings","GetActiveQualityLevelsForPlatformCount","System.String"},

            new List<string>() {"UnityEngine.QualitySettings","TryIncludePlatformAt","System.String","System.Int32","System.Exception&"},
            new List<string>() {"UnityEngine.QualitySettings","TryExcludePlatformAt","System.String","System.Int32","System.Exception&"},
            new List<string>() {"UnityEngine.QualitySettings","GetAllRenderPipelineAssetsForPlatform","System.String","System.Collections.Generic.List`1[UnityEngine.Rendering.RenderPipelineAsset]&"},


            new List<string>() {"UnityEngine.TextureMipmapLimitGroups","CreateGroup","System.String"},
            new List<string>() {"UnityEngine.TextureMipmapLimitGroups","RemoveGroup","System.String"},
         };

        [LuaCallCSharp]
        public static IEnumerable<Type> LuaCallCSharp
        {
            get
            {
                List<string> namespaces = new List<string>() // 在这里添加名字空间
                {
                    "UnityEngine",
                    "UnityEngine.UI",
                    "UnityEngine.AI",
                    "UnityEngine.Events",
                    "Hugula",
                    "Hugula.Loader",
                    "Hugula.Audio",
                    "Hugula.Utils",
                    "Hugula.Framework",
                    "Hugula.UIComponents",
                    "Hugula.Databinding",
                    "Hugula.Databinding.Binder",
                };
                var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                  from type in assembly.GetExportedTypes()
                                  where type.Namespace != null && namespaces.Contains(type.Namespace) && !isExcluded(type) &&
                                type.BaseType != typeof(MulticastDelegate) && !type.IsInterface
                                  select type);

                string[] customAssemblys = new string[] {
                    "Assembly-CSharp",
                };

                var genericTypes = new HashSet<Type>();
                Action<Type> findParentType = (Type t) =>
                {
                    while (t != null && t.BaseType != null && t.BaseType.FullName != null && (t.IsGenericType || t.BaseType.IsGenericType))
                    {
                        genericTypes.Add(t);
                        t = t.BaseType;
                    }
                };

                foreach (var cuAsse in customAssemblys)
                {
                    var types = Assembly.Load(cuAsse).GetExportedTypes();
                    foreach (var type in types)
                    {
                        if (namespaces.Contains(type.Namespace) && type.BaseType != null && type.BaseType.FullName != null && type.BaseType.IsGenericType)
                        {
                            genericTypes.Add(type.BaseType);
                            findParentType(type.BaseType.BaseType);
                        }
                    }
                }

                // foreach (var t in genericTypes)
                // {
                //     Debug.Log($"genericTypes :{t}");
                // }

                var customlist = new List<Type>();
                customlist.Add(typeof(Func<object, int, Component, int, RectTransform, Component>));
                customlist.Add(typeof(Action<object, object, int>));
                customlist.Add(typeof(Action<object, object>));
                customlist.Add(typeof(UnityEngine.Events.UnityAction<float>));
                customlist.Add(typeof(UnityEngine.Events.UnityAction));
                customlist.Add(typeof(UnityEngine.EventSystems.UIBehaviour));
                customlist.Add(typeof(System.Func<System.Object, int, int>));
                customlist.Add(typeof(Action<object, object, int>));
                customlist.Add(typeof(Func<object, int, Component, int, RectTransform, Component>));
                customlist.Add(typeof(Action<object, object, int, int>));
                customlist.Add(typeof(System.Action));
                customlist.Add(typeof(System.Action<object>));
                customlist.Add(typeof(Hugula.Databinding.ICommand));
                customlist.Add(typeof(Hugula.Databinding.IExecute));
                customlist.Add(typeof(Func<object, int, int>));
                customlist.Add(typeof(Action<object, Component, int>));
                customlist.Add(typeof(Hugula.Databinding.BindPathPartGetValueUnpack));
                // customlist.Add(typeof(Hugula.Databinding.UpdateValue));
                customlist.Add(typeof(Hugula.Databinding.UpdateValueUnpack));
                // customlist.Add(typeof(Hugula.Databinding.ApplyActual));

                customlist.Add(typeof(Hugula.Audio.AudioManager));

                customlist.Add(typeof(Action<Vector2>));
                customlist.Add(typeof(System.Action<object, object, string, string, string, string>));
                customlist.Add(typeof(System.Func<int, object, bool>));

                customlist.Add(typeof(System.Collections.Specialized.NotifyCollectionChangedEventArgs));
                customlist.Add(typeof(System.Collections.IList));
                customlist.Add(typeof(System.Collections.Generic.List<int>));
                customlist.Add(typeof(System.Collections.Generic.HashSet<int>));

                customlist.Add(typeof(Action<float, float, float>));
                customlist.Add(typeof(Action<float, float>));
                customlist.Add(typeof(Action<int>));
                customlist.Add(typeof(Func<int, int>));
                customlist.Add(typeof(Action<object>));

                customlist.Add(typeof(System.Action<Vector2, Vector2>));
                customlist.Add(typeof(System.Action<object, string, UnityEngine.Object>)); //poolmanager call back回调

                customlist.Add(typeof(List<UnityEngine.Vector3>));
                customlist.Add(typeof(List<float>));
                customlist.Add(typeof(List<int>));
                // customlist.Add (typeof (Hugula.Databinding.Binding));
                // customlist.Add (typeof (Hugula.Databinding.BindableObject));
                // customlist.Add (typeof (Hugula.Databinding.BindableContainer));
                // customlist.Add (typeof (Hugula.Databinding.BindingUtility));

                // customlist.Add (typeof (Hugula.Databinding.Binder.EventExecuteBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.EventCommandBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.UIBehaviourBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.SelectableBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.CollectionChangedBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.ButtonBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.ImageBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.InputFieldBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.LoopScrollRectBinder));
                // customlist.Add (typeof (Hugula.Databinding.Binder.MaskableGraphicBinder));
                customlist.Add(typeof(UnityEngine.Profiling.Profiler));
                customlist.Add(typeof(UnityEngine.EventSystems.EventSystem));
                #region hot update   
                customlist.Add(typeof(Hugula.ResUpdate.FolderManifest));
                customlist.Add(typeof(Hugula.ResUpdate.FolderManifestQueue));
                customlist.Add(typeof(Hugula.ResUpdate.FolderQueueGroup));
                customlist.Add(typeof(Hugula.ResUpdate.OtherZipMode));
                customlist.Add(typeof(Hugula.ResUpdate.FastMode));
                // customlist.Add (typeof (Hugula.Databinding.Binder.TextBinder));

                customlist.Add(typeof(List<Hugula.ResUpdate.FolderManifest>));
                customlist.Add(typeof(System.Action<Hugula.LoadingEventArg>));
                customlist.Add(typeof(System.Action<Hugula.ResUpdate.FolderManifestQueue, bool>));
                customlist.Add(typeof(System.Action<Hugula.ResUpdate.FolderQueueGroup, bool>));
                #endregion
                return unityTypes.Concat(customlist).Concat(genericTypes);
            }
        }

        //自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
        [CSharpCallLua]
        public static List<Type> CSharpCallLuaDelegate
        {
            get
            {
                var lua_call_csharp = LuaCallCSharp;
                var delegate_types = new List<Type>();

                var flag = BindingFlags.Public | BindingFlags.Instance |
                    BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
                foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                    {
                        delegate_types.Add(field.FieldType);
                    }
                }

                foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
                {
                    if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                    {
                        delegate_types.Add(method.ReturnType);
                    }
                    foreach (var param in method.GetParameters())
                    {
                        var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                        if (typeof(Delegate).IsAssignableFrom(paramType))
                        {
                            delegate_types.Add(paramType);
                        }
                    }
                }
                return delegate_types.Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct().ToList();
            }
        }

        [CSharpCallLua]
        public static List<Type> CSharpCallLua
        {
            get
            {

                var types = new List<Type>();

                //自定义方法
                types.Add(typeof(Func<object, int, Component, int, RectTransform, Component>));
                types.Add(typeof(Action<object, object, int>));
                types.Add(typeof(Action<object, float>));
                types.Add(typeof(Action<object, object>));
                types.Add(typeof(Action<object, float, float>));
                types.Add(typeof(UnityEngine.Events.UnityAction<float>));
                types.Add(typeof(UnityEngine.Events.UnityAction));
                types.Add(typeof(System.Func<System.Object, int, int>));
                types.Add(typeof(Action<object, object, int>));
                types.Add(typeof(System.Action<object>));
                types.Add(typeof(System.Func<string, float, string>));

                types.Add(typeof(Func<object, int, Component, int, RectTransform, Component>));
                types.Add(typeof(Action<object, object, int, int>));
                types.Add(typeof(Hugula.Databinding.ICommand));
                types.Add(typeof(Hugula.Databinding.IExecute));
                types.Add(typeof(Func<object, int, int>));
                types.Add(typeof(Action<object, Component, int>));
                types.Add(typeof(Hugula.Databinding.BindPathPartGetValueUnpack));
                // types.Add(typeof(Hugula.Databinding.UpdateValue));
                types.Add(typeof(Hugula.Databinding.UpdateValueUnpack));
                // types.Add(typeof(Hugula.Databinding.ApplyActual));
                types.Add(typeof(Hugula.Mvvm.VMStateHelper.IVMState));

                types.Add(typeof(Action<Vector2>));
                types.Add(typeof(System.Action<object, object, string, string, string, string>));
                types.Add(typeof(Hugula.Databinding.NotifyCollectionChangedEventHandler));
                types.Add(typeof(Hugula.Databinding.PropertyChangedEventHandler));
                types.Add(typeof(System.Collections.IList));
                types.Add(typeof(Func<string, string>));
                types.Add(typeof(System.Collections.IEnumerator));

                types.Add(typeof(System.Action<object, string, UnityEngine.Object>)); //poolmanager call back回调

                types.Add(typeof(Action<Vector2>));
                types.Add(typeof(System.Action<object, object, string, string, string, string>));
                types.Add(typeof(System.Func<int, object, bool>));

                types.Add(typeof(System.Collections.Specialized.NotifyCollectionChangedEventArgs));
                types.Add(typeof(System.Collections.IList));
                types.Add(typeof(Hugula.Audio.AudioManager));

                types.Add(typeof(Action<float, float, float>));
                types.Add(typeof(Action<float, float>));
                types.Add(typeof(Action<int>));
                types.Add(typeof(Func<int, int>));
                types.Add(typeof(Action<object>));
                types.Add(typeof(Action<int, bool>));

                types.Add(typeof(System.Action<Vector2, Vector2>));
                types.Add(typeof(System.Action<object, string, UnityEngine.Object>)); //poolmanager call back回调

                types.Add(typeof(List<UnityEngine.Vector3>));
                types.Add(typeof(List<float>));
                types.Add(typeof(List<int>));

                types.Add(typeof(UnityEngine.RuntimePlatform));
                types.Add(typeof(System.Object));
                types.Add(typeof(Func<string, bool>));
                types.Add(typeof(Func<string, bool, Action, bool>));
                #region hot update   

                types.Add(typeof(System.Action<Hugula.LoadingEventArg>));
                types.Add(typeof(System.Action<Hugula.ResUpdate.FolderManifestQueue, bool>));
                types.Add(typeof(System.Action<Hugula.ResUpdate.FolderQueueGroup, bool>));

                #endregion
                return types;
            }

        }
        //--------------end 纯lua编程配置参考----------------------------

        /***************热补丁可以参考这份自动化配置***************/
        //[Hotfix]
        //static IEnumerable<Type> HotfixInject
        //{
        //    get
        //    {
        //        return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
        //                where type.Namespace == null || !type.Namespace.StartsWith("XLua")
        //                select type);
        //    }
        //}
        //--------------begin 热补丁自动化配置-------------------------
        static bool hasGenericParameter(Type type)
        {
            if (type.IsGenericTypeDefinition) return true;
            if (type.IsGenericParameter) return true;
            if (type.IsByRef || type.IsArray)
            {
                return hasGenericParameter(type.GetElementType());
            }
            if (type.IsGenericType)
            {
                foreach (var typeArg in type.GetGenericArguments())
                {
                    if (hasGenericParameter(typeArg))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static bool typeHasEditorRef(Type type,bool IgnoreCaseIsNested = false)
        {
            try
            {

                var Namespace = type.Namespace;
                if (Namespace != null && (Namespace == "UnityEditor" || Namespace.StartsWith("UnityEditor.")))
                {
                    return true;
                }
                if (type.IsNested && IgnoreCaseIsNested)
                {
                    return typeHasEditorRef(type.DeclaringType);
                }
                if (type.IsByRef || type.IsArray)
                {
                    return typeHasEditorRef(type.GetElementType());
                }
                if (type.IsGenericType)
                {
                    foreach (var typeArg in type.GetGenericArguments())
                    {
                        if (typeHasEditorRef(typeArg,true ))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError(type);
                Debug.LogError(e);
                return false;
            }
          
        }

        static bool delegateHasEditorRef(Type delegateType)
        {
            if (typeHasEditorRef(delegateType)) return true;
            var method = delegateType.GetMethod("Invoke");
            if (method == null)
            {
                return false;
            }
            if (typeHasEditorRef(method.ReturnType)) return true;
            return method.GetParameters().Any(pinfo => typeHasEditorRef(pinfo.ParameterType));
        }
        /**

        // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
        [CSharpCallLua]
        static IEnumerable<Type> AllDelegate {
            get {
                BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
                List<Type> allTypes = new List<Type> ();
                var allAssemblys = new Assembly[] {
                    Assembly.Load ("Assembly-CSharp")
                };
                foreach (var t in (from assembly in allAssemblys from type in assembly.GetTypes () select type)) {
                    var p = t;
                    while (p != null) {
                        allTypes.Add (p);
                        p = p.BaseType;
                    }
                }
                allTypes = allTypes.Distinct ().ToList ();
                var allMethods = from type in allTypes
                from method in type.GetMethods (flag)
                select method;
                var returnTypes = from method in allMethods
                select method.ReturnType;
                var paramTypes = allMethods.SelectMany (m => m.GetParameters ()).Select (pinfo => pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType () : pinfo.ParameterType);
                var fieldTypes = from type in allTypes
                from field in type.GetFields (flag)
                select field.FieldType;
                return (returnTypes.Concat (paramTypes).Concat (fieldTypes)).Where (t => t.BaseType == typeof (MulticastDelegate) && !hasGenericParameter (t) && !delegateHasEditorRef (t)).Distinct ();
            }
        }
        //--------------end 热补丁自动化配置-------------------------
        */
        //黑名单
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>() {
            new List<string> () { "System.Xml.XmlNodeList", "ItemOf" },
            new List<string> () { "UnityEngine.WWW", "movie" },
#if UNITY_WEBGL
            new List<string> () { "UnityEngine.WWW", "threadPriority" },
#endif
            new List<string> () { "UnityEngine.Texture2D", "alphaIsTransparency" },
            new List<string> () { "UnityEngine.Security", "GetChainOfTrustValue" },
            new List<string> () { "UnityEngine.CanvasRenderer", "onRequestRebuild" },
            new List<string> () { "UnityEngine.Light", "areaSize" },
            new List<string> () { "UnityEngine.Light", "lightmapBakeType" },
            new List<string> () { "UnityEngine.WWW", "MovieTexture" },
            new List<string> () { "UnityEngine.WWW", "GetMovieTexture" },
            new List<string> () { "UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup" },
#if !UNITY_WEBPLAYER
            new List<string> () { "UnityEngine.Application", "ExternalEval" },
#endif
            new List<string> () { "UnityEngine.GameObject", "networkView" }, //4.6.2 not support
            new List<string> () { "UnityEngine.Component", "networkView" }, //4.6.2 not support
            new List<string> () { "System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
            new List<string> () { "System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" },
            new List<string> () { "System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity" },
            new List<string> () { "UnityEngine.MonoBehaviour", "runInEditMode" },

            new List<string> () { "Hugula.Loader.FileManifest", "WriteToFile","System.String" },
            new List<string> () { "Hugula.Loader.ManifestManager", "SimulateAssetBundleInEditor" },
            new List<string> () { "Hugula.ZipConfigs", "CreateInstance" },
            new List<string> () { "Hugula.ZipConfigs", "Delete" },

            new List<string> () { "Hugula.Loader.BackGroundDownload", "currentLoadingCount" },
            new List<string> () { "Hugula.Loader.BackGroundDownload", "loadingList" },
            new List<string> () { "Hugula.Loader.ABInfo", "EqualsDependencies","Hugula.Loader.ABInfo" },
            new List<string> () { "Hugula.Loader.ABInfo", "assetPath"},
            new List<string> () { "Hugula.Loader.CacheManager", "EditorCacheData"},
            new List<string> () { "Hugula.EnterLua", "isDebug"},


        };

#if UNITY_2018_1_OR_NEWER
        [BlackList]
        public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
        {
            if (memberInfo.DeclaringType.IsGenericType && memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                if (memberInfo.MemberType == MemberTypes.Constructor)
                {
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    var parameterInfos = constructorInfo.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                        {
                            return true;
                        }
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    var methodInfo = memberInfo as MethodInfo;
                    if (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        };
#endif
    }
}