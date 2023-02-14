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
            if (posSizeLayerIndex == -1)
            {
                RectTransform rectTransform = target.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }
            ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }
    }
}