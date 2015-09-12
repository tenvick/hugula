// Copyright (c) 2015 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// C transport.
/// </summary>
[SLua.CustomLuaClass]
public class CTransport : MonoBehaviour {

	#region private member
	private WWW www;
	private CRequest _req;
    //private string assetType;
	#endregion

	#region public member
	/// <summary>
	/// if transport car is free
	/// </summary>
	/// <value><c>true</c> if is free; otherwise, <c>false</c>.</value>
	public bool isFree{private set;get;}

	/// <summary>
	/// The req.
	/// </summary>
	public CRequest req {
		get {
			return _req;
		}
	}

	/// <summary>
	/// The key.
	/// </summary>
	public string key;

    /// <summary>
    /// 依赖关系
    /// </summary>
    public static  AssetBundleManifest m_AssetBundleManifest = null;

	#endregion

	#region evnet
	public System.Action<CTransport,float> OnProcess;
	public System.Action<CTransport,CRequest,IList<CRequest>> OnComplete;
	public System.Action<CTransport,CRequest> OnError;
	#endregion

	void Awake()
	{
		isFree = true;
	}

	// Update is called once per frame
	void Update () {
		if(www!=null && isFree==false)
		{
			float pro=www.progress;
			if(OnProcess!=null)
				OnProcess(this,pro);
		}
	}


	#region public method

	public void BeginLoad(CRequest req)
	{
		isFree=false;
		this._req = req;
//		Debug.Log("BeginLoad : url "+req.url+" \n key:"+req.key);
		StopCoroutine (Loadres ());
		StartCoroutine(Loadres());
	}

	#endregion


	#region protected method

	IEnumerator Loadres()
	{
//		Debug.Log("StartCoroutine Loadres : url "+req.url+" \n key:"+req.key);
		if(req.head is WWWForm)
		{
			www=new WWW(req.url,(WWWForm)req.head);
		}else if(req.head is byte[])
		{
			www = new WWW(req.url, (byte[])req.head);
		}
		else
		{
			www=new WWW(req.url); //WWW.LoadFromCacheOrDownload(req.url,1);  
		}
//		Debug.Log("StartCoroutine www : url "+req.url+" \n key:"+req.key);
		yield return www;

		if(www.error!=null)
		{
//			Debug.LogWarning("error : url "+req.url+" \n error:"+www.error);
			isFree =true;
			DispatchErrorEvent(req);
		}
		else
		{
			if(OnProcess!=null)
				OnProcess(this,1);
            //Debug.Log("will complete : url " + req.url + " \n key:" + req.key);
			try
			{

                req.www = www;
                req.assetBundle = www.assetBundle;
                IList<CRequest> depens = null;
                //if (string.IsNullOrEmpty(req.assetName)) req.assetName = req.key;
                if(m_AssetBundleManifest!=null)
                {
                    string[] deps = m_AssetBundleManifest.GetAllDependencies(req.assetBundleName);
                    //foreach (var n in deps)
                    //{
                    //    Debug.Log(n);
                    //}
                    depens = GetDependencies(deps);
                }else if (m_AssetBundleManifest == null)
                {
                    //Debug.LogWarning("Please initialize AssetBundleManifest");
                }
				isFree =true;
				this.DispatchCompleteEvent(this.req,depens);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
		}
	}

	/// <summary>
	/// get Dependencies req
	/// </summary>
	/// <returns>The dependencies.</returns>
	/// <param name="script">Script.</param>
    private IList<CRequest> GetDependencies(string[] paths)
	{

        if (paths==null && paths.Length==0) return null;
        
		IList<CRequest> reqs=new List<CRequest>();
		CRequest item;
		int priority=this.req.priority+10;

        foreach (string p in paths)
		{
            item = new CRequest(CUtils.GetAssetFullPath(RemapVariantName(p)));
			item.isShared=true;
            item.priority = priority;
			reqs.Add(item);
		}

		if (reqs.Count > 0)
						return reqs;
				else
						return null;
	}


	private void DispatchCompleteEvent(CRequest cReq,IList<CRequest> dependencies)
	{
		if(OnComplete!=null)
			OnComplete(this,cReq,dependencies);
	}
	
	
	private void DispatchErrorEvent(CRequest cReq)
	{
		if (OnError != null) {
			OnError (this, cReq);
				}
	}

	#endregion

//	public delegate void OnErrorHandle(CTransport transport,CRequest req);

    static string[] m_Variants = { };
    // Variants which is used to define the active variants.
    public static string[] Variants
    {
        get { return m_Variants; }
        set { m_Variants = value; }
    }
    // Remaps the asset bundle name to the best fitting asset bundle variant.
    public static string RemapVariantName(string assetBundleName)
    {
        string[] bundlesWithVariant = m_AssetBundleManifest.GetAllDependencies(assetBundleName); //.GetAllAssetBundlesWithVariant();

        // If the asset bundle doesn't have variant, simply return.
        if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
            return assetBundleName;

        string[] split = assetBundleName.Split('.');

        int bestFit = int.MaxValue;
        int bestFitIndex = -1;
        // Loop all the assetBundles with variant to find the best fit variant assetBundle.
        for (int i = 0; i < bundlesWithVariant.Length; i++)
        {
            string[] curSplit = bundlesWithVariant[i].Split('.');
            if (curSplit[0] != split[0])
                continue;

            int found = System.Array.IndexOf(m_Variants, curSplit[1]);
            if (found != -1 && found < bestFit)
            {
                bestFit = found;
                bestFitIndex = i;
            }
        }

        if (bestFitIndex != -1)
            return bundlesWithVariant[bestFitIndex];
        else
            return assetBundleName;
    }
}
