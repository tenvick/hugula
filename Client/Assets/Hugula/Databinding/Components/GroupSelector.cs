using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using UnityEngine;

namespace Hugula.UIComponents {
    /// <summary>
    /// 根据inverse的值来激活指定的group
    ///
    /// inverse为true激活inverseGroup
    /// </summary>
    public class GroupSelector : BindableObject {
        [Tooltip ("inverse 为true激活inverseGroup")]
        [SerializeField]
        private bool m_Inverse;
        public bool inverse {
            get { return m_Inverse; }
            set {
                m_Inverse = value;
                SetGroupActive ();
            }
        }

        public GameObject[] group;

        public GameObject[] inverseGroup;

        void SetGroupActive () {
            bool flag = m_Inverse;

            foreach (var g in group)
                g.SetActive (!m_Inverse);
            foreach (var g in inverseGroup)
                g.SetActive (m_Inverse);

        }

        void Start () {
            SetGroupActive ();
        }

    }
}