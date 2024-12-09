using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using XLua;
using Hugula.Utils;
using Hugula;
using System.IO;
using Hugula.Databinding;
using Hugula.Mvvm;
using System.Reflection;
using XLua;
using HugulaEditor;
using System;

namespace Tests
{
    public class GCAllocTest
    {
        [Test]
        public void TestHashSet()
        {
            var  type = typeof(PropertyChangedEventHandlerEvent);
            var str = type.FullName;
            var ToString = type.ToString();
            var Namespace = type.Namespace;
            Debug.Log($"type.FullName: {str} type.ToString: {ToString} type.Namespace: {Namespace}");
            // HashSet<Action<object, string>> m_Events = new HashSet<Action<object, string>>();
            // Action<object, string> action = (sender, property) => { };
            // m_Events.Add(action);
            // m_Events.Remove(action);
        }

    }
}