using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Hugula.Databinding;
using Hugula.Mvvm;
using System;

namespace Hugula.UI
{
    ///
    public class ClickTipsOpen : BindableObject,IPointerUpHandler
    {
        public object arg{get;set;}
        
        [Tooltip("指向的trnasform")]
        [SerializeField] Transform tipsFollow;

        [Tooltip("vm_config 配置的模块名")]
        [SerializeField] string vmName;

        public void OnPointerUp(PointerEventData eventData)
        {
            ClickTipsConvert.m_CurrTransform = tipsFollow;
            VMStateHelper.instance.PushItem(vmName,arg); //弹出提示框
        }

        void OnDestory()
        {
            if(ClickTipsConvert.m_CurrTransform == tipsFollow)
                ClickTipsConvert.m_CurrTransform = null;
        }
    }

    public class ClickTipsConvert:IValueConverter
    {
        static internal Transform m_CurrTransform;
        public object Convert (object value, Type targetType)
        {
            return m_CurrTransform;
        }
		public object ConvertBack (object value, Type targetType)
        {
            return null;
        }
    }
}