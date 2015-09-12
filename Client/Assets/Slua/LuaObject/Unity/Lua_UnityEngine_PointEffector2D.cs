using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_PointEffector2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.PointEffector2D o;
			o=new UnityEngine.PointEffector2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceMagnitude(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushValue(l,self.forceMagnitude);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceMagnitude(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.forceMagnitude=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceVariation(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushValue(l,self.forceVariation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceVariation(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.forceVariation=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_distanceScale(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushValue(l,self.distanceScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_distanceScale(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.distanceScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_drag(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushValue(l,self.drag);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_drag(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.drag=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularDrag(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushValue(l,self.angularDrag);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularDrag(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.angularDrag=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceSource(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushEnum(l,(int)self.forceSource);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceSource(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			UnityEngine.EffectorSelection2D v;
			checkEnum(l,2,out v);
			self.forceSource=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceTarget(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushEnum(l,(int)self.forceTarget);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceTarget(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			UnityEngine.EffectorSelection2D v;
			checkEnum(l,2,out v);
			self.forceTarget=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forceMode(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			pushEnum(l,(int)self.forceMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forceMode(IntPtr l) {
		try {
			UnityEngine.PointEffector2D self=(UnityEngine.PointEffector2D)checkSelf(l);
			UnityEngine.EffectorForceMode2D v;
			checkEnum(l,2,out v);
			self.forceMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.PointEffector2D");
		addMember(l,"forceMagnitude",get_forceMagnitude,set_forceMagnitude,true);
		addMember(l,"forceVariation",get_forceVariation,set_forceVariation,true);
		addMember(l,"distanceScale",get_distanceScale,set_distanceScale,true);
		addMember(l,"drag",get_drag,set_drag,true);
		addMember(l,"angularDrag",get_angularDrag,set_angularDrag,true);
		addMember(l,"forceSource",get_forceSource,set_forceSource,true);
		addMember(l,"forceTarget",get_forceTarget,set_forceTarget,true);
		addMember(l,"forceMode",get_forceMode,set_forceMode,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.PointEffector2D),typeof(UnityEngine.Effector2D));
	}
}
