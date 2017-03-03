using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;

public class LoadingFirst : MonoBehaviour {
	
	public string sceneName = "begin";
	public string sceneAssetBundleName = "begin.u3d";
	// Use this for initialization
	void Start () {
		//load manifest
		CUtils.DebugCastTime("LoadingFirst");
		LoadFirstHelper.LoadManifest(sceneAssetBundleName,sceneName);
	}

}


public class LoadFirstHelper
{
	public static string sceneAssetBundleName;
	public static string sceneName;
	public static void LoadManifest(string sceneAbName,string scenename)
	{
		sceneAssetBundleName = sceneAbName;
		sceneName = scenename;
		var  url = CUtils.GetPlatformFolderForAssetBundles();
		var req = LRequestPool.Get();
		req.relativeUrl = CUtils.GetRightFileName(url);
		req.assetType = typeof(AssetBundleManifest);
		req.assetName = "assetbundlemanifest";
		req.OnComplete = (CRequest req1)=>
		{
			LResLoader.assetBundleManifest=req1.data as AssetBundleManifest;
			BeginLoadScene();
		};
		req.OnEnd = (CRequest req1)=>{BeginLoadScene();};
		req.async = true;
		req.isAssetBundle = true;
		LResLoader.instance.OnSharedComplete+=OnSharedComplete;
		LResLoader.instance.LoadReq(req);
	}
	public static void BeginLoadScene()
	{
		CUtils.DebugCastTime("LoadingFirst");
		var req = LRequestPool.Get();
		req.relativeUrl = CUtils.GetRightFileName(sceneAssetBundleName);
		req.assetName = sceneName;
		req.OnComplete = OnSceneAbLoaded;
		req.OnEnd = OnSceneAbError;
		req.assetType = CacheManager.Typeof_ABScene;
		req.async = true;
		LResLoader.instance.LoadReq(req);
	}

	static void OnSharedComplete(CRequest req) // repaire IOS crash bug when scene assetbundle denpendency sprite atlas assetbundle
	{
		AssetBundle ab = req.data as AssetBundle;
		if(ab)ab.LoadAllAssets();
	}

	static void OnSceneAbLoaded(CRequest req)
	{
		LResLoader.instance.OnSharedComplete-=OnSharedComplete;
		CUtils.DebugCastTime("On "+ sceneName +"Loaded");
	}

	static void OnSceneAbError(CRequest req)
	{
		BeginLoadScene();
	}

}