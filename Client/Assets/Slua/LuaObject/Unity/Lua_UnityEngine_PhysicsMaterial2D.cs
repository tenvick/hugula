using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_PhysicsMaterial2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			UnityEngine.PhysicsMaterial2D o;
			if(argc==1){
				o=new UnityEngine.PhysicsMaterial2D();
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new UnityEngine.PhysicsMaterial2D(a1);
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
	static public int get_bounciness(IntPtr l) {
		try {
			UnityEngine.PhysicsMaterial2D self=(UnityEngine.PhysicsMaterial2D)checkSelf(l);
			pushValue(l,self.bounciness);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bounciness(IntPtr l) {
		try {
			UnityEngine.PhysicsMaterial2D self=(UnityEngine.PhysicsMaterial2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.bounciness=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_friction(IntPtr l) {
		try {
			UnityEngine.PhysicsMaterial2D self=(UnityEngine.PhysicsMaterial2D)checkSelf(l);
			pushValue(l,self.friction);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_friction(IntPtr l) {
		try {
			UnityEngine.PhysicsMaterial2D self=(UnityEngine.PhysicsMaterial2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.friction=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.PhysicsMaterial2D");
		addMember(l,"bounciness",get_bounciness,set_bounciness,true);
		addMember(l,"friction",get_friction,set_friction,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.PhysicsMaterial2D),typeof(UnityEngine.Object));
	}
}
