using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using UnityEditorInternal;
using Hugula;

namespace HugulaEditor.Databinding
{

    [CustomPropertyDrawer(typeof(PopUpComponentsAttribute), true)]
    public class PopUpComponentsDrawer : PropertyDrawer
    {
        List<string> m_AllComponents = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float w = position.width;
            position.width = w * .6f;
            EditorGUI.ObjectField(position, property, typeof(UnityEngine.Object), label);
            UnityEngine.Object content = null;//(UnityEngine.Component)property.objectReferenceValue;
            var obj = property.objectReferenceValue;
            if (obj != null && obj is UnityEngine.Object)
            {
                Component[] comps;
                if (obj is GameObject)
                {
                    comps = ((GameObject)obj).GetComponents<Component>();
                }
                else if (obj is Component)
                {
                    comps = ((Component)obj).GetComponents<Component>();
                }
                else
                {
                    Debug.LogFormat("PopUpComponentsDrawer.value={0}",obj);
                    return;
                }

                m_AllComponents.Clear();
                int selectIndex = 0;
                int i = 0;
                foreach (var comp in comps)
                {
                    m_AllComponents.Add(comp.GetType().Name);
                    if (comp == obj) selectIndex = i;
                    i++;
                }
                position.x = position.xMax;
                position.width = w - position.width;
                selectIndex = EditorGUI.Popup(position, selectIndex, m_AllComponents.ToArray());
                content = comps[selectIndex];
            }
            property.objectReferenceValue = content;

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }


    [CustomPropertyDrawer(typeof(BindableObjectAttribute), true)]
    public class BindableObjectProperty : PropertyDrawer
    {
        List<int> selectedList = new List<int>();
        string propertyName = "";
        SerializedObject temSerialziedObject;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // var serializedObject = property.serializedObject;

            var temp = (BindableObject)property.objectReferenceValue;
            if (temp == null) return;

            var tp = temp.GetType();
            var prop = tp.GetProperty("target", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                if (prop.GetValue(temp) == null)
                    prop.SetValue(temp, temp.GetComponent(prop.PropertyType));
            }

            var rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(34));
            //selected
            bool isExpanded = property.isExpanded;

            var rect1 = rect;
            float w = rect.width;
            EditorGUI.HelpBox(rect, "", MessageType.None);
            rect1.height -= BindableObjectStyle.kExtraSpacing * 2;
            rect1.y += BindableObjectStyle.kExtraSpacing;

            rect1.x += BindableObjectStyle.kExtraSpacing;
            rect1.width = 26f;
            isExpanded = EditorGUI.Toggle(rect1, isExpanded);
            property.isExpanded = isExpanded;
            rect1.x = rect1.xMax + BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .35f;


            CustomBinder customer = null;
            if (temp is CustomBinder)
            {
                customer = (CustomBinder)temp;
                EditorGUI.ObjectField(rect1, customer.target, typeof(Component), false); //显示绑定对象
            }
            else
                EditorGUI.ObjectField(rect1, temp, typeof(BindableObject), false); //显示绑定对象

