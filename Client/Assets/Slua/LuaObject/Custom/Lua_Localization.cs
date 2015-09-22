using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_Localization : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Load_s(IntPtr l) {
		try {
			System.Byte[] a1;
			checkType(l,1,out a1);
			Localization.Load(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Get_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=Localization.Get(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Exists_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=Localization.Exists(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_localizationHasBeenSet(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,Localization.localizationHasBeenSet);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_localizationHasBeenSet(IntPtr l) {
		try {
			System.Boolean v;
			checkType(l,2,out v);
			Localization.localizationHasBeenSet=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dictionary(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,Localization.dictionary);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_dictionary(IntPtr l) {
		try {
			Dictionary<System.String,System.String> v;
			checkType(l,2,out v);
			Localization.dictionary=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_knownLanguages(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,Localization.knownLanguages);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_language(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,Localization.language);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_language(IntPtr l) {
		try {
			string v;
			checkType(l,2,out v);
			Localization.language=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"Localization");
		addMember(l,Load_s);
		addMember(l,Get_s);
		addMember(l,Exists_s);
		addMember(l,"localizationHasBeenSet",get_localizationHasBeenSet,set_localizationHasBeenSet,false);
		addMember(l,"dictionary",get_dictionary,set_dictionary,false);
		addMember(l,"knownLanguages",get_knownLanguages,null,false);
		addMember(l,"language",get_language,set_language,false);
		createTypeMetatable(l,null, typeof(Localization));
	}
}
