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
#if PROFILER_DUMP || !HUGULA_RELEASE
            using (var profiler = ProfilerFactory.GetAndStartComponentProfiler(bindable, "SetContext"))
            {
#endif
                if (data is LuaTable)
                {
                    var lua = data as LuaTable;
                    if (lua.ContainsKey<string>("PropertyChanged")) //ºÏ≤‚ Ù–‘
                    {
#if LUA_PROFILER_DEBUG
                        UnityEngine.Profiling.Profiler.BeginSample("BindingUtility.SetContext_LuaCast:" + bindable ?? "");
#endif
                        bindable.context = lua.Cast<INotifyPropertyChanged>();
#if LUA_PROFILER_DEBUG
                        UnityEngine.Profiling.Profiler.EndSample();
#endif
                        return;
                    }
                }
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("BindingUtility.SetContext:" + bindable ?? "");
#endif
                bindable.context = data;
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
#if PROFILER_DUMP || !HUGULA_RELEASE
            }
#endif


        }

        public static void SetContextByINotifyTable(BindableObject bindable, INotifyTable notify)
        {
#if PROFILER_DUMP || !HUGULA_RELEASE
            using (var profiler = ProfilerFactory.GetAndStartComponentProfiler(bindable, "SetContextByINotifyTable"))
            {
#endif
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("BindingUtility.SetContextByINotifyTable:" + bindable ?? "");
#endif
                bindable.context = notify;
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
#if PROFILER_DUMP || !HUGULA_RELEASE
            }
#endif
        }

        public static void SetContextByIList(BindableObject bindable, IList list)
        {
#if PROFILER_DUMP || !HUGULA_RELEASE
            using (var profiler = ProfilerFactory.GetAndStartComponentProfiler(bindable, "SetContextByIList"))
            {
#endif
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.BeginSample("BindingUtility.SetContextByIList:" + bindable ?? "");
#endif
                bindable.context = list;
#if LUA_PROFILER_DEBUG
                UnityEngine.Profiling.Profiler.EndSample();
#endif
#if PROFILER_DUMP || !HUGULA_RELEASE
            }
#endif
        }
        public static void SetContextByINotifyPropertyChanged(BindableObject bindable, INotifyPropertyChanged notify)
        {
            if (bindable == null)
            {
                UnityEngine.Debug.LogWarningFormat("SetContextByINotifyPropertyChanged arg({0}) is null", bindable);
            }
            else
            {
#if PROFILER_DUMP || !HUGULA_RELEASE
                using (var profiler = ProfilerFactory.GetAndStartComponentProfiler(bindable, "SetContextByINotifyPropertyChanged"))
                {
#endif
#if LUA_PROFILER_DEBUG
                    UnityEngine.Profiling.Profiler.BeginSample("BindingUtility.SetContextByINotifyPropertyChanged:" + bindable ?? "");
#endif
                    bindable.context = notify;
#if LUA_PROFILER_DEBUG
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
#if PROFILER_DUMP || !HUGULA_RELEASE
                }
#endif
            }
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
            UnityEngine.Debug.LogWarning(" ConvertToNotifyTable:" + table);
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

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItems(NotifyCollectionChangedAction action)
        {
            // return new HugulaNotifyCollectionChangedEventArgs(action, changedItems);
            var arg = m_EventArgsPool.Get();
            if (action != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Reset), "action");

            arg.InitializeAdd(action, null, -1);

            return arg;
        }

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItems(NotifyCollectionChangedAction action, IList changedItems)
        {
            // return new HugulaNotifyCollectionChangedEventArgs(action, changedItems);
            var arg = m_EventArgsPool.Get();
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                    && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");

                arg.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                    throw new ArgumentNullException("changedItems");

                arg.InitializeAddOrRemove(action, changedItems, -1);
            }
            return arg;
        }
        //public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItem(NotifyCollectionChangedAction action, object changedItem)
        //{
        //    return new HugulaNotifyCollectionChangedEventArgs(action, changedItem);
        //}

        //public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemsOldItems(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        //{
        //    return new HugulaNotifyCollectionChangedEventArgs(action, newItems, oldItems);
        //}

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemsStartingIndex(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            //return new HugulaNotifyCollectionChangedEventArgs(action, changedItems, startingIndex);
            var arg = m_EventArgsPool.Get();
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                  && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");
                if (startingIndex != -1)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresIndexMinus1), "action");

                arg.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                    throw new ArgumentNullException("changedItems");
                if (startingIndex < -1)
                    throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "startingIndex");

                arg.InitializeAddOrRemove(action, changedItems, startingIndex);
            }

            return arg;

        }

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            //return new HugulaNotifyCollectionChangedEventArgs(action, changedItem, index);
            var arg = m_EventArgsPool.Get();
            if ((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)
                   && (action != NotifyCollectionChangedAction.Reset))
                throw new ArgumentException(SR.GetString(SR.MustBeResetAddOrRemoveActionForCtor), "action");

            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresNullItem), "action");
                if (index != -1)
                    throw new ArgumentException(SR.GetString(SR.ResetActionRequiresIndexMinus1), "action");

                arg.InitializeAdd(action, null, -1);
            }
            else
            {
                arg.InitializeAddOrRemove(action, new object[] { changedItem }, index);
            }

            return arg;
        }

        //public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemOldItem(NotifyCollectionChangedAction action, object newItem, object oldItem)
        //{
        //    return HugulaNotifyCollectionChangedEventArgs(action, newItem, oldItem);
        //}

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemsOldItemsStartingIndex(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            //return new HugulaNotifyCollectionChangedEventArgs(action, newItems, oldItems, startingIndex);
            var arg = m_EventArgsPool.Get();
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");
            if (newItems == null)
                throw new ArgumentNullException("newItems");
            if (oldItems == null)
                throw new ArgumentNullException("oldItems");

            arg.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);

            return arg;
        }

        //public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemsIndexOldIndex(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        //{
        //    //return new HugulaNotifyCollectionChangedEventArgs(action, changedItems, index, oldIndex);
        //    var arg = m_EventArgsPool.Get();

        //}

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsChangedItemIndexOldIndex(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            //return new HugulaNotifyCollectionChangedEventArgs(action, changedItem, index, oldIndex);
            var arg = m_EventArgsPool.Get();
            if (action != NotifyCollectionChangedAction.Move)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Move), "action");
            if (index < 0)
                throw new ArgumentException(SR.GetString(SR.IndexCannotBeNegative), "index");

            object[] changedItems = new object[] { changedItem };
            arg.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
            return arg;
        }

        public static HugulaNotifyCollectionChangedEventArgs CreateCollectionArgsNewItemOldItemIndex(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            var arg = m_EventArgsPool.Get();
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException(SR.GetString(SR.WrongActionForCtor, NotifyCollectionChangedAction.Replace), "action");

            int oldStartingIndex = index;

            arg.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, oldStartingIndex);

            return arg;
        }

    }
}