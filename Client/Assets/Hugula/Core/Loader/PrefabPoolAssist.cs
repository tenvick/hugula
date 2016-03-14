using UnityEngine;
using System.Collections;

/// <summary>
/// 对象池辅助 添加类。
/// 在原始对象实例化时候自动加入对象池
/// </summary>
public class PrefabPoolAssist : MonoBehaviour {

    #region instance
    /// <summary>
    /// 缓存key 推荐使用GamObject.name
    /// </summary>
    public string cacheKey;

    /// <summary>
    /// hash值
    /// </summary>
    internal int cacheHash;

    /// <summary>
    /// 缓存的类型，用于资源回收策略
    /// </summary>
    public byte cacheType;

    //用于标记是否是原始对象
    internal bool isMain;

    void Awake()
    {
        if (string.IsNullOrEmpty(cacheKey))
            cacheKey = this.name;

        cacheHash = LuaHelper.StringToHash(cacheKey);

        if (!PrefabPool.ContainsKey(cacheHash))
        {
            PrefabPool.Add(cacheHash, this.gameObject, this.cacheType);
            isMain = true;
        }
    }

    //void OnDestroy()
    //{
    //    if (isMain && PrefabPool.ContainsKey(cacheHash)) PrefabPool.Remove(cacheHash); //this is the problem
    //}

    #endregion
}
