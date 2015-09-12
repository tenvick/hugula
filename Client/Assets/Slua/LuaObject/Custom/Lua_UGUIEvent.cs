using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UGUIEvent : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onCustomerHandle_s(IntPtr l) {
		try {
			System.Object a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			UGUIEvent.onCustomerHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onPressHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.Boolean a2;
			checkType(l,2,out a2);
			UGUIEvent.onPressHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onClickHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			UGUIEvent.onClickHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onDragHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			UnityEngine.Vector2 a2;
			checkType(l,2,out a2);
			UGUIEvent.onDragHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onDropHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			UGUIEvent.onDropHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onSelectHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			UGUIEvent.onSelectHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int onCancelHandle_s(IntPtr l) {
		try {
			UnityEngine.GameObject a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			UGUIEvent.onCancelHandle(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onCustomerFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onCustomerFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onCustomerFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onCustomerFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onPressFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onPressFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onPressFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onPressFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onClickFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onClickFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onClickFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onClickFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onDragFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onDragFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onDragFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onDragFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onDropFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onDropFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onDropFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onDropFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onSelectFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onSelectFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onSelectFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onSelectFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onCancelFn(IntPtr l) {
		try {
			pushValue(l,UGUIEvent.onCancelFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onCancelFn(IntPtr l) {
		try {
			SLua.LuaFunction v;
			checkType(l,2,out v);
			UGUIEvent.onCancelFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UGUIEvent");
		addMember(l,onCustomerHandle_s);
		addMember(l,onPressHandle_s);
		addMember(l,onClickHandle_s);
		addMember(l,onDragHandle_s);
		addMember(l,onDropHandle_s);
		addMember(l,onSelectHandle_s);
		addMember(l,onCancelHandle_s);
		addMember(l,"onCustomerFn",get_onCustomerFn,set_onCustomerFn,false);
		addMember(l,"onPressFn",get_onPressFn,set_onPressFn,false);
		addMember(l,"onClickFn",get_onClickFn,set_onClickFn,false);
		addMember(l,"onDragFn",get_onDragFn,set_onDragFn,false);
		addMember(l,"onDropFn",get_onDropFn,set_onDropFn,false);
		addMember(l,"onSelectFn",get_onSelectFn,set_onSelectFn,false);
		addMember(l,"onCancelFn",get_onCancelFn,set_onCancelFn,false);
		createTypeMetatable(l,null, typeof(UGUIEvent));
	}
}
