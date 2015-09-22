using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_CHighway : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			CHighway o;
			o=new CHighway();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int LoadReq(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(matchType(l,argc,2,typeof(CRequest))){
				CHighway self=(CHighway)checkSelf(l);
				CRequest a1;
				checkType(l,2,out a1);
				self.LoadReq(a1);
				pushValue(l,true);
				return 1;
			}
			else if(matchType(l,argc,2,typeof(IList<CRequest>))){
				CHighway self=(CHighway)checkSelf(l);
				System.Collections.Generic.IList<CRequest> a1;
				checkType(l,2,out a1);
				self.LoadReq(a1);
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
	static public int InitProgressState(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			self.InitProgressState();
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int SetReqDataFromData_s(IntPtr l) {
		try {
			CRequest a1;
			checkType(l,1,out a1);
			System.Object a2;
			checkType(l,2,out a2);
			CHighway.SetReqDataFromData(a1,a2);
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int GetInstance_s(IntPtr l) {
		try {
			var ret=CHighway.GetInstance();
			pushValue(l,true);
			pushValue(l,ret);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_currentLoading(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.currentLoading);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_maxLoading(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.maxLoading);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_totalLoading(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.totalLoading);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_totalLoading(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.totalLoading=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_currentLoaded(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.currentLoaded);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cache(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			pushValue(l,true);
			pushValue(l,self.cache);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cache(IntPtr l) {
		try {
			CHighway self=(CHighway)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.cache=v;
			pushValue(l,true);
			return 1;
		}
		catch(Exception e) {
			return error(l,e);
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"CHighway");
		addMember(l,LoadReq);
		addMember(l,InitProgressState);
		addMember(l,SetReqDataFromData_s);
		addMember(l,GetInstance_s);
		addMember(l,"currentLoading",get_currentLoading,null,true);
		addMember(l,"maxLoading",get_maxLoading,null,true);
		addMember(l,"totalLoading",get_totalLoading,set_totalLoading,true);
		addMember(l,"currentLoaded",get_currentLoaded,null,true);
		addMember(l,"cache",get_cache,set_cache,true);
		createTypeMetatable(l,constructor, typeof(CHighway));
	}
}
