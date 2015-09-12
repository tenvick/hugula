using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AnimatorControllerParameter : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter o;
			o=new UnityEngine.AnimatorControllerParameter();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_nameHash(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			pushValue(l,self.nameHash);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_type(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			pushEnum(l,(int)self.type);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_type(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			UnityEngine.AnimatorControllerParameterType v;
			checkEnum(l,2,out v);
			self.type=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultFloat(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			pushValue(l,self.defaultFloat);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultFloat(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.defaultFloat=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultInt(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			pushValue(l,self.defaultInt);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultInt(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.defaultInt=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultBool(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			pushValue(l,self.defaultBool);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultBool(IntPtr l) {
		try {
			UnityEngine.AnimatorControllerParameter self=(UnityEngine.AnimatorControllerParameter)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.defaultBool=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AnimatorControllerParameter");
		addMember(l,"nameHash",get_nameHash,null,true);
		addMember(l,"type",get_type,set_type,true);
		addMember(l,"defaultFloat",get_defaultFloat,set_defaultFloat,true);
		addMember(l,"defaultInt",get_defaultInt,set_defaultInt,true);
		addMember(l,"defaultBool",get_defaultBool,set_defaultBool,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AnimatorControllerParameter));
	}
}
