using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Hugula.UIComponents;

namespace PSDUIImporter
{
    public sealed class LoopScrollViewComponentImport : BaseComponentImport<LoopScrollRect>
    {

        public LoopScrollViewComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(Layer layer, LoopScrollRect target, GameObject parent,int posSizeLayerIndex)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            var bgImg = target.GetComponent<Image>();
            DrawBackgroundImage(layer,bgImg,target.gameObject);
    

            // if (TryGetSizePostion(layer, out size, out position, out var index))
            // {
            //     //设置剪裁区域
            //     var mask = target.GetComponentInChildren<Mask>(); //寻找mask
            //     var maskRectTrans = mask.GetComponent<RectTransform>();
            //     // var rect = new Rect(position.x, position.y, size.width, size.height);
            //     // var dtMin = rect.min - lsvRect.min;
            //     // var dtMax = rect.max - lsvRect.max;
            //     // Debug.LogFormat("TryGetSizePostion {0},size={1},position={2} ", layer.name, size, position);
            //     // maskRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, Mathf.Abs(dtMin.x), 0);
            //     // maskRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, Mathf.Abs(dtMax.x));
            //     // maskRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, Mathf.Abs(dtMax.y));
            //     // maskRectTrans.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, Mathf.Abs(dtMin.y));

            //     // maskRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.width);
            //     // maskRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.height);

            //     // PSDImportUtility.SetAnchorMiddleCenter(maskRectTrans);
            //     // maskRectTrans.sizeDelta = new Vector2(size.width, size.height);
            //     // maskRectTrans.anchoredPosition = GetLocalAnchoredPosition(position, maskRectTrans);
            // }

            //fill ItemSource
            var itemSource = target.itemSource;
            for (var i = 0; i < layer.layers.Length; i++)
            {
                var l1 = layer.layers[i];
                if (!PSDImportUtility.NeedDraw(l1)) continue;

                if (itemSource != null && (ctrl.CompareLayerType(l1.type, ComponentType.BindableContainer) || ctrl.CompareLayerType(l1.type, ComponentType.Default)))
                {
                    l1.type = ComponentType.BindableContainer; //强制为.BindableContainer
                    ctrl.DrawLayer(l1, itemSource.gameObject, itemSource.transform.parent.gameObject);
                    itemSource = null;
                }
            }

            ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }

        protected override  void CheckAddBinder(Layer layer, Hugula.UIComponents.LoopScrollRect loopScrollRect)
        {
            var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.LoopScrollRectBinder>(loopScrollRect.gameObject);
            if (binder != null)
            {
                var parent = loopScrollRect.transform.parent;
                if (parent && PSDImportUtility.baseFilename != parent.name)
                    binder.SetBinding(layer.name, null, "context", BindingMode.OneWay, string.Empty, string.Empty);

            }

        }

         protected override LoopScrollRect LoadAndInstant(Layer layer, GameObject parent)
        {
            var txt = PSDImportUtility.LoadAndInstant<LoopScrollRect>(PSDImporterConst.ASSET_PATH_LOOP_SCROLLVIEW, layer.name, parent);
            return txt;
        }

    }
}