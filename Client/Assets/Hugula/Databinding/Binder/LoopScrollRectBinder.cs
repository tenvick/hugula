using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class LoopScrollRectBinder : CollectionChangedBinder {
        public const string OnItemInstantiatedProperty = "onItemInstantiated";
        public const string OnItemRenderProperty = "onItemRender";
        LoopScrollRect m_LoopScrollRect;
        IList items;
        void Awake () {
            m_LoopScrollRect = GetTarget<LoopScrollRect> ();
        }

        #region  重写属性

        public int setScrollIndex {
            get { return m_LoopScrollRect.setScrollIndex; }
            set {
                m_LoopScrollRect.setScrollIndex = value;
                OnPropertyChanged ();
            }
        }

        public int columns {
            get { return m_LoopScrollRect.columns; }
        }

        public int pageSize {
            get { return m_LoopScrollRect.pageSize; }
        }

        public Vector2 itemSize {
            get { return m_LoopScrollRect.itemSize; }
        }

        public float padding {
            get { return m_LoopScrollRect.padding; }
            set {
                m_LoopScrollRect.padding = value;
                OnPropertyChanged ();
            }
        }

        public int renderPerFrames {
            get { return m_LoopScrollRect.renderPerFrames; }
            set {
                m_LoopScrollRect.renderPerFrames = value;
                OnPropertyChanged ();
            }
        }

        public int selectedIndex {
            get { return m_LoopScrollRect.selectedIndex; }
        }

        public Action<object, object, int> onInstantiated {
            get { return m_LoopScrollRect.onInstantiated; }
            set {
                m_LoopScrollRect.onInstantiated = value;
                OnPropertyChanged ();
            }
        }

        public Func<object, int, Component, int, RectTransform, Component> onGetItem {
            get { return m_LoopScrollRect.onGetItem; }
            set {
                m_LoopScrollRect.onGetItem = value;
                OnPropertyChanged ();
            }
        }

        public Action<object, object, int> onItemRender {
            get { return m_LoopScrollRect.onItemRender; }
            set {
                m_LoopScrollRect.onItemRender = value;
                OnPropertyChanged ();
            }
        }

        public Action<object, object, int, int> onSelected {
            get { return m_LoopScrollRect.onSelected; }
            set {
                m_LoopScrollRect.onSelected = value;
                OnPropertyChanged ();
            }
        }

        public ICommand itemCommand {
            get { return m_LoopScrollRect.itemCommand; }
            set {
                m_LoopScrollRect.itemCommand = value;
                OnPropertyChanged ();
            }
        }

        public object itemParameter {
            get { return m_LoopScrollRect.itemParameter; }
            set {
                m_LoopScrollRect.itemParameter = value;
                OnPropertyChanged ();
            }
        }

        public int dataLength {
            get { return m_LoopScrollRect.dataLength; }
            set {
                m_LoopScrollRect.dataLength = value;
                OnPropertyChanged ();
            }
        }

        #endregion

        #region  集合数据变更

        protected override void OnCollectionAdd (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
                count = args.NewItems.Count;
            m_LoopScrollRect.InsertAt (index, count);
            // Debug.LogFormat ("OnCollectionAdd(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopScrollRect.dataLength,items.Count);
        }

        protected override void OnCollectionRemove (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.OldItems != null)
                count = args.OldItems.Count;

            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopScrollRect.dataLength,items.Count);
            if (index >= 0)
                m_LoopScrollRect.RemoveAt (index, count);
            else
                m_LoopScrollRect.RemoveAt (0, count);

        }

        protected override void OnCollectionRepalce (object sender, NotifyCollectionChangedEventArgs args) {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
                count = args.NewItems.Count;

            m_LoopScrollRect.UpdateBegin (index, count);
            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,m_LoopScrollRect.dataLength,items.Count);

        }

        protected override void OnCollectionMove (object sender, NotifyCollectionChangedEventArgs args) {

        }

        protected override void OnCollectionReSet (object sender, NotifyCollectionChangedEventArgs args) {
            m_LoopScrollRect.dataLength = 0;
            m_LoopScrollRect.Refresh ();
        }

        #endregion

        #region  渲染相关
        protected void OnItemInstantiated (object obj1, object obj2, int index) {

        }

        protected void OnItemRender (object obj1, object obj2, int index) {
            BindableContainer item = (BindableContainer) obj2;
            if (item != null) {
                item.SetParent (this); //设置上下文关系利用source表达式 parent.context把事件绑定到父级context
                item.forceContextChanged = true;
                item.context = items[index];
            }
        }

        #endregion
        protected override void OnBindingContextChanged () {
            // if (GetBinding (OnItemInstantiatedProperty) == null)
            //     m_LoopScrollRect.onInstantiated = OnItemInstantiated;

            if (GetBinding (OnItemRenderProperty) == null)
                m_LoopScrollRect.onItemRender = OnItemRender;

            items = (IList) context;

            base.OnBindingContextChanged ();

            m_LoopScrollRect.dataLength = items.Count;
            m_LoopScrollRect.Refresh ();

        }

    }
}