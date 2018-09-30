// Copyright (c) 2014 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hugula;
using Hugula.Cryptograph;
using Hugula.Editor.Task;
using Hugula.Pool;
using Hugula.Update;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;

namespace Hugula.Editor {
    public class BuildScript {

        #region 配置变量
        // public const string streamingPath = "Assets/StreamingAssets"; //打包assetbundle输出目录。
        // public const string TmpPath = "Tmp/";
        // public const string HugulaFolder = "HugulaFolder";

#if UNITY_IPHONE
        public const BuildTarget target = BuildTarget.iOS;
#elif UNITY_ANDROID
        public const BuildTarget target = BuildTarget.Android;
#elif UNITY_WP8
        public const BuildTarget target = BuildTarget.WP8Player;
#elif UNITY_METRO
        public const BuildTarget target = BuildTarget.MetroPlayer;
#elif UNITY_STANDALONE_OSX
        public const BuildTarget target = BuildTarget.StandaloneOSXIntel;
#elif UNITY_FACEBOOK
        public const BuildTarget target = BuildTarget.StandaloneWindows;
#else
        public const BuildTarget target = BuildTarget.StandaloneWindows;
#endif

// #if HUGULA_COMMON_ASSETBUNDLE
//         public const bool isMd5 = false;
// #else
        public const bool isMd5 = true;
// #endif

        #region assetbundle

        public static void GenerateAssetBundlesUpdateFile (string[] allBundles) {
            string title = "Generate Update File ";
            string info = "Compute crc32";
            EditorUtility.DisplayProgressBar (title, info, 0.1f);
            var hotResGenSharedData = new HotResGenSharedData ();
            //流程封装方便自己定义流程
            // HugulaPlayerPrefs.SetString(Hugula.Editor.EditorCommon.KeyVerChannels,"imsdk");
            string KeyVerChannels = HugulaPlayerPrefs.GetString (EditorCommon.KeyVerChannels, string.Empty);
            Debug.LogFormat ("GenerateAssetBundlesUpdateFile KeyVerChannels:{0}", KeyVerChannels);
            TaskManager<HotResGenSharedData>.AddTask (new ReadFirst (allBundles));
            TaskManager<HotResGenSharedData>.AddTask (new CreateCrcListContent ());
            TaskManager<HotResGenSharedData>.AddTask (new CreateLocalFileManifest ());
            TaskManager<HotResGenSharedData>.AddTask (new CreateDiffFileManifest ());
            TaskManager<HotResGenSharedData>.AddTask (new CreateVersionAssetBundle (KeyVerChannels));
            TaskManager<HotResGenSharedData>.AddTask (new CopyChangeFiles ());
            TaskManager<HotResGenSharedData>.AddTask (new ClearAssetBundle ());
            TaskManager<HotResGenSharedData>.AddTask (new ZipBundles ());
            System.Action<float, string> act = (float p, string name) => {
                EditorUtility.DisplayProgressBar (title, name, p);
                Debug.Log (" process =" + name);
            };

            TaskManager<HotResGenSharedData>.Run (hotResGenSharedData, act);

            hotResGenSharedData.manualFileList.WriteToFile ("Assets/" + EditorUtils.TmpPath + "manualFileList.txt");
            HugulaPlayerPrefs.DeleteKey (EditorCommon.KeyVerChannels);

            EditorUtility.ClearProgressBar ();

        }

