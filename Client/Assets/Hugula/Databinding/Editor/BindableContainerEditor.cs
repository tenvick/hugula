using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using Hugula.Databinding.Binder;
using UnityEditor;
using UnityEngine;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableContainer), true)]
    public class BindableContainerEditor : UnityEditor.Editor
    {
        public const string DELETE_TIPS = "choose item for detail or  delete!";

        List<int> selectedList = new List<int>();

        SerializedProperty property_children;
        SerializedProperty m_Property_bindings;
        string propertyName = string.Empty;
        string searchText = string.Empty;
        bool searchResultDirty;
        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            property_children = serializedObject.FindProperty("children");
            searchResultDirty = false;
            // searchText = string.Empty;
            propertyName = string.Empty;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            EditorGUILayout.Separator();
            var temp = target as BindableContainer;

            var rect = EditorGUILayout.BeginVertical(GUILayout.Height(100));
            EditorGUI.HelpBox(rect, "", MessageType.None);

            EditorGUILayout.LabelField("Drag(UIComponet) to there for add", GUILayout.Height(20));
            UnityEngine.Component addComponent = null;
            addComponent = (UnityEngine.Component)EditorGUILayout.ObjectField(addComponent, typeof(UnityEngine.Component), true, GUILayout.Height(40));
            if (GUILayout.Button("auto add hierarchy  children"))
            {
                //清理
                // var children = temp.children;
                // for (int i = 0; i < children.Count;)
                // {
                //     if (children[i] != null)
                //         i++;
                //     else
                //         children.RemoveAt(i);
                // }
                // AddHierarchyChildren(temp.transform, temp, true);
                AutoAddHierarchyChildren(temp);
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            // var rect1 = rect;
            float w = rect.width;
            var toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("bindings: " + m_Property_bindings.arraySize, GUILayout.Width(100), toolbarHeight);
            }
            var rect1 = EditorGUILayout.BeginHorizontal(GUILayout.Height(34));
            EditorGUI.HelpBox(rect1, "", MessageType.None);

            rect1.height -= BindableObjectStyle.kExtraSpacing * 2;
            rect1.x += BindableObjectStyle.kExtraSpacing;
            rect1.y += BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .4f;
            rect1.x = rect1.xMax + BindableObjectStyle.kExtraSpacing;
            rect1.width = w * .4f;

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
            //show databindings
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
                            m_Property_bindings.RemoveElement(i); // DeleteArrayElementAtIndex(i);
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

            EditorGUILayout.Space();

            //
            if (property_children.isArray)
            {
                selectedList.Clear();
                serializedObject.Update();
                var len = property_children.arraySize;
                toolbarHeight = GUILayout.Height(BindableObjectStyle.kSingleLineHeight);

                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label("Children: " + len, GUILayout.Width(100), toolbarHeight);
                    EditorGUI.BeginChangeCheck();
                    {
                        searchText = EditorGUILayout.TextField(string.Empty, searchText, new GUIStyle("ToolbarSeachTextField"), GUILayout.Width(160), toolbarHeight);
                        if (GUILayout.Button("Close", "ToolbarSeachCancelButtonEmpty"))
                        {
                            // reset text
                            searchText = null;
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        searchResultDirty = true;
                    }
                }

                SerializedProperty bindableProperty;
                string childName;
                List<string> childBindingInfo;
                for (int i = 0; i < len; i++)
                {
                    bindableProperty = property_children.GetArrayElementAtIndex(i);
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        var childBindable = (BindableObject)bindableProperty.objectReferenceValue;

                        childName = childBindable?.name;
                        childBindingInfo = childBindable?.GetBindingSourceList();
                        //check 绑定表达式
                        bool has = false;
                        if (childBindingInfo != null)
                        {
                            foreach (var s in childBindingInfo)
                            {
                                if (s.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                                {
                                    has = true;
                                    break;
                                }
                            }

                        }
                        if (childName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0 || has) // item.ExtraInfo.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            EditorGUILayout.PropertyField(bindableProperty, false);
                        }
                        else
                        {
                            bindableProperty.isExpanded = false;
                        }

                    }
                    else
                        EditorGUILayout.PropertyField(bindableProperty, false);

                    if (bindableProperty.isExpanded)
                    {
                        selectedList.Add(i);
                    }
                }

                //删除数据
                float width = 0;
                rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                rect.y += BindableObjectStyle.kExtraSpacing;
                if (selectedList.Count > 0)
                {
                    width = 260;
                    rect.x = rect.xMax - width;
                    rect.width = width;
                    if (GUI.Button(rect, "del BindalbeObject " + selectedList.Count))
                    {
                        selectedList.Sort((int a, int b) =>
                        {
                            if (a < b) return 1;
                            else if (a == b) return 0;
                            else
                                return -1;
                        });
                        foreach (var i in selectedList)
                            temp.children.RemoveAt(i);
                        // property_children.RemoveElement(i);
                    }
                }
                else
                {
                    width = DELETE_TIPS.Length * BindableObjectStyle.kExtraSpacing;
                    rect.x = rect.xMax - width;
                    rect.width = width;
                    GUI.Box(rect, DELETE_TIPS);
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.Separator();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (addComponent)
            {
                AddbindableObjects(temp, addComponent); //allcomps[allcomps.Length - 1]);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            //EditorUtility.SetDirty (target);
        }

        List<string> allowTypes = new List<string>();
        List<Type> allowTypeProperty = new List<Type>();
        List<bool> allowIsMethod = new List<bool>();

        UnityEngine.Object PopupGameObjectComponents(UnityEngine.Object obj, int i)
        {
            UnityEngine.Object selected = null;

            int selectIndex = 0;
            allowTypes.Clear();
            if (obj != null)
            {

                Type currentType = obj.GetType();
                GameObject go = null;
                if (currentType == typeof(GameObject))
                    go = (GameObject)obj;
                else if (currentType.IsSubclassOf(typeof(Component)))
                    go = ((Component)obj).gameObject;

                allowTypes.Add(string.Format("{0}({1})", go.GetType().Name, go.name));

                if (go != null)
                {
                    Component[] comps = go.GetComponents(typeof(Component)); //GetComponents<Component>();
                    Component item = null;
                    for (int j = 0; j < comps.Length; j++)
                    {
                        item = comps[j];
                        if (item)
                            allowTypes.Add(string.Format("{0}({1})", item.GetType().Name, item.name));
                        if (obj == item)
                            selectIndex = j + 1;
                    }
                    selectIndex = EditorGUILayout.Popup(selectIndex, allowTypes.ToArray());
                    if (selectIndex == 0)
                        selected = go;
                    else
                        selected = comps[selectIndex - 1];
                }

            }
            return selected;
        }

        GUIStyle BindingPropertiesStyle = new GUIStyle();
        void SetBindingProperties(BindableContainer refer, int i)
        {
            BindingPropertiesStyle.fontStyle = FontStyle.Italic;
            BindingPropertiesStyle.fontSize = 10;
            BindingPropertiesStyle.alignment = TextAnchor.UpperLeft;
        }

        public void AddbindableObjects(BindableContainer refer, UnityEngine.Component obj)
        {

            if (refer.children == null) //需要新增加
            {
                List<Binding> bindings = new List<Binding>();
                refer.children = new List<BindableObject>(); // bindings.ToArray ();
            }

            var children = refer.children;
            BindableObject bindable = null;

            if (obj is BindableObject)
            {
                bindable = obj as BindableObject;
            }
            else
            {

                if (bindable == null)
                {
                    Component[] allcomps = obj.GetComponents<Component>(); //默认绑定最后一个组件
                    Component target = null;
                    Type addType = typeof(Hugula.Databinding.BindableObject);
                    Type findType;
                    foreach (var comp in allcomps)
                    {
                        if (comp is BindableObject)
                        {
                            bindable = (BindableObject)comp;
                            break;
                        }
                        else if ((findType = BindalbeObjectUtilty.FindBinderType(comp.GetType())) != null)
                        {
                            target = comp;
                            addType = findType;
                        }
                    }

                    if (bindable == null)
                    {
                        bindable = (BindableObject)obj.gameObject.AddComponent(addType);
                    }
                }
            }

            if (bindable != null && children.IndexOf(bindable) < 0)
            {
                refer.AddChild(bindable);
            }
        }

        // public void RemoveAtbindableObjects(BindableContainer refer, int index)
        // {
        //     var children = refer.children;
        //     children.RemoveAt(index);
        // }

        public static void AutoAddHierarchyChildren(BindableContainer container)
        {
            var children = container.children;
            for (int i = 0; i < children.Count;)
            {
                if (children[i] != null)
                    i++;
                else
                    children.RemoveAt(i);
            }

            AddHierarchyChildren(container.transform, container, true);
        }

        public static void AddHierarchyChildren(Transform transform, BindableContainer container, bool checkChildren = false)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                var childTrans = transform.GetChild(i);
                var children = childTrans.GetComponents<BindableObject>(); //当前子节点的所有可绑定对象
                bool needDeep = true;
                bool isSelf = false;
                var oldChildren = container.children;
                foreach (var child in children)
                {
                    isSelf = System.Object.Equals(child, container);
                    if (oldChildren.IndexOf(child) == -1 && !isSelf)
                    {
                        container.AddChild(child);
                    }

                    if (!isSelf && child is ICollectionBinder) //如果遇到容器不需要遍历
                        needDeep = false;

                }

                if (needDeep)
                {
                    AddHierarchyChildren(childTrans, container);
                }
            }

            //var oldChildren = container.children;
            //if (oldChildren == null)
            //{
            //    oldChildren = new List<BindableObject>();
            //    container.children = oldChildren;
            //}

            //bool needDeep = true;
            //bool isSelf = false;
            //foreach (var child in children)
            //{
            //    isSelf = System.Object.Equals(child, container);
            //    if (oldChildren.IndexOf(child) == -1 && !isSelf)
            //    {
            //        container.AddChild(child);
            //    }

            //    if (!isSelf && child is ICollectionBinder) //如果遇到容器不需要遍历
            //        needDeep = false;
            //}

            //if (needDeep)
            //{
            //    for (int i = 0; i < transform.childCount; i++)
            //    {
            //        AddHierarchyChildren(transform.GetChild(i), container);
            //    }
            //}

        }

    }

    public static class BinableObjectMenu
    {
        [MenuItem("CONTEXT/BindableContainer/Clear All Children")]
        static void ClearAllChildren(MenuCommand menuCommand)
        {
            var bc = menuCommand.context as BindableContainer;

            if (bc != null)
            {
                bc.children.Clear();
                Debug.LogFormat("{0} clear ", bc);
            }
        }

        [MenuItem("CONTEXT/BindableObject/Clear All Bindings")]
        static void ClearAllBindings(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableObject;

            if (bo != null)
            {
                bo.GetBindings().Clear();
                Debug.LogFormat("{0} clear ", bo);
            }
        }
    }
}