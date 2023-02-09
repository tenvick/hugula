using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PSDUINewImporter
{
    public abstract class SelectableComponentImport<T> : BaseComponentImport<T> where T : UnityEngine.UI.Selectable
    {
        public const string NormalTag = "Normal";
        public const string Highlighted = "Highlighted";
        public const string PressedTag = "Pressed";
        public const string SelectedTag = "Selected";
        public const string DisabledTag = "Disabled";
        public SelectableComponentImport(PSDComponentImportCtrl ctrl) : base(ctrl)
        {

        }

        protected virtual void DrawSpriteState(Layer layer, T target, GameObject parent, int posSizeLayerIndex, bool findByOrder = false)
        {
            var btnState = target.spriteState;

            var highlightedSprite = btnState.highlightedSprite;
            var pressedSprite = btnState.pressedSprite;
            var selectedSprite = btnState.selectedSprite;
            var disabledSprite = btnState.disabledSprite;

            // int normalIdx = -1;
            int highlightedIdx = -1;
            int pressedIdx = -1;
            int selectedIdx = -1;
            int disabledIdx = -1;

            //按照tag寻找
            for (var i = 0; i < layer.layers.Length; i++)
            {
                var l1 = layer.layers[i];
                if (ComponentType.Image == l1.type && !l1.TagContains(PSDImportUtility.NewTag))
                {
                    if (l1.TagContains(Highlighted)) //高亮
                    {
                        highlightedIdx = i;
                    }
                    else if (l1.TagContains(PressedTag))
                    {
                        pressedIdx = i;
                        // Debug.LogFormat("Button  pressedIdx name={0},i={1},normalImage={2} ", l1.name, i, normalImage.sprite);
                    }
                    else if (l1.TagContains(SelectedTag))
                    {
                        selectedIdx = i;
                    }
                    else if (l1.TagContains(DisabledTag))
                    {
                        disabledIdx = i;
                    }
                }
            }

            if (findByOrder)
            {
                //按照顺序设置状态
                UnityEngine.Sprite defaultSprite = null;//normalImage.sprite;

                //寻找 highlightedSprite pressedSprite selectedSprite disabledSprite图片
                for (var i = 0; i < layer.layers.Length; i++)
                {
                    var l1 = layer.layers[i];
                    if (!PSDImportUtility.NeedDraw(l1) || l1.TagContains(PSDImportUtility.NewTag) || !ctrl.CompareLayerType(l1.type, ComponentType.Image)) continue;

                    //Debug.LogFormat("Button name={0},i={1},target={2} ", l1.name, i, l1.target);
                    //按照属性设置btnState
                    if (highlightedIdx == -1 && (pressedIdx != i && selectedIdx != i && disabledIdx != i)) //如果有设置关键字
                        highlightedIdx = i;
                    else if (pressedIdx == -1 && (highlightedIdx != i && selectedIdx != i && disabledIdx != i))
                        pressedIdx = i;
                    else if (selectedIdx == -1 && (highlightedIdx != i && pressedIdx != i && disabledIdx != i))
                        selectedIdx = i;
                    else if (disabledIdx == -1 && (highlightedIdx != i && pressedIdx != -1 && selectedIdx != i))
                        disabledIdx = i;

                    if (highlightedIdx == i) //高亮按照顺序来渲染
                    {
                        btnState.highlightedSprite = LoadSpriteRes(l1, defaultSprite);
                        l1.target = btnState.highlightedSprite;
                    }
                    else if (pressedIdx == i)
                    {
                        btnState.pressedSprite = LoadSpriteRes(l1, defaultSprite);
                        l1.target = btnState.highlightedSprite;
                    }
                    else if (selectedIdx == i)
                    {
                        btnState.selectedSprite = LoadSpriteRes(l1, defaultSprite);
                        l1.target = btnState.highlightedSprite;
                    }
                    else if (disabledIdx == i)
                    {
                        disabledIdx = i;
                        btnState.disabledSprite = LoadSpriteRes(l1, defaultSprite);
                        l1.target = btnState.highlightedSprite;
                    }
                }
            }

            //
            if (highlightedIdx != -1 || pressedIdx != -1 || selectedIdx != -1 || disabledIdx != -1)
            {
                target.spriteState = btnState;
                target.transition = Selectable.Transition.SpriteSwap;
            }
        }
        // protected override void DrawTargetLayer(Layer layer, T target, GameObject parent,int posSizeLayerIndex)
        // {

        // }

    }
}