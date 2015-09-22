using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_MaskableGraphic : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ParentMaskStateChanged(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			self.ParentMaskStateChanged();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetMaterialDirty(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			self.SetMaterialDirty();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maskable(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.maskable);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maskable(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.maskable=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_material(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.material);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_material(IntPtr l) {
		try {
			UnityEngine.UI.MaskableGraphic self=(UnityEngine.UI.MaskableGraphic)checkSelf(l);
			UnityEngine.Material v;
			checkType(l,2,out v);
			self.material=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.MaskableGraphic");
		addMember(l,ParentMaskStateChanged);
		addMember(l,SetMaterialDirty);
		addMember(l,"maskable",get_maskable,set_maskable,true);
		addMember(l,"material",get_material,set_material,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.MaskableGraphic),typeof(UnityEngine.UI.Graphic));
	}
}
