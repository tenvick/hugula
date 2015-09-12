using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LTSpline : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
            //LTSpline o;
            //UnityEngine.Vector3[] a1;
            //checkParams(l,2,out a1);
            //o=new LTSpline(a1);
            //pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int map(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.map(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int interp(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.interp(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int point(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
				LTSpline self=(LTSpline)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				self.place(a1,a2);
				return 0;
			}
			else if(argc==4){
				LTSpline self=(LTSpline)checkSelf(l);
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
				LTSpline self=(LTSpline)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				self.placeLocal(a1,a2);
				return 0;
			}
			else if(argc==4){
				LTSpline self=(LTSpline)checkSelf(l);
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
	static public int gizmoDraw(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			self.gizmoDraw(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Velocity(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.Velocity(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_pts(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
	static public int get_orientToPath(IntPtr l) {
		try {
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
			LTSpline self=(LTSpline)checkSelf(l);
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
		getTypeTable(l,"LTSpline");
		addMember(l,map);
		addMember(l,interp);
		addMember(l,point);
		addMember(l,place2d);
		addMember(l,placeLocal2d);
		addMember(l,place);
		addMember(l,placeLocal);
		addMember(l,gizmoDraw);
		addMember(l,Velocity);
		addMember(l,"pts",get_pts,set_pts,true);
		addMember(l,"orientToPath",get_orientToPath,set_orientToPath,true);
		addMember(l,"orientToPath2d",get_orientToPath2d,set_orientToPath2d,true);
		createTypeMetatable(l,constructor, typeof(LTSpline));
	}
}
