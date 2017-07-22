// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections.Generic;

namespace Hugula
{
    /// <summary>
    /// 引用挂接
    /// </summary>
    [SLua.CustomLuaClass]
    public class ReferGameObjects : MonoBehaviour
    {

        /// <summary>
        /// for index
        /// </summary>
        [SLua.DoNotToLua]
        [HideInInspector]
        public List<string> names;//= new List<string>();

    //public GameObject[] refers ;//=new List<GameObject>();

        [HideInInspector]
        public Object[] monos;// = new List<Behaviour>();

        public object userObject;

        public bool userBool;

        public int userInt;

        /// <summary>
        /// 缓存使用的hash值
        /// </summary>
        internal int cacheHash;

        public Object Get(string n)
        {
            int index = names.IndexOf(n);
            if (index == -1)
            {
                //Debug.LogWarning(gameObject.name + "ReferGameObjects : not found the key [" + n + "]");
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
                //Debug.LogWarning(gameObject.name + "ReferGameObjects : not found the key [" + index + "]");
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