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
    public static class BindableUtility
    {
        static List<string> allowTypes = new List<string>();
        static List<Type> allowTypeProperty = new List<Type>();
        static List<bool> allowIsMethod = new List<bool>();

        public static void RemoveAtbindableObjects(BindableObject target, int index)
        {
            var binding = target.GetBindings();
            binding.RemoveAt(index);
        }

        public static void AddEmptyBinding(BindableObject target, string property)
        {
            var bindings = target.GetBindings();

            foreach (var b in bindings)
            {
                if (b.propertyName == property)
                {
                    Debug.LogWarningFormat(" target({0}).{1} has already bound.", target, property);
                    return;
                }
            }
            var binding = new Binding();
            binding.propertyName = property;
            target.AddBinding(binding);

        }

        public static List<PropertyInfo> GetObjectProperties(UnityEngine.Object target)
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            if (target)
            {
                var obj = target; //temp
                Type t = obj.GetType();

                var propertes = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                foreach (var mi in propertes)
                {
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        var pi = (PropertyInfo)mi;
                        if (!string.IsNullOrEmpty(pi.Name) && pi.Name != "target")
                        {
                            propertyInfos.Add(pi);
                        }
                    }

                }
            }

            return propertyInfos;
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


        public static ReorderableList CreateBindalbeObjectBindingsReorder(SerializedObject serializedObject, SerializedProperty serializedProperty, UnityEngine.Object target, bool draggable,
        bool displayHeader, bool displayAddButton, bool displayRemoveButton, GenericMenu.MenuFunction2 onAddClick, System.Func<SerializedProperty, string, bool> onFilter)
        {

            FilterReorderableList orderList = new FilterReorderableList(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            orderList.onFilterFunc = onFilter;
            orderList.drawHeaderCallback = (Rect rect) =>
             {
                 var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);
                 EditorGUI.LabelField(rect, $"{serializedProperty.displayName} {serializedProperty.arraySize}");
                 var rect1 = rect;
                 rect1.x = rect.width * 0.4f;
                 rect1.width = rect.width * 0.6f;
                 if (orderList.onFilterFunc != null)
                 {
                     EditorGUI.BeginChangeCheck();
                     {
                         orderList.searchText = EditorGUI.TextField(rect1, string.Empty, orderList.searchText, new GUIStyle("ToolbarSeachTextField"));
                         if (GUILayout.Button("Close", "ToolbarSeachCancelButtonEmpty"))
                         {
                             // reset text
                             orderList.searchText = null;
                         }
                     }
                     if (EditorGUI.EndChangeCheck())
                     {
                         //  orderList.onFilterFunc()
                     }
                 }
             };

            orderList.onRemoveCallback = (ReorderableList orderlist) =>
                {
                    // if(UnityEditor.EditorUtility.DisplayDialog("warnning","Do you want to remove this element?","remove","canel"))
                    ReorderableList.defaultBehaviours.DoRemoveButton(orderlist);
                };


            orderList.drawElementCallback = (Rect rect, int index, bool selected, bool focused) =>
            {
                var element = orderList.serializedProperty.GetArrayElementAtIndex(index);

                if (orderList.onFilterFunc != null && orderList.onFilterFunc(element, orderList.searchText)) //搜索
                {

                }
                else
                {
                    var posRect_label = new Rect(rect)
                    {
                        x = rect.x + 10, //左边距
                        height = EditorGUIUtility.singleLineHeight
                    };

                    var path = element.FindPropertyRelative("path");
                    if (path != null)
                        element.isExpanded = EditorGUI.Foldout(posRect_label, element.isExpanded, $"{index}.{element.displayName}   path={path.stringValue}     ", true);
                    else
                    {
                        element.isExpanded = EditorGUI.Foldout(posRect_label, element.isExpanded, $"{index}.{element.objectReferenceValue}  ", true);
                    }

                    if (element.isExpanded)
                    {
                        var posRect_prop = new Rect(rect)
                        {
                            x = rect.x + 10,
                            y = rect.y + EditorGUIUtility.singleLineHeight,
                            height = rect.height - EditorGUIUtility.singleLineHeight
                        };
                        EditorGUI.PropertyField(posRect_prop, element, true);
                    }
                }

            };

            if (displayAddButton)
            {
                var properties = BindableUtility.GetObjectProperties(target);

                orderList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var per in properties)
                    {
                        menu.AddItem(new GUIContent($"{per.Name} ({per.PropertyType.Name})"), false, onAddClick, new object[] { per, target });
                    }
                    menu.ShowAsContext();
                };
            }

            orderList.elementHeightCallback = (index) =>
            {
                var element = orderList.serializedProperty.GetArrayElementAtIndex(index);

                if (orderList.onFilterFunc != null && orderList.onFilterFunc(element, orderList.searchText)) //搜索
                {
                    return 0;
                }

                var h = EditorGUIUtility.singleLineHeight;
                if (element.isExpanded)
                    h += EditorGUI.GetPropertyHeight(element);
                else
                    h += EditorGUIUtility.singleLineHeight * 0.5f;
                return h;
            };

            return orderList;
        }
    }


    public class BindalbeEditorUtilty
    {

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