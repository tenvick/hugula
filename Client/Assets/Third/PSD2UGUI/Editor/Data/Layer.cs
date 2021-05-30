using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSDUINewImporter
{
    public class Layer
    {
        public string name;
        //导出类型 在Ps中以取文件夹名字@后的关键字
        public string type;
        public Layer[] layers;
        public string[] arguments;
        //ps图层中@后面的内容以"_"分割后放入tag中
        public string[] tag;
        public string special;

        public string layerKind;
        #region 位置信息与透明度
        public float opacity;
        public Size size;
        public Position position;
        #endregion

        #region 文字效果
        public string gradient = string.Empty;
        public string shadow = string.Empty;
        public string outline = string.Empty;
        public string outerGlow = string.Empty;
        #endregion
        internal object target;

        public bool TagContains(string key)
        {
            if (tag != null)
            {
                foreach (var t in tag)
                {
                    if (t.ToLower().Contains(key.ToLower()))
                        return true;
                }
            }
            return false;
        }
    }
}