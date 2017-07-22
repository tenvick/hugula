using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

using Hugula.Utils;
using Hugula.Update;
using Hugula.Loader;

namespace Hugula.Editor
{

    /// <summary>
    /// Split package.
    /// </summary>
    public class SplitPackage
    {
#if UNITY_STANDALONE_WIN || UNITY_IPHONE// || UNITY_ANDROID
        public const BuildAssetBundleOptions DefaultBuildAssetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression; //
#else
        public const BuildAssetBundleOptions DefaultBuildAssetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle;// | BuildAssetBundleOptions.ChunkBasedCompression;
#endif

        public const string VerExtends = EditorCommon.VerExtends;//"VerExtends.txt";
        public const string VerExtendsPath = EditorCommon.ConfigPath;// "Assets/Hugula/Config/";

        public const string ResFolderName = EditorCommon.ResFolderName;//"res";

        /// <summary>
        /// 更新文件输出根目录 release
        /// </summary>
        public static string UpdateOutPath
        {
            get
            {
                if (string.IsNullOrEmpty(_updateOutPath))
                {
                    _updateOutPath = Path.Combine(FirstOutReleasePath, CUtils.platform);
                    DirectoryInfo dinfo = new DirectoryInfo(_updateOutPath);
                    if (!dinfo.Exists) dinfo.Create();
                }
                return _updateOutPath;
            }

            set
            {
                _updateOutPath = value;
            }
        }

        /// <summary>
        /// 更新文件输出根目录 develop
        /// </summary>
        public static string UpdateOutDevelopPath
        {
            get
            {
                if (string.IsNullOrEmpty(_updateOutDevelopPath))
                {
                    _updateOutDevelopPath = Path.Combine(FirstOutDevelopPath, CUtils.platform);
                    DirectoryInfo dinfo = new DirectoryInfo(_updateOutPath);
                    if (!dinfo.Exists) dinfo.Create();
                }
                return _updateOutDevelopPath;
            }
            set
            {
                _updateOutDevelopPath = value;
            }
        }

        /// <summary>
        /// 版本输出develop目录
        /// </summary>
        public static string UpdateOutVersionDevelopPath
        {
            get
            {
                string updateOutPath = string.Format("{0}/v{1}", UpdateOutDevelopPath, CodeVersion.CODE_VERSION);
                return updateOutPath;
            }
        }


        /// <summary>
        /// 版本输出relase目录
        /// </summary>
        public static string UpdateOutVersionPath
        {
            get
            {
                string updateOutPath = string.Format("{0}/v{1}", UpdateOutPath, CodeVersion.CODE_VERSION);
                return updateOutPath;
            }
        }


        private static string _updateOutPath, _updateOutDevelopPath;
        public delegate StringBuilder filterSB(string key, StringBuilder manual, StringBuilder normal, HashSet<string> manualList);
        private static string FirstOutPath
        {
            get
            {

                DirectoryInfo firstDir = new DirectoryInfo(Application.dataPath);
                string firstPath = Path.Combine(firstDir.Parent.Parent.FullName, Common.FirstOutPath);
                return firstPath;
            }

        }

        private static string FirstOutReleasePath
        {
            get
            {
                string releasePath = Path.Combine(FirstOutPath, "release");
                return releasePath;
            }
        }

        private static string FirstOutDevelopPath
        {
            get
            {
                string releasePath = Path.Combine(FirstOutPath, "dev");
                return releasePath;
            }
        }


        #region public


