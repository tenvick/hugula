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
    public sealed class ToggleComponentImport : SelectableComponentImport<Toggle>
    {

        public ToggleComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, Toggle target, GameObject parent, int posSizeLayerIndex)
        {
            var normalImage = (Image)target.targetGraphic;
            int normalIdx = DrawBackgroundImage(layer, normalImage, target.gameObject, false); //背景图覆盖size
            normalImage.GetComponent<RectTransform>().sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
            //寻找check mark Image
            var checkImage = (Image)target.graphic;
            base.DrawTagImage(layer, checkImage, "", 0, checkImage.gameObject);

            base.DrawSpriteState(layer, target, target.gameObject, posSizeLayerIndex);

            ctrl.DrawLayers(layer.layers, null, target.gameObject);

        }

        protected override void CheckAddBinder(Layer layer, Toggle input)
        {
            var binder = PSDImportUtility.AddMissingComponent<Hugula.Databinding.Binder.ToggleBinder>(input.gameObject);
            if (binder != null)
            {
                var txtBinding = binder.GetBinding("isOn");
                if (txtBinding == null)
                {
                    txtBinding = new Hugula.Databinding.Binding();
                    binder.AddBinding(txtBinding);
                    txtBinding.propertyName = "isOn";
                    txtBinding.mode = Hugula.Databinding.BindingMode.TwoWay;
                }
                txtBinding.path = layer.name + "_is_on";

                var binding = binder.GetBinding("onValueChangedCommand");
                if (binding == null)
                {
                    binding = new Hugula.Databinding.Binding();
                    binder.AddBinding(binding);
                    binding.propertyName = "onValueChangedCommand";
                }
                binding.path = "on_value_changed_" + layer.name;
            }

        }

    }
}