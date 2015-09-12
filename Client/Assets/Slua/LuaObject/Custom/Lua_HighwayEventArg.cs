using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_HighwayEventArg : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			HighwayEventArg o;
			o=new HighwayEventArg();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_number(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			pushValue(l,self.number);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_number(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.number=v;
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
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
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
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.target=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_total(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			pushValue(l,self.total);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_total(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.total=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_current(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			pushValue(l,self.current);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_current(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.current=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_progress(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			pushValue(l,self.progress);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_progress(IntPtr l) {
		try {
			HighwayEventArg self=(HighwayEventArg)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.progress=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"HighwayEventArg");
		addMember(l,"number",get_number,set_number,true);
		addMember(l,"target",get_target,set_target,true);
		addMember(l,"total",get_total,set_total,true);
		addMember(l,"current",get_current,set_current,true);
		addMember(l,"progress",get_progress,set_progress,true);
		createTypeMetatable(l,constructor, typeof(HighwayEventArg));
	}
}
