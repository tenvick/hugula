using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AsyncOperation : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AsyncOperation o;
			o=new UnityEngine.AsyncOperation();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isDone(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			pushValue(l,self.isDone);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_progress(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			pushValue(l,self.progress);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_priority(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			pushValue(l,self.priority);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_priority(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.priority=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_allowSceneActivation(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			pushValue(l,self.allowSceneActivation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_allowSceneActivation(IntPtr l) {
		try {
			UnityEngine.AsyncOperation self=(UnityEngine.AsyncOperation)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.allowSceneActivation=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AsyncOperation");
		addMember(l,"isDone",get_isDone,null,true);
		addMember(l,"progress",get_progress,null,true);
		addMember(l,"priority",get_priority,set_priority,true);
		addMember(l,"allowSceneActivation",get_allowSceneActivation,set_allowSceneActivation,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AsyncOperation),typeof(UnityEngine.YieldInstruction));
	}
}
