using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_PLua : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DoUnity3dLua(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			self.DoUnity3dLua();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DoMain(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			self.DoMain();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadBundle(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,2,typeof(SLua.LuaFunction))){
				PLua self=(PLua)checkSelf(l);
				SLua.LuaFunction a1;
				checkType(l,2,out a1);
				self.LoadBundle(a1);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(bool))){
				PLua self=(PLua)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				self.LoadBundle(a1);
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
	static public int RegisterFunc(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			self.RegisterFunc();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Loader_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=PLua.Loader(a1);
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Log_s(IntPtr l) {
		try {
			System.Object a1;
			checkType(l,1,out a1);
			PLua.Log(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Delay_s(IntPtr l) {
		try {
			SLua.LuaFunction a1;
			checkType(l,1,out a1);
			System.Single a2;
			checkType(l,2,out a2);
			System.Object a3;
			checkType(l,3,out a3);
			PLua.Delay(a1,a2,a3);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int StopDelay_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			PLua.StopDelay(a1);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_enterLua(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,PLua.enterLua);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_enterLua(IntPtr l) {
		try {
			System.String v;
			checkType(l,2,out v);
			PLua.enterLua=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onDestroyFn(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.onDestroyFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onDestroyFn(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.onDestroyFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isDebug(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,PLua.isDebug);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isDebug(IntPtr l) {
		try {
			System.Boolean v;
			checkType(l,2,out v);
			PLua.isDebug=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_lua(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.lua);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_lua(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			SLua.LuaSvr v;
			checkType(l,2,out v);
			self.lua=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_net(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.net);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_net(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			LNet v;
			checkType(l,2,out v);
			self.net=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_ChatNet(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.ChatNet);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_ChatNet(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			LNet v;
			checkType(l,2,out v);
			self.ChatNet=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_luacache(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,PLua.luacache);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_luacache(IntPtr l) {
		try {
			System.Collections.Generic.Dictionary<System.String,UnityEngine.TextAsset> v;
			checkType(l,2,out v);
			PLua.luacache=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_package_path(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,PLua.package_path);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_updateFn(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.updateFn);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_updateFn(IntPtr l) {
		try {
			PLua self=(PLua)checkSelf(l);
			SLua.LuaFunction v;
			checkType(l,2,out v);
			self.updateFn=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_instance(IntPtr l) {
		try {
			pushValue(l,true);
			pushValue(l,PLua.instance);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"PLua");
		addMember(l,DoUnity3dLua);
		addMember(l,DoMain);
		addMember(l,LoadBundle);
		addMember(l,RegisterFunc);
		addMember(l,Loader_s);
		addMember(l,Log_s);
		addMember(l,Delay_s);
		addMember(l,StopDelay_s);
		addMember(l,"enterLua",get_enterLua,set_enterLua,false);
		addMember(l,"onDestroyFn",get_onDestroyFn,set_onDestroyFn,true);
		addMember(l,"isDebug",get_isDebug,set_isDebug,false);
		addMember(l,"lua",get_lua,set_lua,true);
		addMember(l,"net",get_net,set_net,true);
		addMember(l,"ChatNet",get_ChatNet,set_ChatNet,true);
		addMember(l,"luacache",get_luacache,set_luacache,false);
		addMember(l,"package_path",get_package_path,null,false);
		addMember(l,"updateFn",get_updateFn,set_updateFn,true);
		addMember(l,"instance",get_instance,null,false);
		createTypeMetatable(l,null, typeof(PLua),typeof(UnityEngine.MonoBehaviour));
	}
}
