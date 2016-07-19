// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;
using System.Linq;

using Hugula.Utils;
using Hugula.Cryptograph;

public class ExportResources
{

    #region public p

    public const string OutLuaPath = "/Tmp/" + Common.LUACFOLDER + "/";
#if Nlua
#if UNITY_EDITOR_OSX
	    public static string luacPath=Application.dataPath+"/../tools/luaTools/luac";
#else
        public static string luacPath = Application.dataPath + "/../tools/luaTools/win/luac.exe";
#endif
#else
#if UNITY_EDITOR_OSX && UNITY_IPHONE
	public static string luacPath=Application.dataPath+"/../tools/luaTools/lua5.1.4c";
#elif UNITY_EDITOR_OSX 
	public static string luacPath=Application.dataPath+"/../tools/luaTools/luajit";
#else
    public static string luacPath = Application.dataPath + "/../tools/luaTools/win/luajit.exe";
#endif
#endif
    #endregion

    #region

    //[MenuItem("Hugula AES/GenerateKey", false, 12)]
    public static void GenerateKey()
    {
        using (System.Security.Cryptography.RijndaelManaged myRijndael = new System.Security.Cryptography.RijndaelManaged())
        {

            myRijndael.GenerateKey();
            byte[] Key = myRijndael.Key;

            KeyVData KeyScri = ScriptableObject.CreateInstance<KeyVData>();
            KeyScri.KEY = Key;
            AssetDatabase.CreateAsset(KeyScri, "Assets/Config/I81.asset");

            Debug.Log("key Generate " + Key.Length);

        }
    }

    //[MenuItem("Hugula AES/GenerateIV", false, 13)]
    public static void GenerateIV()
    {
        using (System.Security.Cryptography.RijndaelManaged myRijndael = new System.Security.Cryptography.RijndaelManaged())
        {

            myRijndael.GenerateIV();
            byte[] IV = myRijndael.IV;

            KeyVData KeyScri = ScriptableObject.CreateInstance<KeyVData>();
            KeyScri.IV = IV;
            AssetDatabase.CreateAsset(KeyScri, "Assets/Config/K81.asset");
            Debug.Log("IV Generate " + IV.Length);
        }
    }

    static byte[] GetKey()
    {
        KeyVData kv = (KeyVData)AssetDatabase.LoadAssetAtPath("Assets/Config/I81.asset", typeof(KeyVData));
        return kv.KEY;
    }

    static byte[] GetIV()
    {
        KeyVData kv = (KeyVData)AssetDatabase.LoadAssetAtPath("Assets/Config/K81.asset", typeof(KeyVData));
        return kv.IV;
    }

    #endregion

    #region update
    /// <summary>
    /// Builds the asset bundles update A.
    /// </summary>
    public static void buildAssetBundlesUpdateAB()
    {
        EditorUtility.DisplayProgressBar("Generate FileList", "loading bundle manifest", 1 / 2);

        string readPath = BuildScript.GetFileStreamingOutAssetsPath();
        var u3dList = getAllChildFiles(readPath, "u3d");
        Debug.Log(u3dList.Count);
        List<string> assets = new List<string>();
        foreach (var s in u3dList)
        {
            string ab = s.Replace(readPath, "").Replace("/", "").Replace("\\", "");
            //Debug.Log(ab);
            assets.Add(ab);
        }

        EditorUtility.ClearProgressBar();
        BuildScript.GenerateAssetBundlesUpdateFile(assets.ToArray());
    }

    #endregion


    #region export

