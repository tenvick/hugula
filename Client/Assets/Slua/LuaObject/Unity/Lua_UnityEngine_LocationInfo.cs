using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_LocationInfo : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.LocationInfo o;
			o=new UnityEngine.LocationInfo();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_latitude(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.latitude);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_longitude(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.longitude);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_altitude(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.altitude);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontalAccuracy(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.horizontalAccuracy);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_verticalAccuracy(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.verticalAccuracy);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_timestamp(IntPtr l) {
		try {
			UnityEngine.LocationInfo self;
			checkValueType(l,1,out self);
			pushValue(l,self.timestamp);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.LocationInfo");
		addMember(l,"latitude",get_latitude,null,true);
		addMember(l,"longitude",get_longitude,null,true);
		addMember(l,"altitude",get_altitude,null,true);
		addMember(l,"horizontalAccuracy",get_horizontalAccuracy,null,true);
		addMember(l,"verticalAccuracy",get_verticalAccuracy,null,true);
		addMember(l,"timestamp",get_timestamp,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.LocationInfo),typeof(System.ValueType));
	}
}
