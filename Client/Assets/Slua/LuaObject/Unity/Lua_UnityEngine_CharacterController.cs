using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_CharacterController : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.CharacterController o;
			o=new UnityEngine.CharacterController();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SimpleMove(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			UnityEngine.Vector3 a1;
			checkType(l,2,out a1);
			var ret=self.SimpleMove(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Move(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			UnityEngine.Vector3 a1;
			checkType(l,2,out a1);
			var ret=self.Move(a1);
			pushEnum(l,(int)ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isGrounded(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.isGrounded);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_velocity(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.velocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_collisionFlags(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushEnum(l,(int)self.collisionFlags);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_radius(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.radius);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_radius(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.radius=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_height(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.height);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_height(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.height=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_center(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.center);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_center(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.center=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_slopeLimit(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.slopeLimit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_slopeLimit(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.slopeLimit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_stepOffset(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.stepOffset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_stepOffset(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.stepOffset=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_detectCollisions(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			pushValue(l,self.detectCollisions);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_detectCollisions(IntPtr l) {
		try {
			UnityEngine.CharacterController self=(UnityEngine.CharacterController)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.detectCollisions=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.CharacterController");
		addMember(l,SimpleMove);
		addMember(l,Move);
		addMember(l,"isGrounded",get_isGrounded,null,true);
		addMember(l,"velocity",get_velocity,null,true);
		addMember(l,"collisionFlags",get_collisionFlags,null,true);
		addMember(l,"radius",get_radius,set_radius,true);
		addMember(l,"height",get_height,set_height,true);
		addMember(l,"center",get_center,set_center,true);
		addMember(l,"slopeLimit",get_slopeLimit,set_slopeLimit,true);
		addMember(l,"stepOffset",get_stepOffset,set_stepOffset,true);
		addMember(l,"detectCollisions",get_detectCollisions,set_detectCollisions,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.CharacterController),typeof(UnityEngine.Collider));
	}
}
