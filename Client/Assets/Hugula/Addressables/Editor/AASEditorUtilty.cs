using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;

namespace HugulaEditor
{
    /// <summary>
    /// addressables编辑器工具
    /// </summary>
    public class AASEditorUtility
    {

        static List<AddressableAssetGroupTemplate> DEFALT_GROUP_SCHEMA;

        public static List<AddressableAssetGroupTemplate> DefaltGroupSchema
        {
            get
            {
                if (DEFALT_GROUP_SCHEMA == null)
                {//                
                    DEFALT_GROUP_SCHEMA = new List<AddressableAssetGroupTemplate>();
                    var setting = LoadAASSetting();
                    var groupTempObjs = setting.GroupTemplateObjects;
                    foreach (var temp in groupTempObjs)
                    {
                        if (temp is AddressableAssetGroupTemplate)
                        {
                            DEFALT_GROUP_SCHEMA.Add((AddressableAssetGroupTemplate)temp);
                        }
                    }
                }

                return DEFALT_GROUP_SCHEMA;
            }
        }

        // static AddressableAssetSettings m_AASSeting;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AddressableAssetSettings LoadAASSetting()
        {
            var setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            return setting;
        }

        // Start is called before the first frame update
        /// <summary>
        /// 创建寻找指导名字的group 
        /// 当autoCreate为true时候不存在就创建
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="Create"></param>
        /// <returns></returns>
        public static AddressableAssetGroup FindGroup(string groupName, AddressableAssetGroupTemplate groupTemplate = null)
        {
            var setting = LoadAASSetting();
            var group = setting.FindGroup(groupName);
            if (group == null && groupTemplate != null)
            {
                group = setting.CreateGroup(groupName, false, false, true, null, groupTemplate.GetTypes());
                groupTemplate.ApplyToAddressableAssetGroup(group);
            }
            return group;
        }


        /// <summary>
        /// 将assetpath路径的内容放入assetgroup中
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="group"></param>
        /// <param name="assetPaths"></param>
        /// <param name="label"></param>
        public static void SetGroupAddress(AddressableAssetSettings setting, AddressableAssetGroup group, List<string> assetPaths, string label = null)
        {
            foreach (var str in assetPaths)
            {
                var guid = AssetDatabase.AssetPathToGUID(str); //获得GUID

                var entry = setting.CreateOrMoveEntry(guid, group); //通过GUID创建entry
                entry.SetAddress(System.IO.Path.GetFileNameWithoutExtension(str));
                if (!string.IsNullOrEmpty(label))
                    entry.SetLabel(label, true);
            }
        }
    }
}