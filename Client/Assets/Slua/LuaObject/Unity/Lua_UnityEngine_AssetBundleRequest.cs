using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_AssetBundleRequest : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.AssetBundleRequest o;
			o=new UnityEngine.AssetBundleRequest();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_asset(IntPtr l) {
		try {
			UnityEngine.AssetBundleRequest self=(UnityEngine.AssetBundleRequest)checkSelf(l);
			pushValue(l,self.asset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_allAssets(IntPtr l) {
		try {
			UnityEngine.AssetBundleRequest self=(UnityEngine.AssetBundleRequest)checkSelf(l);
			pushValue(l,self.allAssets);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.AssetBundleRequest");
		addMember(l,"asset",get_asset,null,true);
		addMember(l,"allAssets",get_allAssets,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.AssetBundleRequest),typeof(UnityEngine.AsyncOperation));
	}
}
