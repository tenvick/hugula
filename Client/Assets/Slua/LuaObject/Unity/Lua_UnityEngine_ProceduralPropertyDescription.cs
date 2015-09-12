using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ProceduralPropertyDescription : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription o;
			o=new UnityEngine.ProceduralPropertyDescription();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_name(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.name);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_name(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.name=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_label(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.label);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_label(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.label=v;
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
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
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
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.String v;
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
	static public int get_type(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
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
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			UnityEngine.ProceduralPropertyType v;
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
	static public int get_hasRange(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.hasRange);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_hasRange(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.hasRange=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minimum(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.minimum);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_minimum(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.minimum=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maximum(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.maximum);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maximum(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.maximum=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_step(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.step);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_step(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.step=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enumOptions(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.enumOptions);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enumOptions(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.String[] v;
			checkType(l,2,out v);
			self.enumOptions=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_componentLabels(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			pushValue(l,self.componentLabels);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_componentLabels(IntPtr l) {
		try {
			UnityEngine.ProceduralPropertyDescription self=(UnityEngine.ProceduralPropertyDescription)checkSelf(l);
			System.String[] v;
			checkType(l,2,out v);
			self.componentLabels=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ProceduralPropertyDescription");
		addMember(l,"name",get_name,set_name,true);
		addMember(l,"label",get_label,set_label,true);
		addMember(l,"group",get_group,set_group,true);
		addMember(l,"type",get_type,set_type,true);
		addMember(l,"hasRange",get_hasRange,set_hasRange,true);
		addMember(l,"minimum",get_minimum,set_minimum,true);
		addMember(l,"maximum",get_maximum,set_maximum,true);
		addMember(l,"step",get_step,set_step,true);
		addMember(l,"enumOptions",get_enumOptions,set_enumOptions,true);
		addMember(l,"componentLabels",get_componentLabels,set_componentLabels,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ProceduralPropertyDescription));
	}
}
