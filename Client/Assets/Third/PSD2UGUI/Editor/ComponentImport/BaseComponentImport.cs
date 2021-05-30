using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace PSDUINewImporter
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
                // rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position,rectTransform);
                rectTransform.localPosition = GetLocalPosition(l1.position, rectTransform);
                return bgIdx;
            } 
            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <param name="overrideTarget">重新覆盖目标对象大小和位置</param>
        /// <param name="autoImagePosition">是否设置图片位置</param>
        /// <returns></returns>
        protected virtual int DrawBackgroundImage(Layer layer,Image image, GameObject overrideTarget = null,bool autoImagePosition = true)
        {
            int bgIdx;
            if (TryGetBackgroundImage(layer,out bgIdx))
            {
                var l1 = layer.layers[bgIdx];
                image.sprite = LoadSpriteRes(l1, null);
                l1.target = image;
                SetImageProperties(image, l1);
                var rectTransform = image.GetComponent<RectTransform>();
                if(autoImagePosition)
                {
                    PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    // rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position, rectTransform);
                    rectTransform.localPosition = GetLocalPosition(l1.position, rectTransform);

                }
                rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);

                if(overrideTarget!=null)
                {
                   rectTransform =  overrideTarget.GetComponent<RectTransform>();
                    PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                    // rectTransform.anchoredPosition = GetLocalAnchoredPosition(l1.position, rectTransform);
                    rectTransform.localPosition = GetLocalPosition(l1.position, rectTransform);

                }

                if (image.sprite == null)
                    image.enabled = false;
                else
                    image.enabled = true;
                    
                return bgIdx;
            }
            return -1;
        }

        /// <summary>
        /// 绘制带特定TAG的图片，如果TAG不存在以repIndex替换 顺序。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="image"></param>
        /// <param name="tag"></param>
        /// <param name="repIndex"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual int DrawTagText(Layer layer, Component text, string tag, int repIndex, GameObject target)
        {
            int tagIdx;
            if (TryGetTagText(layer, tag, repIndex, out tagIdx))
            {
                var l1 = layer.layers[tagIdx];
                ctrl.DrawLayer(l1, text.gameObject, target.gameObject);
                return tagIdx;
            }
            return -1;
        }


        /// <summary>
        /// 绘制带特定TAG的图片，如果TAG不存在以repIndex替换 顺序。
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="image"></param>
        /// <param name="tag"></param>
        /// <param name="repIndex"></param>
        /// <param name="target"></param>
        /// <param name="autoSetPosition">设置image位置</param>
        /// <param name="autoSetSize">设置imagesizeDelta</param>
        /// <returns></returns>
        protected virtual int DrawTagImage(Layer layer,Image image, string tag, int repIndex ,GameObject target, bool autoSetPosition = true, bool autoSetSize = true)
        {
            int tagIdx;
            if (TryGetTagImage(layer, tag, repIndex,out tagIdx))
            {
                var l1 = layer.layers[tagIdx];
                image.sprite = LoadSpriteRes(l1, null);
                l1.target = image;
                SetImageProperties(image, l1);
                var rectTransform = target?.GetComponent<RectTransform>();
                if(autoSetPosition)
                {
                    PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    rectTransform.localPosition = GetLocalPosition(l1.position, rectTransform);
                }
                if(autoSetSize)rectTransform.sizeDelta = new Vector2(l1.size.width, l1.size.height);
                if (image.sprite == null)
                    image.enabled = false;
                else
                    image.enabled = true;
                    
                return tagIdx;
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="defaultSprite"></param>
        /// <returns></returns>
        protected Sprite LoadSpriteRes(Layer layer, Sprite defaultSprite)
        {
            var assetPath = PSDImportUtility.FindFileInDirectory(PSDImportUtility.baseDirectory, layer.name + PSDImporterConst.PNG_SUFFIX);
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
        /// 
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
        /// 首先匹配tag
        /// 然后寻找可以用的replaceIndex
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tag"></param>
        /// <param name="replaceIndex"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool TryGetTagText(Layer layer, string tag, int replaceIndex, out int index)
        {
            //寻找tag
            var tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (!string.IsNullOrEmpty(tag) && ctrl.CompareLayerType(li.type, ComponentType.Text) &&
                    li.TagContains(tag) && PSDImportUtility.NeedDraw(li))
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

            int imgIdx = -1;
            tagIdx = ForeachLayers(layer, (li, i) =>
            {
                if (ctrl.CompareLayerType(li.type, ComponentType.Text)
                    && !li.TagContains(PSDImportUtility.NewTag)
                    && PSDImportUtility.NeedDraw(li)
                   )
                {
                    imgIdx++;
                    if (imgIdx == replaceIndex)
                    {
                        return true;
                    }
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
        /// 首先匹配tag
        /// 然后寻找可以用的replaceIndex
        /// </summary>
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="tag"></param>
        /// <param name="replaceIndex"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool TryGetTagImage(Layer layer,string tag,int replaceIndex,out int index)
        {
            //寻找tag
            var tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (!string.IsNullOrEmpty(tag)  && ctrl.CompareLayerType(li.type, ComponentType.Image) && 
                    li.TagContains(tag) && PSDImportUtility.NeedDraw(li))
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
            
            int imgIdx=-1;
            tagIdx = ForeachLayers(layer, (li, i) =>
            {
                if (ctrl.CompareLayerType(li.type, ComponentType.Image) 
                    && !li.TagContains(PSDImportUtility.NewTag) 
                    && PSDImportUtility.NeedDraw(li)
                   )
                {
                    imgIdx ++;
                    if(imgIdx == replaceIndex)
                    {
                        return true;
                    }
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
        protected Vector3 GetLocalPosition(Position worldPos, RectTransform selfTrans)
        {
            Vector3 anchoredPosition = new Vector3(worldPos.x, worldPos.y,0);
            RectTransform parent = (RectTransform)selfTrans.parent;
            Vector3 parentAnchoredPosition ;//= parent.GetComponent<RectTransform>().anchoredPosition;
                
            RectTransform pRectTrans;
            while (parent != null)
            {
                pRectTrans = parent.GetComponent<RectTransform>();
                parentAnchoredPosition = parent.GetComponent<RectTransform>().localPosition;
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