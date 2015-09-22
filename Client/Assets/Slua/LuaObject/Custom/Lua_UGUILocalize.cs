using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UGUILocalize : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_key(IntPtr l) {
		try {
			UGUILocalize self=(UGUILocalize)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.key);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_key(IntPtr l) {
		try {
			UGUILocalize self=(UGUILocalize)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.key=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_value(IntPtr l) {
		try {
			UGUILocalize self=(UGUILocalize)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.value=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UGUILocalize");
		addMember(l,"key",get_key,set_key,true);
		addMember(l,"value",null,set_value,true);
		createTypeMetatable(l,null, typeof(UGUILocalize),typeof(UnityEngine.MonoBehaviour));
	}
}
