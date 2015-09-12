using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_Slider_SliderEvent : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.UI.Slider.SliderEvent o;
			o=new UnityEngine.UI.Slider.SliderEvent();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		LuaUnityEvent_float.reg(l);
		getTypeTable(l,"UnityEngine.UI.Slider.SliderEvent");
		createTypeMetatable(l,constructor, typeof(UnityEngine.UI.Slider.SliderEvent),typeof(LuaUnityEvent_float));
	}
}
