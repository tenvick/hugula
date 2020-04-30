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

            foreach (var child in children)
            {
                if (child)
                    child.SetInheritedContext(context, true);
                else
                {
                    Debug.LogWarningFormat("OnBindingContextChanged({0}) child({1}) is null ", this, child);
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