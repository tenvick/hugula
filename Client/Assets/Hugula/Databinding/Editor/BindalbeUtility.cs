using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor
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
            // EditorGUILayout.LabelField ("B List", GUILayout.Width (200));
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
            UnityEngine.Object objComponent;
            if (temp.bindings != null)
            {
                for (int i = 0; i < temp.bindings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var binding = temp.bindings[i];
                    BindableExpressionEditor.Expression(binding, temp.targetName, i);
                    //         GUILayout.Label ((i + 1).ToString (), GUILayout.Width (20));
                    //         // objComponent = temp.children[i];
                    //         // objComponent = PopupGameObjectComponents (GetbindableObjects (temp, i).target, i); //选择绑定的component type类型
                    //         // if (objComponent != null) AddbindableObjects (temp, i, objComponent); //绑定选中的类型
                    //         // //显示选中的对象
                    //         // AddbindableObjects (temp, i, EditorGUILayout.ObjectField (GetbindableObjects (temp, i).target, typeof (UnityEngine.Object), true, GUILayout.MaxWidth (80)));
                    //         // //选择可绑定属性
                    //         PopupComponentsProperty (temp, i);

                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }

                    EditorGUILayout.EndHorizontal();
                    //         //设置binding属性
                    //         // SetBindingProperties (temp, i);
                    //         EditorGUILayout.Space ();
                }
            }
            // EditorGUILayout.Space ();
            // EditorGUILayout.BeginHorizontal ();
            // EditorGUILayout.Space ();

        }

        public static void BindableObjectField(CustomBinder target, int index)
        {
            if (target == null) return;
            int key = target.GetHashCode();
            string propertyName;
            dicPropertyName.TryGetValue(key, out propertyName);
            EditorGUILayout.Separator();
            // EditorGUILayout.LabelField ("B List", GUILayout.Width (200));
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            var temp = target as CustomBinder;
            GUILayout.Label(target.name, GUILayout.Width(100));
            EditorGUILayout.ObjectField(temp.target, typeof(CustomBinder), GUILayout.MaxWidth(150)); //显示绑定对象
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
            UnityEngine.Object objComponent;
            if (temp.bindings != null)
            {
                for (int i = 0; i < temp.bindings.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var binding = temp.bindings[i];
                    BindableExpressionEditor.Expression(binding, temp.targetName, i);
                    //         GUILayout.Label ((i + 1).ToString (), GUILayout.Width (20));
                    //         // objComponent = temp.children[i];
                    //         // objComponent = PopupGameObjectComponents (GetbindableObjects (temp, i).target, i); //选择绑定的component type类型
                    //         // if (objComponent != null) AddbindableObjects (temp, i, objComponent); //绑定选中的类型
                    //         // //显示选中的对象
                    //         // AddbindableObjects (temp, i, EditorGUILayout.ObjectField (GetbindableObjects (temp, i).target, typeof (UnityEngine.Object), true, GUILayout.MaxWidth (80)));
                    //         // //选择可绑定属性
                    //         PopupComponentsProperty (temp, i);

                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }

                    EditorGUILayout.EndHorizontal();
                    //         //设置binding属性
                    //         // SetBindingProperties (temp, i);
                    //         EditorGUILayout.Space ();
                }
            }
            // EditorGUILayout.Space ();
            // EditorGUILayout.BeginHorizontal ();
            // EditorGUILayout.Space ();

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
            // var binding = target.bindingExpression[i];
            int selectIndex = 0;
            if (target)
            {


                var obj = target; //temp
                Type t = obj.GetType();
                allowTypes.Clear();
                allowTypeProperty.Clear();
                allowIsMethod.Clear();
                //  add tips
                allowTypes.Add("choose property");
                allowTypeProperty.Add(typeof(Nullable));
                // allowIsMethod.Add (false);
                //end tips

                // string Property = binding.propertyName;
                int j = 1;
                var propertes = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                foreach (var mi in propertes)
                {
                    Type parameterType = null;
                    string name = null;
                    bool isMethod = false;

                    if (mi.MemberType == MemberTypes.Property)
                    {
                        var pi = (PropertyInfo)mi;
                        name = pi.Name;
                    }

                    //没必要绑定方法，可以把方法转换成属性。
                    // if (mi.MemberType == MemberTypes.Method && (!mi.Name.StartsWith ("get_") && !mi.Name.StartsWith ("set_"))) {
                    //     var parameters = ((MethodInfo) mi).GetParameters ();
                    //     if (parameters.Length == 1) {
                    //         parameterType = parameters[0].ParameterType;
                    //         name = mi.Name;
                    //         isMethod = true;
                    //     } else if (parameters.Length == 0) {
                    //         parameterType = typeof (void);
                    //         name = mi.Name;
                    //         isMethod = true;
                    //     }
                    // }

                    if (!string.IsNullOrEmpty(name))
                    {
                        char first = name[0];
                        // if (first >= 'A' && first <= 'Z') {
                        allowTypes.Add(name);
                        allowTypeProperty.Add(parameterType);
                        // allowIsMethod.Add (isMethod);
                        if (name.Equals(propertyName))
                        {
                            selectIndex = j;
                        }
                        j++;
                        // }
                    }
                }

                selectIndex = EditorGUILayout.Popup(selectIndex, allowTypes.ToArray(), options);
            }

            return selectIndex;
            // if (allowTypes.Count > selectIndex) {
            //UnityEngine.UI.Button button; button.onClick UnityEvent
            // binding.propertyName = allowTypes[selectIndex];
            // binding.returnType = BindableContainerEditor.GetLuaType (allowTypeProperty[selectIndex]);
            // binding.isMethod = allowIsMethod[selectIndex];
            // Debug.LogFormat(" propertyName{0} returnType{1},isMethod={2} ",binding.propertyName,binding.returnType,binding.isMethod);
            // }
            // else
            //     Debug.LogWarningFormat (" binding ({0}) property({1}) error" + allowTypes.Count, binding.target, binding.propertyName);
        }

    }

    public class BindalbeEditorUtilty
    {

        static List<string> m_AllComponents = new List<string>();
        public static UnityEngine.Object DrawPopUpComponents(string title, UnityEngine.Object content, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(title), GUILayout.Width(60));
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
            // int selectIndex = System.Array.IndexOf (bindMode, content);
            // if (selectIndex == -1) selectIndex = 0;
            content = (BindingMode)EditorGUILayout.EnumPopup(content, options);
            GUILayout.EndHorizontal();

            return content;
        }
    }


}