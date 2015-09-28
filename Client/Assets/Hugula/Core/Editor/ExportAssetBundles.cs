// Copyright (c) 2014 hugula
// direct https://github.com/tenvick/hugula
//
// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class ExportAssetBundles
{
    public const string outPath = "Assets/StreamingAssets";
    public const string suffix = Common.ASSETBUNDLE_SUFFIX;

    public const BuildAssetBundleOptions optionsDefault = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets; //
    private const BuildAssetBundleOptions optionsDependency = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies;// 
#if UNITY_IPHONE
	public const BuildTarget target=BuildTarget.iOS;
#elif UNITY_ANDROID
    public const BuildTarget target = BuildTarget.Android;
#elif UNITY_WP8
	public const BuildTarget target=BuildTarget.WP8Player;
#elif UNITY_METRO
    public const BuildTarget target = BuildTarget.MetroPlayer;
#elif UNITY_STANDALONE_OSX
     public const BuildTarget target = BuildTarget.StandaloneOSXIntel;
#else
    public const BuildTarget target=BuildTarget.StandaloneWindows;
#endif

    #region protected


    static void checkstreamingAssetsPath()
    {
        string dircAssert = string.Format("{0}/{1}", Application.streamingAssetsPath, target);
        if (!Directory.Exists(dircAssert))
        {
            Directory.CreateDirectory(dircAssert);
        }

        Debug.Log(string.Format("current BuildTarget ={0}", target));
    }


    static string getAssetPath(string fullPath)
    {
        return fullPath.Replace(Application.dataPath, "Assets");
    }

    static List<string> getAllChildFiles(string path, List<string> files = null)
    {
        if (files == null) files = new List<string>();
        addFiles(path, files);
        string[] dires = Directory.GetDirectories(path);
        foreach (string dirp in dires)
        {
            getAllChildFiles(dirp, files);
        }
        return files;
    }

    static void addFiles(string direPath, List<string> files)
    {
        string[] fileMys = Directory.GetFiles(direPath);
        foreach (string f in fileMys)
        {
            if (f.EndsWith("prefab"))
            {
                files.Add(f);
            }
        }
    }

    static string getOutPutPath(BuildTarget buildTarget)
    {
        return outPath + "/" + buildTarget.ToString();
    }


    #endregion

    #region public
   
	static public void BuildABs(string[] assets, string outPath,string abName, BuildAssetBundleOptions bbo)
	{
		AssetBundleBuild[] bab = new AssetBundleBuild[1];
		bab[0].assetBundleName = abName;//打包的资源包名称 随便命名
		bab[0].assetNames = assets;

		BuildPipeline.BuildAssetBundles (outPath, bab, bbo, target);
//		BuildPipeline.BuildAssetBundle(main, assets, pathName, bbo, target);

	}

    static public void BuildAB(Object main, Object[] assets, string pathName, BuildAssetBundleOptions bbo)
    {
        BuildPipeline.BuildAssetBundle(main, assets, pathName, bbo, target);
    }


    static public string GetOutPath()
    {
        return outPath + "/" + target.ToString();
    }

    static public BuildTarget GetTarget()
    {
        return target;
    }
    #endregion

}
