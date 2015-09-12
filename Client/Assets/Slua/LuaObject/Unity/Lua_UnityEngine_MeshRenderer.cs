using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_MeshRenderer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.MeshRenderer o;
			o=new UnityEngine.MeshRenderer();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_additionalVertexStreams(IntPtr l) {
		try {
			UnityEngine.MeshRenderer self=(UnityEngine.MeshRenderer)checkSelf(l);
			pushValue(l,self.additionalVertexStreams);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_additionalVertexStreams(IntPtr l) {
		try {
			UnityEngine.MeshRenderer self=(UnityEngine.MeshRenderer)checkSelf(l);
			UnityEngine.Mesh v;
			checkType(l,2,out v);
			self.additionalVertexStreams=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.MeshRenderer");
		addMember(l,"additionalVertexStreams",get_additionalVertexStreams,set_additionalVertexStreams,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.MeshRenderer),typeof(UnityEngine.Renderer));
	}
}
