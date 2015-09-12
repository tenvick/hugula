using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_JointMotor : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.JointMotor o;
			o=new UnityEngine.JointMotor();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_targetVelocity(IntPtr l) {
		try {
			UnityEngine.JointMotor self;
			checkValueType(l,1,out self);
			pushValue(l,self.targetVelocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_targetVelocity(IntPtr l) {
		try {
			UnityEngine.JointMotor self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.targetVelocity=v;
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
			UnityEngine.JointMotor self;
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
			UnityEngine.JointMotor self;
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
	static public int get_freeSpin(IntPtr l) {
		try {
			UnityEngine.JointMotor self;
			checkValueType(l,1,out self);
			pushValue(l,self.freeSpin);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_freeSpin(IntPtr l) {
		try {
			UnityEngine.JointMotor self;
			checkValueType(l,1,out self);
			bool v;
			checkType(l,2,out v);
			self.freeSpin=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.JointMotor");
		addMember(l,"targetVelocity",get_targetVelocity,set_targetVelocity,true);
		addMember(l,"force",get_force,set_force,true);
		addMember(l,"freeSpin",get_freeSpin,set_freeSpin,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.JointMotor),typeof(System.ValueType));
	}
}
