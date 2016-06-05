using UnityEngine;
using System.Collections;
using SLua;

[SLua.CustomLuaClass]
public class LResLoader : CResLoader
{
    // Use this for initialization
    void Start()
    {
        base.OnAllComplete += L_onAllComplete;
        base.OnProgress += L_onProgress;
        base.OnSharedComplete += L_onSharedComplete;
    }

    /// <summary>
    /// 加载luatable里面的request
    /// </summary>
    /// <param name="reqs"></param>
    public void LoadLuaTable(LuaTable reqs)
    {
        pushGroup = true;

        foreach (var pair in reqs)
        {
            AddReqToQueue((CRequest)pair.value);
        }

        pushGroup = false;

        BeginQueue();
    }

    #region protected method

    void L_onSharedComplete(CRequest req)
    {
        if (onSharedCompleteFn != null)
            onSharedCompleteFn.call(req);
    }

    void L_onProgress(CResLoader loader, LoadingEventArg arg)
    {
        if (onProgressFn != null)
            onProgressFn.call(loader, arg);
    }

    void L_onAllComplete(CResLoader loader)
    {
        if (onAllCompleteFn != null)
            onAllCompleteFn.call(loader);
    }

    #endregion

    #region  delegate and event

    public LuaFunction onAllCompleteFn;
    public LuaFunction onProgressFn;
    public LuaFunction onSharedCompleteFn;
    public LuaFunction onCacheFn;
    #endregion

    #region instance
    //protected static LResLoader _instance;
    /// <summary>
    /// the GetInstance
    /// </summary>
    /// <returns></returns>
    public static CResLoader instance
    {
        get
        {
            if (_instance == null)
            {
                var chighway = new GameObject("CResManager");
                _instance = chighway.AddComponent<LResLoader>();
            }
            return _instance;
        }
    }
    #endregion
}
