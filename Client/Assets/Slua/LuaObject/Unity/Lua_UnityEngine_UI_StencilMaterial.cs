using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_StencilMaterial : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Add_s(IntPtr l) {
		try {
			UnityEngine.Material a1;
			checkType(l,1,out a1);
			System.Int32 a2;
			checkType(l,2,out a2);
			var ret=UnityEngine.UI.StencilMaterial.Add(a1,a2);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Remove_s(IntPtr l) {
		try {
			UnityEngine.Material a1;
			checkType(l,1,out a1);
			UnityEngine.UI.StencilMaterial.Remove(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.StencilMaterial");
		addMember(l,Add_s);
		addMember(l,Remove_s);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.StencilMaterial));
	}
}
