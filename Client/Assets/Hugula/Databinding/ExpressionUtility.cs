using System;
using System.Collections.Generic;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;

namespace Hugula.Databinding {

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public class ExpressionUtility {

		#region lua binding expression
		// [XLua.CSharpCallLua]
		// public delegate IBindingExpression BindingNew (Binding binding);
		// private static BindingNew bindingNew;

		// public static IBindingExpression NewExpression (Binding binding) {
		// 	// if (bindingNew == null) {
		// 	// 	bindingNew = EnterLua.luaenv.Global.GetInPath<BindingNew> ("BindingExpression.New");
		// 	// }
		// 	// return bindingNew (binding);
		// 	return new BindingExpression (binding);
		// }
		#endregion

		#region  获取source的值

		private static GetSourceValue m_GetSourceValue;
		//返回对象的属性值
		public static object GetSourcePropertyValue (object source, string property) {
			if (m_GetSourceValue == null)
				m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<GetSourceValue> ("BindingExpression.get_property");

			return m_GetSourceValue (source, property);
		}

		private static GetSourceValue m_GetMethodSourceValue;
		//获取对象的方法返回值
		public static object InvokeSourceMethod (object source, string property) {
			if (m_GetSourceValue == null)
				m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<GetSourceValue> ("BindingExpression.invoke_method");

			return m_GetSourceValue (source, property);
		}

		#endregion

		#region 获取值并设置到目标对象中
		// private static BindingValue m_SetBindingValue;
		// //返回对象的属性值
		// public static void SetTargetBindingValue (object target, object source, object current, BindingPathPart part) {
		// 	if (m_SetBindingValue == null)
		// 		m_SetBindingValue = EnterLua.luaenv.Global.GetInPath<BindingValue> ("BindingExpression.set_target_value");

		// 	m_SetBindingValue (target, source, current, part);
		// }

		// private static BindingValue m_SetSourceBindingValue;
		// //返回对象的属性值
		// public static void SetSourceBindingValue (object target, object source, object current, BindingPathPart part) {
		// 	if (m_SetSourceBindingValue == null)
		// 		m_SetSourceBindingValue = EnterLua.luaenv.Global.GetInPath<BindingValue> ("BindingExpression.set_source_value");

		// 	m_SetSourceBindingValue (target, source, current, part);
		// }

		#endregion

		#region  lua table的Notify通知注册与移除
		// public static void AddLuaTablePropertyChanged (XLua.LuaTable source, object value) {
		// 	var luaEnv = source.GetLuaEnv ();
		// 	var L = luaEnv.L;
		// 	int err_func = LuaAPI.load_error_func (L, luaEnv.errorFuncRef);
		// 	ObjectTranslator translator = luaEnv.translator;

		// 	LuaAPI.lua_getref (L, source.GetLuaReference ());
		// 	LuaAPI.xlua_pushasciistring (L, "add_PropertyChanged");
		// 	if (0 != LuaAPI.xlua_pgettable (L, -2)) {
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);
		// 	}
		// 	if (!LuaAPI.lua_isfunction (L, -1)) {
		// 		LuaAPI.xlua_pushasciistring (L, "no such function add_PropertyChanged");
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);
		// 	}
		// 	LuaAPI.lua_pushvalue (L, -2);
		// 	LuaAPI.lua_remove (L, -3);
		// 	translator.Push (L, value);

		// 	int __gen_error = LuaAPI.lua_pcall (L, 2, 0, err_func);
		// 	if (__gen_error != 0)
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);

		// 	LuaAPI.lua_settop (L, err_func - 1);
		// }

		// public static void RemoveLuaTablePropertyChanged (XLua.LuaTable source, object value) {
		// 	var luaEnv = source.GetLuaEnv ();
		// 	var L = luaEnv.L;
		// 	int err_func = LuaAPI.load_error_func (L, luaEnv.errorFuncRef);
		// 	ObjectTranslator translator = luaEnv.translator;

		// 	LuaAPI.lua_getref (L, source.GetLuaReference ());
		// 	LuaAPI.xlua_pushasciistring (L, "remove_PropertyChanged");
		// 	if (0 != LuaAPI.xlua_pgettable (L, -2)) {
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);
		// 	}
		// 	if (!LuaAPI.lua_isfunction (L, -1)) {
		// 		LuaAPI.xlua_pushasciistring (L, "no such function remove_PropertyChanged");
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);
		// 	}
		// 	LuaAPI.lua_pushvalue (L, -2);
		// 	LuaAPI.lua_remove (L, -3);
		// 	translator.Push (L, value);

		// 	int __gen_error = LuaAPI.lua_pcall (L, 2, 0, err_func);
		// 	if (__gen_error != 0)
		// 		luaEnv.ThrowExceptionFromError (err_func - 1);

		// 	LuaAPI.lua_settop (L, err_func - 1);
		// }

		#endregion

		#region  lua table 寻值
		private static ApplyActual m_ApplyActual;
		public static void ApplyLuaTable (object sourceObject, BindableObject target, string property, List<BindingPathPart> parts, bool needsGetter, bool needsSetter, bool needSubscribe) {
			if (m_ApplyActual == null)
				m_ApplyActual = EnterLua.luaenv.Global.GetInPath<ApplyActual> ("BindingExpression.apply_actual_by_lua");

			m_ApplyActual (sourceObject, target, property, parts, needsGetter, needsSetter, needSubscribe);
		}
		#endregion

		public static void Dispose () {
			// bindingNew = null;
			m_GetSourceValue = null;
			m_GetMethodSourceValue = null;
			m_ApplyActual = null;
		}
	}

	public delegate object GetSourceValue (object obj, string property);
	public delegate void BindingValue (object target, object source, object current, BindingPathPart part);
	public delegate void ApplyActual (object sourceObject, BindableObject target, string property, List<BindingPathPart> parts, bool needsGetter, bool needsSetter, bool needSubscribe);

}