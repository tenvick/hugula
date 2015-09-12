using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_HumanBone : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.HumanBone o;
			o=new UnityEngine.HumanBone();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_limit(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			pushValue(l,self.limit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_limit(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			UnityEngine.HumanLimit v;
			checkValueType(l,2,out v);
			self.limit=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_boneName(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			pushValue(l,self.boneName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_boneName(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			string v;
			checkType(l,2,out v);
			self.boneName=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_humanName(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			pushValue(l,self.humanName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_humanName(IntPtr l) {
		try {
			UnityEngine.HumanBone self;
			checkValueType(l,1,out self);
			string v;
			checkType(l,2,out v);
			self.humanName=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.HumanBone");
		addMember(l,"limit",get_limit,set_limit,true);
		addMember(l,"boneName",get_boneName,set_boneName,true);
		addMember(l,"humanName",get_humanName,set_humanName,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.HumanBone),typeof(System.ValueType));
	}
}
