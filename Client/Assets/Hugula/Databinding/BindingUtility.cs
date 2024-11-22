using System;
using System.Collections;
using System.Collections.Specialized;
using Hugula.Databinding;
using Hugula.Profiler;
using Hugula.Utils;
using XLua;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public static class BindingUtility
    {

        public static void SetContext(BindableObject bindable, object data)
        {
#if !HUGULA_RELEASE
            UnityEngine.Profiling.Profiler.BeginSample($"{bindable?.name}:SetContext");
#endif
          
            bindable.SetInheritedContext(data);

#if !HUGULA_RELEASE
            UnityEngine.Profiling.Profiler.EndSample();
#endif


        }

        public static BindableContainer GetBindableContainer(UnityEngine.GameObject obj)
        {
            return obj.GetComponent<BindableContainer>();
        }

        public static BindableObject GetBindableObject(UnityEngine.GameObject obj)
        {
            return obj.GetComponent<BindableObject>();
        }

        public static object ConvertToNotifyTable(INotifyPropertyChanged table)
        {
            return table;
        }

    }



    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public static class CollectionChangedEventArgsUtility
    {

        #region pool

        internal static Hugula.Utils.ObjectPool<HugulaNotifyCollectionChangedEventArgs> m_EventArgsPool = new ObjectPool<HugulaNotifyCollectionChangedEventArgs>(m_ActionOnGet, m_ActionOnRelease, 64);

        internal static void m_ActionOnGet(HugulaNotifyCollectionChangedEventArgs arg)
        {

        }

        internal static void m_ActionOnRelease(HugulaNotifyCollectionChangedEventArgs arg)
        {
            arg.Release();
        }

        public static void Release(HugulaNotifyCollectionChangedEventArgs arg)
        {
            m_EventArgsPool.Release(arg);
        }

        #endregion

        /// <summary>
        /// Reset 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItems(NotifyCollectionChangedAction action)
        {
            if (action != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Reset), "action");

            var arg = m_EventArgsPool.Get();
            arg.InitializeAdd(action, 0, -1);

            return arg;
        }

        /// <summary>
        /// 从startingIndex插入了changedItems个元素
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="startingIndex"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemsStartingIndex(NotifyCollectionChangedAction action, int changedItems, int startingIndex)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                  && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            var arg = m_EventArgsPool.Get();
            if (action == NotifyCollectionChangedAction.Reset)
            {
                // if (changedItems != 0)
                //     throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");
                // if (startingIndex != -1)
                //     throw new ArgumentException(SR.GetString(SR.ResetActionRequiresIndexMinus1), "action");

                arg.InitializeAdd(action, 0, -1);
            }
            else
            {
                // if (changedItems == 0)
                //     throw new ArgumentNullException("changedItems");
                if (startingIndex < -1)
                    throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "startingIndex");

                arg.InitializeAddOrRemove(action, changedItems, startingIndex);
            }

            return arg;

        }

        /// <summary>
        /// ChangedItem 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItemCount">数量</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction action, int changedItemCount, int index)
        {
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                   && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            var arg = m_EventArgsPool.Get();

            if (action == NotifyCollectionChangedAction.Reset)
            {
                arg.InitializeAdd(action, 0, -1);
            }
            if (action == NotifyCollectionChangedAction.Add)
                arg.InitializeAdd(action, changedItemCount, index);
            else if (action == NotifyCollectionChangedAction.Remove)
                arg.InitializeRemove(action, changedItemCount, index);
            else
                UnityEngine.Debug.LogError(String.Format("Unsupported action: {0}", action.ToString()));

            return arg;
        }

        /// <summary>
        /// startingIndex 替换元素 newItems替换数量
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        /// <param name="startingIndex"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemsOldItemsStartingIndex(NotifyCollectionChangedAction action, int newItems, int oldItems, int startingIndex)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");
            if (newItems == 0)
                throw new ArgumentNullException("newItems");
            if (oldItems == 0)
                throw new ArgumentNullException("oldItems");

            var arg = m_EventArgsPool.Get();
            arg.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);

            return arg;
        }

        /// <summary>
        /// Move 交换
        /// </summary>
        /// <param name="action"></param>
        /// <param name="changedItems"></param>
        /// <param name="index"></param>
        /// <param name="oldIndex"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndexOldIndex(NotifyCollectionChangedAction action, int changedItems, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Move), "action");
            if (index < 0)
                throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "index");

            var arg = m_EventArgsPool.Get();
            arg.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
            return arg;
        }

        /// <summary>
        /// 替换了index处的元素newItem个
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemOldItemIndex(NotifyCollectionChangedAction action, int newItem, int oldItem, int index)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");

            var arg = m_EventArgsPool.Get();
            int oldStartingIndex = index;

            arg.InitializeMoveOrReplace(action, newItem, oldItem, index, oldStartingIndex);

            return arg;
        }

    }
}