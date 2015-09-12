using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_GridLayoutGroup : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CalculateLayoutInputHorizontal(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			self.CalculateLayoutInputHorizontal();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CalculateLayoutInputVertical(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			self.CalculateLayoutInputVertical();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetLayoutHorizontal(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			self.SetLayoutHorizontal();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetLayoutVertical(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			self.SetLayoutVertical();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_startCorner(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushEnum(l,(int)self.startCorner);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_startCorner(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			UnityEngine.UI.GridLayoutGroup.Corner v;
			checkEnum(l,2,out v);
			self.startCorner=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_startAxis(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushEnum(l,(int)self.startAxis);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_startAxis(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			UnityEngine.UI.GridLayoutGroup.Axis v;
			checkEnum(l,2,out v);
			self.startAxis=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cellSize(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushValue(l,self.cellSize);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cellSize(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.cellSize=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_spacing(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushValue(l,self.spacing);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_spacing(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.spacing=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_constraint(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushEnum(l,(int)self.constraint);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_constraint(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			UnityEngine.UI.GridLayoutGroup.Constraint v;
			checkEnum(l,2,out v);
			self.constraint=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_constraintCount(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			pushValue(l,self.constraintCount);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_constraintCount(IntPtr l) {
		try {
			UnityEngine.UI.GridLayoutGroup self=(UnityEngine.UI.GridLayoutGroup)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.constraintCount=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.GridLayoutGroup");
		addMember(l,CalculateLayoutInputHorizontal);
		addMember(l,CalculateLayoutInputVertical);
		addMember(l,SetLayoutHorizontal);
		addMember(l,SetLayoutVertical);
		addMember(l,"startCorner",get_startCorner,set_startCorner,true);
		addMember(l,"startAxis",get_startAxis,set_startAxis,true);
		addMember(l,"cellSize",get_cellSize,set_cellSize,true);
		addMember(l,"spacing",get_spacing,set_spacing,true);
		addMember(l,"constraint",get_constraint,set_constraint,true);
		addMember(l,"constraintCount",get_constraintCount,set_constraintCount,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.GridLayoutGroup),typeof(UnityEngine.UI.LayoutGroup));
	}
}
