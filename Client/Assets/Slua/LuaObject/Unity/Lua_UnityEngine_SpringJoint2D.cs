using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_SpringJoint2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D o;
			o=new UnityEngine.SpringJoint2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetReactionForce(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.GetReactionForce(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetReactionTorque(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.GetReactionTorque(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_distance(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			pushValue(l,self.distance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_distance(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.distance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dampingRatio(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			pushValue(l,self.dampingRatio);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_dampingRatio(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.dampingRatio=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_frequency(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			pushValue(l,self.frequency);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_frequency(IntPtr l) {
		try {
			UnityEngine.SpringJoint2D self=(UnityEngine.SpringJoint2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.frequency=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.SpringJoint2D");
		addMember(l,GetReactionForce);
		addMember(l,GetReactionTorque);
		addMember(l,"distance",get_distance,set_distance,true);
		addMember(l,"dampingRatio",get_dampingRatio,set_dampingRatio,true);
		addMember(l,"frequency",get_frequency,set_frequency,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.SpringJoint2D),typeof(UnityEngine.AnchoredJoint2D));
	}
}
