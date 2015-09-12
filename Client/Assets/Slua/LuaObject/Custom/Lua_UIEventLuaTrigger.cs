using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UIEventLuaTrigger : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnLuaTrigger(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			self.OnLuaTrigger();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_luaFn(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			pushValue(l,self.luaFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_luaFn(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.luaFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_trigger(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			pushValue(l,self.trigger);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_trigger(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			UnityEngine.MonoBehaviour v;
			checkType(l,2,out v);
			self.trigger=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_target(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			pushValue(l,self.target);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_target(IntPtr l) {
		try {
			UIEventLuaTrigger self=(UIEventLuaTrigger)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.MonoBehaviour> v;
			checkType(l,2,out v);
			self.target=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIEventLuaTrigger");
		addMember(l,OnLuaTrigger);
		addMember(l,"luaFn",get_luaFn,set_luaFn,true);
		addMember(l,"trigger",get_trigger,set_trigger,true);
		addMember(l,"target",get_target,set_target,true);
		createTypeMetatable(l,null, typeof(UIEventLuaTrigger),typeof(UnityEngine.MonoBehaviour));
	}
}
