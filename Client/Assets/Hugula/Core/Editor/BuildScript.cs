// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

using Hugula;
using Hugula.Utils;
using Hugula.Update;
using Hugula.Cryptograph;
using Hugula.Pool;

public class BuildScript
{

    #region 配置变量
    public const string streamingPath = "Assets/StreamingAssets";//打包assetbundle输出目录。
    public const string TmpPath = "Tmp/";

    public const string fileCrc32ListName = "crc32_file";
    public const string fileCrc32Ver = "crc32_ver.u3d";
    public const BuildAssetBundleOptions optionsDefault = BuildAssetBundleOptions.DeterministicAssetBundle; //
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

#if BUILD_COMMON_ASSETBUNDLE
	public const bool isMd5 = false;
#else
    public const bool isMd5 = true;
#endif

    #region assetbundle

    public static void GenerateAssetBundlesUpdateFile(string[] allBundles)
    {
        string title = "Generate Update File ";
        string info = "Compute crc32";
        EditorUtility.DisplayProgressBar(title, info, 0.1f);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("return {");

        var selected = string.Empty;
        float i = 0;
        float allLen = allBundles.Length;

        //忽略列表
        Dictionary<string, bool> ignore = new Dictionary<string, bool>();
        ignore.Add("m_" + CUtils.GetFileName(fileCrc32ListName), true);
        ignore.Add("m_" + CUtils.GetFileName(CUtils.GetKeyURLFileName(fileCrc32Ver)), true);

        foreach (var str in allBundles)
        {
            string url = Path.Combine(CUtils.GetRealStreamingAssetsPath(), str);
            if (!File.Exists(url))
            {
                Debug.LogError("could not find :" + url + " " + str);
                continue;
            }
            string key = "m_" + CUtils.GetKeyURLFileName(str);
            if (!ignore.ContainsKey(key))
            {
                var bytes = File.ReadAllBytes(url);
                uint crc = Crc32.Compute(bytes);
                sb.AppendLine(key + " = " + crc + ",");
            }
            EditorUtility.DisplayProgressBar(title, info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
            i++;
        }
        sb.AppendLine("}");
        Debug.Log(sb.ToString());
        //输出到临时目录
        string tmpPath = Path.Combine(Application.dataPath, TmpPath);
        ExportResources.CheckDirectory(tmpPath);
        string assetPath = "Assets/" + TmpPath + fileCrc32ListName + ".txt";
        EditorUtility.DisplayProgressBar("Generate file list", "write file to " + assetPath, 0.99f);

        string outPath = Path.Combine(tmpPath, fileCrc32ListName + ".txt");
        Debug.Log("write to path=" + outPath);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(sb.ToString());
        }
        //
        //打包到streaming path
        AssetDatabase.Refresh();
        string crc32filename = CUtils.GetFileName(fileCrc32ListName + ".u3d");
        BuildScript.BuildABs(new string[] { assetPath }, null, crc32filename, BuildAssetBundleOptions.DeterministicAssetBundle);
        string topath = Path.Combine(GetOutPutPath(), crc32filename);
        Debug.Log(info + " assetbunle build complate! " + topath);

        //生成版本号码
        string crc32Path = "file://" + Path.Combine(CUtils.GetRealStreamingAssetsPath(), CUtils.GetFileName(fileCrc32ListName + ".u3d")); //CUtils.GetAssetFullPath (fileCrc32ListName+".u3d"); 
        WWW loaderVer = new WWW(crc32Path);
        if (!string.IsNullOrEmpty(loaderVer.error))
        {
            Debug.LogError(loaderVer.error);
            return;
        }
        uint crcVer = Crc32.Compute(loaderVer.bytes);
        loaderVer.Dispose();

        tmpPath = CUtils.GetRealStreamingAssetsPath();//Path.Combine (Application.streamingAssetsPath, CUtils.GetAssetPath(""));
        outPath = Path.Combine(tmpPath, CUtils.GetFileName(fileCrc32Ver));
        Debug.Log("verion to path=" + outPath);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(crcVer.ToString());
        }

