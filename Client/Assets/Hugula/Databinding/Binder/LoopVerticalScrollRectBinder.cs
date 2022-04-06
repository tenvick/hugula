using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    [RequireComponent(typeof(LoopVerticalScrollRect))]
    public class LoopVerticalScrollRectBinder : CollectionChangedBinder<LoopVerticalScrollRect>
    {
        public const string OnItemInstantiatedProperty = "onItemInstantiated";
        public const string OnItemRenderProperty = "onItemRender";

        IList items;

        #region  重写属性

        public System.Action<object, int, int> onScrollIndexChanged
        {
            get { return target.onScrollIndexChanged; }
            set
            {
                target.onScrollIndexChanged = value;
                OnPropertyChanged();
            }
        }

        public float scrollTime{
            get { return target.scrollTime; }
            set
            {
                target.scrollTime = value;
                OnPropertyChanged();
            }
        }
        
        public int setScrollIndex
        {
            get { return target.setScrollIndex; }
            set
            {
                target.setScrollIndex = value;
                OnPropertyChanged();
            }
        }

        public bool scrollToBottom
        {
            get { return target.scrollToBottom; }
            set
            {
                target.scrollToBottom = value;
                OnPropertyChanged();
            }
        }

        public int pageSize
        {
            get { return target.pageSize; }
        }

        public int dragOffsetShow
        {
            get { return target.dragOffsetShow; }
            set
            {
                target.dragOffsetShow = value;
            }
        }

        public float padding
        {
            get { return target.padding; }
            set
            {
                target.padding = value;
                OnPropertyChanged();
            }
        }

        public int renderPerFrames
        {
            get { return target.renderPerFrames; }
            set
            {
                target.renderPerFrames = value;
                OnPropertyChanged();
            }
        }

        public int selectedIndex
        {
            get { return target.selectedIndex; }
        }

        public bool isLoadingData
        {
            get { return target.isLoadingData; }
            set
            {
                target.isLoadingData = value;
                OnPropertyChanged();
            }
        }

        public Action<object, object, int> onInstantiated
        {
            get { return target.onInstantiated; }
            set
            {
                target.onInstantiated = value;
                OnPropertyChanged();
            }
        }

        public Func<object, int, int> onGetItemTemplateType
        {
            get { return target.onGetItemTemplateType; }
            set
            {
                target.onGetItemTemplateType = value;
                OnPropertyChanged();
            }
        }

        public Action<object, Component, int> onItemRender
        {
            get { return target.onItemRender; }
            set
            {
                target.onItemRender = value;
                OnPropertyChanged();
            }
        }

        public Action<object, object, int, int> onSelected
        {
            get { return target.onSelected; }
            set
            {
                target.onSelected = value;
                OnPropertyChanged();
            }
        }

        public ICommand itemCommand
        {
            get { return target.itemCommand; }
            set
            {
                target.itemCommand = value;
                OnPropertyChanged();
            }
        }

        public object itemParameter
        {
            get { return target.itemParameter; }
            set
            {
                target.itemParameter = value;
                OnPropertyChanged();
            }
        }

        public int dataLength
        {
            get { return target.dataLength; }
            set
            {
                target.dataLength = value;
                OnPropertyChanged();
            }
        }

        public ICommand droppedCommand
        {
            get { return target.droppedCommand; }
            set
            {
                target.droppedCommand = value;
                OnPropertyChanged();
            }
        }

        public Action<Vector2> onDropped
        {
            get { return target.onDropped; }
            set
            {
                target.onDropped = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region  集合数据变更

        protected override void OnCollectionAdd(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
            {
                count = args.NewItems.Count;
            }
            target.InsertRange(index, count);
            // Debug.LogFormat ("OnCollectionAdd(index={0},count={1},datalen={2},items.count={3}) ", index, count,target.dataLength,items.Count);
        }

        protected override void OnCollectionRemove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.OldItems != null)
                count = args.OldItems.Count;

            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,target.dataLength,items.Count);
            if (index >= 0)
                target.RemoveAt(index, count);
            else
                target.RemoveAt(0, count);

        }

        protected override void OnCollectionRepalce(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
                count = args.NewItems.Count;

            target.UpdateBegin(index);
            // Debug.LogFormat ("OnCollectionRepalce(index={0},count={1},datalen={2},items.count={3}) ", index, count,target.dataLength,items.Count);

        }

        protected override void OnCollectionMove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            var oldIndex = args.OldStartingIndex;
            target.UpdateBegin(index);
        }

        protected override void OnCollectionReSet(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            target.dataLength = 0;
            target.Refresh();
        }

        #endregion

        #region  渲染相关
        protected void OnItemInstantiated(object obj1, object obj2, int index)
        {

        }

        protected void OnItemRender(object obj1, object obj2, int index)
        {
            BindableObject item = (BindableObject)obj2;
            if (item != null)
            {
                item.forceContextChanged = m_forceBinding;
                // item.context = items[index];
                BindingUtility.SetContext(item, items[index]);
            }
        }

        #endregion
        protected override void OnBindingContextChanged()
        {
            // if (GetBinding (OnItemInstantiatedProperty) == null)
            //     target.onInstantiated = OnItemInstantiated;

            if (GetBinding(OnItemRenderProperty) == null)
                target.onItemRender = OnItemRender;

            if (context is IList)
                items = (IList)context;
            else
                items = null;

            base.OnBindingContextChanged();

            target.dataLength = items?.Count ?? 0;
            target.Refresh();

        }
        public override IList<BindableObject> GetChildren()
        {
            var list = new List<BindableObject>();
            list.AddRange(target.templates);
            return list;
        }

        protected override void OnDestroy()
        {
            items = null;
            base.OnDestroy();
        }
    }
}