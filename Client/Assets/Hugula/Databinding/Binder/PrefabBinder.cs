using System.Collections;
using System.Collections.Generic;
using Hugula;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Databinding.Binder
{

    ///<summary>
    /// prefab的加载和卸载绑定。
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
                    if (m_Instance || string.Empty.Equals(value)) //清空
                    {
                        Unload();
                        m_AssetName = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        m_AssetName = value;
                        LoadPrefab(m_AssetName);
                    }
                }

            }
        }

        #region  protected method

        private GameObject m_Instance;
        private int m_LoadId = -1;
        void LoadPrefab(string assetName)
        {
            if (!string.IsNullOrEmpty(assetName))
            {
                // var abName = assetBundleName;
                m_LoadId = 0;
                ResLoader.InstantiateAsync(assetName, OnCompleted, OnEnd, assetName, this.GetComponent<Transform>());
            }
        }

        void OnCompleted(GameObject data, object arg)
        {
            m_LoadId = -1;
            if (!assetName.Equals(arg))
            {
                ResLoader.ReleaseInstance(data);
                return;
            }
            m_Instance = data;
            // if (data is GameObject)
            // {
            //     m_Instance = GameObject.Instantiate<GameObject>((GameObject)data, this.GetComponent<Transform>(), false);
            var transform = m_Instance.GetComponent<Transform>();
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            // }
        }

        void OnEnd(object data, object arg)
        {
            m_LoadId = -1;
        }

        void Unload()
        {
            if (m_Instance)
            {
                ResLoader.ReleaseInstance(m_Instance);
                m_Instance = null;
            }
        }

        #endregion

        protected override void OnDestroy()
        {
            Unload();
            base.OnDestroy();
        }
    }
}