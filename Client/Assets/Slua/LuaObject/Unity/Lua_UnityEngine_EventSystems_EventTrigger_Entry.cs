using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_EventSystems_EventTrigger_Entry : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.EventSystems.EventTrigger.Entry o;
			o=new UnityEngine.EventSystems.EventTrigger.Entry();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_eventID(IntPtr l) {
		try {
			UnityEngine.EventSystems.EventTrigger.Entry self=(UnityEngine.EventSystems.EventTrigger.Entry)checkSelf(l);
			pushEnum(l,(int)self.eventID);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_eventID(IntPtr l) {
		try {
			UnityEngine.EventSystems.EventTrigger.Entry self=(UnityEngine.EventSystems.EventTrigger.Entry)checkSelf(l);
			UnityEngine.EventSystems.EventTriggerType v;
			checkEnum(l,2,out v);
			self.eventID=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_callback(IntPtr l) {
		try {
			UnityEngine.EventSystems.EventTrigger.Entry self=(UnityEngine.EventSystems.EventTrigger.Entry)checkSelf(l);
			pushValue(l,self.callback);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_callback(IntPtr l) {
		try {
			UnityEngine.EventSystems.EventTrigger.Entry self=(UnityEngine.EventSystems.EventTrigger.Entry)checkSelf(l);
			UnityEngine.EventSystems.EventTrigger.TriggerEvent v;
			checkType(l,2,out v);
			self.callback=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.EventSystems.EventTrigger.Entry");
		addMember(l,"eventID",get_eventID,set_eventID,true);
		addMember(l,"callback",get_callback,set_callback,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.EventSystems.EventTrigger.Entry));
	}
}
