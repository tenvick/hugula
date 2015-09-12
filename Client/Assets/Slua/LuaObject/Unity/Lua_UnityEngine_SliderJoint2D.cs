using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_SliderJoint2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D o;
			o=new UnityEngine.SliderJoint2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetMotorForce(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.GetMotorForce(a1);
			pushValue(l,ret);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			pushValue(l,self.angle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angle(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.angle=v;
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
	static public int get_motor(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			UnityEngine.JointMotor2D v;
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
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
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			UnityEngine.JointTranslationLimits2D v;
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
	static public int get_limitState(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			pushEnum(l,(int)self.limitState);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_referenceAngle(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			pushValue(l,self.referenceAngle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_jointTranslation(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			pushValue(l,self.jointTranslation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_jointSpeed(IntPtr l) {
		try {
			UnityEngine.SliderJoint2D self=(UnityEngine.SliderJoint2D)checkSelf(l);
			pushValue(l,self.jointSpeed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.SliderJoint2D");
		addMember(l,GetMotorForce);
		addMember(l,"angle",get_angle,set_angle,true);
		addMember(l,"useMotor",get_useMotor,set_useMotor,true);
		addMember(l,"useLimits",get_useLimits,set_useLimits,true);
		addMember(l,"motor",get_motor,set_motor,true);
		addMember(l,"limits",get_limits,set_limits,true);
		addMember(l,"limitState",get_limitState,null,true);
		addMember(l,"referenceAngle",get_referenceAngle,null,true);
		addMember(l,"jointTranslation",get_jointTranslation,null,true);
		addMember(l,"jointSpeed",get_jointSpeed,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.SliderJoint2D),typeof(UnityEngine.AnchoredJoint2D));
	}
}
