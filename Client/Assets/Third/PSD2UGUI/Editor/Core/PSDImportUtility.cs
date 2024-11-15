﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace PSDUINewImporter
{
    public static class PSDImportUtility
    {
        public static string baseFilename;
        public static string baseDirectory;
        public static Canvas canvas;
        public static GameObject eventSys;
        // public static readonly Dictionary<Transform, Transform> ParentDic = new Dictionary<Transform, Transform>();
        //需要新建gameobject
        public const string NewTag = "New";
        //布局组件
        public const string SizeTag = "Size";
        //不需要创建
        public const string HideTag = "Hide";
        //需要绑定控件
        public const string DynamicTag = "Dynamic";

        public static bool NeedDraw(Layer layer)
        {
            return (!layer.TagContains(HideTag) || layer.TagContains(DynamicTag)) && layer.target == null ;
        }
  

        public static bool ChildrenLayersTagContains(Layer layer, string tag, out int index)
        {
            Layer lItem;
            for (var i = 0; i < layer.layers.Length; i++)
            {
                lItem = layer.layers[i];
                if (lItem.TagContains(tag))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        public static PSDUI DeserializeXml(string filePath)
        {
            PSDUI instance = null;
            if(File.Exists(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PSDUI));
                serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                serializer.UnknownAttribute += new  XmlAttributeEventHandler(serializer_UnknownAttribute);
                using (FileStream fs = new FileStream(filePath, System.IO.FileMode.Open))
                {
                    instance = (PSDUI)serializer.Deserialize(fs);
                }
            }
            return instance;
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Debug.LogError($"Unknown Node:{e.Name} LineNumber:{e.LineNumber} LinePosition:{e.LinePosition} ObjectBeingDeserialized:{e.ObjectBeingDeserialized} NodeType:{e.NodeType} {e.Text} ");
        }
        private static void  serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Debug.Log("Unknown attribute " +  attr.Name + "='" + attr.Value + "'");
        }

        /// <summary>
        /// 加载并实例化prefab
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">assets全路径，带后缀</param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T LoadAndInstant<T>(string assetPath, string name, GameObject parent) where T : UnityEngine.Object
        {
            GameObject temp = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            //Debug.LogFormat(" LoadAndInstant assetPath={0}",assetPath);
            GameObject item = GameObject.Instantiate(temp, Vector3.zero, Quaternion.identity, parent?.transform) as GameObject;
            if (item == null)
            {
                Debug.LogError("LoadAndInstant asset failed : " + assetPath);
                return null;
            }
            item.name = name;
            item.transform.localScale = Vector3.one;
            return item.GetComponent<T>();
        }

        /// <summary>
        /// 引用一个moban prefab
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">assets全路径，带后缀</param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject LoadAndInstantPrefab(string assetPath, string name, GameObject parent)
        {
            GameObject temp = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            // Debug.LogFormat(" LoadAndInstantPrefab assetPath={0}", assetPath);
            GameObject item = (GameObject)PrefabUtility.InstantiatePrefab(temp,parent.transform);//GameObject.Instantiate(temp, Vector3.zero, Quaternion.identity, parent?.transform) as GameObject;
            if (item == null)
            {
                Debug.LogError("LoadAndInstantPrefab asset failed : " + assetPath);
                return null;
            }
            if(!string.IsNullOrEmpty(name))
                item.name = name;
            item.transform.SetParent(parent?.transform);
            item.transform.localScale = Vector3.one;
            return item;
        }

         /// <summary>
        /// 复制prefab
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">assets全路径，带后缀</param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject LoadAndInstantAttachedPrefab(string assetPath, string name, GameObject parent)
        {
            GameObject temp = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            // Debug.LogFormat(" LoadAndInstantAttachedPrefab assetPath={0}", assetPath);
            GameObject item = (GameObject)PrefabUtility.InstantiateAttachedAsset(temp);//GameObject.Instantiate(temp, Vector3.zero, Quaternion.identity, parent?.transform) as GameObject;
            if (item == null)
            {
                Debug.LogError("LoadAndInstantAttachedPrefab asset failed : " + assetPath);
                return null;
            }
            if (!string.IsNullOrEmpty(name))
                item.name = name;
            item.transform.SetParent(parent?.transform);
            item.transform.localScale = Vector3.one;
            return item;
        }

        public static CacheAnchorInfo SetAnchorMiddleCenter(this RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                Debug.LogWarning("rectTransform is null...");
                return new CacheAnchorInfo();
            }
            CacheAnchorInfo cacheAnchorInfo = new CacheAnchorInfo();
            cacheAnchorInfo.offsetMin = rectTransform.offsetMin;
            cacheAnchorInfo.offsetMax = rectTransform.offsetMax;
            cacheAnchorInfo.anchorMin = rectTransform.anchorMin;
            cacheAnchorInfo.anchorMax = rectTransform.anchorMax;
            cacheAnchorInfo.pivot = rectTransform.pivot;

            if(rectTransform.pivot.x!=.5f && rectTransform.pivot.y!=.5f)
            {
                Debug.LogWarning($" {rectTransform} pivot不是中心点，强制设置为中心点！ ");
                PSDRectLayout.SetPivotSmart(rectTransform, 0.5f, 0, true, true);
                PSDRectLayout.SetPivotSmart(rectTransform, 0.5f, 1, true, true);
            }

            return cacheAnchorInfo;
        }

        public static void SetAnchorFromCache(this RectTransform rectTransform, CacheAnchorInfo cacheAnchorInfo)
        {
            var pivot = cacheAnchorInfo.pivot;
            PSDRectLayout.SetPivotSmart(rectTransform, pivot.x, 0, true, true);
            PSDRectLayout.SetPivotSmart(rectTransform, pivot.y, 1, true, true);
        }
          
        public static RectTransform GetRectTransform(this GameObject source)
        {
            return source.GetComponent<RectTransform>();
        }

        public static T AddMissingComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
                comp = go.AddComponent<T>();
            return comp;
        }

        public static void DestroyComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return;

            T comp = go.GetComponent<T>();
            if (comp != null)
                UnityEngine.Object.Destroy(comp);
        }
    }


    public struct CacheAnchorInfo
    {
        public Vector2 offsetMin;
        public Vector2 offsetMax;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
    }
}