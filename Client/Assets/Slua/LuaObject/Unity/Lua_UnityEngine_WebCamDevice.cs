using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_WebCamDevice : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.WebCamDevice o;
			o=new UnityEngine.WebCamDevice();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_name(IntPtr l) {
		try {
			UnityEngine.WebCamDevice self;
			checkValueType(l,1,out self);
			pushValue(l,self.name);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isFrontFacing(IntPtr l) {
		try {
			UnityEngine.WebCamDevice self;
			checkValueType(l,1,out self);
			pushValue(l,self.isFrontFacing);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.WebCamDevice");
		addMember(l,"name",get_name,null,true);
		addMember(l,"isFrontFacing",get_isFrontFacing,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.WebCamDevice),typeof(System.ValueType));
	}
}
