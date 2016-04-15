// Copyright (c) 2014 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;

using SLua;
using Lua = SLua.LuaState;
//using LuaState = System.IntPtr;
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

    public static void onPressHandle(GameObject sender, object arg)
    {
        if (onPressFn != null && sender != null)
        {
            onPressFn.call(sender, arg);
        }
    }
    public static void onPressHandle(GameObject sender, bool arg)
    {
        if (onPressFn != null && sender != null)
        {
            onPressFn.call(sender, arg);
        }
    }

    public static void onClickHandle(GameObject sender, object arg)
    {
        if (onClickFn != null && sender != null)
        {
            onClickFn.call(sender, arg);
        }
    }
    public static void onClickHandle(GameObject sender, Vector3 arg)
    {
        if (onClickFn != null && sender != null)
        {
            onClickFn.call(sender, arg);
        }
    }

    public static void onDragHandle(GameObject sender, Vector3 arg)
    {
        if (onDragFn != null && sender != null)
        {
            onDragFn.call(sender, arg);
        }
    }

    public static void onDropHandle(GameObject sender, object arg)
    {
        if (onDropFn != null && sender != null)
        {
            onDropFn.call(sender, arg);
        }
    }
    public static void onDropHandle(GameObject sender, bool arg)
    {
        if (onDropFn != null && sender != null)
        {
            onDropFn.call(sender, arg);
        }
    }

    public static void onDropHandle(GameObject sender, Vector2 arg)
    {
        if (onDropFn != null && sender != null)
        {
            onDropFn.call(sender, arg);
        }
    }

    public static void onSelectHandle(GameObject sender, object arg)
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
    public static void onCancelHandle(GameObject sender, object arg)
    {
        if (onCancelFn != null && sender != null)
        {
            onCancelFn.call(sender, arg);
        }
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

