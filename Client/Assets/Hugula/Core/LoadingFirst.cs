using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Update;
using System.IO;

public class LoadingFirst : MonoBehaviour
{

    public string sceneName = "begin";
    public string sceneAssetBundleName = "begin.u3d";

    // Use this for initialization
    void Start()
    {
        CUtils.DebugCastTime("LoadingFirst.Start");
        //load manifest
        ResourcesLoader.Initialize();
        Hugula.Localization.language = PlayerPrefs.GetString("Language", Application.systemLanguage.ToString());
        LoadFirstHelper.LoadManifest(sceneAssetBundleName, sceneName);
    }

}


public static class LoadFirstHelper
{
    public static string sceneAssetBundleName;
    public static string sceneName;

    public static void LoadManifest(string sceneAbName, string scenename)
    {
        Hugula.PLua.DestoryLua();
        // Hugula.PLua.PreInitLua();//初始化slua
        sceneAssetBundleName = sceneAbName;
        sceneName = scenename;
#if UNITY_EDITOR
        Debug.LogFormat("<color=green>SimulateAssetBundleInEditor {0} mode </color> <color=#8cacbc> change( menu AssetBundles/Simulation Mode)</color>", ManifestManager.SimulateAssetBundleInEditor ? "simulate" : "assetbundle");
        if (ManifestManager.SimulateAssetBundleInEditor)
        {
            BeginLoadScene();
            return;
        }
#endif
        LoadFileManifest();
    }

    public static void LoadFileManifest()
    {
        ManifestManager.LoadFileManifest(null);
        ManifestManager.LoadUpdateFileManifest(BeginLoadScene);
    }

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

    static void OnSharedComplete(CRequest req,AssetBundle ab) // repaire IOS crash bug when scene assetbundle denpendency sprite atlas assetbundle
    {
        if (ab && req.isShared) ab.LoadAllAssets();
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

    public static void ReOpen(float sconds)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Debug.LogFormat("ReOpen !");
    }

}