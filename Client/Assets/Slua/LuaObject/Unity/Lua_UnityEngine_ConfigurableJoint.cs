using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ConfigurableJoint : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint o;
			o=new UnityEngine.ConfigurableJoint();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_secondaryAxis(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.secondaryAxis);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_secondaryAxis(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.secondaryAxis=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_xMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.xMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_xMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.xMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_yMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.yMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_yMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.yMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_zMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.zMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_zMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.zMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularXMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.angularXMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularXMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.angularXMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularYMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.angularYMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularYMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.angularYMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularZMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.angularZMotion);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularZMotion(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.ConfigurableJointMotion v;
			checkEnum(l,2,out v);
			self.angularZMotion=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_linearLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.linearLimitSpring);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_linearLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimitSpring v;
			checkValueType(l,2,out v);
			self.linearLimitSpring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularXLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularXLimitSpring);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularXLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimitSpring v;
			checkValueType(l,2,out v);
			self.angularXLimitSpring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularYZLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularYZLimitSpring);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularYZLimitSpring(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimitSpring v;
			checkValueType(l,2,out v);
			self.angularYZLimitSpring=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_linearLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.linearLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_linearLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimit v;
			checkValueType(l,2,out v);
			self.linearLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_lowAngularXLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.lowAngularXLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_lowAngularXLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimit v;
			checkValueType(l,2,out v);
			self.lowAngularXLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_highAngularXLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.highAngularXLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_highAngularXLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimit v;
			checkValueType(l,2,out v);
			self.highAngularXLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularYLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularYLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularYLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimit v;
			checkValueType(l,2,out v);
			self.angularYLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularZLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularZLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularZLimit(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.SoftJointLimit v;
			checkValueType(l,2,out v);
			self.angularZLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_targetPosition(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.targetPosition);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_targetPosition(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.targetPosition=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_targetVelocity(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.targetVelocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_targetVelocity(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.targetVelocity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_xDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.xDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_xDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.xDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_yDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.yDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_yDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.yDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_zDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.zDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_zDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.zDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_targetRotation(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.targetRotation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_targetRotation(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.Quaternion v;
			checkType(l,2,out v);
			self.targetRotation=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_targetAngularVelocity(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.targetAngularVelocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_targetAngularVelocity(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.targetAngularVelocity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotationDriveMode(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.rotationDriveMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rotationDriveMode(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.RotationDriveMode v;
			checkEnum(l,2,out v);
			self.rotationDriveMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularXDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularXDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularXDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.angularXDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_angularYZDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.angularYZDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_angularYZDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.angularYZDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_slerpDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.slerpDrive);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_slerpDrive(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointDrive v;
			checkValueType(l,2,out v);
			self.slerpDrive=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_projectionMode(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushEnum(l,(int)self.projectionMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_projectionMode(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			UnityEngine.JointProjectionMode v;
			checkEnum(l,2,out v);
			self.projectionMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_projectionDistance(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.projectionDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_projectionDistance(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.projectionDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_projectionAngle(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.projectionAngle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_projectionAngle(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.projectionAngle=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_configuredInWorldSpace(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.configuredInWorldSpace);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_configuredInWorldSpace(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.configuredInWorldSpace=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_swapBodies(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			pushValue(l,self.swapBodies);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_swapBodies(IntPtr l) {
		try {
			UnityEngine.ConfigurableJoint self=(UnityEngine.ConfigurableJoint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.swapBodies=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ConfigurableJoint");
		addMember(l,"secondaryAxis",get_secondaryAxis,set_secondaryAxis,true);
		addMember(l,"xMotion",get_xMotion,set_xMotion,true);
		addMember(l,"yMotion",get_yMotion,set_yMotion,true);
		addMember(l,"zMotion",get_zMotion,set_zMotion,true);
		addMember(l,"angularXMotion",get_angularXMotion,set_angularXMotion,true);
		addMember(l,"angularYMotion",get_angularYMotion,set_angularYMotion,true);
		addMember(l,"angularZMotion",get_angularZMotion,set_angularZMotion,true);
		addMember(l,"linearLimitSpring",get_linearLimitSpring,set_linearLimitSpring,true);
		addMember(l,"angularXLimitSpring",get_angularXLimitSpring,set_angularXLimitSpring,true);
		addMember(l,"angularYZLimitSpring",get_angularYZLimitSpring,set_angularYZLimitSpring,true);
		addMember(l,"linearLimit",get_linearLimit,set_linearLimit,true);
		addMember(l,"lowAngularXLimit",get_lowAngularXLimit,set_lowAngularXLimit,true);
		addMember(l,"highAngularXLimit",get_highAngularXLimit,set_highAngularXLimit,true);
		addMember(l,"angularYLimit",get_angularYLimit,set_angularYLimit,true);
		addMember(l,"angularZLimit",get_angularZLimit,set_angularZLimit,true);
		addMember(l,"targetPosition",get_targetPosition,set_targetPosition,true);
		addMember(l,"targetVelocity",get_targetVelocity,set_targetVelocity,true);
		addMember(l,"xDrive",get_xDrive,set_xDrive,true);
		addMember(l,"yDrive",get_yDrive,set_yDrive,true);
		addMember(l,"zDrive",get_zDrive,set_zDrive,true);
		addMember(l,"targetRotation",get_targetRotation,set_targetRotation,true);
		addMember(l,"targetAngularVelocity",get_targetAngularVelocity,set_targetAngularVelocity,true);
		addMember(l,"rotationDriveMode",get_rotationDriveMode,set_rotationDriveMode,true);
		addMember(l,"angularXDrive",get_angularXDrive,set_angularXDrive,true);
		addMember(l,"angularYZDrive",get_angularYZDrive,set_angularYZDrive,true);
		addMember(l,"slerpDrive",get_slerpDrive,set_slerpDrive,true);
		addMember(l,"projectionMode",get_projectionMode,set_projectionMode,true);
		addMember(l,"projectionDistance",get_projectionDistance,set_projectionDistance,true);
		addMember(l,"projectionAngle",get_projectionAngle,set_projectionAngle,true);
		addMember(l,"configuredInWorldSpace",get_configuredInWorldSpace,set_configuredInWorldSpace,true);
		addMember(l,"swapBodies",get_swapBodies,set_swapBodies,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ConfigurableJoint),typeof(UnityEngine.Joint));
	}
}
