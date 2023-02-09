using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using TMPro;

namespace PSDUINewImporter
{
    public sealed class SliderComponentImport : SelectableComponentImport<Slider>
    {
        public const string FillTag = "Fill";
        public const string HandleTag = "Handle";

        public SliderComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, Slider target, GameObject parent, int posSizeLayerIndex)
        {

            var handleImage = (Image)target.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();//0
            var fillImage = (Image)target.transform.Find("Fill Area/Fill").GetComponent<Image>();//1
            var bgImage = (Image)target.transform.Find("Background").GetComponent<Image>();//2 last

            int bgIdx = DrawBackgroundImage(layer, bgImage, target.gameObject, false);
            base.DrawSpriteState(layer, target, parent, posSizeLayerIndex);

            base.DrawTagImage(layer, handleImage, FillTag, 0, handleImage.gameObject, false);

            base.DrawTagImage(layer, fillImage, HandleTag, 0, fillImage.transform.parent.gameObject);

            ctrl.DrawLayers(layer.layers, null, target.gameObject);

        }

        protected override void CheckAddBinder(Layer layer, Slider input)
        {
            var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.SliderBinder>(input.gameObject);
            if (binder != null)
            {
                var binding = binder.GetBinding("value");
                if (binding == null)
                {
                    binding = new Hugula.Databinding.Binding();
                    binder.AddBinding(binding);
                    binding.propertyName = "value";
                }
                binding.mode = Hugula.Databinding.BindingMode.TwoWay;
                binding.path = "slider_val_" + layer.name;

                binding = binder.GetBinding("onValueChangedExecute");
                if (binding == null)
                {
                    binding = new Hugula.Databinding.Binding();
                    binder.AddBinding(binding);
                    binding.propertyName = "onValueChangedExecute";
                }
                binding.path = "on_val_changed_" + layer.name;
            }

        }

    }
}