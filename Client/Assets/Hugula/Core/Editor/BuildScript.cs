// Copyright (c) 2014 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hugula;
using Hugula.Cryptograph;
using Hugula.Pool;
using Hugula.Update;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;


namespace Hugula.Editor
{
    public class BuildScript
    {

        #region 配置变量
        public const string streamingPath = "Assets/StreamingAssets"; //打包assetbundle输出目录。
        public const string TmpPath = "Tmp/";
        public const string HugulaFolder = "HugulaFolder";
#if UNITY_STANDALONE_WIN
        public const BuildAssetBundleOptions optionsDefault = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression; //
#else
        public const BuildAssetBundleOptions optionsDefault = BuildAssetBundleOptions.DeterministicAssetBundle; //
#endif      

#if UNITY_IPHONE
    public const BuildTarget target = BuildTarget.iOS;
#elif UNITY_ANDROID
    public const BuildTarget target = BuildTarget.Android;
#elif UNITY_WP8
	public const BuildTarget target=BuildTarget.WP8Player;
#elif UNITY_METRO
    public const BuildTarget target = BuildTarget.MetroPlayer;
#elif UNITY_STANDALONE_OSX
     public const BuildTarget target = BuildTarget.StandaloneOSXIntel;
#else
        public const BuildTarget target = BuildTarget.StandaloneWindows;
#endif

#if HUGULA_COMMON_ASSETBUNDLE
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
            Dictionary<string, object[]> firstCrcDict = new Dictionary<string, object[]>();
            HashSet<string> manualFileList = new HashSet<string>();
            Dictionary<string, object[]> currCrcDict = new Dictionary<string, object[]>();
            Dictionary<string, object[]> diffCrcDict = new Dictionary<string, object[]>();

            #region 读取首包
            bool firstExists = SplitPackage.ReadFirst(firstCrcDict, manualFileList);
            #endregion

            SplitPackage.DeleteSplitPackageResFolder();

            #region 生成校验列表
            SplitPackage.UpdateOutPath = null;SplitPackage.UpdateOutDevelopPath = null;
            AssetDatabase.Refresh();
            StringBuilder[] sbs = SplitPackage.CreateCrcListContent(allBundles, firstCrcDict, currCrcDict, diffCrcDict, manualFileList);
            uint streaming_crc = SplitPackage.CreateStreamingCrcList(sbs[0]); //本地列表
            // System.Threading.Thread.Sleep(1000);
            uint diff_crc = SplitPackage.CreateStreamingCrcList(sbs[1], firstExists,true); //增量列表
            // System.Threading.Thread.Sleep(1000);
            CUtils.DebugCastTime("Time CreateStreamingCrcList End");
            #endregion

            #region 生成版本号
            //生成版本号码
            SplitPackage.CreateVersionAssetBundle(diff_crc);
            SplitPackage.CreateDevelopVersionAssetBundle(diff_crc);
            CUtils.DebugCastTime("Time CreateVersionAssetBundle End");
            #endregion

            #region copy更新文件导出

            SplitPackage.CopyVersionToSplitFolder(diff_crc);

            SplitPackage.CopyChangeFileToSplitFolder(firstExists, firstCrcDict, currCrcDict, diffCrcDict, manualFileList);

            CUtils.DebugCastTime("Time CopyChangeFileToSplitFolder End");

            Debug.LogFormat("streaming_crc={0},diff_crc{1}", streaming_crc, diff_crc);

            Debug.LogFormat(" firstCrcDict={0},currCrcDict={1},diffCrcDict={2},manualFileList={3}", firstCrcDict.Count, currCrcDict.Count, diffCrcDict.Count, manualFileList.Count);

            #endregion

            #region 删除手动加载文件
#if (UNITY_ANDROID || UNITY_IOS) //&& !UNITY_EDITOR
            SplitPackage.DeleteStreamingFiles (manualFileList);
#endif
            #endregion

            EditorUtility.ClearProgressBar();

        }

