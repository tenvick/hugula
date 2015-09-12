using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_RuntimeAnimatorController : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			UnityEngine.RuntimeAnimatorController o;
			o=new UnityEngine.RuntimeAnimatorController();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_animationClips(IntPtr l) {
		try {
			UnityEngine.RuntimeAnimatorController self=(UnityEngine.RuntimeAnimatorController)checkSelf(l);
			pushValue(l,self.animationClips);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.RuntimeAnimatorController");
		addMember(l,"animationClips",get_animationClips,null,true);
		createTypeMetatable(l,constructor, typeof(UnityEngine.RuntimeAnimatorController),typeof(UnityEngine.Object));
	}
}
