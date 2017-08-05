// Copyright (c) 2014 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using SLua;

namespace Hugula.UGUIExtend
{
    /// <summary>
    /// 事件处理
    /// </summary>
    [SLua.CustomLuaClass]
    public static class UGUIEvent
    {

        #region public

        public static void onCustomerHandle(object sender, object arg)
        {
            if (onCustomerFn != null)
            {
                onCustomerFn.call(sender, arg);
            }
        }

        public static void onCustomerHandle(object sender, Vector3 arg)
        {
            if (onCustomerFn != null && sender != null)
            {
                onCustomerFn.call(sender, arg);
            }
        }

        public static void onPressHandle(Object sender, object arg)
        {

            if (onPressFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onPressHandle");
#endif
                onPressFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }
        public static void onPressHandle(Object sender, bool arg)
        {

            if (onPressFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onPressHandle");
#endif
                onPressFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onClickHandle(Object sender, object arg)
        {

            if (onClickFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onClickHandle");
#endif
                onClickFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }
        public static void onClickHandle(Object sender, Vector3 arg)
        {

            if (onClickFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onClickHandle");
#endif
                onClickFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onDragHandle(Object sender, Vector3 arg)
        {

            if (onDragFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onDragHandle");
#endif
                onDragFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onDropHandle(Object sender, object arg)
        {

            if (onDropFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onDropHandle");
#endif
                onDropFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onDropHandle(Object sender, bool arg)
        {

            if (onDropFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onDropHandle");
#endif
                onDropFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onDropHandle(Object sender, Vector3 arg)
        {

            if (onDropFn != null && sender != null)
            {
#if HUGULA_PROFILER_DEBUG
                Profiler.BeginSample(sender.name + ")onDropHandle");
#endif
                onDropFn.call(sender, arg);
#if HUGULA_PROFILER_DEBUG
                Profiler.EndSample();
#endif
            }

        }

        public static void onSelectHandle(Object sender, object arg)
        {
            if (onSelectFn != null && sender != null)
            {
                onSelectFn.call(sender, arg);
            }
        }

        //public static void onDoubleClickHandle(GameObject sender, object arg)
        //{
        //    if (onDoubleFn != null)
        //    {
        //        onDoubleFn.call(sender, arg);
        //    }
        //}
        public static void onCancelHandle(Object sender, object arg)
        {
            if (onCancelFn != null && sender != null)
            {
                onCancelFn.call(sender, arg);
            }
        }

        public static void RemoveAllEvents()
        {
            onCustomerFn = null;
            onPressFn = null;
            onClickFn = null;
            onDragFn = null;
            onDropFn = null;
            onSelectFn = null;
            onCancelFn = null;
        }

        #endregion
        public static LuaFunction onCustomerFn;

        public static LuaFunction onPressFn;

        public static LuaFunction onClickFn;

        public static LuaFunction onDragFn;

        public static LuaFunction onDropFn;

        public static LuaFunction onSelectFn;

        public static LuaFunction onCancelFn;
        //public static LuaFunction onDoubleFn;

    }

}