    public static void exportLua()
    {
        checkLuaExportPath();
        BuildScript.CheckstreamingAssetsPath();

        string info = "luac";
        string title = "build lua";
        EditorUtility.DisplayProgressBar(title, info, 0);

        var childrens = AssetDatabase.GetAllAssetPaths().Where(p =>
            (p.StartsWith("Assets/Lua")
            || p.StartsWith("Assets/Config"))
            && (p.EndsWith(".lua"))
            ).ToArray();
        string path = "Assets/Lua/"; //lua path
        string path1 = "Assets/Config/"; //config path
        string root = Application.dataPath.Replace("Assets", "");

        Debug.Log("luajit path = " + luacPath);
        string crypName = "", fileName = "", outfilePath = "", arg = "";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //refresh directory
        DirectoryDelete(Application.dataPath + OutLuaPath);
        CheckDirectory(Application.dataPath + OutLuaPath);

        float allLen = childrens.Length;
        float i = 0;

        List<string> exportNames = new List<string>();
        foreach (string file in childrens)
        {
            string filePath = Path.Combine(root, file);
            fileName = CUtils.GetURLFileName(filePath);
            crypName = file.Replace(path, "").Replace(path1, "").Replace(".lua", "." + Common.LUA_LC_SUFFIX).Replace("\\", "_").Replace("/", "_");
            outfilePath = Application.dataPath + OutLuaPath + crypName;
            exportNames.Add("Assets" + OutLuaPath + crypName);
            sb.Append(fileName);
            sb.Append("=");
            sb.Append(crypName);
            sb.Append("\n");

#if Nlua || UNITY_IPHONE 
			arg="-o "+outfilePath+" "+filePath;// luac 
			File.Copy(filePath, outfilePath, true);// source code copy
#else
            arg = "-b " + filePath + " " + outfilePath; //for jit
            //Debug.Log(arg);
            //System.Diagnostics.Process.Start(luacPath, arg);//jit 
            File.Copy(filePath, outfilePath, true);// source code copy
#endif
            i++;
            EditorUtility.DisplayProgressBar(title, info + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);

        }
        Debug.Log("lua:" + path + "files=" + childrens.Length + " completed");
        System.Threading.Thread.Sleep(1000);
        AssetDatabase.Refresh();

        EditorUtility.DisplayProgressBar(title, "build lua", 0.99f);
        //u5 打包
        CheckDirectory(Path.Combine(Application.dataPath, OutLuaPath));
        BuildScript.BuildABs(exportNames.ToArray(), "Assets" + OutLuaPath, "luaout.bytes", BuildAssetBundleOptions.DeterministicAssetBundle);

        EditorUtility.DisplayProgressBar(title, "Encrypt lua", 0.99f);
        //Encrypt
        string tarName = Application.dataPath + OutLuaPath + "luaout.bytes";
        string md5Name = CUtils.GetFileName(Common.LUA_ASSETBUNDLE_FILENAME);
        string realOutPath = Path.Combine(BuildScript.GetOutPutPath(), md5Name);

        byte[] by = File.ReadAllBytes(tarName);
        byte[] Encrypt = CryptographHelper.Encrypt(by, GetKey(), GetIV());
        File.WriteAllBytes(realOutPath, Encrypt);
        Debug.Log(realOutPath + " export");
        EditorUtility.ClearProgressBar();
    }

    public static void exportConfig()
    {
        var files = AssetDatabase.GetAllAssetPaths().Where(p =>
         p.StartsWith("Assets/Config")
         && p.EndsWith(".csv")
         ).ToArray();

        BuildScript.CheckstreamingAssetsPath();
        string cname = CUtils.GetFileName(Common.CONFIG_CSV_NAME);
        BuildScript.BuildABs(files.ToArray(), null, cname, BuildAssetBundleOptions.DeterministicAssetBundle);
        Debug.Log(" Config export " + cname);

    }

    public static void exportLanguage()
    {
        string assetPath = "Assets/Lan/";

        var files = AssetDatabase.GetAllAssetPaths().Where(p =>
            p.StartsWith(assetPath)
            && p.EndsWith(".csv")
        ).ToArray();

        BuildScript.CheckstreamingAssetsPath();

        foreach (string abPath in files)
        {
            string name = CUtils.GetURLFileName(abPath);
            string abName = CUtils.GetFileName(name + "." + Common.LANGUAGE_SUFFIX);
            BuildScript.BuildABs(new string[] { abPath }, null, abName, BuildAssetBundleOptions.CompleteAssets);
            Debug.Log(name + " " + abName + " export");
        }
    }


    public static void exportPublish()
    {
        ExportResources.DirectoryDelete(Path.Combine(Application.streamingAssetsPath, CUtils.GetAssetPath("")));

        exportLua();

        exportConfig();
        //
        //       exportLanguage();

		BuildScript.BuildAssetBundles(); //导出资源
        buildAssetBundlesUpdateAB();//更新列表和版本号码
    }

    #endregion

    #region private

    public static void CheckDirectory(string fullPath)
    {
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    private static void checkLuaChildDirectory(string fullpath)
    {
        DirectoryInfo info = Directory.GetParent(fullpath);
        string Dir = info.FullName;
        if (!Directory.Exists(Dir))
        {
            Directory.CreateDirectory(Dir);
        }
    }

    private static void checkLuaExportPath()
    {
        string dircAssert = Application.dataPath + OutLuaPath;
        if (!Directory.Exists(dircAssert))
        {
            Directory.CreateDirectory(dircAssert);
        }
    }

    public static List<string> getAllChildFiles(string path, string suffix = "lua", List<string> files = null)
    {
        if (files == null) files = new List<string>();
        addFiles(path, suffix, files);
        string[] dires = Directory.GetDirectories(path);
        foreach (string dirp in dires)
        {
            //            Debug.Log(dirp);
            getAllChildFiles(dirp, suffix, files);
        }
        return files;
    }

    public static void addFiles(string direPath, string suffix, List<string> files)
    {
        string[] fileMys = Directory.GetFiles(direPath);
        foreach (string f in fileMys)
        {
            if (f.ToLower().EndsWith(suffix.ToLower()))
            {
                files.Add(f);
            }
        }
    }

    public static void DirectoryDelete(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists) di.Delete(true);
    }
    #endregion
}
