using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public sealed class ButtonComponentImport : SelectableComponentImport<UnityEngine.UI.Button>
    {

        public ButtonComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, Button target, GameObject parent, int posSizeLayerIndex)
        {
            var normalImage = (Image)target.targetGraphic;// target.GetComponent<UnityEngine.UI.Image>();
            var text = target.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            int normalIdx = DrawBackgroundImage(layer, normalImage, target.gameObject); //背景图覆盖size
            base.DrawSpriteState(layer, target, parent, posSizeLayerIndex, true);

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (layer.scale != null)
            {
                SetRectTransformScale(rectTransform, layer);
            }

            int sizeTxtIdx = -1;
            //第一个文本
            for (int i = 0; i < layer.layers.Length; i++)
            {
                var l1 = layer.layers[i];
                if (ComponentType.Text == l1.type && !l1.TagContains(PSDImportUtility.NewTag) && text != null)
                {
                    if (normalIdx == -1 && posSizeLayerIndex == -1) //没有找到 size
                    {
                        sizeTxtIdx = i;
                        PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                        SetRectTransformSize(rectTransform, l1.size);
                        SetRectTransformPosition(rectTransform, l1.position);
                    }

                    ctrl.DrawLayer(i, l1, text.gameObject, target.gameObject);
                    text = null;
                    break;
                }
            }

            if (text != null)
            {
                UnityEngine.GameObject.DestroyImmediate(text.gameObject);
            }


            ctrl.DrawLayers(layer.layers, null, target.gameObject);

            //处理缩放
            if (layer.scale != null)
            {
                //size layer
                Layer sizeLayer = null;
                if (normalIdx >= 0) sizeLayer = layer.layers[normalIdx];
                else if (sizeTxtIdx >= 0) sizeLayer = layer.layers[sizeTxtIdx];

                if (sizeLayer != null)
                {
                    SetRectTransformScale(rectTransform, sizeLayer, layer);
                }
            }
        }

        protected override void CheckAddBinder(Layer layer, Button btn)
        {
            var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.ButtonBinder>(btn.gameObject);
            if (binder != null)
            {
                var binding = binder.GetBinding("onClickCommand");
                if (binding == null)
                {
                    binding = new Hugula.Databinding.Binding();
                    binding.propertyName = "onClickCommand";
                    binder.AddBinding(binding);
                }
                binding.path = "on_click_" + layer.name;
            }

        }

    }
}