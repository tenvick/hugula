using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AnimatorTransitionInfo : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo o;
			o=new UnityEngine.AnimatorTransitionInfo();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsName(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.IsName(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsUserName(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.IsUserName(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fullPathHash(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.fullPathHash);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_nameHash(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.nameHash);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userNameHash(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.userNameHash);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalizedTime(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.normalizedTime);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_anyState(IntPtr l) {
		try {
			UnityEngine.AnimatorTransitionInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.anyState);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AnimatorTransitionInfo");
		addMember(l,IsName);
		addMember(l,IsUserName);
		addMember(l,"fullPathHash",get_fullPathHash,null,true);
		addMember(l,"nameHash",get_nameHash,null,true);
		addMember(l,"userNameHash",get_userNameHash,null,true);
		addMember(l,"normalizedTime",get_normalizedTime,null,true);
		addMember(l,"anyState",get_anyState,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AnimatorTransitionInfo),typeof(System.ValueType));
	}
}
