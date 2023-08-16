using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

using Hugula;
using Hugula.ResUpdate;
using Hugula.Utils;
using HugulaEditor.Addressable;

namespace HugulaEditor.ResUpdate
{

    public class HotResGenSharedData
    {
        public Dictionary<string, FileManifest> allFolderManifest;
        public List<FileManifest> firstFolderManifest = null;//首包
        public List<FileManifest> streamingFolderManifest = null;//streamingAsset
        public List<FileManifest> diffFolderManifest = null;//变更包
        public List<FileResInfo>[] abInfoArray = null;
        //增量更新
        internal bool PreviousContentUpdate = false;

        public FileManifest FindFolderManifestByFolderName(List<FileManifest> list, string folderName)
        {
            if (list == null) return null;
            foreach (var item in list)
            {
                if (item.fileName == folderName)
                    return item;
            }
            return null;
        }

    }

    //构建热更新资源
    public class BuildResPipeline
    {
        // private static Dictionary<string, FileManifest> m_AllFolderManifest;
        // private static bool m_PreviousContentUpdate = false;
        public static void Initialize()
        {
            TaskManager<HotResGenSharedData>.Clear();
            TaskManager<HotResGenSharedData>.AddTask(new ClearHotResCache());
            TaskManager<HotResGenSharedData>.AddTask(new CopyContentStateData());
            TaskManager<HotResGenSharedData>.AddTask(new BuildCustomPackage());
            TaskManager<HotResGenSharedData>.AddTask(new ReadFirstFolderManifestInfo());
            TaskManager<HotResGenSharedData>.AddTask(new BuildDiffBundleManifest());
            TaskManager<HotResGenSharedData>.AddTask(new BuildLocalStreamingFolderManifest());
            TaskManager<HotResGenSharedData>.AddTask(new BuildDiffFolderManifest());
            TaskManager<HotResGenSharedData>.AddTask(new CopyDiffFolderManifestAndRes());

            TaskManager<HotResGenSharedData>.AddTask(new GenVersionJson());
        }

        public static void Build(Dictionary<string, FileManifest> allFolderManifest, bool PreviousContentUpdate)
        {
            Hugula.Utils.CUtils.DebugCastTime($"Run Task BuildResPipeline.Build()");
            var resData = new HotResGenSharedData();
            resData.allFolderManifest = allFolderManifest;
            resData.PreviousContentUpdate = PreviousContentUpdate;
            TaskManager<HotResGenSharedData>.Run(resData, (float p, string name) => UnityEditor.EditorUtility.DisplayProgressBar(name, $"run {name}", p));
        }

    }

    #region lua 构建
    public class BuildLuaBundle : ITask<HotResGenSharedData>
    {
        public static string GetLuaBytesResourcesPath()
        {
            return $"{Common.LUACFOLDER}";
        }

        public static string GetLuaStreamingPath()
        {
            return Path.Combine(CUtils.GetRealStreamingAssetsPath(), Common.LUA_BUNDLE_NAME);
        }

        ///获取导出后的lua bytes文件列表
        public string[] GetOutLuaBytesFileList()
        {
            string OutLuaBytesPath = GetLuaBytesResourcesPath();

            var files = Directory.GetFiles(OutLuaBytesPath, "*.bytes", SearchOption.AllDirectories);
            var dests = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                dests[i] = Path.Combine(OutLuaBytesPath, Path.GetFileName(files[i]));
            }

            return dests;
        }

        public string name { get { return $"build lua bundle ({Common.LUACFOLDER})"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            var allLuaBytesFiles = GetOutLuaBytesFileList();

#if USE_LUA_SEPARATELY
            var zipOutPath = Path.Combine(CUtils.realStreamingAssetsPath, Common.LUAFOLDER_NAME);
            if (Directory.Exists(zipOutPath)) Directory.Delete(zipOutPath, true);
            BuildScriptHotResUpdate.PackSeparatelyZipToOutPath(allLuaBytesFiles, zipOutPath, GetLuaBytesResourcesPath(), Common.BUNDLE_OFF_STR);
#else

            BuildScriptHotResUpdate.BuildABsTogether(allLuaBytesFiles, CUtils.GetRealStreamingAssetsPath(), Common.LUA_BUNDLE_NAME, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions, BuildConfig.GetOffsetData());
#endif
        }

        public BundleManifest GenStreamingLuaBundleManifest()
        {
            var bundleManifest = FolderManifestExtention.CreateBundleManifest(Common.LUA_BUNDLE_NAME);
            var allLuaBytesFiles = GetOutLuaBytesFileList();
            bundleManifest.assetFolderPath = GetLuaBytesResourcesPath();

            foreach (var p in allLuaBytesFiles)
            {
                BuildScriptHotResUpdate.AddToBundleManifest(p, bundleManifest);
            }
            //
            return bundleManifest;
        }


    }

