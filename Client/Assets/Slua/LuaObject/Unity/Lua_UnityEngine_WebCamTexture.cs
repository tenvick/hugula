using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_WebCamTexture : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			UnityEngine.WebCamTexture o;
			if(argc==5){
				System.String a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				System.Int32 a3;
				checkType(l,4,out a3);
				System.Int32 a4;
				checkType(l,5,out a4);
				o=new UnityEngine.WebCamTexture(a1,a2,a3,a4);
				pushValue(l,o);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(string),typeof(int),typeof(int))){
				System.String a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				System.Int32 a3;
				checkType(l,4,out a3);
				o=new UnityEngine.WebCamTexture(a1,a2,a3);
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new UnityEngine.WebCamTexture(a1);
				pushValue(l,o);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(int),typeof(int),typeof(int))){
				System.Int32 a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				System.Int32 a3;
				checkType(l,4,out a3);
				o=new UnityEngine.WebCamTexture(a1,a2,a3);
				pushValue(l,o);
				return 1;
			}
			else if(argc==3){
				System.Int32 a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				o=new UnityEngine.WebCamTexture(a1,a2);
				pushValue(l,o);
				return 1;
			}
			else if(argc==1){
				o=new UnityEngine.WebCamTexture();
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
	static public int Play(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			self.Play();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Pause(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			self.Pause();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Stop(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			self.Stop();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPixel(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			System.Int32 a2;
			checkType(l,3,out a2);
			var ret=self.GetPixel(a1,a2);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPixels(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
				var ret=self.GetPixels();
				pushValue(l,ret);
				return 1;
			}
			else if(argc==5){
				UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
				System.Int32 a1;
				checkType(l,2,out a1);
				System.Int32 a2;
				checkType(l,3,out a2);
				System.Int32 a3;
				checkType(l,4,out a3);
				System.Int32 a4;
				checkType(l,5,out a4);
				var ret=self.GetPixels(a1,a2,a3,a4);
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
	static public int GetPixels32(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
				var ret=self.GetPixels32();
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
				UnityEngine.Color32[] a1;
				checkType(l,2,out a1);
				var ret=self.GetPixels32(a1);
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
	static public int get_isPlaying(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.isPlaying);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_deviceName(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.deviceName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_deviceName(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.deviceName=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_requestedFPS(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.requestedFPS);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_requestedFPS(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.requestedFPS=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_requestedWidth(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.requestedWidth);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_requestedWidth(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.requestedWidth=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_requestedHeight(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.requestedHeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_requestedHeight(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.requestedHeight=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_devices(IntPtr l) {
		try {
			pushValue(l,UnityEngine.WebCamTexture.devices);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_videoRotationAngle(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.videoRotationAngle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_videoVerticallyMirrored(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.videoVerticallyMirrored);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_didUpdateThisFrame(IntPtr l) {
		try {
			UnityEngine.WebCamTexture self=(UnityEngine.WebCamTexture)checkSelf(l);
			pushValue(l,self.didUpdateThisFrame);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.WebCamTexture");
		addMember(l,Play);
		addMember(l,Pause);
		addMember(l,Stop);
		addMember(l,GetPixel);
		addMember(l,GetPixels);
		addMember(l,GetPixels32);
		addMember(l,"isPlaying",get_isPlaying,null,true);
		addMember(l,"deviceName",get_deviceName,set_deviceName,true);
		addMember(l,"requestedFPS",get_requestedFPS,set_requestedFPS,true);
		addMember(l,"requestedWidth",get_requestedWidth,set_requestedWidth,true);
		addMember(l,"requestedHeight",get_requestedHeight,set_requestedHeight,true);
		addMember(l,"devices",get_devices,null,false);
		addMember(l,"videoRotationAngle",get_videoRotationAngle,null,true);
		addMember(l,"videoVerticallyMirrored",get_videoVerticallyMirrored,null,true);
		addMember(l,"didUpdateThisFrame",get_didUpdateThisFrame,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.WebCamTexture),typeof(UnityEngine.Texture));
	}
}
