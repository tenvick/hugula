// Copyright (c) 2022 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hugula;
using Hugula.ResUpdate;
using Hugula.Databinding;
using Hugula.Framework;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using XLua;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Hugula
{

    [XLua.LuaCallCSharp]
    public class EnterLua : MonoBehaviour
    {

#if UNITY_EDITOR
        const string KeyDebugString = "_Plua_Debug_string";

        public static bool isDebug
        {
            get
            {
                bool _debug = EditorPrefs.GetBool(KeyDebugString, true);
                return _debug;
            }
            set
            {
                EditorPrefs.SetBool(KeyDebugString, value);
            }
        }
#endif
        /// <summary>
        /// 一次释放的asset数量
        /// </summary>
        public static int RELEASE_COUNT = 50;
        [SerializeField] string enterLua = "begin"; //main

        internal static LuaEnv luaenv;
        internal static EnterLua instance; //特别单例
        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            var ins = Executor.instance;
            ins = null;
            ReloadLua();
            DontDestroyOnLoad(this.gameObject);
            BehaviourSingletonManager.CanCreateInstance();
            SingletonManager.CanCreateInstance();
            Hugula.ResLoader.Init();

        }
        #region  hot update
        private AssetBundle m_StreamingLuaBundle;

        private AssetBundle GetStreamingLuaBundle()
        {
            if (!m_StreamingLuaBundle)
            {
                var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), Common.LUA_BUNDLE_NAME);
                url = CUtils.GetAndroidABLoadPath(url);
                m_StreamingLuaBundle = AssetBundle.LoadFromFile(url,0,Common.BUNDLE_OFF_SET);
            }
            return m_StreamingLuaBundle;
        }
        private AssetBundle m_PersistentLuaBundle;

        private AssetBundle GetPersistentLuaBundle()
        {
            if (!m_PersistentLuaBundle)
            {
                var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), Common.LUA_BUNDLE_NAME);
                url = CUtils.GetAndroidABLoadPath(url);
                m_PersistentLuaBundle = AssetBundle.LoadFromFile(url,0,Common.BUNDLE_OFF_SET);
            }
            return m_PersistentLuaBundle;
        }

        private BundleManifest m_LuaPersistentBundleManifest;
        internal static bool luaPersistentBundleManifestIsDirty = true;
        private BundleManifest GetLuaPersistentBundleManifest()
        {
            if (luaPersistentBundleManifestIsDirty && m_LuaPersistentBundleManifest == null)
            {
                m_LuaPersistentBundleManifest = FileManifestManager.FindPersistentBundleManifest(Common.LUA_BUNDLE_NAME);
                luaPersistentBundleManifestIsDirty = false;
            }

            return m_LuaPersistentBundleManifest;
        }

        #endregion
        IEnumerator Start()
        {
            while (!ResLoader.Ready)
                yield return null;
            
            Hugula.Atlas.AtlasManager.instance.Init();

            yield return null;
            luaenv.DoString("require('" + enterLua + "')");
#if UNITY_EDITOR
            Debug.LogFormat("<color=green>running {0} mode </color> <color=#8cacbc> change( menu Hugula->Debug Lua)</color>", isDebug ? "debug" : "release");
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyUp(KeyCode.F5))
            {
                GlobalDispatcher.instance?.Dispatch(DispatcherEvent.F5, null);
            }
            else if (Input.GetKeyUp(KeyCode.F6))
            {
                GlobalDispatcher.instance?.Dispatch(DispatcherEvent.F6, null);
            }

