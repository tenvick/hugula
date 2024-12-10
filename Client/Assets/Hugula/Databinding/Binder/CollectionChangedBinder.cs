using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.UIComponents;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Hugula.Databinding.Binder
{
    [XLua.LuaCallCSharp]
    public abstract class CollectionChangedBinder<T> : UIBehaviourBinder<T>, ICollectionBinder where T : UnityEngine.Object
    {
        // [Tooltip("Whether context changed, force refresh binding. ")]
        // [SerializeField] protected bool m_forceBinding = true;

        private NotifyCollectionChangedEventHandlerEvent notify;
        /// <summary>
        /// 数据集合IList或者LuaTable
        /// </summary>
        protected object items;
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

        #region 数据操作

        XLua.LuaFunction m_GetItem;
        protected object GetDataItem(int index)
        {
            if (items is IList list)
            {
                return list[index];
            }
            else if (items is XLua.LuaTable luaTable)
            {
                if(m_GetItem !=  null)
                {
                    return m_GetItem.Func<LuaTable,int,object>(luaTable,index);
                }else
                    return luaTable.Get<int,object>(index+1);

            }
            return null;
        }

        private int m_DataCount = -1;

        protected int GetDataCount()
        {
            if(m_DataCount == -1)
            {
                m_DataCount = 0;
                if (items is IList list)
                {
                    m_DataCount = list.Count;
                }
                else if (items is XLua.LuaTable luaTable)
                {
                    m_DataCount = luaTable.Count; //
                }
            }
          
            return m_DataCount;
        }

        #endregion


        public abstract IList<BindableObject> GetChildren();

        protected override void OnBindingContextChanged()
        {
            
            items = context;
            
            m_GetItem = null;
            m_DataCount = -1;

            NotifyCollectionChangedEventHandlerEvent notifyCCEvent = null;
            if (context is INotifyCollectionChanged incc)
            {
                notifyCCEvent = incc.CollectionChanged;              
            }
            else if(context is XLua.LuaTable luaTable)
            {
                luaTable.TryGet<string, NotifyCollectionChangedEventHandlerEvent>("CollectionChanged",out notifyCCEvent);
                luaTable.TryGet<string, XLua.LuaFunction>("get_Item",out m_GetItem);
            }

            base.OnBindingContextChanged();

            if (ReferenceEquals(notifyCCEvent, notify))
                   return;

            notify?.Remove(OnCollectionChanged);

            if(notifyCCEvent != null)
            {
                notifyCCEvent.Add(OnCollectionChanged);
            }
            notify = notifyCCEvent;

        }

        protected override void OnDestroy()
        {
            notify?.Remove(OnCollectionChanged);
            notify = null;
            items = null;
            m_GetItem = null;

            base.OnDestroy();
        }

        public override void ClearBinding()
        {
            base.ClearBinding();
            notify?.Remove(OnCollectionChanged);

        }

    }
}