using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_Toggle : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Rebuild(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.UI.CanvasUpdate a1;
			checkEnum(l,2,out a1);
			self.Rebuild(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnPointerClick(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnPointerClick(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnSubmit(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.EventSystems.BaseEventData a1;
			checkType(l,2,out a1);
			self.OnSubmit(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_toggleTransition(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			pushEnum(l,(int)self.toggleTransition);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_toggleTransition(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.UI.Toggle.ToggleTransition v;
			checkEnum(l,2,out v);
			self.toggleTransition=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_graphic(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			pushValue(l,self.graphic);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_graphic(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.UI.Graphic v;
			checkType(l,2,out v);
			self.graphic=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onValueChanged(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			pushValue(l,self.onValueChanged);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onValueChanged(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.UI.Toggle.ToggleEvent v;
			checkType(l,2,out v);
			self.onValueChanged=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_group(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			pushValue(l,self.group);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_group(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			UnityEngine.UI.ToggleGroup v;
			checkType(l,2,out v);
			self.group=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isOn(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			pushValue(l,self.isOn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isOn(IntPtr l) {
		try {
			UnityEngine.UI.Toggle self=(UnityEngine.UI.Toggle)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isOn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.Toggle");
		addMember(l,Rebuild);
		addMember(l,OnPointerClick);
		addMember(l,OnSubmit);
		addMember(l,"toggleTransition",get_toggleTransition,set_toggleTransition,true);
		addMember(l,"graphic",get_graphic,set_graphic,true);
		addMember(l,"onValueChanged",get_onValueChanged,set_onValueChanged,true);
		addMember(l,"group",get_group,set_group,true);
		addMember(l,"isOn",get_isOn,set_isOn,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.Toggle),typeof(UnityEngine.UI.Selectable));
	}
}
