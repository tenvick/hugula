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

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.hasMultipleDifferentValues)
            {
                float w = position.width;
                position.width = w * .6f;
                EditorGUI.ObjectField(position, property, typeof(UnityEngine.Object), label);
            }
            else
            {
                float w = position.width;
                position.width = w * .6f;
                EditorGUI.ObjectField(position, property, typeof(UnityEngine.Object), label);
                UnityEngine.Object content = null;//(UnityEngine.Component)property.objectReferenceValue;
                var obj = property.objectReferenceValue;
                if (obj != null && obj is UnityEngine.Object)
                {
                    Component[] comps;
                    GameObject go = null;
                    if (obj is GameObject)
                    {
                        go = (GameObject)obj;
                        comps = ((GameObject)obj).GetComponents<Component>();
                    }
                    else if (obj is Component)
                    {
                        go = ((Component)obj).gameObject;
                        comps = ((Component)obj).GetComponents<Component>();
                    }
                    else
                    {
                        Debug.LogFormat("PopUpComponentsDrawer.value={0}", obj);
                        return;
                    }

                    var m_AllComponents = Hugula.Utils.ListPool<string>.Get();
                    int selectIndex = 0;
                    int i = 0;
                    foreach (var comp in comps)
                    {
                        m_AllComponents.Add(comp.GetType().Name);
                        if (comp == obj) selectIndex = i;
                        i++;
                    }
                    //add gameobject
                    m_AllComponents.Add(go.GetType().Name);
                    if (go == obj) selectIndex = i;

                    position.x = position.xMax;
                    position.width = w - position.width;
                    selectIndex = EditorGUI.Popup(position, selectIndex, m_AllComponents.ToArray());

                    if (selectIndex < comps.Length)
                        content = comps[selectIndex];
                    else if (selectIndex == i)
                        content = go;

                    Hugula.Utils.ListPool<string>.Release(m_AllComponents);
                }
                property.objectReferenceValue = content;
            }

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static void PropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.hasMultipleDifferentValues)
            {
                float w = position.width;
                position.width = w * .6f;
                EditorGUI.ObjectField(position, property, typeof(UnityEngine.Object), label);
            }
            else
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

                    var m_AllComponents = Hugula.Utils.ListPool<string>.Get();
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
                    Hugula.Utils.ListPool<string>.Release(m_AllComponents);
                }
                property.objectReferenceValue = content;
            }

        }
    }


    [CustomPropertyDrawer(typeof(BindableObject), true)]
    public class BindableObjectProperty : PropertyDrawer
    {
        string propertyName = "";
        SerializedObject bindableObjectSerializedObject;
        ReorderableList reorderableListBindings;
        UnityEngine.Object currTarget;
        SerializedProperty m_Property_bindings;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUILayout.Separator();
            // base.OnGUI(position, property, label);
            var serializedObject = property.serializedObject;
            var target = property.objectReferenceValue;
            if (target is null) return;

            var temp = target as BindableObject;
            var tp = temp.GetType();
            var prop = tp.GetProperty("target", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                if (prop.GetValue(target) == null)
                    prop.SetValue(target, temp.GetComponent(prop.PropertyType));
            }

            var rect1 = position;
            float w = position.width;
            rect1.height = BindableObjectStyle.kSingleLineHeight;
            rect1.width = 46;
            //
            BindableObjectStyle.LabelFieldStyle(rect1, "target", "#90BC8Cff", 12);
            rect1.x = rect1.x + 46;
            rect1.width = w - 50;

            CustomBinder customer = null;
            bool refresh = false;
            if (temp is CustomBinder)
            {
                customer = (CustomBinder)temp;
                if (currTarget != customer.target)
                {
                    refresh = true;
                    currTarget = customer.target;
                }

                EditorGUI.ObjectField(rect1, customer.target, typeof(Component), false); //显示绑定对象
            }
            else
            {
                EditorGUI.ObjectField(rect1, temp, typeof(BindableObject), false); //显示绑定对象
            }

            // EditorGUILayout.Separator();
            if (reorderableListBindings == null || refresh)
            {
                bindableObjectSerializedObject = new SerializedObject(target);
                m_Property_bindings = bindableObjectSerializedObject.FindProperty("bindings");
                reorderableListBindings = BindableUtility.CreateBindalbeObjectBindingsReorder(bindableObjectSerializedObject, m_Property_bindings, target, true,
                true, true, true, OnAddClick, null);
            }

            //显示列表
            position.y += BindableObjectStyle.kSingleLineHeight * 1.1f;
            serializedObject.Update();
            if (bindableObjectSerializedObject != null) bindableObjectSerializedObject.Update();
            reorderableListBindings.DoList(position);
            if (bindableObjectSerializedObject != null) bindableObjectSerializedObject.ApplyModifiedProperties();

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
                {
                    h = (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 7f + BindableObjectStyle.kControlVerticalSpacing * 2;
                }
            }
            else
            {
                if (reorderableListBindings != null)
                    h = reorderableListBindings.GetHeight() + (BindableObjectStyle.kSingleLineHeight) * 2f;
                else
                    h = (EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 2f;
            }
            return h;
        }

        // bool OnFilter(SerializedProperty property, string searchText)
        // {
        //     var displayName = property.displayName;
        //     if (!string.IsNullOrEmpty(searchText) && displayName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) < 0) //搜索
        //     {
        //         return true;
        //     }
        //     else
        //         return false;

        // }

        void OnAddClick(object args)
        {
            var arr = (object[])args;
            var per = (PropertyInfo)arr[0];
            var bindable = (BindableObject)arr[1];
            var property = per.Name;
            BindableUtility.AddEmptyBinding(bindable, property);
        }
    }

    [CustomPropertyDrawer(typeof(Binding), true)]
    public class BindingDrawer : PropertyDrawer
    {

        static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        static GUIStyle m_FontStyle;//= new GUIStyle();
        static GUIStyle fontStyle
        {
            get
            {
                if (m_FontStyle == null)
                {
                    m_FontStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 12, // 设置字体大小
                        richText = true,
                        normal = { textColor = Color.green }, // 设置正常状态下的字体颜色
                        hover = { textColor = Color.green }, // 设置悬停状态下的字体颜色
                        active = { textColor = Color.blue } // 设置激活状态下的字体颜色
                    };
                }
                return m_FontStyle;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string propertyName = property.FindPropertyRelative("propertyName").stringValue;
            bool toggle = property.isExpanded;
            position.x += EditorGUIUtility.singleLineHeight;
            position.width -= EditorGUIUtility.singleLineHeight;
            if (toggle == false)
            {
                var path = property.FindPropertyRelative("path").stringValue;
                var partConfigs = property.FindPropertyRelative("partConfigs");
                BindingMode mode = (BindingMode)property.FindPropertyRelative("mode").enumValueIndex;
                var converter = property.FindPropertyRelative("converter").stringValue;
                var source = property.FindPropertyRelative("source").objectReferenceValue;

                sb.Clear();
                sb.Append(propertyName);
                sb.Append(":    ");

                if (!string.IsNullOrEmpty(path))
                    sb.AppendFormat("path({0})    ", path);
                // if (!string.IsNullOrEmpty(format))
                //     sb.AppendFormat("format({0})    ", format);
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
                int index = 0;
                bool isSharedContext = property.serializedObject is ISharedContext;
                int count = isSharedContext ? 6 : 5;

                var rects = GetRowRects(position, count);
                if (isSharedContext)
                    EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("target"));

                SerializedProperty partConfigsProperty = property.FindPropertyRelative("partConfigs");
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("path"));
                if (EditorGUI.EndChangeCheck())
                {
                    ParsePathToConfig(property);
                }

                //显示sb.Tostring的内容
                DisplayNamePartConfigsProperty(rects[index++], partConfigsProperty, fontStyle);
                EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("mode"));
                EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("converter"));
                EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("source"));
            }
        }

        internal static void DisplayNamePartConfigsProperty(Rect position, SerializedProperty partConfigsProperty, GUIStyle fontStyle)
        {
            sb.Clear();
            bool emptyPath = false;
            for (int i = 0; i < partConfigsProperty.arraySize; i++)
            {
                var element = partConfigsProperty.GetArrayElementAtIndex(i);
                var path = element.FindPropertyRelative("path").stringValue;
                if (string.IsNullOrEmpty(path))
                {
                    sb.Append("null");
                    emptyPath = true;
                }

                var flags = (BindingPathPartFlags)element.FindPropertyRelative("_flags").intValue;
                var isGlobal = (flags & BindingPathPartFlags.IsGlobal) != 0;
                if ((flags & Hugula.Databinding.BindingPathPartFlags.IsExpSetter) != 0)
                {
                    if (isGlobal)
                        sb.Append($"<color=red>${path}</color>");
                    else
                        sb.Append($"${path}");
                }
                else if ((flags & Hugula.Databinding.BindingPathPartFlags.IsExpGetter) != 0)
                {
                    if (isGlobal)
                        sb.Append($"<color=red>&{path}</color>");
                    else
                        sb.Append($"&{path}");
                }
                else if ((flags & Hugula.Databinding.BindingPathPartFlags.IsIndexer) != 0)
                {
                    sb.Append($"[{path}]");
                }
                else if ((flags & Hugula.Databinding.BindingPathPartFlags.IsMethod) != 0) //(_flags & BindingPathPartFlags.IsMethod) != 0
                {
                    sb.Append($"{path}()");
                }
                else
                    sb.Append(path);

                if (i < partConfigsProperty.arraySize - 1)
                    sb.Append(".");
            }

            sb.Append($"     Len:{partConfigsProperty.arraySize}");

            if (emptyPath)
                fontStyle.normal.textColor = Color.red;

            EditorGUI.LabelField(position, sb.ToString(), fontStyle);
        }


        static readonly char[] ExpressionSplit = new char[] { '.' };

        public static bool ParsePathToConfig(SerializedProperty bindingSerializedProperty)
        {
            var path = bindingSerializedProperty.FindPropertyRelative("path").stringValue;

            if (string.IsNullOrEmpty(path)) return false;

            // 创建一个临时实例（假设类名为Binding）
            Binding tempBinding = new Binding();
            tempBinding.ParsePathToConfig(path); // 调用之前已写好的解析逻辑

            // 获得解析好的partConfigs
            // var parsedPartConfigs = tempBinding.partConfigs;
            var fieldInfo = typeof(Binding).GetField("partConfigs", BindingFlags.NonPublic | BindingFlags.Instance);
            var parsedPartConfigs = (BindingPathPartConfig[])fieldInfo.GetValue(tempBinding);

            var partConfigsProp = bindingSerializedProperty.FindPropertyRelative("partConfigs");
            partConfigsProp.ClearArray();

            // 将解析结果写回SerializedProperty中
            for (int i = 0; i < parsedPartConfigs.Length; i++)
            {
                partConfigsProp.InsertArrayElementAtIndex(i);
                var element = partConfigsProp.GetArrayElementAtIndex(i);

                element.FindPropertyRelative("path").stringValue = parsedPartConfigs[i].path;

                // 根据parsedPartConfigs[i]的标志位设置Flags
                BindingPathPartFlags flags = 0;
                if (parsedPartConfigs[i].isIndexer) flags |= BindingPathPartFlags.IsIndexer;
                if (parsedPartConfigs[i].isExpSetter) flags |= BindingPathPartFlags.IsExpSetter;
                if (parsedPartConfigs[i].isExpGetter) flags |= BindingPathPartFlags.IsExpGetter;
                if (parsedPartConfigs[i].isGlobal) flags |= BindingPathPartFlags.IsGlobal;
                if (parsedPartConfigs[i].isSelf) flags |= BindingPathPartFlags.IsSelf;

                element.FindPropertyRelative("_flags").intValue = (int)flags;
            }

            return true;
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h;
            if (property.isExpanded)
            {
                // h = (BindableObjectStyle.kSingleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 7f + BindableObjectStyle.kControlVerticalSpacing * 2;
                h = BindableObjectStyle.kControlVerticalSpacing;
                h = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("target"), true);
                h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("path"), true);
                h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("mode"), true);
                h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("converter"), true);
                h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("source"), true);

                // 加上 'partConfigs' 列表的高度
                var listHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("partConfigs"), true);
                h += listHeight;
                h += BindableObjectStyle.kControlVerticalSpacing * 2;
            }
            else
            {
                h = (EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing) * 1f;
            }
            return h;
        }

        static Rect[] GetRowRects(Rect rect, int count = 5)
        {
            Rect[] rects = new Rect[count];
            rect.height = BindableObjectStyle.kSingleLineHeight;
            Rect rect0 = rect;
            rect0.y += 5;//EditorGUIUtility.singleLineHeight;//- BindableObjectStyle.kControlVerticalSpacing;//+ BindableObjectStyle.kControlVerticalSpacing;
            rects[0] = rect0;
            for (int i = 1; i < count; i++)
            {
                var rectItem = rects[i - 1];
                rectItem.y += EditorGUIUtility.singleLineHeight + BindableObjectStyle.kControlVerticalSpacing;
                rects[i] = rectItem;
            }
            return rects;
        }

        public static void OnMulitTargetsBinderGUI(Rect position, SerializedProperty property)
        {
            bool toggle = property.isExpanded;
            position.x += EditorGUIUtility.singleLineHeight;
            position.width -= EditorGUIUtility.singleLineHeight;

            fontStyle.fontSize = 12;
            fontStyle.fontStyle = FontStyle.Italic;

            position.width -= EditorGUIUtility.singleLineHeight * .5f;
            int index = 0;
            var rects = GetRowRects(position, 7);

            EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("target"));
            OnTargetPropertyChooseGUI(rects[index++], property);

            SerializedProperty partConfigsProperty = property.FindPropertyRelative("partConfigs");
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("path"));
            if (EditorGUI.EndChangeCheck())
            {
                ParsePathToConfig(property);
            }

            DisplayNamePartConfigsProperty(rects[index++], partConfigsProperty, fontStyle);
            EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("mode"));
            EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("converter"));
            EditorGUI.PropertyField(rects[index++], property.FindPropertyRelative("source"));
        }

        public static void OnTargetPropertyChooseGUI(Rect position, SerializedProperty property)
        {
            var target = property.FindPropertyRelative("target").objectReferenceValue;
            var propertyName = property.FindPropertyRelative("propertyName");
            string propertyNameValue = string.Empty;

            float w = position.width;
            position.width = w * .6f;

            if (target != null)
            {
                var propList = BindableUtility.GetObjectProperties(target);

                EditorGUI.PropertyField(position, propertyName, true);
                propertyNameValue = propertyName.stringValue;

                var m_AllComponents = Hugula.Utils.ListPool<string>.Get();
                int selectIndex = -1;
                int i = 0;
                bool findValue = false;
                int maxScore = 0;
                foreach (var pi in propList)
                {
                    m_AllComponents.Add(pi.Name);
                    var score = CalculateMatchScore(propertyNameValue, pi.Name);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        selectIndex = i;
                        findValue = true;
                    }
                    i++;
                }
                position.x = position.xMax;
                position.width = w - position.width;

                var currSelectIndex = EditorGUI.Popup(position, selectIndex, m_AllComponents.ToArray());
                if (findValue == false && !string.IsNullOrEmpty(propertyNameValue))
                {
                    Debug.LogError($" property:{propertyNameValue} does't find in target({target}) ");
                    if (currSelectIndex != selectIndex) //改变了选择
                    {
                        propertyNameValue = propList[currSelectIndex].Name;
                    }
                }
                else
                    propertyNameValue = propList[currSelectIndex].Name;

                Hugula.Utils.ListPool<string>.Release(m_AllComponents);
            }
            else
            {
                EditorGUI.PropertyField(position, propertyName, true);
                propertyNameValue = propertyName.stringValue;
            }
            propertyName.stringValue = propertyNameValue;
        }

        private static int CalculateMatchScore(string input, string name)
        {

            if (name.Equals(input, System.StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }
            else if (name.StartsWith(input, System.StringComparison.OrdinalIgnoreCase))
            {
                return 80;
            }
            else
            {
                int index = name.IndexOf(input, System.StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    return 60 - index;
                }
                else
                {
                    return 0;
                }
            }
        }

    }

    // [CustomPropertyDrawer(typeof(BindingPathPartConfig), true)]
    public class BindingPathPartConfigDrawer : PropertyDrawer
    {
        // 定义子属性的高度
        private const float padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 开始绘制属性
            EditorGUI.BeginProperty(position, label, property);

            // 计算绘制区域
            Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // 不缩进
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // 获取子属性
            SerializedProperty configNameProp = property.FindPropertyRelative("configName");
            SerializedProperty configValueProp = property.FindPropertyRelative("configValue");

            // 计算每个字段的高度
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float totalHeight = singleLineHeight * 2 + padding;

            // 绘制 configName
            Rect configNameRect = new Rect(contentRect.x, contentRect.y, contentRect.width, singleLineHeight);
            EditorGUI.PropertyField(configNameRect, configNameProp, new GUIContent("Config Name"));

            // 绘制 configValue
            Rect configValueRect = new Rect(contentRect.x, contentRect.y + singleLineHeight + padding, contentRect.width, singleLineHeight);
            EditorGUI.PropertyField(configValueRect, configValueProp, new GUIContent("Config Value"));

            // 恢复缩进
            EditorGUI.indentLevel = indent;

            // 结束绘制属性
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 每个 BindingPathPartConfig 显示两行属性
            return EditorGUIUtility.singleLineHeight * 2 + padding;
        }
    }

    [CustomPropertyDrawer(typeof(BindableContainer), true)]
    public class BindableContainerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var serializedObject = property.serializedObject;
            var target = property.objectReferenceValue;
            if (target is null) return;
            var rect = position;
            rect.width = 0.45f * position.width;
            EditorGUI.LabelField(rect, property.displayName);
            position.x = rect.width;
            position.width = position.width * .55f;
            EditorGUI.ObjectField(position, target, typeof(Component), false); //显示绑定对象

        }
    }
}