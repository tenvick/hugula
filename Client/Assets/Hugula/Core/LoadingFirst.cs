using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;

public class LoadingFirst : MonoBehaviour {
	
	public string sceneName = "begin";
	public string sceneAssetBundleName = "begin.u3d";
	// Use this for initialization
	void Start () {
		//load manifest
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
		LResLoader.instance.LoadReq(req);
	}
	public static void BeginLoadScene()
	{

		var req = LRequestPool.Get();
		req.relativeUrl = CUtils.GetRightFileName(sceneAssetBundleName);
		req.assetName = sceneName;
		req.OnComplete = OnSceneAbLoaded;
		req.OnEnd = OnSceneAbError;
		req.assetType = CacheManager.Typeof_ABScene;
		LResLoader.instance.LoadReq(req);
	}

	static void OnSceneAbLoaded(CRequest req)
	{

	}

	static void OnSceneAbError(CRequest req)
	{
		BeginLoadScene();
	}

}