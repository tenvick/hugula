using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public sealed class BindableContainerComponentImport : BaseComponentImport<BindableContainer>
    {

        public BindableContainerComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected override void DrawTargetLayer(int index,Layer layer, BindableContainer target, GameObject parent,int posSizeLayerIndex)
        {
            var targetComp = target.GetComponent<BindableContainer>();
            layer.target = targetComp;
            if(posSizeLayerIndex==-1)
            {
                RectTransform rectTransform = target.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }

            ctrl.DrawLayers(layer.layers, null, target.gameObject);
            AutoAddChildren(target);
        }
     

        protected override void CheckAddBinder(Layer layer, BindableContainer target)
        {
            var binder = target;
            if (binder != null)
            {
                var parent = target.transform.parent;
                if (parent && PSDImportUtility.baseFilename != parent.name)
                    binder.SetBinding(layer.name, null, "context", BindingMode.OneWay, string.Empty, string.Empty);
            }
        }

        private void AutoAddChildren(BindableContainer target)
        {
            HugulaEditor.Databinding.BindableContainerEditor.AutoAddHierarchyChildren(target);
        }

    }
}