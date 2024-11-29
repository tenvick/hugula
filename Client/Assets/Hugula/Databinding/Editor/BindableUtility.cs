using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding.Binder;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

namespace HugulaEditor.Databinding
{
    public static class BindableUtility
    {
        //获取安全的名字
        public static string GetSafeName(string name)
        {
            int i = name.IndexOf("@");
            int j = name.IndexOf("(");
            if (i < j) i = j;
            if (i < 0) i = name.Length;
            return name.Substring(0, i).Replace(" ", "_").Replace(" (", "").Replace("）", "").Replace("(", "_").Replace(")", "");
        }

        public static void RemoveAtbindableObjects(BindableObject target, int index)
        {
            var binding = target.GetBindings();
            binding.RemoveAt(index);
        }

        public static Binding AddEmptyBinding(BindableObject target, Binding binding)
        {
            var bindings = target.GetBindings();

            if (bindings != null)
            {
                foreach (var b in bindings)
                {
                    if (b.propertyName == binding.propertyName && b.target == binding.target)
                    {
                        Debug.LogWarningFormat(" target({0}).{1} has already bound.", target, binding.propertyName);
                        return b;
                    }
                }
            }
            target.AddBinding(binding);

            return binding;
        }

        public static Binding AddEmptyBinding(BindableObject target, string property)
        {
          
            var binding = new Binding();
            binding.propertyName = property;
            binding.target = target;
            AddEmptyBinding(target,binding);
            return binding;
        }
        public static Binding AddEmptyBinding(BindableObject target, string property, string path)
        {
            var binding = new Binding();
            binding.target = target;
            binding.path = path;
            binding.propertyName = property;
            AddEmptyBinding(target,binding);
            return binding;
        }

        public static Binding AddEmptyBinding(BindableObject bindableObject, UnityEngine.Object bindingTarget, string property)
        {
            var binding = new Binding();
            binding.propertyName = property;
            binding.target = bindingTarget;
            AddEmptyBinding(bindableObject,binding);
            return binding;
        }

        public static Binding AddEmptyBinding(BindableObject bindableObject, UnityEngine.Object bindingTarget, string property, string path)
        {
            var binding = new Binding();
            binding.propertyName = property;
            binding.path = path;
            binding.target = bindingTarget;
            AddEmptyBinding(bindableObject,binding);
            return binding;
        }

