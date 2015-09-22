using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_CanvasRenderer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer o;
			o=new UnityEngine.CanvasRenderer();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetColor(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			UnityEngine.Color a1;
			checkType(l,2,out a1);
			self.SetColor(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetColor(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			var ret=self.GetColor();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAlpha(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			var ret=self.GetAlpha();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetAlpha(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			self.SetAlpha(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetMaterial(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			UnityEngine.Material a1;
			checkType(l,2,out a1);
			UnityEngine.Texture a2;
			checkType(l,3,out a2);
			self.SetMaterial(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetMaterial(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			var ret=self.GetMaterial();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetVertices(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==2){
				UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
				System.Collections.Generic.List<UnityEngine.UIVertex> a1;
				checkType(l,2,out a1);
				self.SetVertices(a1);
				pushValue(l,true);
				return 1;
			}
			else if(argc==3){
				UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
				UnityEngine.UIVertex[] a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				self.SetVertices(a1,a2);
				pushValue(l,true);
				return 1;
			}
			pushValue(l,false);
			LuaDLL.lua_pushstring(l,"No matched override function to call");
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Clear(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			self.Clear();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isMask(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.isMask);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isMask(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isMask=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_relativeDepth(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.relativeDepth);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_absoluteDepth(IntPtr l) {
		try {
			UnityEngine.CanvasRenderer self=(UnityEngine.CanvasRenderer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.absoluteDepth);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.CanvasRenderer");
		addMember(l,SetColor);
		addMember(l,GetColor);
		addMember(l,GetAlpha);
		addMember(l,SetAlpha);
		addMember(l,SetMaterial);
		addMember(l,GetMaterial);
		addMember(l,SetVertices);
		addMember(l,Clear);
		addMember(l,"isMask",get_isMask,set_isMask,true);
		addMember(l,"relativeDepth",get_relativeDepth,null,true);
		addMember(l,"absoluteDepth",get_absoluteDepth,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.CanvasRenderer),typeof(UnityEngine.Component));
	}
}
