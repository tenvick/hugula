using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Gyroscope : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotationRate(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.rotationRate);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotationRateUnbiased(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.rotationRateUnbiased);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_gravity(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.gravity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userAcceleration(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.userAcceleration);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_attitude(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.attitude);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enabled(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.enabled);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enabled(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.enabled=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_updateInterval(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			pushValue(l,self.updateInterval);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_updateInterval(IntPtr l) {
		try {
			UnityEngine.Gyroscope self=(UnityEngine.Gyroscope)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.updateInterval=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Gyroscope");
		addMember(l,"rotationRate",get_rotationRate,null,true);
		addMember(l,"rotationRateUnbiased",get_rotationRateUnbiased,null,true);
		addMember(l,"gravity",get_gravity,null,true);
		addMember(l,"userAcceleration",get_userAcceleration,null,true);
		addMember(l,"attitude",get_attitude,null,true);
		addMember(l,"enabled",get_enabled,set_enabled,true);
		addMember(l,"updateInterval",get_updateInterval,set_updateInterval,true);
		createTypeMetatable(l,null, typeof(UnityEngine.Gyroscope));
	}
}
