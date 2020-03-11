// Copyright (c) 2022 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hugula.Databinding;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnterLua : MonoBehaviour {
#if UNITY_EDITOR
    const string KeyDebugString = "_Plua_Debug_string";

    public static bool isDebug {
        get {
            bool _debug = EditorPrefs.GetBool (KeyDebugString, true);
            return _debug;
        }
        set {
            EditorPrefs.SetBool (KeyDebugString, value);
        }
    }
#endif

    public static string enterLua = "begin"; //main

    internal static LuaEnv luaenv;

    // Start is called before the first frame update
    void Awake () {
        DontDestroyOnLoad (this.gameObject);
        Executor.Initialize ();
        if (ManifestManager.fileManifest == null)
            ManifestManager.LoadFileManifest (null);
#if UNITY_EDITOR
        Debug.LogFormat ("<color=green>running {0} mode </color> <color=#8cacbc> change( menu Hugula->Debug Lua)</color>", isDebug ? "debug" : "release");
#endif
        if (luaenv == null) luaenv = new LuaEnv ();
        luaenv.AddLoader (Loader);
        luaenv.DoString ("require('" + enterLua + "')");
    }

    // Update is called once per frame
    void Update () {
        if (luaenv != null) {
            luaenv.Tick ();
        }
    }

    void OnDestroy () {
                luaenv.DoString (@"
                local util = require 'xlua.util'
        util.print_func_ref_by_csharp()");
        // if (luaenv != null) luaenv.Dispose ();
        ExpressionUtility.Dispose();
    }

    void OnApplicationQuit () {
        Debug.Log ("OnApplicationQuit");
    }

    /// <summary>
    ///  loader
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private byte[] Loader (ref string name) {
        byte[] str = null;
#if UNITY_EDITOR
        string name1 = name.Replace ('.', '/');
        string path = Application.dataPath + "/Lua/" + name1 + ".lua";
        // if (!File.Exists (path))
        //     path = Application.dataPath + "/Config/" + name1 + ".lua";

        if (File.Exists (path)) {
            str = File.ReadAllBytes (path); //LuaState.CleanUTF8Bom(
        } else {
            Debug.LogWarningFormat ("lua({0}) path={1} not exists.", name, path);
            name = name.Replace ('.', '+').Replace ('/', '+');
            str = LoadLuaBytes (name);
        }
#else
        name = name.Replace ('.', '+').Replace ('/', '+');
        string cryName = "";
        if (System.IntPtr.Size == 4)
            cryName = CUtils.GetRightFileName (name);
        else
            cryName = CUtils.GetRightFileName (string.Format ("{0}_64", name));

        string abName = cryName + Common.CHECK_ASSETBUNDLE_SUFFIX;
        bool isupdate = ManifestManager.CheckIsUpdateFile (abName);

#if !HUGULA_NO_LOG
        Debug.LogFormat ("loader lua {0}={1}", name, cryName);
#endif

        string path = null;
        if (isupdate && File.Exists (path = CUtils.PathCombine (CUtils.realPersistentDataPath, abName))) {
            str = File.ReadAllBytes (path);
        } else {
            var textAsset = (TextAsset) Resources.Load ("luac/" + cryName);
            if (textAsset == null) {
                Debug.LogWarningFormat ("lua({0}={1}) bytes error!", name, cryName);
            } else
                str = textAsset.bytes; // --Resources.Load
            Resources.UnloadAsset (textAsset);
        }
#endif
        return str;
    }

    /// <summary>
    /// load lua bytes
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private byte[] LoadLuaBytes (string name) {
        byte[] ret = null;
        string cryName = "";
        if (System.IntPtr.Size == 4)
            cryName = CUtils.GetRightFileName (name);
        else
            cryName = CUtils.GetRightFileName (string.Format ("{0}_64", name));

        string abName = cryName + Common.CHECK_ASSETBUNDLE_SUFFIX;
        bool isupdate = ManifestManager.CheckIsUpdateFile (abName);
        string path = CUtils.PathCombine (CUtils.realPersistentDataPath, abName);
        if (isupdate && File.Exists (path)) {
            ret = File.ReadAllBytes (path);
        } else {
            var textAsset = (TextAsset) Resources.Load ("luac/" + cryName);
            ret = textAsset.bytes; // --Resources.Load
            Resources.UnloadAsset (textAsset);
        }
        return ret;
    }

    //重启动游戏
    public static void ReOpen (float sconds) {
        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
        Debug.LogFormat ("ReOpen !");
    }
}