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
            // if (targetT == null)//find in parent 
            // {
            //     targetT = FindInParent(index, layer, parent);
            // }
            if (targetT == null)
            {
                targetT = LoadAndInstant(layer, parent, index);
            }

            if (targetT == null) //创建模板
            {
                var miniType = string.IsNullOrEmpty(layer.miniType) ? "Default" : layer.miniType;
                // if (layer.templateName.StartsWith("btn_"))
                // {
                //     miniType = "Button";
                // }
                layer.importType = miniType;
                ctrl.GetLayerImport(layer.importType).DrawLayer(index, layer, target, parent, autoPos, autoSize);
                //保存
                SavePrefabToCustomer(layer);
            }
            else
            {
                layer.target = targetT;
                DrawTargetLayer(index, layer, targetT, parent, DrawSizeAndPos(layer, targetT.gameObject, autoPos,autoSize));
                CheckAddBinder(layer, targetT);
            }

        }

        protected override void DrawTargetLayer(int index, Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            RectTransform rectTransform = target;
            bool isImg, isTxt = false;
            if (layer.layers == null && ((isImg = layer.miniType == ComponentType.Image) || (isTxt = layer.miniType == ComponentType.Text))) //单张image或者text模板
            {
                Layer layer1 = layer;
                if (layer1.size != null )
                {
                    var cach = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform, layer1.size);
                    SetRectTransformPosition(rectTransform, layer1.position);
                    PSDImportUtility.SetAnchorFromCache(rectTransform, cach);
                }

                if (isTxt)
                    UpdateTextPrefab(target.GetComponent<TMPro.TextMeshProUGUI>(), layer);
            }
            else
            {
                if (posSizeLayerIndex == -1)
                    posSizeLayerIndex = GetBackgroundImageLayer(layer);

                if (posSizeLayerIndex != -1)
                {
                    Layer layer1 = layer.layers[posSizeLayerIndex];
                    if (layer1.size != null && layer.scale!=null)
                    {
                        SetRectTransformScale(rectTransform, layer1,layer);
                    }
                }
            }


            // ctrl.DrawLayers(layer.layers, null, target.gameObject); //更新组件？
        }

        protected void UpdateTextPrefab(TMPro.TextMeshProUGUI text, Layer layer)
        {
            TextComponentImport.SetText(text, layer);
            TextComponentImport.SetColor(text, layer);
        }

    }
}