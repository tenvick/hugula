using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class LoopVerticalScrollRectBinder : CollectionChangedBinder {
        public const string OnItemInstantiatedProperty = "onItemInstantiated";
        public const string OnItemRenderProperty = "onItemRender";
        LoopVerticalScrollRect m_target;
        LoopVerticalScrollRect m_LoopVerticalScrollRect {
            get {
                if (m_target == null)
                    m_target = GetTarget<LoopVerticalScrollRect> ();
                return m_target;
            }
            set {
                m_target = null;
            }
        }
        IList items;

        #region  重写属性

        public bool scrollToBottom {
            get { return m_LoopVerticalScrollRect.scrollToBottom; }
            set {
                m_LoopVerticalScrollRect.scrollToBottom = value;
                OnPropertyChanged ();
            }
        }

        public int pageSize {
            get { return m_LoopVerticalScrollRect.pageSize; }
        }

        public int dragOffsetShow {
            get { return m_LoopVerticalScrollRect.dragOffsetShow; }
            set {
                m_LoopVerticalScrollRect.dragOffsetShow = value;
            }
        }

        public float padding {
            get { return m_LoopVerticalScrollRect.padding; }
            set {
                m_LoopVerticalScrollRect.padding = value;
                OnPropertyChanged ();
            }
        }

        public int renderPerFrames {
            get { return m_LoopVerticalScrollRect.renderPerFrames; }
            set {
                m_LoopVerticalScrollRect.renderPerFrames = value;
                OnPropertyChanged ();
            }
        }

        public int selectedIndex {
            get { return m_LoopVerticalScrollRect.selectedIndex; }
        }

        public Action<object, object, int> onInstantiated {
            get { return m_LoopVerticalScrollRect.onInstantiated; }
            set {
                m_LoopVerticalScrollRect.onInstantiated = value;
                OnPropertyChanged ();
            }
        }

        public Func<object, int, int> onGetItemTemplateType {
            get { return m_LoopVerticalScrollRect.onGetItemTemplateType; }
            set {
                m_LoopVerticalScrollRect.onGetItemTemplateType = value;
                OnPropertyChanged ();
            }
        }

        public Action<object, Component, int> onItemRender {
            get { return m_LoopVerticalScrollRect.onItemRender; }
            set {
                m_LoopVerticalScrollRect.onItemRender = value;
                OnPropertyChanged ();
            }
        }

        public Action<object, object, int, int> onSelected {
            get { return m_LoopVerticalScrollRect.onSelected; }
            set {
                m_LoopVerticalScrollRect.onSelected = value;
                OnPropertyChanged ();
            }
        }

        public ICommand itemCommand {
            get { return m_LoopVerticalScrollRect.itemCommand; }
            set {
                m_LoopVerticalScrollRect.itemCommand = value;
                OnPropertyChanged ();
            }
        }

        public object itemParameter {
            get { return m_LoopVerticalScrollRect.itemParameter; }
            set {
                m_LoopVerticalScrollRect.itemParameter = value;
                OnPropertyChanged ();
            }
        }

        public int dataLength {
            get { return m_LoopVerticalScrollRect.dataLength; }
            set {
                m_LoopVerticalScrollRect.dataLength = value;
                OnPropertyChanged ();
            }
        }

        public ICommand droppedCommand {
            get { return m_LoopVerticalScrollRect.droppedCommand; }
            set {
                m_LoopVerticalScrollRect.droppedCommand = value;
                OnPropertyChanged ();
            }
        }

        public Action<Vector2> onDropped {
            get { return m_LoopVerticalScrollRect.onDropped; }
            set {
                m_LoopVerticalScrollRect.onDropped = value;
                OnPropertyChanged ();
            }
        }

        #endregion

        #region  集合数据变更

        protected override void OnCollectionAdd (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null) {
                count = args.NewItems.Count;
            }
            m_LoopVerticalScrollRect.InsertRange (index, count);
            // Debug.LogFormat ("OnCollectionAdd(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopVerticalScrollRect.dataLength,items.Count);
        }

        protected override void OnCollectionRemove (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.OldItems != null)
                count = args.OldItems.Count;

            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopVerticalScrollRect.dataLength,items.Count);
            if (index >= 0)
                m_LoopVerticalScrollRect.RemoveAt (index, count);
            else
                m_LoopVerticalScrollRect.RemoveAt (0, count);

        }

        protected override void OnCollectionRepalce (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
                count = args.NewItems.Count;

            m_LoopVerticalScrollRect.UpdateBegin (index);
            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopVerticalScrollRect.dataLength,items.Count);

        }

        protected override void OnCollectionMove (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            var oldIndex = args.OldStartingIndex;
            m_LoopVerticalScrollRect.UpdateBegin (index);
        }

        protected override void OnCollectionReSet (object sender, NotifyCollectionChangedEventArgs args) {
            m_LoopVerticalScrollRect.dataLength = 0;
            m_LoopVerticalScrollRect.Refresh ();
        }

        #endregion

        #region  渲染相关
        protected void OnItemInstantiated (object obj1, object obj2, int index) {

        }

        protected void OnItemRender (object obj1, object obj2, int index) {
            BindableContainer item = (BindableContainer) obj2;
            if (item != null) {
                item.forceContextChanged = true;
                item.context = items[index];
            }
        }

        #endregion
        protected override void OnBindingContextChanged () {
            // if (GetBinding (OnItemInstantiatedProperty) == null)
            //     m_LoopVerticalScrollRect.onInstantiated = OnItemInstantiated;

            if (GetBinding (OnItemRenderProperty) == null)
                m_LoopVerticalScrollRect.onItemRender = OnItemRender;

            items = (IList) context;

            base.OnBindingContextChanged ();

            m_LoopVerticalScrollRect.dataLength = items.Count;
            m_LoopVerticalScrollRect.Refresh ();

        }

        protected override void OnDestroy () {
            items = null;
            m_LoopVerticalScrollRect = null;
            base.OnDestroy ();
        }
    }
}