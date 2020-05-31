using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding
{

    public class BindableContainer : BindableObject
    {
        ///<summary>
        /// 绑定的对象
        ///<summary>
        [HideInInspector]
        public List<BindableObject> children;

        public void AddChild(BindableObject child)
        {
            // child.SetParent (this);
            children.Add(child);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindableObject child;
            for (int i = 0; i < children.Count; i++)
            // foreach (var child in children)
            {
                child = children[i];
                if (child)
                    child.SetInheritedContext(context, true);
                else
                {
                    Debug.LogErrorFormat("OnBindingContextChanged({0}) children index({1})  is null ", this, i);
                }
            }
        }

        #region mono

        protected override void OnDestroy()
        {
            children.Clear();
            base.OnDestroy();
        }
        #endregion
    }
}