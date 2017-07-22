// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.IO;
using System.Collections.Generic;
using Hugula.Update;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Loader
{

    /// <summary>
    /// ManifestManager
    /// </summary>
    [SLua.CustomLuaClass]
    public static class ManifestManager
    {

#if UNITY_EDITOR
        const string kSimulateAssetBundles = "SimulateAssetBundles";
        static int m_SimulateAssetBundleInEditor = -1;
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        [SLua.DoNotToLuaAttribute]
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = UnityEditor.EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                if (value) PLua.isDebug = true;
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    UnityEditor.EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }

#endif

        [SLua.DoNotToLuaAttribute]
        public static bool CheckPersistentCrc(ABInfo abInfo)
        {
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                return true;
            }
#endif
            uint crc = 0;
            if (abInfo.state == ABInfoState.None)
            {
                uint len = 0;
                var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), abInfo.abName);
                crc = CrcCheck.GetLocalFileCrc(url, out len);
                if (crc == abInfo.crc32)
                {
                    abInfo.state = ABInfoState.Success;
                }
                else
                {
                    abInfo.state = ABInfoState.Fail;
                }
            }
            return abInfo.state == ABInfoState.Success;
        }

        [SLua.DoNotToLuaAttribute]
        public static bool CheckReqCrc(CRequest req)
        {
            if (req.url.StartsWith(Common.HTTP_STRING)) return true;
            var abName = req.assetBundleName;
            ABInfo abInfo = null;
            if (updateFileManifest != null &&
                (abInfo = updateFileManifest.GetABInfo(abName)) != null) //update file need crc check
            {
                return CheckPersistentCrc(GetABInfo(abInfo.abName));
            }
            else if (fileManifest != null && (abInfo = fileManifest.GetABInfo(abName)) != null
           && abInfo.priority > FileManifestOptions.StreamingAssetsPriority) // auto update file need crc check
            {
                return CheckPersistentCrc(GetABInfo(abInfo.abName));
            }
            return FileHelper.PersistentFileExists(abName);
        }

        [SLua.DoNotToLuaAttribute]
        public static ABInfo GetABInfo(string abName)
        {
            ABInfo abInfo = null;
            if (fileManifest != null)
                abInfo = fileManifest.GetABInfo(abName);

            return abInfo;
        }

        public static bool CheckExtendsFolderIsDown(string folderName)
        {
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                return true;
            }
#endif
            if (!HugulaSetting.instance.spliteExtensionFolder) return true;

            if (fileManifest != null)
            {
                var allABInfo = fileManifest.allAbInfo;
                ABInfo abInfo = null;
                for (int i = 0; i < allABInfo.Count; i++)
                {
                    abInfo = allABInfo[i];
                    if (abInfo.abName.StartsWith(folderName) && !CheckPersistentCrc(abInfo)) //!FileHelper.PersistentFileExists(abInfo.abName))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
                return false;
        }

        public static bool CheckABIsDone(string abName)
        {
            if (!HugulaSetting.instance.spliteExtensionFolder) return true;

            if (fileManifest != null)
            {
                var isDone = true;
                string[] allDeps = fileManifest.GetAllDependencies(abName); //判断依赖项目
                if (allDeps.Length > 0)
                {
                    var depABName = string.Empty;
                    for (int i = 0; i < allDeps.Length; i++)
                    {
                        depABName = allDeps[i];
                        if (!fileManifest.CheckABIsDone(depABName))
                        {
#if HUGULA_RELEASE && (UNITY_IOS || UNITY_ANDROID)
                                BackGroundDownload.instance.AddTask(fileManifest.GetABInfo(depABName), FileManifestOptions.UserPriority, null, null);
#endif
                            isDone = false;
                        }
                    }
                }

                if (!fileManifest.CheckABIsDone(abName))
                {
#if HUGULA_RELEASE && (UNITY_IOS || UNITY_ANDROID)
                        BackGroundDownload.instance.AddTask(fileManifest.GetABInfo(abName), FileManifestOptions.UserPriority, null, null);
#endif
                    isDone = false;
                }

                return isDone;
            }
            else
                return false;
        }

        public static bool CheckIsInFileManifest(string abName)
        {
            if (fileManifest != null)
            {
                return fileManifest.GetABInfo(abName) != null;
            }
            else
                return false;
        }

        public static FileManifest fileManifest;

        public static FileManifest updateFileManifest;

        public static string[] bundlesWithVariant;

        public static void LoadUpdateFileManifest(System.Action onComplete)
        {
            var fileListName = Common.CRC32_FILELIST_NAME;
            var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), CUtils.GetRightFileName(fileListName));
            AssetBundle ab = null;
            if (FileHelper.FileExists(url) && (ab = AssetBundle.LoadFromFile(url)) != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                if (assets.Length > 0)
                {
                    ManifestManager.updateFileManifest = assets[0];
                }
                ab.Unload(false);

                if (ManifestManager.fileManifest == null)
                    LoadFileManifest(null);

                if (ManifestManager.fileManifest != null) ManifestManager.fileManifest.AppendFileManifest(ManifestManager.updateFileManifest);

#if HUGULA_LOADER_DEBUG || UNITY_EDITOR
                Debug.LogFormat("LoadUpdateFileManifest 2 {0} is done ! {1}", url, fileListName, ManifestManager.updateFileManifest);
#endif
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }
        public static void LoadFileManifest(System.Action onComplete)
        {
            var fileListName = Common.CRC32_FILELIST_NAME;

            var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), CUtils.GetRightFileName(fileListName));
            url = CUtils.GetAndroidABLoadPath(url);
            AssetBundle ab = AssetBundle.LoadFromFile(url);
            if (ab != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                if (assets.Length > 0)
                    ManifestManager.fileManifest = assets[0];
                else
                    Debug.LogWarning("there is no fileManifest in StreamingAssetsPath use (Hugula/BUild For Bublish) build ");

#if HUGULA_LOADER_DEBUG || UNITY_EDITOR
                Debug.LogFormat("LoadFileManifest 1 {0} is done !\r\n ManifestManager.fileManifest.count = {1}", url, ManifestManager.fileManifest.Count);
#endif
                ab.Unload(false);
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }

        static string[] _activeVariants = { };

        /// <summary>
        ///  Variants which is used to define the active variants.
        /// </summary>
        public static string[] ActiveVariants
        {
            get { return _activeVariants; }
            set { _activeVariants = value; }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        internal static string RemapVariantName(string assetBundleName)
        {
            string baseName = assetBundleName.Split('.')[0];
            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            bundlesWithVariant = ManifestManager.fileManifest.allAssetBundlesWithVariant;
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                int found = System.Array.IndexOf(_activeVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
#endif
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

    }
}