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
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
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
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(System.Byte[]))){
				Session self=(Session)checkSelf(l);
				System.Byte[] a1;
				checkType(l,2,out a1);
				self.Send(a1);
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
	static public int Close(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			self.Close();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Receive(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			self.Receive();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetMessage(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			var ret=self.GetMessage();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_id(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.id);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Client(IntPtr l) {
		try {
			Session self=(Session)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.Client);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
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
