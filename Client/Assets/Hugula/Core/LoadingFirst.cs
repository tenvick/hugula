using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Update;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hugula;
// using UnityEngine.UI;
public class LoadingFirst : MonoBehaviour
{

    public string enterLua = "main";
    public string sceneName = "s_begin";
	public string sceneAssetBundleName = "s_begin.u3d";

    // public Text tips;
    // Use this for initialization
    IEnumerator Start()
    {
        CUtils.DebugCastTime("LoadingFirst.Start");
        //load manifest
        ResourcesLoader.Initialize();
        ResourcesLoader.RegisterOverrideBaseAssetbundleURL(LoadFirstHelper.OverrideBaseAssetbundleURL);
        HttpDnsHelper.Initialize();
        CUtils.DebugCastTime("LoadingFirst.Initialize");
        LogSys();
        LoadFirstHelper.LoadFileManifest(sceneAssetBundleName,sceneName);
        yield return null;
#if  UNITY_ANDROID //&& !UNITY_EDITOR
        if (ManifestManager.fileManifest == null)
        {
            //obb丢失
        }
#endif
        Hugula.Localization.language = PlayerPrefs.GetString("Language", Application.systemLanguage.ToString());
        yield return new WaitForLanguageHasBeenSet();

        if (ManifestManager.CheckNeedUncompressStreamingAssets())
        {
            // todo
        }
        
        LoadFirstHelper.BeginLoadScene(enterLua);

    }

    void LogSys()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("platform:{0}\r\n udid:", Application.platform.ToString());
        sb.Append(SystemInfo.deviceUniqueIdentifier);
        sb.AppendFormat("\r\n deviceName={0};\r\n date:", SystemInfo.deviceName);
        sb.Append(System.DateTime.Now.ToString());
        sb.AppendFormat("\r\n systemMemorySize={0};\r\n bundleIdentifier:", SystemInfo.systemMemorySize);
        sb.Append(CUtils.bundleIdentifier);
        sb.AppendFormat("\r\n internetReachability={0};\r\n deviceModel:", Application.internetReachability);
        sb.Append(SystemInfo.deviceModel);
        sb.AppendFormat("\r\n version={0};\r\n unityVersion:", Application.version);
        sb.Append(Application.unityVersion);
        Debug.Log(sb.ToString());
    }

    void OnDecomper()
    {
        ManifestManager.CompleteUncompressStreamingAssets();
        LoadFirstHelper.BeginLoadScene(enterLua);
    }

}

[SLua.CustomLuaClass]
public static class LoadFirstHelper
{
    static string sceneAssetBundleName;
	static string sceneName;

    public static List<string> specialAssetbundles = new List<string>();

    internal static void LoadFileManifest(string sceneAbName,string scenename)
    {
        sceneAssetBundleName = sceneAbName;
		sceneName = scenename;
#if UNITY_EDITOR
        Debug.LogFormat("<color=green>SimulateAssetBundleInEditor {0} mode </color> <color=#8cacbc> change( menu AssetBundles/Simulation Mode)</color>", ManifestManager.SimulateAssetBundleInEditor ? "simulate" : "assetbundle");
        // if (ManifestManager.SimulateAssetBundleInEditor)
        // {
        //     return;
        // }
#endif
        ManifestManager.LoadFileManifest(null);

        CUtils.DebugCastTime("LoadingFirst.LoadFileManifest");
    }

    //开始加载场景
    internal static void BeginLoadScene(string beginLua)
    {

		ManifestManager.LoadUpdateFileManifest(null);
        PLua.DestoryLua();
		PLua.enterLua = beginLua;
        CUtils.DebugCastTime("LoadingFirst");
		var req = CRequest.Get();
		req.vUrl = CUtils.GetRightFileName(sceneAssetBundleName);
		req.assetName = sceneName;
		req.OnComplete = OnSceneAbLoaded;
		req.OnEnd = OnSceneAbError;
		req.assetType = LoaderType.Typeof_ABScene;
		req.async = true;
		ResourcesLoader.LoadAsset(req);
        ResourcesLoader.OnAssetBundleComplete = OnSharedComplete;
    }

    static void OnSceneAbLoaded(CRequest req)
	{
		#if HUGULA_LOADER_DEBUG 
		Debug.LogFormat("OnSceneAbLoaded {0} is done !",req.url);
		#endif
		CUtils.DebugCastTime("On "+ sceneName +"Loaded");
	}

	static void OnSceneAbError(CRequest req)
	{
		#if UNITY_EDITOR 
		Debug.LogFormat("OnSceneAbLoaded {0} is Fail !",req.url);
		#endif
	}

    static void OnSharedComplete(CRequest req, AssetBundle ab) // repaire IOS crash bug when scene assetbundle denpendency sprite atlas assetbundle
    {
#if HUGULA_ASSETBUNDLE_LOG || UNITY_EDITOR
        //AddUpLogInfo(string.Format("{0}\t{1}\t{2}", req.key, System.DateTime.Now.ToString(), Time.frameCount));
#endif
        if (req.isShared && ab && !specialAssetbundles.Contains(req.key))
        {
            ab.LoadAllAssets();
        }   
    }

    //重启动游戏
    public static void ReOpen(float sconds)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Debug.LogFormat("ReOpen !");
    }

    public static string OverrideBaseAssetbundleURL(string abName)
    {
        string path = null;
        bool isupdate = ManifestManager.CheckIsUpdateFile(abName);
        if(isupdate)     
        {
        //    if(ManifestManager.CheckReqCrc(abName))//crc check
           path = CUtils.PathCombine(CUtils.GetRealPersistentDataPath (), abName);
        }

        return path;
    }

}