using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_Shadow : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ModifyVertices(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.UIVertex> a1;
			checkType(l,2,out a1);
			self.ModifyVertices(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_effectColor(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			pushValue(l,self.effectColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_effectColor(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.effectColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_effectDistance(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			pushValue(l,self.effectDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_effectDistance(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.effectDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useGraphicAlpha(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			pushValue(l,self.useGraphicAlpha);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useGraphicAlpha(IntPtr l) {
		try {
			UnityEngine.UI.Shadow self=(UnityEngine.UI.Shadow)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useGraphicAlpha=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.Shadow");
		addMember(l,ModifyVertices);
		addMember(l,"effectColor",get_effectColor,set_effectColor,true);
		addMember(l,"effectDistance",get_effectDistance,set_effectDistance,true);
		addMember(l,"useGraphicAlpha",get_useGraphicAlpha,set_useGraphicAlpha,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.Shadow),typeof(UnityEngine.UI.BaseVertexEffect));
	}
}
