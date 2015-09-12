using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ConstantForce2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D o;
			o=new UnityEngine.ConstantForce2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_force(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			pushValue(l,self.force);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_force(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.force=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_relativeForce(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			pushValue(l,self.relativeForce);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_relativeForce(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.relativeForce=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_torque(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			pushValue(l,self.torque);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_torque(IntPtr l) {
		try {
			UnityEngine.ConstantForce2D self=(UnityEngine.ConstantForce2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.torque=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ConstantForce2D");
		addMember(l,"force",get_force,set_force,true);
		addMember(l,"relativeForce",get_relativeForce,set_relativeForce,true);
		addMember(l,"torque",get_torque,set_torque,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ConstantForce2D),typeof(UnityEngine.PhysicsUpdateBehaviour2D));
	}
}
