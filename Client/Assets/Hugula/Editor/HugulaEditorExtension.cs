using System.Collections.Generic;
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
    }
}