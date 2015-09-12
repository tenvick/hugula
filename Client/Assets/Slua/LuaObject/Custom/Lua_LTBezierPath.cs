using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LTBezierPath : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			LTBezierPath o;
			if(argc==1){
				o=new LTBezierPath();
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				UnityEngine.Vector3[] a1;
				checkType(l,2,out a1);
				o=new LTBezierPath(a1);
				pushValue(l,o);
				return 1;
			}
			LuaDLL.luaL_error(l,"New object failed.");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setPoints(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			UnityEngine.Vector3[] a1;
			checkType(l,2,out a1);
			self.setPoints(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int point(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.point(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int place2d(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			UnityEngine.Transform a1;
			checkType(l,2,out a1);
			System.Single a2;
			checkType(l,3,out a2);
			self.place2d(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int placeLocal2d(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			UnityEngine.Transform a1;
			checkType(l,2,out a1);
			System.Single a2;
			checkType(l,3,out a2);
			self.placeLocal2d(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int place(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==3){
				LTBezierPath self=(LTBezierPath)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				self.place(a1,a2);
				return 0;
			}
			else if(argc==4){
				LTBezierPath self=(LTBezierPath)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				UnityEngine.Vector3 a3;
				checkType(l,4,out a3);
				self.place(a1,a2,a3);
				return 0;
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
	static public int placeLocal(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==3){
				LTBezierPath self=(LTBezierPath)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				self.placeLocal(a1,a2);
				return 0;
			}
			else if(argc==4){
				LTBezierPath self=(LTBezierPath)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				UnityEngine.Vector3 a3;
				checkType(l,4,out a3);
				self.placeLocal(a1,a2,a3);
				return 0;
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
	static public int get_pts(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			pushValue(l,self.pts);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_pts(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			UnityEngine.Vector3[] v;
			checkType(l,2,out v);
			self.pts=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_length(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			pushValue(l,self.length);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_length(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.length=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_orientToPath(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			pushValue(l,self.orientToPath);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_orientToPath(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.orientToPath=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_orientToPath2d(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			pushValue(l,self.orientToPath2d);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_orientToPath2d(IntPtr l) {
		try {
			LTBezierPath self=(LTBezierPath)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.orientToPath2d=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LTBezierPath");
		addMember(l,setPoints);
		addMember(l,point);
		addMember(l,place2d);
		addMember(l,placeLocal2d);
		addMember(l,place);
		addMember(l,placeLocal);
		addMember(l,"pts",get_pts,set_pts,true);
		addMember(l,"length",get_length,set_length,true);
		addMember(l,"orientToPath",get_orientToPath,set_orientToPath,true);
		addMember(l,"orientToPath2d",get_orientToPath2d,set_orientToPath2d,true);
		createTypeMetatable(l,constructor, typeof(LTBezierPath));
	}
}