        public static void GenerateAssetBundlesMd5Mapping(string[] allAssets)
        {
            string info = "Generate AssetBundles Md5Mapping ";
            EditorUtility.DisplayProgressBar("GenerateAssetBundlesMd5Mapping", info, 0);
            AssetImporter
            import = null;
            float i = 0;
            float allLen = allAssets.Length;
            string name = "";
            string nameMd5 = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("return {");
            foreach (string path in allAssets)
            {
                import = AssetImporter.GetAtPath(path);
                if (
                    import != null && string.IsNullOrEmpty(
                        import.assetBundleName) == false)
                {
                    Object s = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                    name = s.name.ToLower();

                    string line = string.Empty;
                    if (string.IsNullOrEmpty(
                            import.assetBundleVariant))
                        line = "[\"" +
                        import.assetBundleName + "\"] = { name = \"" + name + "\", path = \"" + path + "\"},";
                    else
                        line = "[\"" +
                        import.assetBundleName + "." +
                        import.assetBundleVariant + "\"] = { name = \"" + name + "\", path = \"" + path + "\"},";

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

            string[] spceil = new string[] { CUtils.platform, Common.CONFIG_CSV_NAME, Common.CRC32_FILELIST_NAME, Common.CRC32_VER_FILENAME };
            foreach (string p in spceil)
            {
                name = GetAssetBundleName(p);
                nameMd5 = CUtils.GetRightFileName(name);
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

            AssetImporter
            import = null;
            foreach (Object s in selection)
            {
                import = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s));
                import.assetBundleName = null;
                if (s is GameObject)
                {
                    GameObject tar = s as GameObject;
                    ReferenceCount refe = tar.GetComponent<ReferenceCount>(); //LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                    Object.DestroyImmediate(refe, true);
                    EditorUtility.SetDirty(s);
                }
                Debug.Log(s.name + " clear");
            }
        }

        public static void DeleteAssetBundlesName()
        {
            Object[] selection = Selection.objects;

            string assetBundleName = "";
            List<string> del = new List<string>();

            foreach (Object s in selection)
            {
                string abPath = AssetDatabase.GetAssetPath(s);
                string folder = GetLabelsByPath(abPath);
                string name = CUtils.GetRightFileName(s.name.ToLower());

                if (string.IsNullOrEmpty(folder))
                    assetBundleName = name + "." + Common.ASSETBUNDLE_SUFFIX;
                else
                    assetBundleName = string.Format("{0}/{1}.{2}", folder, name, Common.ASSETBUNDLE_SUFFIX);

                if (s.name.Contains(" ")) Debug.LogWarning(s.name + " contains space");
                Debug.Log("delete : " + s.name + " md5 = " + assetBundleName);
                del.Add(assetBundleName);
            }

            SplitPackage.DeleteStreamingFiles(del); //删除选中对象的ab
        }

        public static void SetAssetBundlesName(bool replace = false)
        {
            Object[] selection = Selection.objects;

            foreach (Object s in selection)
            {
                SetAssetBundleName(s, replace);
            }
        }

        /// <summary>
        /// 设置变体
        /// </summary>
        public static void SetAssetBundlesVariantsAndName()
        {
            Object[] selection = Selection.objects;
            string apath = null;
            foreach (Object s in selection)
            {
                apath = AssetDatabase.GetAssetPath(s);
                if (Directory.Exists(apath))
                {
                    string[] myFolderNames;
                    AssetImporter
                    import = AssetImporter.GetAtPath(apath);
                    myFolderNames = s.name.ToLower().Split('-');
                    if (myFolderNames.Length == 2)
                    {
                        string folder = GetLabelsByPath(apath);
                        string name = CUtils.GetRightFileName(myFolderNames[0]);

                        if (string.IsNullOrEmpty(folder))
                            import.assetBundleName = name;
                        else
                            import.assetBundleName = string.Format("{0}/{1}", folder, name);

                        import.assetBundleVariant = myFolderNames[1];
                        EditorUtility.SetDirty(s);
                    }
                    else
                    {
                        Debug.LogErrorFormat("{0} folder name must like floderName-xx (xx is variant name)", apath);
                    }
                }
                else
                {
                    SetAssetBundleName(s, true, true);
                }

            }
        }

        /// <summary>
        /// 设置assetbundleName
        /// </summary>
        /// <param name="s"></param>
        public static void SetAssetBundleName(Object s, bool replace = false, bool variant = false)
        {
            string abPath = AssetDatabase.GetAssetPath(s);
            AssetImporter
            import = AssetImporter.GetAtPath(abPath);
            string folder = GetLabelsByPath(abPath);
            string objName = s.name.ToLower();
            if (replace)
            {
                objName = HugulaEditorSetting.instance.GetAssetBundleNameByReplaceIgnore(objName);
                // Debug.LogFormat ("{0} replace {1}", s.name, objName);
            }

            bool isScene = abPath.EndsWith(".unity");
            string name = CUtils.GetRightFileName(objName);

            var suffix = Common.CHECK_ASSETBUNDLE_SUFFIX;
            if (variant) // variant
            {
                suffix = "";
            }

            if (string.IsNullOrEmpty(folder))
                import.assetBundleName = name + suffix;
            else
                import.assetBundleName = string.Format("{0}/{1}{2}", folder, name, suffix);

            if (variant)
            {
                DirectoryInfo filePath = new DirectoryInfo(abPath);
                var m_variant = filePath.Parent.Name.ToLower();
                import.assetBundleVariant = m_variant;
                HugulaSetting.instance.AddVariant(m_variant);
                if (m_variant.Equals(Common.ASSETBUNDLE_SUFFIX))
                    Debug.LogWarningFormat("{0} variant folder name Equals {1}", abPath, Common.ASSETBUNDLE_SUFFIX);
                if (m_variant == Common.DOT_BYTES.Replace(".", ""))
                    Debug.LogWarningFormat("{0} variant folder name Equals {1}", abPath, Common.DOT_BYTES);
            }

            if (s.name.Contains(" ")) Debug.LogWarning(s.name + " contains space");

            Debug.Log(import.assetBundleName + ",variant=" + import.assetBundleVariant);

            if (s is GameObject)
            {
                GameObject tar = s as GameObject;
                if (tar.transform.parent == null)
                {
                    ReferenceCount refe = LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                    if (refe != null)
                    {
                        refe.assetbundle =
                            import.assetBundleName;
                        EditorUtility.SetDirty(s);
                    }
                }
            }
            else if (isScene) //如果是场景需要添加引用计数脚本
            { //UnityEngine.SceneAsset
                var sce = s; // as SceneAsset;
                Debug.Log(sce);
                AssetDatabase.OpenAsset(sce);
                GameObject gobj = GameObject.Find(sce.name);
                if (gobj == null) gobj = new GameObject(sce.name);
                ReferenceCount refe = LuaHelper.AddComponent(gobj, typeof(ReferenceCount)) as ReferenceCount;
                if (refe != null)
                {
                    refe.assetbundle = import.assetBundleName;
                    EditorUtility.SetDirty(sce);
                }

                var refers = GameObject.FindObjectsOfType<ReferenceCount>();
                foreach (var rf in refers)
                {
                    if (rf != refe)
                    {
                        Debug.LogWarningFormat("you should not add ReferenceCount in {0}", GetGameObjectPathInScene(rf.transform, string.Empty));
                    }
                }
            }

        }

        public static void ChangeAssetsToBytesAsset(string[] allAssets)
        {
            foreach (var p in allAssets)
            {
                byte[] luabytes = File.ReadAllBytes(p);
                var bytesAsset = ScriptableObject.CreateInstance<BytesAsset>();
                bytesAsset.bytes = luabytes;
                string assetPath = p.Replace(".bytes", ".asset").Replace(".txt", ".asset");
                AssetDatabase.CreateAsset(bytesAsset, assetPath);
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
            AssetImporter
            import = null;
            float i = 0;
            float allLen = allAssets.Length;
            string name = "";
            string cacheAbName = "";
            StringBuilder tipsInfo = new StringBuilder();
            foreach (string path in allAssets)
            {
                import = AssetImporter.GetAtPath(path);
                if (
                    import != null && string.IsNullOrEmpty(
                        import.assetBundleName) == false)
                {
                    string folder = GetLabelsByPath(path);
                    Object s = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                    name = s.name.ToLower();
                    if (string.IsNullOrEmpty(
                            import.assetBundleVariant))
                    {
                        name = CUtils.GetRightFileName(name);

                        if (string.IsNullOrEmpty(folder))
                            import.assetBundleName = name + "." + Common.ASSETBUNDLE_SUFFIX;
                        else
                            import.assetBundleName = string.Format("{0}/{1}.{2}", folder, name, Common.ASSETBUNDLE_SUFFIX);

                        cacheAbName =
                            import.assetBundleName;
                    }
                    else
                    {
                        string[] names = name.Split('-');
                        name = names[0];
                        if (string.IsNullOrEmpty(folder))
                            import.assetBundleName = CUtils.GetRightFileName(name);
                        else
                            import.assetBundleName = string.Format("{0}/{1}", folder, CUtils.GetRightFileName(name));

                        import.assetBundleVariant = names[1];

                        cacheAbName =
                            import.assetBundleName + "." +
                            import.assetBundleVariant;
                    }

                    tipsInfo.AppendLine(cacheAbName + " path = " + path);

                    if (s is GameObject)
                    {
                        GameObject tar = s as GameObject;
                        ReferenceCount refe = LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                        if (refe != null)
                        {
                            refe.assetbundle = cacheAbName;
                            // refe.assetHashCode = LuaHelper.StringToHash(cacheAbName);
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
            Debug.Log(tipsInfo.ToString() + " Complete!");
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
                    AssetImporter
                    import = AssetImporter.GetAtPath(apath);
                    import.assetBundleName = null;
                    //import.assetBundleVariant = null;
                    import.userData = HugulaFolder;
                    AssetDatabase.SetLabels(s, new string[] { HugulaFolder });
                    import.SaveAndReimport();
                    if (!HugulaExtensionFolderEditor.ContainsExtendsPath(apath))
                    {
                        Debug.LogFormat("add extends path = {0}", s.name);
                        HugulaExtensionFolderEditor.AddExtendsPath(apath);
                    }
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>
        /// 添加为扩展文件
        /// </summary>
        public static void ExtensionFiles(bool delete = false)
        {
            Object[] selection = Selection.objects;
            string apath = null;
            string suffix = "";
            string abName = "";
            foreach (Object s in selection)
            {
                apath = AssetDatabase.GetAssetPath(s);
                AssetImporter
                import = AssetImporter.GetAtPath(apath);
                if (!string.IsNullOrEmpty(import.assetBundleVariant))
                    suffix = import.assetBundleVariant;
                else
                    suffix = "";

                if (string.IsNullOrEmpty(suffix))
                {
                    abName = import.assetBundleName;
                }
                else
                {
                    abName = import.assetBundleName = "." + suffix;
                }

                if (delete)
                    HugulaExtensionFolderEditor.RemoveExtendsFile(abName);
                else
                    HugulaExtensionFolderEditor.AddExtendsFile(abName);

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
                    AssetImporter
                    import = AssetImporter.GetAtPath(apath);
                    import.userData = null;
                    import.assetBundleName = null;
                    //import.assetBundleVariant = null;
                    AssetDatabase.SetLabels(s, null);
                    import.SaveAndReimport();
                    //apath = apath.Replace("\\","/");
                    if (HugulaExtensionFolderEditor.ContainsExtendsPath(apath))
                    {
                        Debug.LogFormat("{0},Clear AssetLabels,path ={1}", s.name, apath);
                        HugulaExtensionFolderEditor.RemoveExtendsPath(apath);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        #endregion

        #endregion

        public static void BuildAssetBundles()
        {
            CUtils.DebugCastTime("Time HandleUpdateMaterail End");

            CheckstreamingAssetsPath();
            CUtils.DebugCastTime("Time CheckstreamingAssetsPath End");
            BuildPipeline.BuildAssetBundles(GetOutPutPath(), optionsDefault, target);
            CUtils.DebugCastTime("Time BuildPipeline.BuildAssetBundles End");
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
            bab[0].assetBundleName = abName; //打包的资源包名称 随便命名
            bab[0].assetNames = assets;
            if (string.IsNullOrEmpty(outPath))
                outPath = GetOutPutPath();

            string tmpPath = BuildScript.GetProjectTempPath();
            ExportResources.CheckDirectory(tmpPath);
            string tmpFileName = Path.Combine(tmpPath, abName);
            BuildPipeline.BuildAssetBundles(tmpPath, bab, bbo, target);

            string targetFileName = Path.Combine(outPath, abName);
            FileInfo tInfo = new FileInfo(targetFileName);
            if (tInfo.Exists) tInfo.Delete();
            FileInfo fino = new FileInfo(tmpFileName);
            fino.CopyTo(targetFileName);
        }

        /// <summary>
        /// 自动构建ab
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="abName"></param>
        /// <param name="bbo"></param>
        static public void BuildABs(AssetBundleBuild[] bab, string outPath, BuildAssetBundleOptions bbo)
        {
            if (string.IsNullOrEmpty(outPath))
                outPath = GetOutPutPath();

            string tmpPath = BuildScript.GetProjectTempPath();
            ExportResources.CheckDirectory(tmpPath);

            BuildPipeline.BuildAssetBundles(tmpPath, bab, bbo, target);

            foreach (AssetBundleBuild abb in bab)
            {
                string abName = abb.assetBundleName;
                string tmpFileName = Path.Combine(tmpPath, abName);
                string targetFileName = Path.Combine(outPath, abName);
                FileInfo tInfo = new FileInfo(targetFileName);
                if (tInfo.Exists) tInfo.Delete();
                FileInfo fino = new FileInfo(tmpFileName);
                fino.CopyTo(targetFileName);
            }

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

        public static string GetLuaBytesResourcesPath()
        {
            string luapath = Path.Combine(Application.dataPath, Common.LUACFOLDER);
            luapath = Path.Combine(luapath, "Resources");
            return luapath;
        }

        public static string GetOutPutPath()
        {
            return Path.Combine(streamingPath, CUtils.GetAssetPath(""));
        }

        public static string GetProjectTempPath()
        {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), "Temp/hugula");
        }

        public static string GetAssetTmpPath()
        {
            return Path.Combine(Application.dataPath, TmpPath);
        }

        static public BuildTarget GetTarget()
        {
            return target;
        }

        static public string GetGameObjectPathInScene(Transform obj, string path)
        {
            if (obj.parent == null)
            {
                if (string.IsNullOrEmpty(path)) path = obj.name;
                return path; //  +/+path
            }
            else
            {
                string re = string.Format("{0}/{1}", obj.parent.name, obj.name);
                return GetGameObjectPathInScene(obj.transform.parent, re);
            }
        }

        static public string GetAssetBundleName(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            int idxEnd = url.IndexOf('?');
            int idxBegin = url.IndexOf(CUtils.platformFloder); // 

            if (idxBegin == -1)
            {
                idxBegin = url.LastIndexOf("/") + 1;
                int idx1 = url.LastIndexOf("\\") + 1;
                if (idx1 > idxBegin) idxBegin = idx1;
            }
            else
                idxBegin = idxBegin + CUtils.platformFloder.Length + 1;

            if (idxBegin >= url.Length) idxBegin = 0;
            if (idxEnd == -1) idxEnd = url.Length;

            string re = url.Substring(idxBegin, idxEnd - idxBegin);
            return re;
        }

        static public string GetLabelsByPath(string abPath)
        {
            return HugulaExtensionFolderEditor.GetLabelsByPath(abPath);
        }

        #endregion
    }
}