using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_RenderSettings : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.RenderSettings o;
			o=new UnityEngine.RenderSettings();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fog(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.fog);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fog(IntPtr l) {
		try {
			bool v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.fog=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fogMode(IntPtr l) {
		try {
			pushEnum(l,(int)UnityEngine.RenderSettings.fogMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fogMode(IntPtr l) {
		try {
			UnityEngine.FogMode v;
			checkEnum(l,2,out v);
			UnityEngine.RenderSettings.fogMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fogColor(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.fogColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fogColor(IntPtr l) {
		try {
			UnityEngine.Color v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.fogColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fogDensity(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.fogDensity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fogDensity(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.fogDensity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fogStartDistance(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.fogStartDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fogStartDistance(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.fogStartDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fogEndDistance(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.fogEndDistance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fogEndDistance(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.fogEndDistance=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientMode(IntPtr l) {
		try {
			pushEnum(l,(int)UnityEngine.RenderSettings.ambientMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientMode(IntPtr l) {
		try {
			UnityEngine.Rendering.AmbientMode v;
			checkEnum(l,2,out v);
			UnityEngine.RenderSettings.ambientMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientSkyColor(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientSkyColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientSkyColor(IntPtr l) {
		try {
			UnityEngine.Color v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.ambientSkyColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientEquatorColor(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientEquatorColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientEquatorColor(IntPtr l) {
		try {
			UnityEngine.Color v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.ambientEquatorColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientGroundColor(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientGroundColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientGroundColor(IntPtr l) {
		try {
			UnityEngine.Color v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.ambientGroundColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientLight(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientLight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientLight(IntPtr l) {
		try {
			UnityEngine.Color v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.ambientLight=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientIntensity(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientIntensity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientIntensity(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.ambientIntensity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ambientProbe(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.ambientProbe);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ambientProbe(IntPtr l) {
		try {
			UnityEngine.Rendering.SphericalHarmonicsL2 v;
			checkValueType(l,2,out v);
			UnityEngine.RenderSettings.ambientProbe=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_reflectionIntensity(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.reflectionIntensity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_reflectionIntensity(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.reflectionIntensity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_reflectionBounces(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.reflectionBounces);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_reflectionBounces(IntPtr l) {
		try {
			int v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.reflectionBounces=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_haloStrength(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.haloStrength);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_haloStrength(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.haloStrength=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_flareStrength(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.flareStrength);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_flareStrength(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.flareStrength=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_flareFadeSpeed(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.flareFadeSpeed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_flareFadeSpeed(IntPtr l) {
		try {
			float v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.flareFadeSpeed=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_skybox(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.skybox);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_skybox(IntPtr l) {
		try {
			UnityEngine.Material v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.skybox=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultReflectionMode(IntPtr l) {
		try {
			pushEnum(l,(int)UnityEngine.RenderSettings.defaultReflectionMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultReflectionMode(IntPtr l) {
		try {
			UnityEngine.Rendering.DefaultReflectionMode v;
			checkEnum(l,2,out v);
			UnityEngine.RenderSettings.defaultReflectionMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_defaultReflectionResolution(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.defaultReflectionResolution);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_defaultReflectionResolution(IntPtr l) {
		try {
			int v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.defaultReflectionResolution=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_customReflection(IntPtr l) {
		try {
			pushValue(l,UnityEngine.RenderSettings.customReflection);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_customReflection(IntPtr l) {
		try {
			UnityEngine.Cubemap v;
			checkType(l,2,out v);
			UnityEngine.RenderSettings.customReflection=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.RenderSettings");
		addMember(l,"fog",get_fog,set_fog,false);
		addMember(l,"fogMode",get_fogMode,set_fogMode,false);
		addMember(l,"fogColor",get_fogColor,set_fogColor,false);
		addMember(l,"fogDensity",get_fogDensity,set_fogDensity,false);
		addMember(l,"fogStartDistance",get_fogStartDistance,set_fogStartDistance,false);
		addMember(l,"fogEndDistance",get_fogEndDistance,set_fogEndDistance,false);
		addMember(l,"ambientMode",get_ambientMode,set_ambientMode,false);
		addMember(l,"ambientSkyColor",get_ambientSkyColor,set_ambientSkyColor,false);
		addMember(l,"ambientEquatorColor",get_ambientEquatorColor,set_ambientEquatorColor,false);
		addMember(l,"ambientGroundColor",get_ambientGroundColor,set_ambientGroundColor,false);
		addMember(l,"ambientLight",get_ambientLight,set_ambientLight,false);
		addMember(l,"ambientIntensity",get_ambientIntensity,set_ambientIntensity,false);
		addMember(l,"ambientProbe",get_ambientProbe,set_ambientProbe,false);
		addMember(l,"reflectionIntensity",get_reflectionIntensity,set_reflectionIntensity,false);
		addMember(l,"reflectionBounces",get_reflectionBounces,set_reflectionBounces,false);
		addMember(l,"haloStrength",get_haloStrength,set_haloStrength,false);
		addMember(l,"flareStrength",get_flareStrength,set_flareStrength,false);
		addMember(l,"flareFadeSpeed",get_flareFadeSpeed,set_flareFadeSpeed,false);
		addMember(l,"skybox",get_skybox,set_skybox,false);
		addMember(l,"defaultReflectionMode",get_defaultReflectionMode,set_defaultReflectionMode,false);
		addMember(l,"defaultReflectionResolution",get_defaultReflectionResolution,set_defaultReflectionResolution,false);
		addMember(l,"customReflection",get_customReflection,set_customReflection,false);
		createTypeMetatable(l,constructor, typeof(UnityEngine.RenderSettings),typeof(UnityEngine.Object));
	}
}
