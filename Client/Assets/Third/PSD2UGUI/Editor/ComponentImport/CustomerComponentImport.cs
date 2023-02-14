using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public class CustomerComponentImport : BaseComponentImport<RectTransform>
    {

        public CustomerComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        //可能需要创建模板
        public override void DrawLayer(int index, Layer layer, GameObject target, GameObject parent, bool autoPos, bool autoSize)
        {
            RectTransform targetT = default(RectTransform);
            if (target != null)
            {
                targetT = target.GetComponent<RectTransform>();
            }
            if (targetT == null)//find in parent 
            {
                targetT = FindInParent(index, layer, parent);
            }
            if (targetT == null)
            {
                targetT = LoadAndInstant(layer, parent, index);
            }

            if (targetT == null) //创建模板
            {
                layer.importType = layer.miniType;
                ctrl.GetLayerImport(layer.importType).DrawLayer(index, layer, target, parent, autoPos, autoSize);
                //保存
                SavePrefabToCustomer(layer);
            }
            else
            {
                layer.target = targetT;
                DrawTargetLayer(index, layer, targetT, parent, DrawSizeAndPos(layer, targetT.gameObject, autoPos));
                CheckAddBinder(layer, targetT);
            }

        }

        protected override void DrawTargetLayer(int index, Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            RectTransform rectTransform = target;

            if (layer.layers == null && layer.miniType == ComponentType.Image) //单张image模板
            {
                Layer layer1 = layer;
                if (layer1.size != null && layer1.position != null)
                {
                    var cach = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform, layer1.size);
                    SetRectTransformPosition(rectTransform, layer1.position);
                }
            }
            else
            {
                if (posSizeLayerIndex != -1)
                {
                    Layer layer1 = layer.layers[posSizeLayerIndex];
                    if (layer1.size != null && layer1.position != null)
                    {
                        //check缩放 ?
                        // var sizeData = rectTransform.sizeDelta;
                        // if (sizeData != Vector2.zero)
                        // {
                        //     Vector3 scale = Vector3.one;
                        //     scale.x = layer1.size.width / sizeData.x;
                        //     scale.y = layer1.size.height / sizeData.y;
                        //     rectTransform.localScale = scale;
                        //     //
                        // }
                        var cach = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                        SetRectTransformSize(rectTransform, layer1.size);
                        SetRectTransformPosition(rectTransform, layer1.position);
                    }
                }
            }
        }

    }
}