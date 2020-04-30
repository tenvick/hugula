using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEngine.UI;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableObject), true)]
    public class BindableObjectEditor : UnityEditor.Editor
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            var temp = target as BindableObject;
            var tp = temp.GetType();
            var prop = tp.GetProperty("target", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                if (prop.GetValue(target) == null)
                    prop.SetValue(target, temp.GetComponent(prop.PropertyType));
            }
            else
                base.OnInspectorGUI();
            BindalbeObjectUtilty.BindableObjectField(temp, 0);
        }

        private static void DisplayTypeInfo(Type t)
        {
            Debug.LogFormat("\r\n{0}", t);

            Debug.LogFormat("\tIs this a generic type definition? {0}",
               t.IsGenericTypeDefinition);

            Debug.LogFormat("\tIs it a generic type? {0}",
               t.IsGenericType);

            Type[] typeArguments = t.GetGenericArguments();
            Debug.LogFormat("\tList type arguments ({0}):", typeArguments.Length);
            foreach (Type tParam in typeArguments)
            {
                Debug.LogFormat("\t\t{0}", tParam);
            }
        }
    }

    // [CustomEditor (typeof (UIBehaviourBinder<UnityEngine.Object>), true)]
    // public class UIBehaviourBinderEditor : UnityEditor.Editor {


    //     void OnEnable()
    //     {
    //         var temp = target as UIBehaviourBinder<UnityEngine.Object>;

    //         // if (temp && temp.target == null)
    //         // {
    //         //     List<UnityEngine.EventSystems.UIBehaviour> results = new List<UnityEngine.EventSystems.UIBehaviour>();
    //         //     temp.GetComponents<UnityEngine.EventSystems.UIBehaviour>(results);
    //         //     if (results.Count > 0)
    //         //         temp.target = results[results.Count - 1];
    //         // }
    //     }

    //     public override void OnInspectorGUI () {
    //         // base.OnInspectorGUI ();
    //         EditorGUILayout.Separator ();
    //         var temp = target as UIBehaviourBinder<UnityEngine.Object>;
    //         base.OnInspectorGUI();
    //         // BindalbeObjectUtilty.BindableObjectField (temp, 0);
    //     }
    // }
}