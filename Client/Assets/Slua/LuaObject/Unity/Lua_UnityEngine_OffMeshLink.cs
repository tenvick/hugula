using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_OffMeshLink : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.OffMeshLink o;
			o=new UnityEngine.OffMeshLink();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UpdatePositions(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			self.UpdatePositions();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activated(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.activated);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activated(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.activated=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_occupied(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.occupied);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_costOverride(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.costOverride);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_costOverride(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.costOverride=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_biDirectional(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.biDirectional);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_biDirectional(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.biDirectional=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_area(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.area);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_area(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.area=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_autoUpdatePositions(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.autoUpdatePositions);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_autoUpdatePositions(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.autoUpdatePositions=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_startTransform(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.startTransform);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_startTransform(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			UnityEngine.Transform v;
			checkType(l,2,out v);
			self.startTransform=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_endTransform(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			pushValue(l,self.endTransform);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_endTransform(IntPtr l) {
		try {
			UnityEngine.OffMeshLink self=(UnityEngine.OffMeshLink)checkSelf(l);
			UnityEngine.Transform v;
			checkType(l,2,out v);
			self.endTransform=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.OffMeshLink");
		addMember(l,UpdatePositions);
		addMember(l,"activated",get_activated,set_activated,true);
		addMember(l,"occupied",get_occupied,null,true);
		addMember(l,"costOverride",get_costOverride,set_costOverride,true);
		addMember(l,"biDirectional",get_biDirectional,set_biDirectional,true);
		addMember(l,"area",get_area,set_area,true);
		addMember(l,"autoUpdatePositions",get_autoUpdatePositions,set_autoUpdatePositions,true);
		addMember(l,"startTransform",get_startTransform,set_startTransform,true);
		addMember(l,"endTransform",get_endTransform,set_endTransform,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.OffMeshLink),typeof(UnityEngine.Component));
	}
}
