using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Collider2D : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.Collider2D o;
			o=new UnityEngine.Collider2D();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OverlapPoint(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			UnityEngine.Vector2 a1;
			checkType(l,2,out a1);
			var ret=self.OverlapPoint(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsTouching(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			UnityEngine.Collider2D a1;
			checkType(l,2,out a1);
			var ret=self.IsTouching(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsTouchingLayers(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
				var ret=self.IsTouchingLayers();
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
				System.Int32 a1;
				checkType(l,2,out a1);
				var ret=self.IsTouchingLayers(a1);
				pushValue(l,ret);
				return 1;
			}
			LuaDLL.luaL_error(l,"No matched override function to call");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isTrigger(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.isTrigger);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isTrigger(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isTrigger=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_usedByEffector(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.usedByEffector);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_usedByEffector(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.usedByEffector=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_offset(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.offset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_offset(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.offset=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_attachedRigidbody(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.attachedRigidbody);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_shapeCount(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.shapeCount);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bounds(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.bounds);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sharedMaterial(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			pushValue(l,self.sharedMaterial);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sharedMaterial(IntPtr l) {
		try {
			UnityEngine.Collider2D self=(UnityEngine.Collider2D)checkSelf(l);
			UnityEngine.PhysicsMaterial2D v;
			checkType(l,2,out v);
			self.sharedMaterial=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Collider2D");
		addMember(l,OverlapPoint);
		addMember(l,IsTouching);
		addMember(l,IsTouchingLayers);
		addMember(l,"isTrigger",get_isTrigger,set_isTrigger,true);
		addMember(l,"usedByEffector",get_usedByEffector,set_usedByEffector,true);
		addMember(l,"offset",get_offset,set_offset,true);
		addMember(l,"attachedRigidbody",get_attachedRigidbody,null,true);
		addMember(l,"shapeCount",get_shapeCount,null,true);
		addMember(l,"bounds",get_bounds,null,true);
		addMember(l,"sharedMaterial",get_sharedMaterial,set_sharedMaterial,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Collider2D),typeof(UnityEngine.Behaviour));
	}
}
