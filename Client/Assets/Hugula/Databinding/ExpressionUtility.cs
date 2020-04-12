using System;
using System.Collections.Generic;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class ExpressionUtility
    {

        #region  获取source的值

        private static BindPathPartGetValue m_GetSourceValue;
        //返回对象的属性值
        public static object GetSourcePropertyValue(object source, BindingPathPart part, bool needSubscribe)
        {
            if (m_GetSourceValue == null)
                m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<BindPathPartGetValue>("BindingExpression.get_property");

            return m_GetSourceValue(source, part, needSubscribe);
        }


        #endregion

        #region 获取值并设置到目标对象中
        private static UpdateValue m_UpdateTargetValue;
        //利用lua更新目标属性
        public static void UpdateTargetValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            if (m_UpdateTargetValue == null)
                m_UpdateTargetValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_target");

            m_UpdateTargetValue(target, property, source, part, format, converter);
        }

        private static UpdateValue m_UpdateSourceValue;
        //利用lua更新源属性
        public static void UpdateSourceValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            if (m_UpdateSourceValue == null)
                m_UpdateSourceValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_source");

            m_UpdateSourceValue(target, property, source, part, format, converter);
        }

        #endregion



        #region  lua table 寻值
        private static ApplyActual m_ApplyActual;
        public static void ApplyByLua(Binding binding, object source)
        {
            if (m_ApplyActual == null)
                m_ApplyActual = EnterLua.luaenv.Global.GetInPath<ApplyActual>("BindingExpression.apply_actual_by_lua");
            m_ApplyActual(binding, source);
        }
        #endregion

        public static void Dispose()
        {
            m_GetSourceValue = null;
            m_ApplyActual = null;
            m_UpdateSourceValue = null;
        }
    }

    public delegate object BindPathPartGetValue(object obj, BindingPathPart property, bool needSubscribe);
    public delegate void UpdateValue(object target, string property, object source, BindingPathPart part, string format, object converter);
    public delegate void ApplyActual(Binding binding, object source);

}