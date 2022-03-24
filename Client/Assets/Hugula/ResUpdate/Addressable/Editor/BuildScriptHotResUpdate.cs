using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using Hugula.ResUpdate;
using HugulaEditor.ResUpdate;

namespace HugulaEditor.Addressable
{
    /// <summary>
    /// In addition to the Default Build Script behavior (building AssetBundles), this script assigns Android bundled content to "install-time" or "on-demand" custom asset packs
    /// specified in <see cref="CustomAssetPackSettings"/>.
    ///
    /// We will create the config files necessary for creating an asset pack (see https://docs.unity3d.com/Manual/play-asset-delivery.html#custom-asset-packs).
    /// The files are:
    /// * An {asset pack name}.androidpack folder located in 'Assets/PlayAssetDelivery/Build/CustomAssetPackContent'
    /// * A 'build.gradle' file for each .androidpack folder. If this file is missing, Unity will assume that the asset pack uses "on-demand" delivery.
    ///
    /// Additionally we generate some files to store build and runtime data that are located in in 'Assets/PlayAssetDelivery/Build':
    /// * Create a 'BuildProcessorData.json' file to store the build paths and .androidpack paths for bundles that should be assigned to custom asset packs.
    /// At build time this will be used by the <see cref="PlayAssetDeliveryBuildProcessor"/> to relocate bundles to their corresponding .androidpack paths.
    /// * Create a 'CustomAssetPacksData.json' file to store custom asset pack information to be used at runtime. See <see cref="PlayAssetDeliveryInitialization"/>.
    ///
    /// We assign any content marked for "install-time" delivery to the generated asset packs. In most cases the asset pack containing streaming assets will use "install-time" delivery,
    /// but in large projects it may use "fast-follow" delivery instead. For more information see https://docs.unity3d.com/Manual/play-asset-delivery.html#generated-asset-packs.
    ///
    /// Because <see cref="AddressablesPlayerBuildProcessor"/> moves all Addressables.BuildPath content to the streaming assets path, any content in that directory
    /// will be included in the generated asset packs even if they are not marked for "install-time" delivery.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildScriptHotResUpdate.asset", menuName = "Addressables/Custom Build/Build HotResource Update")]
    public class BuildScriptHotResUpdate : BuildScriptPackedMode
    {
        /// <inheritdoc/>
        public override string Name
        {
            get { return "Hot Resource Update"; }
        }

        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            // Build AssetBundles
            TResult result = base.DoBuild<TResult>(builderInput, aaContext);

            BuildConfig.BuildTarget = builderInput.Target;

            Debug.Log(result.OutputPath);
            Debug.Log(result);
            // ClearTmpFolder();

            var bundleIdToFolderManifest = new Dictionary<string, FolderManifest>(); //一个bundle只能添加到一个文件夹
            Dictionary<string, FolderManifest> folderManifestDic = new Dictionary<string, FolderManifest>(); //所有foldermanifest列表
            var buildBundlePathData = new BuildBundlePathData();
            var streamingPack = HugulaResUpdatePacking.PackingType.streaming.ToString();
            foreach (AddressableAssetGroup group in aaContext.Settings.groups)
            {
                var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                if (resupPacking != null)
                {
                    string folderName = resupPacking.packingType.ToString();
                    if (resupPacking.packingType == HugulaResUpdatePacking.PackingType.custom)
                        folderName = resupPacking.customName;

                    AddToFolderManifest(folderName, group, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);
                }
                else
                {
                    AddToFolderManifest(streamingPack, group, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);
                }

            }

            #region catelog文件处理

            string localLoadPath = "{UnityEngine.AddressableAssets.Addressables.RuntimePath}/" + builderInput.RuntimeCatalogFilename;
            var settingsPath = Addressables.BuildPath + "/" + builderInput.RuntimeSettingsFilename;

            var m_CatalogBuildPath = Path.Combine(Addressables.BuildPath, builderInput.RuntimeCatalogFilename);
            if (aaContext.Settings.BundleLocalCatalog)
            {
                m_CatalogBuildPath = m_CatalogBuildPath.Replace(".json", ".bundle");
            }
            AddToFolderManifest(streamingPack, m_CatalogBuildPath, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);

            #endregion

            #region  序列化foldermanifest文件
            //保存bundle构建路径
            BuildBundlePathData.SerializeBuildBundlePathData(buildBundlePathData);
            #endregion


            System.Threading.Thread.Sleep(10);
            //分包与热更新资源copy
            BuildResPipeline.Initialize(folderManifestDic);
            BuildResPipeline.Build();

            return result;
        }

        /// <inheritdoc />
        public override void ClearCachedData()
        {
            base.ClearCachedData();
            ClearTmpFolder();
        }
        void AddToFolderManifest(string folderName, AddressableAssetGroup group, Dictionary<string, FolderManifest> bundleIdToFolderManifest, Dictionary<string, FolderManifest> folderManifestDic, BuildBundlePathData buildBundlePathData)
        {
            FolderManifest folderManifest = null;
            if (!folderManifestDic.TryGetValue(folderName, out folderManifest))
            {
                folderManifest = FolderManifestRuntionExtention.Create(folderName);
                folderManifestDic.Add(folderName, folderManifest);
            }
            // var buildRootPath = Addressables.BuildPath.Replace("\\","/");

            foreach (AddressableAssetEntry entry in group.entries)
            {
                string bundleBuildPath = AddressablesRuntimeProperties.EvaluateString(entry.BundleFileId);
                string bundleName = Path.GetFileName(bundleBuildPath);
                if (string.IsNullOrEmpty(bundleBuildPath))
                {
                    Debug.LogWarning($"address={entry.address},assetpath={entry.AssetPath},BundleFileId is empty =  {entry.BundleFileId} ");
                    continue;
                }
                if (bundleIdToFolderManifest.ContainsKey(bundleName))
                    continue;

                uint crc = 0;
                uint fileLen = 0;
                Debug.Log(bundleBuildPath);
                crc = CrcCheck.GetLocalFileCrc(bundleBuildPath, out fileLen);
                string parentFolder = Path.GetDirectoryName(bundleBuildPath).Replace("\\", "/");
                folderManifest.AddFileInfo(bundleName, crc, fileLen);

                buildBundlePathData.AddBuildBundlePath(bundleName, bundleBuildPath);
                bundleIdToFolderManifest.Add(bundleName, folderManifest);
            }
        }

