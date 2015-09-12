using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UIPanelCamackTable : LuaObject {
	static public void reg(IntPtr l) {
		getTypeTable(l,"UIPanelCamackTable");
		createTypeMetatable(l,null, typeof(UIPanelCamackTable),typeof(UnityEngine.MonoBehaviour));
	}
}
