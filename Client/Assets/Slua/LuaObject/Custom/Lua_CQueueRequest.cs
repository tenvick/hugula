using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_CQueueRequest : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			CQueueRequest o;
			o=new CQueueRequest();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Add(IntPtr l) {
		try {
			CQueueRequest self=(CQueueRequest)checkSelf(l);
			CRequest a1;
			checkType(l,2,out a1);
			self.Add(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int First(IntPtr l) {
		try {
			CQueueRequest self=(CQueueRequest)checkSelf(l);
			var ret=self.First();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Size(IntPtr l) {
		try {
			CQueueRequest self=(CQueueRequest)checkSelf(l);
			var ret=self.Size();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"CQueueRequest");
		addMember(l,Add);
		addMember(l,First);
		addMember(l,Size);
		createTypeMetatable(l,constructor, typeof(CQueueRequest));
	}
}
