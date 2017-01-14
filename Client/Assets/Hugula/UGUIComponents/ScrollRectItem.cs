// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Hugula.UGUIExtend
{
    /// <summary>
    /// 滚动列表
    /// </summary>
    [SLua.CustomLuaClass]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ScrollRectItem : MonoBehaviour
    {

        /// <summary>
        /// for index
        /// </summary>
        [SLua.DoNotToLua]
        [HideInInspector]
        public List<string> names = new List<string>();

        public RectTransform rectTransform;

        public GameObject[] refers;
        [HideInInspector]
        public Object[] monos;

        public object data;

        public float fdata;

        //public int idata;

        //public string sdata;

        // Use this for initialization
        void Start()
        {
            if (rectTransform == null)
                rectTransform = this.GetComponent<RectTransform>();

        }

        public Object Get(string n)
        {
            int index = names.IndexOf(n);
            if (index == -1)
            {
                Debug.LogWarning(gameObject.name + "ScrollRectItem : not found the key [" + n + "]");
                return null;
            }
            else
                return Get(index + 1);
        }

        public Object Get(int index)
        {
            index = index - 1;
            if (index >= 0 && index < monos.Length)
            {
                return monos[index];
            }
            else
            {
                Debug.LogWarning(gameObject.name + "ScrollRectItem : not found the key [" + index + "]");
                return null;
            }
        }

        /// <summary>
        /// monos的长度
        /// </summary>
        public int Length
        {
            get
            {
                if (monos != null)
                    return monos.Length;
                else
                    return 0;
            }
        }
    }
}