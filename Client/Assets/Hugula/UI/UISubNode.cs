using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hugula.UI 
{

    public class UISubNode : MonoBehaviour
    {
        [Tooltip("挂接的容器名,与UISubContainer中的属性名字对应")]
        public string uiSubContainerName;
        // Start is called before the first frame update
        IEnumerator Start()
        {
            Transform subTrans = null;

            while ((subTrans = UISubManager.instance?.GetContainer(uiSubContainerName)) == null)
                yield return null;

            SetRootParent(subTrans);
        }

        // void OnDestroy()
        // {
        //     // Transform subTrans = UISubManager.instance?.GetContainer(uiSubContainerName);
        //     // if(subTrans)
        // }

        /// <summary>
        /// 设置父节点
        /// </summary>
        private void SetRootParent(Transform subTrans)
        {
            Transform ts = transform;
            ts.SetParent(subTrans);

            //初始化位置
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            RectTransform rectTransform = ts.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }

    }
}