using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEngine.UI;
using UnityEditorInternal;
using Hugula.Databinding.Binder;


namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableObject), true)]
    public class BindableObjectEditor : UnityEditor.Editor
    {

        SerializedProperty m_Property_bindings;
        SerializedProperty m_Property_m_Target;
        string propertyName = string.Empty;

        List<int> selectedList = new List<int>();

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            m_Property_m_Target = serializedObject.FindProperty("m_Target");
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
            EditorGUI.HelpBox(rect, "", MessageType.None);

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

            if (customer != null)
            {
                propertyName = BindalbeObjectUtilty.PopupComponentsProperty(rect1, customer.target, propertyName); //绑定属性
            }
            else
                propertyName = BindalbeObjectUtilty.PopupComponentsProperty(rect1, temp, propertyName); //绑定属性

            rect1.x = rect1.xMax + BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .2f - BindableObjectStyle.kExtraSpacing * 4;
            if (GUI.Button(rect1, "add"))
            {
                if (string.Equals(propertyName, BindableObjectStyle.FIRST_PROPERTY_NAME))
                {
                    Debug.LogWarningFormat("please choose a property to binding");
                    return;
                }
                Binding expression = new Binding();
                expression.propertyName = propertyName;
                temp.AddBinding(expression);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndHorizontal();

            //显示列表
            if (m_Property_bindings.isArray)
            {
                selectedList.Clear();
                serializedObject.Update();

                var len = temp.GetBindings().Count;
                SerializedProperty bindingProperty;
                for (int i = 0; i < len; i++)
                {
                    bindingProperty = m_Property_bindings.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(bindingProperty, true);
                    if (bindingProperty.isExpanded)
                    {
                        selectedList.Add(i);
                    }
                }

                //删除数据
                if (selectedList.Count > 0)
                {
                    rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                    rect.x = rect.xMax - 100;
                    rect.width = 100;
                    if (GUI.Button(rect, "del property " + selectedList.Count))
                    {

                        selectedList.Sort((int a, int b) =>
                            {
                                if (a < b) return 1;
                                else if (a == b) return 0;
                                else
                                    return -1;
                            });

                        foreach (var i in selectedList)
                            m_Property_bindings.RemoveElement(i);// DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.Separator();
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // GUILayout.Box(BindableObjectStyle.PROPPERTY_CHOOSE_TIPS);
                }
                serializedObject.ApplyModifiedProperties();

            }
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
}