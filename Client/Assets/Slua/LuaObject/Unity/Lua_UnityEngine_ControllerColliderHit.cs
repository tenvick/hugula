using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ControllerColliderHit : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit o;
			o=new UnityEngine.ControllerColliderHit();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_controller(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.controller);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_collider(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.collider);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rigidbody(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.rigidbody);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_gameObject(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.gameObject);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_transform(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.transform);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_point(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.point);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normal(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.normal);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_moveDirection(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.moveDirection);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_moveLength(IntPtr l) {
		try {
			UnityEngine.ControllerColliderHit self=(UnityEngine.ControllerColliderHit)checkSelf(l);
			pushValue(l,self.moveLength);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ControllerColliderHit");
		addMember(l,"controller",get_controller,null,true);
		addMember(l,"collider",get_collider,null,true);
		addMember(l,"rigidbody",get_rigidbody,null,true);
		addMember(l,"gameObject",get_gameObject,null,true);
		addMember(l,"transform",get_transform,null,true);
		addMember(l,"point",get_point,null,true);
		addMember(l,"normal",get_normal,null,true);
		addMember(l,"moveDirection",get_moveDirection,null,true);
		addMember(l,"moveLength",get_moveLength,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ControllerColliderHit));
	}
}
