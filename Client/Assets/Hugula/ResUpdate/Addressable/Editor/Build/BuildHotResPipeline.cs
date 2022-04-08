using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Hugula;
using Hugula.ResUpdate;
using Hugula.Utils;
using HugulaEditor.Addressable;

namespace HugulaEditor.ResUpdate
{

    public class HotResGenSharedData
    {
        public Dictionary<string, FolderManifest> allFolderManifest;
        public List<FolderManifest> firstFolderManifest = null;//首包
        public List<FolderManifest> streamingFolderManifest = null;//streamingAsset
        public List<FolderManifest> diffFolderManifest = null;//变更包
        public List<FileResInfo>[] abInfoArray = null;

        public FolderManifest FindFolderManifestByFolderName(List<FolderManifest> list, string folderName)
        {
            if (list == null) return null;
            foreach (var item in list)
            {
                if (item.folderName == folderName)
                    return item;
            }
            return null;
        }

        public uint diff_crc;
    }

    //构建热更新资源
    public class BuildResPipeline
    {
        private static Dictionary<string, FolderManifest> m_AllFolderManifest;
        public static void Initialize(Dictionary<string, FolderManifest> allFolderManifest)
        {
            m_AllFolderManifest = allFolderManifest;
            TaskManager<HotResGenSharedData>.AddTask(new ClearHotResCache());
            TaskManager<HotResGenSharedData>.AddTask(new BuildCustomPackage());
            TaskManager<HotResGenSharedData>.AddTask(new BuildLocalStreamingFolderManifest());
            TaskManager<HotResGenSharedData>.AddTask(new ReadFolderManifestInfo());
            TaskManager<HotResGenSharedData>.AddTask(new BuildDiffFolderManifest());
            TaskManager<HotResGenSharedData>.AddTask(new CopyDiffFolderManifestAndRes());

            TaskManager<HotResGenSharedData>.AddTask(new GenVersionJson());
        }

        public static void Build()
        {
            var resData = new HotResGenSharedData();
            resData.allFolderManifest = m_AllFolderManifest;
            TaskManager<HotResGenSharedData>.Run(resData, (float p, string name) => UnityEditor.EditorUtility.DisplayProgressBar(name, $"run {name}", p));
        }

    }



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

    //构建本地包
    public class BuildLocalStreamingFolderManifest : ITask<HotResGenSharedData>
    {
        public string name { get { return "build all local streaming folderManifest"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            string verPath = BuildConfig.UpdateResOutVersionPath;
            var folderManifestDic = data.allFolderManifest;
            string zipFilePath = string.Empty;

            List<string> folderManifest = new List<string>();
            foreach (var folder in folderManifestDic.Values)
            {
                zipFilePath = Path.Combine(verPath, BuildConfig.GetTmpZipName(folder.folderName));
                uint fileLen = 0;
                var zipCrc = CrcCheck.GetLocalFileCrc(zipFilePath, out fileLen); //读取zip包size
                folder.zipSize = fileLen;
                folder.zipVersion = zipCrc.ToString();
                var zipFile = new FileInfo(zipFilePath);
                if (zipFile.Exists)
                {
                    folder.zipName = string.Empty;
                    var newZipName = Path.Combine(verPath, folder.zipName + ".zip");
                    if (File.Exists(newZipName))
                    {
                        File.Delete(newZipName);
                    }
                    zipFile.MoveTo(newZipName);
                    Debug.Log($"change filename:{zipFilePath} to:{newZipName}");
                }

                var strAsset = $"Assets/Tmp/{folder.folderName}.asset";
                folder.WriteToFile($"Assets/Tmp/folderManifest_{folder.folderName}.txt");
                folder.SaveAsset(strAsset);
                folderManifest.Add(strAsset);
            }

            //构建所有foldermanifest
            BuildScriptHotResUpdate.BuildABsTogether(folderManifest.ToArray(), null, Hugula.Utils.Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions);

        }
    }

    //读取首包与本地包内容
    public class ReadFolderManifestInfo : ITask<HotResGenSharedData>
    {
        public string name { get { return "Read first and streaming foldermanifest to compare"; } }
        public int priority { get { return 0; } }

        public void Run(HotResGenSharedData data)
        {
            var firstPath = Path.Combine(BuildConfig.CurrentUpdateResOutPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//首包folderManifest路径
            var streamingPath = Path.Combine(CUtils.realStreamingAssetsPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//folderManifest路径
                                                                                                                              //读取首包
            AssetBundle ab = null;
            if (File.Exists(firstPath) && (ab = AssetBundle.LoadFromFile(firstPath)) != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                data.firstFolderManifest = new List<FolderManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + firstPath);

                Debug.LogFormat("Load UpdateResFolder  folderManifest {0} is done !\r\n ManifestManager.firstFolderManifest.count = {1}", firstPath, data.firstFolderManifest.Count);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].ToString());
                }
                ab.Unload(false);
            }
            else
            {
                Debug.Log($"copy streamingAsset({streamingPath}) 到热更新目录:{firstPath}");
                FileHelper.CheckCreateFilePathDirectory(firstPath);
                File.Copy(streamingPath, firstPath);
            }
            // Debug.LogWarning ($"there is no folderManifest in {path} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

