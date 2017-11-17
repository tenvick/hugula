// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;


namespace Hugula.Pool
{

    /// <summary>
    /// 引用计数器
    /// </summary>
    public class ReferenceCount : MonoBehaviour
    {
        internal int assetHashCode;

        public string assetbundle;

        internal void AddReferCount()
        {
            if (string.IsNullOrEmpty(assetbundle)) assetbundle = CUtils.GetRightFileName(this.name.Replace("(Clone)", "") + Common.CHECK_ASSETBUNDLE_SUFFIX);
            assetHashCode = LuaHelper.StringToHash(assetbundle);
            int addre = CountMananger.Add(this.assetHashCode);
#if UNITY_EDITOR || !HUGULA_RELEASE
            if (addre == -1)
            {
                Debug.LogWarningFormat("AddReferCount: name({0}) abName({1}) assetHashCode({2},count={3})refer add error ", this.name.Replace("(Clone)", ""), assetbundle, assetHashCode, addre);
            }
#endif
#if HUGULA_CACHE_DEBUG                
            else
            {
                HugulaDebug.FilterLogFormat(assetbundle, "<color=green>AddReferCount: name({0}) abName({1}) assetHashCode({2}) referCount={3} </color> ", this.name.Replace("(Clone)", ""), assetbundle, assetHashCode, addre);
            }
#endif
        }

        internal void SubtractReferCount()
        {
            var subre = CountMananger.Subtract(this.assetHashCode);
#if HUGULA_CACHE_DEBUG
            HugulaDebug.FilterLogFormat(assetbundle," SubtractReferCount: name({0}) abName({1}) assetHashCode({2})  referCount={3} ", this.name, assetbundle, assetHashCode, subre);
#endif
#if UNITY_EDITOR ||!HUGULA_RELEASE
            if (subre == -1)
            {
                Debug.LogWarningFormat("SubtractReferCount: name({0}) abName({1}) assetHashCode({2},count={3}) refer delete error ", this.name, assetbundle, assetHashCode, subre);
            }
#endif
        }

        void Awake()
        {
            AddReferCount();
        }

        void OnDestroy()
        {
            SubtractReferCount();
        }

    }
}