using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_EventSystems_PhysicsRaycaster : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Raycast(IntPtr l) {
		try {
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
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
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
			pushValue(l,self.eventCamera);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_depth(IntPtr l) {
		try {
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
			pushValue(l,self.depth);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_finalEventMask(IntPtr l) {
		try {
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
			pushValue(l,self.finalEventMask);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_eventMask(IntPtr l) {
		try {
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
			pushValue(l,self.eventMask);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_eventMask(IntPtr l) {
		try {
			UnityEngine.EventSystems.PhysicsRaycaster self=(UnityEngine.EventSystems.PhysicsRaycaster)checkSelf(l);
			UnityEngine.LayerMask v;
			checkValueType(l,2,out v);
			self.eventMask=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.EventSystems.PhysicsRaycaster");
		addMember(l,Raycast);
		addMember(l,"eventCamera",get_eventCamera,null,true);
		addMember(l,"depth",get_depth,null,true);
		addMember(l,"finalEventMask",get_finalEventMask,null,true);
		addMember(l,"eventMask",get_eventMask,set_eventMask,true);
		createTypeMetatable(l,null, typeof(UnityEngine.EventSystems.PhysicsRaycaster),typeof(UnityEngine.EventSystems.BaseRaycaster));
	}
}
