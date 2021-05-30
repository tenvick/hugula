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

        protected override void DrawTargetLayer(Layer layer, RectTransform target, GameObject parent,int posSizeLayerIndex)
        {
             if(posSizeLayerIndex==-1)
            {
                RectTransform rectTransform = target.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
            }
            ctrl.DrawLayers(layer.layers, null, target.gameObject);
        }

        // /// <summary>
        // /// ps的对齐转换到ugui，暂时只做水平的对齐
        // /// </summary>
        // /// <param name="justification"></param>
        // /// <returns></returns>
        // public TextAlignmentOptions ParseAlignmentPS2UGUI(string justification)
        // {
        //     var defaut = TextAlignmentOptions.Center;
        //     if (string.IsNullOrEmpty(justification))
        //     {
        //         return defaut;
        //     }

        //     string[] temp = justification.Split('.');
        //     if (temp.Length != 2)
        //     {
        //         Debug.LogWarning("ps exported justification is error !");
        //         return defaut;
        //     }
        //     Justification justi = (Justification)System.Enum.Parse(typeof(Justification), temp[1]);
        //     int index = (int)justi;
        //     //转化成TextAlignment
        //     TextAlignmentOptions[] textAlignmentOptions =
        //     {
        //         TextAlignmentOptions.TopLeft, TextAlignmentOptions.Top | TextAlignmentOptions.Center,
        //         TextAlignmentOptions.TopRight, TextAlignmentOptions.Left | TextAlignmentOptions.Center,
        //         TextAlignmentOptions.Center, TextAlignmentOptions.Right | TextAlignmentOptions.Center,
        //         TextAlignmentOptions.BottomLeft
        //     };
        //     //defaut = (TextAnchor)System.Enum.ToObject(typeof(TextAnchor), index);
        //     defaut = textAlignmentOptions[index];
        //     return defaut;
        // }

        // //ps的对齐方式
        // public enum Justification
        // {
        //     CENTERJUSTIFIED = 0,
        //     LEFTJUSTIFIED = 1,
        //     RIGHTJUSTIFIED = 2,
        //     LEFT = 3,
        //     CENTER = 4,
        //     RIGHT = 5,
        //     FULLYJUSTIFIED = 6,
        // }
    }
}