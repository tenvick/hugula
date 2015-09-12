using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_FileHelper : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			FileHelper o;
			o=new FileHelper();
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UnZipFile_s(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,1,typeof(System.Byte[]),typeof(string))){
				System.Byte[] a1;
				checkType(l,1,out a1);
				System.String a2;
				checkType(l,2,out a2);
				FileHelper.UnZipFile(a1,a2);
				return 0;
			}
			else if(matchType(l,argc,1,typeof(System.IO.Stream),typeof(string))){
				System.IO.Stream a1;
				checkType(l,1,out a1);
				System.String a2;
				checkType(l,2,out a2);
				FileHelper.UnZipFile(a1,a2);
				return 0;
			}
			else if(matchType(l,argc,1,typeof(string),typeof(string))){
				System.String a1;
				checkType(l,1,out a1);
				System.String a2;
				checkType(l,2,out a2);
				FileHelper.UnZipFile(a1,a2);
				return 0;
			}
			LuaDLL.luaL_error(l,"No matched override function to call");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UnpackConfigZip_s(IntPtr l) {
		try {
			System.Byte[] a1;
			checkType(l,1,out a1);
			System.Action<System.String,System.String> a2;
			LuaDelegation.checkDelegate(l,2,out a2);
			FileHelper.UnpackConfigZip(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int PersistentUTF8File_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			System.String a2;
			checkType(l,2,out a2);
			FileHelper.PersistentUTF8File(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadUTF8File_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=FileHelper.ReadUTF8File(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DeleteFile_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			FileHelper.DeleteFile(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int UnpackConfigAssetBundleFn_s(IntPtr l) {
		try {
			UnityEngine.AssetBundle a1;
			checkType(l,1,out a1);
			SLua.LuaFunction a2;
			checkType(l,2,out a2);
			FileHelper.UnpackConfigAssetBundleFn(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"FileHelper");
		addMember(l,UnZipFile_s);
		addMember(l,UnpackConfigZip_s);
		addMember(l,PersistentUTF8File_s);
		addMember(l,ReadUTF8File_s);
		addMember(l,DeleteFile_s);
		addMember(l,UnpackConfigAssetBundleFn_s);
		createTypeMetatable(l,constructor, typeof(FileHelper));
	}
}
