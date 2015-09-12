using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Avatar : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.Avatar o;
			o=new UnityEngine.Avatar();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isValid(IntPtr l) {
		try {
			UnityEngine.Avatar self=(UnityEngine.Avatar)checkSelf(l);
			pushValue(l,self.isValid);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isHuman(IntPtr l) {
		try {
			UnityEngine.Avatar self=(UnityEngine.Avatar)checkSelf(l);
			pushValue(l,self.isHuman);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Avatar");
		addMember(l,"isValid",get_isValid,null,true);
		addMember(l,"isHuman",get_isHuman,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Avatar),typeof(UnityEngine.Object));
	}
}