#endif
            //if(Time.frameCount %10 ==0)
            ResLoader.DoReleaseNext(Time.frameCount, RELEASE_COUNT);
        }

        void OnDestroy()
        {
            // #if UNITY_EDITOR
            ClearLuaRef();
            // #endif
        }

        void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
        }

        internal static void BeforeLuaDispose()
        {
            GlobalDispatcher.instance?.Dispose();
            ExpressionUtility.instance?.Dispose();
            Hugula.Framework.SingletonManager.DisposeAll();
            Hugula.Framework.BehaviourSingletonManager.DisposeAll();
            // luaenv.DoString(@"print('')");
        }

        internal static void DisposeLua()
        {
            luaenv?.GC();
            luaenv?.DoString(@"
                    local util = require 'xlua.util'
            util.print_func_ref_by_csharp()");

            if (luaenv != null)
            {
                luaenv.Dispose();
                luaenv = null;
            }
            // if (luaenv != null) luaenv.Dispose ();
        }

        private void ClearLuaRef()
        {
            BeforeLuaDispose();
        }

        private void ReloadLua()
        {
            // LuaEntry
            if (luaenv != null)
            {
                ClearLuaRef();
                luaenv.Dispose();
                luaenv = null;
            }
            luaenv = new LuaEnv();
            luaenv.AddLoader(Loader);
        }

        /// <summary>
        ///  loader
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private byte[] Loader(ref string name)
        {
            byte[] str = null;

#if UNITY_EDITOR
            string path = Application.dataPath + "/Lua/" + name.Replace('.', '/') + ".lua";
            bool exist = File.Exists(path);
            if (isDebug && exist)
            {
                str = File.ReadAllBytes(path); //LuaState.CleanUTF8Bom(
            }
            else
            {
                if (!exist) Debug.LogErrorFormat("file {0} does't exists. ", path);
                str = LoadLuaBytes(name);// (Common.LUA_PREFIX + name.Replace('.', '+'));
            }
#elif UNITY_STANDALONE && !HUGULA_RELEASE

            var path = Application.dataPath + "/config_data/" + name.Replace ('.', '/') + ".lua"; //���ȶ�ȡ�����ļ�
            if (File.Exists (path)) {
                str = File.ReadAllBytes (path);
            } else {
                str = LoadLuaBytes(name);//(Common.LUA_PREFIX + name.Replace ('.', '+'));
            }
            if (str == null) Debug.LogErrorFormat ("lua({0}) path={1} not exists.", name, path);
#else
#if !HUGULA_RELEASE||LUA_ANDROID_DEBUG
            var path = Application.persistentDataPath + "/lua/" + name.Replace ('.', '/') + ".lua";
            if (File.Exists (path)) {
                str = File.ReadAllBytes (path);
            } else
#endif  
            {
                str = LoadLuaBytes(name);// (Common.LUA_PREFIX + name.Replace ('.', '+'));
            }
#endif

#if UNITY_EDITOR
            if (str == null)
            {
                if (isDebug)
                    Debug.LogErrorFormat("lua ({0}) path={1} not exists.", name, path);
                else
                    Debug.LogErrorFormat("the file(Assets/lua_bundle/{0}.lua)  did't exists.", name);

            }

            name = path;//chunkname 调试用
#endif
            return str;
        }

        /// <summary>
        /// load lua bytes
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private byte[] LoadLuaBytes(string name)
        {
            byte[] ret = null;
            var bundleManifest = GetLuaPersistentBundleManifest();
            AssetBundle luaAb = null;
            if (bundleManifest && bundleManifest.GetFileResInfo(name) != null)//如果有热更新走热更新
            {
                luaAb = GetPersistentLuaBundle();
                #if UNITY_EDITOR || UNITY_STANDALONE
                    Debug.Log($"load lua ({name}) from persistent: {CUtils.GetRealPersistentDataPath()}/{Common.LUA_BUNDLE_NAME}");
                #endif
            }
            else
            {
                luaAb = GetStreamingLuaBundle();
            }

            var txt = luaAb?.LoadAsset<TextAsset>(name);
             if (txt != null)
            {
                ret = txt.bytes;
                Resources.UnloadAsset(txt); //释放ab资源
                luaAb?.Unload(false);
            }

            return ret;

        }

        private static bool m_StopAllCoroutines = false;
        public static void MarkStopAllCoroutines()
        {
            m_StopAllCoroutines = true;
            instance?.StopAllCoroutines();
        }
        //重启动游戏
        public static void ReOpen(float sconds)
        {
            // System.GC.Collect();
            // luaenv?.GC();
            // var ins = Manager.Get<EnterLua>();
            // GameObject.Destroy(ins?.gameObject);
            // await Task.Delay((int)(sconds * 1000));
            // BeforeLuaDispose();
            UnityEngine.SceneManagement.SceneManager.LoadScene("re_loading");
            Debug.LogFormat("ReOpen !");
        }

        static internal string LuaTraceback()
        {
            return Hugula.EnterLua.luaenv?.DoString("return debug.traceback('')")[0].ToString();
        }

        #region delay

        public static void StopDelay(object arg)
        {
            var ins = instance;
            if (ins != null && arg is Coroutine)
                ins.StopCoroutine((Coroutine)arg);
        }

        public static Coroutine Delay(LuaFunction luafun, float time, object args)
        {
            if (m_StopAllCoroutines) return null;
            var ins = instance;
            var _corout = DelayDo(luafun, time, args);
            var cor = ins.StartCoroutine(_corout);
            return cor;
        }

        private static IEnumerator DelayDo(LuaFunction luafun, float time, object args)
        {
            yield return new WaitForSeconds(time);
            luafun.Call(args);
        }

        public static Coroutine DelayFrame(LuaFunction luafun, int frame, object args)
        {
            if (m_StopAllCoroutines) return null;
            var ins = instance;
            var _corout = DelayFrameDo(luafun, frame, args);
            var cor = ins.StartCoroutine(_corout);
            return cor;
        }

        private static IEnumerator DelayFrameDo(LuaFunction luafun, int frame, object args)
        {
            var waitFrame = WaitForFrameCountPool.Get();
            waitFrame.SetEndFrame(frame);
            yield return waitFrame;
            WaitForFrameCountPool.Release(waitFrame);
            luafun.Call(args);
        }

        static Hugula.Utils.ObjectPool<WaitForFrameCount> WaitForFrameCountPool = new Hugula.Utils.ObjectPool<WaitForFrameCount>(null, null);

        [XLua.LuaCallCSharp]
        public class WaitForFrameCount : IEnumerator
        {
            int m_EndCount;
            public WaitForFrameCount(int frameCount)
            {
                SetEndFrame(frameCount);
            }

            public WaitForFrameCount()
            {

            }

            public void SetEndFrame(int frameCount)
            {
                m_EndCount = Time.frameCount + frameCount;
            }

            bool IEnumerator.MoveNext()
            {
                return Time.frameCount <= m_EndCount;
            }

            object IEnumerator.Current
            {
                get
                {
                    return null;
                }
            }

            void IEnumerator.Reset()
            {

            }
        }
        #endregion
    }
}
