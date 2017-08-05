using UnityEngine;
using System.Collections.Generic;
using Hugula.Editor;
using Hugula.Loader;
using Hugula.Update;
using Hugula.Utils;

namespace Hugula.Editor.Task
{

    public class HotResGenSharedData
    {
        public bool firstExists;
        public FileManifest firstCrcDict = null;//首包
        public FileManifest streamingManifest = null;//streamingAsset
        public FileManifest manualFileList = null;
        public FileManifest diffstreamingManifest = null;
        public List<ABInfo>[] abInfoArray = null;

        public uint diff_crc;
    }
    //读取首包
    public class ReadFirst : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "ReadFirst";
        }

        public void Run(HotResGenSharedData sharedata)
        {
            FileManifest firstCrcDict = null;//首包
            FileManifest streamingManifest = null;//streamingAsset
            FileManifest manualFileList = ScriptableObject.CreateInstance(typeof(FileManifest)) as FileManifest;//后置下载

            bool firstExists = SplitPackage.ReadFirst(out firstCrcDict, out streamingManifest, manualFileList);
            sharedata.firstExists = firstExists;
            sharedata.firstCrcDict = firstCrcDict;
            sharedata.streamingManifest = streamingManifest;
            sharedata.manualFileList = manualFileList;
        }

    }

    public class CreateCrcListContent : ITask<HotResGenSharedData>
    {

        public string GetName()
        {
            return "CreateCrcListContent";
        }
        private string[] m_allBundles;
        public CreateCrcListContent(string[] allBundles)
        {
            m_allBundles = allBundles;
        }

        public void Run(HotResGenSharedData sharedata)
        {
            SplitPackage.UpdateOutPath = null;
            SplitPackage.UpdateOutDevelopPath = null;
            sharedata.abInfoArray = SplitPackage.CreateCrcListContent(m_allBundles, sharedata.firstCrcDict, sharedata.streamingManifest, sharedata.manualFileList);
        }
    }

    public class CreateLocalFileManifest : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Create Local FileManifest";
        }

        public CreateLocalFileManifest()
        {

        }

        public void Run(HotResGenSharedData sharedata)
        {
            var streamingManifest = sharedata.streamingManifest;
            var streamingManifestClone = ScriptableObject.CreateInstance(typeof(FileManifest)) as FileManifest;
            streamingManifestClone.allAbInfo = streamingManifest.allAbInfo;
            streamingManifestClone.allAssetBundlesWithVariant = streamingManifest.allAssetBundlesWithVariant;
            streamingManifestClone.OnAfterDeserialize();
            streamingManifestClone.crc32 = streamingManifest.crc32;
            streamingManifestClone.hasFirstLoad = streamingManifest.hasFirstLoad;
            sharedata.diff_crc = SplitPackage.CreateStreamingCrcList(streamingManifestClone, Common.CRC32_FILELIST_NAME, sharedata.firstExists); //本地列表
            streamingManifestClone.WriteToFile("Assets/" + BuildScript.TmpPath + "streamingManifest0(Clone).txt");
        }

    }

    public class CreateDiffFileManifest : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Create Diffrent FileManifest";
        }

        public void Run(HotResGenSharedData sharedata)
        {
            var streamingManifest = sharedata.streamingManifest;
            var diffstreamingManifest = ScriptableObject.CreateInstance(typeof(FileManifest)) as FileManifest;
            diffstreamingManifest.allAbInfo = sharedata.abInfoArray[0];
            diffstreamingManifest.allAssetBundlesWithVariant = streamingManifest.allAssetBundlesWithVariant;
            diffstreamingManifest.OnAfterDeserialize();
            sharedata.diff_crc = SplitPackage.CreateStreamingCrcList(diffstreamingManifest, Common.CRC32_FILELIST_NAME, true, true); //增量列表
            sharedata.diffstreamingManifest = diffstreamingManifest;
            diffstreamingManifest.WriteToFile("Assets/" + BuildScript.TmpPath + "diffstreamingManifest.txt");
        }
    }

    public class CreateVersionAssetBundle : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Create Version AssetBundle";
        }

        public void Run(HotResGenSharedData sharedata)
        {
            var diff_crc = sharedata.diff_crc;
            SplitPackage.CreateVersionAssetBundle(diff_crc, true, !CUtils.isRelease);
            SplitPackage.CreateVersionAssetBundle(diff_crc, false, true);
        }
    }

    public class CopyChangeFiles : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Copy Change Files";
        }

        public void Run(HotResGenSharedData sharedata)
        {
            SplitPackage.CopyChangeFileToSplitFolder(sharedata.firstExists, sharedata.firstCrcDict, sharedata.streamingManifest, sharedata.diffstreamingManifest, sharedata.manualFileList);
        }
    }

    public class ZipBundles : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Zip Bundles";
        }

        public void Run(HotResGenSharedData sharedata)
        {
#if HUGULA_RELEASE
    
#endif
            // if (HugulaSetting.instance.compressStreamingAssets)
           //     SplitPackage.ZipAssetbundles();
            // else
            // {
            //     ZipConfigs.Delete();
            // }
        }
    }

    public class ClearAssetBundle : ITask<HotResGenSharedData>
    {
        public string GetName()
        {
            return "Clear AssetBundle";
        }

        public void Run(HotResGenSharedData sharedata)
        {
#if (UNITY_ANDROID || UNITY_IOS) && HUGULA_RELEASE
            bool spExtFolder = HugulaSetting.instance.spliteExtensionFolder;
            if (spExtFolder)
            {
                var manualFileList = sharedata.manualFileList;
                SplitPackage.DeleteStreamingFiles(manualFileList.allAbInfo);
            }
#endif

        }
    }
}