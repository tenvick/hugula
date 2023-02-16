using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Xml;
using System.Xml.Serialization;

namespace PSDUINewImporter
{
    public class Layer
    {
        //资源名
        [XmlAttribute]
        public string name;
        //ps图层的名字
        [XmlAttribute]
        public string layerName;
        //默认可见性
        [XmlAttribute]
        public bool visible = true;

        //导出类型 标记图片文字组件等
        public string type;
        public Layer[] layers;
        //记录的额外参数
        public string[] arguments;
        //ps图层中@后面的内容以"_"分割后放入tag中 tag标记用于导入判断
        public string[] tag;
        //自定义模板名
        public string templateName;
        //小类型
        public string miniType;

        [XmlIgnore]
        private string m_ImportType;
        //导入时候的Type用于查询导入的Import组件
        public string importType
        {
            get
            {
                if (string.IsNullOrEmpty(m_ImportType))
                    return type;
                else
                    return m_ImportType;

            }
            set
            {
                m_ImportType = value;
            }
        }



        #region 位置信息与透明度
        [System.ComponentModel.DefaultValueAttribute(100)]
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
        [XmlIgnore]
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