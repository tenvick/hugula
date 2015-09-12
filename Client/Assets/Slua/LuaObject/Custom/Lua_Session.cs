using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_Session : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			Session o;
			System.Net.Sockets.TcpClient a1;
			checkType(l,2,out a1);
			o=new Session(a1);
			pushValue(l,o);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Send(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,2,typeof(Msg))){
				Session self=(Session)checkSelf(l);
				Msg a1;
				checkType(l,2,out a1);
				self.Send(a1);
				return 0;
			}
			else if(matchType(l,argc,2,typeof(System.Byte[]))){
				Session self=(Session)checkSelf(l);
				System.Byte[] a1;
				checkType(l,2,out a1);
				self.Send(a1);
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
	static public int Close(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			self.Close();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Receive(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			self.Receive();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetMessage(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			var ret=self.GetMessage();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_id(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			pushValue(l,self.id);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Client(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			pushValue(l,self.Client);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"Session");
		addMember(l,Send);
		addMember(l,Close);
		addMember(l,Receive);
		addMember(l,GetMessage);
		addMember(l,"id",get_id,null,true);
		addMember(l,"Client",get_Client,null,true);
		createTypeMetatable(l,constructor, typeof(Session));
	}
}
