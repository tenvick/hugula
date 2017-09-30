// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.IO;
using SLua;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lua = SLua.LuaSvr;
using Hugula.Utils;
using Hugula.Loader;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula
{
    /// <summary>
    /// 
    /// </summary>
    [CustomLuaClass]
    public class PLua : MonoBehaviour
    {

        public static string enterLua = "main";
        public LuaFunction onDestroyFn;
        public LuaFunction onAppPauseFn;
        public LuaFunction onAppQuitFn;
        public LuaFunction onAppFocusFn;

#if UNITY_EDITOR
        const string KeyDebugString = "_Plua_Debug_string";
        [SLua.DoNotToLua]
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

        public static Lua lua;

        private static bool isLuaInitFinished = false;
        private string luaMain = "";
        private LuaFunction _updateFn;

        #region mono

        public LuaFunction updateFn
        {
            get { return _updateFn; }
            set
            {
                _updateFn = value;
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (lua == null) PreInitLua();
        }

        IEnumerator Start()
        {
            while (isLuaInitFinished == false)
                yield return null;

            LoadScript();
#if !HUGULA_RELEASE
            // Debug.Log(luaBytesAsset);
            Debug.LogFormat("domain frame {0}", Time.frameCount);
#endif
            DoMain();
        }

	    void Update()
	    {
	        if (_updateFn != null) _updateFn.call();
	    }

        public static void DestoryLua()
        {
            if (lua != null && lua.luaState != null) lua.luaState.Close();
            lua = null;
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy = " + name);
            if (onDestroyFn != null) onDestroyFn.call();
            RemoveAllEvents();
            isLuaInitFinished = false;
            if (_instance == this) _instance = null;
            // if (ManifestManager.assetBundleManifest != null) UnityEngine.Object.Destroy(ManifestManager.assetBundleManifest);

        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (onAppFocusFn != null) onAppFocusFn.call(this, focusStatus);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (onAppPauseFn != null) onAppPauseFn.call(this, pauseStatus);
        }

        void OnApplicationQuit()
        {
            if (onAppQuitFn != null) onAppQuitFn.call(this, true);
        }
        #endregion

        #region private method

        private void SetLuaPath()
        {
            this.luaMain = "return require(\"" + enterLua + "\") \n";
        }

        private void LoadScript()
        {
            SetLuaPath();

            RegisterFunc();
#if UNITY_EDITOR
            Debug.LogFormat("<color=green>running {0} mode </color> <color=#8cacbc> change( menu Hugula->Debug Lua)</color>", isDebug ? "debug" : "release");
#endif
        }

        /// <summary>
        /// lua begin
        /// </summary>
        private void DoMain()
        {
            CUtils.DebugCastTime("");
            lua.luaState.doString(this.luaMain);
        }

        #endregion

        #region public method

        /// <summary>
        /// Pre Init Lua.
        /// </summary>
        /// <param name="sconds">Sconds.</param>
        public static void PreInitLua()
        {
            if (lua == null) lua = new Lua();
            CUtils.DebugCastTime("");
            lua.init(null, () =>
            {
                CUtils.DebugCastTime("Slua binded");
                isLuaInitFinished = true;
            }, LuaSvrFlag.LSF_3RDDLL);
        }

        /// <summary>
        /// ReStart.
        /// </summary>
        /// <param name="sconds">Sconds.</param>
        public void ReStart(float sconds)
        {
            GameObject.Destroy(this.gameObject);
            LoadFirstHelper.ReOpen(sconds);
            //StartCoroutine(ReOpen(sconds));
        }

        /// <summary>
        /// Removes all events.
        /// </summary>
        public void RemoveAllEvents()
        {
            onDestroyFn = null;
            onAppPauseFn = null;
            onAppQuitFn = null;
            onAppFocusFn = null;
        }
        #endregion

        #region toolMethod

        /// <summary>
        /// load lua bytes
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private byte[] LoadLuaBytes(string name)
        {
            byte[] ret = null;
#if UNITY_EDITOR_WIN
            string cryName = CUtils.GetRightFileName(string.Format("{0}.{1}", name, Common.ASSETBUNDLE_SUFFIX));
            string path = CUtils.PathCombine(Application.dataPath, Common.LUACFOLDER + "/win");
            path = CUtils.PathCombine(path, cryName);
            ret = File.ReadAllBytes(path);
#elif UNITY_EDITOR_OSX
            string cryName = CUtils.GetRightFileName (string.Format ("{0}.{1}", name, Common.ASSETBUNDLE_SUFFIX));
            string path = CUtils.PathCombine (Application.dataPath, Common.LUACFOLDER + "/osx");
            path = CUtils.PathCombine (path, cryName);
            ret = File.ReadAllBytes (path);
#elif UNITY_IOS
            string cryName = "";
            if (System.IntPtr.Size == 4)
                cryName = CUtils.GetRightFileName (name);
            else
                cryName = CUtils.GetRightFileName (string.Format ("{0}_64", name));

            string path = CUtils.PathCombine (CUtils.realPersistentDataPath, cryName + Common.CHECK_ASSETBUNDLE_SUFFIX);
            if (File.Exists (path)) {
                ret = File.ReadAllBytes (path);
            } else {
                var textAsset =(TextAsset)Resources.Load ("luac/"+cryName);
                ret = textAsset.bytes; // --Resources.Load
                Resources.UnloadAsset(textAsset);
            }
#else //android
            string cryName = CUtils.GetRightFileName(name);
            string path = CUtils.PathCombine(CUtils.realPersistentDataPath, cryName + Common.CHECK_ASSETBUNDLE_SUFFIX);
            if (File.Exists(path))
            {
                ret = File.ReadAllBytes(path);
            }
            else
            {
                var textAsset = (TextAsset)Resources.Load ("luac/"+cryName); //pc luaBundle.LoadAsset<TextAsset>(cryName);
                ret = textAsset.bytes; // --Resources.Load
                Resources.UnloadAsset(textAsset);
            }
#endif
            return ret;
        }

        private void RegisterFunc()
        {
            LuaState.loaderDelegate = Loader;
        }

        /// <summary>
        ///  loader
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private byte[] Loader(string name)
        {
            byte[] str = null;
#if UNITY_EDITOR

            if (isDebug)
            {
                string name1 = name.Replace('.', '/');
                string path = Application.dataPath + "/Lua/" + name1 + ".lua";
                if (!File.Exists(path))
                    path = Application.dataPath + "/Config/" + name1 + ".lua";

                if (File.Exists(path))
                {
                    str = File.ReadAllBytes(path);
                }
                else
                {
                    Debug.LogWarningFormat("lua({0}) path={1} not exists.", name, path);
                    name = name.Replace('.', '+').Replace('/', '+');
                    str = LoadLuaBytes(name);
                }
            }
            else
            {
                name = name.Replace('.', '+').Replace('/', '+');
                str = LoadLuaBytes(name);
            }

#elif UNITY_STANDALONE
        
            name = name.Replace('.', '/');
            string path = Application.dataPath + "/config_data/" + name + ".lua"; //���ȶ�ȡ�����ļ�
            if (File.Exists(path))
            {
                str = File.ReadAllBytes(path);
            }
            else
            {
                name = name.Replace('.', '+').Replace('/', '+');
                str = LoadLuaBytes(name);
            }

#else
            name = name.Replace ('.', '+').Replace ('/', '+');
            str = LoadLuaBytes (name);
#endif
            return str;
        }

        public static Coroutine MultipleRequires(string[] requires)
        {
            return instance.StartCoroutine(MultipleRequiresDo(requires, null));
        }

        public static Coroutine MultipleRequires(string[] requires, LuaFunction luafn)
        {
            return instance.StartCoroutine(MultipleRequiresDo(requires, luafn));
        }

        public static Coroutine Delay(LuaFunction luafun, float time, params object[] args)
        {
            return instance.StartCoroutine(DelayDo(luafun, time, args));
        }

        public static void StopDelay(Coroutine coroutine)
        {
            if (coroutine != null)
                instance.StopCoroutine(coroutine);
        }

        public static void StopAllDelay()
        {
            instance.StopAllCoroutines();
        }

        private static IEnumerator MultipleRequiresDo(string[] requires, LuaFunction onCompFn)
        {
            var item = requires.GetEnumerator();
            string luastr = string.Empty;
            while (item.MoveNext())
            {
                luastr = item.Current.ToString();
                if ("eof".Equals(luastr))
                    yield return null;
                else if (luastr.Contains("("))
                {
#if UNITY_EDITOR
                    // Debug.LogFormat("{0},framcount={1}", luastr, Time.frameCount);
#endif
                    lua.luaState.doString(luastr);
                }
                else
                {
#if UNITY_EDITOR
                    // Debug.LogFormat("{0},framcount={1}", luastr, Time.frameCount);
#endif    
                    lua.luaState.doFile(luastr);
                }
            }
            yield return null;
            // Debug.LogFormat("call function {0} ;", onCompFn != null);
            if (onCompFn != null) onCompFn.call();
        }

        private static IEnumerator DelayDo(LuaFunction luafun, float time, params object[] args)
        {
            yield return new WaitForSeconds(time);
            luafun.call(args);
        }

        #endregion

        #region static
        private static PLua _instance;

        public static PLua instance
        {
            get
            {
                if (_instance == null)
                {
                    var gam = new GameObject("PLua");
                    _instance = gam.AddComponent<PLua>();
                }
                return _instance;
            }
        }
        #endregion

    }
}