        /// <summary>
        /// 1 读取首包，找出忽略文件
        /// </summary>
        /// <param name="ignoreFiles">Ignore files.</param>
        public static bool ReadFirst(out FileManifest firstCrcDict, out FileManifest streamingManifest, FileManifest extensionFileManifest)
        {
            // string title = "read first crc file list";
            bool firstExists = false;
            firstCrcDict = null;
            // extensionFileManifest = null;
            string readPath = Path.Combine(FirstOutReleasePath, CUtils.platform);
            string firstFileName = CUtils.InsertAssetBundleName(CUtils.GetRightFileName(Common.CRC32_FILELIST_NAME), "_v" + CodeVersion.CODE_VERSION.ToString());
            readPath = Path.Combine(readPath, firstFileName);
            Debug.Log(readPath);

            // extensionFileManifest.Clear();
            //读取首包
            WWW abload = new WWW("file://" + readPath);
            if (string.IsNullOrEmpty(abload.error) && abload.assetBundle != null)
            {
                var ab = abload.assetBundle;
                Object[] assets = ab.LoadAllAssets();
                foreach (Object o in assets)
                {
                    if (o is FileManifest)
                    {
                        firstCrcDict = o as FileManifest;
                        firstExists = true;
                        Debug.Log(firstCrcDict.Count);
                    }
                }
                ab.Unload(false);
            }
            else
            {
                Debug.LogWarning(abload.error + "no frist packeage in " + readPath);
            }
            abload.Dispose();

            //读取本地AB包AssetBundleManifest
            var fileListName = Common.CRC32_FILELIST_NAME;
            var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), CUtils.GetRightFileName(fileListName));
            Debug.Log(url);
            url = CUtils.GetAndroidABLoadPath(url);
            AssetBundle assetbundle = AssetBundle.LoadFromFile(url);
            var assets1 = assetbundle.LoadAllAssets<FileManifest>();

            Debug.Log(assets1.Length);
            var streamingManifest1 = assets1[0];
            assetbundle.Unload(false);
            streamingManifest = streamingManifest1;
            Debug.Log(streamingManifest1.appNumVersion);

            //读取忽略扩展包

            System.Action<string, int> AddExtensionFileManifest = (string ab, int priority1) =>
             {
                 var abinfo = streamingManifest1.GetABInfo(ab);
                 if (abinfo == null)
                     abinfo = new ABInfo(ab, 0, 0, priority1);

                 abinfo.priority = priority1;
                 extensionFileManifest.Add(abinfo);
             };

            string firstStreamingPath = CUtils.realStreamingAssetsPath;
            //读取忽略扩展文件夹
            DirectoryInfo dinfo = new DirectoryInfo(firstStreamingPath);
            var dircs = dinfo.GetDirectories();
            var priority = FileManifestOptions.AutoHotPriority;
            foreach (var dir in dircs)
            {
                var u3dList = ExportResources.getAllChildFiles(dir.FullName, @"\.meta$|\.manifest$|\.DS_Store$", null, false);
                foreach (var s in u3dList)
                {
                    priority++;
                    string ab = CUtils.GetAssetBundleName(s);
                    ab = ab.Replace("\\", "/");
                    AddExtensionFileManifest(ab, priority);
                }
            }

            //读取忽略别名后缀
            priority = FileManifestOptions.StreamingAssetsPriority;
            var inclusionVariants = HugulaSetting.instance.inclusionVariants;
            var allVariants = HugulaSetting.instance.allVariants;
            string pattern = "";
            string sp = "";
            foreach (var s in allVariants)
            {
                if (!inclusionVariants.Contains(s))
                {
                    pattern += sp + @"\." + s + "$";
                    sp = "|";
                }
            }

            if (!string.IsNullOrEmpty(pattern))
            {
                // Debug.Log(pattern);
                var u3dList = ExportResources.getAllChildFiles(dinfo.FullName, pattern, null, true);
                foreach (var s in u3dList)
                {
                    priority++;
                    string ab = CUtils.GetAssetBundleName(s);
                    ab = ab.Replace("\\", "/");
                    AddExtensionFileManifest(ab, priority);
                }
            }

            //读取手动排除后缀
            var extensionFiles = HugulaExtensionFolderEditor.instance.ExtensionFiles;
            foreach (var s in extensionFiles)
            {
                var ab = CUtils.GetRightFileName(s);
                priority++;
                AddExtensionFileManifest(ab, priority);
            }

            // Debug.LogFormat("extensionFileManifest.Count:{0}.", extensionFileManifest.Count);

            //从网络读取扩展加载列表 todo

