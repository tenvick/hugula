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
namespace  HugulaEditor.Addressable
{
    public static class FolderManifestExtention
    {
        ///<summary>
        /// 添加文件信息判断重名
        ///</summary>
        public static FileResInfo AddFileInfo(this FolderManifest self,string name, uint crc,uint size,string relativeDir = null)
        {
            var info = new FileResInfo(name,crc,size);
            self.allFileInfos.Add(info);
            return info;
        }

        
        public static void WriteToFile(this FolderManifest self, string path)
        {
            using (StreamWriter wr = new StreamWriter(path))
            {
                wr.Write(self.ToString());
            }
        }

        public static void SaveAsset(this FolderManifest self,string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
                UnityEditor.AssetDatabase.Refresh();
            }
            UnityEditor.AssetDatabase.CreateAsset(self, path);
            Debug.Log(self.ToString());
            Debug.Log($"save to path:{path} ");
            UnityEditor.AssetDatabase.Refresh();
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