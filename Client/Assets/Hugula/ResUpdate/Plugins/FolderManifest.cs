using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{
    /// <summary>
    /// 文件夹信息文件列表
    /// 以文件夹为基准进行差异文件更新
    /// 包内为streaming.asset
    /// 1 如果缓存有folderName的Manifest文件那么表明此文件夹下所有文件已经下载，需要判断是否有热更新
    /// </summary>
    public class FolderManifest : ScriptableObject
    {

        #region instance field
        /// <summary>
        /// 所有列表字典映射
        /// </summary>
        internal Dictionary<string, FileResInfo> fileInfoDict;// = new Dictionary<string, FileResInfo>(System.StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 序列化的所有文件列表
        /// </summary>
        public List<FileResInfo> allFileInfos = new List<FileResInfo>();

        /// <summary>
        /// 当前文件夹的优先级
        /// </summary>
        public int priority;
        /// <summary>
        /// 当前打包时候版本号
        /// </summary>
        public int appNumVersion;
        /// <summary>
        /// 当前app版本号 string方式
        /// </summary>
        public string version;

        /// <summary>
        /// 文件夹名字
        /// </summary>
        public string folderName;

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
        public string zipName{
            get
            {
                if(string.IsNullOrEmpty(m_ZipName))
                    m_ZipName = string.Format("{0}_{1}",folderName, zipVersion);
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
                if(string.IsNullOrEmpty(m_ZipMarkName))
                    m_ZipMarkName = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(),string.Format("{0}_{1}.zipd",folderName, zipVersion));
                return m_ZipMarkName;
            }
        }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int Count
        {
            get
            {
                return allFileInfos.Count;
            }
        }

        private uint m_TotalSize = uint.MinValue;
        /// <summary>
        /// 统计文件总体大小
        /// </summary>
        public uint totalSize
        {
            get
            {
                if(m_TotalSize == uint.MinValue)
                {
                    uint s=0;
                    for(int i=0;i<allFileInfos.Count;i++)
                    {
                        s+=allFileInfos[i].size;
                    }
                    m_TotalSize = s;
                }
                return m_TotalSize;
            }
        }
        #endregion


        #region  instance method

        public void OnAfterDeserialize()
        {
            if (fileInfoDict == null)
            {
                fileInfoDict = new Dictionary<string, FileResInfo>(allFileInfos.Count + 2,System.StringComparer.OrdinalIgnoreCase);
                FileResInfo abinfo;
                for (int i = 0; i < allFileInfos.Count; i++)
                {
                    abinfo = allFileInfos[i];
                    fileInfoDict[abinfo.name] = abinfo;
                }
            }             
        }

        public void Add(FileResInfo fileInfo)
        {
            var abInfo = GetFileResInfo(fileInfo.name);
            int i = -1;
            if (abInfo != null)
            {
                i = allFileInfos.IndexOf(abInfo);
            }
            fileInfoDict[fileInfo.name] = fileInfo;
            if (i >= 0)
                allFileInfos[i] = fileInfo;//更新
            else
                allFileInfos.Add(fileInfo);
        }
        
        /// <summary>
        /// 获取单个文件信息
        /// </summary>
        public FileResInfo GetFileResInfo (string name) {
            FileResInfo abInfo = null;
            if (fileInfoDict == null) OnAfterDeserialize();
            fileInfoDict.TryGetValue(name ,out abInfo);
            return abInfo;
        }

        /// <summary>
        /// 判断文件是否变更， true变更
        /// </summary>
        public bool CheckFileIsChanged(FileResInfo fileResInfo)
        {
             var localInfo = GetFileResInfo(fileResInfo.name);
             if(localInfo == null) //如果不存在表示变更
             {
                return true;
             }
             else
             {
                return localInfo.crc32 != fileResInfo.crc32;//校验码不相等表示文件变更
             }
        }

        /// <summary>
        /// 用remote的foldermanifest对比本地 找出差异文件
        /// </summary>
        public List<FileResInfo> Compare(FolderManifest remote)
        {
            List<FileResInfo> re = new List<FileResInfo>();
            if(remote==null) return re;
            if(appNumVersion > remote.appNumVersion) return re; //如果本地大于远端不需要更新
            var compareABInfos = remote.allFileInfos;
            FileResInfo abInfo;
            for (int i = 0; i < compareABInfos.Count; i++)
            {
                abInfo = compareABInfos[i];
                if (CheckFileIsChanged(abInfo))
                {
                    re.Add(abInfo);
                }
            }

            return re;
        }

        /// <summary>
        /// 用remote的foldermanifest覆盖本地配置
        /// </summary>
        public bool AppendFileManifest(FolderManifest newFolderManifest)
        {
            if(newFolderManifest == this) return false; //不能覆盖自身
            bool canAppend = appNumVersion <= newFolderManifest.appNumVersion;
            if(canAppend)
            {
                version = newFolderManifest.version;
                appNumVersion = newFolderManifest.appNumVersion;
                var allFileInfos = newFolderManifest.allFileInfos;
                foreach(var finfo in allFileInfos)
                {
                    Add(finfo);
                }
            }

            return canAppend;
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

    

        /// <summary>
        /// 简单clone folderManifest对象  (不clone  allFileInfos文件列表)
        /// </summary>
        public FolderManifest CloneWithOutAllFileInfos()
        {
            var folderManifest = ScriptableObject.CreateInstance<FolderManifest>();
            folderManifest.appNumVersion = this.appNumVersion;
            folderManifest.version = this.version;
            folderManifest.folderName = this.folderName;
            folderManifest.priority = this.priority;
            folderManifest.zipSize = this.zipSize;
            folderManifest.zipVersion = this.zipVersion;
            return folderManifest;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine($"FolderManifest(name={folderName},count={Count},priority={priority},version={version}, AppNumber={appNumVersion},zipSize={zipSize},zipVersion={zipVersion})");
            stringBuilder.AppendLine("allFileInfos:");
            foreach (var info in allFileInfos)
            {
                stringBuilder.AppendLine(info.ToString());
            }
            return stringBuilder.ToString();
        }
       
       
        #endregion

       


        #region  editor

        #endregion
    }
  

}