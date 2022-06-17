using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{

    /// <summary>
    /// 整个assetbundle中assets信息列表用于ab增量更新
    /// </summary>
    public class BundleManifest : FileManifest
    {
        //目标bundle的资源目录
        public string assetFolderPath;
        public new BundleManifest Clone()
        {
            var folderManifest = ScriptableObject.CreateInstance<BundleManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            folderManifest.assetFolderPath = this.assetFolderPath;

            var allFileInfos = this.allFileInfos;

            for (int i = 0; i < allFileInfos.Count; i++)
            {
                folderManifest.allFileInfos.Add(allFileInfos[i]);
            }

            return folderManifest;
        }

        /// <summary>
        /// 简单clone folderManifest对象  (不clone  allFileInfos文件列表)
        /// </summary>
        public new BundleManifest CloneWithOutAllFileInfos()
        {
            var folderManifest = ScriptableObject.CreateInstance<BundleManifest>();
            folderManifest.resNumber = this.resNumber;
            folderManifest.version = this.version;
            folderManifest.fileName = this.fileName;
            folderManifest.priority = this.priority;
            folderManifest.assetFolderPath = this.assetFolderPath;
            return folderManifest;
        }

        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendLine($"{this.GetType().FullName}(assetFolderPath={assetFolderPath},fileName={fileName},count={Count},total={totalSize},priority={priority},version={version}, resNumber={resNumber})");
            stringBuilder.AppendLine("allFileInfos:");
            foreach (var info in allFileInfos)
            {
                stringBuilder.AppendLine(info.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}