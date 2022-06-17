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
namespace HugulaEditor.Addressable
{
    public static class FolderManifestExtention
    {
        ///<summary>
        /// 添加文件信息判断重名
        ///</summary>
        public static FileResInfo AddFileInfo(this FileManifest self, string name, uint crc, uint size, string relativeDir = null)
        {
            var info = new FileResInfo(name, crc, size);
            self.allFileInfos.Remove(info);
            self.allFileInfos.Add(info);
            return info;
        }


        public static void WriteToFile(this FileManifest self, string path)
        {
            using (StreamWriter wr = new StreamWriter(path))
            {
                wr.Write(self.ToString());
            }
        }

        public static void SaveAsset(this FileManifest self, string path)
        {
            if (self is BundleManifest)
            {
                SaveAsset((BundleManifest)self, path);
            }
            else if (self is FolderManifest)
            {
                SaveAsset((FolderManifest)self, path);
            }
            else
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    UnityEditor.AssetDatabase.Refresh();
                }
                Debug.Log($"save to path:{path} ");
                var clone = self.CloneWithOutAllFileInfos();
                clone.allFileInfos = self.allFileInfos;
                // clone.allAddressKeys = self.allAddressKeys;
                Debug.Assert(clone != null, $"save {path} error, target asset is null");
                Debug.Log(clone.ToString());

                UnityEditor.AssetDatabase.CreateAsset(clone, path);
                UnityEditor.AssetDatabase.Refresh();
            }
        }

        public static void SaveAsset(this BundleManifest self, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                UnityEditor.AssetDatabase.Refresh();
            }
            Debug.Log($"save to path:{path} ");
            var clone = self.CloneWithOutAllFileInfos();
            clone.allFileInfos = self.allFileInfos;
            clone.assetFolderPath = self.assetFolderPath;
            Debug.Assert(clone != null, $"save {path} error, target asset is null");
            Debug.Log(clone.ToString());

            UnityEditor.AssetDatabase.CreateAsset(clone, path);
            UnityEditor.AssetDatabase.Refresh();
        }

        public static void SaveAsset(this FolderManifest self, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                UnityEditor.AssetDatabase.Refresh();
            }
            Debug.Log($"save to path:{path} ");
            var clone = self.CloneWithOutAllFileInfos();
            clone.allFileInfos = self.allFileInfos;
            clone.allAddressKeys = self.allAddressKeys;
            Debug.Assert(clone != null, $"save {path} error, target asset is null");
            Debug.Log(clone.ToString());

            UnityEditor.AssetDatabase.CreateAsset(clone, path);
            UnityEditor.AssetDatabase.Refresh();
        }


        ///<summary>
        /// 创建一个新的实例
        ///</summary>
        public static FolderManifest Create(string folderName)
        {
            // Hugula.CodeVersion.APP_NUMBER = 0;
            var folderManifest = ScriptableObject.CreateInstance<FolderManifest>();
            folderManifest.resNumber = HugulaEditor.EditorUtils.GetResNumber();
            folderManifest.version = Hugula.CodeVersion.APP_VERSION;
            folderManifest.fileName = folderName;
            return folderManifest;
        }

        ///<summary>
        /// 创建一个新的实例
        ///</summary>
        public static BundleManifest CreateBundleManifest(string fileName)
        {
            // Hugula.CodeVersion.APP_NUMBER = 0;
            var bundleManifest = ScriptableObject.CreateInstance<BundleManifest>();
            bundleManifest.resNumber = HugulaEditor.EditorUtils.GetResNumber();
            bundleManifest.version = Hugula.CodeVersion.APP_VERSION;
            bundleManifest.fileName = fileName;
            return bundleManifest;
        }
    }


    public class HotUpdateMenuItems
    {

        #region hugula hot resupdate debug
        const string kDebugLuaAssetBundlesMenu = "Hugula/Debug HotResUpdate";

        [MenuItem(kDebugLuaAssetBundlesMenu, false, 1)]
        public static void ToggleSimulateAssetBundle()
        {
            HotUpdate.isDebug = !HotUpdate.isDebug;
        }

        [MenuItem(kDebugLuaAssetBundlesMenu, true, 1)]
        public static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(kDebugLuaAssetBundlesMenu, HotUpdate.isDebug);
            return true;
        }
        #endregion
    }



}