using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PSDUIImporter
{
    public abstract class BaseComponentImport<T> : IComponentImport where T : UnityEngine.Component
    {
        protected PSDComponentImportCtrl ctrl;
        //有size图层
        // protected int sizeIdx = -1;
        public BaseComponentImport(PSDComponentImportCtrl ctrl)
        {
            this.ctrl = ctrl;
        }

        public void DrawLayer(Layer layer, GameObject target, GameObject parent)
        {
            T targetT = default(T);
            if (target != null)
            {
                targetT = target.GetComponent<T>();
            }
            if (targetT == null)
            {
                targetT = LoadAndInstant(layer, parent);
            }

            DrawTargetLayer(layer, targetT, parent,DrawSizeAndPos(layer,targetT.gameObject));
            CheckAddBinder(layer, targetT);
        }


        protected virtual int DrawSizeAndPos(Layer layer,GameObject target)
        {
            int bgIdx;
            if (TryGetSizePostion(layer, out var size, out var position, out bgIdx))
            {
                var l1 = layer.layers[bgIdx];
                var rectTransform = target.GetComponent<RectTransform>();
                PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position,rectTransform);
                return bgIdx;
            } 
            return -1;
        }

        protected virtual int DrawBackgroundImage(Layer layer,Image image, GameObject target)
        {
            int bgIdx;
            if (TryGetBackgroundImage(layer,out bgIdx))
            {
                var l1 = layer.layers[bgIdx];
                image.sprite = LoadSpriteRes(l1, null);
                l1.target = image;
                SetImageProperties(image, l1);
                var rectTransform = target.GetComponent<RectTransform>();
                // lsvRect = new Rect(l1.position.x, l1.position.y, l1.size.width, l1.size.height);
                PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position, rectTransform);
                if (image.sprite == null)
                    image.enabled = false;
                else
                    image.enabled = true;
                    
                return bgIdx;
            }
            return -1;
        }

        protected Sprite LoadSpriteRes(Layer layer, Sprite defaultSprite)
        {
            string assetPath = PSDImportUtility.baseDirectory + layer.name + PSDImporterConst.PNG_SUFFIX;
            Sprite sprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            if (sprite == null)
            {
                sprite = defaultSprite;
                Debug.LogWarningFormat("Load Sprite asset fail path = {0} .", assetPath);

            }
            return sprite;
        }

        /// <summary>
        /// 按照psd中图层倒序（从下往上）遍历
        /// </summary>
        protected int ForeachLayersDesc(Layer layer, System.Func<Layer, int, bool> func)
        {
            var layers = layer.layers;
            if (layers != null)
            {
                Layer item;
                for (int i = layers.Length - 1; i >= 0; i--)
                {
                    item = layers[i];
                    if (func(item, i))
                    {
                        return i;
                    }

                }
            }
            return -1;
        }

        /// <summary>
        /// 按照psd中图层顺序（从上往下）变量
        /// </summary>
        protected int ForeachLayers(Layer layer, System.Func<Layer, int, bool> func)
        {
            var layers = layer.layers;
            if (layers != null)
            {
                Layer item;
                for (int i = 0; i < layers.Length; i++)
                {
                    item = layers[i];
                    if (func(item, i))
                    {
                        return i;
                    }

                }
            }
            return -1;
        }

        /// <summary>
        /// 按照psd中图层倒序（从下往上）遍历
        /// 首先匹配tag
        /// 然后匹配位置
        /// </summary>
        protected bool TryGetBackgroundImage(Layer layer,out int index)
        {
            //寻找tag
            var tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (ctrl.CompareLayerType(li.type, ComponentType.Image) && 
                    (li.TagContains(ImageTag.BackGround) || li.TagContains(ImageTag.NormalTag) ) && PSDImportUtility.NeedDraw(li))
                {
                    return true;
                }
                return false;
            });

            if (tagIdx != -1)
            {
                index = tagIdx;
                return true;
            }

            //按照倒序寻找
            tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (ctrl.CompareLayerType(li.type, ComponentType.Image) 
                    && !li.TagContains(PSDImportUtility.NewTag) 
                    && PSDImportUtility.NeedDraw(li)
                   )
                {
                    return true;
                }
                return false;
            });

            if (tagIdx != -1)
            {
                index = tagIdx;
                return true;
            }
            index = -1;
            return false;

        }

        /// <summary>
        ///按照psd中图层顺序获取带有Size布局属性的层
        /// </summary>
        protected bool TryGetSizePostion(Layer layer, out Size size, out Position position, out int index)
        {
            var layers = layer.layers;
            if (layers != null)
            {
                Layer item;
                for (int i = 0; i < layers.Length; i++)
                {
                    item = layers[i];
                    if (item.TagContains(PSDImportUtility.SizeTag))
                    {
                        // Debug.LogFormat(" layer={0},l.name={1} size and pos",layer.name,l.name);
                        size = item.size;
                        position = item.position;
                        index = i;
                        return true;
                    }
                }
            }
            size = null;
            position = null;
            index = -1;
            return false;
        }

        /// <summary>
        /// 得到本地 AnchoredPosition
        /// </summary>
        protected Vector2 GetLocalAnchoredPosition(Position worldPos, RectTransform selfTrans)
        {
            var worldPosition = new Vector2(worldPos.x, worldPos.y);
            RectTransform parent = (RectTransform)selfTrans.parent;
            Vector2 anchoredPosition = worldPosition;
            Vector2 parentAnchoredPosition = parent.GetComponent<RectTransform>().anchoredPosition;

            while (parent != null)
            {
                parentAnchoredPosition = parent.GetComponent<RectTransform>().anchoredPosition;
                anchoredPosition = anchoredPosition - parentAnchoredPosition;
                //Debug.LogFormat("p={0},ppos={1},anpos={2} ,worldPos={3}",parent.name,parentAnchoredPosition,anchoredPosition,worldPos);
                parent = (RectTransform)parent.parent;
                if (parent != null && parent.parent == null) break;
            }

            return anchoredPosition;
        }

        protected void SetImageProperties(Image image, Layer layer)
        {
            if (layer.TagContains(ImageTag.SliceTag))
            {
                image.type = UnityEngine.UI.Image.Type.Sliced;
            }
        }

        protected abstract void DrawTargetLayer(Layer layer, T target, GameObject parent,int posSizeLayerIndex);

        protected virtual void  CheckAddBinder(Layer layer, T comp){
            
        }

        protected virtual T LoadAndInstant(Layer layer, GameObject parent)
        {
            var typeName = typeof(T).Name;
            if(typeof(RectTransform) == typeof(T) ) typeName = "Empty";
            var  asset = (T)PSDImportUtility.LoadAndInstant<T>(PSDImporterConst.GetAssetPath(typeName), layer.name, parent);
            return asset;
        }
    }
}