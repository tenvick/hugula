using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;
using Hugula.Databinding.Binder;
// using ICSharpCode.NRefactory.Ast;

namespace HugulaEditor.Databinding.ConvertLua
{

    public interface IRectTransformBinderConvert
    {
        string GetSetSourceCodeByPropertyName(object binder, string target, string propertyName, string value);
    }

    public class BinderConvertUtils
    {
        static Dictionary<string, Type> m_BinderTypeConverts = new Dictionary<string, Type>();
        static Dictionary<string, IRectTransformBinderConvert> m_BinderConverts = new Dictionary<string, IRectTransformBinderConvert>();
        private static Type FindComponentConvertType(string binderTypeName)
        {
            if (m_BinderTypeConverts.TryGetValue(binderTypeName, out var type))
                return type;

            string binderType = string.Format("HugulaEditor.Databinding.ConvertLua.{0}Convert", binderTypeName);
            var reType = Hugula.Utils.LuaHelper.GetClassType(binderType);
            if (reType == null)
            {
                Debug.LogWarningFormat("Databinding.Convert type ({0}) was't found. use DefaultBinderConvert repleaced. ", binderType);
                reType = typeof(BinderConvertUtils);
            }
            m_BinderTypeConverts[binderTypeName] = reType;
            return reType;
        }

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="binderShortTypeName"></param>
        /// <returns></returns>
        public static IRectTransformBinderConvert GetConvertByBinderType(string binderShortTypeName)
        {
            if (m_BinderConverts.TryGetValue(binderShortTypeName, out var convert))
                return convert;

            var type = FindComponentConvertType(binderShortTypeName);
            if (type == typeof(BinderConvertUtils))
                type = typeof(BaseBinderConvert);

            var convertInstance = Activator.CreateInstance(type) as IRectTransformBinderConvert;
            m_BinderConverts[binderShortTypeName] = convertInstance;
            return convertInstance;
        }

        public static bool CheckBinderConvertIsExist(string binderShortTypeName)
        {
            if (m_BinderTypeConverts.TryGetValue(binderShortTypeName, out var type))
            {
                if (type == typeof(BinderConvertUtils))
                    return false;
                else
                    return true;
            }

            var re = true;
            string binderType = string.Format("HugulaEditor.Databinding.ConvertLua.{0}Convert", binderShortTypeName);
            var reType = Hugula.Utils.LuaHelper.GetClassType(binderType);
            if (reType == null)
            {
                reType = typeof(BinderConvertUtils);
                re = false;
            }
            m_BinderTypeConverts[binderShortTypeName] = reType;

            return re;
        }
        public static void Clear()
        {
            m_BinderTypeConverts.Clear();
            m_BinderConverts.Clear();
        }
    }

    public class BaseBinderConvert : IRectTransformBinderConvert
    {
        public virtual string GetSetSourceCodeByPropertyName(object binder, string target, string propertyName, string value)
        {
            if (propertyName == "activeSelf")
                return $"{target}.gameObject:SetActive({value})";
            else
                return $"{target}.{propertyName} = {value}";
        }
    }
}