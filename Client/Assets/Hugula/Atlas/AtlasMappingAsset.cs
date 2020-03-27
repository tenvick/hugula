using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hugula.Atlas
{
    public class AtlasMappingAsset : ScriptableObject
    {
        public const string ATLAS_MAPPING_ROOT_NAME = "atlas_mapping_root";
        public List<int> allSprites;
        public List<string> atlasNames;
       
    }
}