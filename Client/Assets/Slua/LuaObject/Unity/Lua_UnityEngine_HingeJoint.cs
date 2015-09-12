using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_HingeJoint : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.HingeJoint o;
			o=new UnityEngine.HingeJoint();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_motor(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.motor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_motor(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			UnityEngine.JointMotor v;
			checkValueType(l,2,out v);
			self.motor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_limits(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.limits);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_limits(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			UnityEngine.JointLimits v;
			checkValueType(l,2,out v);
			self.limits=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_spring(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
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
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			UnityEngine.JointSpring v;
			checkValueType(l,2,out v);
			self.spring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useMotor(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.useMotor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useMotor(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useMotor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useLimits(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.useLimits);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useLimits(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useLimits=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useSpring(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.useSpring);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useSpring(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useSpring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_velocity(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.velocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angle(IntPtr l) {
		try {
			UnityEngine.HingeJoint self=(UnityEngine.HingeJoint)checkSelf(l);
			pushValue(l,self.angle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.HingeJoint");
		addMember(l,"motor",get_motor,set_motor,true);
		addMember(l,"limits",get_limits,set_limits,true);
		addMember(l,"spring",get_spring,set_spring,true);
		addMember(l,"useMotor",get_useMotor,set_useMotor,true);
		addMember(l,"useLimits",get_useLimits,set_useLimits,true);
		addMember(l,"useSpring",get_useSpring,set_useSpring,true);
		addMember(l,"velocity",get_velocity,null,true);
		addMember(l,"angle",get_angle,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.HingeJoint),typeof(UnityEngine.Joint));
	}
}
