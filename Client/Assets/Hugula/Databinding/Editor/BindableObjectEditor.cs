using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor {
    [CustomEditor (typeof (BindableObject), true)]
    public class BindableObjectEditor : UnityEditor.Editor {
        public override void OnInspectorGUI () {
            // base.OnInspectorGUI ();
            EditorGUILayout.Separator ();
            var temp = target as BindableObject;
            // GUILayout.Label ((index).ToString (), GUILayout.Width (20));
            // EditorGUILayout.BeginHorizontal ();
            // GUILayout.Label ("target:", GUILayout.Width (40));
            // temp.target = EditorGUILayout.ObjectField (temp.target, typeof (UnityEngine.Object), GUILayout.MaxWidth (150)); //显示绑定对象
            // EditorGUILayout.EndHorizontal ();
            base.OnInspectorGUI();
            BindableObjectHelper.BindableObjectField (temp, 0);
        }
    }
}