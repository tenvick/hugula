// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//

using UnityEngine;
using System.Collections;
using Hugula.Utils;
using Hugula.Loader;

namespace Hugula.Pool
{

    /// <summary>
    /// 引用计数器
    /// </summary>
    public class ReferenceCount : MonoBehaviour
    {
        internal int assetHashCode;

        public string assetbundle;

        void Awake()
        {
            if(string.IsNullOrEmpty(assetbundle)) assetbundle = CUtils.GetRightFileName(this.name.Replace("(Clone)","")+".u3d");

            assetHashCode = LuaHelper.StringToHash(assetbundle);

            if (!CountMananger.Add(this.assetHashCode))
            {
                Debug.LogWarning(string.Format("ReferenceCount: name({0}) abName({1}) refer add error ", this.name.Replace("(Clone)",""),assetbundle));
            }
        }

        void OnDestroy()
        {
            if (!CountMananger.Subtract(this.assetHashCode))
            {
                Debug.LogWarning(string.Format("ReferenceCount: name({0}) abName({1}) refer delete error ", this.name,assetbundle));
            }
        }

    }
}