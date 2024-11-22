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
    [XLua.LuaCallCSharp]
    public class CollectionViewBinder : CollectionChangedBinder<Transform>
    {
        #region  数据绑定相关
        [Tooltip("template for clone")]
        [PopUpComponentsAttribute]
        public BindableObject templateItem;
        [Tooltip("销毁item")]
        public bool destoryItem = false;
        [Tooltip("是否在刷新的时候设置顺序")]
        public bool enableSiblingIndex = true;
        // IList items;
        List<BindableObject> viewItems = new List<BindableObject>();

        protected override void OnCollectionAdd(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 0;
            if (args.NewItems > 0)
            {
                count = args.NewItems;
            }

            InsertItems(index, count);
        }


        virtual protected void ItemActive(GameObject ob, bool active)
        {
            ob.SetActive(active);
        }

        protected override void OnCollectionRemove(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.OldStartingIndex;
            int count = 0;
            if (args.OldItems > 0)
                count = args.OldItems;

            if (index >= 0)
                RemoveItems(index, count);
            else
            {
                UpdateView(0, GetDataCount());
            }
        }

        protected override void OnCollectionRepalce(object sender, HugulaNotifyCollectionChangedEventArgs args)
        {
            var index = args.NewStartingIndex;
            int count = 0;
            if (args.NewItems > 0)
                count = args.NewItems;

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
                if (!item.gameObject.activeSelf) ItemActive(item.gameObject, true);
                if (enableSiblingIndex) item.transform.SetSiblingIndex(i);
                itemData = GetDataItem(i);  //items[i];
                BindingUtility.SetContext(item, itemData);

            }
        }

        void ClearItems()
        {
            foreach (var item in viewItems)
            {
                item.ClearContextRef();
                if (destoryItem)
                {
                    GameObject.Destroy(item.gameObject);
                }
                else
                {
                    item.Unapply();
                    ItemActive(item.gameObject, false);
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
                ItemActive(view.gameObject, false);
                viewItems.Add(view);
            }
        }

        /// <summary>
        /// 获取item
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public BindableObject GetItemAt(int idx)
        {
            if (idx >= 0 && idx < viewItems.Count)
                return viewItems[idx];
            else
                return null;

        }

        int CheckCount(int endIdx)
        {
            if (endIdx >= GetDataCount()) endIdx = GetDataCount();
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

                if (!item.gameObject.activeSelf) ItemActive(item.gameObject, true);
                if (enableSiblingIndex) item.transform.SetSiblingIndex(i);


                itemData = GetDataItem(i);  // items[i];
                item.forceContextChanged = m_forceBinding;
                BindingUtility.SetContext(item, itemData);
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
            if (context != null && !(context is IList || context is XLua.LuaTable))
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

            ClearItems();
            base.OnBindingContextChanged();

            UpdateView(0, GetDataCount());
        }

        protected override void OnDestroy()
        {
            destoryItem = true;
            ClearItems();
            base.OnDestroy();
        }
    }
}