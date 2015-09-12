using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UGUIEventSystem : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_eventSystem(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			pushValue(l,self.eventSystem);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_eventSystem(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			UnityEngine.EventSystems.EventSystem v;
			checkType(l,2,out v);
			self.eventSystem=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_standaloneInputModule(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			pushValue(l,self.standaloneInputModule);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_standaloneInputModule(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			UnityEngine.EventSystems.StandaloneInputModule v;
			checkType(l,2,out v);
			self.standaloneInputModule=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_touchInputModule(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			pushValue(l,self.touchInputModule);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_touchInputModule(IntPtr l) {
		try {
			UGUIEventSystem self=(UGUIEventSystem)checkSelf(l);
			UnityEngine.EventSystems.TouchInputModule v;
			checkType(l,2,out v);
			self.touchInputModule=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UGUIEventSystem");
		addMember(l,"eventSystem",get_eventSystem,set_eventSystem,true);
		addMember(l,"standaloneInputModule",get_standaloneInputModule,set_standaloneInputModule,true);
		addMember(l,"touchInputModule",get_touchInputModule,set_touchInputModule,true);
		createTypeMetatable(l,null, typeof(UGUIEventSystem),typeof(UnityEngine.MonoBehaviour));
	}
}
