using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UdpMasterServer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_UdpPort(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,UdpMasterServer.UdpPort);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_UdpPort(IntPtr l) {
		try {
			System.Int32 v;
			checkType(l,2,out v);
			UdpMasterServer.UdpPort=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UdpMasterServer");
		addMember(l,"UdpPort",get_UdpPort,set_UdpPort,false);
		createTypeMetatable(l,null, typeof(UdpMasterServer),typeof(UnityEngine.MonoBehaviour));
	}
}
