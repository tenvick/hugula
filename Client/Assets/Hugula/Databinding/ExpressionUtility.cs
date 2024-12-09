using System;
using System.Collections.Generic;
using XLua;
using LuaAPI = XLua.LuaDLL.Lua;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;

using Hugula.Framework;
using System.Diagnostics;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class ExpressionUtility : Singleton<ExpressionUtility>, IDisposable
    {

        public void NewInitialize(LuaGetSourceInvoke m_GetSourcePropertyInvoke, LuaGetSourceInvoke m_GetSourceMethodInvoke,
        LuaSetSourceInvoke m_SetSourcePropertyInvoke, LuaSetSourceInvoke m_SetSourceMethodInvoke,
        LuaSetTargetInvoke m_SetTargetPropertyInvoke, LuaSetTargetInvoke m_SetTargetPropertyNoConvertInvoke, LuaSetTargetInvoke m_SetTargetMethodInvoke
        )
        {
            this.m_GetSourcePropertyInvoke = m_GetSourcePropertyInvoke;
            this.m_GetSourceMethodInvoke = m_GetSourceMethodInvoke;
            this.m_SetSourcePropertyInvoke = m_SetSourcePropertyInvoke;
            this.m_SetSourceMethodInvoke = m_SetSourceMethodInvoke;
            this.m_SetTargetPropertyInvoke = m_SetTargetPropertyInvoke;
            this.m_SetTargetPropertyNoConvertInvoke = m_SetTargetPropertyNoConvertInvoke;
            this.m_SetTargetMethodInvoke = m_SetTargetMethodInvoke;
        }


        #region new lua invoke
        internal LuaGetSourceInvoke m_GetSourcePropertyInvoke;
        internal LuaGetSourceInvoke m_GetSourceMethodInvoke;
        internal LuaSetSourceInvoke m_SetSourcePropertyInvoke;
        internal LuaSetSourceInvoke m_SetSourceMethodInvoke;

        /// <summary>
        /// no convert
        /// </summary>
        internal LuaSetTargetInvoke m_SetTargetPropertyInvoke;
        /// <summary>
        /// no format and no convert
        /// </summary>
        internal LuaSetTargetInvoke m_SetTargetPropertyNoConvertInvoke;

        public LuaSetTargetInvoke m_SetTargetMethodInvoke;


        internal static object GetSourceInvoke(object obj, string path, bool isMethod, bool isIndexer)
        {
            if (isMethod)
            {
                return instance?.m_GetSourceMethodInvoke(obj, path, isIndexer);
            }
            else
            {
                return instance?.m_GetSourcePropertyInvoke(obj, path, isIndexer);
            }
        }

        internal static object SetSourceInvoke(object obj, BindingPathPartConfig part, Binding binding)
        {
            var converter = binding?.converter;
            object convert = null;
            if (!string.IsNullOrEmpty(converter))
                convert = ValueConverterRegister.instance?.Get(converter);

            if (part.isMethod)
            {
                instance?.m_SetSourceMethodInvoke(obj, binding.target, part.path, binding.propertyName, part.isIndexer, convert);
            }
            else
            {
                instance?.m_SetSourcePropertyInvoke(obj, binding.target, part.path, binding.propertyName, part.isIndexer, convert);
            }

            return null;
        }

        internal static void SetTargetInvoke(object source, object target, string property, ref BindingPathPartConfig part, string converter)
        {
            try
            {
                string path = part.path;
                bool is_indexer = part.isIndexer;
                bool is_self = part.isSelf;
                bool is_method = part.isMethod;

                object convert = null;
                if (!string.IsNullOrEmpty(converter))
                    convert = ValueConverterRegister.instance?.Get(converter);

                if (!is_method && convert == null)
                {
                    instance?.m_SetTargetPropertyNoConvertInvoke(source, target, path, property, is_indexer, is_self, convert); //object source, string path,object target,string property, bool is_indexer,bool is_self,object converter);
                }
                else if (is_method)
                {
                    instance?.m_SetTargetMethodInvoke(source, target, path, property, is_indexer, is_self, convert);
                }
                else
                {
                    instance?.m_SetTargetPropertyInvoke(source, target, path, property, is_indexer, is_self, convert);
                }
            }
            catch (Exception ex)
            {
                string fullPath = string.Empty;

                if (target is UnityEngine.MonoBehaviour cmp)
                {
                    fullPath = Hugula.Utils.CUtils.GetFullPath(cmp);
                }
                else if (target is UnityEngine.GameObject go)
                {
                    fullPath = Hugula.Utils.CUtils.GetGameObjectFullPath(go) + ":" + target.ToString();
                }

                UnityEngine.Debug.LogError($"Update target.property [{fullPath}].[{property}] error. \r\nC#:{ex.ToString()}");
            }

        }

        #endregion

        public override void Reset()
        {

            m_GetSourcePropertyInvoke = null;
            m_GetSourceMethodInvoke = null;
            m_SetSourcePropertyInvoke = null;
            m_SetSourceMethodInvoke = null;
            m_SetTargetPropertyInvoke = null;
            m_SetTargetPropertyNoConvertInvoke = null;
            m_SetTargetMethodInvoke = null;
        }

        public override void Dispose()
        {
            base.Dispose();

            m_GetSourcePropertyInvoke = null;
            m_GetSourceMethodInvoke = null;
            m_SetSourcePropertyInvoke = null;
            m_SetSourceMethodInvoke = null;
            m_SetTargetPropertyInvoke = null;
            m_SetTargetPropertyNoConvertInvoke = null;
            m_SetTargetMethodInvoke = null;
        }
    }

    // public delegate object LuaInvoke(object obj, BindingPathPartConfig part);

    public delegate object LuaGetSourceInvoke(object source, string path, bool is_indexer);

    public delegate object LuaSetSourceInvoke(object source, object target, string path, string property, bool is_indexer, object converter);

    public delegate void LuaSetTargetInvoke(object source, object target, string path, string property, bool is_indexer, bool is_self, object converter);

}