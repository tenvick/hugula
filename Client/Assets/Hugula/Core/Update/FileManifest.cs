using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Hugula.Loader;
using Hugula.Utils;

namespace Hugula.Update
{
    /// <summary>
    /// Manifest.
    /// </summary>
    [SLua.CustomLuaClass]
    public class FileManifest : ScriptableObject,ISerializationCallbackReceiver
    {
        private static readonly string[] EmptyStringArray = new string[0];
        private static readonly int[] EmptyIntArray = new int[0];
        internal Dictionary<string, ABInfo> abInfoDict = new Dictionary<string, ABInfo>(System.StringComparer.OrdinalIgnoreCase);

        [SLua.DoNotToLuaAttribute]
        public List<ABInfo> allAbInfo = new List<ABInfo>();

        [SLua.DoNotToLuaAttribute]
        public string[] allAssetBundlesWithVariant;

        public int appNumVersion;

        public uint crc32 = 0;

        public bool hasFirstLoad = false;

        public int Count
        {
            get
            {
                return allAbInfo.Count;
            }
        }

        [SLua.DoNotToLua]
        public void OnBeforeSerialize()
        {

        }

        [SLua.DoNotToLua]
        public void OnAfterDeserialize()
        {
            if (abInfoDict == null) abInfoDict = new Dictionary<string, ABInfo>(allAbInfo.Count + 32);

             ABInfo abinfo;
            for (int i = 0; i < allAbInfo.Count; i++)
            {
                abinfo = allAbInfo[i];
                abInfoDict[abinfo.abName] = abinfo;
            }

        }

        public bool CheckABIsDone(string abName)
        {
            ABInfo abInfo = GetABInfo(abName);
            if (abInfo != null)
            {
                if (abInfo.state == ABInfoState.Success) return true;
                if (abInfo.state == ABInfoState.Fail) return false;
                if (abInfo.priority >= FileManifestOptions.FirstLoadPriority)
                {
                    return ManifestManager.CheckPersistentCrc(abInfo);
                }
                else
                    return true;
            }
            return false;
        }

        public void AppendFileManifest(FileManifest newFileManifest)
        {
            if (appNumVersion <= newFileManifest.appNumVersion)
            {
                this.allAssetBundlesWithVariant = newFileManifest.allAssetBundlesWithVariant;

                var list = newFileManifest.allAbInfo;
                for (int i = 0; i < list.Count; i++)
                {
                    Add(list[i]);
                }
            }
        }

        public string[] GetAllDependencies(string assetBundleName)
        {
            var allDependencies = ListPool<string>.Get();
            GetAllDependenciesFromDirect(allDependencies, assetBundleName);
            string[] re = allDependencies.ToArray();
            ListPool<string>.Release(allDependencies);
            return re;
        }

        public string[] GetDirectDependencies(string assetBundleName)
        {
            string[] re = EmptyStringArray;
            var abInfo = GetABInfo(assetBundleName);
            if (abInfo != null) re = abInfo.dependencies;
            return re;
        }

        public void Clear()
        {
            abInfoDict.Clear();
            allAbInfo.Clear();
        }

        private void GetAllDependenciesFromDirect(List<string> allDependencies, string assetBundleName)
        {
            string[] dependencies = GetDirectDependencies(assetBundleName);
            string s;
            for (int i = 0; i < dependencies.Length; i++)
            {
                s = dependencies[i];
                GetAllDependenciesFromDirect(allDependencies, s);
                if (allDependencies.IndexOf(s) == -1)
                    allDependencies.Add(s);
            }
        }

        [SLua.DoNotToLuaAttribute]
        public ABInfo GetABInfo(string abName)
        {
            ABInfo abInfo = null;
            abInfoDict.TryGetValue(abName, out abInfo);
            return abInfo;
        }

        [SLua.DoNotToLuaAttribute]
        public List<ABInfo> CompareFileManifest(FileManifest compare)
        {
            List<ABInfo> re = new List<ABInfo>();
            var compareABInfos = compare.allAbInfo;
            ABInfo abInfo;
            for (int i = 0; i < compareABInfos.Count; i++)
            {
                abInfo = compareABInfos[i];
                if (!CheckABCrc(abInfo))
                {
                    re.Add(abInfo);
                }
            }

            return re;
        }

        [SLua.DoNotToLuaAttribute]
        public bool CheckABCrc(ABInfo abInfo)
        {
            var localInfo = GetABInfo(abInfo.abName);
            if (localInfo != null && localInfo.crc32 == abInfo.crc32)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendFormat("FileManifest AppNumber={0},count={1},crc32={2},hasFirstLoad={3}", appNumVersion, Count,crc32,hasFirstLoad);
            stringBuilder.AppendLine(" ABInfos:");
            foreach (var info in allAbInfo)
            {
                stringBuilder.AppendLine(info.ToString());
            }
            if (allAssetBundlesWithVariant != null)
            {
                stringBuilder.AppendLine("AssetBundlesWithVariant:");
                foreach (var info in allAssetBundlesWithVariant)
                {
                    stringBuilder.AppendLine(info);
                }
            }
            return stringBuilder.ToString();
        }

        [SLua.DoNotToLuaAttribute]
        public void Add(ABInfo ab)
        {
            var abInfo = GetABInfo(ab.abName);
            int i = -1;
            if (abInfo != null)
            {
                i = allAbInfo.IndexOf(abInfo);
                if (i >= 0) allAbInfo.RemoveAt(i);
            }
            abInfoDict[ab.abName] = ab;
            if (i >= 0)
                allAbInfo.Insert(i, ab);
            else
                allAbInfo.Add(ab);
        }


#if UNITY_EDITOR
        [SLua.DoNotToLuaAttribute]
        public void WriteToFile(string path)
        {
            using (StreamWriter wr = new StreamWriter(path))
            {
                wr.Write(ToString());
            }
        }
#endif
    }

    [SLua.CustomLuaClass]
    public static class FileManifestOptions
    {   
        /// <summary>
        /// 本地
        /// </summary>
        public const int StreamingAssetsPriority = 0;

        /// <summary>
        /// 首包下载的资源
        /// </summary>
        public const int FirstLoadPriority = 256;//2^8

        /// <summary>
        /// 自动后台下载级别
        /// </summary>
        public const int AutoHotPriority = 262144; //2^18

        /// <summary>
        /// 用户使用频率优先级别
        /// </summary>
        public const int UserPriority = 524288; //2^19
        
        /// <summary>
        /// 手动下载目录
        /// </summary>
        public const int ManualPriority = 1048576;//2^20
    }
}