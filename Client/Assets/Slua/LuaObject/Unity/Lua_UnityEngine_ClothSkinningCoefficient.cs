using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ClothSkinningCoefficient : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ClothSkinningCoefficient o;
			o=new UnityEngine.ClothSkinningCoefficient();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxDistance(IntPtr l) {
		try {
			UnityEngine.ClothSkinningCoefficient self;
			checkValueType(l,1,out self);
			pushValue(l,self.maxDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maxDistance(IntPtr l) {
		try {
			UnityEngine.ClothSkinningCoefficient self;
			checkValueType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.maxDistance=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_collisionSphereDistance(IntPtr l) {
		try {
			UnityEngine.ClothSkinningCoefficient self;
			checkValueType(l,1,out self);
			pushValue(l,self.collisionSphereDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_collisionSphereDistance(IntPtr l) {
		try {
			UnityEngine.ClothSkinningCoefficient self;
			checkValueType(l,1,out self);
			System.Single v;
			checkType(l,2,out v);
			self.collisionSphereDistance=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ClothSkinningCoefficient");
		addMember(l,"maxDistance",get_maxDistance,set_maxDistance,true);
		addMember(l,"collisionSphereDistance",get_collisionSphereDistance,set_collisionSphereDistance,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ClothSkinningCoefficient),typeof(System.ValueType));
	}
}
