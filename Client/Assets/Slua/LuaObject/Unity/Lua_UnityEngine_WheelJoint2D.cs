using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_WheelJoint2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D o;
			o=new UnityEngine.WheelJoint2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetMotorTorque(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.GetMotorTorque(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_suspension(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
			pushValue(l,self.suspension);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_suspension(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
			UnityEngine.JointSuspension2D v;
			checkValueType(l,2,out v);
			self.suspension=v;
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
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
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
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
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
	static public int get_motor(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
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
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
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
	static public int get_jointTranslation(IntPtr l) {
		try {
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
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
			UnityEngine.WheelJoint2D self=(UnityEngine.WheelJoint2D)checkSelf(l);
			pushValue(l,self.jointSpeed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.WheelJoint2D");
		addMember(l,GetMotorTorque);
		addMember(l,"suspension",get_suspension,set_suspension,true);
		addMember(l,"useMotor",get_useMotor,set_useMotor,true);
		addMember(l,"motor",get_motor,set_motor,true);
		addMember(l,"jointTranslation",get_jointTranslation,null,true);
		addMember(l,"jointSpeed",get_jointSpeed,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.WheelJoint2D),typeof(UnityEngine.AnchoredJoint2D));
	}
}
