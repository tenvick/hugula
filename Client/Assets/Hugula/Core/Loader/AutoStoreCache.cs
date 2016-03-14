using UnityEngine;
using System.Collections;

/// <summary>
/// 当目标SelfActive==false的时候 自动归还到对象池
/// </summary>
public class AutoStoreCache : MonoBehaviour
{

    public ReferGameObjects refer;

    /// <summary>
    /// 自动放回对象到池子
    /// </summary>
    [Tooltip("自动放回对象到池子")]
    public bool storeCacheOnDisable = true;

    // Use this for initialization
    void Start()
    {
        if (refer == null) refer = GetComponent<ReferGameObjects>();
    }

    void OnDisable()
    {
        if (storeCacheOnDisable)
        {
            //Debug.Log(string.Format("AutoStoreCache name = {0} cacheHash={1} ", this.name, refer != null ? refer.cacheHash : 0));
            if (refer == null) refer = GetComponent<ReferGameObjects>();
            if (refer != null) PrefabPool.StoreCache(refer);
        }
    }

}
