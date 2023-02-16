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
  
        protected override void DrawTargetLayer(int index, Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            RectTransform rectTransform = target;

            if (layer.layers == null && (layer.miniType == ComponentType.Image || layer.miniType == ComponentType.Text)) //单张image或者text模板
            {
                Layer layer1 = layer;
                if (layer1.size != null && layer1.position != null)
                {
                    var cache =PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform, layer1.size);
                    SetRectTransformPosition(rectTransform, layer1.position);
                    PSDImportUtility.SetAnchorFromCache(rectTransform,cache);
                }
            }
            else
            {
                if(posSizeLayerIndex == -1)
                    posSizeLayerIndex = GetBackgroundImageLayer(layer);

                if (posSizeLayerIndex != -1)
                {
                    Layer layer1 = layer.layers[posSizeLayerIndex];
                    if (layer1.size != null && layer1.position != null)
                    {
                        //check缩放
                        var sizeData = rectTransform.sizeDelta;
                        if (sizeData != Vector2.zero)
                        {
                            Vector3 scale = Vector3.one;
                            scale.x = layer1.size.width / sizeData.x;
                            scale.y = layer1.size.height / sizeData.y;
                            rectTransform.localScale = scale;
                            //
                        }

                        SetRectTransformSizeAndPos(rectTransform,layer1);
                    }
                }
            }
        }

    }
}