using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_ReferGameObjects : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_refers(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.refers);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_refers(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.GameObject> v;
			checkType(l,2,out v);
			self.refers=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_monos(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
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
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Collections.Generic.List<UnityEngine.Behaviour> v;
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
	static public int get_userObject(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.userObject);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userObject(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.userObject=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userBool(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.userBool);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userBool(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.userBool=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userInt(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.userInt);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userInt(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.userInt=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userFloat(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.userFloat);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userFloat(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.userFloat=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userString(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.userString);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userString(IntPtr l) {
		try {
			ReferGameObjects self=(ReferGameObjects)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.userString=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"ReferGameObjects");
		addMember(l,"refers",get_refers,set_refers,true);
		addMember(l,"monos",get_monos,set_monos,true);
		addMember(l,"userObject",get_userObject,set_userObject,true);
		addMember(l,"userBool",get_userBool,set_userBool,true);
		addMember(l,"userInt",get_userInt,set_userInt,true);
		addMember(l,"userFloat",get_userFloat,set_userFloat,true);
		addMember(l,"userString",get_userString,set_userString,true);
		createTypeMetatable(l,null, typeof(ReferGameObjects),typeof(UnityEngine.MonoBehaviour));
	}
}
