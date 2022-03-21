using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEditorInternal;


namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableObject), true)]
    public class BindableObjectEditor : UnityEditor.Editor
    {

        SerializedProperty m_Property_bindings;
        SerializedProperty m_Property_m_Target;

        ReorderableList reorderableList_bindings;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            m_Property_m_Target = serializedObject.FindProperty("m_Target");

            #region bindings
            reorderableList_bindings = BindableUtility.CreateBindalbeObjectBindingsReorder(serializedObject, m_Property_bindings, GetRealTarget(),
            true, false, true, true, OnAddClick, OnFilter);
            
            #endregion
        }

        bool OnFilter(SerializedProperty property,string searchText)
        {
            var displayName = property.displayName;
            if (!string.IsNullOrEmpty(searchText) && displayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) < 0) //搜索
                return true;

            return false;

        }

        void OnAddClick(object args)
        {
            var arr = (object[])args;
            var per = (PropertyInfo)arr[0];
            var bindable = (BindableObject)arr[1];
            var property = per.Name;
            BindableUtility.AddEmptyBinding(bindable, property);
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

            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            //显示列表
            serializedObject.Update();
            reorderableList_bindings.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        UnityEngine.Object GetRealTarget()
        {
            // if (target is CustomBinder)
            // {
            //     return ((CustomBinder)target).target;
            // }
            // else
                return target;
        }

    }
}