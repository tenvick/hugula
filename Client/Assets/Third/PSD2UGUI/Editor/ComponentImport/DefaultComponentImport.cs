using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace PSDUINewImporter
{
    public sealed class DefaultComponentImport : BaseComponentImport<RectTransform>
    {

        public DefaultComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index, Layer layer, RectTransform target, GameObject parent, int posSizeLayerIndex)
        {
            target.name = string.IsNullOrEmpty(layer.name)?layer.layerName:layer.name;

            if (posSizeLayerIndex == -1)
            {
                SetSizeAndPosByBackgroundImage(target.GetComponent<RectTransform>(),layer);
            }
            ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }
    }
}