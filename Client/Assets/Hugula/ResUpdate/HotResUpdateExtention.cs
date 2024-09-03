using UnityEngine;
using System.IO;
using Hugula.Utils;
using System.Threading.Tasks;
using UnityEngine.Networking;


namespace Hugula.ResUpdate
{

    public static class FolderManifestRuntionExtention
    {

        /// <summary>
        /// 标记zip文件下载完成
        /// </summary>
        public static async void MarkZipDone(this FolderManifest self)
        {
            if (!Directory.Exists(CUtils.GetRealPersistentDataPath()))
                Directory.CreateDirectory(CUtils.GetRealPersistentDataPath());

            if (!File.Exists(self.zipMarkPathName)) 
                File.Create(self.zipMarkPathName); //先标记完成，在覆盖列表
            else
                Debug.LogWarning("MarkZipDone file is exists " + self.zipMarkPathName);

            var zipOutPath = self.GetZipOutFolderPath();
            zipOutPath = Path.Combine(zipOutPath, self.fileName + Common.CHECK_ASSETBUNDLE_SUFFIX);

            var zipOutPathName = Path.Combine(zipOutPath, self.fileName);
            var zipPathName = Path.Combine(CUtils.GetRealStreamingAssetsPath(), self.fileName + Common.CHECK_ASSETBUNDLE_SUFFIX);

            await CopyFileToPersistentDataAsync(zipPathName, zipOutPath);

        }

        static async Task CopyFileToPersistentDataAsync(string srcPath, string destPath)
        {
            FileHelper.CheckCreateFilePathDirectory(destPath);

            // #if UNITY_ANDROID  && !UNITY_EDITOR
            srcPath = CUtils.CheckWWWUrl(srcPath);
            UnityWebRequest request = UnityWebRequest.Get(srcPath);
            var op = request.SendWebRequest();
            while (!op.isDone)
            {
                await Task.Yield();
            }

            if (request.responseCode != 200)
            {
                Debug.LogErrorFormat("CopyFileToPersistentDataAsync error code = {0},error = {1}. src={2},dest={3}", request.responseCode, request.error, srcPath, destPath);
            }
            else
            {
                try
                {

                    byte[] data = request.downloadHandler.data;
                    File.WriteAllBytes(destPath, data);
#if !HUGULA_NO_LOG
                Debug.Log($"File ({srcPath}) successfully copied to:{destPath} " );
#endif
                }
                catch (System.Exception e)
                {
                    Debug.LogErrorFormat("CopyFileToPersistentDataAsync error = {0}. src={1},dest={2}", e.Message, srcPath, destPath);
                }

            }

            // #else
            //             if (File.Exists(srcPath))
            //             {
            //                 using (var srcStream = File.OpenRead(srcPath))
            //                 using (var destStream = File.Create(destPath))
            //                 {
            //                     await srcStream.CopyToAsync(destStream);
            //                 }

            //                 Debug.Log("File successfully copied to: " + destPath);

            //             }
            // #endif
        }

        /// <summary>
        /// 获取zip文件解压后的目录
        /// </summary>
        public static string GetZipOutFolderPath(this FolderManifest self)
        {
            return CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), self.fileName);
        }

    }

}