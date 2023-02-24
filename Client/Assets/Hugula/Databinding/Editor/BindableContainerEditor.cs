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
        SerializedProperty property_monos;
        SerializedProperty property_children;
        SerializedProperty m_Property_bindings;
        ReorderableList reorderableList_bindings;
        ReorderableList reorderableList_children;

        ReorderableList reorderableList_monos;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");
            property_children = serializedObject.FindProperty("children");
            property_monos = serializedObject.FindProperty("monos");


            reorderableList_bindings = BindableUtility.CreateBindalbeObjectBindingsReorder(serializedObject, m_Property_bindings,
            target, true, true, true, true, OnAddClick, OnFilter);

            reorderableList_children = BindableUtility.CreateBindalbeObjectBindingsReorder(serializedObject, property_children, target, true, true, false, true, null, OnChildrenFilter);

            reorderableList_children.onRemoveCallback = (ReorderableList orderlist) =>
                      {
                          if (UnityEditor.EditorUtility.DisplayDialog("warnning", "Do you want to remove this element?", "remove", "canel"))
                          {
                              //   ReorderableList.defaultBehaviours.DoRemoveButton(orderlist);
                              ((BindableContainer)target).children.RemoveAt(orderlist.index);
                          }
                      };

            reorderableList_monos = BindableUtility.CreateMonoContainerChildrenReorder(serializedObject, property_monos, target, true, true, true, true, OnMonoAddClick, OnChildrenFilter);


            reorderableList_monos.onRemoveCallback = (ReorderableList orderlist) =>
                       {
                           RemoveMonos((BindableContainer)target, orderlist.index);
                       };

            if(target is BindableContainer)
                CheckNamesAndMonos((BindableContainer)target);

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

        bool OnChildrenFilter(SerializedProperty property, string searchText)
        {
            var refObj = property.objectReferenceValue;
            if (refObj == null)
            {
                return false;
            }
            var displayName = refObj.name;
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
            BindableUtility.AddEmptyBinding(bindable, property);
        }


        void OnMonoAddClick(object args)
        {
            var bindableContainer = ((BindableContainer)target);
            AddMonos(bindableContainer, -1, null);
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            var temp = target as BindableContainer;
            var rect = EditorGUILayout.BeginVertical(GUILayout.Height(100));
            EditorGUI.HelpBox(rect, "", MessageType.None);

            EditorGUILayout.LabelField("Drag(UIComponet) to there for add to Children(BindableObject)", GUILayout.Height(20));
            UnityEngine.Component addComponent = null;
            addComponent = (UnityEngine.Component)EditorGUILayout.ObjectField(addComponent, typeof(UnityEngine.Component), true, GUILayout.Height(40));
            if (GUILayout.Button("auto add hierarchy  children"))
            {
                AutoAddHierarchyChildren(temp);
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("auto add hierarchy  children to lua monos"))
            {
                AutoAddToLuaMonosHierarchyChildren(temp);
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            //show databindings
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            serializedObject.Update();
            reorderableList_bindings.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            //show children
            EditorGUILayout.Space();
            serializedObject.Update();
            reorderableList_children.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            //show mono
            EditorGUILayout.Space();
            serializedObject.Update();
            reorderableList_monos.DoLayoutList();
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
                        else if ((findType = BindableUtility.FindBinderType(comp.GetType())) != null)
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

        internal static void AutoAddToLuaMonosHierarchyChildren(BindableContainer container)
        {
            var children = container.children;
            for (int i = 0; i < children.Count;)
            {
                if (children[i] != null)
                    i++;
                else
                    children.RemoveAt(i);
            }

            AddHierarchyChildren(container.transform, container, false);

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


            var monos = container.monos;
            var names = container.names;

            for (int i = 0; i < monos.Count;)
            {
                var obj = monos[i];
                if (obj != null)
                {
                    if(names.Count<=i)
                    {
                        var name = names.Contains(obj.name)?obj.name+i.ToString():obj.name;
                        names.Add(name);
                    }
                    else if(string.IsNullOrEmpty(names[i]))
                    {
                        var name = names.Contains(obj.name)?obj.name+i.ToString():obj.name;
                        names[i] = name;
                    }
                    i++;
                }
                else
                    monos.RemoveAt(i);
            }

            while(names.Count>monos.Count)
            {
                names.RemoveAt(names.Count-1);
            }

            AddHierarchyChildren(container.transform, container, true);
        }

        public static void AddHierarchyChildren(Transform transform, BindableContainer container, bool toBinding = true)
        {
            List<BindableObject> oldChildren = null;
            List<UnityEngine.Object> oldMonos = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                var childTrans = transform.GetChild(i);
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

                if (!toBinding)
                {
                    oldMonos = container.monos;
                    oldChildren = null;
                }
                else
                {
                    oldChildren = container.children;
                    oldMonos = null;
                }

                var children = childTrans.GetComponents<BindableObject>(); //当前子节点的所有可绑定对象
                bool needDeep = true;
                bool isSelf = false;

                //
                foreach (var child in children)
                {
                    isSelf = System.Object.Equals(child, container);
                    if (oldChildren != null && oldChildren.IndexOf(child) == -1 && !isSelf)
                    {
                        container.AddChild(child);
                    }
                    else if (oldMonos != null && oldMonos.IndexOf(child) == -1 && !isSelf)
                    {
                        oldMonos.Add(child);
                        //check name
                        var names = container.names;
                        var name = names.Contains(child.name)?child.name+oldMonos.Count.ToString():child.name;
                        if(names.Count<=oldMonos.Count)
                        {
                            names.Add(name);
                        }
                    }

                    if (!isSelf && child is ICollectionBinder) //如果遇到容器不需要遍历
                        needDeep = false;

                }

                if (needDeep)
                {
                    AddHierarchyChildren(childTrans, container,toBinding);
                }
            }
        }

        internal static void AddMonos(BindableContainer refer, int i, UnityEngine.Object obj)
        {
            List<UnityEngine.Object> monos = refer.monos;

            if (monos == null)
            {
                monos = new List<UnityEngine.Object>();
                refer.monos = monos;
            }

            if (i < 0)
            {
                monos.Add(obj);
            }
            else
            {
                while (monos.Count <= i)
                    monos.Add(null);
                monos[i] = obj;
            }
        }

        internal static void CheckNamesAndMonos(BindableContainer refer)
        {
            while (refer.names.Count < refer.monos.Count)
            {
                refer.names.Add(refer.monos[refer.names.Count].name);
            }

            while (refer.names.Count > refer.monos.Count)
            {
                refer.names.RemoveAt(refer.names.Count - 1);
            }
        }

        internal static void RemoveMonos(BindableContainer refer, int i)
        {
            if (refer.monos != null)
            {
                CheckNamesAndMonos(refer);
                refer.monos.RemoveAt(i);
                refer.names.RemoveAt(i);
            }

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
                Debug.LogFormat("{0} children clear ", bc);
                // bc.names.Clear();
                // bc.monos?.Clear();
                // Debug.LogFormat("{0} mono clear ", bc);

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

        [MenuItem("CONTEXT/BindableContainer/Clear All Monos")]
        static void ClearAllMonos(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableContainer;

            if (bo != null)
            {
                bo.monos.Clear();
                bo.names.Clear();
                Debug.LogFormat("{0} clear ", bo);
            }
        }
    }
}