        void AddToFolderManifest(string folderName, string filePath, Dictionary<string, FolderManifest> bundleIdToFolderManifest, Dictionary<string, FolderManifest> folderManifestDic, BuildBundlePathData buildBundlePathData)
        {
            FolderManifest folderManifest = null;
            if (!folderManifestDic.TryGetValue(folderName, out folderManifest))
            {
                folderManifest = FolderManifestRuntionExtention.Create(folderName);
                folderManifestDic.Add(folderName, folderManifest);
            }

            string bundleName = Path.GetFileName(filePath);
            string parentFolder = Path.GetDirectoryName(filePath).Replace("\\", "/");
            var buildPath = Addressables.BuildPath.Replace("\\", "/");
            string relative = RelativePath(parentFolder, buildPath);
            if (!bundleIdToFolderManifest.ContainsKey(bundleName))
            {
                uint crc = 0;
                uint fileLen = 0;
                crc = CrcCheck.GetLocalFileCrc(filePath, out fileLen);
                folderManifest.AddFileInfo(bundleName, crc, fileLen, relative);
                buildBundlePathData.AddBuildBundlePath(bundleName, filePath);
                bundleIdToFolderManifest.Add(bundleName, folderManifest);
            }

        }



        #region static tool

        public const BuildAssetBundleOptions DefaultBuildAssetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;// | BuildAssetBundleOptions.ChunkBasedCompression;

        /// <summary>
        /// 自动构建abs
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="abName"></param>
        /// <param name="bbo"></param>
        static public void BuildABs(AssetBundleBuild[] bab, string outPath, BuildAssetBundleOptions bbo)
        {
            if (string.IsNullOrEmpty(outPath))
                outPath = EditorUtils.GetOutPutPath();

            string tmpPath = EditorUtils.GetProjectTempPath();
            EditorUtils.CheckDirectory(tmpPath);
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(tmpPath, bab, bbo, BuildConfig.BuildTarget);

            var abNames = assetBundleManifest.GetAllAssetBundles();

            foreach (var abName in abNames)
            {
                string tmpFileName = Path.Combine(tmpPath, abName);
                string targetFileName = Path.Combine(outPath, abName);
                EditorUtils.CheckDirectory(outPath);
                FileInfo tInfo = new FileInfo(targetFileName);
                if (tInfo.Exists) tInfo.Delete();
                FileInfo fino = new FileInfo(tmpFileName);
                fino.CopyTo(targetFileName);
                Debug.LogFormat("Build assetbundle : {0} ", targetFileName);
            }

        }

        /// <summary>
        /// 按照单文件构建
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="bbo"></param>
        static public void BuildABsSeparately(string[] assets, string outPath, BuildAssetBundleOptions bbo)
        {
            AssetBundleBuild[] builds = new AssetBundleBuild[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                AssetBundleBuild curr = new AssetBundleBuild();
                string path = assets[i];
                curr.assetNames = new string[] { path };
                curr.assetBundleName = Path.GetFileNameWithoutExtension(path).ToLower() + Hugula.Utils.Common.CHECK_ASSETBUNDLE_SUFFIX;
                builds[i] = curr;
            }

            BuildABs(builds, outPath, bbo);
        }

        /// <summary>
        /// 将多个文件打成一个bundle
        /// </summary>
        static public void BuildABsTogether(string[] assets, string outPath, string name, BuildAssetBundleOptions bbo)
        {
            AssetBundleBuild[] builds = new AssetBundleBuild[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                AssetBundleBuild curr = new AssetBundleBuild();
                string path = assets[i];
                curr.assetNames = new string[] { path };
                curr.assetBundleName = name;
                // curr.assetBundleName = Path.GetFileNameWithoutExtension(path).ToLower() + Hugula.Utils.Common.CHECK_ASSETBUNDLE_SUFFIX;
                builds[i] = curr;
            }

            BuildABs(builds, outPath, bbo);
        }

        public static void ClearTmpFolder()
        {
            try
            {
                // AssetDatabase.StartAssetEditing();

                string tmpFolder = "Assets/Tmp";
                EditorUtils.DirectoryDelete(tmpFolder);
                Debug.Log($"清理缓存文件夹{tmpFolder}");
                if (!Directory.Exists(tmpFolder)) Directory.CreateDirectory(tmpFolder);

                string tmpPath = EditorUtils.GetProjectTempPath();
                Debug.Log($"清理缓存文件夹{tmpPath}");
                EditorUtils.DirectoryDelete(tmpPath);
                if (!Directory.Exists(tmpPath)) Directory.CreateDirectory(tmpPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception occured when clear tmp folder: {e.Message}.");
            }
            finally
            {
                // AssetDatabase.StopAssetEditing();
            }

        }

        public static string RelativePath(string fullPath, string rootPath)
        {
            return fullPath.Replace(rootPath, "");
        }
        #endregion
    }
}
