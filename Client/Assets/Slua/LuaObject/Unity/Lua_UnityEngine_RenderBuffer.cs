using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_RenderBuffer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.RenderBuffer o;
			o=new UnityEngine.RenderBuffer();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetNativeRenderBufferPtr(IntPtr l) {
		try {
			UnityEngine.RenderBuffer self;
			checkValueType(l,1,out self);
			var ret=self.GetNativeRenderBufferPtr();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.RenderBuffer");
		addMember(l,GetNativeRenderBufferPtr);
		createTypeMetatable(l,constructor, typeof(UnityEngine.RenderBuffer),typeof(System.ValueType));
	}
}