        public static void GenerateAssetBundlesMd5Mapping (string[] allAssets) {
            string info = "Generate AssetBundles Md5Mapping ";
            EditorUtility.DisplayProgressBar ("GenerateAssetBundlesMd5Mapping", info, 0);
            string speciallyPath = "Assets/Config/Lan/";
            string luaPath = "Assets/Lua/";
            AssetImporter import = null;
            float i = 0;
            float allLen = allAssets.Length;
            string name = "";
            string nameMd5 = "";

            //name mapping
            StringBuilder nameSb = new StringBuilder ();

            //asset map
            StringBuilder sb = new StringBuilder ();
            sb.AppendLine ("return {");

            foreach (string path in allAssets) {
                import = AssetImporter.GetAtPath (path);
                string line = string.Empty;

                if (import != null && string.IsNullOrEmpty (import.assetBundleName) == false) {

                    string abName = import.assetBundleName;
                    name = CUtils.GetAssetName (path).ToLower ();

                    if (!string.IsNullOrEmpty (import.assetBundleVariant))
                        abName = import.assetBundleName + "." + import.assetBundleVariant; // line = "{\"" + import.assetBundleName + "\" = { size = \"" + name + "\", path = \"" + path + "\"}},";

                    line = "[\"" + abName + "\"] = { size = " + GetAssetbundleSize (abName) + ", path = \"" + path + "\"},";
                    sb.AppendLine (line);
                    nameSb.AppendFormat ("{0}={1}\r\n", CryptographHelper.Md5String (name), name);
                    if (name.Contains (" ")) Debug.LogWarning (name + " contains space");
                } else if (import != null && path.Contains (speciallyPath)) {
                    name = CUtils.GetAssetName (path).ToLower ();
                    string md5name = CryptographHelper.Md5String (name) + Common.CHECK_ASSETBUNDLE_SUFFIX;
                    line = "[\"" + md5name + "\"] = { size = " + GetAssetbundleSize (md5name) + ", path = \"" + path + "\"},";
                    sb.AppendLine (line);
                    nameSb.AppendFormat ("{0}={1}\r\n", md5name, name);
                } else if (import != null && path.Contains (luaPath)) {
                    string luaname = path.Replace (luaPath, "").Replace ("\\", ".").Replace ("/", ".");
                    string luacname = luaname.Replace (".lua", "").Replace (".", "+");
                    string luaMd5Name = CryptographHelper.Md5String (luacname);

                    line = "[\"" + luaMd5Name + "\"] = { size = " + GetAssetbundleSize (luaMd5Name + ".bytes") + ", path = \"" + path + "\"},";
                    sb.AppendLine (line);
                    nameSb.AppendFormat ("{0}={1}\r\n", luaMd5Name, luaname);
                }
                EditorUtility.DisplayProgressBar ("Generate AssetBundles Md5Mapping", info + "=>" + i.ToString () + "/" + allLen.ToString (), i / allLen);

                i++;
            }

            string[] special = new string[] { CUtils.platform, Common.CONFIG_CSV_NAME, Common.CRC32_FILELIST_NAME, Common.CRC32_VER_FILENAME };
            foreach (string p in special) {
                name = EditorUtils.GetAssetBundleName (p);
                nameMd5 = CUtils.GetRightFileName (name);
                string line = "[\"" + nameMd5 + "\"] ={ size = 0, path = \"" + p + "\" },";
                sb.AppendLine (line);
                nameSb.AppendFormat ("{0}={1}\r\n", CryptographHelper.Md5String (name), name);
            }

            sb.AppendLine ("}");
            string tmpPath = Path.Combine (Application.dataPath, EditorUtils.TmpPath);
            EditorUtils.CheckDirectory (tmpPath);
            EditorUtility.DisplayProgressBar ("Generate AssetBundles Md5Mapping", "write file to Assets/" + EditorUtils.TmpPath + "Md5Mapping.txt", 0.99f);

            string outPath = Path.Combine (tmpPath, "md5_asset_mapping.txt");
            Debug.Log ("write to path=" + outPath);
            using (StreamWriter sr = new StreamWriter (outPath, false)) {
                sr.Write (sb.ToString ());
            }

            outPath = Path.Combine (tmpPath, "md5_name_mapping.txt");
            Debug.Log ("write to path=" + outPath);
            using (StreamWriter sr = new StreamWriter (outPath, false)) {
                sr.Write (nameSb.ToString ());
            }
            EditorUtility.ClearProgressBar ();
            Debug.Log (info + " Complete! Assets/" + EditorUtils.TmpPath + "md5_asset_mapping.txt");
        }

        public static void ClearUnUsedAssetBundlesName () {

        }

