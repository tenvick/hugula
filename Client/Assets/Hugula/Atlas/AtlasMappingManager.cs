using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;

namespace Hugula.Atlas
{

    public class AtlasMappingManager
    {
        static AtlasMappingAsset m_Instance;
        public static AtlasMappingAsset instance
        {
            get
            {
                if (m_Instance == null)
                {
                    string atlas_mapping_name = AtlasMappingAsset.ATLAS_MAPPING_ROOT_NAME;
                    m_Instance = ResourcesLoader.LoadAsset<AtlasMappingAsset>( atlas_mapping_name + Common.CHECK_ASSETBUNDLE_SUFFIX,atlas_mapping_name);
                }

                return m_Instance;
            }
        }

        public static string GetSpriteBundle(string spriteName)
        {
            if (instance != null)
            {
                int idx = UnityEngine.Animator.StringToHash(spriteName);
                int fIdx = instance.allSprites.IndexOf(idx);
                if (fIdx >= 0)
                {
                    return m_Instance.atlasNames[fIdx];
                }
            }

            return null;
        }
    }
}
