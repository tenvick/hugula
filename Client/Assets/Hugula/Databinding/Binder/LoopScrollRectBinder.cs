using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(LoopScrollRect))]
    [XLua.LuaCallCSharp]
    public class LoopScrollRectBinder : CollectionChangedBinder<LoopScrollRect>
    {
        public const string OnItemInstantiatedProperty = "onItemInstantiated";
        public const string OnItemRenderProperty = "onItemRender";
        // IList items;

        #region  重写属性

        public int setScrollIndex
        {
            get { return target.setScrollIndex; }
            set
            {
                target.setScrollIndex = value;
                // OnPropertyChanged();
            }
        }

        public ICommand droppedCommand
        {
            get { return target.droppedCommand; }
            set
            {
                target.droppedCommand = value;
                // OnPropertyChanged();
            }
        }

        public Action<Vector2> onDropped
        {
            get { return target.onDropped; }
            set
            {
                target.onDropped = value;
                // OnPropertyChanged();
            }
        }

        public float scrollTime
        {
            get { return target.scrollTime; }
            set
            {
                target.scrollTime = value;
                // OnPropertyChanged();
            }
        }


        public int columns
        {
            get { return target.columns; }
        }

        public int pageSize
        {
            get { return target.pageSize; }
        }

        public Vector2 itemSize
        {
            get { return target.itemSize; }
        }

        public Vector4 padding
        {
            get { return target.padding; }
            set
            {
                target.padding = value;
                // OnPropertyChanged();
            }
        }

        public bool removeEasing
        {
            get { return target.removeEasing; }
            set
            {
                target.removeEasing = value;
                // OnPropertyChanged();
            }
        }

        public int selectedIndex
        {
            get
            {
                return target.selectedIndex;
            }
            set
            {
                target.selectedIndex = value;
            }
        }

        public Action<object, int, int> onScrollIndexChanged
        {
            get { return target.onScrollIndexChanged; }
            set
            {
                target.onScrollIndexChanged = value;
                // OnPropertyChanged();
            }
        }

        public Action<object, object, int> onInstantiated
        {
            get { return target.onInstantiated; }
            set
            {
                target.onInstantiated = value;
                // OnPropertyChanged();
            }
        }

        // public Func<object, int, Component, int, RectTransform, Component> onGetItem
        // {
        //     get { return target.onGetItem; }
        //     set
        //     {
        //         target.onGetItem = value;
        //         // OnPropertyChanged();
        //     }
        // }

        public Action<object, object, int> onItemRender
        {
            get { return target.onItemRender; }
            set
            {
                target.onItemRender = value;
                // OnPropertyChanged();
            }
        }


        public Action<object, object, int, int> onSelected
        {
            get { return target.onSelected; }
            set
            {
                target.onSelected = value;
                // OnPropertyChanged();
            }
        }

        public ICommand itemCommand
        {
            get { return target.itemCommand; }
            set
            {
                target.itemCommand = value;
                // OnPropertyChanged();
            }
        }

        public object itemParameter
        {
            get { return target.itemParameter; }
            set
            {
                target.itemParameter = value;
                // OnPropertyChanged();
            }
        }

        public int dataLength
        {
            get { return target.dataLength; }
            set
            {
                target.dataLength = value;
                // OnPropertyChanged();
            }
        }

        #endregion

        #region  集合数据变更

        protected override void OnCollectionAdd(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 0;
            if (args.NewItems > 0)
                count = args.NewItems;
            target.InsertAt(index, count);
        }

        protected override void OnCollectionRemove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.OldStartingIndex;
            int count = 0;
            if (args.OldItems > 0)
                count = args.OldItems;

            if (index >= 0)
                target.RemoveAt(index, count);
            else
                target.RemoveAt(0, count);

        }

        protected override void OnCollectionRepalce(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 0;
            if (args.NewItems > 0)
                count = args.NewItems;

            target.UpdateBegin(index, count);

        }

        protected override void OnCollectionMove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {

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
                BindingUtility.SetContext(item, GetDataItem(index));
            }
        }

        protected override void OnBindingContextChanged()
        {
            // if (GetBinding (OnItemInstantiatedProperty) == null)
            //     target.onInstantiated = OnItemInstantiated;

            if (target.onItemRender == null)
                target.onItemRender = OnItemRender;

            base.OnBindingContextChanged();

            target.dataLength = GetDataCount();

            target.Refresh();

        }

        #endregion

        void OnScrollIndexValueChanged(object obj, int curr, int last)
        {
            OnPropertyChangedBindingApply("setScrollIndex");

        }

        protected override void Awake()
        {
            target.onScrollIndexChanged += OnScrollIndexValueChanged;
            base.Awake();
        }

        public override IList<BindableObject> GetChildren()
        {
            var list = new List<BindableObject>();
            list.AddRange(target.templates);
            return list;
        }

        protected override void OnDestroy()
        {
            target.onScrollIndexChanged -= OnScrollIndexValueChanged;
            items = null;
            base.OnDestroy();
        }
    }
}