        public static void ClearAssetBundlesName () {
            Object[] selection = EditorUtils.SelectObjects ();

            AssetImporter
            import = null;
            foreach (Object s in selection) {
                import = AssetImporter.GetAtPath (AssetDatabase.GetAssetPath (s));
                import.assetBundleName = null;
                if (s is GameObject) {
                    GameObject tar = s as GameObject;
                    ReferenceCount refe = tar.GetComponent<ReferenceCount> (); //LuaHelper.AddComponent(tar, typeof(ReferenceCount)) as ReferenceCount;
                    Object.DestroyImmediate (refe, true);
                    EditorUtility.SetDirty (s);
                }
                Debug.Log (s.name + " clear");
            }
        }

        public static void DeleteAssetBundlesName () {
            string assetBundleName = "";
            List<ABInfo> del = new List<ABInfo> ();
            Object[] selection = Selection.objects; //EditorUtils.SelectObjects();

            foreach (Object s in selection) {
                string abPath = AssetDatabase.GetAssetPath (s);
                string folder = EditorUtils.GetLabelsByPath (abPath);
                string name = CUtils.GetRightFileName (s.name.ToLower ());

                if (string.IsNullOrEmpty (folder))
                    assetBundleName = name + "." + Common.ASSETBUNDLE_SUFFIX;
                else
                    assetBundleName = string.Format ("{0}/{1}.{2}", folder, name, Common.ASSETBUNDLE_SUFFIX);

                if (s.name.Contains (" ")) Debug.LogWarning (s.name + " contains space");
                Debug.Log ("delete : " + s.name + " md5 = " + assetBundleName);
                del.Add (new ABInfo (assetBundleName, 0, 0, 0));
            }

            SplitPackage.DeleteStreamingFiles (del); //删除选中对象的ab
        }

        public static void SetAssetBundlesName (bool replace = false) {
            Object[] selection = Selection.objects;

            foreach (Object s in selection) {
                SetAssetBundleName (s, replace);
            }
        }

        /// <summary>
        /// 设置变体
        /// </summary>
        public static void SetAssetBundlesVariantsAndName () {
            Object[] selection = Selection.objects;
            string apath = null;
            foreach (Object s in selection) {
                apath = AssetDatabase.GetAssetPath (s);
                if (Directory.Exists (apath)) {
                    var myFolderName = s.name.ToLower ();
                    var allFiles = EditorUtils.getAllChildFiles (apath, ".prefab$");
                    string title = "set folder " + myFolderName + " variants";
                    string info = "";
                    Debug.LogWarningFormat ("the folder({0})'s all prefab's variant will set to {1} ", apath, myFolderName);
                    EditorUtility.DisplayProgressBar (title, info, 0);
                    float i = 0;
                    float all = allFiles.Count;
                    foreach (var abPath in allFiles) {
                        info = abPath;
                        var prefab = AssetDatabase.LoadAssetAtPath (abPath, typeof (Object));
                        if (prefab)
                            SetAssetBundleName (prefab, true, true, myFolderName);
                        else
                            Debug.LogWarningFormat ("{0} is not exists", abPath);
                        i = i + 1;
                        EditorUtility.DisplayProgressBar (title, info, i / all);
                    }
                    EditorUtility.ClearProgressBar ();
                } else {
                    SetAssetBundleName (s, true, true);
                }

            }
        }

