using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_EventSystems_BaseRaycaster : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Raycast(IntPtr l) {
		try {
			UnityEngine.EventSystems.BaseRaycaster self=(UnityEngine.EventSystems.BaseRaycaster)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> a2;
			checkType(l,3,out a2);
			self.Raycast(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_eventCamera(IntPtr l) {
		try {
			UnityEngine.EventSystems.BaseRaycaster self=(UnityEngine.EventSystems.BaseRaycaster)checkSelf(l);
			pushValue(l,self.eventCamera);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sortOrderPriority(IntPtr l) {
		try {
			UnityEngine.EventSystems.BaseRaycaster self=(UnityEngine.EventSystems.BaseRaycaster)checkSelf(l);
			pushValue(l,self.sortOrderPriority);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_renderOrderPriority(IntPtr l) {
		try {
			UnityEngine.EventSystems.BaseRaycaster self=(UnityEngine.EventSystems.BaseRaycaster)checkSelf(l);
			pushValue(l,self.renderOrderPriority);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.EventSystems.BaseRaycaster");
		addMember(l,Raycast);
		addMember(l,"eventCamera",get_eventCamera,null,true);
		addMember(l,"sortOrderPriority",get_sortOrderPriority,null,true);
		addMember(l,"renderOrderPriority",get_renderOrderPriority,null,true);
		createTypeMetatable(l,null, typeof(UnityEngine.EventSystems.BaseRaycaster),typeof(UnityEngine.EventSystems.UIBehaviour));
	}
}
