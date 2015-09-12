using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_TweenAction : LuaObject {
	static public void reg(IntPtr l) {
		getEnumTable(l,"TweenAction");
		addMember(l,0,"MOVE_X");
		addMember(l,1,"MOVE_Y");
		addMember(l,2,"MOVE_Z");
		addMember(l,3,"MOVE_LOCAL_X");
		addMember(l,4,"MOVE_LOCAL_Y");
		addMember(l,5,"MOVE_LOCAL_Z");
		addMember(l,6,"MOVE_CURVED");
		addMember(l,7,"MOVE_CURVED_LOCAL");
		addMember(l,8,"MOVE_SPLINE");
		addMember(l,9,"MOVE_SPLINE_LOCAL");
		addMember(l,10,"SCALE_X");
		addMember(l,11,"SCALE_Y");
		addMember(l,12,"SCALE_Z");
		addMember(l,13,"ROTATE_X");
		addMember(l,14,"ROTATE_Y");
		addMember(l,15,"ROTATE_Z");
		addMember(l,16,"ROTATE_AROUND");
		addMember(l,17,"ROTATE_AROUND_LOCAL");
		addMember(l,18,"ALPHA");
		addMember(l,19,"ALPHA_VERTEX");
		addMember(l,20,"COLOR");
		addMember(l,21,"CALLBACK_COLOR");
		addMember(l,22,"CALLBACK");
		addMember(l,23,"MOVE");
		addMember(l,24,"MOVE_LOCAL");
		addMember(l,25,"ROTATE");
		addMember(l,26,"ROTATE_LOCAL");
		addMember(l,27,"SCALE");
		addMember(l,28,"VALUE3");
		addMember(l,29,"GUI_MOVE");
		addMember(l,30,"GUI_MOVE_MARGIN");
		addMember(l,31,"GUI_SCALE");
		addMember(l,32,"GUI_ALPHA");
		addMember(l,33,"GUI_ROTATE");
		addMember(l,34,"DELAYED_SOUND");
		LuaDLL.lua_pop(l, 1);
	}
}
