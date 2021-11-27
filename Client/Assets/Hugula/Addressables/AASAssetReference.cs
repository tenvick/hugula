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
    	
    void Awake()
    {
        if (reference != null)
        {
            reference.InstantiateAsync(transform).Completed += OnInstantiateAsync;
        }
    }

    void OnInstantiateAsync(AsyncOperationHandle<GameObject> op)
    {
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
        reference?.ReleaseAsset();
    }
}
