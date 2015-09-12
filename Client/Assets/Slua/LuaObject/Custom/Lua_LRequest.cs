using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LRequest : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			LRequest o;
			if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new LRequest(a1);
				pushValue(l,o);
				return 1;
			}
			else if(argc==4){
				System.String a1;
				checkType(l,2,out a1);
				System.String a2;
				checkType(l,3,out a2);
				System.String a3;
				checkType(l,4,out a3);
				o=new LRequest(a1,a2,a3);
				pushValue(l,o);
				return 1;
			}
			LuaDLL.luaL_error(l,"New object failed.");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onCompleteFn(IntPtr l) {
		try {
			LRequest self=(LRequest)checkSelf(l);
			pushValue(l,self.onCompleteFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onCompleteFn(IntPtr l) {
		try {
			LRequest self=(LRequest)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onCompleteFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onEndFn(IntPtr l) {
		try {
			LRequest self=(LRequest)checkSelf(l);
			pushValue(l,self.onEndFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onEndFn(IntPtr l) {
		try {
			LRequest self=(LRequest)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onEndFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LRequest");
		addMember(l,"onCompleteFn",get_onCompleteFn,set_onCompleteFn,true);
		addMember(l,"onEndFn",get_onEndFn,set_onEndFn,true);
		createTypeMetatable(l,constructor, typeof(LRequest),typeof(CRequest));
	}
}
