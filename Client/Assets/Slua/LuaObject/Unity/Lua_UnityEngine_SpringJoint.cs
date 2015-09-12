using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_SpringJoint : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.SpringJoint o;
			o=new UnityEngine.SpringJoint();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_spring(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			pushValue(l,self.spring);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_spring(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.spring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_damper(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			pushValue(l,self.damper);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_damper(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.damper=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_minDistance(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			pushValue(l,self.minDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_minDistance(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.minDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxDistance(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			pushValue(l,self.maxDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_maxDistance(IntPtr l) {
		try {
			UnityEngine.SpringJoint self=(UnityEngine.SpringJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.maxDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.SpringJoint");
		addMember(l,"spring",get_spring,set_spring,true);
		addMember(l,"damper",get_damper,set_damper,true);
		addMember(l,"minDistance",get_minDistance,set_minDistance,true);
		addMember(l,"maxDistance",get_maxDistance,set_maxDistance,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.SpringJoint),typeof(UnityEngine.Joint));
	}
}
