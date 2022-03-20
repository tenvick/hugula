using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

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
        ReorderableList reorderableList_bindings;
        ReorderableList reorderableList_children;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            property_children = serializedObject.FindProperty("children");
            searchResultDirty = false;
            // searchText = string.Empty;
            propertyName = string.Empty;

            reorderableList_bindings = BindalbeObjectUtilty.CreateBindalbeObjectBindingsReorder(serializedObject, m_Property_bindings,
            target, true, true, true, true, OnAddClick, OnFilter);

            reorderableList_children = BindalbeObjectUtilty.CreateBindalbeObjectBindingsReorder(serializedObject, property_children, target, true, true, false, true, null, OnFilter);

            reorderableList_children.onRemoveCallback = (ReorderableList orderlist) =>
                      {
                          Debug.Log(orderlist);
                          if (UnityEditor.EditorUtility.DisplayDialog("warnning", "Do you want to remove this element?", "remove", "canel"))
                          {
                              ReorderableList.defaultBehaviours.DoRemoveButton(orderlist);
                          }
                      };

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
            var bindable = ((BindableObject)target);
            var property = per.Name;
            BindalbeObjectUtilty.AddEmptyBinding(bindable, property);
        }

        public override void OnInspectorGUI()
        {
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

            EditorGUILayout.Separator();
            EditorGUILayout.EndHorizontal();
            //show databindings
            serializedObject.Update();
            reorderableList_bindings.DoLayoutList();
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.Space();

            serializedObject.Update();
            reorderableList_children.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (addComponent)
            {
                AddbindableObjects(temp, addComponent); //allcomps[allcomps.Length - 1]);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

        }

        GUIStyle BindingPropertiesStyle = new GUIStyle();

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
                if (childTrans.name.ToLower().EndsWith(":ignore"))
                {
                    Debug.LogWarning($"{childTrans} 不会添加到容器{container} ");
                    continue;
                }
                var ignorePeerBinder = childTrans.GetComponents<IIgnorePeerBinder>();
                if (ignorePeerBinder != null && ignorePeerBinder.Length > 0)
                {
                    Debug.LogWarning($"{childTrans} 不会添加到容器{container} ");
                    continue;
                }
                //

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