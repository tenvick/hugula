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
    [CustomEditor(typeof(MulitTargetsBinder), true)]
    public class MulitTargetsBinderEditor : UnityEditor.Editor
    {

        SerializedProperty m_Property_bindings;
        // SerializedProperty m_Property_m_Target;

        ReorderableList reorderableList_bindings;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");

            #region bindings
            reorderableList_bindings = BindableUtility.CreateMulitTargetsBinderReorder(serializedObject, m_Property_bindings, GetRealTarget(),
              true, false, true, true, OnAddClick, OnFilter);

            #endregion
        }

        bool OnFilter(SerializedProperty property, string searchText)
        {
            var displayName = property.displayName;
            if (!string.IsNullOrEmpty(searchText) && displayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) < 0) //搜索
                return true;

            return false;

        }

        void OnAddClick(object args)
        {
            var bindable = (BindableObject)args;
            BindableUtility.AddEmptyBinding(bindable,bindable, string.Empty);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            //显示列表
            serializedObject.Update();
            reorderableList_bindings.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        UnityEngine.Object GetRealTarget()
        {
            return target;
        }

    }
}