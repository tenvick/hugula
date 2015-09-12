using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_CRequest : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			CRequest o;
			if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new CRequest(a1);
				pushValue(l,o);
				return 1;
			}
			else if(argc==4){
				System.String a1;
				checkType(l,2,out a1);
				System.String a2;
				checkType(l,3,out a2);
				System.String a3;
				checkType(l,4,out a3);
				o=new CRequest(a1,a2,a3);
				pushValue(l,o);
				return 1;
			}
			LuaDLL.luaL_error(l,"New object failed.");
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
			CRequest self=(CRequest)checkSelf(l);
			self.Dispose();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DispatchComplete(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			self.DispatchComplete();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int DispatchEnd(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			self.DispatchEnd();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_assetType(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.assetType);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_assetType(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.assetType=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_assetBundle(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.assetBundle);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_assetBundle(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			UnityEngine.AssetBundle v;
			checkType(l,2,out v);
			self.assetBundle=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_www(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.www);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_www(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			UnityEngine.WWW v;
			checkType(l,2,out v);
			self.www=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_userData(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.userData);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_userData(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.userData=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_priority(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.priority);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_priority(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.priority=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_times(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.times);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_times(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.times=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_childrenReq(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.childrenReq);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_childrenReq(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			CRequest v;
			checkType(l,2,out v);
			self.childrenReq=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_dependenciesCount(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.dependenciesCount);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_dependenciesCount(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.dependenciesCount=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_suffix(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.suffix);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_suffix(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.suffix=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_assetBundleName(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.assetBundleName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_assetBundleName(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.assetBundleName=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_assetName(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.assetName);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_assetName(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.assetName=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_head(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.head);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_head(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.head=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_data(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.data);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_data(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			System.Object v;
			checkType(l,2,out v);
			self.data=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_isShared(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.isShared);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_isShared(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.isShared=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_url(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.url);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_url(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.url=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_key(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
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
			CRequest self=(CRequest)checkSelf(l);
			string v;
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
	static public int get_udKey(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.udKey);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_udKey(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			string v;
			checkType(l,2,out v);
			self.udKey=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_cache(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			pushValue(l,self.cache);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_cache(IntPtr l) {
		try {
			CRequest self=(CRequest)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.cache=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"CRequest");
		addMember(l,Dispose);
		addMember(l,DispatchComplete);
		addMember(l,DispatchEnd);
		addMember(l,"assetType",get_assetType,set_assetType,true);
		addMember(l,"assetBundle",get_assetBundle,set_assetBundle,true);
		addMember(l,"www",get_www,set_www,true);
		addMember(l,"userData",get_userData,set_userData,true);
		addMember(l,"priority",get_priority,set_priority,true);
		addMember(l,"times",get_times,set_times,true);
		addMember(l,"childrenReq",get_childrenReq,set_childrenReq,true);
		addMember(l,"dependenciesCount",get_dependenciesCount,set_dependenciesCount,true);
		addMember(l,"suffix",get_suffix,set_suffix,true);
		addMember(l,"assetBundleName",get_assetBundleName,set_assetBundleName,true);
		addMember(l,"assetName",get_assetName,set_assetName,true);
		addMember(l,"head",get_head,set_head,true);
		addMember(l,"data",get_data,set_data,true);
		addMember(l,"isShared",get_isShared,set_isShared,true);
		addMember(l,"url",get_url,set_url,true);
		addMember(l,"key",get_key,set_key,true);
		addMember(l,"udKey",get_udKey,set_udKey,true);
		addMember(l,"cache",get_cache,set_cache,true);
		createTypeMetatable(l,constructor, typeof(CRequest));
	}
}
