using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Update;
using System.Collections;
using System.IO;
using System.Text;
using Hugula;

public class LoadingFirst : MonoBehaviour
{

    public string sceneName = "begin";
    public string sceneAssetBundleName = "begin.u3d";

    // Use this for initialization
    IEnumerator Start()
    {
        CUtils.DebugCastTime("LoadingFirst.Start");
        //load manifest
        ResourcesLoader.Initialize();
        CUtils.DebugCastTime("LoadingFirst.Initialize");
        LogSys();
        LoadFirstHelper.LoadFileManifest();
        yield return new WaitForSeconds(0.16f);
        Hugula.Localization.language = PlayerPrefs.GetString("Language", Application.systemLanguage.ToString());
        yield return new WaitForLanguageHasBeenSet();
        LoadFirstHelper.SetScene(sceneAssetBundleName, sceneName);
        if (ManifestManager.CheckNeedUncompressStreamingAssets())
        {
            // todo
        }
        
        LoadFirstHelper.BeginLoadScene();

    }

    void LogSys()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("platform:{0}\r\n udid:", Application.platform.ToString());
        sb.Append(SystemInfo.deviceUniqueIdentifier);
        sb.AppendFormat("\r\n deviceName={0};\r\n date:", SystemInfo.deviceName);
        sb.Append(System.DateTime.Now.ToString());
        sb.AppendFormat("\r\n systemMemorySize={0};\r\n bundleIdentifier:", SystemInfo.systemMemorySize);
        sb.Append(Application.bundleIdentifier);
        sb.AppendFormat("\r\n internetReachability={0};\r\n deviceModel:", Application.internetReachability);
        sb.Append(SystemInfo.deviceModel);
        // TLogger.LogSys(sb.ToString());
    }

    void OnDecomper()
    {
        ManifestManager.CompleteUncompressStreamingAssets();
        LoadFirstHelper.BeginLoadScene();
    }

}


public static class LoadFirstHelper
{
    public static string sceneAssetBundleName;
    public static string sceneName;

    public static void SetScene(string sceneAbName, string scenename)
    {
        Hugula.PLua.DestoryLua();
        // Hugula.PLua.PreInitLua();//初始化slua
        sceneAssetBundleName = sceneAbName;
        sceneName = scenename;
    }

    public static void LoadFileManifest()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=green>SimulateAssetBundleInEditor {0} mode </color> <color=#8cacbc> change( menu AssetBundles/Simulation Mode)</color>", ManifestManager.SimulateAssetBundleInEditor ? "simulate" : "assetbundle");
        if (ManifestManager.SimulateAssetBundleInEditor)
        {
            return;
        }
#endif
        ManifestManager.LoadFileManifest(null);
        ManifestManager.LoadUpdateFileManifest(null);
        CUtils.DebugCastTime("LoadingFirst.LoadFileManifest");
    }

    //开始加载场景
    public static void BeginLoadScene()
    {
        ResourcesLoader.OnAssetBundleComplete = OnSharedComplete;
        CUtils.DebugCastTime("LoadingFirst.BeginLoadScene");
        var req = CRequest.Get();
        req.relativeUrl = CUtils.GetRightFileName(sceneAssetBundleName);
        req.assetName = sceneName;
        req.OnComplete = OnSceneAbLoaded;
        req.OnEnd = OnSceneAbError;
        req.assetType = CacheManager.Typeof_ABScene;
        CacheManager.Unload(req.keyHashCode);
        ResourcesLoader.LoadAsset(req);
    }

    static void OnSharedComplete(CRequest req, AssetBundle ab) // repaire IOS crash bug when scene assetbundle denpendency sprite atlas assetbundle
    {
        if (ab && req.isShared && Application.platform == RuntimePlatform.IPhonePlayer) ab.LoadAllAssets();
    }

    static void OnSceneAbLoaded(CRequest req)
    {
        // LResLoader.instance.OnSharedComplete -= OnSharedComplete;
#if HUGULA_LOADER_DEBUG
		Debug.LogFormat("OnSceneAbLoaded {0} is done !",req.url);
#endif
        CUtils.DebugCastTime("On " + sceneName + "Loaded");
    }

    static void OnSceneAbError(CRequest req)
    {
#if UNITY_EDITOR
        Debug.LogFormat("OnSceneAbLoaded {0} is Fail !", req.url);
#endif
        // BeginLoadScene();
    }

    //重启动游戏
    public static void ReOpen(float sconds)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Debug.LogFormat("ReOpen !");
    }

}