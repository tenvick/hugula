using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding
{

    public interface IBinding
    {
        void UpdateTarget();
        void Apply(object context, bool invoke = true);
        void Unapply();
        void UpdateSource();
    }

    //编辑器使用
    public interface ICollectionBinder
    {
        IList<BindableObject> GetChildren();
    }

    public interface IClearBingding
    {
        /// <summary>
        /// 清理绑定列表
        /// </summary>
        void ClearBinding();

    }
}