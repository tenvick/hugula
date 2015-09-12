using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ProceduralMaterial : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralPropertyDescriptions(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			var ret=self.GetProceduralPropertyDescriptions();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int HasProceduralProperty(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.HasProceduralProperty(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralBoolean(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralBoolean(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralBoolean(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Boolean a2;
			checkType(l,3,out a2);
			self.SetProceduralBoolean(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralFloat(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralFloat(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralFloat(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Single a2;
			checkType(l,3,out a2);
			self.SetProceduralFloat(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralVector(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralVector(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralVector(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			UnityEngine.Vector4 a2;
			checkType(l,3,out a2);
			self.SetProceduralVector(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralColor(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralColor(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralColor(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			UnityEngine.Color a2;
			checkType(l,3,out a2);
			self.SetProceduralColor(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralEnum(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralEnum(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralEnum(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Int32 a2;
			checkType(l,3,out a2);
			self.SetProceduralEnum(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetProceduralTexture(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetProceduralTexture(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetProceduralTexture(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			UnityEngine.Texture2D a2;
			checkType(l,3,out a2);
			self.SetProceduralTexture(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsProceduralPropertyCached(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.IsProceduralPropertyCached(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int CacheProceduralProperty(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Boolean a2;
			checkType(l,3,out a2);
			self.CacheProceduralProperty(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ClearCache(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			self.ClearCache();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RebuildTextures(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			self.RebuildTextures();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RebuildTexturesImmediately(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			self.RebuildTexturesImmediately();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetGeneratedTextures(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			var ret=self.GetGeneratedTextures();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetGeneratedTexture(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.GetGeneratedTexture(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int StopRebuilds_s(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial.StopRebuilds();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cacheSize(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushEnum(l,(int)self.cacheSize);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cacheSize(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			UnityEngine.ProceduralCacheSize v;
			checkEnum(l,2,out v);
			self.cacheSize=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_animationUpdateRate(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.animationUpdateRate);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_animationUpdateRate(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.animationUpdateRate=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isProcessing(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.isProcessing);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isCachedDataAvailable(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.isCachedDataAvailable);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isLoadTimeGenerated(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.isLoadTimeGenerated);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isLoadTimeGenerated(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isLoadTimeGenerated=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_loadingBehavior(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushEnum(l,(int)self.loadingBehavior);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isSupported(IntPtr l) {
		try {
			pushValue(l,UnityEngine.ProceduralMaterial.isSupported);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_substanceProcessorUsage(IntPtr l) {
		try {
			pushEnum(l,(int)UnityEngine.ProceduralMaterial.substanceProcessorUsage);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_substanceProcessorUsage(IntPtr l) {
		try {
			UnityEngine.ProceduralProcessorUsage v;
			checkEnum(l,2,out v);
			UnityEngine.ProceduralMaterial.substanceProcessorUsage=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_preset(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.preset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_preset(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.preset=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isReadable(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			pushValue(l,self.isReadable);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isReadable(IntPtr l) {
		try {
			UnityEngine.ProceduralMaterial self=(UnityEngine.ProceduralMaterial)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isReadable=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ProceduralMaterial");
		addMember(l,GetProceduralPropertyDescriptions);
		addMember(l,HasProceduralProperty);
		addMember(l,GetProceduralBoolean);
		addMember(l,SetProceduralBoolean);
		addMember(l,GetProceduralFloat);
		addMember(l,SetProceduralFloat);
		addMember(l,GetProceduralVector);
		addMember(l,SetProceduralVector);
		addMember(l,GetProceduralColor);
		addMember(l,SetProceduralColor);
		addMember(l,GetProceduralEnum);
		addMember(l,SetProceduralEnum);
		addMember(l,GetProceduralTexture);
		addMember(l,SetProceduralTexture);
		addMember(l,IsProceduralPropertyCached);
		addMember(l,CacheProceduralProperty);
		addMember(l,ClearCache);
		addMember(l,RebuildTextures);
		addMember(l,RebuildTexturesImmediately);
		addMember(l,GetGeneratedTextures);
		addMember(l,GetGeneratedTexture);
		addMember(l,StopRebuilds_s);
		addMember(l,"cacheSize",get_cacheSize,set_cacheSize,true);
		addMember(l,"animationUpdateRate",get_animationUpdateRate,set_animationUpdateRate,true);
		addMember(l,"isProcessing",get_isProcessing,null,true);
		addMember(l,"isCachedDataAvailable",get_isCachedDataAvailable,null,true);
		addMember(l,"isLoadTimeGenerated",get_isLoadTimeGenerated,set_isLoadTimeGenerated,true);
		addMember(l,"loadingBehavior",get_loadingBehavior,null,true);
		addMember(l,"isSupported",get_isSupported,null,false);
		addMember(l,"substanceProcessorUsage",get_substanceProcessorUsage,set_substanceProcessorUsage,false);
		addMember(l,"preset",get_preset,set_preset,true);
		addMember(l,"isReadable",get_isReadable,set_isReadable,true);
		createTypeMetatable(l,null, typeof(UnityEngine.ProceduralMaterial),typeof(UnityEngine.Material));
	}
}
