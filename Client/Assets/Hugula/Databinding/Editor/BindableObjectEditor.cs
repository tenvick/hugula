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
        string propertyName = string.Empty;

        List<int> selectedList = new List<int>();

        ReorderableList reorderableList_bindings;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            m_Property_m_Target = serializedObject.FindProperty("m_Target");

            #region bindings
            reorderableList_bindings = BindalbeObjectUtilty.CreateBindalbeObjectBindingsReorder(serializedObject, m_Property_bindings, GetRealTarget(),
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
            BindalbeObjectUtilty.AddEmptyBinding(bindable, property);
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

            if (temp is CustomBinder)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(m_Property_m_Target);
                serializedObject.ApplyModifiedProperties();
            }

            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(34));
            var rect1 = rect;
            float w = rect.width;
            // EditorGUI.HelpBox(rect, "", MessageType.None);

            rect1.height -= BindableObjectStyle.kExtraSpacing * 2;
            rect1.x += BindableObjectStyle.kExtraSpacing;
            rect1.y += BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .4f;
            //
            CustomBinder customer = null;
            if (temp is CustomBinder)
            {
                customer = (CustomBinder)temp;
                EditorGUI.ObjectField(rect1, customer.target, typeof(Component), false); //显示绑定对象
            }
            else
            {
                EditorGUI.ObjectField(rect1, temp, typeof(BindableObject), false); //显示绑定对象
            }

            rect1.x = rect1.xMax + BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .4f;

            var len = m_Property_bindings.arraySize;
            var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);

            EditorGUILayout.Separator();
            EditorGUILayout.EndHorizontal();
            //显示列表
            reorderableList_bindings.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        UnityEngine.Object GetRealTarget()
        {
            if (target is CustomBinder)
            {
                return ((CustomBinder)target).target;
            }
            else
                return target;
        }

    }
}