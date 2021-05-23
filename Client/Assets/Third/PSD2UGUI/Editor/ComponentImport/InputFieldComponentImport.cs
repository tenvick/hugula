using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace PSDUIImporter
{
    public sealed class InputFieldComponentImport : BaseComponentImport<TMP_InputField>
    {

        public InputFieldComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(Layer layer, TMP_InputField target, GameObject parent,int posSizeLayerIndex)
        {
            var targetComp = target.GetComponent<TMPro.TMP_InputField>();
            
            var normalImage = target.GetComponent<UnityEngine.UI.Image>();
            var textComp = targetComp.textComponent;
            var holderTxt = (TextMeshProUGUI)targetComp.placeholder;

            int normalIdx= DrawBackgroundImage(layer,normalImage,target.gameObject);

            int textCompIdx =-1;

            //text文本
            for (var i = 0; i < layer.layers.Length; i++)
            {
                var l1 = layer.layers[i];
                if (ComponentType.Text == l1.type && !l1.TagContains(PSDImportUtility.NewTag)) 
                {
                    if(textComp !=null )
                    {
                        if (normalIdx == -1) //没有找到normal图片
                        {
                            RectTransform rectTransform = target.GetComponent<RectTransform>();
                            PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                            rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                            rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position, rectTransform); //layer.position - parentAnchoredPosition;
                        }

                        ctrl.DrawLayer(l1, textComp.gameObject, target.gameObject);
                        textCompIdx = i;
                        textComp = null;
                        continue;
                    }
                    else if(holderTxt !=null)
                    {
                        ctrl.DrawLayer(l1, holderTxt.gameObject, target.gameObject);
                        holderTxt = null;
                    }
                }
            }

            if(holderTxt!=null && textCompIdx !=-1 )
            {
                var l1 = layer.layers[textCompIdx];
                ctrl.DrawLayer(l1, holderTxt.gameObject, target.gameObject);
                holderTxt = null;
            }

            ctrl.DrawLayers(layer.layers, null, target.gameObject);

        }

        protected override void CheckAddBinder(Layer layer, TMP_InputField input)
        {
            var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.TMP_InputFieldBinder>(input.gameObject);
            if (binder != null)
            {
                var txtBinding = binder.GetBinding("text");
                if(txtBinding == null)
                {
                    txtBinding = new Hugula.Databinding.Binding();
                    binder.AddBinding(txtBinding);
                    txtBinding.propertyName = "text";
                }
                txtBinding.mode = Hugula.Databinding.BindingMode.TwoWay;
                txtBinding.path = "input_txt_"+layer.name;

                var binding = binder.GetBinding("submitEventCommand");
                if (binding == null)
                {
                    binding = new Hugula.Databinding.Binding();
                    binder.AddBinding(binding);
                    binding.propertyName = "submitEventCommand";
                }
                binding.path = "on_submit_" + layer.name;
            }

        }

    }
}