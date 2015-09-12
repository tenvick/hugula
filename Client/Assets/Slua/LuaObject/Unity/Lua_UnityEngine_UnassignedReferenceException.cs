using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UnassignedReferenceException : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			UnityEngine.UnassignedReferenceException o;
			if(argc==1){
				o=new UnityEngine.UnassignedReferenceException();
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				System.String a1;
				checkType(l,2,out a1);
				o=new UnityEngine.UnassignedReferenceException(a1);
				pushValue(l,o);
				return 1;
			}
			else if(argc==3){
				System.String a1;
				checkType(l,2,out a1);
				System.Exception a2;
				checkType(l,3,out a2);
				o=new UnityEngine.UnassignedReferenceException(a1,a2);
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
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UnassignedReferenceException");
		createTypeMetatable(l,constructor, typeof(UnityEngine.UnassignedReferenceException),typeof(System.SystemException));
	}
}