        /// <summary>
        /// 设置assetbundleName
        /// </summary>
        /// <param name="s"></param>
        public static void SetAssetBundleName (Object s, bool replace = false, bool variant = false, string variantName = null) {
            string abPath = AssetDatabase.GetAssetPath (s);
            AssetImporter
            import = AssetImporter.GetAtPath (abPath);
            string folder = EditorUtils.GetLabelsByPath (abPath);
            string objName = s.name.ToLower ();
            if (replace) {
                objName = HugulaEditorSetting.instance.GetAssetBundleNameByReplaceIgnore (objName);
                // Debug.LogFormat ("{0} replace {1}", s.name, objName);
            }

            bool isScene = abPath.EndsWith (".unity");
            string name = CUtils.GetRightFileName (objName);

            var suffix = Common.CHECK_ASSETBUNDLE_SUFFIX;
            // if (variant) // variant
            // {
            //     suffix = "";
            // }

            if (string.IsNullOrEmpty (folder))
                import.assetBundleName = name + suffix;
            else
                import.assetBundleName = string.Format ("{0}/{1}{2}", folder, name, suffix);

            if (variant) {
                string m_variant;
                if (abPath.IndexOf ("Assets/TapEnjoy/WarX/Effect/Prefabs/high") == 0)
                    m_variant = "high";
                else if (abPath.IndexOf ("Assets/TapEnjoy/WarX/Effect/Prefabs/medium") == 0)
                    m_variant = "medium";
                else if (abPath.IndexOf ("Assets/TapEnjoy/WarX/Effect/Prefabs/low") == 0)
                    m_variant = "low";
                else {
                    DirectoryInfo filePath = new DirectoryInfo (abPath);
                    m_variant = variantName != null ? variantName : filePath.Parent.Name.ToLower ();
                }

                import.assetBundleVariant = m_variant;
                HugulaSetting.instance.AddVariant (m_variant);
                if (m_variant.Equals (Common.ASSETBUNDLE_SUFFIX))
                    Debug.LogWarningFormat ("{0} variant folder name Equals {1}", abPath, Common.ASSETBUNDLE_SUFFIX);
                if (m_variant == Common.DOT_BYTES.Replace (".", ""))
                    Debug.LogWarningFormat ("{0} variant folder name Equals {1}", abPath, Common.DOT_BYTES);
            }

            if (s.name.Contains (" ")) Debug.LogWarning (s.name + " contains space");

            Debug.Log (import.assetBundleName + ",variant=" + import.assetBundleVariant);

            if (s is GameObject) {
                GameObject tar = s as GameObject;
                if (tar.transform.parent == null) {
                    ReferenceCount refe = LuaHelper.AddComponent (tar, typeof (ReferenceCount)) as ReferenceCount;
                    if (refe != null) {
                        if (string.IsNullOrEmpty (import.assetBundleVariant))
                            refe.assetbundle = import.assetBundleName;
                        else
                            refe.assetbundle = import.assetBundleName + "." + import.assetBundleVariant;

                        EditorUtility.SetDirty (s);
                    }
                }
            } else if (isScene) //如果是场景需要添加引用计数脚本
            { //UnityEngine.SceneAsset
                var sce = s; // as SceneAsset;
                Debug.Log (sce);
                AssetDatabase.OpenAsset (sce);
                GameObject gobj = GameObject.Find (sce.name);
                if (gobj == null) gobj = new GameObject (sce.name);
                ReferenceCount refe = LuaHelper.AddComponent (gobj, typeof (ReferenceCount)) as ReferenceCount;
                if (refe != null) {
                    if (string.IsNullOrEmpty (import.assetBundleVariant))
                        refe.assetbundle = import.assetBundleName;
                    else
                        refe.assetbundle = import.assetBundleName + "." + import.assetBundleVariant;

                    EditorUtility.SetDirty (sce);
                }

                var refers = GameObject.FindObjectsOfType<ReferenceCount> ();
                foreach (var rf in refers) {
                    if (rf != refe) {
                        Debug.LogWarningFormat ("you should not add ReferenceCount in {0}", EditorUtils.GetGameObjectPathInScene (rf.transform, string.Empty));
                    }
                }
            }

        }

        public static void ChangeAssetsToBytesAsset (string[] allAssets) {
            foreach (var p in allAssets) {
                byte[] luabytes = File.ReadAllBytes (p);
                var bytesAsset = ScriptableObject.CreateInstance<BytesAsset> ();
                bytesAsset.bytes = luabytes;
                string assetPath = p.Replace (".bytes", ".asset").Replace (".txt", ".asset");
                AssetDatabase.CreateAsset (bytesAsset, assetPath);
            }
        }