    #endregion

    #region 构建流程
    //清理缓存
    public class ClearHotResCache : ITask<HotResGenSharedData>
    {
        public string name { get { return "clear hot resource update folder"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            string verPath = BuildConfig.UpdateResOutVersionPath;
            //Debug.Log($"清理本地热更新缓存:{verPath}");
            //EditorUtils.DirectoryDelete(verPath);  需要保留zip文件
        }
    }

    public class CopyContentStateData : ITask<HotResGenSharedData>
    {
        public string name { get { return "copy Content State Data (addressables_content_state.bin) "; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            var contentStatePath = UnityEditor.AddressableAssets.Build.ContentUpdateScript.GetContentStateDataPath(false);
            if (!data.PreviousContentUpdate)
            {
                var updateContentStateDataPath = BuildConfig.UpdateContentStateDataPath;
                if (!Directory.Exists(updateContentStateDataPath))
                    Directory.CreateDirectory(updateContentStateDataPath);
                string verPath = Path.Combine(updateContentStateDataPath, $"{CodeVersion.APP_VERSION}_{EditorUtils.GetResNumber()}_addressables_content_state.bin");

                File.Copy(contentStatePath, verPath,true);
                Debug.Log($" File.Copy({contentStatePath},{verPath})");
            }
            else
            {
                Debug.Log($"PreviousContentUpdate does't need File.Copy {contentStatePath};");
            }
        }
    }

    //构建本地包
    public class BuildLocalStreamingFolderManifest : ITask<HotResGenSharedData>
    {
        public string name { get { return "build all local streaming folderManifest"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            string verPath = BuildConfig.UpdatePackagesOutVersionPath;
            var folderManifestDic = data.allFolderManifest;
            string zipFilePath = string.Empty;

            List<string> folderManifest = new List<string>();
            List<FileManifest> folderManifestBuild = new List<FileManifest>();
            // FolderManifest folderMani = null;
            foreach (var folder in folderManifestDic.Values)
            {
                if (folder is FolderManifest)
                {
                    var folderMani = folder as FolderManifest;

                    zipFilePath = Path.Combine(verPath, BuildConfig.GetTmpZipName(folderMani.fileName));
                    uint fileLen = 0;
                    var zipCrc = CrcCheck.GetLocalFileCrc(zipFilePath, out fileLen); //读取zip包size
                    folderMani.zipSize = fileLen;
                    folderMani.zipVersion = zipCrc.ToString();
                    var zipFile = new FileInfo(zipFilePath);
                    if (zipFile.Exists)
                    {
                        folderMani.zipName = string.Empty;
                        var newZipName = Path.Combine(verPath, folderMani.zipName + ".zip");
                        if (File.Exists(newZipName))
                        {
                            File.Delete(newZipName);
                        }
                        zipFile.MoveTo(newZipName);
                        Debug.Log($"change filename:{zipFilePath} to:{newZipName}");
                    }

                    var strAsset = $"Assets/Tmp/{folder.fileName}.asset";
                    folderMani.WriteToFile($"Assets/Tmp/local_{folder.fileName}.txt");
                    folderMani.SaveAsset(strAsset);
                    folderManifest.Add(strAsset);
                    folderManifestBuild.Add(folderMani);
                }
                else if (folder is BundleManifest)
                {
                    var bundleManifest = folder as BundleManifest;

                    var strAsset = $"Assets/Tmp/{folder.fileName}.asset";
                    bundleManifest.WriteToFile($"Assets/Tmp/local_lua_{folder.fileName}.txt");
                    bundleManifest.SaveAsset(strAsset);
                    folderManifest.Add(strAsset);
                    folderManifestBuild.Add(bundleManifest);

                }

            }

            //构建所有foldermanifest

            BuildScriptHotResUpdate.BuildABsTogether(folderManifest.ToArray(), null, Hugula.Utils.Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions, BuildConfig.GetOffsetData());
            new ReadFirstFolderManifestInfo().Run(data);
            ReadStreamingFolderManifest(data);

        }

        // 读取本地foldermanifest信息

        public void ReadStreamingFolderManifest(HotResGenSharedData data)
        {
            AssetBundle ab = null;
            var firstPath = Path.Combine(BuildConfig.CurrentUpdateResOutPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//首包folderManifest路径
                                                                                                                               //读取本地包
            var streamingPath = Path.Combine(CUtils.realStreamingAssetsPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//folderManifest路径
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath, 0, Common.BUNDLE_OFF_SET)) != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                data.streamingFolderManifest = new List<FileManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                // for (int i = 0; i < assets.Length; i++)
                // {
                //     Debug.Log(assets[i].ToString());
                // }
                ab.Unload(false);

                if (data.firstFolderManifest == null)
                {
                    FileHelper.CheckCreateFilePathDirectory(firstPath);
                    File.Copy(streamingPath, firstPath);
                    Debug.Log($"copy streamingAsset({streamingPath}) 到热更新目录:{firstPath}");
                }
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

        }
    }

