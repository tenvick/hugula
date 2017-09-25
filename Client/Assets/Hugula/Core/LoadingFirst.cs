using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Update;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hugula;
using UnityEngine.UI;
public class LoadingFirst : MonoBehaviour
{

    public string enterLua = "main";

    public Text tips;
    // Use this for initialization
    IEnumerator Start()
    {
        CUtils.DebugCastTime("LoadingFirst.Start");
        //load manifest
        ResourcesLoader.Initialize();
        CUtils.DebugCastTime("LoadingFirst.Initialize");
        LogSys();
        LoadFirstHelper.LoadFileManifest();
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


public static class LoadFirstHelper
{
    public static List<string> specialAssetbundles = new List<string>();

    public static void LoadFileManifest()
    {
#if UNITY_EDITOR
        Debug.LogFormat("<color=green>SimulateAssetBundleInEditor {0} mode </color> <color=#8cacbc> change( menu AssetBundles/Simulation Mode)</color>", ManifestManager.SimulateAssetBundleInEditor ? "simulate" : "assetbundle");
        // if (ManifestManager.SimulateAssetBundleInEditor)
        // {
        //     return;
        // }
#endif
        ManifestManager.LoadFileManifest(null);
        ManifestManager.LoadUpdateFileManifest(null);
        CUtils.DebugCastTime("LoadingFirst.LoadFileManifest");
    }

    //开始加载场景
    public static void BeginLoadScene(string enterLua)
    {
        ResourcesLoader.OnAssetBundleComplete = OnSharedComplete;
         PLua.enterLua = enterLua;
        if (PLua.instance != null)
        {
            Debug.Log("lua begin dofile :" + enterLua + ".lua");
        }
    }

    static void OnSharedComplete(CRequest req, AssetBundle ab) // repaire IOS crash bug when scene assetbundle denpendency sprite atlas assetbundle
    {
#if HUGULA_ASSETBUNDLE_LOG || UNITY_EDITOR
        //AddUpLogInfo(string.Format("{0}\t{1}\t{2}", req.key, System.DateTime.Now.ToString(), Time.frameCount));
#endif
        if (ab && req.isShared && !specialAssetbundles.Contains(req.key))
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

}