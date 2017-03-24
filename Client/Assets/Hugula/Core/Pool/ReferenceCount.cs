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
            if (string.IsNullOrEmpty(assetbundle)) assetbundle = CUtils.GetRightFileName(this.name.Replace("(Clone)", "") + ".u3d");

            assetHashCode = LuaHelper.StringToHash(assetbundle);
            int addre = CountMananger.Add(this.assetHashCode);
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            if (addre == -1)
            {
                Debug.LogWarningFormat("ReferenceCount: name({0}) abName({1}) assetHashCode({2})refer add error ", this.name.Replace("(Clone)", ""), assetbundle,assetHashCode);
            }
#endif
#if HUGULA_CACHE_DEBUG                
                else{
                    Debug.LogFormat("<color=green>ReferenceCount: name({0}) abName({1}) assetHashCode({2}) referCount={3} </color> ", this.name.Replace("(Clone)",""),assetbundle,assetHashCode,addre);
                }
#endif

        }

        void OnDestroy()
        {
#if HUGULA_CACHE_DEBUG
            Debug.LogWarning(string.Format(" OnDestroy() ReferenceCount -- : name({0}) abName({1}) assetHashCode({2}) refer ", this.name,assetbundle,assetHashCode));
#endif
            var subre = CountMananger.Subtract(this.assetHashCode);
#if UNITY_EDITOR || HUGULA_CACHE_DEBUG
            if (subre == -1)
            {
                Debug.LogWarning(string.Format("ReferenceCount: name({0}) abName({1}) assetHashCode({2}) refer delete error ", this.name, assetbundle,assetHashCode));
            }
#endif
        }

    }
}