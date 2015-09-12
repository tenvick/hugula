using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_OcclusionPortal : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.OcclusionPortal o;
			o=new UnityEngine.OcclusionPortal();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_open(IntPtr l) {
		try {
			UnityEngine.OcclusionPortal self=(UnityEngine.OcclusionPortal)checkSelf(l);
			pushValue(l,self.open);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_open(IntPtr l) {
		try {
			UnityEngine.OcclusionPortal self=(UnityEngine.OcclusionPortal)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.open=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.OcclusionPortal");
		addMember(l,"open",get_open,set_open,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.OcclusionPortal),typeof(UnityEngine.Component));
	}
}
