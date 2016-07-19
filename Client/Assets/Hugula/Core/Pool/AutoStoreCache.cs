// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;

namespace Hugula.Pool
{
    /// <summary>
    /// 当目标SelfActive==false的时候 自动归还到对象池
    /// </summary>
    public class AutoStoreCache : MonoBehaviour
    {

        public ReferGameObjects refer;

        ///// <summary>
        ///// 自动放回对象到池子
        ///// </summary>
        //[Tooltip("自动放回对象到池子")]
        //public bool storeCacheOnDisable = true;
        private bool isStart = false;

        // Use this for initialization
        void Start()
        {
            if (refer == null) refer = GetComponent<ReferGameObjects>();
            isStart = true;
        }

        void OnDisable()
        {
            if (isStart)
            {
                //Debug.Log(string.Format("AutoStoreCache name = {0} cacheHash={1} enable = {2} ", this.name, refer != null ? refer.cacheHash : 0,this.enabled));
                if (refer == null) refer = GetComponent<ReferGameObjects>();
                if (refer != null) PrefabPool.StoreCache(refer);
            }
        }

    }

}