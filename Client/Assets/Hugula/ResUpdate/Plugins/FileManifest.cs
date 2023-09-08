using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{
    /// <summary>
    /// 文件信息列表 记录每个文件的crc与size信息
    /// </summary>
    public class FileManifest : ScriptableObject
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
        /// 当前打包时候资源版本号用于热更新对比
        /// </summary>
        public int resNumber;

        /// <summary>
        /// 当前app版本号 string方式
        /// </summary>
        public string version;

        /// <summary>
        /// 名字
        /// </summary>
        public string fileName;

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
                if (m_TotalSize == uint.MinValue)
                {
                    uint s = 0;
                    for (int i = 0; i < allFileInfos.Count; i++)
                    {
                        s += allFileInfos[i].size;
                    }
                    m_TotalSize = s;
                }
                return m_TotalSize;
            }
            set
            {
                m_TotalSize = uint.MinValue;
            }
        }


        #endregion


        #region  instance method

        public void OnAfterDeserialize()
        {
            if (fileInfoDict == null)
            {
                fileInfoDict = new Dictionary<string, FileResInfo>(allFileInfos.Count + 2, System.StringComparer.OrdinalIgnoreCase);
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

        public void UpdateInfo(string name, FileResInfo fileResInfo)
        {
            if (fileInfoDict == null) OnAfterDeserialize();

            if (fileInfoDict.TryGetValue(name, out var fileInfo))
            {
                var i = allFileInfos.IndexOf(fileInfo);
                if (i >= 0)
                {
                    allFileInfos[i] = fileResInfo;//更新
                }
                else
                    allFileInfos.Add(fileResInfo);
                fileInfoDict[name] = fileResInfo;
            }
        }

        public void Remove(FileResInfo fileInfo)
        {
            if (fileInfoDict == null) OnAfterDeserialize();
            fileInfoDict.Remove(fileInfo.name);
            allFileInfos.Remove(fileInfo);
        }

        /// <summary>
        /// 获取单个文件信息
        /// </summary>
        public FileResInfo GetFileResInfo(string name)
        {
            FileResInfo abInfo = null;
            if (fileInfoDict == null) OnAfterDeserialize();
            fileInfoDict.TryGetValue(name, out abInfo);
            return abInfo;
        }

        /// <summary>
        /// 判断文件是否变更， true变更
        /// </summary>
        public bool CheckFileIsChanged(FileResInfo fileResInfo)
        {
            var localInfo = GetFileResInfo(fileResInfo.name);
            if (localInfo == null) //如果不存在表示变更
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
        public List<FileResInfo> Compare(FileManifest remote)
        {
            List<FileResInfo> re = new List<FileResInfo>();
            if (remote == null) return re;
            if (resNumber > remote.resNumber) return re; //如果本地大于远端不需要更新
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
        /// 用remote的foldermanifest对比本地 找出差异文件
        /// </summary>
        public List<FileResInfo> NotSafeCompare(FileManifest remote)
        {
            List<FileResInfo> re = new List<FileResInfo>();
            if (remote == null) return re;
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
        /// 移除自己相同的文件信息
        /// </summary>
        public bool RemoveSameFileResInfoFrom(FileManifest targetFolderManifest)
        {
            bool canAppend = resNumber > targetFolderManifest.resNumber;
            if (!canAppend) return false; //判断版本号

            FileResInfo fileResInfo;
            FileResInfo fileResInfo1;
            bool re = false;

            for (int i = 0; i < allFileInfos.Count;)
            {
                fileResInfo = allFileInfos[i];
                fileResInfo1 = targetFolderManifest.GetFileResInfo(fileResInfo.name);
                if (fileResInfo1 != null && fileResInfo.crc32 == fileResInfo1.crc32) //如果相同移除自己
                {
                    re = true;
                    allFileInfos.RemoveAt(i);
                    fileInfoDict.Remove(fileResInfo.name);
#if HUGULA_NO_LOG
                    UnityEngine.Debug.Log($"remove fileResInfo:({fileResInfo})");
#endif
                }
                else
                {
                    i++;
                }
            }

            return re;
        }

        #endregion

        public FileManifest Clone()
        {
            var folderManifest = ScriptableObject.CreateInstance<FileManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            var allFileInfos = this.allFileInfos;

            for (int i = 0; i < allFileInfos.Count; i++)
            {
                folderManifest.allFileInfos.Add(allFileInfos[i]);
            }

            return folderManifest;
        }

        /// <summary>
        /// 简单clone FileManifest  (不clone  allFileInfos文件列表)
        /// </summary>
        public FileManifest CloneWithOutAllFileInfos()
        {
            var folderManifest = ScriptableObject.CreateInstance<FileManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            return folderManifest;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine($"{this.GetType().FullName}(fileName={fileName},count={Count},total={totalSize},priority={priority},version={version}, resNumber={resNumber})");
            stringBuilder.AppendLine("allFileInfos:");
            foreach (var info in allFileInfos)
            {
                stringBuilder.AppendLine(info.ToString());
            }

            return stringBuilder.ToString();
        }

    }
}