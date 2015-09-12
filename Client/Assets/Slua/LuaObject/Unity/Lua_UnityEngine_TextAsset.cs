using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_TextAsset : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.TextAsset o;
			o=new UnityEngine.TextAsset();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_text(IntPtr l) {
		try {
			UnityEngine.TextAsset self=(UnityEngine.TextAsset)checkSelf(l);
			pushValue(l,self.text);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bytes(IntPtr l) {
		try {
			UnityEngine.TextAsset self=(UnityEngine.TextAsset)checkSelf(l);
			pushValue(l,self.bytes);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.TextAsset");
		addMember(l,"text",get_text,null,true);
		addMember(l,"bytes",get_bytes,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.TextAsset),typeof(UnityEngine.Object));
	}
}
