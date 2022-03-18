using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using Hugula.Framework;
using System;


namespace Hugula
{

    public abstract class MappingManager<T> : Singleton<T>, IDisposable where T : MappingManager<T>, IDisposable
    {
        MappingAsset m_MappingAsset;

        bool m_IsReady = false;
        public bool isReady
        {
            get
            {
                return m_IsReady;
            }
        }

        #region  static
        public static string GetKey(string name)
        {
            if (instance != null)
            {
                return instance.m_MappingAsset?.GetKey(name);
            }
            return null;
        }

        #endregion

        public MappingManager()
        {
            // LoadMappingAsset();
        }

        public void LoadMappingAsset()
        {
            if(ResLoader.Ready)
            {
                m_IsReady = false;
                string mappingAssetName = GetMappingAssetName();
                ResLoader.LoadAssetAsync<MappingAsset>(mappingAssetName, OnCompelete, OnEnd);
            }
        }

        void OnCompelete(MappingAsset asset, object arg)
        {
            m_MappingAsset = asset;
            m_IsReady = true;
#if UNITY_EDITOR
            Debug.LogFormat("mapping asset：{0}加载完成.", asset);
#endif
        }

        void OnEnd(object asset, object arg)
        {

        }

        #region  protected
        protected abstract string GetMappingAssetName();
        #endregion


        public override void Reset()
        {
            m_MappingAsset = null;
        }
        public override void Dispose()
        {
            m_MappingAsset = null;
            base.Dispose();
        }
    }
}
