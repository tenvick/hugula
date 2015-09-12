using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_CTransport : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int BeginLoad(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			CRequest a1;
			checkType(l,2,out a1);
			self.BeginLoad(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int RemapVariantName_s(IntPtr l) {
		try {
			System.String a1;
			checkType(l,1,out a1);
			var ret=CTransport.RemapVariantName(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_key(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			pushValue(l,self.key);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_key(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.key=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_m_AssetBundleManifest(IntPtr l) {
		try {
			pushValue(l,CTransport.m_AssetBundleManifest);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_m_AssetBundleManifest(IntPtr l) {
		try {
			UnityEngine.AssetBundleManifest v;
			checkType(l,2,out v);
			CTransport.m_AssetBundleManifest=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_OnProcess(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			System.Action<CTransport,System.Single> v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.OnProcess=v;
			else if(op==1) self.OnProcess+=v;
			else if(op==2) self.OnProcess-=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_OnComplete(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			System.Action<CTransport,CRequest,System.Collections.Generic.IList<CRequest>> v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.OnComplete=v;
			else if(op==1) self.OnComplete+=v;
			else if(op==2) self.OnComplete-=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_OnError(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			System.Action<CTransport,CRequest> v;
			int op=LuaDelegation.checkDelegate(l,2,out v);
			if(op==0) self.OnError=v;
			else if(op==1) self.OnError+=v;
			else if(op==2) self.OnError-=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isFree(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			pushValue(l,self.isFree);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_req(IntPtr l) {
		try {
			CTransport self=(CTransport)checkSelf(l);
			pushValue(l,self.req);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Variants(IntPtr l) {
		try {
			pushValue(l,CTransport.Variants);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_Variants(IntPtr l) {
		try {
			System.String[] v;
			checkType(l,2,out v);
			CTransport.Variants=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"CTransport");
		addMember(l,BeginLoad);
		addMember(l,RemapVariantName_s);
		addMember(l,"key",get_key,set_key,true);
		addMember(l,"m_AssetBundleManifest",get_m_AssetBundleManifest,set_m_AssetBundleManifest,false);
		addMember(l,"OnProcess",null,set_OnProcess,true);
		addMember(l,"OnComplete",null,set_OnComplete,true);
		addMember(l,"OnError",null,set_OnError,true);
		addMember(l,"isFree",get_isFree,null,true);
		addMember(l,"req",get_req,null,true);
		addMember(l,"Variants",get_Variants,set_Variants,false);
		createTypeMetatable(l,null, typeof(CTransport),typeof(UnityEngine.MonoBehaviour));
	}
}
