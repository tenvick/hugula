using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding
{

    public interface IBinding
    {
        void UpdateTarget();
        void Apply(object content);
        void Unapply();
        void UpdateSource();
    }

    //编辑器使用
    public interface ICollectionBinder
    {
        IList<BindableObject> GetChildren();
    }

    //自动绑定忽略当前的所有同辈组件
    public interface IIgnorePeerBinder
    {

    }

    public interface IClearBingding
    {
        /// <summary>
        /// 清理绑定列表
        /// </summary>
        void ClearBinding();

    }
}