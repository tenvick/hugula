using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class CollectionChangedBinder : UIBehaviourBinder {

        private INotifyCollectionChanged notify;
        #region  集合数据变更

        protected virtual void OnCollectionAdd (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected virtual void OnCollectionRemove (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected virtual void OnCollectionRepalce (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected virtual void OnCollectionMove (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected virtual void OnCollectionReSet (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs args) {
            if (args.Action == NotifyCollectionChangedAction.Add) {
                OnCollectionAdd (sender, args);
            } else if (args.Action == NotifyCollectionChangedAction.Remove) {
                OnCollectionRemove (sender, args);
            } else if (args.Action == NotifyCollectionChangedAction.Replace) {
                OnCollectionRepalce (sender, args);
            } else if (args.Action == NotifyCollectionChangedAction.Move) {
                OnCollectionMove (sender, args);
            } else if (args.Action == NotifyCollectionChangedAction.Reset) {
                OnCollectionReSet (sender, args);
            }
        }
        #endregion

        protected override void OnBindingContextChanging () {
            base.OnBindingContextChanging ();
            if (context is INotifyCollectionChanged) {
                notify = ((INotifyCollectionChanged) context);
                notify.CollectionChanged -= OnCollectionChanged;
            }
        }
        protected override void OnBindingContextChanged () {
            base.OnBindingContextChanged ();

            if (context is INotifyCollectionChanged) {
                notify = ((INotifyCollectionChanged) context);
                notify.CollectionChanged += OnCollectionChanged;
            }
        }

        protected override void OnDestroy () {
            if (notify != null) notify.CollectionChanged -= OnCollectionChanged;
            notify = null;
            base.OnDestroy ();
        }

    }
}