        /// <summary>
        /// Updates the name of the asset bundles.
        /// </summary>
        /// <param name="allAssets">All assets.</param>
        public static void UpdateAssetBundlesName (string[] allAssets) {
            string info = string.Format ("Update AssetBundles Name To {0}", isMd5 == true ? "Md5" : " Prefab.Name");
            EditorUtility.DisplayProgressBar ("UpdateAssetBundlesName", info, 0);
            AssetImporter
            import = null;
            float i = 0;
            float allLen = allAssets.Length;
            StringBuilder tipsInfo = new StringBuilder ();
            foreach (string path in allAssets) {
                import = AssetImporter.GetAtPath (path);
                if (import != null && string.IsNullOrEmpty (import.assetBundleName) == false) {
                    string folder = EditorUtils.GetLabelsByPath (path);
                    Object s = AssetDatabase.LoadAssetAtPath (path, typeof (Object));
                    SetAssetBundleName (s, true);
                    tipsInfo.AppendLine (s.name + " path = " + path);
                } else {
                    //Debug.LogWarning(path + " import not exist");
                }
                EditorUtility.DisplayProgressBar ("UpdateAssetBundlesName", info + "=>" + i.ToString () + "/" + allLen.ToString (), i / allLen);
                i++;
            }

            EditorUtility.ClearProgressBar ();
            Debug.Log (tipsInfo.ToString () + " Complete!");
        }

