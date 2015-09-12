using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_LOD : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.LOD o;
			System.Single a1;
			checkType(l,2,out a1);
			UnityEngine.Renderer[] a2;
			checkType(l,3,out a2);
			o=new UnityEngine.LOD(a1,a2);
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_screenRelativeTransitionHeight(IntPtr l) {
		try {
			UnityEngine.LOD self;
			checkValueType(l,1,out self);
			pushValue(l,self.screenRelativeTransitionHeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_screenRelativeTransitionHeight(IntPtr l) {
		try {
			UnityEngine.LOD self;
			checkValueType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.screenRelativeTransitionHeight=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_renderers(IntPtr l) {
		try {
			UnityEngine.LOD self;
			checkValueType(l,1,out self);
			pushValue(l,self.renderers);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_renderers(IntPtr l) {
		try {
			UnityEngine.LOD self;
			checkValueType(l,1,out self);
			UnityEngine.Renderer[] v;
			checkType(l,2,out v);
			self.renderers=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.LOD");
		addMember(l,"screenRelativeTransitionHeight",get_screenRelativeTransitionHeight,set_screenRelativeTransitionHeight,true);
		addMember(l,"renderers",get_renderers,set_renderers,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.LOD),typeof(System.ValueType));
	}
}
