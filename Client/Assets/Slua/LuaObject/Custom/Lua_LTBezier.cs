using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LTBezier : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			LTBezier o;
			UnityEngine.Vector3 a1;
			checkType(l,2,out a1);
			UnityEngine.Vector3 a2;
			checkType(l,3,out a2);
			UnityEngine.Vector3 a3;
			checkType(l,4,out a3);
			UnityEngine.Vector3 a4;
			checkType(l,5,out a4);
			System.Single a5;
			checkType(l,6,out a5);
			o=new LTBezier(a1,a2,a3,a4,a5);
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int point(IntPtr l) {
		try {
			LTBezier self=(LTBezier)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.point(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_length(IntPtr l) {
		try {
			LTBezier self=(LTBezier)checkSelf(l);
			pushValue(l,self.length);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_length(IntPtr l) {
		try {
			LTBezier self=(LTBezier)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.length=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LTBezier");
		addMember(l,point);
		addMember(l,"length",get_length,set_length,true);
		createTypeMetatable(l,constructor, typeof(LTBezier));
	}
}