        /// <summary>
        /// 设置为扩展包路径文件夹
        /// 
        /// </summary>
        public static void SetAsExtendsFloder () {
            Object[] selection = Selection.objects;
            string apath = null;
            foreach (Object s in selection) {
                if (s is DefaultAsset) {
                    apath = AssetDatabase.GetAssetPath (s);
                    AssetImporter
                    import = AssetImporter.GetAtPath (apath);
                    import.assetBundleName = null;
                    //import.assetBundleVariant = null;
                    import.userData = EditorUtils.HugulaFolder;
                    AssetDatabase.SetLabels (s, new string[] { EditorUtils.HugulaFolder });
                    import.SaveAndReimport ();
                    if (!HugulaExtensionFolderEditor.ContainsExtendsPath (apath)) {
                        Debug.LogFormat ("add extends path = {0}", s.name);
                        HugulaExtensionFolderEditor.AddExtendsPath (apath);
                    } else {
                        Debug.LogFormat ("extends path = {0} is already exists", s.name);
                    }
                    AssetDatabase.Refresh ();
                }
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        /// <summary>
        /// 添加为扩展文件
        /// </summary>
        public static void ExtensionFiles (bool delete = false) {
            Object[] selection = EditorUtils.SelectObjects ();
            string abName = "";
            string apath = "";
            foreach (Object s in selection) {
                abName = GetOriginalAssetBundleName (s, out apath);
                if (string.IsNullOrEmpty (abName)) continue;

                if (delete)
                    HugulaExtensionFolderEditor.RemoveExtendsFile (abName);
                else if (!string.IsNullOrEmpty (abName) && !HugulaExtensionFolderEditor.ContainsExtendsPath (apath))
                    HugulaExtensionFolderEditor.AddExtendsFile (abName);
                else
                    Debug.LogFormat ("assetPath({0}) can't add to extends file list  ", abName);
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        /// <summary>
        /// 添加保留文件
        /// </summary>
        public static void AddOnlyInclusionFiles (bool delete = false) {
            string abName = "";
            string apath = "";

            Object[] selection = EditorUtils.SelectObjects (); //typeof(GameObject),typeof(Texture2D));
            foreach (Object s in selection) {
                abName = GetOriginalAssetBundleName (s, out apath);
                if (string.IsNullOrEmpty (abName)) continue;

                if (delete)
                    HugulaExtensionFolderEditor.RemoveExtendsFile (abName);
                else if (!string.IsNullOrEmpty (abName) && !HugulaExtensionFolderEditor.ContainsExtendsPath (apath))
                    HugulaExtensionFolderEditor.AddOnlyInclusionFiles (abName);
                else
                    Debug.LogFormat ("assetPath({0}) can't add to extends file list  ", abName);
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        /// <summary>
        /// 添加为首包下载文档
        /// </summary>
        public static void FirstLoadFiles (bool delete = false) {
            Object[] selection = EditorUtils.SelectObjects ();
            string abName = "";
            string apath = "";
            foreach (Object s in selection) {
                abName = GetOriginalAssetBundleName (s, out apath);
                Debug.LogFormat ("abname={0}", abName, apath);
                if (string.IsNullOrEmpty (abName)) continue;

                if (delete)
                    HugulaExtensionFolderEditor.RemoveExtendsFile (abName);
                else if (!string.IsNullOrEmpty (abName) && !HugulaExtensionFolderEditor.ContainsExtendsPath (apath))
                    HugulaExtensionFolderEditor.AddFirstLoadFile (abName);
                else
                    Debug.LogFormat ("assetPath({0}) can't add to extends file list  ", apath);
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        /// <summary>
        /// 从选中的txt排除extension Files
        /// </summary>
        public static void ExcludeExtensionFiles () {
            Object[] selection = Selection.objects;
            List<string> excludes = new List<string> ();
            foreach (Object s in selection) {
                if (s is TextAsset) {
                    excludes.Clear ();
                    string txt = ((TextAsset) s).text;
                    string[] sps = txt.Split ('\n');
                    Debug.Log (s);
                    foreach (var str in sps) {
                        excludes.Add (str.Trim ().Split (' ') [0]);
                    }
                    HugulaExtensionFolderEditor.RemoveExtendsFiles (excludes);
                    Debug.LogFormat ("delete: {0}\r\n {1}", s.name, txt);
                }
            }

            HugulaExtensionFolderEditor.SaveSettingData ();

        }

        public static void AddOnlyInclusionFilesByTxt () {
            Object[] selection = Selection.objects;

            foreach (Object s in selection) {
                if (s is TextAsset) {
                    string txt = ((TextAsset) s).text;
                    string[] sps = txt.Split ('\n');
                    Debug.Log (s);
                    foreach (var str in sps) {
                        if (!string.IsNullOrEmpty (str.Trim ()))
                            HugulaExtensionFolderEditor.AddOnlyInclusionFiles (str.Trim ());
                    }
                }
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        public static void RemapMd5fileName () {
            //read mad5 name mapping file
            TextAsset md5mapping = (TextAsset) AssetDatabase.LoadAssetAtPath ("Assets/Tmp/md5_name_mapping.txt", typeof (TextAsset));
            if (md5mapping == null) {
                // Debug.LogWarning("md5_name_mapping.txt is not exists use(AssetBundles/Generate/AssetBundle Md5Mapping) generate");
                AssetbundlesMenuItems.GenerateAssetBundlesMd5Mapping ();
                md5mapping = (TextAsset) AssetDatabase.LoadAssetAtPath ("Assets/Tmp/md5_name_mapping.txt", typeof (TextAsset));
            }

            ByteReader reader = new ByteReader (md5mapping.bytes);
            var mDictionary = reader.ReadDictionary ();
            string line = string.Empty;
            string fileName = string.Empty;
            string value = string.Empty;
            Object[] selection = Selection.objects;
            foreach (Object s in selection) {
                if (s is TextAsset) {
                    StringBuilder outSb = new StringBuilder ();
                    string txt = ((TextAsset) s).text;
                    // string fileName = 	txt.Split(' ');
                    string[] sps = txt.Split ('\n');
                    Debug.Log (s);
                    foreach (var str in sps) {
                        fileName = str.Split ('\t') [0].Trim ();
                        line = fileName.Split ('.') [0].Trim ();
                        if (mDictionary.TryGetValue (line, out value)) {
                            outSb.AppendLine (fileName.Replace (line, value));
                        } else {
                            Debug.LogWarningFormat ("md5_name_mapping.txt is not contains {0} ", line);
                        }
                    }

                    using (FileStream fs = new FileStream ("Assets/Tmp/" + s.name + "-new.txt", FileMode.Create)) {
                        var bytes = LuaHelper.GetBytes (outSb.ToString ());
                        fs.Write (bytes, 0, bytes.Length);
                    }

                }
            }
        }

        public static void CheckInCludeAssetbundleSize () {
            //
            HugulaExtensionFolderEditor.instance = null;
            var onlyInclusionFiles = HugulaExtensionFolderEditor.instance.OnlyInclusionFiles;

            uint size = 0;
            foreach (var f in onlyInclusionFiles) {
                size += (uint) GetAssetbundleSize (f);
                // FileInfo finfo = new FileInfo (CUtils.PathCombine (CUtils.realStreamingAssetsPath, CUtils.GetRightFileName (f)));
                // if (finfo.Exists)
                //     size += (uint) finfo.Length;
                // else
                //     Debug.LogWarningFormat ("file not exits:{0}", f);
            }

            Debug.LogFormat ("OnlyInclusionFiles total:{0} kb", (float) size / 1024.0f);
            // onlyInclusionRightFiles.Add();
        }

        private static int GetAssetbundleSize (string abName) {
            FileInfo finfo = new FileInfo (CUtils.PathCombine (CUtils.realStreamingAssetsPath, abName));
            int abSize = 0;
            if (finfo.Exists) {
                abSize = (int) finfo.Length;
            } else {
                finfo = new FileInfo (EditorUtils.GetLuaBytesResourcesPath () + "/" + abName);
                if (finfo.Exists) abSize = (int) finfo.Length;
            }

            return abSize;
        }

        public static void CheckFirstLoaddAssetbundleSize () {
            //
            HugulaExtensionFolderEditor.instance = null;
            var files = HugulaExtensionFolderEditor.instance.FirstLoadFiles;

            uint size = 0;
            foreach (var f in files) {
                FileInfo finfo = new FileInfo (CUtils.PathCombine (CUtils.realStreamingAssetsPath, CUtils.GetRightFileName (f)));
                if (finfo.Exists)
                    size += (uint) finfo.Length;
                else
                    Debug.LogWarningFormat ("file not exits:{0}", f);
            }

            Debug.LogFormat ("FirstLoadFiles total:{0} kb", (float) size / 1024.0f);
            // onlyInclusionRightFiles.Add();
        }

        public static void CheckSelectedAssetbundleSize () {
            //
            List<string> onlyInclusionFiles = new List<string> ();
            var floder = Selection.objects[0];
            var path=AssetDatabase.GetAssetPath(floder);
            Object[] selection = EditorUtils.SelectObjects ();
            string apath = null;
            foreach (Object s in selection) {
                string suffix = string.Empty;
                apath = AssetDatabase.GetAssetPath (s);
                AssetImporter import = AssetImporter.GetAtPath (apath);

                if (string.IsNullOrEmpty (import.assetBundleName)) {
                    // Debug.LogWarningFormat ("file:{0} has't set assetbundle name!", apath);
                    continue;
                };

                if (!string.IsNullOrEmpty (import.assetBundleVariant))
                    suffix = "." + import.assetBundleVariant;

                onlyInclusionFiles.Add (import.assetBundleName + suffix);
            }

            uint size = 0;
            int count = 0;
            foreach (var f in onlyInclusionFiles) {
                FileInfo finfo = new FileInfo (CUtils.PathCombine (CUtils.realStreamingAssetsPath, f));
                if (finfo.Exists) {
                    count++;
                    size += (uint) finfo.Length;
                } else
                    Debug.LogWarningFormat ("file not exits:{0}", f);
            }

            Debug.LogFormat ("total:{0} kb,count:{1},path={2}", (float) size / 1024.0f, count,path);
        }

        public static void ZipFiles (bool delete = false) {
            // Object[] selection = Selection.objects;
            // string abName = "";
            // string apath = "";

            // List<string> abNames = new List<string>();
            // foreach (Object s in selection)
            // {
            //     abName = GetOriginalAssetBundleName(s, out apath);
            //     if (!HugulaExtensionFolderEditor.ContainsExtendsPath(apath))
            //         abNames.Add(abName);
            //     else
            //         Debug.LogFormat("assetPath({0}) can't add to zip file list ", abName);

            // }

            // if (abNames.Count > 0 && delete)
            // {
            //     HugulaExtensionFolderEditor.RemoveZipFile(abNames);
            // }
            // else if (abNames.Count > 0)
            // {
            //     HugulaExtensionFolderEditor.AddZipFile(abNames);
            // }

        }

        //获取明文的assetbundle name用于editor显示
        private static string GetOriginalAssetBundleName (Object s, out string apath) {
            string suffix = string.Empty;
            apath = AssetDatabase.GetAssetPath (s);
            string abName = "";
            AssetImporter import = AssetImporter.GetAtPath (apath);

            if (string.IsNullOrEmpty (import.assetBundleName)) return string.Empty;

            if (!string.IsNullOrEmpty (import.assetBundleVariant))
                suffix = "." + import.assetBundleVariant;
            string repName = HugulaEditorSetting.instance.GetAssetBundleNameByReplaceIgnore (s.name.ToLower ());
            abName = repName + Common.CHECK_ASSETBUNDLE_SUFFIX + suffix;
            return abName;
        }

        /// <summary>
        /// 清理扩展文件夹
        /// </summary>
        public static void ClearExtendsFloder () {
            Object[] selection = Selection.objects;
            string apath = null;
            foreach (Object s in selection) {
                if (s is DefaultAsset) {
                    apath = AssetDatabase.GetAssetPath (s);
                    AssetImporter
                    import = AssetImporter.GetAtPath (apath);
                    import.userData = null;
                    import.assetBundleName = null;
                    //import.assetBundleVariant = null;
                    AssetDatabase.SetLabels (s, null);
                    import.SaveAndReimport ();
                    //apath = apath.Replace("\\","/");
                    if (HugulaExtensionFolderEditor.ContainsExtendsPath (apath)) {
                        Debug.LogFormat ("{0},Clear AssetLabels,path ={1}", s.name, apath);
                        HugulaExtensionFolderEditor.RemoveExtendsPath (apath);
                        AssetDatabase.Refresh ();
                    }
                }
            }

            HugulaExtensionFolderEditor.SaveSettingData ();
        }

        #endregion

        #endregion

        public static void BuildAssetBundles () {
            CUtils.DebugCastTime ("Time HandleUpdateMaterail End");
            EditorUtils.CheckstreamingAssetsPath ();
            CUtils.DebugCastTime ("Time CheckstreamingAssetsPath End");
            var ab = BuildPipeline.BuildAssetBundles (EditorUtils.GetOutPutPath (), SplitPackage.DefaultBuildAssetBundleOptions, target);
            SplitPackage.CreateStreamingFileManifest (ab);
            CUtils.DebugCastTime ("Time BuildPipeline.BuildAssetBundles End");
        }

        /// <summary>
        /// 自动构建ab
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="abName"></param>
        /// <param name="bbo"></param>
        static public void BuildABs (string[] assets, string outPath, string abName, BuildAssetBundleOptions bbo) {
            AssetBundleBuild[] bab = new AssetBundleBuild[1];
            bab[0].assetBundleName = abName; //打包的资源包名称 随便命名
            bab[0].assetNames = assets;
            if (string.IsNullOrEmpty (outPath))
                outPath = EditorUtils.GetOutPutPath ();

            string tmpPath = EditorUtils.GetProjectTempPath ();
            EditorUtils.CheckDirectory (tmpPath);
            string tmpFileName = Path.Combine (tmpPath, abName);
            BuildPipeline.BuildAssetBundles (tmpPath, bab, bbo, target);

            string targetFileName = Path.Combine (outPath, abName);
            FileInfo tInfo = new FileInfo (targetFileName);
            if (tInfo.Exists) tInfo.Delete ();
            FileInfo fino = new FileInfo (tmpFileName);
            fino.CopyTo (targetFileName);
        }

        /// <summary>
        /// 自动构建abs
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="abName"></param>
        /// <param name="bbo"></param>
        static public void BuildABs (AssetBundleBuild[] bab, string outPath, BuildAssetBundleOptions bbo) {
            if (string.IsNullOrEmpty (outPath))
                outPath = EditorUtils.GetOutPutPath ();

            string tmpPath = EditorUtils.GetProjectTempPath ();
            EditorUtils.CheckDirectory (tmpPath);

            BuildPipeline.BuildAssetBundles (tmpPath, bab, bbo, target);

            foreach (AssetBundleBuild abb in bab) {
                string abName = abb.assetBundleName;
                string tmpFileName = Path.Combine (tmpPath, abName);
                string targetFileName = Path.Combine (outPath, abName);
                FileInfo tInfo = new FileInfo (targetFileName);
                if (tInfo.Exists) tInfo.Delete ();
                FileInfo fino = new FileInfo (tmpFileName);
                fino.CopyTo (targetFileName);
            }

        }

    }
}