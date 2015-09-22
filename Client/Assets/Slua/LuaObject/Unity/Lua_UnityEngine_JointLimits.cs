using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_JointLimits : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.JointLimits o;
			o=new UnityEngine.JointLimits();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_min(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.min);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_min(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.min=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minBounce(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.minBounce);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_minBounce(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.minBounce=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_max(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.max);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_max(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.max=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxBounce(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.maxBounce);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maxBounce(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.maxBounce=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_contactDistance(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			pushValue(l,true);
			pushValue(l,self.contactDistance);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_contactDistance(IntPtr l) {
		try {
			UnityEngine.JointLimits self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.contactDistance=v;
			setBack(l,self);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.JointLimits");
		addMember(l,"min",get_min,set_min,true);
		addMember(l,"minBounce",get_minBounce,set_minBounce,true);
		addMember(l,"max",get_max,set_max,true);
		addMember(l,"maxBounce",get_maxBounce,set_maxBounce,true);
		addMember(l,"contactDistance",get_contactDistance,set_contactDistance,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.JointLimits),typeof(System.ValueType));
	}
}
