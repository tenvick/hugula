using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_SurfaceEffector2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D o;
			o=new UnityEngine.SurfaceEffector2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_speed(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.speed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_speed(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.speed=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_speedVariation(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.speedVariation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_speedVariation(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.speedVariation=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceScale(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.forceScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceScale(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.forceScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useContactForce(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.useContactForce);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useContactForce(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useContactForce=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useFriction(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.useFriction);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useFriction(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useFriction=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useBounce(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			pushValue(l,self.useBounce);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useBounce(IntPtr l) {
		try {
			UnityEngine.SurfaceEffector2D self=(UnityEngine.SurfaceEffector2D)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useBounce=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.SurfaceEffector2D");
		addMember(l,"speed",get_speed,set_speed,true);
		addMember(l,"speedVariation",get_speedVariation,set_speedVariation,true);
		addMember(l,"forceScale",get_forceScale,set_forceScale,true);
		addMember(l,"useContactForce",get_useContactForce,set_useContactForce,true);
		addMember(l,"useFriction",get_useFriction,set_useFriction,true);
		addMember(l,"useBounce",get_useBounce,set_useBounce,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.SurfaceEffector2D),typeof(UnityEngine.Effector2D));
	}
}