        public static string GetFullPath(Transform transform)
        {
            if (transform == null) return string.Empty;
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        public static BindableContainer GetRootBindableContainer(Component bindableObject)
        {
            var curr = bindableObject;
            var root = bindableObject.GetComponent<BindableContainer>();
            do
            {
                curr = curr.transform.parent;
                if (curr == null) break;
                var c = curr.GetComponent<BindableContainer>();
                if (c != null)
                {
                    root = c;
                }
            } while (curr.transform.parent != null);

            return root;
        }

        public static List<PropertyInfo> GetObjectProperties(UnityEngine.Object target, bool checkCanWrite = true)
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
                            if (!checkCanWrite || (checkCanWrite && pi.CanWrite))
                                propertyInfos.Add(pi);
                        }
                    }

                }
            }

            return propertyInfos;
        }

        /// <summary>
        /// 检查是否有属性
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool CheckHasProperty(UnityEngine.Object target, string propertyName)
        {
            if (target)
            {
                var obj = target; //temp
                Type t = obj.GetType();
                var propertes = t.GetMember(propertyName);
                return propertes.Length > 0;
            }

            return false;
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

        /// <summary>
        /// 默认filter过滤函数
        /// </summary>
        /// <param name="property"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static bool OnDefaultFilter(SerializedProperty property, string searchText)
        {
            var bingTarget = property.FindPropertyRelative("target").objectReferenceValue?.ToString();
            string propertyName = property.FindPropertyRelative("propertyName").stringValue;
            var path = property.FindPropertyRelative("path").stringValue;

            if (!string.IsNullOrEmpty(searchText))  //搜索
            {
                var id1 = propertyName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0;
                var id2 = bingTarget.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0;
                var id3 = path.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0;
                if (!(id1 || id2 || id3))
                {
                    return true; //不显示
                }
            }

            return false;
        }

        public static ReorderableList CreateMulitTargetsBinderReorder(SerializedObject serializedObject, SerializedProperty serializedProperty, UnityEngine.Object target, bool draggable,
       bool displayHeader, bool displayAddButton, bool displayRemoveButton, GenericMenu.MenuFunction2 onAddClick, System.Func<SerializedProperty, string, bool> onFilter)
        {

            FilterReorderableList orderList = new FilterReorderableList(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            if (onFilter == null) onFilter = OnDefaultFilter;
            orderList.onFilterFunc = onFilter;
            orderList.drawHeaderCallback = (Rect rect) =>
             {
                 var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);
                 BindableObjectStyle.LabelFieldStyle(rect, $"{serializedProperty.displayName} {serializedProperty.arraySize}", "#0e6dcbff", 12);

                 var rect1 = rect;
                 rect1.x = rect.width * 0.4f;
                 rect1.width = rect.width * 0.6f;
                 if (orderList.onFilterFunc != null)
                 {
                     EditorGUI.BeginChangeCheck();
                     {
                         orderList.searchText = EditorGUI.TextField(rect1, string.Empty, orderList.searchText, BindableObjectStyle.ToolbarSeachTextField);
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
                    var bingTarget = element.FindPropertyRelative("target");
                    if (path != null)
                    {
                        var disObjName = bingTarget.objectReferenceValue == null ? "null" : bingTarget.objectReferenceValue.ToString();
                        element.isExpanded = BindableObjectStyle.FoldoutStyle(posRect_label, element.isExpanded, $"{index} ({disObjName}).{element.displayName}   path={path.stringValue}     ", "#2c76adff", 11);
                    }

                    if (element.isExpanded)
                    {
                        var posRect_prop = new Rect(rect)
                        {
                            x = rect.x + 10,
                            y = rect.y + EditorGUIUtility.singleLineHeight,
                            height = rect.height - EditorGUIUtility.singleLineHeight
                        };
                        // EditorGUI.PropertyField(posRect_prop, element, true);
                        BindingDrawer.OnMulitTargetsBinderGUI(posRect_prop, element);
                    }
                }

            };

            if (displayAddButton)
            {
                List<PropertyInfo> properties = new List<PropertyInfo>();
                {
                    properties = BindableUtility.GetObjectProperties(target);
                }
                orderList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent($"add empty binding"), false, onAddClick, target);
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
                {
                    h += (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 8f + BindableObjectStyle.kControlVerticalSpacing * 2;
                }
                else
                    h += EditorGUIUtility.singleLineHeight * 0.5f;
                return h;
            };

            return orderList;
        }


        public static ReorderableList CreateBindalbeObjectBindingsReorder(SerializedObject serializedObject, SerializedProperty serializedProperty, UnityEngine.Object target, bool draggable,
        bool displayHeader, bool displayAddButton, bool displayRemoveButton, GenericMenu.MenuFunction2 onAddClick, System.Func<SerializedProperty, string, bool> onFilter)
        {

            FilterReorderableList orderList = new FilterReorderableList(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            if (onFilter == null) onFilter = OnDefaultFilter;
            orderList.onFilterFunc = onFilter;
            orderList.drawHeaderCallback = (Rect rect) =>
             {
                 var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);
                 BindableObjectStyle.LabelFieldStyle(rect, $"{serializedProperty.displayName} {serializedProperty.arraySize}", "#0e6dcbff", 12);

                 var rect1 = rect;
                 rect1.x = rect.width * 0.4f;
                 rect1.width = rect.width * 0.6f;
                 if (orderList.onFilterFunc != null)
                 {
                     EditorGUI.BeginChangeCheck();
                     {
                         orderList.searchText = EditorGUI.TextField(rect1, string.Empty, orderList.searchText, BindableObjectStyle.ToolbarSeachTextField);
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
                    {
                        element.isExpanded = BindableObjectStyle.FoldoutStyle(posRect_label, element.isExpanded, $"{index}.{element.displayName}   path={path.stringValue}     ", "#2c76adff", 11);
                        // element.isExpanded = EditorGUI.Foldout(posRect_label, element.isExpanded, $"{index}.{element.displayName}   path={path.stringValue}     ", true);
                    }
                    else
                    {
                        element.isExpanded = BindableObjectStyle.FoldoutStyle(posRect_label, element.isExpanded, $"{index}.{element.objectReferenceValue}  ", "#0e6dcbff", 12);
                        // element.isExpanded = EditorGUI.Foldout(posRect_label, element.isExpanded,BindableObjectStyle.ColorText($"{index}.{element.objectReferenceValue}","#8cacbcff"), true);
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
                List<PropertyInfo> properties = new List<PropertyInfo>();
                if (target is CustomBinder)
                {
                    properties = BindableUtility.GetObjectProperties(((CustomBinder)target).target);
                }
                else
                {
                    properties = BindableUtility.GetObjectProperties(target);
                }


                orderList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
                {
                    var dropdown = new BindablePropertyDropdown(new AdvancedDropdownState(), properties, target, onAddClick);
                    dropdown.Show(rect);
                };
                // orderList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
                // {
                //     GenericMenu menu = new GenericMenu();
                //     foreach (var per in properties)
                //     {
                //         menu.AddItem(new GUIContent($"{per.Name} ({per.PropertyType.Name})"), false, onAddClick, new object[] { per, target });
                //     }
                //     menu.ShowAsContext();
                // };
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


        public static ReorderableList CreateMonoContainerChildrenReorder(SerializedObject serializedObject, SerializedProperty serializedProperty, UnityEngine.Object target, bool draggable,
        bool displayHeader, bool displayAddButton, bool displayRemoveButton, GenericMenu.MenuFunction2 onAddClick, System.Func<SerializedProperty, string, bool> onFilter)
        {

            FilterReorderableList orderList = new FilterReorderableList(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton);
            if (onFilter == null) onFilter = OnDefaultFilter;
            orderList.onFilterFunc = onFilter;
            orderList.drawHeaderCallback = (Rect rect) =>
             {
                 var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);
                 BindableObjectStyle.LabelFieldStyle(rect, $"{serializedProperty.displayName} {serializedProperty.arraySize}", "#0e6dcbff", 12);

                 var rect1 = rect;
                 rect1.x = rect.width * 0.4f;
                 rect1.width = rect.width * 0.6f;
                 if (orderList.onFilterFunc != null)
                 {
                     EditorGUI.BeginChangeCheck();
                     {
                         orderList.searchText = EditorGUI.TextField(rect1, string.Empty, orderList.searchText, BindableObjectStyle.ToolbarSeachTextField);
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
                var taget = (BindableContainer)orderList.serializedProperty.serializedObject.targetObject;

                while (taget.names.Count <= index)
                    taget.names.Add(string.Empty);

                if (orderList.onFilterFunc != null && orderList.onFilterFunc(element, orderList.searchText)) //搜索
                {

                }
                else
                {

                    var rectTF = new Rect(rect)
                    {
                        x = rect.x + 11,
                        y = rect.y + EditorGUIUtility.singleLineHeight * .5f,
                        height = rect.height - EditorGUIUtility.singleLineHeight
                    };


                    var posRect_prop = new Rect(rect)
                    {
                        x = rect.x + 10, //左边距
                        y = rect.y + EditorGUIUtility.singleLineHeight * .5f,
                        height = EditorGUIUtility.singleLineHeight
                    };
                    posRect_prop.xMin = 140;

                    EditorGUILayout.BeginHorizontal();
                    rectTF.xMin = 20;
                    rectTF.width = 40;
                    EditorGUI.LabelField(rectTF, $"{index}");

                    rectTF.xMin = 40;
                    rectTF.width = 110;
                    var oldName = taget.names[index];
                    if (string.IsNullOrEmpty(oldName) && taget.monos[index])
                    {
                        oldName = BindableUtility.GetSafeName(taget.monos[index].name);
                        // Debug.Log($" name= {taget.monos[index]?.name} obj:{taget.monos[index]}");
                    }
                    taget.names[index] = EditorGUI.TextField(rectTF, oldName);
                    if (oldName != taget.names[index])
                        EditorUtility.SetDirty(taget);
                    PopUpComponentsDrawer.PropertyField(posRect_prop, element, GUIContent.none);
                    EditorGUILayout.EndHorizontal();

                }

            };

            if (displayAddButton)
            {
                orderList.onAddDropdownCallback = (Rect rect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent($"添加空对象"), false, onAddClick, null);
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
                    h += EditorGUI.GetPropertyHeight(element);//EditorGUIUtility.singleLineHeight * 0.5f;
                return h;
            };

            return orderList;
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

        #region  color

        public static string ColorText(string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }


        public static void LabelFieldStyle(Rect rect, string label, string color, int fontSize = 14)
        {
            HtmlGUIStyle.fontSize = fontSize;
            EditorGUI.LabelField(rect, BindableObjectStyle.ColorText(label, color), HtmlGUIStyle);
        }

        public static bool FoldoutStyle(Rect position, bool foldout, string text, string color, int fontSize = 14)
        {
            HtmlFoldoutGUIStyle.fontSize = fontSize;
            return EditorGUI.Foldout(position, foldout, new GUIContent(BindableObjectStyle.ColorText(text, color)), true, HtmlFoldoutGUIStyle);
        }

        private static GUIStyle m_HtmlGUIStyle;
        public static GUIStyle HtmlGUIStyle
        {
            get
            {
                if (m_HtmlGUIStyle == null)
                {
                    m_HtmlGUIStyle = new GUIStyle(EditorStyles.label);
                    m_HtmlGUIStyle.richText = true;
                    m_HtmlGUIStyle.fontSize = 12;
                }
                return m_HtmlGUIStyle;
            }
        }


        private static GUIStyle m_HtmlFoldoutGUIStyle;
        public static GUIStyle HtmlFoldoutGUIStyle
        {
            get
            {
                if (m_HtmlFoldoutGUIStyle == null)
                {
                    m_HtmlFoldoutGUIStyle = new GUIStyle(EditorStyles.foldout);
                    m_HtmlFoldoutGUIStyle.richText = true;
                    m_HtmlFoldoutGUIStyle.fontSize = 12;
                }
                return m_HtmlFoldoutGUIStyle;
            }
        }


        public static GUIStyle ToolbarSeachTextField = new GUIStyle(EditorStyles.toolbarSearchField);
        // public 
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
            var target = binding.target;
            var property = binding.propertyName;
            var path = binding.path;
            // var format = binding.format;
            BindingMode mode = binding.mode;
            var converter = binding.converter;
            var source = binding.source;
            if (target != null)
                sb.Append(target.ToString());
            if (!string.IsNullOrEmpty(path))
                sb.AppendFormat(".{0}=({1}) ", property, path);
            // if (!string.IsNullOrEmpty(format))
            //     sb.AppendFormat("format({0}) ", format);
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