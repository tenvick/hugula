using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AnimationState : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AnimationState o;
			o=new UnityEngine.AnimationState();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int AddMixingTransform(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==2){
				UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				self.AddMixingTransform(a1);
				return 0;
			}
			else if(argc==3){
				UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
				UnityEngine.Transform a1;
				checkType(l,2,out a1);
				System.Boolean a2;
				checkType(l,3,out a2);
				self.AddMixingTransform(a1,a2);
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
	static public int RemoveMixingTransform(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			UnityEngine.Transform a1;
			checkType(l,2,out a1);
			self.RemoveMixingTransform(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enabled(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.enabled);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enabled(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.enabled=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_weight(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.weight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_weight(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.weight=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_wrapMode(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushEnum(l,(int)self.wrapMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_wrapMode(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			UnityEngine.WrapMode v;
			checkEnum(l,2,out v);
			self.wrapMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_time(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.time);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_time(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.time=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalizedTime(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.normalizedTime);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_normalizedTime(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.normalizedTime=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_speed(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.speed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_speed(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.speed=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalizedSpeed(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.normalizedSpeed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_normalizedSpeed(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.normalizedSpeed=v;
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
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.length);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_layer(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.layer);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_layer(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.layer=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_clip(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.clip);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_name(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushValue(l,self.name);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_name(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.name=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_blendMode(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			pushEnum(l,(int)self.blendMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_blendMode(IntPtr l) {
		try {
			UnityEngine.AnimationState self=(UnityEngine.AnimationState)checkSelf(l);
			UnityEngine.AnimationBlendMode v;
			checkEnum(l,2,out v);
			self.blendMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AnimationState");
		addMember(l,AddMixingTransform);
		addMember(l,RemoveMixingTransform);
		addMember(l,"enabled",get_enabled,set_enabled,true);
		addMember(l,"weight",get_weight,set_weight,true);
		addMember(l,"wrapMode",get_wrapMode,set_wrapMode,true);
		addMember(l,"time",get_time,set_time,true);
		addMember(l,"normalizedTime",get_normalizedTime,set_normalizedTime,true);
		addMember(l,"speed",get_speed,set_speed,true);
		addMember(l,"normalizedSpeed",get_normalizedSpeed,set_normalizedSpeed,true);
		addMember(l,"length",get_length,null,true);
		addMember(l,"layer",get_layer,set_layer,true);
		addMember(l,"clip",get_clip,null,true);
		addMember(l,"name",get_name,set_name,true);
		addMember(l,"blendMode",get_blendMode,set_blendMode,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AnimationState),typeof(UnityEngine.TrackedReference));
	}
}
