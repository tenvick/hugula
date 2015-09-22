using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LHighway : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			LHighway o;
			o=new LHighway();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadLuaTable(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			SLua.LuaTable a1;
			checkType(l,2,out a1);
			self.LoadLuaTable(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onAllCompleteFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onAllCompleteFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onAllCompleteFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onAllCompleteFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onProgressFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onProgressFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onProgressFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onProgressFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onSharedCompleteFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onSharedCompleteFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onSharedCompleteFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onSharedCompleteFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onCacheFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onCacheFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onCacheFn(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onCacheFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cache(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cache);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cache(IntPtr l) {
		try {
			LHighway self=(LHighway)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.cache=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_instance(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,LHighway.instance);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LHighway");
		addMember(l,LoadLuaTable);
		addMember(l,"onAllCompleteFn",get_onAllCompleteFn,set_onAllCompleteFn,true);
		addMember(l,"onProgressFn",get_onProgressFn,set_onProgressFn,true);
		addMember(l,"onSharedCompleteFn",get_onSharedCompleteFn,set_onSharedCompleteFn,true);
		addMember(l,"onCacheFn",get_onCacheFn,set_onCacheFn,true);
		addMember(l,"cache",get_cache,set_cache,true);
		addMember(l,"instance",get_instance,null,false);
		createTypeMetatable(l,constructor, typeof(LHighway),typeof(CHighway));
	}
}
