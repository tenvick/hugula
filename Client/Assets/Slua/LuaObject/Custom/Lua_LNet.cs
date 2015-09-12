using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LNet : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Connect(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.Int32 a2;
			checkType(l,3,out a2);
			self.Connect(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReConnect(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			self.ReConnect();
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
			LNet self=(LNet)checkSelf(l);
			self.Close();
			return 0;
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
				LNet self=(LNet)checkSelf(l);
				Msg a1;
				checkType(l,2,out a1);
				self.Send(a1);
				return 0;
			}
			else if(matchType(l,argc,2,typeof(System.Byte[]))){
				LNet self=(LNet)checkSelf(l);
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
	static public int Update(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			self.Update();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnApplicationPause(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			System.Boolean a1;
			checkType(l,2,out a1);
			self.OnApplicationPause(a1);
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
			LNet self=(LNet)checkSelf(l);
			self.Receive();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SendErro(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			System.String a2;
			checkType(l,3,out a2);
			self.SendErro(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Dispose(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			self.Dispose();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_pingDelay(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.pingDelay);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_pingDelay(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.pingDelay=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_timeoutMiliSecond(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.timeoutMiliSecond);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_timeoutMiliSecond(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.timeoutMiliSecond=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onAppErrorFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onAppErrorFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onAppErrorFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onAppErrorFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onConnectionCloseFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onConnectionCloseFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onConnectionCloseFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onConnectionCloseFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onConnectionFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onConnectionFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onConnectionFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onConnectionFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onMessageReceiveFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onMessageReceiveFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onMessageReceiveFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onMessageReceiveFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onConnectionTimeoutFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onConnectionTimeoutFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onConnectionTimeoutFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onConnectionTimeoutFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onReConnectFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onReConnectFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onReConnectFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onReConnectFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onAppPauseFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onAppPauseFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onAppPauseFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onAppPauseFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onIntervalFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.onIntervalFn);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onIntervalFn(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onIntervalFn=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isConnectCall(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.isConnectCall);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_IsConnected(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.IsConnected);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Host(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.Host);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Port(IntPtr l) {
		try {
			LNet self=(LNet)checkSelf(l);
			pushValue(l,self.Port);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ChatInstance(IntPtr l) {
		try {
			pushValue(l,LNet.ChatInstance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_instance(IntPtr l) {
		try {
			pushValue(l,LNet.instance);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LNet");
		addMember(l,Connect);
		addMember(l,ReConnect);
		addMember(l,Close);
		addMember(l,Send);
		addMember(l,Update);
		addMember(l,OnApplicationPause);
		addMember(l,Receive);
		addMember(l,SendErro);
		addMember(l,Dispose);
		addMember(l,"pingDelay",get_pingDelay,set_pingDelay,true);
		addMember(l,"timeoutMiliSecond",get_timeoutMiliSecond,set_timeoutMiliSecond,true);
		addMember(l,"onAppErrorFn",get_onAppErrorFn,set_onAppErrorFn,true);
		addMember(l,"onConnectionCloseFn",get_onConnectionCloseFn,set_onConnectionCloseFn,true);
		addMember(l,"onConnectionFn",get_onConnectionFn,set_onConnectionFn,true);
		addMember(l,"onMessageReceiveFn",get_onMessageReceiveFn,set_onMessageReceiveFn,true);
		addMember(l,"onConnectionTimeoutFn",get_onConnectionTimeoutFn,set_onConnectionTimeoutFn,true);
		addMember(l,"onReConnectFn",get_onReConnectFn,set_onReConnectFn,true);
		addMember(l,"onAppPauseFn",get_onAppPauseFn,set_onAppPauseFn,true);
		addMember(l,"onIntervalFn",get_onIntervalFn,set_onIntervalFn,true);
		addMember(l,"isConnectCall",get_isConnectCall,null,true);
		addMember(l,"IsConnected",get_IsConnected,null,true);
		addMember(l,"Host",get_Host,null,true);
		addMember(l,"Port",get_Port,null,true);
		addMember(l,"ChatInstance",get_ChatInstance,null,false);
		addMember(l,"instance",get_instance,null,false);
		createTypeMetatable(l,null, typeof(LNet));
	}
}
