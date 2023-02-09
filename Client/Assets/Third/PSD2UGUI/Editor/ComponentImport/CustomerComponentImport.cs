using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public sealed class CustomerComponentImport : BaseComponentImport<RectTransform>
    {

        public CustomerComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index,Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            RectTransform rectTransform = target;

            if (layer.layers == null  && layer.imageType == ComponentType.Image) //单张image模板
            {
                Layer layer1 = layer;
                if (layer1.size != null && layer1.position != null)
                {
                   // var cach = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform,layer1.size);
                    SetRectTransformPosition(rectTransform,layer1.position);
                }
            }
            else
            {
                if (posSizeLayerIndex != -1)
                {
                    Layer layer1 = layer.layers[posSizeLayerIndex];
                    if (layer1.size != null && layer1.position != null)
                    {
                        //var cach = PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                        SetRectTransformSize(rectTransform,layer1.size);
                        SetRectTransformPosition(rectTransform,layer1.position);
                    }
                }

                ctrl.DrawLayers(layer.layers, null, target.gameObject);
            }


        }



    }
}