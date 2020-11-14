using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using Hugula.Framework;

namespace Hugula.Atlas
{

    public class AtlasManager : MappingManager<AtlasManager>//: Singleton<AassetBundleMappingAsset>
    {
        public const string ATLAS_MAPPING_ROOT_NAME = "atlas_mapping_root";
        private const string LEFT_S = "[";
        private const string RIGHT_S = "]";

        private static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        public static string GetAtlasKey(string spriteName)
        {
            var atlas = GetKey(spriteName);
            if (!string.IsNullOrEmpty(atlas))
            {
                sb.Clear();
                sb.Append(atlas);
                sb.Append(LEFT_S);
                sb.Append(spriteName);
                sb.Append(RIGHT_S);
                spriteName = sb.ToString();
            }
            return spriteName;
        }

        protected override string GetMappingAssetName()
        {
            return ATLAS_MAPPING_ROOT_NAME;
        }

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var ins = AtlasManager.instance;
            ins.LoadMappingAsset();
        }
    }
}
