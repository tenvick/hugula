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
        public void Initialize(BindPathPartGetValueUnpack getSourceValue, UpdateValueUnpack updateTargetValueUnpack, UpdateValueUnpack UpdateSourceValueUnpack)
        {
            m_GetSourceValue = getSourceValue;
            m_UpdateTargetValueUnpack = updateTargetValueUnpack;
            m_UpdateSourceValueUnpack = UpdateSourceValueUnpack;
        }

        #region  获取source的值

        private BindPathPartGetValueUnpack m_GetSourceValue;
        //返回对象的属性值
        public static object GetSourcePropertyValue(object source, BindingPathPart part, bool needSubscribe)
        {
            return instance.m_GetSourceValue(source, part, part.path, part.isSelf, part.isMethod, part.isIndexer, needSubscribe);
        }


        #endregion

        #region 获取值并设置到目标对象中

        private UpdateValueUnpack m_UpdateTargetValueUnpack;
        //object target, string property, object source, string path, bool isself, bool isMethod, bool is_index, string format, object converter
        public static void UpdateTargetValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            instance.m_UpdateTargetValueUnpack(target, property, part.source, part.path, part.isSelf, part.isMethod, part.isIndexer, format, converter);
        }

        private UpdateValueUnpack m_UpdateSourceValueUnpack;
        public static void UpdateSourceValue(object target, string property, object source, BindingPathPart part, string format, object converter)
        {
            instance.m_UpdateSourceValueUnpack(target, property, part.source, part.path, part.isSelf, part.isMethod, part.isIndexer, format, converter);
        }

        #endregion

        public override void Reset()
        {
            m_GetSourceValue = null;
            m_UpdateTargetValueUnpack = null;
            m_UpdateSourceValueUnpack = null;

        }

        public override void Dispose()
        {
            base.Dispose();
            m_GetSourceValue = null;
            m_UpdateTargetValueUnpack = null;
            m_UpdateSourceValueUnpack = null;
        }
    }

    public delegate object BindPathPartGetValueUnpack(object obj, BindingPathPart part, string path, bool isSelf, bool isMethod, bool isIndex, bool needSubscribe);

    public delegate void UpdateValueUnpack(object target, string property, object source, string path, bool isSelf, bool isMethod, bool isIndex, string format, object converter);


}