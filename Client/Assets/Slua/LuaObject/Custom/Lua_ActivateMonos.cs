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
			pushValue(l,true);
			pushValue(l,self.monos);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_monos(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.MonoBehaviour> v;
			checkType(l,2,out v);
			self.monos=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_activateGameObj(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.activateGameObj);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_activateGameObj(IntPtr l) {
		try {
			ActivateMonos self=(ActivateMonos)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.GameObject> v;
			checkType(l,2,out v);
			self.activateGameObj=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"ActivateMonos");
		addMember(l,"monos",get_monos,set_monos,true);
		addMember(l,"activateGameObj",get_activateGameObj,set_activateGameObj,true);
		createTypeMetatable(l,null, typeof(ActivateMonos),typeof(UnityEngine.MonoBehaviour));
	}
}
