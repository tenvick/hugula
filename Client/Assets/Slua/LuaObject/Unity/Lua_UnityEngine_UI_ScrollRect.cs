using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_ScrollRect : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Rebuild(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.UI.CanvasUpdate a1;
			checkEnum(l,2,out a1);
			self.Rebuild(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int IsActive(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			var ret=self.IsActive();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int StopMovement(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			self.StopMovement();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnScroll(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnScroll(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnInitializePotentialDrag(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnInitializePotentialDrag(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnBeginDrag(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnBeginDrag(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnEndDrag(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnEndDrag(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int OnDrag(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.EventSystems.PointerEventData a1;
			checkType(l,2,out a1);
			self.OnDrag(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_content(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.content);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_content(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.RectTransform v;
			checkType(l,2,out v);
			self.content=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontal(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.horizontal);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_horizontal(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.horizontal=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_vertical(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.vertical);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_vertical(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.vertical=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_movementType(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushEnum(l,(int)self.movementType);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_movementType(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.UI.ScrollRect.MovementType v;
			checkEnum(l,2,out v);
			self.movementType=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_elasticity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.elasticity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_elasticity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.elasticity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_inertia(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.inertia);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_inertia(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			bool v;
			checkType(l,2,out v);
			self.inertia=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_decelerationRate(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.decelerationRate);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_decelerationRate(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.decelerationRate=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_scrollSensitivity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.scrollSensitivity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_scrollSensitivity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.scrollSensitivity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontalScrollbar(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.horizontalScrollbar);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_horizontalScrollbar(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.UI.Scrollbar v;
			checkType(l,2,out v);
			self.horizontalScrollbar=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_verticalScrollbar(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.verticalScrollbar);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_verticalScrollbar(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.UI.Scrollbar v;
			checkType(l,2,out v);
			self.verticalScrollbar=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_onValueChanged(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.onValueChanged);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_onValueChanged(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.UI.ScrollRect.ScrollRectEvent v;
			checkType(l,2,out v);
			self.onValueChanged=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_velocity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.velocity);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_velocity(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.velocity=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_normalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.normalizedPosition);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_normalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.normalizedPosition=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_horizontalNormalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.horizontalNormalizedPosition);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_horizontalNormalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.horizontalNormalizedPosition=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_verticalNormalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			pushValue(l,self.verticalNormalizedPosition);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_verticalNormalizedPosition(IntPtr l) {
		try {
			UnityEngine.UI.ScrollRect self=(UnityEngine.UI.ScrollRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.verticalNormalizedPosition=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"UnityEngine.UI.ScrollRect");
		addMember(l,Rebuild);
		addMember(l,IsActive);
		addMember(l,StopMovement);
		addMember(l,OnScroll);
		addMember(l,OnInitializePotentialDrag);
		addMember(l,OnBeginDrag);
		addMember(l,OnEndDrag);
		addMember(l,OnDrag);
		addMember(l,"content",get_content,set_content,true);
		addMember(l,"horizontal",get_horizontal,set_horizontal,true);
		addMember(l,"vertical",get_vertical,set_vertical,true);
		addMember(l,"movementType",get_movementType,set_movementType,true);
		addMember(l,"elasticity",get_elasticity,set_elasticity,true);
		addMember(l,"inertia",get_inertia,set_inertia,true);
		addMember(l,"decelerationRate",get_decelerationRate,set_decelerationRate,true);
		addMember(l,"scrollSensitivity",get_scrollSensitivity,set_scrollSensitivity,true);
		addMember(l,"horizontalScrollbar",get_horizontalScrollbar,set_horizontalScrollbar,true);
		addMember(l,"verticalScrollbar",get_verticalScrollbar,set_verticalScrollbar,true);
		addMember(l,"onValueChanged",get_onValueChanged,set_onValueChanged,true);
		addMember(l,"velocity",get_velocity,set_velocity,true);
		addMember(l,"normalizedPosition",get_normalizedPosition,set_normalizedPosition,true);
		addMember(l,"horizontalNormalizedPosition",get_horizontalNormalizedPosition,set_horizontalNormalizedPosition,true);
		addMember(l,"verticalNormalizedPosition",get_verticalNormalizedPosition,set_verticalNormalizedPosition,true);
		createTypeMetatable(l,null, typeof(UnityEngine.UI.ScrollRect),typeof(UnityEngine.EventSystems.UIBehaviour));
	}
}
