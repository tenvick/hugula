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


	protected  override object  GetCache(string Key)
	{
		if (this._cache != null)
			return _cache [Key];
		else
			return null;
	}
	
	protected override void SetCache(string key,object Value)
	{
		if (onCacheFn != null) {
			onCacheFn.call(key,Value);
				}
		else if (this._cache != null)
			this._cache [key] = Value;
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
	
	private LuaTable _cache;
	
	public override object cache
	{
		get { return _cache; }
		set
		{
			if (value is LuaTable)
				_cache = (LuaTable)value;
		}
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
