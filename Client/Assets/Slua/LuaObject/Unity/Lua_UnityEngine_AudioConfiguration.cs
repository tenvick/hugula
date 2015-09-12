using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AudioConfiguration : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration o;
			o=new UnityEngine.AudioConfiguration();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_speakerMode(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			pushEnum(l,(int)self.speakerMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_speakerMode(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			UnityEngine.AudioSpeakerMode v;
			checkEnum(l,2,out v);
			self.speakerMode=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dspBufferSize(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			pushValue(l,self.dspBufferSize);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_dspBufferSize(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.dspBufferSize=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sampleRate(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			pushValue(l,self.sampleRate);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sampleRate(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.sampleRate=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_numRealVoices(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			pushValue(l,self.numRealVoices);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_numRealVoices(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.numRealVoices=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_numVirtualVoices(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			pushValue(l,self.numVirtualVoices);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_numVirtualVoices(IntPtr l) {
		try {
			UnityEngine.AudioConfiguration self;
			checkValueType(l,1,out self);
			System.Int32 v;
			checkType(l,2,out v);
			self.numVirtualVoices=v;
			setBack(l,self);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AudioConfiguration");
		addMember(l,"speakerMode",get_speakerMode,set_speakerMode,true);
		addMember(l,"dspBufferSize",get_dspBufferSize,set_dspBufferSize,true);
		addMember(l,"sampleRate",get_sampleRate,set_sampleRate,true);
		addMember(l,"numRealVoices",get_numRealVoices,set_numRealVoices,true);
		addMember(l,"numVirtualVoices",get_numVirtualVoices,set_numVirtualVoices,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AudioConfiguration),typeof(System.ValueType));
	}
}
