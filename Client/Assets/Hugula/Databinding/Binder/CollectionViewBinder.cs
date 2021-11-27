using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    ///<summary>
    /// 普通集合绑定view
    ///</summary>
    public sealed class CollectionViewBinder : CollectionChangedBinder<Transform>
    {
        #region  数据绑定相关
        [Tooltip("template for clone")]
        [PopUpComponentsAttribute]
        public BindableObject templateItem;
        [Tooltip("销毁item")]
        public bool destoryItem = false;
        IList items;
        List<BindableObject> viewItems = new List<BindableObject>();

        protected override void OnCollectionAdd(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
            {
                count = args.NewItems.Count;
            }

            InsertItems(index, count);
        }

        protected override void OnCollectionRemove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.OldStartingIndex;
            int count = 1;
            if (args.OldItems != null)
                count = args.OldItems.Count;

            if (index >= 0)
                RemoveItems(index, 1);
            else
            {
                UpdateView(0, items.Count);
            }
        }

        protected override void OnCollectionRepalce(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 1;
            if (args.NewItems != null)
                count = args.NewItems.Count;

            UpdateView(index, count);
        }

        protected override void OnCollectionMove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            var oldIndex = args.OldStartingIndex;
            MoveItems(index, oldIndex);
        }

        protected override void OnCollectionReSet(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            ClearItems();
        }

        #endregion

        #region  渲染相关

        void RemoveItems(int start, int count)
        {
            for (int i = start; i < start + count; i++)
            {
                var item = GetItemAt(i);
                if (item != null)
                {
                    item.context = null;
                    ReleaseItem(item);
                }
            }
        }

        void InsertItems(int start, int count)
        {
            object itemData = null;
            for (int i = start; i < start + count; i++)
            {
                var item = GetItemAt(i);
                if (item == null)
                {
                    item = GameObject.Instantiate<BindableObject>(templateItem, this.transform);
                    viewItems.Add(item);
                }
                if (!item.gameObject.activeSelf) item.gameObject.SetActive(true);
                item.transform.SetSiblingIndex(i);
                itemData = items[i];
                item.context = itemData;

            }
        }

        void ClearItems()
        {
            foreach (var item in viewItems)
            {
                item.context = null;
                if (destoryItem)
                {
                    GameObject.Destroy(item.gameObject);
                }
                else
                {
                    item.Unapply();
                    item.gameObject.SetActive(false);
                }
            }

            if (destoryItem)
                viewItems.Clear();
        }

        void MoveItems(int newIndex, int oldIndex)
        {
            var newView = GetItemAt(newIndex);
            var oldView = GetItemAt(oldIndex);
            if (newView && oldView)
            {
                newView.transform.SetSiblingIndex(oldIndex);
                oldView.transform.SetSiblingIndex(newIndex);
            }
        }

        void ReleaseItem(BindableObject view)
        {
            if (destoryItem)
            {
                viewItems.Remove(view);
                GameObject.Destroy(view.gameObject);
            }
            else
            {
                int idx = viewItems.IndexOf(view);
                viewItems.RemoveAt(idx);
                view.transform.SetSiblingIndex(viewItems.Count - 1);
                view.Unapply();
                view.gameObject.SetActive(false);
                viewItems.Add(view);
            }
        }

        BindableObject GetItemAt(int idx)
        {
            if (idx >= 0 && idx < viewItems.Count)
                return viewItems[idx];
            else
                return null;

        }

        int CheckCount(int endIdx)
        {
            if (endIdx >= items.Count) endIdx = items.Count;
            return endIdx;
        }

        void UpdateView(int index, int count)
        {
            count = CheckCount(index + count);

            object itemData = null;

            for (int i = index; i < count; i++)
            {
                var item = GetItemAt(i);
                if (item == null)
                {
                    item = GameObject.Instantiate<BindableObject>(templateItem, this.transform);
                    viewItems.Add(item);
                }

                if (!item.gameObject.activeSelf) item.gameObject.SetActive(true);
                item.transform.SetSiblingIndex(i);

                itemData = items[i];
                item.forceContextChanged = m_forceBinding;
                item.context = itemData;
            }
        }

        #endregion

        public override IList<BindableObject> GetChildren()
        {
            var list = new List<BindableObject>();
            list.Add(templateItem);
            return list;
        }

        protected override void OnBindingContextChanged()
        {

#if !HUGULA_RELEASE || UNITY_EDITOR
            if (context != null && !(context is IList))
            {
                var t = this.transform;
                var path = this.ToString();
                while (t.parent != null)
                {
                    path = t.parent.name + "." + path;
                    t = t.parent;
                }
                Debug.LogWarningFormat("{0} context is not IList ({1}) ", path, context);
            }
#endif
            if (context is IList)
                items = (IList)context;
            else
                items = null;

            ClearItems();
            base.OnBindingContextChanged();

            if (items != null)
                UpdateView(0, items.Count);
        }

        protected override void OnDestroy()
        {
            destoryItem = true;
            ClearItems();
            items = null;
            base.OnDestroy();
        }
    }
}