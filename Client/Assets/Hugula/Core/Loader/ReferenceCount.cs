using UnityEngine;
using System.Collections;

/// <summary>
/// 引用计数器
/// </summary>
[SLua.CustomLuaClass]
public class ReferenceCount : MonoBehaviour
{
    public string assetBundleName;

    internal int assetHashCode;

    void Awake()
    {
        if (string.IsNullOrEmpty(assetBundleName))
            assetBundleName = this.name;

        //if(assetHashCode == 0)
        assetHashCode = LuaHelper.StringToHash(assetBundleName);

        if (!CountMananger.Add(this.assetHashCode))
        {
            Debug.LogWarning(string.Format("ReferenceCount: {0} {1} refer add error ", this.name, this.assetHashCode));
        }
    }

    void OnDestroy()
    {
        if (!CountMananger.Subtract(this.assetHashCode))
        {
            Debug.LogWarning(string.Format("ReferenceCount: {0} {1} refer delete error ", this.name, this.assetHashCode));
        }
    }

    ///// <summary>
    ///// 设置gameobject管理的assetbundle.name
    ///// </summary>
    ///// <param name="assetBundleName"></param>
    //public void SetAssetBundleName(string assetBundleName)
    //{
    //    assetHashCode = LuaHelper.StringToHash(assetBundleName);
    //}
}
