using UnityEngine;
using System.IO;
using Hugula.Utils;

namespace Hugula.ResUpdate
{

    public static class FolderManifestRuntionExtention
    {

        /// <summary>
        /// 标记zip文件下载完成
        /// </summary>
        public static void MarkZipDone(this FolderManifest self)
        {
            if (!Directory.Exists(CUtils.GetRealPersistentDataPath()))
                Directory.CreateDirectory(CUtils.GetRealPersistentDataPath());

            if (!File.Exists(self.zipMarkPathName)) File.Create(self.zipMarkPathName);
        }

        /// <summary>
        /// 获取zip文件解压后的目录
        /// </summary>
        public static string GetZipOutFolderPath(this FolderManifest self)
        {
            return CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), self.zipName);
        }
    }

}