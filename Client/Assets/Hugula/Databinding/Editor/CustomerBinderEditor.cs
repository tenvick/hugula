using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor {
    [CustomEditor (typeof (CustomBinder), true)]
    public class CustomerBinderEditor : UnityEditor.Editor {
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
        public override void OnInspectorGUI () {
            // base.OnInspectorGUI ();
            EditorGUILayout.Separator ();
            var temp = target as CustomBinder;
            base.OnInspectorGUI();
            BindalbeObjectUtilty.BindableObjectField (temp, 0);
        }
    }
}