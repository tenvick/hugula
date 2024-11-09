using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    [XLua.LuaCallCSharp]
    public abstract class CollectionChangedBinder<T> : UIBehaviourBinder<T>, ICollectionBinder where T : UnityEngine.Object
    {
        // [Tooltip("Whether context changed, force refresh binding. ")]
        [SerializeField] protected bool m_forceBinding = true;

        private INotifyCollectionChanged notify;
        #region  集合数据变更

        protected virtual void OnCollectionAdd(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

        }

        protected virtual void OnCollectionRemove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

        }

        protected virtual void OnCollectionRepalce(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

        }

        protected virtual void OnCollectionMove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

        }

        protected virtual void OnCollectionReSet(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

        }

        protected void OnCollectionChanged(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                OnCollectionAdd(sender, args);
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                OnCollectionRemove(sender, args);
            }
            else if (args.Action == NotifyCollectionChangedAction.Replace)
            {
                OnCollectionRepalce(sender, args);
            }
            else if (args.Action == NotifyCollectionChangedAction.Move)
            {
                OnCollectionMove(sender, args);
            }
            else if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                OnCollectionReSet(sender, args);
            }
        }
        #endregion

        public abstract IList<BindableObject> GetChildren();
        protected override void OnBindingContextChanging()
        {
            base.OnBindingContextChanging();
            if (context is INotifyCollectionChanged)
            {
                notify = ((INotifyCollectionChanged)context);
                notify.CollectionChanged?.Remove(OnCollectionChanged);
            }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (context is INotifyCollectionChanged)
            {
                notify = ((INotifyCollectionChanged)context);
                notify.CollectionChanged?.Add(OnCollectionChanged);
            }
        }

        protected override void OnDestroy()
        {
            if (notify != null) notify.CollectionChanged?.Remove(OnCollectionChanged);
            notify = null;
            base.OnDestroy();
        }

        public override void ClearBinding()
        {
            base.ClearBinding();
            if (notify != null) notify.CollectionChanged?.Remove(OnCollectionChanged);

        }

    }
}