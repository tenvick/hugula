// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

using UnityEngine;
using System.Collections;
using SLua;

/// <summary>
/// L highway.
/// </summary>
[SLua.CustomLuaClass]
public class LHighway : CHighway {


	public LHighway()
		: base()
	{
		base.OnAllComplete += LHighway_onAllComplete;
		base.OnProgress += LHighway_onProgress;
		base.OnSharedComplete += LHighway_onSharedComplete;
	}
	
    /// <summary>
    /// 加载luatable里面的request
    /// </summary>
    /// <param name="reqs"></param>
	public void LoadLuaTable(LuaTable reqs)
	{
        //System.Collections.IEnumerator luatb = reqs.GetEnumerator();
        //while (luatb.MoveNext())
        //{
        //    AddReqToQueue((CRequest)luatb.Current);
        //}
        foreach (var pair in reqs)
        {
            AddReqToQueue((CRequest)pair.value);
        }
        BeginQueue();
	}

	#region protected method
	
	void LHighway_onSharedComplete(CRequest req)
	{
		if (onSharedCompleteFn != null)
			onSharedCompleteFn.call(req);
	}
	
	void LHighway_onProgress(CHighway loader, HighwayEventArg arg)
	{
		if (onProgressFn != null)
			onProgressFn.call(loader, arg);
	}
	
	void LHighway_onAllComplete(CHighway loader)
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
	
	private static LHighway _instance;
	
	public static LHighway instance
	{
		get
		{
			if (_instance == null)
				_instance = new LHighway();
			return _instance;
		}
	}
}
