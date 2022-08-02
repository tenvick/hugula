using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.AddressableAssets.Settings.GroupSchemas
{
    /// <summary>
    /// Schema for content update packing.
    /// </summary>
    //  [CreateAssetMenu(fileName = "ContentUpdateGroupSchema.asset", menuName = "Addressables/Group Schemas/Content Update")]
    [DisplayName("Hugula ResUpdate Packing")]
    public class HugulaResUpdatePacking : AddressableAssetGroupSchema
    {
        public enum PackingType
        {
            /// <summary>
            /// streamingAsset中的资源，如果是aab则放在install-time 根据google规则整体包大小应该小于1G，IOS没有此规则。
            /// </summary>
            streaming,
            /// <summary>
            /// 第一次启动时候下载，或者根据需求下载，todo:google play放入fast-follow。
            /// </summary>
            fast,
            /// <summary>
            /// 根据需要下载，todo:google play放入on-demand。
            /// </summary>
            demand,
            /// <summary>
            /// 自定义包，根据需要下载，多模块支持。
            /// </summary>
            custom
        }

        public PackingType packingType = PackingType.streaming ;
        public string customName;
        public int priority;
    }
}