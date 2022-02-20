using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
/// <summary>
/// addressable 引用控件
/// </summary>
public class AASAssetReference : MonoBehaviour
{
    public AssetReference reference;
    GameObject obj;
    bool isDestroy = false;
    // public bool NeedSetRectransform = false;
    void Awake()
    {
        isDestroy = false;
        if (reference != null)
        {
            reference.InstantiateAsync(transform).Completed += OnInstantiateAsync;
        }
    }

    void OnInstantiateAsync(AsyncOperationHandle<GameObject> op)
    {
        obj = op.Result;
        if (isDestroy)
        {
            ReleaseAsset();
            return;
        }
        var loadRectTransform = op.Result?.GetComponent<Transform>();
        if (loadRectTransform is RectTransform)
            ((RectTransform)loadRectTransform).anchoredPosition3D = Vector3.zero;
        else if (loadRectTransform != null)
        {
            loadRectTransform.localPosition = Vector3.zero;
            loadRectTransform.localScale = Vector3.one;
        }
    }

    void OnDestroy()
    {
        isDestroy = true;
        ReleaseAsset();
    }

    void ReleaseAsset()
    {
        if (null == obj)
            return;
        reference?.ReleaseInstance(obj);
        obj = null;
    }
}