            //读取本地包
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath)) != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                data.streamingFolderManifest = new List<FolderManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].ToString());
                }
                ab.Unload(false);
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");


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
            List<FolderManifest> diffFolderManifest = new List<FolderManifest>();
            for (int i = 0; i < streamingManifest.Count; i++)
            {
                var itemFolderManifest = streamingManifest[i];
                var firstFind = data.FindFolderManifestByFolderName(data.firstFolderManifest, itemFolderManifest.folderName);
                List<FileResInfo> diffInfos = null;
                if (firstFind != null)
                {
                    diffInfos = firstFind.Compare(itemFolderManifest);
                }
                else
                {
                    //没有首包不需要更新文件
                    diffInfos = new List<FileResInfo>();
                }
                var diffItemFolderManifest = FolderManifestRuntionExtention.Create(itemFolderManifest.folderName);
                diffItemFolderManifest.allFileInfos = diffInfos;
                diffItemFolderManifest.zipSize = itemFolderManifest.zipSize;
                diffItemFolderManifest.zipVersion = itemFolderManifest.zipVersion;
                diffFolderManifest.Add(diffItemFolderManifest);
                if (diffInfos.Count > 0)
                    Debug.Log($"变更的文件信息:{diffItemFolderManifest.ToString()} ");
            }

            data.diffFolderManifest = diffFolderManifest;

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
            string source, target;
            foreach (var folderManifest in diffFolderManifest)
            {
                foreach (var fileList in folderManifest.allFileInfos)
                {
                    source = buildBundlePathData.GetBundleBuildPath(fileList.name);
                    if (source != null)
                    {
                        target = Path.Combine(verPath, fileList.name);
                        if (!CopyFileTo(source, target))
                        {

                        }
                    }
                }
            }

            //构建变更folderManifest文件
            List<string> folderManifestAssets = new List<string>();
            foreach (var folderManifest in diffFolderManifest)
            {
                var strAsset = $"Assets/Tmp/{folderManifest.folderName}.asset";
                folderManifest.SaveAsset(strAsset);
                folderManifestAssets.Add(strAsset);
            }

            //单独构建所有foldermanifest到当前版本热更新目录
            BuildScriptHotResUpdate.BuildABsTogether(folderManifestAssets.ToArray(), verPath, Hugula.Utils.Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, BuildScriptHotResUpdate.DefaultBuildAssetBundleOptions);
            //读取manifest的crc值
            string old_name = Path.Combine(BuildConfig.UpdateResOutVersionPath, Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);
            var crc32 = CrcCheck.GetLocalFileCrc(old_name, out var le);
            data.diff_crc = crc32;
            var newName = CUtils.InsertAssetBundleName(Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, $"_{data.diff_crc}");
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
            string verPath = BuildConfig.UpdateResOutVersionPath;//Path.Combine(BuildConfig.UpdateResOutVersionPath, BuildConfig.ResFolderName);//特定版本资源目录用于资源备份
            FileHelper.CheckCreateDirectory(verPath);
            var aasBuildPath = Path.Combine(UnityEngine.AddressableAssets.Addressables.BuildPath, BuildConfig.BuildTarget.ToString());

            //构建打包自定义包内容
            List<string> fileToZipFullPath = new List<string>();
            var streamingFolderManifest = data.allFolderManifest.Values;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            BuildBundlePathData buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (folderManifest.folderName == exception) continue; //streaming目录默认不打包
                var zipName = BuildConfig.GetTmpZipName(folderManifest.folderName);//   folderManifest.zipName;
                var zipOutPath = Path.Combine(verPath, zipName);
                fileToZipFullPath.Clear();
                foreach (var file in folderManifest.allFileInfos)
                {
                    fileToZipFullPath.Add(buildBundlePathData.GetBundleBuildPath(file.name));//Path.Combine(aasBuildPath, file.name));
                }
                if (File.Exists(zipOutPath)) File.Delete(zipOutPath);
                ZipHelper.CreateZip(zipOutPath, fileToZipFullPath, aasBuildPath);
                Debug.Log($"zip 文件{zipOutPath} 打包成功");
            }

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
            CodeVersion.APP_NUMBER = 0;

            string verPath = BuildConfig.UpdateResOutVersionPath;
            string rootPath = BuildConfig.CurrentUpdateResOutPath;


            //当前目录配置文件
            var configPath = Path.Combine(BuildConfig.VersionConfigPath, CUtils.platform + ".json");
            var configDevPath = Path.Combine(BuildConfig.VersionConfigPath, CUtils.platform + "_dev" + ".json");
            var config = JsonUtility.FromJson<VerionConfig>(File.ReadAllText(configPath));
            var configDev = JsonUtility.FromJson<VerionConfig>(File.ReadAllText(configDevPath));
            FastMode fastMode = config.fast;
            if (data.firstFolderManifest.Count > 0) fastMode = FastMode.sync;
            var versionConfig = new VerionConfig();
            versionConfig.version = CodeVersion.APP_VERSION;
            versionConfig.code = CodeVersion.CODE_VERSION;
            versionConfig.time = CUtils.ConvertDateTimeInt(System.DateTime.Now);
            //release配置的变量
            versionConfig.cdn_host = config.cdn_host;
            versionConfig.update_url = config.update_url;
            versionConfig.manifest_name = config.manifest_name;
            versionConfig.fast = fastMode;
            var outConfig = ReplaceTemplate(JsonUtility.ToJson(versionConfig), data.diff_crc);
            var savePath = Path.Combine(verPath, "version.json");
            File.WriteAllText(savePath, outConfig);
            Debug.Log($"保存当前版本:{savePath}");
            var rootVersionPath = Path.Combine(rootPath, "version.json");
            File.WriteAllText(rootVersionPath, outConfig);
            Debug.Log($"刷新根目录配置:{rootVersionPath}");

            //保存本地配置信息

