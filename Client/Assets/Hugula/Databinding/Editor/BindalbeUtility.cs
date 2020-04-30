using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding.Binder;

namespace HugulaEditor.Databinding
{
    public static class BindalbeObjectUtilty
    {
        static Dictionary<int, string> dicPropertyName = new Dictionary<int, string>();
        public static void BindableObjectField(BindableObject target, int index)
        {
            if (target == null) return;
            int key = target.GetHashCode();
            string propertyName;
            dicPropertyName.TryGetValue(key, out propertyName);
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            var temp = target as BindableObject;
            GUILayout.Label(target.name, GUILayout.Width(100));
            EditorGUILayout.ObjectField(temp, typeof(BindableObject), false, GUILayout.MaxWidth(150)); //显示绑定对象
            int selectedIndex = PopupComponentsProperty(temp, propertyName, GUILayout.MaxWidth(150)); //绑定属性
            propertyName = GetSelectedPropertyByIndex(selectedIndex);
            dicPropertyName[key] = propertyName;
            if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
            {
                if (selectedIndex == 0)
                {
                    Debug.LogWarningFormat("please choose a property to binding");
                    return;
                }
                if (temp.bindings == null) temp.bindings = new List<Binding>();

                Binding expression = new Binding();
                expression.propertyName = propertyName;
                temp.bindings.Add(expression);
            }
            Undo.RecordObject(target, "F");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            if (temp.bindings != null)
            {
                for (int i = 0; i < temp.bindings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var binding = temp.bindings[i];
                    BindableExpressionEditor.Expression(binding, i);

                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }

                    EditorGUILayout.EndHorizontal();

                }
            }

        }

        public static void BindableObjectField(CustomBinder target, int index)
        {
            if (target == null) return;
            int key = target.GetHashCode();
            string propertyName;
            dicPropertyName.TryGetValue(key, out propertyName);
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            var temp = target as CustomBinder;
            GUILayout.Label(target.name, GUILayout.Width(100));
            GUILayout.Label("property:", GUILayout.MinWidth(60));
            // EditorGUILayout.ObjectField(temp.binderTarget, typeof(CustomBinder), false, GUILayout.MaxWidth(150)); //显示绑定对象
            int selectedIndex = PopupComponentsProperty(temp.target, propertyName, GUILayout.MaxWidth(150)); //绑定属性
            propertyName = GetSelectedPropertyByIndex(selectedIndex);
            dicPropertyName[key] = propertyName;
            if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
            {
                if (selectedIndex == 0)
                {
                    Debug.LogWarningFormat("please choose a property to binding");
                    return;
                }
                if (temp.bindings == null) temp.bindings = new List<Binding>();

                Binding expression = new Binding();
                expression.propertyName = propertyName;
                temp.bindings.Add(expression);
            }
            Undo.RecordObject(target, "F");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if (temp.bindings != null)
            {
                for (int i = 0; i < temp.bindings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var binding = temp.bindings[i];
                    BindableExpressionEditor.Expression(binding, i);

                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        }
        static List<string> allowTypes = new List<string>();
        static List<Type> allowTypeProperty = new List<Type>();
        static List<bool> allowIsMethod = new List<bool>();

        public static void RemoveAtbindableObjects(BindableObject target, int index)
        {
            var binding = target.bindings;
            binding.RemoveAt(index);
        }

        public static string GetSelectedPropertyByIndex(int selectIndex)
        {
            if (allowTypes.Count > selectIndex)
                return allowTypes[selectIndex];
            return null;
        }
        public static int PopupComponentsProperty(UnityEngine.Object target, string propertyName, params GUILayoutOption[] options)
        {
            int selectIndex = 0;
            if (target)
            {

                var obj = target; //temp
                Type t = obj.GetType();
                allowTypes.Clear();
                allowTypeProperty.Clear();
                allowIsMethod.Clear();
                allowTypes.Add("choose property");
                allowTypeProperty.Add(typeof(Nullable));
                allowTypes.Add(BindableObject.ContextProperty);
                allowTypeProperty.Add(typeof(object));
                int j = 2;

                var propertes = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                foreach (var mi in propertes)
                {
                    Type parameterType = null;
                    string name = null;

                    if (mi.MemberType == MemberTypes.Property)
                    {
                        var pi = (PropertyInfo)mi;
                        name = pi.Name;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        allowTypes.Add(name);
                        allowTypeProperty.Add(parameterType);
                        if (name.Equals(propertyName))
                        {
                            selectIndex = j;
                        }
                        j++;
                    }
                }
                if (string.Equals(propertyName, BindableObject.ContextProperty))
                {
                    selectIndex = allowTypes.IndexOf(BindableObject.ContextProperty);
                }
                selectIndex = EditorGUILayout.Popup(selectIndex, allowTypes.ToArray(), options);
            }
            return selectIndex;
        }

    }

    public class BindalbeEditorUtilty
    {

        static List<string> m_AllComponents = new List<string>();
        public static UnityEngine.Object DrawPopUpComponents(string title, UnityEngine.Object content, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(title), GUILayout.Width(80));
            content = EditorGUILayout.ObjectField(content, typeof(UnityEngine.Component), true, GUILayout.MaxWidth(150)); //显示绑定对象
            if (content is UnityEngine.Component)
            {
                var comps = ((UnityEngine.Component)content).GetComponents<Component>();
                m_AllComponents.Clear();
                int selectIndex = 0;
                int i = 0;
                foreach (var comp in comps)
                {
                    m_AllComponents.Add(comp.GetType().Name);
                    if (comp == content) selectIndex = i;
                    i++;
                }

                selectIndex = EditorGUILayout.Popup(selectIndex, m_AllComponents.ToArray(), options);
                content = comps[selectIndex];
            }
            GUILayout.EndHorizontal();
            return content;

        }

        public static string DrawEditorLabl(string title, string content, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(title), GUILayout.Width(60));
            content = GUILayout.TextField(content, options);
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(content)) content = content.Replace(",", "").Replace("=", "");
            return content;
        }

        public static BindingMode DrawEume(string title, BindingMode content, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(title));
            content = (BindingMode)EditorGUILayout.EnumPopup(content, options);
            GUILayout.EndHorizontal();

            return content;
        }
    }


}