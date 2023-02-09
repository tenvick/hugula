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

        public void DrawLayer(int index, Layer layer, GameObject target, GameObject parent, bool autoPos, bool autoSize)
        {
            T targetT = default(T);
            if (target != null)
            {
                targetT = target.GetComponent<T>();
            }
            if (targetT == null)//find in parent 
            {
                targetT = FindInParent(index, layer, parent);
            }
            if (targetT == null)
            {
                targetT = LoadAndInstant(layer, parent,index);
            }

            layer.target = targetT;
            DrawTargetLayer(index, layer, targetT, parent, DrawSizeAndPos(layer, targetT.gameObject, autoPos));
            CheckAddBinder(layer, targetT);
        }


        protected virtual int DrawSizeAndPos(Layer layer, GameObject target, bool autoPosition = true, bool autoSize = true)
        {
            int bgIdx;
            if (TryGetSizePostion(layer, out var size, out var position, out bgIdx))//自定义组件需要控制布局
            {
                var l1 = layer.layers[bgIdx];
                if (!ctrl.CompareLayerType(layer.type, ComponentType.Customer))
                {
                    var rectTransform = target.GetComponent<RectTransform>();
                // PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                if (autoSize) SetRectTransformSize(rectTransform, l1.size);
                if (autoPosition) SetRectTransformPosition(rectTransform, l1.position);
				}
                // Debug.LogFormat("target={0}, autoPosition={1}, autoSize={2},l1.position={3}, rectTransform.localPosition={4} ", target, autoPosition, autoSize, l1.position, rectTransform.localPosition);
                l1.target = target;
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
        protected virtual int DrawBackgroundImage(Layer layer, Image image, GameObject overrideTarget = null, bool autoImagePosition = true, bool autoSize = true)
        {
            int bgIdx;
            if (TryGetBackgroundImage(layer, out bgIdx))
            {
                var l1 = layer.layers[bgIdx];
                image.sprite = LoadSpriteRes(l1, null);
                l1.target = image;
                SetImageProperties(image, l1);
                var rectTransform = image.GetComponent<RectTransform>();
                if (autoSize)
                    SetRectTransformSize(rectTransform, l1.size);
                if (autoImagePosition)
                {
                    //PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformPosition(rectTransform, l1.position);
                }

                if (overrideTarget != null)
                {
                    rectTransform = overrideTarget.GetComponent<RectTransform>();
                    //PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformSize(rectTransform, l1.size);
                    SetRectTransformPosition(rectTransform, l1.position);
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
        protected virtual int DrawTagText(int index, Layer layer, Component text, string tag, int repIndex, GameObject target, bool autoPos = true, bool autoSize = true)
        {
            int tagIdx;
            if (TryGetTagText(layer, tag, repIndex, out tagIdx))
            {
                var l1 = layer.layers[tagIdx];
                ctrl.DrawLayer(index, l1, text.gameObject, target.gameObject, autoPos, autoSize);
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
        protected virtual int DrawTagImage(Layer layer, Image image, string tag, int repIndex, GameObject target, bool autoSetPosition = true, bool autoSetSize = true)
        {
            int tagIdx;
            if (TryGetTagImage(layer, tag, repIndex, out tagIdx))
            {
                var l1 = layer.layers[tagIdx];
                image.sprite = LoadSpriteRes(l1, null);
                l1.target = image;
                SetImageProperties(image, l1);
                var rectTransform = target?.GetComponent<RectTransform>();
                if (autoSetPosition)
                {
                    //PSDImportUtility.SetAnchorMiddleCenter(rectTransform);
                    SetRectTransformPosition(rectTransform, l1.position);
                }
                if (autoSetSize)
                {
                    SetRectTransformSize(rectTransform, l1.size);
                }

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

        protected Texture LoadTextureRes(Layer layer)
        {
            var assetPath = PSDImportUtility.FindFileInDirectory(PSDImportUtility.baseDirectory, layer.name + PSDImporterConst.PNG_SUFFIX);
            Texture texture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
            if (texture == null)
            {
                // texture = defaultSprite;
                Debug.LogWarningFormat("Load Texture asset fail path = {0} .", assetPath);
            }
            return texture;
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
        protected bool TryGetTagImage(Layer layer, string tag, int replaceIndex, out int index)
        {
            //寻找tag
            var tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (!string.IsNullOrEmpty(tag) && ctrl.CompareLayerType(li.type, ComponentType.Image) &&
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
                if (ctrl.CompareLayerType(li.type, ComponentType.Image)
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
        /// 按照psd中图层倒序（从下往上）遍历
        /// 首先匹配tag
        /// 然后匹配位置
        /// </summary>
        protected bool TryGetBackgroundImage(Layer layer, out int index)
        {
            //寻找tag
            var tagIdx = ForeachLayersDesc(layer, (li, i) =>
            {
                if (ctrl.CompareLayerType(li.type, ComponentType.Image) &&
                    (li.TagContains(ImageTag.BackGround) || li.TagContains(ImageTag.NormalTag)) && PSDImportUtility.NeedDraw(li))
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
        protected virtual bool TryGetSizePostion(Layer layer, out Size size, out Position position, out int index)
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
        protected Vector2 GetLocalPosition(Position worldPos, RectTransform selfTrans)
        {
            Vector2 anchoredPosition = new Vector3(worldPos.x, worldPos.y);
            RectTransform parent = (RectTransform)selfTrans.parent;
            Vector2 parentAnchoredPosition;

            RectTransform pRectTrans;
            while (parent != null)
            {
                pRectTrans = parent.GetComponent<RectTransform>();
                parentAnchoredPosition = GetMiddleCenterPos(pRectTrans);
                anchoredPosition = anchoredPosition - parentAnchoredPosition;
                parent = (RectTransform)parent.parent;
                if (parent != null && parent.parent == null) break;
            }

            return anchoredPosition;
            // Vector3 anchoredPosition = new Vector3(worldPos.x, worldPos.y,0);
            // RectTransform parent = (RectTransform)selfTrans.parent;
            // Vector3 parentAnchoredPosition ;//= parent.GetComponent<RectTransform>().anchoredPosition;

            // RectTransform pRectTrans;
            // while (parent != null)
            // {
            //     pRectTrans = parent.GetComponent<RectTransform>();
            //     parentAnchoredPosition = parent.GetComponent<RectTransform>().localPosition;
            //     anchoredPosition = anchoredPosition - parentAnchoredPosition;
            //     Debug.LogFormat(" self:{4};pRectTrans={5};parent={0},p.pos={1},local.anpos={2} ,worldPos={3},",parent.name,parentAnchoredPosition,anchoredPosition,worldPos,selfTrans,pRectTrans);
            //     parent = (RectTransform)parent.parent;
            //     if (parent != null && parent.parent == null) break;
            // }

            // return anchoredPosition;
        }


        ///我们需要把坐标校正到中心点
        protected Vector2 GetMiddleCenterPos(RectTransform rectTransform)
        {
            var parent = (RectTransform)rectTransform.parent;
            //anchor 校正 到中心点 
            var anchorCenter = (rectTransform.anchorMin + rectTransform.anchorMax) * .5f; ;
            var rect = parent.rect;
            var sizeDelta = new Vector2(rect.width, rect.height);
            var anchorOffset = Vector2.Scale(sizeDelta, anchorCenter - Vector2.one * 0.5f);
            var pivotOffset = Vector2.Scale(rectTransform.sizeDelta, Vector2.one * 0.5f - rectTransform.pivot);
            return rectTransform.anchoredPosition + anchorOffset + pivotOffset;
        }

        protected void SetRectTransformSize(RectTransform rectTransform, Size size, float scale = 1)
        {
            rectTransform.sizeDelta = new Vector2(size.width * scale, size.height * scale);
        }

        protected void SetRectTransformPosition(RectTransform rectTransform, Position pos)
        {
            var middleCenter = GetLocalPosition(pos, rectTransform);

            //anchor 校正 到中心点 
            var parent = (RectTransform)rectTransform.parent;
            var anchorCenter = (rectTransform.anchorMin + rectTransform.anchorMax) * .5f; ;
            var rect = parent.rect;
            var sizeDelta = new Vector2(rect.width, rect.height);
            var anchorOffset = Vector2.Scale(sizeDelta, anchorCenter - Vector2.one * 0.5f);
            var pivotOffset = Vector2.Scale(rectTransform.sizeDelta, Vector2.one * 0.5f - rectTransform.pivot);
            rectTransform.anchoredPosition = middleCenter - anchorOffset - pivotOffset;
            //  Debug.LogFormat("self={0},pos={1},middleCenter={2},pivotOffset={3},anchorOffset={4},anchorCenter={5}",rectTransform,rectTransform.anchoredPosition,middleCenter,pivotOffset,anchorOffset,anchorCenter);
        }

        protected void SetImageProperties(Image image, Layer layer)
        {
            if (layer.TagContains(ImageTag.SliceTag))
            {
                image.type = UnityEngine.UI.Image.Type.Sliced;
            }
        }

        protected abstract void DrawTargetLayer(int index, Layer layer, T target, GameObject parent, int posSizeLayerIndex);

        protected virtual void CheckAddBinder(Layer layer, T comp)
        {

        }

        //更新时候查找
        protected virtual T FindInParent(int index, Layer layer, GameObject parent)
        {
            T t = default(T);
            var transform = parent.transform;
            Transform find = null;
            // find by index
            if (index < transform.childCount)
            {
                find = transform.GetChild(index);
                if (find.name != layer.name)
                    find = null;
            }

            // if (find == null) find = transform.Find(layer.name);

            if (find) t = find.GetComponent<T>();
            return t;
        }

        protected virtual T LoadAndInstant(Layer layer, GameObject parent, int index)
        {
            if (ctrl.CompareLayerType(layer.type, ComponentType.Customer)) //模板控件
            {
                var uiSourcePath = PSDImporterConst.SearAttachedPrefab(PSDImporterConst.PSDUI_CONSTOM_PATH, layer.templateName);
                if (string.IsNullOrEmpty(uiSourcePath))
                {
                    Debug.LogError($"there is no customer component ({layer.templateName},{layer.name}) in path {PSDImporterConst.PSDUI_CONSTOM_PATH} ");
                }
                var asset = PSDImportUtility.LoadAndInstantPrefab(uiSourcePath, layer.name, parent);
                asset.transform.SetSiblingIndex(index);
                return asset.GetComponent<T>();
            }
            else if (ctrl.CompareLayerType(layer.type, ComponentType.Text) && !string.IsNullOrEmpty(layer.templateName)) //模板字体
            {
                var fontStylePath = PSDImporterConst.SearAttachedPrefab(PSDImporterConst.PSDUI_CONSTOM_FONT_PATH, layer.templateName);
                if (string.IsNullOrEmpty(fontStylePath))
                {
                    Debug.LogError($"there is no font component ({layer.templateName}) in path {PSDImporterConst.PSDUI_CONSTOM_FONT_PATH} use (Default.prefab) replaced");
                    fontStylePath = "Default";
                }
                var asset = PSDImportUtility.LoadAndInstantAttachedPrefab(fontStylePath, layer.name,
                   parent);
                asset.transform.SetSiblingIndex(index);
                return asset.GetComponent<T>();
            }
            else
            {
                var typeName = typeof(T).Name;
                if (typeof(RectTransform) == typeof(T)) typeName = "Empty";
                var asset = (T)PSDImportUtility.LoadAndInstant<T>(PSDImporterConst.GetAssetPath(typeName), layer.name, parent);
                asset.transform.SetSiblingIndex(index);
                return asset;
            }
        }



    }
}