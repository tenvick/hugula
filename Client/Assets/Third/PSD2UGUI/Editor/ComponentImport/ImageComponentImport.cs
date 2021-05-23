using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUIImporter
{
    public sealed class ImageComponentImport : BaseComponentImport<Image>
    {

        public ImageComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(Layer layer, Image target, GameObject parent,int posSizeLayerIndex)
        {
            var targetComp = target.GetComponent<Image>();
            layer.target = targetComp;
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            PSDImportUtility.SetAnchorMiddleCenter(rectTransform);

            targetComp.sprite = LoadSpriteRes(layer, null);
            targetComp.raycastTarget = false;
            targetComp.maskable = false;
            SetImageProperties(targetComp, layer);

            rectTransform.sizeDelta = new Vector2(layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = GetLocalAnchoredPosition(layer.position, rectTransform); //layer.position - parentAnchoredPosition;

            // ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }


        protected override  void CheckAddBinder(Layer layer, Image image)
        {
            if (layer.TagContains(PSDImportUtility.DynamicTag))
            {
                var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.ImageBinder>(image.gameObject);
                if (binder != null)
                {
                    var binding = binder.GetBinding("spriteName");
                    if (binding == null)
                    {
                        binding = new Hugula.Databinding.Binding();
                        binding.propertyName = "spriteName";
                        binder.AddBinding(binding);
                    }
                    binding.path = layer.name;
                }
            }

        }

    }
}