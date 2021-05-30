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

        protected override void DrawTargetLayer(Layer layer, Button target, GameObject parent,int posSizeLayerIndex)
        {
            var normalImage = (Image)target.targetGraphic;// target.GetComponent<UnityEngine.UI.Image>();
            var text = target.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    
            int normalIdx = DrawBackgroundImage(layer,normalImage,target.gameObject); //背景图覆盖size
            base.DrawSpriteState(layer,target,parent,posSizeLayerIndex,true);

            //第一个文本
            foreach (var l1 in layer.layers)
            {
                if (ComponentType.Text == l1.type && !l1.TagContains(PSDImportUtility.NewTag) && text != null)
                {
                    if (normalIdx == -1 && posSizeLayerIndex==-1) //没有找到 size
                    {
                        RectTransform rectTransform = target.GetComponent<RectTransform>();
                        PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                        rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                        rectTransform.localPosition = GetLocalPosition(l1.position, rectTransform);
                        // rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position, rectTransform); 
                    }

                    ctrl.DrawLayer(l1, text.gameObject, target.gameObject);
                    text = null;
                    break;
                }
            }

            if(text!=null )
            {
                UnityEngine.GameObject.DestroyImmediate(text.gameObject);
            }


            ctrl.DrawLayers(layer.layers, null, target.gameObject);


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