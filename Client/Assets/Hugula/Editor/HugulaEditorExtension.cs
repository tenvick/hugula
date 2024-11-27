using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using Hugula.Utils;
using UnityEngine;
using UnityEditor;

namespace HugulaEditor
{

    public static class ObjectExtension
    {
        public static void CallMethod(this object obj, string method, params object[] args)
        {
            var tp = obj.GetType();
            tp.InvokeMember(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, obj, args);
        }

        public static object GetField(this object obj,string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                throw new InvalidOperationException("Field 'bindings' not found in the specified object.");
            }

            return field.GetValue(obj);
        }

        public static void SetField(this object obj,string fieldName,object value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                throw new InvalidOperationException("Field 'bindings' not found in the specified object.");
            }

            field.SetValue(obj,value);
        }
    }
}