    //读取首包内容
    public class ReadFirstFolderManifestInfo : ITask<HotResGenSharedData>
    {
        public string name { get { return "Read first foldermanifest to compare"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            var firstPath = Path.Combine(BuildConfig.CurrentUpdateResOutPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//首包folderManifest路径
                                                                                                                               //读取首包
            AssetBundle ab = null;
            if (File.Exists(firstPath) && (ab = AssetBundle.LoadFromFile(firstPath, 0, Common.BUNDLE_OFF_SET)) != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                if (assets.Length == 0)
                {
                    Debug.LogError("there is no folderManifest at " + firstPath);
                    //如果没有内容删除首包
                    ab.Unload(true);
                    File.Delete(firstPath);
                    Debug.LogError("delete first folderManifest : " + firstPath);

                    return;
                }
                data.firstFolderManifest = new List<FileManifest>(assets);

                Debug.LogFormat("Load UpdateResFolder  folderManifest {0} is done !\r\n ManifestManager.firstFolderManifest.count = {1}", firstPath, data.firstFolderManifest.Count);
                string version = CodeVersion.APP_VERSION;
                FileManifest fileManifest;
                for (int i = 0; i < assets.Length; i++)
                {
                    fileManifest = assets[i];
                    // Debug.Log(fileManifest.ToString());
                    version = fileManifest.version;
                }

                ab.Unload(false);

                if (CodeVersion.CODE_VERSION > CodeVersion.CovertVerToCodeVersion(version)) // 本地code版本号大于首包code version删除旧包所有内容
                {
                    HugulaEditor.EditorUtils.DirectoryDelete(BuildConfig.CurrentUpdateResOutPath);
                    Debug.Log($"Delete UpdateResFolder  Folder  {BuildConfig.CurrentUpdateResOutPath} is done!");
                }

            }
            else
            {
                Debug.Log($"there is no first folderManifest in {firstPath}");
                FileHelper.CheckCreateFilePathDirectory(firstPath);
                // File.Copy(streamingPath, firstPath);
            }
            // Debug.LogWarning ($"there is no folderManifest in {path} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

        }
    }

    //构建lua 增量更新
    public class BuildDiffBundleManifest : ITask<HotResGenSharedData>
    {
        public string name { get { return "Build diffrence bundleManifest"; } }
        public int priority { get { return 0; } }
        public void Run(HotResGenSharedData data)
        {
            var sb = new System.Text.StringBuilder();
            var firstFolderManifest = data.firstFolderManifest;
            var streamingManifest = new List<BundleManifest>();
            var streamingName = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();

            foreach (var s in data.allFolderManifest.Values)
            {
                if (s is BundleManifest)
                {
                    streamingManifest.Add((BundleManifest)s);
                }
            }


            List<BundleManifest> diffFolderManifest = new List<BundleManifest>();
            foreach (var manifest in streamingManifest)
            {
                // var itemFolderManifest = manifest;
                Debug.Log($"Build diffrence bundleManifest: {manifest.fileName}");
                var firstFind = data.FindFolderManifestByFolderName(data.firstFolderManifest, manifest.fileName);
                List<FileResInfo> diffInfos = null;
                if (firstFind != null)
                {
                    diffInfos = firstFind.Compare(manifest);
                    var str = $"Build diffrence first BundleManifest:({manifest.fileName}) count:{diffInfos.Count}";
                    sb.AppendLine(str);
                    // Debug.Log(str);
                }
                else
                {
                    var str = $"Build diffrence first BundleManifest({manifest.fileName}) is't exist ";
                    sb.AppendLine(str);
                    // Debug.Log(str);
                    //没有首包全部记录
                    diffInfos = new List<FileResInfo>(); //manifest.allFileInfos;
                }

                var diffItemFolderManifest = manifest.CloneWithOutAllFileInfos();  //HugulaEditor.Addressable.FolderManifestExtention.CreateBundleManifest(itemFolderManifest.fileName);
                diffItemFolderManifest.allFileInfos = diffInfos;
                if (diffInfos.Count > 0)
                {
                    diffFolderManifest.Add(diffItemFolderManifest);
                    var str = $"Build diffrence diffItemFolderManifest:变更的文件信息:{diffItemFolderManifest.ToString()} ";
                    Debug.Log(str);
                    sb.AppendLine(str);
                }
                else
                {
                    var str = $"Build diffrence diffItemFolderManifest:没有变更文件:{diffItemFolderManifest.ToString()}";
                    Debug.Log(str);
                    sb.AppendLine(str);

                }
            }

            //根据差异文件生成bundle
            List<string> assets = new List<string>();

            var file = string.Empty;
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            foreach (var folder in diffFolderManifest)
            {
                assets.Clear();
                sb.AppendLine($"\r\n {folder.fileName}  diffFileInfos:\r\n ");
                foreach (var f in folder.allFileInfos)
                {
                    assets.Add(Path.Combine(folder.assetFolderPath, f.name + Common.DOT_BYTES));
                    sb.AppendLine(assets[assets.Count - 1]);
                }

                var fileName = CUtils.GetPersistentBundleFileName(folder.fileName);
#if USE_LUA_SEPARATELY
                if (folder.fileName.Equals(Common.LUA_BUNDLE_NAME))
                {
                        file = Path.Combine("Assets/Tmp", fileName);
                    //BuildScriptHotResUpdate.PackSeparatelyZipToOutPath(assets.ToArray(), file, folder.assetFolderPath, Common.BUNDLE_OFF_STR);
                    BuildScriptHotResUpdate.BuildZipTogether(assets.ToArray(), file, folder.assetFolderPath, Common.BUNDLE_OFF_STR);
                }
                else
#endif
                {
                BuildScriptHotResUpdate.BuildABsTogether(assets.ToArray(), "Assets/Tmp", fileName, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions, BuildConfig.GetOffsetData());
                file = Path.Combine("Assets/Tmp", fileName);
                }

                FolderManifest findStreaming = null;
                if (data.allFolderManifest.TryGetValue(streamingName, out var find))
                {
                    findStreaming = find as FolderManifest;
                    BuildScriptHotResUpdate.AddToFolderManifest(file, findStreaming, buildBundlePathData);
                    Debug.Log($"BuildDiffBundleManifest:AddToFolderManifest {file} to : {findStreaming} ");
                }
                else
                {
                    Debug.LogError($"BuildDiffBundleManifest: {fileName} , streaming folder asset  doest find! ");
                }
            }

            BuildBundlePathData.SerializeBuildBundlePathData(buildBundlePathData);
            EditorUtils.WriteToTmpFile(name + "_log.txt", sb.ToString());
        }
    }

    //对比差异文件生成差异配置
    public class BuildDiffFolderManifest : ITask<HotResGenSharedData>
    {
        public string name { get { return "Build diffrence folderManifest"; } }
        public int priority { get { return 0; } }
        public void Run(HotResGenSharedData data)
        {
            var firstFolderManifest = data.firstFolderManifest;
            var streamingManifest = data.streamingFolderManifest;

            List<FileManifest> diffFolderManifest = new List<FileManifest>();
            for (int i = 0; i < streamingManifest.Count; i++)
            {
                var itemFolderManifest = streamingManifest[i];
                var firstFind = data.FindFolderManifestByFolderName(firstFolderManifest, itemFolderManifest.fileName);
                List<FileResInfo> diffInfos = null;
                if (firstFind)
                {
                    diffInfos = Compare(firstFind, itemFolderManifest); //firstFind.Compare(itemFolderManifest);
                }
                else
                {
                    diffInfos = new List<FileResInfo>();
                }

                FileManifest diffItemFolderManifest = null;
                if (itemFolderManifest is FolderManifest)
                {
                    var newManifest = HugulaEditor.Addressable.FolderManifestExtention.Create(itemFolderManifest.fileName);
                    newManifest.allFileInfos = diffInfos;
                    newManifest.zipSize = ((FolderManifest)itemFolderManifest).zipSize;
                    newManifest.zipVersion = ((FolderManifest)itemFolderManifest).zipVersion;
                    diffFolderManifest.Add(newManifest);
                    diffItemFolderManifest = newManifest;
                }
                else if (itemFolderManifest is BundleManifest)
                {
                    var newManifest = HugulaEditor.Addressable.FolderManifestExtention.CreateBundleManifest(itemFolderManifest.fileName);
                    newManifest.allFileInfos = diffInfos;
                    diffFolderManifest.Add(newManifest);
                    diffItemFolderManifest = newManifest;
                }

                if (diffInfos.Count > 0)
                    Debug.Log($"modify manifest:{diffItemFolderManifest} count={diffInfos.Count} ");
            }

            data.diffFolderManifest = diffFolderManifest;

        }


        /// <summary>
        /// 用remote的foldermanifest对比本地 找出差异文件
        /// </summary>
        public static List<FileResInfo> Compare(FileManifest self, FileManifest remote)
        {
            List<FileResInfo> re = new List<FileResInfo>();
            if (remote == null) return re;
            if (self.resNumber > remote.resNumber) return re; //如果本地大于远端不需要更新
            var compareABInfos = remote.allFileInfos;
            FileResInfo abInfo;
            for (int i = 0; i < compareABInfos.Count; i++)
            {
                abInfo = compareABInfos[i];
                if (self.CheckFileIsChanged(abInfo) && abInfo.crc32 != 0)
                {
                    re.Add(abInfo);
                }
            }

            return re;
        }

    }


    ///<summary>
    /// copy差异文件到热更新目录
    /// 此处会构建新的assetbundle应该放到最后一步执行
    ///</summary>
    public class CopyDiffFolderManifestAndRes : ITask<HotResGenSharedData>
    {

        public string name { get { return "Copy diffrence folderManifest and assetbundle"; } }
        public int priority { get { return 0; } }
        public void Run(HotResGenSharedData data)
        {
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            string verPath = BuildConfig.UpdateResOutVersionPath;//Path.Combine(BuildConfig.UpdateResOutVersionPath, BuildConfig.ResFolderName);//特定版本资源目录用于资源备份
                                                                 //清理缓存
            FileHelper.CheckCreateDirectory(verPath);

            var aasBuildPath = UnityEngine.AddressableAssets.Addressables.BuildPath;

            //copy 变更的文件到更新目录
            var diffFolderManifest = data.diffFolderManifest;
            string target;
            foreach (var folderManifest in diffFolderManifest)
            {
                if (folderManifest is FolderManifest)
                {
                    foreach (var file in folderManifest.allFileInfos)
                    {
                        var source = buildBundlePathData.GetBundleBuildPath(file.name);
                        if (source != null)
                        {
                            if (file.crc32 == 0)
                                target = Path.Combine(verPath, file.name); //如果不需要校验
                            else
                                target = Path.Combine(verPath, CUtils.InsertAssetBundleName(file.name, $"_{file.crc32}"));

                            if (file.crc32 != 0 && source.crc != file.crc32)
                            {
                                Debug.LogError($"源文件:{source.fullBuildPath},crc:{source.crc}!=目标crc:{file.crc32},target:{target}");
                            }

                            if (!CopyFileTo(source.fullBuildPath, target))
                            {

                            }
                        }
                    }
                }
            }

            //构建变更folderManifest文件
            List<string> folderManifestAssets = new List<string>();
            foreach (var folderManifest in diffFolderManifest)
            {
                var strAsset = $"Assets/Tmp/{folderManifest.fileName}.asset";
                folderManifest.SaveAsset(strAsset);
                folderManifest.WriteToFile($"Assets/Tmp/remote_diff_{folderManifest.fileName}.txt");
                folderManifestAssets.Add(strAsset);
            }

            //单独构建所有foldermanifest到当前版本热更新目录
            var hotStreamingName = CUtils.InsertAssetBundleName(Hugula.Utils.Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, "+" + CodeVersion.APP_VERSION);
            BuildScriptHotResUpdate.BuildABsTogether(folderManifestAssets.ToArray(), verPath, hotStreamingName, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions, BuildConfig.GetOffsetData());
            //读取manifest的crc值
            string old_name = Path.Combine(verPath, hotStreamingName);
            var crc32 = CrcCheck.GetLocalFileCrc(old_name, out var le);
            UnityEditor.EditorPrefs.SetString($"DIFF_CRC_{Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME}", crc32.ToString());
            var newName = CUtils.InsertAssetBundleName(Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, $"_{crc32.ToString()}");
            CopyFileTo(old_name, Path.Combine(BuildConfig.UpdateResOutVersionPath, newName));
        }

        static bool CopyFileTo(string s, string t)
        {
            if (File.Exists(s))
            {
                if (File.Exists(t))
                {
                    File.Delete(t);
                    Debug.Log($"删除目标：{t}");
                }
                File.Copy(s, t);
                return true;
            }
            else
            {
                Debug.LogError($"原文件:{s}不存在无法copy到目标目录:{t}");
                return false;
            }

        }
    }

    //生成自定义zip包文件
    public class BuildCustomPackage : ITask<HotResGenSharedData>
    {

        public string name { get { return "Build Custom Package zip"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            if (data.PreviousContentUpdate)
            {
                Debug.Log($"PreviousContentUpdate does't need {name} ");
                return;
            }
            string verPath = BuildConfig.UpdatePackagesOutVersionPath;//Path.Combine(BuildConfig.UpdateResOutVersionPath, BuildConfig.ResFolderName);//特定版本资源目录用于资源备份
            FileHelper.CheckCreateDirectory(verPath);
            var aasBuildPath = Path.Combine(UnityEngine.AddressableAssets.Addressables.BuildPath, BuildConfig.BuildTarget.ToString());

            //构建打包自定义包内容
            List<string> fileToZipFullPath = new List<string>();
            var streamingFolderManifest = data.allFolderManifest.Values;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            BuildBundlePathData buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            var sb = new System.Text.StringBuilder();
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (folderManifest.fileName == exception || !(folderManifest is FolderManifest)) continue; //streaming目录默认不打包
                var zipName = BuildConfig.GetTmpZipName(folderManifest.fileName);//   folderManifest.zipName;
                var zipOutPath = Path.Combine(verPath, zipName);
                sb.AppendLine(zipName);
                sb.AppendLine($"    outpath={zipOutPath}");
                sb.AppendLine($"    zipCount={folderManifest.allFileInfos.Count}");
                fileToZipFullPath.Clear();
                BuildBundlePath buildBundlePath;
                foreach (var file in folderManifest.allFileInfos)
                {
                    buildBundlePath = buildBundlePathData.GetBundleBuildPath(file.name);
                    fileToZipFullPath.Add(buildBundlePath.fullBuildPath);//Path.Combine(aasBuildPath, file.name));
                    sb.AppendLine($"            {file}");
                }
                //address key:
                var mFolderManifest = folderManifest as FolderManifest;
                sb.AppendLine($"    addressCount={mFolderManifest.allAddressKeys.Count}");
                foreach(var address in mFolderManifest.allAddressKeys)
                {
                    sb.AppendLine($"            {address}");
                }
                if (File.Exists(zipOutPath)) File.Delete(zipOutPath);
                ZipHelper.CreateZip(zipOutPath, fileToZipFullPath, aasBuildPath);
                Debug.Log($"zip 文件{zipOutPath} 打包成功");
            }
            Debug.Log(sb.ToString());
        }
    }

    ///<summary>
    /// 生成版本对比文件
    /// version.json
    ///</summary>
    public class GenVersionJson : ITask<HotResGenSharedData>
    {
        public string name { get { return "generate version json file "; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            CodeVersion.CODE_VERSION = 0;
            // CodeVersion.APP_NUMBER = 0;

            string verPath = BuildConfig.UpdateResOutVersionPath;
            string rootPath = BuildConfig.CurrentUpdateResOutPath;

            //release 配置
            var outConfig = SaveVersionJson(data);

            //根目录review配置
            var rootVersionPath = Path.Combine(rootPath, $"review_{Common.CRC32_VER_FILENAME}");//Path.Combine(rootPath, CUtils.InsertAssetBundleName(Common.CRC32_VER_FILENAME,"_review"));
            File.WriteAllText(rootVersionPath, outConfig);
            Debug.Log($"刷新根目录配置:{rootVersionPath}");

            //dev配置
            SaveVersionJson(data, "dev_");

        }

        string SaveVersionJson(HotResGenSharedData data, string prefix = "")
        {
            string verPath = BuildConfig.UpdateResOutVersionPath;
            string rootPath = BuildConfig.CurrentUpdateResOutPath;
            string resourcesPath = BuildConfig.LocalResourcesUpdateResOutPath;
            FileHelper.CheckCreateDirectory(verPath);
            FileHelper.CheckCreateDirectory(rootPath);
            FileHelper.CheckCreateDirectory(resourcesPath);

            var configName = prefix + CUtils.platform + ".json";
            var saveFileName = prefix + Common.CRC32_VER_FILENAME; //CUtils.InsertAssetBundleName(Common.CRC32_VER_FILENAME,prefix);
            //当前配置文件
            var configPath = Path.Combine(BuildConfig.VersionConfigPath, configName);
            var config = JsonUtility.FromJson<VersionConfig>(File.ReadAllText(configPath));
            FastMode fastMode = config.fast;
            var versionConfig = new VersionConfig();
            versionConfig.version = CodeVersion.APP_VERSION;
            versionConfig.res_number = EditorUtils.GetResNumber(); // CodeVersion.APP_NUMBER;
            SetVersionResNumber(versionConfig);
            versionConfig.time = CUtils.ConvertDateTimeInt(System.DateTime.Now);
            //
            if (data.firstFolderManifest != null && data.firstFolderManifest.Count > 0) fastMode = FastMode.sync;
            versionConfig.cdn_host = config.cdn_host;
            versionConfig.update_url = config.update_url;
            versionConfig.manifest_name = config.manifest_name;
            versionConfig.fast = fastMode;
            var crc_str = UnityEditor.EditorPrefs.GetString($"DIFF_CRC_{Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME}");

            var outConfig = ReplaceTemplate(JsonUtility.ToJson(versionConfig), crc_str);

            string versionFileName = CUtils.InsertAssetBundleName(saveFileName, $"_{CodeVersion.APP_VERSION}");
            //保存单独版本文件
            var savePath = Path.Combine(verPath, versionFileName);
            File.WriteAllText(savePath, outConfig);
            Debug.Log($"保存当前版本:{savePath}");

            //本地缓存

            savePath = Path.Combine(resourcesPath, saveFileName);
            File.WriteAllText(savePath, outConfig);
            Debug.Log($"本地缓存Resrouces 目录:{savePath}");


            //覆盖全局配置
            var rootVersionPath = Path.Combine(rootPath, saveFileName);
            File.WriteAllText(rootVersionPath, outConfig);
            Debug.Log($"刷新根目录配置:{rootVersionPath}");
            return outConfig;
        }

        void SetVersionResNumber(VersionConfig versionConfig)
        {
            versionConfig.res_number = EditorUtils.GetResNumber();//CodeVersion.APP_NUMBER; //从svn获取版本号信息
        }

        string ReplaceTemplate(string content, string crc)
        {
            content = content.Replace("{app.platform}", CUtils.platform);
            content = content.Replace("{app.version}", CodeVersion.APP_VERSION);
            content = content.Replace("{app.res_ver_folder}", Common.RES_VER_FOLDER);

            content = content.Replace("{app.manifest_crc}", crc);
            content = content.Replace("{app.manifest_name}", Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);
            var full_manifest_name = CUtils.InsertAssetBundleName(Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, $"_{crc}");
            content = content.Replace("{app.manifest_full_name}", full_manifest_name);

            return content;
        }
    }

    ///<summary>
    /// 删除自定义包内容
    /// 打包之前调用
    ///</summary>
    public class DelFolderManifestFiles : ITask<HotResGenSharedData>
    {
        public string name { get { return "Del FolderManifest Files"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            var streamingPath = Path.Combine(CUtils.realStreamingAssetsPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//folderManifest路径
            AssetBundle ab = null;
            //读取本地包
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath, 0, Common.BUNDLE_OFF_SET)) != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                data.streamingFolderManifest = new List<FileManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                // for (int i = 0; i < assets.Length; i++)
                // {
                //     Debug.Log(assets[i].ToString());
                // }
                ab.Unload(false);
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

            //读取当前打包bundle构建路径配置信息
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            // Debug.Log(JsonUtility.ToJson(buildBundlePathData));

            var streamingFolderManifest = data.streamingFolderManifest;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            // string filePath = string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (folderManifest.name == exception || folderManifest is BundleManifest) continue; //streaming目录默认不打包
                foreach (var file in folderManifest.allFileInfos)
                {
                    var buildBundlePath = buildBundlePathData.GetBundleBuildPath(file.name);
                    if (buildBundlePath != null && File.Exists(buildBundlePath.fullBuildPath))
                    {
                        File.Delete(buildBundlePath.fullBuildPath);
                        sb.AppendLine($"delete file：{buildBundlePath.fullBuildPath}");
                    }
                }
            }

            Debug.Log(sb.ToString());
        }
    }

    //android aab 模式专用
    public class MoveDataToAppBundleFolder : ITask<HotResGenSharedData>
    {
        public string name { get { return "Move Data To AppBundle Folder"; } }
        public int priority { get { return 0; } }

        public Dictionary<string, string> PAD_CONFIG = new Dictionary<string, string>()
        {
          {"fast","custom_fastfollow.androidpack"}
        };

        public const string PAD_BUILD_PATH = "Assets/Hugula/ResUpdate/PAD_Build/";

        public void Run(HotResGenSharedData data)
        {
            var streamingPath = Path.Combine(CUtils.realStreamingAssetsPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//folderManifest路径
            AssetBundle ab = null;
            //读取本地包
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath, 0, Common.BUNDLE_OFF_SET)) != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                data.streamingFolderManifest = new List<FileManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                // for (int i = 0; i < assets.Length; i++)
                // {
                //     Debug.Log(assets[i].ToString());
                // }
                ab.Unload(false);
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

            //读取当前打包bundle构建路径配置信息
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            Debug.Log(JsonUtility.ToJson(buildBundlePathData));

            var streamingFolderManifest = data.streamingFolderManifest;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            // string filePath = string.Empty;
            string pad_build_path = string.Empty;
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (PAD_CONFIG.TryGetValue(folderManifest.fileName, out pad_build_path))
                {
                    pad_build_path = Path.Combine(PAD_BUILD_PATH, pad_build_path);
                    EditorUtils.CheckDirectory(pad_build_path);
                    EditorUtils.DeleteFilesException(pad_build_path, ".gradle", false);
                    foreach (var file in folderManifest.allFileInfos)
                    {
                        var buildBundlePath = buildBundlePathData.GetBundleBuildPath(file.name);
                        if (buildBundlePath != null && File.Exists(buildBundlePath.fullBuildPath))
                        {
                            var destFile = Path.Combine(pad_build_path, file.name);
                            File.Copy(buildBundlePath.fullBuildPath, destFile);
                            Debug.Log("aab copy：{filePath} to:{destFile}");
                        }
                    }
                }

            }
        }
    }


    #endregion
}