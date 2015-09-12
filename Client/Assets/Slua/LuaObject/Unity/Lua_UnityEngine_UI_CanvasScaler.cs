using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_CanvasScaler : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uiScaleMode(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushEnum(l,(int)self.uiScaleMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_uiScaleMode(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			UnityEngine.UI.CanvasScaler.ScaleMode v;
			checkEnum(l,2,out v);
			self.uiScaleMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_referencePixelsPerUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.referencePixelsPerUnit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_referencePixelsPerUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.referencePixelsPerUnit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_scaleFactor(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.scaleFactor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_scaleFactor(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.scaleFactor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_referenceResolution(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.referenceResolution);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_referenceResolution(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.referenceResolution=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_screenMatchMode(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushEnum(l,(int)self.screenMatchMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_screenMatchMode(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			UnityEngine.UI.CanvasScaler.ScreenMatchMode v;
			checkEnum(l,2,out v);
			self.screenMatchMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_matchWidthOrHeight(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.matchWidthOrHeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_matchWidthOrHeight(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.matchWidthOrHeight=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_physicalUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushEnum(l,(int)self.physicalUnit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_physicalUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			UnityEngine.UI.CanvasScaler.Unit v;
			checkEnum(l,2,out v);
			self.physicalUnit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fallbackScreenDPI(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.fallbackScreenDPI);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fallbackScreenDPI(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.fallbackScreenDPI=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultSpriteDPI(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.defaultSpriteDPI);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultSpriteDPI(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.defaultSpriteDPI=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dynamicPixelsPerUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			pushValue(l,self.dynamicPixelsPerUnit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_dynamicPixelsPerUnit(IntPtr l) {
		try {
			UnityEngine.UI.CanvasScaler self=(UnityEngine.UI.CanvasScaler)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.dynamicPixelsPerUnit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.CanvasScaler");
		addMember(l,"uiScaleMode",get_uiScaleMode,set_uiScaleMode,true);
		addMember(l,"referencePixelsPerUnit",get_referencePixelsPerUnit,set_referencePixelsPerUnit,true);
		addMember(l,"scaleFactor",get_scaleFactor,set_scaleFactor,true);
		addMember(l,"referenceResolution",get_referenceResolution,set_referenceResolution,true);
		addMember(l,"screenMatchMode",get_screenMatchMode,set_screenMatchMode,true);
		addMember(l,"matchWidthOrHeight",get_matchWidthOrHeight,set_matchWidthOrHeight,true);
		addMember(l,"physicalUnit",get_physicalUnit,set_physicalUnit,true);
		addMember(l,"fallbackScreenDPI",get_fallbackScreenDPI,set_fallbackScreenDPI,true);
		addMember(l,"defaultSpriteDPI",get_defaultSpriteDPI,set_defaultSpriteDPI,true);
		addMember(l,"dynamicPixelsPerUnit",get_dynamicPixelsPerUnit,set_dynamicPixelsPerUnit,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.EventSystems.UIBehaviour));
	}
}
