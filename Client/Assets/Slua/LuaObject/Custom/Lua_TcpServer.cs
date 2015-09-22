using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_TcpServer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int BroadCast(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==1){
				TcpServer self=(TcpServer)checkSelf(l);
				self.BroadCast();
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(Msg))){
				TcpServer self=(TcpServer)checkSelf(l);
				Msg a1;
				checkType(l,2,out a1);
				self.BroadCast(a1);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(System.Byte[]))){
				TcpServer self=(TcpServer)checkSelf(l);
				System.Byte[] a1;
				checkType(l,2,out a1);
				self.BroadCast(a1);
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
	static public int Kick(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			self.Kick(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Stop(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			self.Stop();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int StartTcpServer_s(IntPtr l) {
		try {
			TcpServer.StartTcpServer();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int StopTcpServer_s(IntPtr l) {
		try {
			TcpServer.StopTcpServer();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetLocalIP_s(IntPtr l) {
		try {
			var ret=TcpServer.GetLocalIP();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_GAME_TYPE(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,TcpServer.GAME_TYPE);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_port(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,TcpServer.port);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_port(IntPtr l) {
		try {
			System.Int32 v;
			checkType(l,2,out v);
			TcpServer.port=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_autoBroadcast(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.autoBroadcast);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_autoBroadcast(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.autoBroadcast=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onClientConnectFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onClientConnectFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onClientConnectFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onClientConnectFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onMessageArriveFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onMessageArriveFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onMessageArriveFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onMessageArriveFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onClientCloseFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onClientCloseFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onClientCloseFn(IntPtr l) {
		try {
			TcpServer self=(TcpServer)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onClientCloseFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_tcpClientConnected(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,TcpServer.tcpClientConnected);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_tcpClientConnected(IntPtr l) {
		try {
			System.Threading.ManualResetEvent v;
			checkType(l,2,out v);
			TcpServer.tcpClientConnected=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_currTcpServer(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,TcpServer.currTcpServer);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_currTcpServer(IntPtr l) {
		try {
			TcpServer v;
			checkType(l,2,out v);
			TcpServer.currTcpServer=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"TcpServer");
		addMember(l,BroadCast);
		addMember(l,Kick);
		addMember(l,Stop);
		addMember(l,StartTcpServer_s);
		addMember(l,StopTcpServer_s);
		addMember(l,GetLocalIP_s);
		addMember(l,"GAME_TYPE",get_GAME_TYPE,null,false);
		addMember(l,"port",get_port,set_port,false);
		addMember(l,"autoBroadcast",get_autoBroadcast,set_autoBroadcast,true);
		addMember(l,"onClientConnectFn",get_onClientConnectFn,set_onClientConnectFn,true);
		addMember(l,"onMessageArriveFn",get_onMessageArriveFn,set_onMessageArriveFn,true);
		addMember(l,"onClientCloseFn",get_onClientCloseFn,set_onClientCloseFn,true);
		addMember(l,"tcpClientConnected",get_tcpClientConnected,set_tcpClientConnected,false);
		addMember(l,"currTcpServer",get_currTcpServer,set_currTcpServer,false);
		createTypeMetatable(l,null, typeof(TcpServer),typeof(UnityEngine.MonoBehaviour));
	}
}
