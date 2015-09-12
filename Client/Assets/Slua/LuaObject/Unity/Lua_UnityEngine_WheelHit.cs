using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_WheelHit : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.WheelHit o;
			o=new UnityEngine.WheelHit();
			pushValue(l,o);
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
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.collider);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_collider(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			UnityEngine.Collider v;
			checkType(l,2,out v);
			self.collider=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_point(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.point);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_point(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.point=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normal(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.normal);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_normal(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.normal=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forwardDir(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.forwardDir);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forwardDir(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.forwardDir=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sidewaysDir(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.sidewaysDir);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sidewaysDir(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.sidewaysDir=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_force(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
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
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.force=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_forwardSlip(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.forwardSlip);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_forwardSlip(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.forwardSlip=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sidewaysSlip(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			pushValue(l,self.sidewaysSlip);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sidewaysSlip(IntPtr l) {
		try {
			UnityEngine.WheelHit self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.sidewaysSlip=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.WheelHit");
		addMember(l,"collider",get_collider,set_collider,true);
		addMember(l,"point",get_point,set_point,true);
		addMember(l,"normal",get_normal,set_normal,true);
		addMember(l,"forwardDir",get_forwardDir,set_forwardDir,true);
		addMember(l,"sidewaysDir",get_sidewaysDir,set_sidewaysDir,true);
		addMember(l,"force",get_force,set_force,true);
		addMember(l,"forwardSlip",get_forwardSlip,set_forwardSlip,true);
		addMember(l,"sidewaysSlip",get_sidewaysSlip,set_sidewaysSlip,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.WheelHit),typeof(System.ValueType));
	}
}
