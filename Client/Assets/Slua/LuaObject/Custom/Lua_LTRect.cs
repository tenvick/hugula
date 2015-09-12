using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_LTRect : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			LTRect o;
			if(argc==1){
				o=new LTRect();
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				UnityEngine.Rect a1;
				checkValueType(l,2,out a1);
				o=new LTRect(a1);
				pushValue(l,o);
				return 1;
			}
			else if(argc==5){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				System.Single a3;
				checkType(l,4,out a3);
				System.Single a4;
				checkType(l,5,out a4);
				o=new LTRect(a1,a2,a3,a4);
				pushValue(l,o);
				return 1;
			}
			else if(argc==6){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				System.Single a3;
				checkType(l,4,out a3);
				System.Single a4;
				checkType(l,5,out a4);
				System.Single a5;
				checkType(l,6,out a5);
				o=new LTRect(a1,a2,a3,a4,a5);
				pushValue(l,o);
				return 1;
			}
			else if(argc==7){
				System.Single a1;
				checkType(l,2,out a1);
				System.Single a2;
				checkType(l,3,out a2);
				System.Single a3;
				checkType(l,4,out a3);
				System.Single a4;
				checkType(l,5,out a4);
				System.Single a5;
				checkType(l,6,out a5);
				System.Single a6;
				checkType(l,7,out a6);
				o=new LTRect(a1,a2,a3,a4,a5,a6);
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
	static public int setId(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			System.Int32 a2;
			checkType(l,3,out a2);
			self.setId(a1,a2);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int reset(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			self.reset();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int resetForRotation(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			self.resetForRotation();
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setStyle(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.GUIStyle a1;
			checkType(l,2,out a1);
			var ret=self.setStyle(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setFontScaleToFit(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean a1;
			checkType(l,2,out a1);
			var ret=self.setFontScaleToFit(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setColor(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Color a1;
			checkType(l,2,out a1);
			var ret=self.setColor(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setAlpha(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			var ret=self.setAlpha(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setLabel(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			var ret=self.setLabel(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setUseSimpleScale(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			if(argc==2){
				LTRect self=(LTRect)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				var ret=self.setUseSimpleScale(a1);
				pushValue(l,ret);
				return 1;
			}
			else if(argc==3){
				LTRect self=(LTRect)checkSelf(l);
				System.Boolean a1;
				checkType(l,2,out a1);
				UnityEngine.Rect a2;
				checkValueType(l,3,out a2);
				var ret=self.setUseSimpleScale(a1,a2);
				pushValue(l,ret);
				return 1;
			}
			LuaDLL.luaL_error(l,"No matched override function to call");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int setSizeByHeight(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean a1;
			checkType(l,2,out a1);
			var ret=self.setSizeByHeight(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get__rect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self._rect);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set__rect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Rect v;
			checkValueType(l,2,out v);
			self._rect=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_alpha(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.alpha);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_alpha(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.alpha=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotation(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.rotation);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rotation(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Single v;
			checkType(l,2,out v);
			self.rotation=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_pivot(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.pivot);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_pivot(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.pivot=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_margin(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.margin);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_margin(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Vector2 v;
			checkType(l,2,out v);
			self.margin=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_relativeRect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.relativeRect);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_relativeRect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Rect v;
			checkValueType(l,2,out v);
			self.relativeRect=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotateEnabled(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.rotateEnabled);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rotateEnabled(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.rotateEnabled=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rotateFinished(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.rotateFinished);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rotateFinished(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.rotateFinished=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_alphaEnabled(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.alphaEnabled);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_alphaEnabled(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.alphaEnabled=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_labelStr(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.labelStr);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_labelStr(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.String v;
			checkType(l,2,out v);
			self.labelStr=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_type(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushEnum(l,(int)self.type);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_type(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			LTGUI.Element_Type v;
			checkEnum(l,2,out v);
			self.type=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_style(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.style);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_style(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.GUIStyle v;
			checkType(l,2,out v);
			self.style=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useColor(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.useColor);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useColor(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.useColor=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_color(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.color);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_color(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Color v;
			checkType(l,2,out v);
			self.color=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_fontScaleToFit(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.fontScaleToFit);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_fontScaleToFit(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.fontScaleToFit=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_useSimpleScale(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.useSimpleScale);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_useSimpleScale(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.useSimpleScale=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_sizeByHeight(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.sizeByHeight);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_sizeByHeight(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Boolean v;
			checkType(l,2,out v);
			self.sizeByHeight=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_texture(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.texture);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_texture(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Texture v;
			checkType(l,2,out v);
			self.texture=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_counter(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.counter);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_counter(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			System.Int32 v;
			checkType(l,2,out v);
			self.counter=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_colorTouched(IntPtr l) {
		try {
			pushValue(l,LTRect.colorTouched);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_colorTouched(IntPtr l) {
		try {
			System.Boolean v;
			checkType(l,2,out v);
			LTRect.colorTouched=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_hasInitiliazed(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.hasInitiliazed);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_id(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.id);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_x(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.x);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_x(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.x=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_y(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.y);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_y(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.y=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_width(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.width);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_width(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.width=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_height(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.height);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_height(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			float v;
			checkType(l,2,out v);
			self.height=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_rect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			pushValue(l,self.rect);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_rect(IntPtr l) {
		try {
			LTRect self=(LTRect)checkSelf(l);
			UnityEngine.Rect v;
			checkValueType(l,2,out v);
			self.rect=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"LTRect");
		addMember(l,setId);
		addMember(l,reset);
		addMember(l,resetForRotation);
		addMember(l,setStyle);
		addMember(l,setFontScaleToFit);
		addMember(l,setColor);
		addMember(l,setAlpha);
		addMember(l,setLabel);
		addMember(l,setUseSimpleScale);
		addMember(l,setSizeByHeight);
		addMember(l,"_rect",get__rect,set__rect,true);
		addMember(l,"alpha",get_alpha,set_alpha,true);
		addMember(l,"rotation",get_rotation,set_rotation,true);
		addMember(l,"pivot",get_pivot,set_pivot,true);
		addMember(l,"margin",get_margin,set_margin,true);
		addMember(l,"relativeRect",get_relativeRect,set_relativeRect,true);
		addMember(l,"rotateEnabled",get_rotateEnabled,set_rotateEnabled,true);
		addMember(l,"rotateFinished",get_rotateFinished,set_rotateFinished,true);
		addMember(l,"alphaEnabled",get_alphaEnabled,set_alphaEnabled,true);
		addMember(l,"labelStr",get_labelStr,set_labelStr,true);
		addMember(l,"type",get_type,set_type,true);
		addMember(l,"style",get_style,set_style,true);
		addMember(l,"useColor",get_useColor,set_useColor,true);
		addMember(l,"color",get_color,set_color,true);
		addMember(l,"fontScaleToFit",get_fontScaleToFit,set_fontScaleToFit,true);
		addMember(l,"useSimpleScale",get_useSimpleScale,set_useSimpleScale,true);
		addMember(l,"sizeByHeight",get_sizeByHeight,set_sizeByHeight,true);
		addMember(l,"texture",get_texture,set_texture,true);
		addMember(l,"counter",get_counter,set_counter,true);
		addMember(l,"colorTouched",get_colorTouched,set_colorTouched,false);
		addMember(l,"hasInitiliazed",get_hasInitiliazed,null,true);
		addMember(l,"id",get_id,null,true);
		addMember(l,"x",get_x,set_x,true);
		addMember(l,"y",get_y,set_y,true);
		addMember(l,"width",get_width,set_width,true);
		addMember(l,"height",get_height,set_height,true);
		addMember(l,"rect",get_rect,set_rect,true);
		createTypeMetatable(l,constructor, typeof(LTRect));
	}
}
