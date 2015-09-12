// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Text;

using LuaInterface;
using SLua;
using Lua = SLua.LuaSvr;
//using LuaState = System.IntPtr;
//using MonoPInvokeCallbackAttribute = SLua.MonoPInvokeCallbackAttribute;
//using LuaCSFunction = SLua.LuaCSFunction;

[CustomLuaClass]
public class PLua : MonoBehaviour
{

    public static string enterLua = "main";
    public LuaFunction onDestroyFn;
    public static bool isDebug = true;

    public Lua lua;
    public LNet net;
    public LNet ChatNet;
    //public LuaState luaState;
    //搜索路径
    public static string package_path { private set; get; }
    //lua资源缓存路径
    public static Dictionary<string, TextAsset> luacache;

    #region priveta
    //入口lua
    private string luaMain = "";
    private const string initLua = @"require(""core.unity3d"")";

    //程序集名key
    private const string assemblyname = "assemblyname";
    private LuaFunction _updateFn;

    #endregion

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
        luacache = new Dictionary<string, TextAsset>();
        lua = new Lua();
        _instance = this;
        LoadScript();
    }

    void Start()
    {
        net = LNet.instance;
        ChatNet = LNet.ChatInstance;
        LoadBundle(true);
    }

    void Update()
    {
        if (net != null) net.Update();
        if (ChatNet != null) ChatNet.Update();
        if (_updateFn != null) _updateFn.call();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (net != null) net.OnApplicationPause(pauseStatus);
    }

    void OnDestroy()
    {
        if (onDestroyFn != null) onDestroyFn.call();
        updateFn = null;
        if (lua != null) lua.luaState.Close();
        lua = null;
        _instance = null;
        if(net!=null)net.Dispose();
        net = null;
		if(ChatNet!=null)ChatNet.Dispose();
        ChatNet = null;
        luacache.Clear();
    }

    #endregion

    private void SetLuaPath()
    {
        //luaBegin.Append("return require(\"" + enterLua + "\") \n");
        //this.luaMain = luaBegin.ToString();
        this.luaMain = "return require(\"" + enterLua + "\") \n";
        Debug.Log(luaMain);
    }

    private void LoadScript()
    {
        SetLuaPath();

        RegisterFunc();

        Assembly assb = Assembly.GetExecutingAssembly();
        string assemblyname = assb.GetName().Name;
        lua.luaState["assemblyname"] = assemblyname;
    }

    /// <summary>
    /// 加载lua bundle
    /// </summary>
    /// <returns></returns>
    private IEnumerator loadLuaBundle(bool domain,LuaFunction onLoadedFn)
    {
        string keyName = "";
        string luaP = CUtils.GetAssetFullPath("font.u3d");
        //Debug.Log("load lua bundle" + luaP);
        WWW luaLoader = new WWW(luaP);
        yield return luaLoader;
        if (luaLoader.error == null)
        {
			byte[] byts=CryptographHelper.Decrypt(luaLoader.bytes,DESHelper.instance.Key,DESHelper.instance.IV);
			AssetBundle item = AssetBundle.CreateFromMemoryImmediate(byts);

//            item = luaLoader.assetBundle;
#if UNITY_5
                    TextAsset[] all = item.LoadAllAssets<TextAsset>();
                    foreach (var ass in all)
                    {
                        keyName = Regex.Replace(ass.name,@"\.","");
                        //Debug.Log("cache : " + keyName);
                        luacache[keyName] = ass;
                    }
#else
            UnityEngine.Object[] all = item.LoadAll(typeof(TextAsset));
            foreach (var ass in all)
            {
                keyName = Regex.Replace(ass.name,@"\.","");
                Debug.Log(keyName + " complete");
                luacache[keyName] = ass as TextAsset;
            }
#endif
                    //Debug.Log("loaded lua bundle complete" + luaP);
//            luaLoader.assetBundle.Unload(false);
			item.Unload(false);
            luaLoader.Dispose();
        }

        DoUnity3dLua();
        if (domain)
            DoMain();

        if (onLoadedFn != null) onLoadedFn.call();
    }

    #region public

    public void DoUnity3dLua()
    {
        //Require(
        lua.luaState.doString(initLua);//执行unity3dlua
        //ToLuaCS.InitValueTypeChange();
    }

    /// <summary>
    /// 执行开始文件
    /// </summary>
    public void DoMain()
    {
        lua.luaState.doString(this.luaMain);
    }

    /// <summary>
    /// 加载lua 打包文件
    /// </summary>
    public void LoadBundle(bool domain)
    {
        StopCoroutine(loadLuaBundle(domain, null));
        StartCoroutine(loadLuaBundle(domain, null));
    }

    /// <summary>
    /// 加载lua 打包文件
    /// </summary>
    public void LoadBundle(LuaFunction onLoadedFn)
    {
        //Debug.Log(" refresh LoadBundle ");
        StopCoroutine(loadLuaBundle(false, onLoadedFn));
        StartCoroutine(loadLuaBundle(false, onLoadedFn));
    }

    #endregion

    #region toolMethod

    public void RegisterFunc()
    {
        LuaState.loaderDelegate=Loader;
    }

    /// <summary>
    /// 自定义loader
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static byte[] Loader(string name)
    {
        byte[] str = null;
        name = name.Replace('.','/'); // Regex.Replace(name, @'.', @'/');
        //Debug.Log(" loade : " + name);
#if UNITY_EDITOR

        if (isDebug)
        {
            string path = Application.dataPath + "/Lua/" + name+".lua";
//			string path = Application.dataPath + "/Tmp/PW/" + name+".bytes";
            //Debug.Log(" Loader: "+path);
            try
            {
				str =  File.ReadAllBytes(path);
            }
            catch
            {
                //LuaDLL.luaL_error(ToLuaCS.lua.L, "Loader file failed: " + name);
            }
        } 
        else
        {
            name =  Regex.Replace(name, @"/", "");
            if (luacache.ContainsKey(name))
            {
                TextAsset file = luacache[name];
                str = file.bytes;
                //Debug.Log(name + " exist");
            }
        }
#else
		name = Regex.Replace(name,"/",""); 

        if(luacache.ContainsKey(name))
        {
        TextAsset file = luacache[name];
        str = file.bytes;
        }
#endif
        return str;
    }


    public static void Log(object msg)
    {
        Debug.Log(msg);
    }

    public static void Delay(LuaFunction luafun, float time, object args = null)
    {
        _instance.StartCoroutine(DelayDo(luafun, args, time));
    }

    public static void StopDelay(string methodName = "DelayDo")
    {
        _instance.StopCoroutine(methodName);
    }

    private static IEnumerator DelayDo(LuaFunction luafun, object arg, float time)
    {
        yield return new WaitForSeconds(time);
        luafun.call(arg);
    }

    #endregion

    #region static
    private static PLua _instance;

    public static PLua instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion

}
