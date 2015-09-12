using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_WWW : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			UnityEngine.WWW o;
			if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new UnityEngine.WWW(a1);
				pushValue(l,o);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(string),typeof(UnityEngine.WWWForm))){
				System.String a1;
				checkType(l,2,out a1);
				UnityEngine.WWWForm a2;
				checkType(l,3,out a2);
				o=new UnityEngine.WWW(a1,a2);
				pushValue(l,o);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(string),typeof(System.Byte[]))){
				System.String a1;
				checkType(l,2,out a1);
				System.Byte[] a2;
				checkType(l,3,out a2);
				o=new UnityEngine.WWW(a1,a2);
				pushValue(l,o);
				return 1;
			}
			else if(argc==4){
				System.String a1;
				checkType(l,2,out a1);
				System.Byte[] a2;
				checkType(l,3,out a2);
				System.Collections.Generic.Dictionary<System.String,System.String> a3;
				checkType(l,4,out a3);
				o=new UnityEngine.WWW(a1,a2,a3);
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
	static public int Dispose(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			self.Dispose();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int InitWWW(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Byte[] a2;
			checkType(l,3,out a2);
			System.String[] a3;
			checkType(l,4,out a3);
			self.InitWWW(a1,a2,a3);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAudioClip(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==2){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				var ret=self.GetAudioClip(a1);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==3){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				System.Boolean a2;
				checkType(l,3,out a2);
				var ret=self.GetAudioClip(a1,a2);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==4){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				System.Boolean a2;
				checkType(l,3,out a2);
				UnityEngine.AudioType a3;
				checkEnum(l,4,out a3);
				var ret=self.GetAudioClip(a1,a2,a3);
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
	static public int GetAudioClipCompressed(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				var ret=self.GetAudioClipCompressed();
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				var ret=self.GetAudioClipCompressed(a1);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==3){
				UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				UnityEngine.AudioType a2;
				checkEnum(l,3,out a2);
				var ret=self.GetAudioClipCompressed(a1,a2);
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
	static public int LoadImageIntoTexture(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			UnityEngine.Texture2D a1;
			checkType(l,2,out a1);
			self.LoadImageIntoTexture(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int EscapeURL_s(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				System.String a1;
				checkType(l,1,out a1);
				var ret=UnityEngine.WWW.EscapeURL(a1);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				System.String a1;
				checkType(l,1,out a1);
				System.Text.Encoding a2;
				checkType(l,2,out a2);
				var ret=UnityEngine.WWW.EscapeURL(a1,a2);
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
	static public int UnEscapeURL_s(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				System.String a1;
				checkType(l,1,out a1);
				var ret=UnityEngine.WWW.UnEscapeURL(a1);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				System.String a1;
				checkType(l,1,out a1);
				System.Text.Encoding a2;
				checkType(l,2,out a2);
				var ret=UnityEngine.WWW.UnEscapeURL(a1,a2);
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
	static public int LoadFromCacheOrDownload_s(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,1,typeof(string),typeof(UnityEngine.Hash128))){
				System.String a1;
				checkType(l,1,out a1);
				UnityEngine.Hash128 a2;
				checkValueType(l,2,out a2);
				var ret=UnityEngine.WWW.LoadFromCacheOrDownload(a1,a2);
				pushValue(l,ret);
				return 1;
			}
			else if(matchType(l,argc,1,typeof(string),typeof(int))){
				System.String a1;
				checkType(l,1,out a1);
				System.Int32 a2;
				checkType(l,2,out a2);
				var ret=UnityEngine.WWW.LoadFromCacheOrDownload(a1,a2);
				pushValue(l,ret);
				return 1;
			}
			else if(matchType(l,argc,1,typeof(string),typeof(UnityEngine.Hash128),typeof(System.UInt32))){
				System.String a1;
				checkType(l,1,out a1);
				UnityEngine.Hash128 a2;
				checkValueType(l,2,out a2);
				System.UInt32 a3;
				checkType(l,3,out a3);
				var ret=UnityEngine.WWW.LoadFromCacheOrDownload(a1,a2,a3);
				pushValue(l,ret);
				return 1;
			}
			else if(matchType(l,argc,1,typeof(string),typeof(int),typeof(System.UInt32))){
				System.String a1;
				checkType(l,1,out a1);
				System.Int32 a2;
				checkType(l,2,out a2);
				System.UInt32 a3;
				checkType(l,3,out a3);
				var ret=UnityEngine.WWW.LoadFromCacheOrDownload(a1,a2,a3);
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
	static public int get_responseHeaders(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.responseHeaders);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_text(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.text);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bytes(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.bytes);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_size(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.size);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_error(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.error);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_texture(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.texture);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_textureNonReadable(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.textureNonReadable);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_audioClip(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.audioClip);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isDone(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.isDone);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_progress(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.progress);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_uploadProgress(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.uploadProgress);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bytesDownloaded(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.bytesDownloaded);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_url(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.url);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_assetBundle(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushValue(l,self.assetBundle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_threadPriority(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			pushEnum(l,(int)self.threadPriority);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_threadPriority(IntPtr l) {
		try {
			UnityEngine.WWW self=(UnityEngine.WWW)checkSelf(l);
			UnityEngine.ThreadPriority v;
			checkEnum(l,2,out v);
			self.threadPriority=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.WWW");
		addMember(l,Dispose);
		addMember(l,InitWWW);
		addMember(l,GetAudioClip);
		addMember(l,GetAudioClipCompressed);
		addMember(l,LoadImageIntoTexture);
		addMember(l,EscapeURL_s);
		addMember(l,UnEscapeURL_s);
		addMember(l,LoadFromCacheOrDownload_s);
		addMember(l,"responseHeaders",get_responseHeaders,null,true);
		addMember(l,"text",get_text,null,true);
		addMember(l,"bytes",get_bytes,null,true);
		addMember(l,"size",get_size,null,true);
		addMember(l,"error",get_error,null,true);
		addMember(l,"texture",get_texture,null,true);
		addMember(l,"textureNonReadable",get_textureNonReadable,null,true);
		addMember(l,"audioClip",get_audioClip,null,true);
		addMember(l,"isDone",get_isDone,null,true);
		addMember(l,"progress",get_progress,null,true);
		addMember(l,"uploadProgress",get_uploadProgress,null,true);
		addMember(l,"bytesDownloaded",get_bytesDownloaded,null,true);
		addMember(l,"url",get_url,null,true);
		addMember(l,"assetBundle",get_assetBundle,null,true);
		addMember(l,"threadPriority",get_threadPriority,set_threadPriority,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.WWW));
	}
}
