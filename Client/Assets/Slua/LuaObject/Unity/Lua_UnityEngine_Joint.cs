using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Joint : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.Joint o;
			o=new UnityEngine.Joint();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_connectedBody(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.connectedBody);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_connectedBody(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			UnityEngine.Rigidbody v;
			checkType(l,2,out v);
			self.connectedBody=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_axis(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.axis);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_axis(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.axis=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_anchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.anchor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_anchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.anchor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_connectedAnchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.connectedAnchor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_connectedAnchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.connectedAnchor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_autoConfigureConnectedAnchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.autoConfigureConnectedAnchor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_autoConfigureConnectedAnchor(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.autoConfigureConnectedAnchor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_breakForce(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.breakForce);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_breakForce(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.breakForce=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_breakTorque(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.breakTorque);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_breakTorque(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.breakTorque=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enableCollision(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.enableCollision);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enableCollision(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.enableCollision=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enablePreprocessing(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			pushValue(l,self.enablePreprocessing);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enablePreprocessing(IntPtr l) {
		try {
			UnityEngine.Joint self=(UnityEngine.Joint)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.enablePreprocessing=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Joint");
		addMember(l,"connectedBody",get_connectedBody,set_connectedBody,true);
		addMember(l,"axis",get_axis,set_axis,true);
		addMember(l,"anchor",get_anchor,set_anchor,true);
		addMember(l,"connectedAnchor",get_connectedAnchor,set_connectedAnchor,true);
		addMember(l,"autoConfigureConnectedAnchor",get_autoConfigureConnectedAnchor,set_autoConfigureConnectedAnchor,true);
		addMember(l,"breakForce",get_breakForce,set_breakForce,true);
		addMember(l,"breakTorque",get_breakTorque,set_breakTorque,true);
		addMember(l,"enableCollision",get_enableCollision,set_enableCollision,true);
		addMember(l,"enablePreprocessing",get_enablePreprocessing,set_enablePreprocessing,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Joint),typeof(UnityEngine.Component));
	}
}
