using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_CUtils : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			CUtils o;
			o=new CUtils();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetURLFileSuffix_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetURLFileSuffix(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetURLFileName_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetURLFileName(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetKeyURLFileName_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetKeyURLFileName(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetURLFullFileName_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetURLFullFileName(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetFileFullPath_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetFileFullPath(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAssetFullPath_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetAssetFullPath(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetFileFullPathNoProtocol_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetFileFullPathNoProtocol(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetDirectoryFullPathNoProtocol_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetDirectoryFullPathNoProtocol(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetAssetPath_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CUtils.GetAssetPath(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetPlatformFolderForAssetBundles_s(IntPtr l) {
		try {
			var ret=CUtils.GetPlatformFolderForAssetBundles();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Collect_s(IntPtr l) {
		try {
			CUtils.Collect();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Execute_s(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,1,typeof(BetterList<System.Action>))){
				BetterList<System.Action> a1;
				checkType(l,1,out a1);
				CUtils.Execute(a1);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,1,typeof(IList<System.Action>))){
				System.Collections.Generic.IList<System.Action> a1;
				checkType(l,1,out a1);
				CUtils.Execute(a1);
				pushValue(l,true);
				return 1;
			}
			pushValue(l,false);
			LuaDLL.lua_pushstring(l,"No matched override function to call");
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_currPersistentExist(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,CUtils.currPersistentExist);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_currPersistentExist(IntPtr l) {
		try {
			System.Boolean v;
			checkType(l,2,out v);
			CUtils.currPersistentExist=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dataPath(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,CUtils.dataPath);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"CUtils");
		addMember(l,GetURLFileSuffix_s);
		addMember(l,GetURLFileName_s);
		addMember(l,GetKeyURLFileName_s);
		addMember(l,GetURLFullFileName_s);
		addMember(l,GetFileFullPath_s);
		addMember(l,GetAssetFullPath_s);
		addMember(l,GetFileFullPathNoProtocol_s);
		addMember(l,GetDirectoryFullPathNoProtocol_s);
		addMember(l,GetAssetPath_s);
		addMember(l,GetPlatformFolderForAssetBundles_s);
		addMember(l,Collect_s);
		addMember(l,Execute_s);
		addMember(l,"currPersistentExist",get_currPersistentExist,set_currPersistentExist,false);
		addMember(l,"dataPath",get_dataPath,null,false);
		createTypeMetatable(l,constructor, typeof(CUtils));
	}
}
