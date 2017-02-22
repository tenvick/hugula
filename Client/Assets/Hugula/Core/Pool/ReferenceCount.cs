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
            int addre = CountMananger.Add(this.assetHashCode);
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
                if (addre == -1)
                {
                    Debug.LogWarningFormat("ReferenceCount: name({0}) abName({1}) refer add error ", this.name.Replace("(Clone)",""),assetbundle);
                }
#endif
#if HUGULA_CACHE_DEBUG                
                else{
                    Debug.LogFormat("<color=green>ReferenceCount: name({0}) abName({1}) referCount={2} </color> ", this.name.Replace("(Clone)",""),assetbundle,addre);
                }
#endif

        }

        void OnDestroy()
        {
            bool subre = CountMananger.Subtract(this.assetHashCode);
 #if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            if (!subre)
            {
                Debug.LogWarning(string.Format("ReferenceCount: name({0}) abName({1}) refer delete error ", this.name,assetbundle));
            }
#endif
        }

    }
}