            rect1.x = rect1.xMax + BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .35f;


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
                }
                else
                {
                    Binding expression = new Binding();
                    expression.propertyName = propertyName;
                    temp.AddBinding(expression);
                    property.isExpanded = true;
                }

            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndHorizontal();

            if (isExpanded)
            {
                //显示列表
                if (temSerialziedObject == null || temSerialziedObject.targetObject != temp)
                    temSerialziedObject = new SerializedObject(temp);
                {
                    var bindings = temSerialziedObject.FindProperty("bindings");
                    // serializedObject.targetObject
                    if (bindings != null && bindings.isArray)
                    {
                        selectedList.Clear();
                        temSerialziedObject.Update();

                        var len = bindings.arraySize;
                        SerializedProperty bindingProperty;
                        for (int i = 0; i < len; i++)
                        {
                            bindingProperty = bindings.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(bindingProperty, true);
                            if (bindingProperty.isExpanded)
                            {
                                selectedList.Add(i);
                            }
                        }


                        rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                        rect.y += BindableObjectStyle.kExtraSpacing;

                        float width;
                        //删除数据
                        if (selectedList.Count > 0)
                        {
                            width = 102;
                            rect.x = rect.xMax - width;
                            rect.width = width;
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
                                {
                                    bindings.DeleteArrayElementAtIndex(i);
                                }
                            }
                            EditorGUILayout.Separator();
                        }
                        else
                        {
                            string DELETE_TIPS = BindableObjectStyle.PROPPERTY_CHOOSE_TIPS;
                            width = DELETE_TIPS.Length * BindableObjectStyle.kExtraSpacing;
                            rect.x = rect.xMax - width;
                            rect.width = width;
                            GUI.Box(rect, DELETE_TIPS);
                        }

                        temSerialziedObject.ApplyModifiedProperties();

                        EditorGUILayout.Separator();
                        EditorGUILayout.EndHorizontal();
                    }
                }

            }
            else
            {
                // GUILayout.Box("click checkbox  to see detail or delete!");
            }
        }


    }

    [CustomPropertyDrawer(typeof(Binding), true)]
    public class BindingDrawer : PropertyDrawer
    {
        static Dictionary<string, bool> toggleDic = new Dictionary<string, bool>();
        static Dictionary<string, float> toggleHeight = new Dictionary<string, float>();

        static System.Text.StringBuilder sb = new System.Text.StringBuilder();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float w = position.width;
            var rect = position;
            string propertyName = property.FindPropertyRelative("propertyName").stringValue;

            EditorGUI.LabelField(rect, BindableObjectStyle.Temp(propertyName));
            EditorGUI.HelpBox(rect, "", MessageType.None);
            rect.width = 28;
            rect.height = 26;
            rect.x = EditorGUIUtility.singleLineHeight * 1.5f;
            rect.y += EditorGUIUtility.singleLineHeight;
            bool toggle = property.isExpanded;
            toggle = EditorGUI.Toggle(rect, toggle);
            if (toggle)
            {
                property.isExpanded = true;

                position.x = EditorGUIUtility.singleLineHeight * 2.5f;
                position.y += EditorGUIUtility.singleLineHeight;
                position.width -= EditorGUIUtility.singleLineHeight * 2f;
                var rects = GetRowRects(position);
                EditorGUI.PropertyField(rects[0], property.FindPropertyRelative("path"));
                EditorGUI.PropertyField(rects[1], property.FindPropertyRelative("mode"));
                EditorGUI.PropertyField(rects[2], property.FindPropertyRelative("format"));
                EditorGUI.PropertyField(rects[3], property.FindPropertyRelative("converter"));
                EditorGUI.PropertyField(rects[4], property.FindPropertyRelative("source"));
            }
            else
            {
                property.isExpanded = false;
                position.x = EditorGUIUtility.singleLineHeight * 2.5f;
                position.y += EditorGUIUtility.singleLineHeight;
                var path = property.FindPropertyRelative("path").stringValue;
                var format = property.FindPropertyRelative("format").stringValue;
                BindingMode mode = (BindingMode)property.FindPropertyRelative("mode").enumValueIndex;
                var converter = property.FindPropertyRelative("converter").stringValue;
                var source = property.FindPropertyRelative("source").objectReferenceValue;

                sb.Clear();
                if (!string.IsNullOrEmpty(path))
                    sb.AppendFormat("path({0})    ", path);
                if (!string.IsNullOrEmpty(format))
                    sb.AppendFormat("format({0})    ", format);
                if (mode != BindingMode.OneWay)
                    sb.AppendFormat("mode({0})    ", mode);
                if (!string.IsNullOrEmpty(converter))
                    sb.AppendFormat("converter({0})    ", converter);
                if (source)
                {
                    sb.AppendLine();
                    sb.AppendFormat("source:{0}", source.ToString());
                }

                EditorGUI.LabelField(position, sb.ToString());
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (property.isExpanded)
            {
                h = (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 6.2f + BindableObjectStyle.kControlVerticalSpacing * 2;
            }
            else
            {
                h = (EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 3;
            }
            return h;
        }

        Rect[] GetRowRects(Rect rect)
        {
            Rect[] rects = new Rect[5];
            rect.height = BindableObjectStyle.kSingleLineHeight;
            Rect rect0 = rect;
            rect0.y += EditorGUIUtility.singleLineHeight;//+ BindableObjectStyle.kControlVerticalSpacing;

            Rect rect1 = rect0;
            rect1.y += EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing; ;
            Rect rect2 = rect1;
            rect2.y += EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing; ;

            Rect rect3 = rect2;
            rect3.y += EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing; ;

            Rect rect4 = rect3;
            rect4.y += EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing; ;

            rects[0] = rect0;
            rects[1] = rect1;
            rects[2] = rect2;
            rects[3] = rect3;
            rects[4] = rect4;

            return rects;
        }

    }

}