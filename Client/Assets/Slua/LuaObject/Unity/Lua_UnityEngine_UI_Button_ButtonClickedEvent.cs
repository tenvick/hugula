using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_Button_ButtonClickedEvent : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.UI.Button.ButtonClickedEvent o;
			o=new UnityEngine.UI.Button.ButtonClickedEvent();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.Button.ButtonClickedEvent");
		createTypeMetatable(l,constructor, typeof(UnityEngine.UI.Button.ButtonClickedEvent),typeof(UnityEngine.Events.UnityEvent));
	}
}
