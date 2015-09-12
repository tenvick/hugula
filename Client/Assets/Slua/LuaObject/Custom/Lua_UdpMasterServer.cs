using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UdpMasterServer : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_UdpPort(IntPtr l) {
		try {
			pushValue(l,UdpMasterServer.UdpPort);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_UdpPort(IntPtr l) {
		try {
			System.Int32 v;
			checkType(l,2,out v);
			UdpMasterServer.UdpPort=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UdpMasterServer");
		addMember(l,"UdpPort",get_UdpPort,set_UdpPort,false);
		createTypeMetatable(l,null, typeof(UdpMasterServer),typeof(UnityEngine.MonoBehaviour));
	}
}
