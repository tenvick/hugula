using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_ReflectionProbe : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe o;
			o=new UnityEngine.ReflectionProbe();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RenderProbe(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
				var ret=self.RenderProbe();
				pushValue(l,ret);
				return 1;
			}
			else if(argc==2){
				UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
				UnityEngine.RenderTexture a1;
				checkType(l,2,out a1);
				var ret=self.RenderProbe(a1);
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
	static public int IsFinishedRendering(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			var ret=self.IsFinishedRendering(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int BlendCubemap_s(IntPtr l) {
		try {
			UnityEngine.Texture a1;
			checkType(l,1,out a1);
			UnityEngine.Texture a2;
			checkType(l,2,out a2);
			System.Single a3;
			checkType(l,3,out a3);
			UnityEngine.RenderTexture a4;
			checkType(l,4,out a4);
			var ret=UnityEngine.ReflectionProbe.BlendCubemap(a1,a2,a3,a4);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_type(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushEnum(l,(int)self.type);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_type(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeType v;
			checkEnum(l,2,out v);
			self.type=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_hdr(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.hdr);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_hdr(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.hdr=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_size(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.size);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_size(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.size=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_center(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.center);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_center(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Vector3 v;
			checkType(l,2,out v);
			self.center=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_nearClipPlane(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.nearClipPlane);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_nearClipPlane(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.nearClipPlane=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_farClipPlane(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.farClipPlane);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_farClipPlane(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.farClipPlane=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_shadowDistance(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.shadowDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_shadowDistance(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.shadowDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_resolution(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.resolution);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_resolution(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.resolution=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cullingMask(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.cullingMask);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cullingMask(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.cullingMask=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_clearFlags(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushEnum(l,(int)self.clearFlags);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_clearFlags(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeClearFlags v;
			checkEnum(l,2,out v);
			self.clearFlags=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_backgroundColor(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.backgroundColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_backgroundColor(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.backgroundColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_intensity(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.intensity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_intensity(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.intensity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_boxProjection(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.boxProjection);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_boxProjection(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.boxProjection=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bounds(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.bounds);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_mode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushEnum(l,(int)self.mode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_mode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeMode v;
			checkEnum(l,2,out v);
			self.mode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_importance(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.importance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_importance(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.importance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_refreshMode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushEnum(l,(int)self.refreshMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_refreshMode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeRefreshMode v;
			checkEnum(l,2,out v);
			self.refreshMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_timeSlicingMode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushEnum(l,(int)self.timeSlicingMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_timeSlicingMode(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeTimeSlicingMode v;
			checkEnum(l,2,out v);
			self.timeSlicingMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_bakedTexture(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.bakedTexture);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_bakedTexture(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.bakedTexture=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_customBakedTexture(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.customBakedTexture);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_customBakedTexture(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.customBakedTexture=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_texture(IntPtr l) {
		try {
			UnityEngine.ReflectionProbe self=(UnityEngine.ReflectionProbe)checkSelf(l);
			pushValue(l,self.texture);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.ReflectionProbe");
		addMember(l,RenderProbe);
		addMember(l,IsFinishedRendering);
		addMember(l,BlendCubemap_s);
		addMember(l,"type",get_type,set_type,true);
		addMember(l,"hdr",get_hdr,set_hdr,true);
		addMember(l,"size",get_size,set_size,true);
		addMember(l,"center",get_center,set_center,true);
		addMember(l,"nearClipPlane",get_nearClipPlane,set_nearClipPlane,true);
		addMember(l,"farClipPlane",get_farClipPlane,set_farClipPlane,true);
		addMember(l,"shadowDistance",get_shadowDistance,set_shadowDistance,true);
		addMember(l,"resolution",get_resolution,set_resolution,true);
		addMember(l,"cullingMask",get_cullingMask,set_cullingMask,true);
		addMember(l,"clearFlags",get_clearFlags,set_clearFlags,true);
		addMember(l,"backgroundColor",get_backgroundColor,set_backgroundColor,true);
		addMember(l,"intensity",get_intensity,set_intensity,true);
		addMember(l,"boxProjection",get_boxProjection,set_boxProjection,true);
		addMember(l,"bounds",get_bounds,null,true);
		addMember(l,"mode",get_mode,set_mode,true);
		addMember(l,"importance",get_importance,set_importance,true);
		addMember(l,"refreshMode",get_refreshMode,set_refreshMode,true);
		addMember(l,"timeSlicingMode",get_timeSlicingMode,set_timeSlicingMode,true);
		addMember(l,"bakedTexture",get_bakedTexture,set_bakedTexture,true);
		addMember(l,"customBakedTexture",get_customBakedTexture,set_customBakedTexture,true);
		addMember(l,"texture",get_texture,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.ReflectionProbe),typeof(UnityEngine.Behaviour));
	}
}
