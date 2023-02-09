using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public sealed class RawImageComponentImport : BaseComponentImport<RawImage>
    {

        public RawImageComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, RawImage target, GameObject parent, int posSizeLayerIndex)
        {
            var targetComp = target.GetComponent<RawImage>();
            // layer.target = targetComp;
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            PSDImportUtility.SetAnchorMiddleCenter(rectTransform);

            targetComp.texture = LoadTextureRes(layer);
            targetComp.raycastTarget = false;
            targetComp.maskable = false;

            SetRectTransformSize(rectTransform, layer.size);
            SetRectTransformPosition(rectTransform, layer.position);
            // ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }


        protected override void CheckAddBinder(Layer layer, RawImage image)
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