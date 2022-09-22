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
        internal string enterLua = "begin"; //main

        internal static LuaEnv luaenv;
        internal static EnterLua instance; //特别单例
        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            var ins = Executor.instance;
            ins = null;
            DontDestroyOnLoad(this.gameObject);
        }
        #region  hot update
        private LuaBundleRead m_StreamingLuaBundle;

        private LuaBundleRead GetStreamingLuaBundle()
        {
            if (m_StreamingLuaBundle == null)
            {
                var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), Common.LUA_BUNDLE_NAME);
                url = CUtils.GetAndroidABLoadPath(url);
                bool isAndroid = false;
#if UNITY_ANDROID && !UNITY_EDITOR
                isAndroid = true;
#endif
                m_StreamingLuaBundle = new LuaBundleRead(url,isAndroid);
            }
            return m_StreamingLuaBundle;
        }
        private LuaBundleRead m_PersistentLuaBundle;

        private LuaBundleRead GetPersistentLuaBundle()
        {
            if (m_PersistentLuaBundle == null)
            {
                var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), CUtils.GetPersistentBundleFileName(Common.LUA_BUNDLE_NAME));
                m_PersistentLuaBundle = new LuaBundleRead(url);
            }
            return m_PersistentLuaBundle;
        }

        private BundleManifest m_LuaPersistentBundleManifest;
        internal bool luaPersistentBundleManifestIsDirty = true;
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
        //启动lua
        IEnumerator Start()
        {
            ReloadLua();
            BehaviourSingletonManager.CanCreateInstance();
            SingletonManager.CanCreateInstance();

            Hugula.Atlas.AtlasManager.instance.Init();

            yield return null;
            //pb
#if UNITY_EDITOR
            if (isDebug)
            {
                string PROTO_PATH = Application.dataPath + "/proto";
                var files = Directory.GetFiles(PROTO_PATH, "*.proto", SearchOption.TopDirectoryOnly);
                LuaFunction loadFile = luaenv.DoString(
                    $@"
                    local pc = require('protoc').new()
                    pc.experimental_allow_proto3_optional = true
                    pc:addpath('{PROTO_PATH}')
                    return function (txt)
                        return pc:loadfile(txt)
                    end"
                )[0] as LuaFunction;
                foreach (var pbFile in files)
                {
                    loadFile.Func<string, string>(Path.GetFileName(pbFile));
                }
            }
            else
            {
                var asynHandle = Addressables.LoadAssetsAsync<TextAsset>(Common.PROTO_GROUP_NAME, null);
                while (!asynHandle.IsDone)
                {
                    yield return null;
                }
                LuaFunction loadFunc = luaenv.DoString("return require('pb').load")[0] as LuaFunction;
                yield return null;
                foreach (var item in asynHandle.Result)
                {
                    loadFunc.Action<byte[]>(item.bytes);
                }
            }
#else
            var asynHandle = Addressables.LoadAssetsAsync<TextAsset> (Common.PROTO_GROUP_NAME, null);
            while (!asynHandle.IsDone) {
                yield return null;
            }
            LuaFunction loadFunc = luaenv.DoString ("return require('pb').load") [0] as LuaFunction;
            yield return null;
            foreach (var item in asynHandle.Result) {
                loadFunc.Action<byte[]> (item.bytes);
            }
#endif

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
            m_PersistentLuaBundle?.Unload();
            m_PersistentLuaBundle = null;
            m_StreamingLuaBundle?.Unload();
            m_StreamingLuaBundle = null;
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
                    Debug.LogError($"the file({Common.LUACFOLDER}/{name}.lua)  did't exists.");

            }
#endif
            name = path;//chunkname 调试用

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
#if PROFILER_DUMP && LUA_REQUIRE_PROFILER
            using (Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("lua require:", name,null,true))
            {
#endif
            var bundleManifest = GetLuaPersistentBundleManifest();
            LuaBundleRead luaAb = null;
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

            ret = luaAb?.LoadBytes(name);
#if PROFILER_DUMP && LUA_REQUIRE_PROFILER
            }
#endif
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
            Debug.LogWarning($"LoadScene(re_loading) time:{System.DateTime.Now.ToString()}");
            UnityEngine.SceneManagement.SceneManager.LoadScene("re_loading");
        }

        static internal string LuaTraceback()
        {
            return Hugula.EnterLua.luaenv?.DoString("return debug.traceback('',3)")[0].ToString().Replace("stack traceback:","");
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
            var cor = ins?.StartCoroutine(_corout);
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
