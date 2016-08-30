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
    public const string HugulaFolder = "HugulaFolder";
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

        #region 读取首包
        CrcCheck.Clear();
        bool firstExists = false;
        DirectoryInfo firstDir = new DirectoryInfo(Application.dataPath);
        string firstPath = Path.Combine(firstDir.Parent.Parent.FullName, Common.FirstOutPath);
        string readPath = Path.Combine(firstPath, CUtils.GetAssetPath(""));
        readPath = Path.Combine(readPath, CUtils.GetFileName(Common.CRC32_FILELIST_NAME));
        Debug.Log(readPath);

        WWW abload = new WWW("file://" + readPath);
        if (string.IsNullOrEmpty(abload.error) && abload.assetBundle != null)
        {
            var ab = abload.assetBundle;
            TextAsset ta = ab.LoadAllAssets<TextAsset>()[0];
            //ta.text
            Debug.Log(ta);
            string context = ta.text;
            string[] split = context.Split('\n');
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\[""(\w+)""\]\s+=\s+(\d+)");
            float j = 1;
            float l = split.Length;
            foreach (var line in split)
            {
                System.Text.RegularExpressions.Match match = regex.Match(line);
                if (match.Success)
                {
                    //Debug.Log(match.Groups[1].Value + " " + match.Groups[2].Value);
                    CrcCheck.Add(match.Groups[1].Value, System.Convert.ToUInt32(match.Groups[2].Value));
                }
                //Debug.Log(line);
                EditorUtility.DisplayProgressBar(title, "read first crc => " + j.ToString() + "/" + l.ToString(), j / l);
                j++;
            }
            ab.Unload(true);
            firstExists = true;
        }
        else
        {
            Debug.LogWarning(abload.error + "no frist packeage in " + readPath);
        }
        abload.Dispose();
        #endregion

        #region 生成校验列表
        Dictionary<string, uint> updateList = new Dictionary<string, uint>();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("return {");

        var selected = string.Empty;
        float i = 0;
        float allLen = allBundles.Length;

        //忽略列表
        Dictionary<string, bool> ignore = new Dictionary<string, bool>();
        ignore.Add(CUtils.GetFileName(CUtils.GetKeyURLFileName(Common.CRC32_FILELIST_NAME)), true);
        ignore.Add(CUtils.GetFileName(CUtils.GetKeyURLFileName(Common.CRC32_VER_FILENAME)), true);

        foreach (var str in allBundles)
        {
            string url = Path.Combine(CUtils.GetRealStreamingAssetsPath(), str);
            uint outCrc = 0;
            string key = CUtils.GetKeyURLFileName(str);
            if (!ignore.ContainsKey(key) && CrcCheck.CheckLocalFileWeakCrc(url, out outCrc) == false) //如果不一致需要更新
            {
                updateList.Add(str, outCrc);//记录导出记录
                sb.AppendLine("[\"" + key + "\"] = " + outCrc + ",");
            }
            EditorUtility.DisplayProgressBar(title, info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
            i++;
        }
        sb.AppendLine("}");
        //Debug.Log (sb.ToString ());
        CrcCheck.Clear();

        //输出到临时目录
        var crc32filename = CUtils.GetKeyURLFileName(Common.CRC32_FILELIST_NAME);
        string tmpPath = Path.Combine(Application.dataPath, TmpPath);
        ExportResources.CheckDirectory(tmpPath);
        string assetPath = "Assets/" + TmpPath + crc32filename + ".txt";
        EditorUtility.DisplayProgressBar("Generate file list", "write file to " + assetPath, 0.99f);

        string outPath = Path.Combine(tmpPath, crc32filename + ".txt");
        Debug.Log("write to path=" + outPath);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(sb.ToString());
        }
        //
        //打包到streaming path
        AssetDatabase.Refresh();
        string crc32outfilename = CUtils.GetFileName(Common.CRC32_FILELIST_NAME); //(fileCrc32ListName + ".u3d");
        BuildScript.BuildABs(new string[] { assetPath }, null, crc32outfilename, BuildAssetBundleOptions.DeterministicAssetBundle);
        string topath = Path.Combine(GetOutPutPath(), crc32outfilename);
        Debug.Log(info + " assetbunle build complate! " + topath);

        #endregion

        #region 生成版本号
        //生成版本号码
        string crc32Path = "file://" + Path.Combine(CUtils.GetRealStreamingAssetsPath(), CUtils.GetFileName(Common.CRC32_FILELIST_NAME)); //CUtils.GetAssetFullPath (fileCrc32ListName+".u3d"); 
        WWW loaderVer = new WWW(crc32Path);
        if (!string.IsNullOrEmpty(loaderVer.error))
        {
            Debug.LogError(loaderVer.error);
            return;
        }
        uint crcVer = Crc32.Compute(loaderVer.bytes);
        loaderVer.Dispose();

        tmpPath = CUtils.GetRealStreamingAssetsPath();//Path.Combine (Application.streamingAssetsPath, CUtils.GetAssetPath(""));
        outPath = Path.Combine(tmpPath, CUtils.GetFileName(Common.CRC32_VER_FILENAME));
        Debug.Log("verion to path=" + outPath);
        //json 化version{ code,crc32,version}
        StringBuilder verJson = new StringBuilder();
        verJson.Append("{");
        verJson.Append(@"""code"":" + CodeVersion.CODE_VERSION + ",");
        verJson.Append(@"""crc32"":" + crcVer.ToString() + ",");
        verJson.Append(@"""time"":" + CUtils.ConvertDateTimeInt(System.DateTime.Now) + "");
        verJson.Append("}");

        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(verJson.ToString());
        }

        Debug.Log(info + " Complete! ver=" + crcVer.ToString() + " path " + outPath);
        BuildScript.BuildAssetBundles();

        #endregion

        #region copy更新文件导出
        if (updateList.Count > 0)
        {
            info = "copy updated file ";
            string updateOutPath = Path.Combine(firstPath, CUtils.GetAssetPath("") + System.DateTime.Now.ToString("_yyyy-MM-dd_hh-mm"));
            DirectoryInfo outDic = new DirectoryInfo(updateOutPath);
            if (outDic.Exists) outDic.Delete();
            outDic.Create();

            if (!firstExists) updateList.Clear(); //如果没有首包，只导出校验文件。

            updateList.Add(CUtils.GetFileName(Common.CRC32_VER_FILENAME), 0);
            updateList.Add(CUtils.GetFileName(Common.CRC32_FILELIST_NAME), crcVer);

            string sourcePath;
            string outfilePath;
            i = 1;
            allLen = updateList.Count;
            string key = "";
            foreach (var k in updateList)
            {
                key = CUtils.GetKeyURLFileName(k.Key);
                sourcePath = Path.Combine(CUtils.GetRealStreamingAssetsPath(), k.Key);
                if (k.Value == 0)
                {
                    key = key + "." + Common.ASSETBUNDLE_SUFFIX;
                }
                else
                {
                    key = key + "_" + k.Value.ToString() + "." + Common.ASSETBUNDLE_SUFFIX;
                }
                outfilePath = Path.Combine(updateOutPath, key);
                File.Copy(sourcePath, outfilePath, true);// source code copy
                EditorUtility.DisplayProgressBar(title, info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
                i++;
            }
            Debug.Log(" copy  file complete!");
        }
        #endregion

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
        sb.AppendLine("return {");
        foreach (string path in allAssets)
        {
            import = AssetImporter.GetAtPath(path);
            if (import != null && string.IsNullOrEmpty(import.assetBundleName) == false)
            {
                Object s = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                name = s.name.ToLower();
                nameMd5 = CryptographHelper.Md5String(name);
                //				Debug.LogFormat("path({0}),nameMd5({1}),name({2})", path, nameMd5, name);
                string line = "[\"" + nameMd5 + "\"] = { name = \"" + name + "\", path = \"" + path + "\"},";
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

        string[] spceil = new string[] { Common.LUA_ASSETBUNDLE_FILENAME, "iOS", "Android", "StandaloneWindows", Common.CONFIG_CSV_NAME, CUtils.GetKeyURLFileName(Common.CRC32_FILELIST_NAME), CUtils.GetKeyURLFileName(Common.CRC32_VER_FILENAME) };
        foreach (string p in spceil)
        {
            name = CUtils.GetKeyURLFileName(p);
            nameMd5 = CryptographHelper.Md5String(name);
            string line = "[\"" + nameMd5 + "\"] ={ name = \"" + name + "\", path = \"" + p + "\" },";
            sb.AppendLine(line);
        }

        sb.AppendLine("}");
        string tmpPath = Path.Combine(Application.dataPath, TmpPath);
        ExportResources.CheckDirectory(tmpPath);
        EditorUtility.DisplayProgressBar("Generate AssetBundles Md5Mapping", "write file to Assets/" + TmpPath + "Md5Mapping.txt", 0.99f);

        string outPath = Path.Combine(tmpPath, "md5mapping.txt");
        Debug.Log("write to path=" + outPath);
        using (StreamWriter sr = new StreamWriter(outPath, false))
        {
            sr.Write(sb.ToString());
        }

        EditorUtility.ClearProgressBar();
        Debug.Log(info + " Complete! Assets/" + TmpPath + "md5mapping.txt");
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

        foreach (Object s in selection)
        {
            SetAssetBundlesName(s);
        }
    }

    public static void SetAssetBundlesVariantsAndName()
    {
        Object[] selection = Selection.objects;
        string apath = null;
        foreach (Object s in selection)
        {
            apath = AssetDatabase.GetAssetPath(s);
            string[] myFolderNames;
            string folder = GetLabelsByPath(apath);
            AssetImporter import = AssetImporter.GetAtPath(apath);
            myFolderNames = s.name.ToLower().Split('-');
            if (myFolderNames.Length == 2)
            {
                string md5Name = myFolderNames[0];
                if (isMd5)
                {
                    md5Name = CryptographHelper.Md5String(md5Name);
                }

                import.assetBundleName = md5Name; //string.Format("{0}/{1}", md5Name, md5Name);
                import.assetBundleVariant = myFolderNames[1];
                EditorUtility.SetDirty(s);
            }
            else
            {
                Debug.LogWarningFormat("{0} file name not like floderName-xx", apath);
            }
        }
    }

    public static void SetAssetBundlesName(Object s)
    {
        string abPath = AssetDatabase.GetAssetPath(s);
        AssetImporter import = AssetImporter.GetAtPath(abPath);
        string folder = GetLabelsByPath(abPath);
        string md5Name = s.name.ToLower();
        if (isMd5)
        {
            md5Name = CryptographHelper.Md5String(s.name.ToLower());
        }
        if(string.IsNullOrEmpty(folder))
            import.assetBundleName = md5Name + "." + Common.ASSETBUNDLE_SUFFIX;
        else
            import.assetBundleName = string.Format("{0}/{1}.{2}",folder,md5Name,Common.ASSETBUNDLE_SUFFIX);

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
        else if(s is SceneAsset) //如果是场景需要添加引用计数脚本
        {//UnityEngine.SceneAsset
            SceneAsset sce = s as SceneAsset;
            Debug.Log(sce);
            AssetDatabase.OpenAsset(sce);
            GameObject gobj = GameObject.Find(sce.name);
            if (gobj == null) gobj = new GameObject(sce.name);
            ReferenceCount refe = LuaHelper.AddComponent(gobj, typeof(ReferenceCount)) as ReferenceCount;
            if (refe != null)
            {
                refe.assetBundleName = md5Name;
                EditorUtility.SetDirty(sce);
            }

           var refers = GameObject.FindObjectsOfType<ReferenceCount>();
           foreach (var rf in refers)
           {
               if (rf != refe)
               {
                   Debug.LogWarningFormat("you should not add ReferenceCount in {0}", GetGameObjectPathInScene(rf.transform,string.Empty));
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

    /// <summary>
    /// 设置为扩展包路径文件夹
    /// </summary>
    public static void SetAsExtendsFloder()
    {
        Object[] selection = Selection.objects;
        string apath = null;
        foreach (Object s in selection)
        {
            if (s is DefaultAsset)
            {
                apath = AssetDatabase.GetAssetPath(s);
                AssetImporter import = AssetImporter.GetAtPath(apath);
                import.assetBundleName = null;
                //import.assetBundleVariant = null;
                import.userData = HugulaFolder;
                AssetDatabase.SetLabels(s, new string[] { HugulaFolder });
                import.SaveAndReimport();
                Debug.LogFormat("{0},AssetLabels={2},path ={1}", s.name, apath, HugulaFolder);
                if (!HugulaSetting.instance.AssetLabels.Contains(apath))
                {
                    HugulaSetting.instance.AssetLabels.Add(apath);
                }
            }
        }
    }

    /// <summary>
    /// 清理扩展文件夹
    /// </summary>
    public static void ClearExtendsFloder()
    {
        Object[] selection = Selection.objects;
        string apath = null;
        foreach (Object s in selection)
        {
            if (s is DefaultAsset)
            {
                apath = AssetDatabase.GetAssetPath(s);
                AssetImporter import = AssetImporter.GetAtPath(apath);
                import.userData = null;
                import.assetBundleName = null;
                //import.assetBundleVariant = null;
                AssetDatabase.SetLabels(s, null);
                import.SaveAndReimport();
                Debug.LogFormat("{0},Clear AssetLabels,path ={1}", s.name, apath);
                if (HugulaSetting.instance.AssetLabels.Contains(apath))
                {
                    HugulaSetting.instance.AssetLabels.Remove(apath);
                }
            }
        }
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

    static public string GetGameObjectPathInScene(Transform obj,string path)
    {
        if (obj.parent == null)
        {
            if (string.IsNullOrEmpty(path)) path = obj.name;
            return path;//  +/+path
        }
        else
        {
            string re = string.Format("{0}/{1}", obj.parent.name, obj.name);
            return GetGameObjectPathInScene(obj.transform.parent, re);
        }
    }

    static public string GetLabelsByPath(string abPath)
    {
        string folder = null; 
        var allLabels = HugulaSetting.instance.AssetLabels;
        foreach (var labelPath in allLabels)
        {
            if (abPath.StartsWith(labelPath))
            {
                folder = CUtils.GetKeyURLFileName(labelPath).ToLower();
            }
        }
        return folder;
    }

    #endregion
}
