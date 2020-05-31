using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Framework;
using System;
using Hugula;

namespace Hugula.Audio
{

    public class AudioClipMappingManager : AssetBundleMappingManager<AudioClipMappingManager, AudioClip>
    {
        public const string MAPPING_ROOT_NAME = "audioclip_mapping";

        protected override string GetMappingAssetName()
        {
            return MAPPING_ROOT_NAME;
        }

    }
}
