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

        private static BindPathPartGetValue m_GetSourceValue;
        //返回对象的属性值
        public static object GetSourcePropertyValue (object source, BindingPathPart part, bool needSubscribe) {
            if (m_GetSourceValue == null)
                m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<BindPathPartGetValue> ("BindingExpression.get_property");

            return m_GetSourceValue (source, part, needSubscribe);
        }

        // private static BindPathPartGetValue m_GetMethodSourceValue;
        // //获取对象的方法返回值
        // public static object InvokeSourceMethod (object source, BindingPathPart part, bool needSubscribe) {
        //     if (m_GetSourceValue == null)
        //         m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<BindPathPartGetValue> ("BindingExpression.invoke_method");

        //     return m_GetSourceValue (source, part, needSubscribe);
        // }

        #endregion

        #region 获取值并设置到目标对象中
        private static UpdateValue m_UpdateTargetValue;
        //利用lua更新目标属性
        public static void UpdateTargetValue (object target, string property, object source, BindingPathPart part, string format, object converter) {
            if (m_UpdateTargetValue == null)
                m_UpdateTargetValue = EnterLua.luaenv.Global.GetInPath<UpdateValue> ("BindingExpression.update_target");

            // string path = part.path;
            // bool is_index = part.isIndexer;
            // bool is_method = part.isGetter || part.isGetter;
            m_UpdateTargetValue (target, property, source, part, format, converter);
        }

        private static UpdateValue m_UpdateSourceValue;
        //利用lua更新源属性
        public static void UpdateSourceValue (object target, string property, object source, BindingPathPart part, string format, object converter) {
            if (m_UpdateTargetValue == null)
                m_UpdateTargetValue = EnterLua.luaenv.Global.GetInPath<UpdateValue> ("BindingExpression.update_source");

            // string path = part.path;
            // bool is_index = part.isIndexer;
            // bool is_method = part.isGetter || part.isGetter;
            m_UpdateTargetValue (target, property, source, part, format, converter);
        }

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
        public static void ApplyByLua (Binding binding,object source) {
            if (m_ApplyActual == null)
                m_ApplyActual = EnterLua.luaenv.Global.GetInPath<ApplyActual> ("BindingExpression.apply_actual_by_lua");
            // object sourceObject, BindableObject target, string property, List<BindingPathPart> parts, bool needsGetter, bool needsSetter, bool needSubscribe
            // m_ApplyActual (sourceObject, target, property, parts, needsGetter, needsSetter, needSubscribe);
            m_ApplyActual (binding,source);
        }
        #endregion

        public static void Dispose () {
            m_GetSourceValue = null;
            m_ApplyActual = null;
            m_UpdateSourceValue = null;
        }
    }

    public delegate object BindPathPartGetValue (object obj, BindingPathPart property, bool needSubscribe);
    public delegate void UpdateValue (object target, string property, object source, BindingPathPart part, string format, object converter);
    public delegate void ApplyActual (Binding binding,object source);

    public class ExpressionLuaUtility {

        ///<summary>
        /// 
        ///</summary>
        ///<code>
        /// 
        ///local function get_property(source, property, isIndexer)
        ///     if isIndexer then
        ///         property = tonumber(property)
        ///     end
        ///     local val = source[property]
        ///     return val
        ///end
        ///</code>
        public object GetPropertyValue (object source, string property, bool isIndexer) {
            var L = EnterLua.luaenv.rawL;
            var translator = EnterLua.luaenv.translator;
            // LuaAPI.lua_pushvalue(L,)
            translator.Push (L, source);
            LuaAPI.lua_pushstring (L, property);
            // LuaAPI.lua_getfield(L,)
            // if (source is XLua.LuaTable) {
            //     if (((XLua.LuaTable) source).ContainsKey<string> ("PropertyChanged")) {

            //     }
            // }
            // RealStatePtr L = luaEnv.rawL;

            return null;
        }
    }
}