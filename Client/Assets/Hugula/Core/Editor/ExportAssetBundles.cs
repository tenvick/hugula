// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
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

    #region menuitem

    #region selected
    [MenuItem("Assets/", false, 1)]
    static void Breaker() { }

    [MenuItem("Assets/AssetBundle Build", false, 5)]
    static void ExportAssetBundle()
    {
        ExportGourpAssetBundleDependenGameObjectNotNGUI(target);
    }

    [MenuItem("Assets/NGUI AssetBundle UI Build", false, 2)]
    static void ExportSelectedUIGroup()
    {
        ExportGourpAssetBundleDependenGameObject(target);
    }

    [MenuItem("Assets/NGUI [folder]  AssetBundle UI Build", false, 4)]
    static void ExportselectedUIFolder()
    {
        ExportFolderDependencies(target);
    }

    [MenuItem("Assets/Texture AssetBundle Build (share .texture)", false, 3)]
    static void ExportselectedAssetBundle()
    {
        ExportGourpAssetBundle(target);
    }

    #endregion

    #endregion

    #region protected

    /// <summary>·
    /// 导出NGUI资源
    /// </summary>
    /// <param name="path"></param>
    public static void ExportNGUIAssetBundle(string Path)
    {
        checkstreamingAssetsPath();
        BuildPipeline.PushAssetDependencies();
        ExportSingleAssetGameObjectDependenciesCorssReference(Path, optionsDefault, target);
        BuildPipeline.PopAssetDependencies();
    }

    static void ExportGourpAssetBundleDependenGameObjectNotNGUI(BuildTarget btarget)
    {

        checkstreamingAssetsPath();

        string path = "";

        Object[] selection = Selection.objects;
        string name;
        foreach (Object s in selection)
        {
            //if (s is GameObject)
            //{
            path = AssetDatabase.GetAssetPath(s);
            //ExportSingleAssetGameObjectDependenciesCorssReference(Path, optionsDefault, btarget);

            Debug.Log("path :" + path);
            name = s.name + "." + suffix;
            name = name.Replace(" ", "_");//
            string tarName = getOutPutPath(btarget) + "/" + name;
            //File.Delete(tarName); AssetDatabase.LoadMainAssetAtPath(path)
            BuildPipeline.BuildAssetBundle(s, null, tarName, optionsDependency, btarget);

#if UNITY_EDITOR
            Debug.Log(" Export :" + s.name);
#endif
            // System.Threading.Thread.Sleep(100);
            //}
        }

#if UNITY_5
        EditorUtility.UnloadUnusedAssetsImmediate();
#else
        EditorUtility.UnloadUnusedAssets();
#endif
    }

    static void ExportGourpAssetBundleDependenGameObject(BuildTarget btarget)
    {

        checkstreamingAssetsPath();

        string Path = "";

        BuildPipeline.PushAssetDependencies();
        Object[] selection = Selection.objects;

        foreach (Object s in selection)
        {
            if (s is GameObject)
            {
                Path = AssetDatabase.GetAssetPath(s);
                ExportSingleAssetGameObjectDependenciesCorssReference(Path, optionsDefault, btarget);
                //System.Threading.Thread.Sleep(100);
            }
        }

        BuildPipeline.PopAssetDependencies();
#if UNITY_5
        EditorUtility.UnloadUnusedAssetsImmediate();
#else
        EditorUtility.UnloadUnusedAssets();
#endif
    }

    static void ExportFolderDependencies(BuildTarget btarget)
    {

        checkstreamingAssetsPath();

        Object obj = Selection.activeObject;
        if (obj == null)
        {
            Debug.LogException(new System.Exception("必须选择一个文件夹！"), obj);
        }

        string path = AssetDatabase.GetAssetPath(obj).Replace("Assets", "");
        List<string> files = getAllChildFiles(Application.dataPath + path);// Directory.GetFiles(Application.dataPath + path);

#if UNITY_EDITOR
        Debug.Log("floder Asset Bundles:" + path + "files=" + files.Count);
#endif
        IList<string> childrens = new List<string>();

        foreach (string file in files)
        {
            if (file.EndsWith("prefab"))
            {
                childrens.Add(getAssetPath(file));
            }
        }

        BuildPipeline.PushAssetDependencies();

        foreach (string filePath in childrens)
        {
            ExportSingleAssetGameObjectDependenciesCorssReference(filePath, optionsDefault, btarget);
        }

        BuildPipeline.PopAssetDependencies();
#if UNITY_5
        EditorUtility.UnloadUnusedAssetsImmediate();
#else
        EditorUtility.UnloadUnusedAssets();
#endif
    }

    static void ExportGourpAssetBundle(BuildTarget btarget)
    {

        checkstreamingAssetsPath();

        string Path = "";

        BuildPipeline.PushAssetDependencies();
        Object[] selection = Selection.objects;

        foreach (Object s in selection)
        {
            if (s is GameObject)
            {
                Path = AssetDatabase.GetAssetPath(s);
                ExportSingleAssetTextureDependenciesCorssReference(Path, optionsDefault, btarget);
                //System.Threading.Thread.Sleep(100);
            }
        }

        BuildPipeline.PopAssetDependencies();

#if UNITY_5
        EditorUtility.UnloadUnusedAssetsImmediate();
#else
        EditorUtility.UnloadUnusedAssets();
#endif
    }

    static void checkstreamingAssetsPath()
    {
        string dircAssert = string.Format("{0}/{1}", Application.streamingAssetsPath, target);
        // Application.streamingAssetsPath+"/AssetBundles/"+target.ToString();
        if (!Directory.Exists(dircAssert))
        {
            Directory.CreateDirectory(dircAssert);
        }

        Debug.Log(string.Format("current BuildTarget ={0}", target));
    }

    static void ExportSingleAssetTextureDependenciesCorssReference(string path, BuildAssetBundleOptions options, BuildTarget buildTarget)
    {
        Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        string mainPath = path;
        string name = "";// obj.name;
        Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { obj });
        bool haveTexture = false, haveGameObject = false;
        BuildPipeline.PushAssetDependencies();
        string paths = "", split = "";
        foreach (Object obj1 in denpendencies)
        {
            if (obj1 is UnityEngine.Texture)//|| obj1 is UnityEditor.MonoScript
            {
                path = AssetDatabase.GetAssetPath(obj1);
                name = obj1.name + "_.texture";
                name = name.Replace(" ", "_");
                BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(path), null, getOutPutPath(buildTarget) + "/" + name, optionsDependency, buildTarget);//| | BuildAssetBundleOptions.UncompressedAssetBundle
#if UNITY_EDITOR
                Debug.Log("dependencies Texture:" + obj1.name + ",type=" + obj1.GetType() + "rename " + obj1.name);
#endif
                paths += split + "AssetBundles/" + buildTarget.ToString() + "/" + name;
                split = ",";
                haveTexture = true;
            }
        }

        if (haveTexture) BuildPipeline.PushAssetDependencies();
        foreach (Object obj1 in denpendencies)
        {
            if (obj1 is UnityEngine.GameObject)
            {
                path = AssetDatabase.GetAssetPath(obj1);
                if (path != mainPath)
                {
                    //Debug.Log("path :"+path);
                    haveGameObject = true;
                    name = obj1.name + "." + suffix;
                    name = name.Replace(" ", "_");
                    BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(path), null, getOutPutPath(buildTarget) + "/" + name, options, buildTarget);

                    paths += split + "AssetBundles/" + buildTarget.ToString() + "/" + name;
                    split = ",";
#if UNITY_EDITOR
                    Debug.Log("dependencies GameObject:" + obj1.name + ",type=" + obj1.GetType() + "rename " + obj1.name);
#endif
                }
            }
        }

        name = "/" + obj.name + "." + suffix;

        if (haveGameObject) BuildPipeline.PushAssetDependencies();
        if (paths != "")
        {
            GameObject game = new GameObject();
            CDependencies denp = game.AddComponent<CDependencies>();
            denp.paths = paths;
            GameObject c_dependen = PrefabUtility.CreatePrefab("Assets/CObjDependencies.prefab", game);

            BuildPipeline.BuildAssetBundle(obj, new Object[] { c_dependen }, getOutPutPath(buildTarget) + name, options, buildTarget);
#if UNITY_EDITOR
            Debug.Log(name + " has export Contains :" + paths);
#endif
            GameObject.DestroyImmediate(game);
        }
        else
        {
            BuildPipeline.BuildAssetBundle(obj, null, getOutPutPath(buildTarget) + name, options, buildTarget);
            Debug.Log(name + " has export");

        }

        if (haveGameObject) BuildPipeline.PopAssetDependencies();
        if (haveTexture) BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();

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


    static void ExportSingleAssetGameObjectDependenciesCorssReference(string path, BuildAssetBundleOptions options, BuildTarget buildTarget)
    {
        Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        string mainPath = path;
        string name = "";// obj.name;
        Object[] denpendencies = EditorUtility.CollectDependencies(new Object[] { obj });
        bool haveGameObject = false;
        BuildPipeline.PushAssetDependencies();

        string paths = "", split = "";
        foreach (Object obj1 in denpendencies)
        {
            if (obj1 is UnityEngine.GameObject || obj1 is UnityEngine.Font)
            {
                path = AssetDatabase.GetAssetPath(obj1);
                if (path != mainPath)
                {
                    //						Debug.Log("path :"+path);
                    name = obj1.name + "." + suffix;
                    name = name.Replace(" ", "_");//
                    string tarName = getOutPutPath(buildTarget) + "/" + name;
                    BuildPipeline.BuildAssetBundle(obj1, null, tarName, optionsDependency, buildTarget);
                    paths += split + buildTarget.ToString() + "/" + name;
                    split = ",";
                    haveGameObject = true;
#if UNITY_EDITOR
                    Debug.Log("dependencies :" + obj1.name + ",type=" + obj1.GetType() + "," + path);
#endif
                    AssetDatabase.Refresh();
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        name = "/" + obj.name + "." + suffix;

        if (haveGameObject) BuildPipeline.PushAssetDependencies();
        if (paths != "")
        {
            //#if UNITY_5
            CDependenciesScript denpend = ScriptableObject.CreateInstance<CDependenciesScript>();
            //denpend.name = Common.DEPENDENCIES_OBJECT_NAME;
            denpend.paths = paths;
            string pathAsset = "Assets/" + Common.DEPENDENCIES_OBJECT_NAME + ".asset";
            AssetDatabase.CreateAsset(denpend, pathAsset);
            Object o = AssetDatabase.LoadAssetAtPath(pathAsset, typeof(CDependenciesScript));
            o.name = Common.DEPENDENCIES_OBJECT_NAME;
            BuildPipeline.BuildAssetBundle(obj, new Object[] { o }, getOutPutPath(buildTarget) + name, options, buildTarget);
            //#else
            //                            GameObject game=new GameObject();
            //                            game.name = Common.DEPENDENCIES_OBJECT_NAME;
            //                            CDependencies denp=game.AddComponent<CDependencies>();
            //                            denp.paths=paths;
            //                            GameObject c_dependen = PrefabUtility.CreatePrefab("Assets/" + Common.DEPENDENCIES_OBJECT_NAME + ".prefab", game);
            //                            BuildPipeline.BuildAssetBundle(obj ,new Object[]{c_dependen}, getOutPutPath(buildTarget) + name, options,buildTarget);
            //                            GameObject.DestroyImmediate(game);

            //#endif
#if UNITY_EDITOR
            Debug.Log(name + " has export Contains :" + paths);
#endif
        }
        else
        {
            BuildPipeline.BuildAssetBundle(obj, null, getOutPutPath(buildTarget) + name, options, buildTarget);
            Debug.Log(name + " has export");
        }

        if (haveGameObject) BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();

    }

    #endregion

    #region public
   
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
