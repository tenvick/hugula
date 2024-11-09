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
        LuaSetTargetInvoke m_SetTargetPropertyInvoke, LuaSetTargetInvoke m_SetTargetPropertyNoFormatInvoke, LuaSetTargetInvoke m_SetTargetMethodInvoke,
        LuaInvoke m_PartSubscribe)
        {
            this.m_GetSourcePropertyInvoke = m_GetSourcePropertyInvoke;
            this.m_GetSourceMethodInvoke = m_GetSourceMethodInvoke;
            this.m_SetSourcePropertyInvoke = m_SetSourcePropertyInvoke;
            this.m_SetSourceMethodInvoke = m_SetSourceMethodInvoke;
            this.m_SetTargetPropertyInvoke = m_SetTargetPropertyInvoke;
            this.m_SetTargetPropertyNoFormatInvoke = m_SetTargetPropertyNoFormatInvoke;
            this.m_SetTargetMethodInvoke = m_SetTargetMethodInvoke;
            this.m_PartSubscribe = m_PartSubscribe;
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
        internal LuaSetTargetInvoke m_SetTargetPropertyNoFormatInvoke;

        public LuaSetTargetInvoke m_SetTargetMethodInvoke;

        public LuaInvoke m_PartSubscribe;


        internal static object GetSourceInvoke(object obj, BindingPathPart part)
        {
            if (part.isMethod)
            {
                return instance?.m_GetSourceMethodInvoke(obj, part.path, part.isIndexer);
            }
            else
            {
                return instance?.m_GetSourcePropertyInvoke(obj, part.path, part.isIndexer);
            }
        }

        internal static object SetSourceInvoke(object obj, BindingPathPart part)
        {
            var m_Binding = part.m_Binding;
            if (part.isMethod)
            {
                instance?.m_SetSourceMethodInvoke(obj, part.path, m_Binding.target, m_Binding.propertyName, part.isIndexer, m_Binding.convert);
            }
            else
            {
                instance?.m_SetSourcePropertyInvoke(obj, part.path, m_Binding.target, m_Binding.propertyName, part.isIndexer, m_Binding.convert);
            }

            return null;
        }

        internal static void SetTargetInvoke(object source, object target, string property, BindingPathPart part)
        {
            try
            {
                string path = part.path;
                bool is_indexer = part.isIndexer;
                bool is_self = part.isSelf;
                bool is_method = part.isMethod;
                string format = part.m_Binding?.format ?? string.Empty;
                object convert = part.m_Binding?.convert;

                if (!is_method && string.IsNullOrEmpty(format) && convert == null)
                {
                    instance?.m_SetTargetPropertyNoFormatInvoke(source, path, target, property, is_indexer, is_self, format, convert); //object source, string path,object target,string property, bool is_indexer,bool is_self,object converter);
                }
                else if (is_method)
                {
                    instance?.m_SetTargetMethodInvoke(source, path, target, property, is_indexer, is_self, format, convert);
                }
                else
                {
                    instance?.m_SetTargetPropertyInvoke(source, path, target, property, is_indexer, is_self, format, convert);
                }
            }
            catch (Exception ex)
            {
                string fullPath = target.ToString();
                if (target is UnityEngine.Component)
                {
                    fullPath = Hugula.Utils.CUtils.GetGameObjectFullPath(((UnityEngine.Component)target).gameObject) + " :" + fullPath;
                }

                UnityEngine.Debug.LogError($"Update target.property [{fullPath}].[{property}] error. \r\nC#:{ex.ToString()}");
            }

        }

        internal static object PartSubscribeInvoke(object obj, BindingPathPart part)
        {
            return instance?.m_PartSubscribe(obj, part);
        }
        #endregion

        #region new luastack invoke

        internal static object SetSourcePropertyInvoke(object obj, BindingPathPart part)
        {


            return null;
        }

        #endregion




        public override void Reset()
        {

            m_GetSourcePropertyInvoke = null;
            m_GetSourceMethodInvoke = null;
            m_SetSourcePropertyInvoke = null;
            m_SetSourceMethodInvoke = null;
            m_SetTargetPropertyInvoke = null;
            m_SetTargetPropertyNoFormatInvoke = null;
            m_SetTargetMethodInvoke = null;
            m_PartSubscribe = null;
        }

        public override void Dispose()
        {
            base.Dispose();

            m_GetSourcePropertyInvoke = null;
            m_GetSourceMethodInvoke = null;
            m_SetSourcePropertyInvoke = null;
            m_SetSourceMethodInvoke = null;
            m_SetTargetPropertyInvoke = null;
            m_SetTargetPropertyNoFormatInvoke = null;
            m_SetTargetMethodInvoke = null;
            m_PartSubscribe = null;
        }
    }

    public delegate object LuaInvoke(object obj, BindingPathPart part);

    public delegate object LuaGetSourceInvoke(object source, string path, bool is_indexer);

    public delegate object LuaSetSourceInvoke(object source, string path, object target, string property, bool is_indexer, object converter);

    public delegate void LuaSetTargetInvoke(object source, string path, object target, string property, bool is_indexer, bool is_self, string format, object converter);



    #region  lua function
    class XLuaExpressionUtility
    {
        static ObjectTranslator translator;

        static public void Initialize(LuaEnv luaEnv)
        {
            translator = luaEnv.translator;
        }


        static int CS_Lua_GetObject_ByPath(IntPtr L)
        {
            int oldTop = LuaAPI.lua_gettop(L);
            var source = translator.GetObject(L, 1, typeof(object));
            translator.Get(L, 2, out object BindingPathPart);

            // translator.Get
            // var path = LuaAPI.lua_tostring(L, 2);
            // ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            // object source = translator.GetObject(L, 1, typeof(object));
            // string path = LuaAPI.lua_tostring(L, 2);
            // bool is_indexer = LuaAPI.lua_toboolean(L, 3);
            // // object ret = ExpressionUtility.GetSourceInvoke(source, new BindingPathPart() { path = path, isIndexer = is_indexer });
            // translator.PushAny(L, ret);
            return 1;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int m_LuaLGetSourcePropertyInvoke(IntPtr L)
        {
            int oldTop = LuaAPI.lua_gettop(L);
            object source = translator.GetObject(L, 1, typeof(object));
            BindingPathPart bindingPart = translator.GetObject(L, 2, typeof(BindingPathPart)) as BindingPathPart;
            var property = bindingPart.path;
            var is_index = bindingPart.isIndexer;
            if (is_index)
            {
                int.TryParse(property, out int index);
                LuaAPI.xlua_pushinteger(L, index);
            }
            else
            {
                LuaAPI.lua_pushstring(L, property);
            }

            if (bindingPart.isMethod)
            {

                LuaAPI.lua_getfield(L, -2, property);
                // LuaAPI.lua_pushvalue(L, -2);
                // LuaAPI.lua_gettable(L, -2);
                // LuaAPI.lua_remove(L, -2);
                // LuaAPI.lua_remove(L, -2);
            }
            else
            {
                // LuaAPI.lua_getref(L, -1, "x");               /* push result of t.x (2nd arg) */


                // LuaAPI.lua_gettable(L, -2);
                // LuaAPI.lua_remove(L, -2);
            }


            // 交换栈顶的两个元素，使得栈顶顺序为 [source, key]
            // LuaAPI.lua_insert(L, -2);
            // 执行 lua_gettable(L, index)，在指定索引的表中查找键对应的值
            // LuaAPI.lua_gettable(L, -2); // 这将弹出键，获取值并压入栈顶     

            return 1;

            // int errFunc = LuaAPI.load_error_func(L, luaEnv.errorFuncRef);
            // LuaAPI.lua_getref(L, luaReference);
            // translator.PushByType(L, a);
            // int error = LuaAPI.lua_pcall(L, 1, 1, errFunc);
            // if (error != 0)
            //     luaEnv.ThrowExceptionFromError(oldTop);
            // TResult ret;
            // try
            // {
            //     translator.Get(L, -1, out ret);
            // }
            // catch (Exception e)
            // {
            //     throw e;
            // }
            // finally
            // {
            //     LuaAPI.lua_settop(L, oldTop);
            // }
            // return ret;
            // try
            // {
            //     object obj = LuaAPI.lua_touserdata(L, 1);//source
            //     BindingPathPart bindingPathPart = (BindingPathPart)LuaAPI.lua_touserdata(L, 2);

            //     BindingPathPart part = (BindingPathPart)LuaAPI.lua_touserdata(L, 2);
            //     object ret = GetSourceInvoke(obj, part);
            //     if (ret != null)
            //     {
            //         LuaAPI.lua_pushlightuserdata(L, ret);
            //         return 1;
            //     }
            //     else
            //     {
            //         LuaAPI.lua_pushnil(L);
            //         return 1;
            //     }
            // }
            // catch (Exception ex)
            // {
            //     LuaAPI.lua_pushnil(L);
            //     LuaAPI.lua_pushstring(L, ex.ToString());
            //     return 2;
            // }
        }
    }

    #endregion
}