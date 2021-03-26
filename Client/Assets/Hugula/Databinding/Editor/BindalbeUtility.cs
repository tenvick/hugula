using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding.Binder;
using UnityEditorInternal;

namespace HugulaEditor.Databinding
{
    public static class BindalbeObjectUtilty
    {
        static List<string> allowTypes = new List<string>();
        static List<Type> allowTypeProperty = new List<Type>();
        static List<bool> allowIsMethod = new List<bool>();

        public static void RemoveAtbindableObjects(BindableObject target, int index)
        {
            var binding = target.GetBindings();
            binding.RemoveAt(index);
        }

        public static int GetObjectPropertyIndex(UnityEngine.Object target, string propertyName, out string[] properties)
        {
            int selectIndex = 0;
            if (target)
            {
                var obj = target; //temp
                Type t = obj.GetType();
                allowTypes.Clear();
                allowTypeProperty.Clear();
                allowIsMethod.Clear();
                allowTypes.Add(BindableObjectStyle.FIRST_PROPERTY_NAME);
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
                properties = allowTypes.ToArray();
            }
            else
            {
                properties = new string[] { };
            }
            return selectIndex;
        }
        public static string PopupComponentsProperty(UnityEngine.Object target, string propertyName, params GUILayoutOption[] options)
        {
            string[] propertes = null;
            int selectIndex = GetObjectPropertyIndex(target, propertyName, out propertes);
            selectIndex = EditorGUILayout.Popup(selectIndex, propertes, options);
            propertyName = propertes[selectIndex];
            return propertyName;
        }
        public static string PopupComponentsProperty(Rect rect, UnityEngine.Object target, string propertyName)
        {
            string[] propertes = null;
            if (target == null) return string.Empty;
            int selectIndex = GetObjectPropertyIndex(target, propertyName, out propertes);
            selectIndex = EditorGUI.Popup(rect, selectIndex, propertes);
            propertyName = propertes[selectIndex];
            return propertyName;
        }

        /// <summary>
        /// 根据组件类型查找对应的binder
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Type FindBinderType(Type componentType)
        {
            string typName = componentType.Name;
            string binderType = string.Format("Hugula.Databinding.Binder.{0}Binder", typName);
            var reType = Hugula.Utils.LuaHelper.GetClassType(binderType);
            return reType;
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

    public class BindableObjectStyle
    {

        #region  tips
        public const string PROPPERTY_CHOOSE_TIPS = "choose property to editor or delete!";
        public const string FIRST_PROPERTY_NAME = "choose property";

        #endregion

        #region  style
        internal const float kSingleLineHeight = 18f;
        internal static readonly float kControlVerticalSpacing = 2f;
        private static readonly GUIContent s_MixedValueContent = EditorGUIUtility.TrTextContent("\u2014", "Mixed Values");

        internal const float kSpacing = 5;
        internal const int kExtraSpacing = 9;

        internal static GUIContent mixedValueContent => s_MixedValueContent;

        internal static GUIStyle textStyle1 = new GUIStyle();

        private static readonly GUIContent s_Text = new GUIContent();
        private static readonly GUIContent s_Image = new GUIContent();
        private static readonly GUIContent s_TextImage = new GUIContent();

        internal static GUIContent Temp(string t)
        {
            s_Text.text = t;
            s_Text.tooltip = string.Empty;
            return s_Text;
        }
        internal static GUIContent Temp(string t, string tooltip)
        {
            s_Text.text = t;
            s_Text.tooltip = tooltip;
            return s_Text;
        }

        internal static GUIContent Temp(Texture i)
        {
            s_Image.image = i;
            s_Image.tooltip = string.Empty;
            return s_Image;
        }

        internal static GUIContent Temp(Texture i, string tooltip)
        {
            s_Image.image = i;
            s_Image.tooltip = tooltip;
            return s_Image;
        }

        internal static GUIContent Temp(string t, Texture i)
        {
            s_TextImage.text = t;
            s_TextImage.image = i;
            return s_TextImage;
        }

        #endregion
    }

    public static class SerializedPropertyExtend
    {

        public static void RemoveElement(this SerializedProperty list, int index)
        {
            if (list == null)
                throw new ArgumentNullException();

            if (!list.isArray)
                throw new ArgumentException("Property is not an array");

            if (index < 0 || index >= list.arraySize)
                throw new IndexOutOfRangeException();

            list.GetArrayElementAtIndex(index).SetPropertyValue(null);
            list.DeleteArrayElementAtIndex(index);

            list.serializedObject.ApplyModifiedProperties();
        }

        public static void SetPropertyValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {

                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = value as AnimationCurve;
                    break;

                case SerializedPropertyType.ArraySize:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Boolean:
                    property.boolValue = Convert.ToBoolean(value);
                    break;

                case SerializedPropertyType.Bounds:
                    property.boundsValue = (value == null)
                            ? new Bounds()
                            : (Bounds)value;
                    break;

                case SerializedPropertyType.Character:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Color:
                    property.colorValue = (value == null)
                            ? new Color()
                            : (Color)value;
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value);
                    break;

                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.LayerMask:
                    property.intValue = (value is LayerMask) ? ((LayerMask)value).value : Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;

                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (value == null)
                            ? Quaternion.identity
                            : (Quaternion)value;
                    break;

                case SerializedPropertyType.Rect:
                    property.rectValue = (value == null)
                            ? new Rect()
                            : (Rect)value;
                    break;

                case SerializedPropertyType.String:
                    property.stringValue = value as string;
                    break;

                case SerializedPropertyType.Vector2:
                    property.vector2Value = (value == null)
                            ? Vector2.zero
                            : (Vector2)value;
                    break;

                case SerializedPropertyType.Vector3:
                    property.vector3Value = (value == null)
                            ? Vector3.zero
                            : (Vector3)value;
                    break;

                case SerializedPropertyType.Vector4:
                    property.vector4Value = (value == null)
                            ? Vector4.zero
                            : (Vector4)value;
                    break;

            }
        }

        public static List<string> GetBindingSourceList(this Hugula.Databinding.BindableObject bindableObject)
        {
            var list = new List<string>();
            var bindings = bindableObject.GetBindings();


            foreach (var binding in bindings)
            {
                list.Add(binding.BindingToString());
            }

            return list;
        }

        static System.Text.StringBuilder sb = new StringBuilder();
        public static string BindingToString(this Hugula.Databinding.Binding binding)
        {
            sb.Clear();
            var property = binding.propertyName;
            var path = binding.path;
            var format = binding.format;
            BindingMode mode = binding.mode;
            var converter = binding.converter;
            var source = binding.source;
            if (!string.IsNullOrEmpty(path))
                sb.AppendFormat(".{0}=({1}) ", property, path);
            if (!string.IsNullOrEmpty(format))
                sb.AppendFormat("format({0}) ", format);
            if (mode != BindingMode.OneWay)
                sb.AppendFormat("mode({0}) ", mode);
            if (!string.IsNullOrEmpty(converter))
                sb.AppendFormat("converter({0}) ", converter);
            if (source)
            {
                //sb.AppendLine();
                sb.AppendFormat("source={0}", source.name);
            }

            return sb.ToString();
        }

    }
}