        Debug.Log(info + " Complete! ver=" + crcVer.ToString() + " path " + outPath);

        BuildScript.BuildAssetBundles();

        EditorUtility.ClearProgressBar();

    }

    public static void GenerateAssetBundlesMd5Mapping(string[] allAssets)
    {
        string info = "Generate AssetBundles Md5Mapping ";
        EditorUtility.DisplayProgressBar("GenerateAssetBundlesMd5Mapping", info, 0);
        AssetImporter import = null;
        float i = 0;
        float allLen = allAssets.Length;
        string name = "";
        string nameMd5 = "";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[");
        foreach (string path in allAssets)
        {
            import = AssetImporter.GetAtPath(path);
            if (import != null && string.IsNullOrEmpty(import.assetBundleName) == false)
            {
                Object s = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                name = s.name.ToLower();
                nameMd5 = CryptographHelper.Md5String(name);
                //				Debug.LogFormat("path({0}),nameMd5({1}),name({2})", path, nameMd5, name);
                string line = "'m_" + nameMd5 + "'={'name':'" + name + "','md5':'" + nameMd5 + "','path':'" + path + "' },";
                sb.AppendLine(line);
                if (name.Contains(" ")) Debug.LogWarning(name + " contains space");
            }
            else
            {
                //Debug.LogWarning(path + " import not exist");
            }
            EditorUtility.DisplayProgressBar("Generate AssetBundles Md5Mapping", info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);

            i++;
        }

        string[] spceil = new string[] { Common.LUA_ASSETBUNDLE_FILENAME, "iOS", "Android", "StandaloneWindows", Common.CONFIG_CSV_NAME, fileCrc32ListName, fileCrc32Ver };
        foreach (string p in spceil)
        {
            name = CUtils.GetKeyURLFileName(p);
            nameMd5 = CryptographHelper.Md5String(name);
            string line = "'m_" + nameMd5 + "'={'name':'" + name + "','md5':'" + nameMd5 + "','path':'" + p + "' },";
            sb.AppendLine(line);
        }

        sb.AppendLine("{}]");
        string tmpPath = Path.Combine(Application.dataPath, TmpPath);
        ExportResources.CheckDirectory(tmpPath);
        EditorUtility.DisplayProgressBar("Generate AssetBundles Md5Mapping", "write file to Assets/" + TmpPath + "Md5Mapping.txt", 0.99f);

        string outPath = Path.Combine(tmpPath, "Md5Mapping.txt");
        Debug.Log("write to path=" + outPath);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(sb.ToString());
        }

        EditorUtility.ClearProgressBar();
        Debug.Log(info + " Complete! Assets/" + TmpPath + "Md5Mapping.txt");
    }

    public static void ClearUnUsedAssetBundlesName()
    {

    }

    public static void ClearAssetBundlesName()
    {
        Object[] selection = Selection.objects;

        AssetImporter import = null;
        foreach (Object s in selection)
        {
            import = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s));
            import.assetBundleName = null;
            Debug.Log(s.name + " clear");
            if (s is GameObject)
            {
                GameObject tar = s as GameObject;
                ReferenceCount refe = LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                if (refe != null)
                {
                    refe.assetBundleName = string.Empty;
                    EditorUtility.SetDirty(s);
                }
            }
        }
    }

    public static void SetAssetBundlesName()
    {
        Object[] selection = Selection.objects;

        AssetImporter import = null;
        foreach (Object s in selection)
        {
            import = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s));
            string md5Name = s.name.ToLower();
            if (isMd5)
            {
                md5Name = CryptographHelper.Md5String(s.name.ToLower());
            }

            import.assetBundleName = md5Name + "." + Common.ASSETBUNDLE_SUFFIX;
            if (s.name.Contains(" ")) Debug.LogWarning(s.name + " contains space");
            Debug.Log(s.name);
            if (s is GameObject)
            {
                GameObject tar = s as GameObject;
                ReferenceCount refe = LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                if (refe != null)
                {
                    refe.assetBundleName = md5Name;
                    EditorUtility.SetDirty(s);
                }
            }

        }
    }

    /// <summary>
    /// Updates the name of the asset bundles.
    /// </summary>
    /// <param name="allAssets">All assets.</param>
    public static void UpdateAssetBundlesName(string[] allAssets)
    {
        string info = string.Format("Update AssetBundles Name To {0}", isMd5 == true ? "Md5" : " Prefab.Name");
        EditorUtility.DisplayProgressBar("UpdateAssetBundlesName", info, 0);
        AssetImporter import = null;
        float i = 0;
        float allLen = allAssets.Length;
        string name = "";
        string nameMd5 = "";
        foreach (string path in allAssets)
        {
            import = AssetImporter.GetAtPath(path);
            if (import != null && string.IsNullOrEmpty(import.assetBundleName) == false)
            {
                Object s = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                name = s.name.ToLower();
                if (isMd5)
                    nameMd5 = CryptographHelper.Md5String(name);
                else
                    nameMd5 = name;

                Debug.LogFormat("path({0}),assetBundleName({1}),name({2})", path, import.assetBundleName, name);

                import.assetBundleName = nameMd5 + "." + Common.ASSETBUNDLE_SUFFIX;

                if (s is GameObject)
                {
                    GameObject tar = s as GameObject;
                    ReferenceCount refe = LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                    if (refe != null)
                    {
                        refe.assetBundleName = nameMd5;
                        EditorUtility.SetDirty(s);
                    }
                }

                if (name.Contains(" ")) Debug.LogWarning(name + " contains space");
            }
            else
            {
                //Debug.LogWarning(path + " import not exist");
            }
            EditorUtility.DisplayProgressBar("UpdateAssetBundlesName", info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);

            i++;
        }

        EditorUtility.ClearProgressBar();
        Debug.Log(info + " Complete!");
    }

    #endregion





    #endregion

    public static void BuildAssetBundles()
    {
        CheckstreamingAssetsPath();

        BuildPipeline.BuildAssetBundles(GetOutPutPath(), optionsDefault, target);
    }

    /// <summary>
    /// 自动构建ab
    /// </summary>
    /// <param name="assets"></param>
    /// <param name="outPath"></param>
    /// <param name="abName"></param>
    /// <param name="bbo"></param>
    static public void BuildABs(string[] assets, string outPath, string abName, BuildAssetBundleOptions bbo)
    {
        AssetBundleBuild[] bab = new AssetBundleBuild[1];
        bab[0].assetBundleName = abName;//打包的资源包名称 随便命名
        bab[0].assetNames = assets;
        if (string.IsNullOrEmpty(outPath))
            outPath = GetOutPutPath();
        BuildPipeline.BuildAssetBundles(outPath, bab, bbo, target);
    }

    #region

    /// <summary>
    /// 检查输出目标
    /// </summary>
    public static void CheckstreamingAssetsPath()
    {
        string dircAssert = GetFileStreamingOutAssetsPath();
        if (!Directory.Exists(dircAssert))
        {
            Directory.CreateDirectory(dircAssert);
        }

        Debug.Log(string.Format("current BuildTarget ={0},path = {1} ", target, dircAssert));
    }

    public static string GetFileStreamingOutAssetsPath()
    {
        string dircAssert = Path.Combine(Application.streamingAssetsPath, CUtils.GetAssetPath(""));
        return dircAssert;
    }

    public static string GetOutPutPath()
    {
        return Path.Combine(streamingPath, CUtils.GetAssetPath(""));
    }

    static public BuildTarget GetTarget()
    {
        return target;
    }
    #endregion
}
