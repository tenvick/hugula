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
using Hugula.Utils;

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

        BuildBundlePathData buildBundlePathData = new BuildBundlePathData();
        Dictionary<string, string> redirectionBundleName = new Dictionary<string, string>();
        public string EvaluateString(string path)
        {
            var fileName = Path.GetFileName(path);
            if (redirectionBundleName.TryGetValue(fileName, out var fPath))
            {
                path = fPath;
            }
            return AddressablesRuntimeProperties.EvaluateString(path);
        }

        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            Hugula.Utils.CUtils.DebugCastTime($"DoBuild End {builderInput.Target}");
            BuildBundlePathData.ClearBuildBundlePathData();
            redirectionBundleName.Clear();
            // Build AssetBundles
            TResult result = base.DoBuild<TResult>(builderInput, aaContext);

            BuildConfig.BuildTarget = builderInput.Target;

            if (!string.IsNullOrEmpty(result.Error))
                throw new Exception(result.Error);

            Debug.Log(result.OutputPath);
            Debug.Log($"result FileRegistry:{result.LocationCount}");
            // ClearTmpFolder();

            //增量热更新构建时候bundle重定向
            if (builderInput.PreviousContentState != null)
            {
                var sb = new System.Text.StringBuilder();
                var locs = aaContext.locations.OrderBy(f => f.InternalId).ToList();
                sb.AppendLine("\r\n--------locations--------------");
                var fileName = string.Empty;
                foreach (var loc in locs)
                {
                    foreach (var k in loc.Keys)
                    {
                        fileName = k.ToString();
                        if (fileName.EndsWith("bundle"))
                        {
                            var InternalId =loc.InternalId.Replace("\\","/");
                            redirectionBundleName.Add(fileName,InternalId);
                            sb.AppendLine($"\r\n redirection {fileName} = {InternalId}");
                        }
                    }
                }
                Debug.Log(sb.ToString());
            }

            Hugula.Utils.CUtils.DebugCastTime($"DoBuild BuildLuaBundle Begin");
            //构建Lua bundle
            var buildLuaBundle = new BuildLuaBundle();
            buildLuaBundle.Run(null);
            var luaBundleManifest = buildLuaBundle.GenStreamingLuaBundleManifest();
            // Debug.Log(luaBundleManifest.ToString());
            Hugula.Utils.CUtils.DebugCastTime($"DoBuild BuildLuaBundle End");

            var bundleIdToFolderManifest = new Dictionary<string, FileManifest>(); //一个bundle只能添加到一个文件夹
            var folderManifestDic = new Dictionary<string, FileManifest>(); //所有foldermanifest列表
            var streamingPack = HugulaResUpdatePacking.PackingType.streaming.ToString();

            List<AddressableAssetGroup> allGroups = aaContext.Settings.groups;

            FileManifest fileManifestAdded = null;
            foreach (AddressableAssetGroup group in allGroups)
            {
#if !DONT_SUB_PACKAGE
                var resupPacking = group.GetSchema<HugulaResUpdatePacking>();
                if (resupPacking != null)
                {
                    string folderName = resupPacking.packingType.ToString();
                    if (resupPacking.packingType == HugulaResUpdatePacking.PackingType.custom)
                        folderName = resupPacking.customName;

                    fileManifestAdded = AddToFolderManifest(folderName, group, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);
                    if (fileManifestAdded != null && resupPacking.priority != 0)
                    {
                        fileManifestAdded.priority = resupPacking.priority;
                    }
                }
                else
#endif
                {
                    fileManifestAdded = AddToFolderManifest(streamingPack, group, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);
                }

            }

            //添加lua luaBundleManifest 文件信息记录
            folderManifestDic.Add(Hugula.Utils.Common.LUA_BUNDLE_NAME, luaBundleManifest);
            Hugula.Utils.CUtils.DebugCastTime($"DoBuild AddToFolderManifest End");

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

            #region other bundle eg: _mono_monoscripts  _unitybuiltinshaders
            Add_monoscripts_unitybuiltinshaders_ToFolderManifest(streamingPack,builderInput.Registry, bundleIdToFolderManifest, folderManifestDic, buildBundlePathData);
            #endregion

            #region  序列化foldermanifest文件
            Hugula.Utils.CUtils.DebugCastTime($"DoBuild SerializeBuildBundlePathData Start");
            //保存bundle构建路径
            BuildBundlePathData.SerializeBuildBundlePathData(buildBundlePathData);
            Hugula.Utils.CUtils.DebugCastTime($"DoBuild SerializeBuildBundlePathData End");

            #endregion


            System.Threading.Thread.Sleep(10);
            //分包与热更新资源copy
            BuildResPipeline.Initialize();

            BuildResPipeline.Build(folderManifestDic, builderInput.PreviousContentState != null);

            return result;
        }

        /// <inheritdoc />
        public override void ClearCachedData()
        {
            base.ClearCachedData();
            ClearTmpFolder();
            BuildBundlePathData.ClearBuildBundlePathData();
        }

        public static void AddToBundleManifest(string filePath, BundleManifest bundleManifest)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            uint crc = 0;
            uint fileLen = 0;
            crc = CrcCheck.GetLocalFileCrc(filePath, out fileLen);
            bundleManifest.AddFileInfo(fileName, crc, fileLen);
        }

        public static void AddToFolderManifest(string filePath, FolderManifest folderManifest, BuildBundlePathData buildBundlePathData)
        {
            filePath = filePath.Replace("\\", "/");
            string fileName = Path.GetFileName(filePath);
            uint crc = 0;
            uint fileLen = 0;
            crc = CrcCheck.GetLocalFileCrc(filePath, out fileLen);
            folderManifest.AddFileInfo(fileName, crc, fileLen);
            buildBundlePathData?.AddBuildBundlePath(fileName, filePath, crc);
        }

        FileManifest AddToFolderManifest(string folderName, AddressableAssetGroup group, Dictionary<string, FileManifest> bundleIdToFolderManifest, Dictionary<string, FileManifest> folderManifestDic, BuildBundlePathData buildBundlePathData)
        {
            FolderManifest folderManifest = null;

            if (!folderManifestDic.TryGetValue(folderName, out var outFolderManifest))
            {
                folderManifest = HugulaEditor.Addressable.FolderManifestExtention.Create(folderName);
                folderManifestDic.Add(folderName, folderManifest);
            }
            else
            {
                folderManifest = outFolderManifest as FolderManifest;
            }

            if (folderManifest == null)
            {
                Debug.LogWarning($"AddToFolderManifest fail folderManifest is null folderName={folderName}");
                return folderManifest;
            }

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (string.IsNullOrEmpty(entry.BundleFileId))
                {
                    Debug.LogError($"address={entry.address},assetpath={entry.AssetPath},BundleFileId is empty =  {entry.BundleFileId} ");
                    continue;
                }

                string bundleBuildPath = EvaluateString(entry.BundleFileId.Replace("\\", "/"));
                string bundleName = Path.GetFileName(bundleBuildPath);

                if (folderManifest.fileName != Hugula.Utils.Common.FOLDER_STREAMING_NAME && !folderManifest.allAddressKeys.Contains(entry.address)) //添加key
                {
                    folderManifest.allAddressKeys.Add(entry.address);
                }

                if (bundleIdToFolderManifest.ContainsKey(bundleName))
                {
                    // Debug.LogWarning($"address={entry.address},assetpath={entry.AssetPath}, has aleady added. ");
                    continue;
                }

                uint crc = 0;
                uint fileLen = 0;

                crc = CrcCheck.GetLocalFileCrc(bundleBuildPath, out fileLen);
                folderManifest.AddFileInfo(bundleName, crc, fileLen);

                buildBundlePathData.AddBuildBundlePath(bundleName, bundleBuildPath, crc);
                bundleIdToFolderManifest.Add(bundleName, folderManifest);
            }

            return folderManifest;
        }

        FileManifest AddToFolderManifest(string folderName, string filePath, Dictionary<string, FileManifest> bundleIdToFolderManifest, Dictionary<string, FileManifest> folderManifestDic, BuildBundlePathData buildBundlePathData)
        {
            FolderManifest folderManifest = null;
            if (!folderManifestDic.TryGetValue(folderName, out var outFolderManifest))
            {
                folderManifest = HugulaEditor.Addressable.FolderManifestExtention.Create(folderName);
                folderManifestDic.Add(folderName, folderManifest);
            }
            else
            {
                folderManifest = outFolderManifest as FolderManifest;
            }

            if (folderManifest == null) return folderManifest;

            filePath = filePath.Replace("\\", "/");

            string bundleName = Path.GetFileName(filePath);
            if (!bundleIdToFolderManifest.ContainsKey(bundleName))
            {
                uint crc = 0;
                uint fileLen = 0;
                crc = CrcCheck.GetLocalFileCrc(filePath, out fileLen);
                folderManifest.AddFileInfo(bundleName, crc, fileLen);
                buildBundlePathData.AddBuildBundlePath(bundleName, filePath, crc);
                bundleIdToFolderManifest.Add(bundleName, folderManifest);
            }

            return folderManifest;
        }

        //_monoscripts.bundle  _unitybuiltinshaders.bundle
        FileManifest Add_monoscripts_unitybuiltinshaders_ToFolderManifest(string folderName, FileRegistry fileRegistry, Dictionary<string, FileManifest> bundleIdToFolderManifest, Dictionary<string, FileManifest> folderManifestDic, BuildBundlePathData buildBundlePathData)
        {
            FolderManifest folderManifest = null;
            if (!folderManifestDic.TryGetValue(folderName, out var outfolderManifest))
            {
                folderManifest = HugulaEditor.Addressable.FolderManifestExtention.Create(folderName);
                folderManifestDic.Add(folderName, folderManifest);
            }
            else
            {
                folderManifest = outfolderManifest as FolderManifest;
            }

            if (folderManifest == null) return folderManifest;


            var paths = fileRegistry.GetFilePaths();

            foreach (var filePath in paths)
            {
                string bundleName = Path.GetFileName(filePath.Replace("\\", "/"));
                if(bundleName.Contains("_monoscripts.bundle") || bundleName.Contains("_unitybuiltinshaders.bundle"))
                {
                    uint crc = 0;
                    uint fileLen = 0;
                    crc = CrcCheck.GetLocalFileCrc(filePath, out fileLen);
                    folderManifest.AddFileInfo(bundleName, crc, fileLen);
                    buildBundlePathData.AddBuildBundlePath(bundleName, filePath, crc);
                    bundleIdToFolderManifest.Add(bundleName, folderManifest);
                    // Debug.Log($"add other bundle({filePath}) to ({folderName})");
                }
            }

            return folderManifest;
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
        static public bool BuildABs(AssetBundleBuild[] bab, string outPath, BuildAssetBundleOptions bbo, byte[] offset = null)
        {
            if (string.IsNullOrEmpty(outPath))
                outPath = EditorUtils.GetOutPutPath();

            string tmpPath = EditorUtils.GetProjectTempPath();
            EditorUtils.CheckDirectory(tmpPath);
            var sb = new System.Text.StringBuilder();
            foreach(var b in bab)
            {
                sb.Append(b.assetBundleName);
                sb.Append(",");
                break;
            }
            Hugula.Utils.CUtils.DebugCastTime($"Build BuildABs BuildPipeline.BuildAssetBundles begin length={bab.Length},outPath={outPath} abNames={sb.ToString()}");
            var assetBundleManifest = BuildPipeline.BuildAssetBundles(tmpPath, bab, bbo, BuildConfig.BuildTarget);
            
            Hugula.Utils.CUtils.DebugCastTime($"Build BuildABs BuildPipeline.BuildAssetBundles end length={bab.Length},outPath={outPath}    abNames={sb.ToString()}");

            if(assetBundleManifest==null)
            {
                Debug.LogError($"Build BuildABs BuildPipeline.BuildAssetBundles fail length={bab.Length},outPath={outPath}    abNames={sb.ToString()}");
                return false;
            }

            var abNames = assetBundleManifest.GetAllAssetBundles();

            foreach (var abName in abNames)
            {
                string tmpFileName = Path.Combine(tmpPath, abName);
                string targetFileName = Path.Combine(outPath, abName);
                EditorUtils.CheckDirectory(outPath);
                FileInfo tInfo = new FileInfo(targetFileName);
                if (tInfo.Exists) tInfo.Delete();
                FileInfo fino = new FileInfo(tmpFileName);

                if (offset != null && offset.Length > 0)
                {
                    using (var sw = tInfo.Create())
                    {
                        sw.Write(offset, 0, offset.Length);
                        using (var sr = fino.OpenRead())
                        {
                            InternalCopyTo(sr, sw, 128);
                        }
                    }
                    Debug.Log($"Build assetbundle : {targetFileName} offset:{offset.Length}");
                }
                else
                {
                    fino.CopyTo(targetFileName);
                    Debug.LogFormat("Build assetbundle : {0} ", targetFileName);
                }
            }

            return true;
        }

        private static void InternalCopyTo(Stream source, Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
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
        static public void BuildZipTogether(string[] assets, string outPath, string rootPath = null, string password = "")
        {
            Hugula.Utils.ZipHelper.CreateZip(outPath, new List<string>(assets), rootPath, password);
            Debug.Log($"BuildZipTogether success path:{outPath} password:{password}");

        }

        /// <summary>
        /// 将文件copy到指定目录并加密
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="outPath"></param>
        /// <param name="rootPath"></param>
        /// <param name="password"></param>
        static public void PackSeparatelyToOutPath(string[] assets, string outPath,string rootPath = null,string password = "")
        {
            Debug.Log($"PackSeparatelyZipToOutPath( {assets.Length},{outPath},{rootPath}) ");
            //
            foreach (var asset in assets)
            {
                //获取rootPath相对路径
                string fileName = asset.Replace(rootPath, "");
                //将fileName路径分隔符替换为.
                fileName = fileName.Replace("\\", ".").Replace("/", ".").Substring(1);
                string targetPath = Path.Combine(outPath, fileName);
                EditorUtils.CheckDirectory(outPath);
                Debug.Log($"{asset} to {targetPath} ");
                FileInfo tInfo = new FileInfo(targetPath);
                if (tInfo.Exists) tInfo.Delete();
                FileInfo fino = new FileInfo(asset);
                fino.CopyTo(targetPath);
            }
        }


        /// <summary>
        /// 将多个文件打成一个bundle
        /// </summary>
        static public void BuildABsTogether(string[] assets, string outPath, string name, BuildAssetBundleOptions bbo, byte[] offset = null)
        {

            AssetBundleBuild[] builds = new AssetBundleBuild[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                AssetBundleBuild curr = new AssetBundleBuild();
                string path = assets[i];
                curr.assetNames = new string[] { path };
                curr.assetBundleName = name;
                builds[i] = curr;
            }

            BuildABs(builds, outPath, bbo, offset);

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

        public static string StripHashFromBundleLocation(string hashedBundleLocation)
        {
            return hashedBundleLocation.Remove(hashedBundleLocation.LastIndexOf("_")) + ".bundle";
        }
        #endregion
    }
}
