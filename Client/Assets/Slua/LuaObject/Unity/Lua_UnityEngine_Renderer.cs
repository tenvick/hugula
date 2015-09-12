using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_Renderer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.Renderer o;
			o=new UnityEngine.Renderer();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetPropertyBlock(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.MaterialPropertyBlock a1;
			checkType(l,2,out a1);
			self.SetPropertyBlock(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPropertyBlock(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.MaterialPropertyBlock a1;
			checkType(l,2,out a1);
			self.GetPropertyBlock(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetClosestReflectionProbes(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.Rendering.ReflectionProbeBlendInfo> a1;
			checkType(l,2,out a1);
			self.GetClosestReflectionProbes(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isPartOfStaticBatch(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.isPartOfStaticBatch);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_worldToLocalMatrix(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.worldToLocalMatrix);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_localToWorldMatrix(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.localToWorldMatrix);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enabled(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
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
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
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
	static public int get_shadowCastingMode(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushEnum(l,(int)self.shadowCastingMode);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_shadowCastingMode(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Rendering.ShadowCastingMode v;
			checkEnum(l,2,out v);
			self.shadowCastingMode=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_receiveShadows(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.receiveShadows);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_receiveShadows(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.receiveShadows=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_material(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.material);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_material(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Material v;
			checkType(l,2,out v);
			self.material=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sharedMaterial(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.sharedMaterial);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sharedMaterial(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Material v;
			checkType(l,2,out v);
			self.sharedMaterial=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_materials(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.materials);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_materials(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Material[] v;
			checkType(l,2,out v);
			self.materials=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sharedMaterials(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.sharedMaterials);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sharedMaterials(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Material[] v;
			checkType(l,2,out v);
			self.sharedMaterials=v;
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
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.bounds);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_lightmapIndex(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.lightmapIndex);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_lightmapIndex(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.lightmapIndex=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_realtimeLightmapIndex(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.realtimeLightmapIndex);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_lightmapScaleOffset(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.lightmapScaleOffset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_lightmapScaleOffset(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Vector4 v;
			checkType(l,2,out v);
			self.lightmapScaleOffset=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_realtimeLightmapScaleOffset(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.realtimeLightmapScaleOffset);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_realtimeLightmapScaleOffset(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Vector4 v;
			checkType(l,2,out v);
			self.realtimeLightmapScaleOffset=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isVisible(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.isVisible);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useLightProbes(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.useLightProbes);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useLightProbes(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.useLightProbes=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_probeAnchor(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.probeAnchor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_probeAnchor(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Transform v;
			checkType(l,2,out v);
			self.probeAnchor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_reflectionProbeUsage(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushEnum(l,(int)self.reflectionProbeUsage);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_reflectionProbeUsage(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			UnityEngine.Rendering.ReflectionProbeUsage v;
			checkEnum(l,2,out v);
			self.reflectionProbeUsage=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sortingLayerName(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.sortingLayerName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sortingLayerName(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.sortingLayerName=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sortingLayerID(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.sortingLayerID);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sortingLayerID(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.sortingLayerID=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sortingOrder(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			pushValue(l,self.sortingOrder);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sortingOrder(IntPtr l) {
		try {
			UnityEngine.Renderer self=(UnityEngine.Renderer)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.sortingOrder=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.Renderer");
		addMember(l,SetPropertyBlock);
		addMember(l,GetPropertyBlock);
		addMember(l,GetClosestReflectionProbes);
		addMember(l,"isPartOfStaticBatch",get_isPartOfStaticBatch,null,true);
		addMember(l,"worldToLocalMatrix",get_worldToLocalMatrix,null,true);
		addMember(l,"localToWorldMatrix",get_localToWorldMatrix,null,true);
		addMember(l,"enabled",get_enabled,set_enabled,true);
		addMember(l,"shadowCastingMode",get_shadowCastingMode,set_shadowCastingMode,true);
		addMember(l,"receiveShadows",get_receiveShadows,set_receiveShadows,true);
		addMember(l,"material",get_material,set_material,true);
		addMember(l,"sharedMaterial",get_sharedMaterial,set_sharedMaterial,true);
		addMember(l,"materials",get_materials,set_materials,true);
		addMember(l,"sharedMaterials",get_sharedMaterials,set_sharedMaterials,true);
		addMember(l,"bounds",get_bounds,null,true);
		addMember(l,"lightmapIndex",get_lightmapIndex,set_lightmapIndex,true);
		addMember(l,"realtimeLightmapIndex",get_realtimeLightmapIndex,null,true);
		addMember(l,"lightmapScaleOffset",get_lightmapScaleOffset,set_lightmapScaleOffset,true);
		addMember(l,"realtimeLightmapScaleOffset",get_realtimeLightmapScaleOffset,set_realtimeLightmapScaleOffset,true);
		addMember(l,"isVisible",get_isVisible,null,true);
		addMember(l,"useLightProbes",get_useLightProbes,set_useLightProbes,true);
		addMember(l,"probeAnchor",get_probeAnchor,set_probeAnchor,true);
		addMember(l,"reflectionProbeUsage",get_reflectionProbeUsage,set_reflectionProbeUsage,true);
		addMember(l,"sortingLayerName",get_sortingLayerName,set_sortingLayerName,true);
		addMember(l,"sortingLayerID",get_sortingLayerID,set_sortingLayerID,true);
		addMember(l,"sortingOrder",get_sortingOrder,set_sortingOrder,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.Renderer),typeof(UnityEngine.Component));
	}
}
