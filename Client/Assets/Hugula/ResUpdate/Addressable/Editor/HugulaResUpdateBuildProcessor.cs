using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HugulaEditor.Addressable
{
    /// <summary>
    /// Moves custom asset pack data from their default build location <see cref="BuildScriptPlayAssetDelivery"/> to their correct player build data location.
    /// For an Android App Bundle, bundles assigned to a custom asset pack must be located in their {asset pack name}.androidpack directory in the Assets folder.
    /// The 'CustomAssetPacksData.json' file is also moved to StreamingAssets.
    ///
    /// This script executes before the <see cref="AddressablesPlayerBuildProcessor"/> which moves all Addressables data to StreamingAssets.
    /// </summary>
    public class HugulaResUpdateBuildProcessor : IPreprocessBuildWithReport
    {
        /// <summary>
        /// Returns the player build processor callback order.
        /// </summary>
        public int callbackOrder
        {
            get { return 0; }
        }

        ///<summary>
        /// Initializes temporary build data.
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && EditorUserBuildSettings.buildAppBundle) //移动
            {
                MoveDataForAppBundleBuild();
            }
            DelFileInUpdatePackage();
        }

        void MoveDataForAppBundleBuild()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                Hugula.HugulaSetting.instance.aabFastEnable = true;
                var hotResGenSharedData = new HugulaEditor.ResUpdate.HotResGenSharedData();
                var task = new HugulaEditor.ResUpdate.MoveDataToAppBundleFolder();
                Debug.Log($"run task ({task.name})");
                Hugula.Utils.CUtils.DebugCastTime($"Run Task ({task.name})");
                task.Run(hotResGenSharedData);
                Hugula.Utils.CUtils.DebugCastTime($"Run Task ({task.name}) end");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception occured when moving data for an app bundle build: {e.Message}.");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        ///<summary>
        /// 删除已经打包好的资源防止重复添加到最终包里
        /// </summary>
        void DelFileInUpdatePackage()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                var hotResGenSharedData = new HugulaEditor.ResUpdate.HotResGenSharedData();
                var task = new HugulaEditor.ResUpdate.DelFolderManifestFiles();
                Debug.Log($"run task ({task.name})");
                task.Run(hotResGenSharedData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception occured when moving data for an app bundle build: {e.Message}.");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}