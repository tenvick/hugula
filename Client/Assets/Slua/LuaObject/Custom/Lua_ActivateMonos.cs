using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_ActivateMonos : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_monos(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			pushValue(l,self.monos);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_monos(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.MonoBehaviour> v;
			checkType(l,2,out v);
			self.monos=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activateGameObj(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			pushValue(l,self.activateGameObj);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activateGameObj(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.GameObject> v;
			checkType(l,2,out v);
			self.activateGameObj=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"ActivateMonos");
		addMember(l,"monos",get_monos,set_monos,true);
		addMember(l,"activateGameObj",get_activateGameObj,set_activateGameObj,true);
		createTypeMetatable(l,null, typeof(ActivateMonos),typeof(UnityEngine.MonoBehaviour));
	}
}