#if HUGULA_RELEASE
                var verOutPath = Path.Combine(BuildConfig.VersionLocalOutPath,"version.txt");
                File.WriteAllText(verOutPath,outConfig);
#endif

            //dev配置
            fastMode = configDev.fast;
            if (data.firstFolderManifest.Count > 0) fastMode = FastMode.sync;
            versionConfig.cdn_host = configDev.cdn_host;
            versionConfig.update_url = configDev.update_url;
            versionConfig.manifest_name = configDev.manifest_name;
            versionConfig.fast = fastMode;
            var outDevConfig = ReplaceTemplate(JsonUtility.ToJson(versionConfig), data.diff_crc);
            savePath = Path.Combine(verPath, $"version_dev.json");
            File.WriteAllText(savePath, outDevConfig);
            Debug.Log($"保存当前版本:{savePath}");
            rootVersionPath = Path.Combine(rootPath, "version_dev.json");
            File.WriteAllText(rootVersionPath, outDevConfig);
            Debug.Log($"刷新根目录配置:{rootVersionPath}");

            //本地配置用于zip包下载等
#if !HUGULA_RELEASE
            var verOutPath = Path.Combine(BuildConfig.VersionLocalOutPath, "version.txt");
            File.WriteAllText(verOutPath, outDevConfig);
#endif

        }

        string ReplaceTemplate(string content, uint crc)
        {
            content = content.Replace("{app.platform}", CUtils.platform);
            content = content.Replace("{app.version}", CodeVersion.APP_VERSION);
            content = content.Replace("{app.number}", CodeVersion.APP_NUMBER.ToString());

            content = content.Replace("{app.manifest_crc}", crc.ToString());
            content = content.Replace("{app.manifest_name}", Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);
            var full_manifest_name = CUtils.InsertAssetBundleName(Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME, $"_{crc.ToString()}");
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
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath)) != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                data.streamingFolderManifest = new List<FolderManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].ToString());
                }
                ab.Unload(false);
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

            //读取当前打包bundle构建路径配置信息
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            Debug.Log(JsonUtility.ToJson(buildBundlePathData));

            var streamingFolderManifest = data.streamingFolderManifest;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            string filePath = string.Empty;
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (folderManifest.name == exception) continue; //streaming目录默认不打包
                foreach (var file in folderManifest.allFileInfos)
                {
                    filePath = buildBundlePathData.GetBundleBuildPath(file.name);
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Debug.Log($"delete file：{filePath}");
                    }
                }
            }
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
            if (File.Exists(streamingPath) && (ab = AssetBundle.LoadFromFile(streamingPath)) != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                data.streamingFolderManifest = new List<FolderManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no folderManifest in StreamingAssetsPath " + streamingPath);

                Debug.LogFormat("Load streamingAsset folderManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", streamingPath, data.streamingFolderManifest.Count);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].ToString());
                }
                ab.Unload(false);
            }
            else
                Debug.LogWarning($"there is no folderManifest in {streamingPath} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");

            //读取当前打包bundle构建路径配置信息
            var buildBundlePathData = BuildBundlePathData.ReadBuildBundlePathData();
            Debug.Log(JsonUtility.ToJson(buildBundlePathData));

            var streamingFolderManifest = data.streamingFolderManifest;
            var exception = UnityEditor.AddressableAssets.Settings.GroupSchemas.HugulaResUpdatePacking.PackingType.streaming.ToString();
            string filePath = string.Empty;
            string pad_build_path = string.Empty;
            foreach (var folderManifest in streamingFolderManifest)
            {
                if (PAD_CONFIG.TryGetValue(folderManifest.folderName, out pad_build_path))
                {
                    pad_build_path = Path.Combine(PAD_BUILD_PATH, pad_build_path);
                    EditorUtils.CheckDirectory(pad_build_path);
                    EditorUtils.DeleteFilesException(pad_build_path, ".gradle", false);
                    foreach (var file in folderManifest.allFileInfos)
                    {
                        filePath = buildBundlePathData.GetBundleBuildPath(file.name);
                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            var destFile = Path.Combine(pad_build_path, file.name);
                            File.Copy(filePath, destFile);
                            Debug.Log("aab copy：{filePath} to:{destFile}");
                        }
                    }
                }

            }
        }
    }
    #endregion
}