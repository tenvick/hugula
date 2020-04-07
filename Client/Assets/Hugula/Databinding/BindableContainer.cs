using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Databinding {

    public class BindableContainer : BindableObject {
        ///<summary>
        /// 绑定的对象
        ///<summary>
        [HideInInspector]
        public List<BindableObject> children;

        public void AddChild (BindableObject child) {
            // child.SetParent (this);
            children.Add (child);
        }

        protected override void OnBindingContextChanged () {
            base.OnBindingContextChanged ();

            foreach (var child in children) {
               child.SetInheritedContext (context, true);
            }
        }

        #region mono

        protected override void OnDestroy () {
            children.Clear ();
          }
        #endregion
    }
}