            EditorUtility.ClearProgressBar();
            return firstExists;
        }

        /// <summary>
        /// 2 Creates the content of the crc list.
        /// </summary>
        /// <returns>The crc list content.</returns>
        /// <param name="allBundles">All bundles.</param>
        /// <param name="manualFileList">manual file list.</param>
        public static List<ABInfo>[] CreateCrcListContent(string[] allBundles, FileManifest firstCrcDict, FileManifest streamingManifest, FileManifest manualFileList)
        {
            string title = "create crc list content ";
            List<ABInfo>[] abInfoArray = new List<ABInfo>[2];
            var diffManifest = new List<ABInfo>();
            abInfoArray[0] = diffManifest;

            float i = 0;
            float allLen = allBundles.Length;

            //忽略列表
            Dictionary<string, bool> ignore = new Dictionary<string, bool>();
            ignore.Add(CUtils.GetRightFileName(Common.CRC32_FILELIST_NAME), true);
            ignore.Add(CUtils.GetRightFileName(Common.CRC32_VER_FILENAME), true);
            ignore.Add(CUtils.GetRightFileName(CUtils.platform),true);

            string extension;
            foreach (var str in allBundles)
            {
                string url = Path.Combine(Application.dataPath, str);
                uint outCrc = 0;
                uint fileLen = 0;
                string abName = str.Replace("\\", "/");
                string key = BuildScript.GetAssetBundleName(abName);
                //后缀替换
                extension = System.IO.Path.GetExtension(key);
                if (string.IsNullOrEmpty(extension))
                {
                    key = key + Common.CHECK_ASSETBUNDLE_SUFFIX;
                }
                else if (extension.Equals(Common.DOT_BYTES))
                {
                    key = key.Replace(extension, Common.CHECK_ASSETBUNDLE_SUFFIX);
                }

                if (!ignore.ContainsKey(key))
                {
                    outCrc = CrcCheck.GetLocalFileCrc(url, out fileLen);
                    var extendsABinfo = streamingManifest.GetABInfo(key);
                    if (extendsABinfo == null)
                    {
                        extendsABinfo = new ABInfo(key, outCrc, fileLen, 0);
                        streamingManifest.Add(extendsABinfo);
                    }

                    extendsABinfo.crc32 = outCrc;
                    extendsABinfo.size = fileLen;

                    ABInfo localFirstInfo;
                    if (firstCrcDict != null && (localFirstInfo = firstCrcDict.GetABInfo(extendsABinfo.abName)) != null)
                    {
                        if (!firstCrcDict.CheckABCrc(extendsABinfo))
                        {
                            var newAbinfo = extendsABinfo.Clone();
                            newAbinfo.assetPath = str;
                            diffManifest.Add(newAbinfo);
                        }
                        else if (!extendsABinfo.EqualsDependencies(localFirstInfo)) //dependencies change
                        {
                            var newAbinfo = extendsABinfo.Clone();
                            // newAbinfo.assetPath = str;
                            diffManifest.Add(newAbinfo);
                        }
                    }
                }
                EditorUtility.DisplayProgressBar(title, title + "=>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
                i++;
            }

            // CrcCheck.Clear();
            EditorUtility.ClearProgressBar();
            return abInfoArray;
        }

        public static uint CreateStreamingFileManifest(AssetBundleManifest assetBundleManifest)
        {

            var allABs = assetBundleManifest.GetAllAssetBundles();
            var bundlesWithVariant = assetBundleManifest.GetAllAssetBundlesWithVariant();

            var streamingManifest = ScriptableObject.CreateInstance(typeof(FileManifest)) as FileManifest;
            //读取 bundlesWithVariant
            var MyVariant = new string[bundlesWithVariant.Length];
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                var curSplit = bundlesWithVariant[i].Split('.');
                MyVariant[i] = bundlesWithVariant[i];
            }
            //读取abinfo
            List<ABInfo> allABInfos = new List<ABInfo>();
            foreach (var abs in allABs)
            {
                var abInfo = new ABInfo(abs, 0, 0, 0);
                var dependencies = assetBundleManifest.GetDirectDependencies(abs);
                abInfo.dependencies = dependencies;
                allABInfos.Add(abInfo);
            }

            //fill data
            streamingManifest.allAbInfo = allABInfos;
            streamingManifest.allAssetBundlesWithVariant = bundlesWithVariant;
            streamingManifest.appNumVersion = CodeVersion.APP_NUMBER;
            //create asset
            string tmpPath = BuildScript.GetAssetTmpPath();// Path.Combine(Application.dataPath, BuildScript.TmpPath);
            ExportResources.CheckDirectory(tmpPath);
            var crc32filename = CUtils.GetAssetName(Common.CRC32_FILELIST_NAME);
            string assetPath = "Assets/" + BuildScript.TmpPath + crc32filename + ".asset";
            AssetDatabase.CreateAsset(streamingManifest, assetPath);
            //build assetbundle
            string crc32outfilename = CUtils.GetRightFileName(Common.CRC32_FILELIST_NAME);
            BuildScript.BuildABs(new string[] { assetPath }, null, crc32outfilename, DefaultBuildAssetBundleOptions);

            streamingManifest.WriteToFile("Assets/" + BuildScript.TmpPath + "local_streamingManifest.txt");
            Debug.LogFormat("FileManifest  Path = {0}/{1};", CUtils.realStreamingAssetsPath, crc32outfilename);
            return 0;
        }

        /// <summary>
        /// Creates the streaming crc list.
        /// </summary>
        /// <param name="sb">Sb.</param>
        public static uint CreateStreamingCrcList(FileManifest sb, string fileListName, bool firstExists = false, bool copyToResFolder = false)
        {
            sb.appNumVersion = CodeVersion.APP_NUMBER;
            var crc32filename = CUtils.GetAssetName(fileListName);
            string tmpPath = BuildScript.GetAssetTmpPath();// Path.Combine(Application.dataPath, BuildScript.TmpPath);
            ExportResources.CheckDirectory(tmpPath);

            string assetPath = "Assets/" + BuildScript.TmpPath + crc32filename + ".asset";
            EditorUtility.DisplayProgressBar("Generate streaming crc file list", "write file to " + assetPath, 0.99f);
            AssetDatabase.CreateAsset(sb, assetPath);

            string crc32outfilename = CUtils.GetRightFileName(fileListName);
            // Debug.Log("write to path=" + outPath);
            //读取crc
            string abPath = string.Empty;
            string resOutPath = null;
            uint fileSize = 0;
            uint fileCrc = 0;

            if (copyToResFolder)
            {
                resOutPath = tmpPath;//Path.Combine(tmpPath, ResFolderName); //Path.Combine(SplitPackage.UpdateOutPath, ResFolderName);
                abPath = Path.Combine(resOutPath, crc32outfilename);
                BuildScript.BuildABs(new string[] { assetPath }, resOutPath, crc32outfilename, DefaultBuildAssetBundleOptions);
                fileCrc = CrcCheck.GetLocalFileCrc(abPath, out fileSize);

                //copy crc list
                FileInfo finfo = new FileInfo(abPath);
                var resType = HugulaSetting.instance.backupResType;
                if (resType == CopyResType.VerResFolder)
                {
                    string verPath = Path.Combine(UpdateOutVersionPath, ResFolderName);//特定版本资源目录用于资源备份
                    string newName = Path.Combine(verPath, InsertAssetBundleName(crc32outfilename, "_" + fileCrc.ToString()));
                    FileHelper.CheckCreateFilePathDirectory(newName);
                    if (File.Exists(newName)) File.Delete(newName);
                    finfo.CopyTo(newName);
                }

                if (resType == CopyResType.OneResFolder)
                {
                    string updateOutPath = Path.Combine(UpdateOutPath, ResFolderName);//总的资源目录
                    string newName = Path.Combine(updateOutPath, InsertAssetBundleName(crc32outfilename, "_" + fileCrc.ToString()));
                    FileHelper.CheckCreateFilePathDirectory(newName);
                    if (File.Exists(newName)) File.Delete(newName);
                    finfo.CopyTo(newName);
                }

            }
            else
            {

                abPath = Path.Combine(CUtils.realStreamingAssetsPath, crc32outfilename);
                BuildScript.BuildABs(new string[] { assetPath }, null, crc32outfilename, DefaultBuildAssetBundleOptions);
                fileCrc = CrcCheck.GetLocalFileCrc(abPath, out fileSize);
            }
            // CrcCheck.Clear();

            //copy first crc list
            if (!firstExists && File.Exists(abPath)) //如果没有首包 copy first package
            {
                string crc32FirstOutName = CUtils.InsertAssetBundleName(crc32outfilename, "_v" + CodeVersion.CODE_VERSION.ToString());
                string destFirst = Path.Combine(UpdateOutPath, crc32FirstOutName);
                Debug.LogFormat("abpath={0},destFirst={1}:", abPath, destFirst);
                File.Copy(abPath, destFirst, true);
            }

            EditorUtility.ClearProgressBar();
            // File.Delete(assetPath);

            Debug.Log("Crc file list assetbunle build complate! " + fileCrc.ToString() + abPath);

            return fileCrc;
        }

        private static StringBuilder ReadVerExtensionFile(string path)
        {
            StringBuilder verExtSB = new StringBuilder();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string item;
                    string[] kvs;
                    while ((item = sr.ReadLine()) != null)
                    {
                        kvs = item.Split(':');
                        if (kvs[0] == @"""cdn_host""")
                        {
                            //APP_NUMBER CODE_VERSION
                            if (HugulaSetting.instance.appendCrcToFile)
                                item = item.Replace("%s", "v" + CodeVersion.CODE_VERSION.ToString());
                            else
                                item = item.Replace("%s", "v" + CodeVersion.APP_NUMBER.ToString()); //文件crc 不变路径需要改变
                        }
                        verExtSB.AppendFormat(",{0}", item);

                    }
                }
            }

            return verExtSB;
        }

        /// <summary>
        /// Creates the version asset bundle.
        /// </summary>
        /// <param name="fileCrc">File crc.</param>
        public static void CreateVersionAssetBundle(uint fileCrc, bool toRelease, bool is_dev_Context = false)
        {
            CodeVersion.CODE_VERSION = 0;
            CodeVersion.APP_NUMBER = 0;
            //read ver extends
            string ver_file_name = VerExtends;
            string path = string.Empty;
            string verCopyToPath = string.Empty;

            if (is_dev_Context)
                ver_file_name = Path.Combine(VerExtendsPath, CUtils.InsertAssetBundleName(ver_file_name, "_" + CUtils.platform + "_dev").Replace("//", "/"));
            else
                ver_file_name = Path.Combine(VerExtendsPath, CUtils.InsertAssetBundleName(ver_file_name, "_" + CUtils.platform).Replace("//", "/"));

            if (toRelease)
            {
                path = CUtils.GetRealStreamingAssetsPath();
            }
            else
            {
                path = UpdateOutVersionDevelopPath;
            }

            //check path
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            StringBuilder verExtSB = ReadVerExtensionFile(ver_file_name);
            Debug.LogFormat("read extends:{0},content={1}", ver_file_name, verExtSB.ToString());

            string outPath = Path.Combine(path, CUtils.GetRightFileName(Common.CRC32_VER_FILENAME));
            Debug.Log("verion to path=" + outPath);
            EditorUtility.DisplayProgressBar("Create Version AssetBundle File", "write file to " + outPath, 0.99f);
            //json 化version{ code,crc32,version}
            StringBuilder verJson = new StringBuilder();
            verJson.Append("{");
            verJson.Append(@"""code"":" + CodeVersion.CODE_VERSION + ",");
            verJson.Append(@"""crc32"":" + fileCrc.ToString() + ",");
            verJson.Append(@"""time"":" + CUtils.ConvertDateTimeInt(System.DateTime.Now) + ",");
            verJson.Append(@"""version"":""" + CodeVersion.APP_VERSION + @"""");
            verJson.Append(verExtSB.ToString());
            verJson.Append("}");

            if (toRelease)
            {
                verCopyToPath = UpdateOutPath; //输出ver.txt
                // build ver.u to release
                if (!Directory.Exists(UpdateOutVersionPath)) Directory.CreateDirectory(UpdateOutVersionPath);
                string outVerU = Path.Combine(UpdateOutVersionPath, CUtils.GetRightFileName(Common.CRC32_VER_FILENAME));
                using (StreamWriter sr = new StreamWriter(outVerU, false))
                {
                    sr.Write(verJson.ToString());
                }
            }
            else
            {
                verCopyToPath = UpdateOutDevelopPath; //输出ver.txt
            }

            //copy ver.txt
            if (!Directory.Exists(verCopyToPath)) Directory.CreateDirectory(verCopyToPath);
            using (StreamWriter sr = new StreamWriter(Path.Combine(verCopyToPath, "ver.txt"), false))
            {
                sr.Write(verJson.ToString());
            }

            //build ver.u
            using (StreamWriter sr = new StreamWriter(outPath, false))
            {
                sr.Write(verJson.ToString());
            }
            Debug.Log(verJson.ToString());
            Debug.Log("Build Version Complete = " + fileCrc.ToString() + " path " + outPath);
            // BuildScript.BuildAssetBundles();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        ///   delete res folder
        /// </summary>
        public static void DeleteSplitPackageResFolder()
        {
            string updateOutPath = Path.Combine(UpdateOutPath, ResFolderName);
            Debug.Log("Delete directory " + updateOutPath);
            ExportResources.DirectoryDelete(updateOutPath);

            string updateOutVersionPath = Path.Combine(UpdateOutVersionPath, ResFolderName);
            Debug.Log("Delete directory " + updateOutVersionPath);
            ExportResources.DirectoryDelete(updateOutVersionPath);

        }

        public static void CopyChangeFileToSplitFolder(bool firstExists, FileManifest firstCrcDict, FileManifest currCrcDict, FileManifest diffCrcDict, FileManifest manualFileList)
        {
            Dictionary<string, ABInfo> updateList = new Dictionary<string, ABInfo>();

            bool copyManual = HugulaSetting.instance.spliteExtensionFolder;

            if (firstExists) //没有首包不copy资源
            {
                Debug.LogFormat("copy diffCrcDict.count={0}", diffCrcDict.Count);

                var infos = diffCrcDict.allAbInfo;
                foreach (var ab in infos)
                {
                    updateList[ab.abName] = ab;// Debug.Log(ab);
                }

            }

            if (copyManual)//copy 手动下载列表
            {
                Debug.LogFormat("copy manualFileList.count={0},currCrcDict.count={1}", manualFileList.Count, currCrcDict.Count);
                if (manualFileList.Count > 0)
                {
                    var infos = manualFileList.allAbInfo;
                    foreach (var ab in infos)
                    {
                        var ckAb = currCrcDict.GetABInfo(ab.abName);
                        if (ckAb != null)
                        {
                            updateList[ab.abName] = ckAb;
                        }
                    }
                }
            }

            CopyFileToSplitFolder(updateList);

            AssetDatabase.Refresh();
        }

        public static void DeleteStreamingFiles(ICollection<ABInfo> abNames)
        {
            EditorUtility.DisplayProgressBar("Delete Streaming AssetBundle File", "", 0.09f);
            string path = CUtils.realStreamingAssetsPath;
            float all = abNames.Count;
            float i = 0;
            foreach (var ab in abNames)
            {
                i = i + 1;
                string delPath = Path.Combine(path, ab.abName);
                File.Delete(delPath);
                File.Delete(delPath + ".meta");
                File.Delete(delPath + ".manifest");
                File.Delete(delPath + ".manifest.meta");
                EditorUtility.DisplayProgressBar("Delete Streaming AssetBundle File", "file " + ab.abName, i / all);

            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        //压缩extendsFoler.txt zipFiles
        public static void ZipAssetbundles()
        {
            HugulaExtensionFolderEditor.instance = null;
            var zipFiles = HugulaExtensionFolderEditor.instance.ZipFiles;
            List<string> outZips = new List<string>();
            string zipGroupName = "zip_group_{0}.zip";
            string zipedName = "";
            string zipedPath = "";
            List<string> fileToZip = null;
            List<string> fileToZipFullPath = new List<string>();
            List<ABInfo> deleteAbList = new List<ABInfo>();
            EditorUtility.DisplayProgressBar("Zip Some Streaming AssetBundle File", "", 0.09f);

            for (int i = 0; i < zipFiles.Count; i++)
            {
                fileToZip = zipFiles[i];
                zipedName = string.Format(zipGroupName, i);
                zipedPath = Path.Combine(CUtils.realStreamingAssetsPath, zipedName);
                fileToZipFullPath.Clear();
                foreach (var p in fileToZip)
                {
                    string ab = CUtils.GetRightFileName(p);
                    deleteAbList.Add(new ABInfo(ab, 0, 0, 0));
                    fileToZipFullPath.Add(Path.Combine(CUtils.realStreamingAssetsPath, ab));
                }

                EditorUtility.DisplayProgressBar("Zip AssetBundle File ", zipedName, (float)i / (float)zipFiles.Count);

                if (ZipHelper.CreateZip(zipedPath, fileToZipFullPath))
                    outZips.Add(zipedName);

                //delete source assetbundle 
                DeleteStreamingFiles(deleteAbList);
            }

            var zipins = ZipConfigs.CreateInstance();
            zipins.zipFiles = outZips;
            EditorUtility.ClearProgressBar();

        }

        #endregion


        #region private

        private static void CopyFileToSplitFolder(Dictionary<string, ABInfo> updateList)
        {
            string updateOutPath = Path.Combine(UpdateOutPath, ResFolderName);//总的资源目录
            // ExportResources.CheckDirectory(updateOutPath);
            string verPath = Path.Combine(UpdateOutVersionPath, ResFolderName);//特定版本资源目录用于资源备份
            // ExportResources.CheckDirectory(verPath);

            int allLen = updateList.Count;
            int i = 0;
            Debug.LogFormat("CopyFileToSplitFolder.Count = {0}", updateList.Count);
            EditorUtility.DisplayProgressBar("Copy Change AssetBundle File", "copy file to " + updateOutPath + updateList.Count, 0.09f);

            string sourcePath;
            string outfilePath, outfileVerionPath;
            string key, extension;
            uint crc = 0;
            StringBuilder erro = new StringBuilder();
            foreach (var k in updateList)
            {
                key = k.Key;//CUtils.GetAssetBundleName(k.Key);
                sourcePath = Path.Combine(Application.dataPath, k.Value.assetPath);
                if (string.IsNullOrEmpty(k.Value.assetPath)) continue;
                if (!File.Exists(sourcePath)) //
                {
                    string e = string.Format("copy file ({0}) not Exists ", sourcePath);
                    Debug.LogWarning(e);
                    erro.AppendLine(e);
                    continue;
                }
                extension = System.IO.Path.GetExtension(key);
                crc = (uint)k.Value.crc32;
                if (crc != 0)
                {
                    if (string.IsNullOrEmpty(extension))
                    {
                        key = InsertAssetBundleName(key + Common.CHECK_ASSETBUNDLE_SUFFIX, "_" + crc.ToString());
                    }
                    else if (extension == Common.DOT_BYTES)
                    {
                        key = key.Replace(extension, Common.CHECK_ASSETBUNDLE_SUFFIX);
                        key = InsertAssetBundleName(key, "_" + crc.ToString());// 
                    }
                    else
                        key = InsertAssetBundleName(key, "_" + crc.ToString());// 
                }
                outfilePath = Path.Combine(updateOutPath, key);
                outfileVerionPath = Path.Combine(verPath, key);
                // Debug.LogFormat("{0} copy to {1}", outfilePath, outfileVerionPath);
                //
                uint filelen = 0;
                uint copyFileCrc = 0;

                var resType = HugulaSetting.instance.backupResType;
                if (resType == CopyResType.VerResFolder)
                {
                    FileHelper.CheckCreateFilePathDirectory(outfileVerionPath);
                    File.Copy(sourcePath, outfileVerionPath, true);// copy to v{d}/res folder
                    copyFileCrc = CrcCheck.GetLocalFileCrc(outfileVerionPath, out filelen);
                }

                if (resType == CopyResType.OneResFolder)
                {
                    FileHelper.CheckCreateFilePathDirectory(outfilePath);
                    File.Copy(sourcePath, outfilePath, true);// copy to /res folder
                    copyFileCrc = CrcCheck.GetLocalFileCrc(outfilePath, out filelen);
                }

                //check file crc
                if (copyFileCrc != crc)
                {
                    string e = string.Format("crc(source{0}!=copy{1}),path={2}", crc, copyFileCrc, outfilePath);
                    Debug.LogError(e);
                    erro.AppendLine(e);
                }
                EditorUtility.DisplayProgressBar("copy file to split folder " + updateOutPath, " copy file  =>" + i.ToString() + "/" + allLen.ToString(), i / allLen);
                i++;
            }
            Debug.Log(" copy  file complete!");
            EditorUtility.ClearProgressBar();
            string errContent = erro.ToString();
            if (!string.IsNullOrEmpty(errContent))
            {
                string tmpPath = BuildScript.GetAssetTmpPath();
                ExportResources.CheckDirectory(tmpPath);
                string outPath = Path.Combine(tmpPath, "error.txt");
                Debug.Log("write to path=" + outPath);
                using (StreamWriter sr = new StreamWriter(outPath, true))
                {
                    sr.WriteLine(" Error : " + System.DateTime.Now.ToString());
                    sr.Write(errContent);
                }
            }
        }

        #endregion

        private static string InsertAssetBundleName(string assetbundleName, string insert)
        {
            var append = HugulaSetting.instance.appendCrcToFile;
            if (append)
            {
                var str = CUtils.InsertAssetBundleName(assetbundleName, insert);
                return str;
            }
            else
            {
                return assetbundleName;
            }

        }
    }
}
