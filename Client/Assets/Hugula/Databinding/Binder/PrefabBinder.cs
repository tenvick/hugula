using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;

namespace Hugula.Databinding.Binder
{

    ///<summary>
    /// prefab的加载和卸载绑定。
    /// 注意绑定顺序assetBundleName在前面，assetName在后
    ///</summary>
    public class PrefabBinder : BindableObject
    {
        private string m_AssetName;
        public string assetName
        {
            get { return m_AssetName; }
            set
            {
                if (!string.Equals(m_AssetName, value))
                {
                    if (m_LoadId > 0)
                        ResourcesLoader.StopComplete(m_LoadId);

                    if (m_Instance || string.Empty.Equals(value)) //清空
                    {
                        Unload(m_AssetName);
                    }

                    m_AssetName = value;
                    LoadPrefab(m_AssetName);
                }

            }
        }

        private string m_AssetBundleName;
        public string assetBundleName
        {
            get
            {
                if (string.IsNullOrEmpty(m_AssetBundleName) && !string.IsNullOrEmpty(assetName))
                    return assetName.ToLower() + Common.CHECK_ASSETBUNDLE_SUFFIX;
                else
                    return m_AssetBundleName;
            }
            set { m_AssetBundleName = value; }
        }

        // public Transform 
        #region  protected method

        private GameObject m_Instance;
        private int m_LoadId = -1;
        void LoadPrefab(string assetName)
        {
            if (!string.IsNullOrEmpty(assetName))
            {
                var abName = assetBundleName;
                m_LoadId = ResourcesLoader.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject), OnCompleted, OnEnd);
            }
        }

        void OnCompleted(object data, object arg)
        {
            m_LoadId = -1;
            if (data is GameObject)
            {
                m_Instance = GameObject.Instantiate<GameObject>((GameObject)data, this.GetComponent<Transform>(), false);
                var transform = m_Instance.GetComponent<Transform>();
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;
            }
        }

        void OnEnd(object data, object arg)
        {
            m_LoadId = -1;
        }

        void Unload(string abName)
        {
            if (m_Instance)
            {
                GameObject.Destroy(m_Instance);
                m_Instance = null;
            }
            if (!string.IsNullOrEmpty(abName))
                CacheManager.Subtract(abName);
        }

        #endregion

        protected override void OnDestroy()
        {
            if (m_Instance && string.IsNullOrEmpty(m_AssetName)) //清空
            {
                Unload(m_AssetName);
            }
            base.OnDestroy();
        }
    }
}