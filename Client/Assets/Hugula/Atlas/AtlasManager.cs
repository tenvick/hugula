using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Framework;

namespace Hugula.Atlas
{

    public class AtlasManager : AssetBundleMappingManager<AtlasManager, Sprite>//: Singleton<AassetBundleMappingAsset>
    {
        public const string ATLAS_MAPPING_ROOT_NAME = "atlas_mapping_root";

        protected override string GetMappingAssetName()
        {
            return ATLAS_MAPPING_ROOT_NAME;
        }
        
    }
}
