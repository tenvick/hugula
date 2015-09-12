using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_MatchTargetWeightMask : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.MatchTargetWeightMask o;
			UnityEngine.Vector3 a1;
			checkType(l,2,out a1);
			System.Single a2;
			checkType(l,3,out a2);
			o=new UnityEngine.MatchTargetWeightMask(a1,a2);
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_positionXYZWeight(IntPtr l) {
		try {
			UnityEngine.MatchTargetWeightMask self;
			checkValueType(l,1,out self);
			pushValue(l,self.positionXYZWeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_positionXYZWeight(IntPtr l) {
		try {
			UnityEngine.MatchTargetWeightMask self;
			checkValueType(l,1,out self);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.positionXYZWeight=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotationWeight(IntPtr l) {
		try {
			UnityEngine.MatchTargetWeightMask self;
			checkValueType(l,1,out self);
			pushValue(l,self.rotationWeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rotationWeight(IntPtr l) {
		try {
			UnityEngine.MatchTargetWeightMask self;
			checkValueType(l,1,out self);
			float v;
			checkType(l,2,out v);
			self.rotationWeight=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.MatchTargetWeightMask");
		addMember(l,"positionXYZWeight",get_positionXYZWeight,set_positionXYZWeight,true);
		addMember(l,"rotationWeight",get_rotationWeight,set_rotationWeight,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.MatchTargetWeightMask),typeof(System.ValueType));
	}
}
