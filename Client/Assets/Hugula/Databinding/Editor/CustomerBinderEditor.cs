using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(CustomBinder), true)]
    public class CustomerBinderEditor : UnityEditor.Editor
    {
        SerializableAttribute binderTarget;
        void OnEnable()
        {
            var temp = target as CustomBinder;

            if (temp && temp.target == null)
            {
                List<UnityEngine.EventSystems.UIBehaviour> results = new List<UnityEngine.EventSystems.UIBehaviour>();
                temp.GetComponents<UnityEngine.EventSystems.UIBehaviour>(results);
                if (results.Count > 0)
                    temp.target = results[results.Count - 1];
            }

        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            var temp = target as CustomBinder;
            // base.OnInspectorGUI();
            GUILayout.BeginHorizontal();

            temp.target = (Component)BindalbeEditorUtilty.DrawPopUpComponents("Binder Target", temp.target, GUILayout.MinWidth(150));
            GUILayout.EndHorizontal();
            BindalbeObjectUtilty.BindableObjectField(temp, 0);
        }
    }
}