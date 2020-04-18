using System;
using System.Collections.Generic;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;
using Hugula.Framework;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class ExpressionUtility : Singleton<ExpressionUtility>, IDisposable
    {
        public void Initialize()
        {
            if (m_GetSourceValue == null)
                m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<BindPathPartGetValue>("BindingExpression.get_property");
            if (m_UpdateTargetValue == null)
                m_UpdateTargetValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_target");
            if (m_UpdateSourceValue == null)
                m_UpdateSourceValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_source");
        }

        #region  获取source的值

        private BindPathPartGetValue m_GetSourceValue;
        //返回对象的属性值
        public static object GetSourcePropertyValue(object source, BindingPathPart part, bool needSubscribe)
        {
            if (instance.m_GetSourceValue == null)
                instance.m_GetSourceValue = EnterLua.luaenv.Global.GetInPath<BindPathPartGetValue>("BindingExpression.get_property");

            return instance.m_GetSourceValue(source, part, needSubscribe);
        }


        #endregion

        #region 获取值并设置到目标对象中
        private UpdateValue m_UpdateTargetValue;
        //利用lua更新目标属性
        public static void UpdateTargetValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            if (instance.m_UpdateTargetValue == null)
                instance.m_UpdateTargetValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_target");

            instance.m_UpdateTargetValue(target, property, source, part, format, converter);
        }

        private UpdateValue m_UpdateSourceValue;
        //利用lua更新源属性
        public static void UpdateSourceValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            if (instance.m_UpdateSourceValue == null)
                instance.m_UpdateSourceValue = EnterLua.luaenv.Global.GetInPath<UpdateValue>("BindingExpression.update_source");

            instance.m_UpdateSourceValue(target, property, source, part, format, converter);
        }

        #endregion



        #region  lua table 寻值
        private ApplyActual m_ApplyActual;
        public static void ApplyByLua(Binding binding, object source)
        {
            if (instance.m_ApplyActual == null)
                instance.m_ApplyActual = EnterLua.luaenv.Global.GetInPath<ApplyActual>("BindingExpression.apply_actual_by_lua");
            instance.m_ApplyActual(binding, source);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            m_GetSourceValue = null;
            m_ApplyActual = null;
            m_UpdateSourceValue = null;
        }
    }

    public delegate object BindPathPartGetValue(object obj, BindingPathPart property, bool needSubscribe);
    public delegate void UpdateValue(object target, string property, object source, BindingPathPart part, string format, object converter);
    public delegate void ApplyActual(Binding binding, object source);

}