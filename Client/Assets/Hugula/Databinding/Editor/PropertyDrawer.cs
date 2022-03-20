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
                    Debug.LogFormat("PopUpComponentsDrawer.value={0}", obj);
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


    [CustomPropertyDrawer(typeof(BindableObject), true)]
    public class BindableObjectProperty : PropertyDrawer
    {
        string propertyName = "";
        SerializedObject bindableObjectSerializedObject;
        ReorderableList reorderableListBindings;
        SerializedProperty m_Property_bindings;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Separator();
            var serializedObject = property.serializedObject;
            var target = property.objectReferenceValue;
            if(target is null) return;


            var temp = target as BindableObject;
            var tp = temp.GetType();
            var prop = tp.GetProperty("target", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                if (prop.GetValue(target) == null)
                    prop.SetValue(target, temp.GetComponent(prop.PropertyType));
            }

            if (temp is CustomBinder)
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(property);
                serializedObject.ApplyModifiedProperties();
            }
            var rect1 = position;
            float w = position.width;

            rect1.height = BindableObjectStyle.kSingleLineHeight;
            rect1.width = w * .5f;
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

            // EditorGUILayout.Separator();
            if (reorderableListBindings == null)
            {
                bindableObjectSerializedObject = new SerializedObject(target);
                m_Property_bindings = bindableObjectSerializedObject.FindProperty("bindings");
                reorderableListBindings = BindableUtility.CreateBindalbeObjectBindingsReorder(bindableObjectSerializedObject, m_Property_bindings, target, true,
                true, true, true, OnAddClick,OnFilter);
            }

            //显示列表
            position.y += BindableObjectStyle.kSingleLineHeight * 1.1f;
            serializedObject.Update();
            if(bindableObjectSerializedObject !=null) bindableObjectSerializedObject.Update();
            reorderableListBindings.DoList(position);
            if(bindableObjectSerializedObject !=null) bindableObjectSerializedObject.ApplyModifiedProperties();

            serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (property.isExpanded)
            {
                if (reorderableListBindings != null)
                    h = reorderableListBindings.GetHeight() + (BindableObjectStyle.kSingleLineHeight) * 2f;
                else
                    h = (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 7f + BindableObjectStyle.kControlVerticalSpacing * 2;
            }
            else
            {
                h = (EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 1f;
            }
            return h;
        }

        bool OnFilter(SerializedProperty property, string searchText)
        {
            var displayName = property.displayName;
            if (!string.IsNullOrEmpty(searchText) && displayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) < 0) //搜索
            {
                return true;
            }
            else
                return false;

        }

        void OnAddClick(object args)
        {
            var arr = (object[])args;
            var per = (PropertyInfo)arr[0];
            var bindable = (BindableObject)arr[1];
            var property = per.Name;
            BindableUtility.AddEmptyBinding(bindable,property);
        }
    }

    [CustomPropertyDrawer(typeof(Binding), true)]
    public class BindingDrawer : PropertyDrawer
    {
        static Dictionary<string, bool> toggleDic = new Dictionary<string, bool>();
        static Dictionary<string, float> toggleHeight = new Dictionary<string, float>();

        static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        static GUIStyle fontStyle = new GUIStyle();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string propertyName = property.FindPropertyRelative("propertyName").stringValue;
            bool toggle = property.isExpanded;
            position.x += EditorGUIUtility.singleLineHeight;
            position.width -= EditorGUIUtility.singleLineHeight;
            if (toggle == false)
            {
                var path = property.FindPropertyRelative("path").stringValue;
                var format = property.FindPropertyRelative("format").stringValue;
                BindingMode mode = (BindingMode)property.FindPropertyRelative("mode").enumValueIndex;
                var converter = property.FindPropertyRelative("converter").stringValue;
                var source = property.FindPropertyRelative("source").objectReferenceValue;

                sb.Clear();
                sb.Append(propertyName);
                sb.Append(":    ");

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

                propertyName = sb.ToString();
                position.height = EditorGUIUtility.singleLineHeight * 2f;
            }

            var rect = position;
            rect.height = 26;
            rect.x = EditorGUIUtility.singleLineHeight * 1.5f;

            fontStyle.fontSize = 12;
            fontStyle.fontStyle = FontStyle.Italic;
            if (toggle)
            {
                // position.y += EditorGUIUtility.singleLineHeight * .5f;
                position.width -= EditorGUIUtility.singleLineHeight * .5f;
                var rects = GetRowRects(position);
                EditorGUI.PropertyField(rects[0], property.FindPropertyRelative("path"));
                EditorGUI.PropertyField(rects[1], property.FindPropertyRelative("mode"));
                EditorGUI.PropertyField(rects[2], property.FindPropertyRelative("format"));
                EditorGUI.PropertyField(rects[3], property.FindPropertyRelative("converter"));
                EditorGUI.PropertyField(rects[4], property.FindPropertyRelative("source"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (property.isExpanded)
            {
                h = (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 7f + BindableObjectStyle.kControlVerticalSpacing * 2;
            }
            else
            {
                h = (EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 1f;
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