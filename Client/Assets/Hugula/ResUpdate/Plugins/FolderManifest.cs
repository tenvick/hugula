using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{

    /// <summary>
    /// 文件夹信息文件列表
    /// 包含的是assetbundle文件信息 用于常规aasetbundle下载更新
    /// 包内为streaming.asset
    /// </summary>
    public class FolderManifest : FileManifest
    {
        /// <summary>
        /// 包含的所有key
        /// </summary>
        public List<string> allAddressKeys = new List<string>();

        /// <summary>
        /// zip文件size
        /// </summary>
        public uint zipSize;

        /// <summary>
        /// zip版本号
        /// </summary>
        public string zipVersion;

        private string m_ZipName;

        /// <summary>
        /// zip文件名称,版本号为application.version
        /// </summary>
        public string zipName
        {
            get
            {
                if (string.IsNullOrEmpty(m_ZipName))
                    m_ZipName = string.Format("{0}_{1}", fileName, zipVersion);
                return m_ZipName;
            }
            set
            {
                m_ZipName = null;
            }
        }

        private string m_ZipMarkName;
        /// <summary>
        /// zip标记文件名称
        /// </summary>
        public string zipMarkPathName
        {
            get
            {
                if (string.IsNullOrEmpty(m_ZipMarkName))
                    m_ZipMarkName = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), string.Format("{0}_{1}.zipd", fileName, zipVersion));
                return m_ZipMarkName;
            }
        }

        /// <summary>
        /// 是否要重定向到zip文件夹
        /// </summary>
        public bool transformZipFolder
        {
            get;
            set;
        }

        /// <summary>
        /// zip文件是否已经下载完成,
        /// streaming包里内容无需下载
        /// </summary>
        public bool isZipDone
        {
            get
            {
                return File.Exists(zipMarkPathName);
            }
        }

        public new FolderManifest Clone()
        {
            var folderManifest = ScriptableObject.CreateInstance<FolderManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            folderManifest.zipSize = this.zipSize;
            folderManifest.zipVersion = this.zipVersion;
            var allFileInfos = this.allFileInfos;

            for (int i = 0; i < allFileInfos.Count; i++)
            {
                folderManifest.allFileInfos.Add(allFileInfos[i]);
            }

            var keys = this.allAddressKeys;
            for (int i = 0; i < keys.Count; i++)
            {
                folderManifest.allAddressKeys.Add(keys[i]);
            }

            return folderManifest;
        }

        /// <summary>
        /// 简单clone folderManifest对象  (不clone  allFileInfos文件列表)
        /// </summary>
        public new FolderManifest CloneWithOutAllFileInfos()
        {
            var folderManifest = ScriptableObject.CreateInstance<FolderManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            folderManifest.zipSize = this.zipSize;
            folderManifest.zipVersion = this.zipVersion;
            return folderManifest;
        }

        /// <summary>
        /// 追加文件到当前文件夹,如果文件已经存在则更新,版本号大于等于才能追加
        /// </summary>
        /// <param name="folderManifest"></param>
        /// <returns></returns>
        public bool AppendFolder(FolderManifest folderManifest)
        {
            bool canAppend = resNumber <= folderManifest.resNumber;//大于等于才能追加
            if (!canAppend) return false; //判断版本号

            FileResInfo fileResInfo;
            FileResInfo fileResInfo1;

            var tagetAllFileInfos = folderManifest.allFileInfos;

            for (int i = 0; i < tagetAllFileInfos.Count;i++)
            {
                fileResInfo = tagetAllFileInfos[i];
                fileResInfo1 = GetFileResInfo(fileResInfo.name);
                if(fileResInfo1==null)
                {
                    Add(fileResInfo);
                }else if (fileResInfo.crc32 != fileResInfo1.crc32) //如果相同移除自己
                {
                    UpdateInfo(fileResInfo.name,fileResInfo);
#if HUGULA_NO_LOG
                    UnityEngine.Debug.Log($"update fileResInfo:({fileResInfo})");
#endif
                }
               
            }

            return true;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine($"FolderManifest(fileName={fileName},count={Count},total={totalSize},priority={priority},version={version}, resNumber={resNumber},zipSize={zipSize},zipVersion={zipVersion})");
            stringBuilder.AppendLine("allFileInfos:");
            foreach (var info in allFileInfos)
            {
                stringBuilder.AppendLine(info.ToString());
            }
            stringBuilder.AppendLine("allAddressKeys:");
            foreach (var k in allAddressKeys)
            {
                stringBuilder.AppendLine(k);
            }
            return stringBuilder.ToString();
        }


    }

}