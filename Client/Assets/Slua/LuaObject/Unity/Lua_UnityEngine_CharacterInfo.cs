using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_CharacterInfo : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.CharacterInfo o;
			o=new UnityEngine.CharacterInfo();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_index(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.index);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_index(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.index=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_size(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.size);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_size(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.size=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_style(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushEnum(l,(int)self.style);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_style(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			UnityEngine.FontStyle v;
			checkEnum(l,2,out v);
			self.style=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_advance(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.advance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_glyphWidth(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.glyphWidth);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_glyphHeight(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.glyphHeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bearing(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.bearing);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minY(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.minY);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxY(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.maxY);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minX(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.minX);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxX(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.maxX);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uvBottomLeft(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.uvBottomLeft);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uvBottomRight(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.uvBottomRight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uvTopRight(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.uvTopRight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uvTopLeft(IntPtr l) {
		try {
			UnityEngine.CharacterInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.uvTopLeft);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.CharacterInfo");
		addMember(l,"index",get_index,set_index,true);
		addMember(l,"size",get_size,set_size,true);
		addMember(l,"style",get_style,set_style,true);
		addMember(l,"advance",get_advance,null,true);
		addMember(l,"glyphWidth",get_glyphWidth,null,true);
		addMember(l,"glyphHeight",get_glyphHeight,null,true);
		addMember(l,"bearing",get_bearing,null,true);
		addMember(l,"minY",get_minY,null,true);
		addMember(l,"maxY",get_maxY,null,true);
		addMember(l,"minX",get_minX,null,true);
		addMember(l,"maxX",get_maxX,null,true);
		addMember(l,"uvBottomLeft",get_uvBottomLeft,null,true);
		addMember(l,"uvBottomRight",get_uvBottomRight,null,true);
		addMember(l,"uvTopRight",get_uvTopRight,null,true);
		addMember(l,"uvTopLeft",get_uvTopLeft,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.CharacterInfo),typeof(System.ValueType));
	}
}
