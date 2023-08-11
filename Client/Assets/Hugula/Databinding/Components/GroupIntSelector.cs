using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.UIComponents
{
    public class GroupIntSelector : BindableObject
    {
        [Serializable]
        public class GroupItems
        {
           public GameObject[] items;
        }

        private int m_lastIdx = -1;
        [SerializeField]
        private int m_GroupIndex = -1;
        public int activeGroup
        {
            get { return m_GroupIndex; }
            set
            {
                m_lastIdx = m_GroupIndex;
                m_GroupIndex = value;
                SetGroupActive();
            }
        }
        public List<GroupItems> m_Group = new List<GroupItems>();

        void SetGroupActive()
        {
            GameObject[] gobjects;
            if (m_lastIdx >= 0 && m_lastIdx < m_Group.Count)
            {
                gobjects = m_Group[m_lastIdx].items;
                foreach (var g in gobjects)
                {
                    if (g != null)
                    {
                        LuaHelper.DelayDeActive(g.GetComponent<Transform>());
                    }
                }                    
            }

            if (m_GroupIndex >= 0 && m_GroupIndex < m_Group.Count)
            {
                gobjects = m_Group[m_GroupIndex].items;
                foreach (var g in gobjects)
                    LuaHelper.Active(g);
            }

        }

        void Start()
        {
            SetGroupActive();
        }

    }
}