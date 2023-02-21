using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public sealed class MobanComponentImport : CustomerComponentImport
    {

        public MobanComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        //对于老的模板不需要设置尺寸和位置
        protected override int DrawSizeAndPos(Layer layer, GameObject target, bool autoPosition = true, bool autoSize = true)
        {
            int bgIdx;
            if (TryGetSizePostion(layer, out var size, out var position, out bgIdx))//自定义组件需要控制布局
            {
                var l1 = layer.layers[bgIdx];
                // var rectTransform = target.GetComponent<RectTransform>();
                // PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                // if (autoSize) SetRectTransformSize(rectTransform, l1.size);
                // if (autoPosition) SetRectTransformPosition(rectTransform, l1.position);
                //l1.target = target;
                return bgIdx;
            }
            return -1;
        }

        protected override void DrawTargetLayer(int index, Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            RectTransform rectTransform = target;

            if (layer.layers == null && (layer.miniType == ComponentType.Image || layer.miniType == ComponentType.Text)) //单张image或者text模板
            {
                Layer layer1 = layer;
                if (layer1.size != null)
                {
                    var cache = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform, layer1.size);
                    SetRectTransformPosition(rectTransform, layer1.position);
                    PSDImportUtility.SetAnchorFromCache(rectTransform, cache);
                }
            }
            else
            {
                if (posSizeLayerIndex == -1)
                    posSizeLayerIndex = GetBackgroundImageLayer(layer);

                if (posSizeLayerIndex != -1)
                {
                    Layer layer1 = layer.layers[posSizeLayerIndex];
                    if (layer1.size != null)
                    {
                        //check缩放
                        var sizeData = rectTransform.sizeDelta;
                        if (sizeData != Vector2.zero)
                        {
                            var rect = rectTransform.rect;
                            Vector3 scale = Vector3.one;
                            scale.x = layer1.size.width / rect.width;
                            scale.y = layer1.size.height / rect.height;
                            rectTransform.localScale = scale;
                            //
                            if (scale != Vector3.one)
                            {
                                var cache = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                                SetRectTransformPosition(rectTransform, layer1.position);
                                PSDImportUtility.SetAnchorFromCache(rectTransform, cache);
                                return;
                            }

                        }

                        SetRectTransformSizeAndPos(rectTransform, layer1);
                    }
                }
            }
        }

    }
}