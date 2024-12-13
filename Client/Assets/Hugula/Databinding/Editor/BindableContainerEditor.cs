using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Hugula.Databinding.Binder;

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


            reorderableList_bindings = BindableUtility.CreateMulitTargetsBinderReorder(serializedObject, m_Property_bindings,
            target, true, true, true, true, OnAddClick, null);

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

            if (target is BindableContainer)
                CheckNamesAndMonos((BindableContainer)target);

        }

        bool OnChildrenFilter(SerializedProperty property, string searchText)
        {

            var refObj = property.objectReferenceValue as BindableObject;
            if (refObj == null)
            {
                return false;
            }
            var bingTarget = refObj.ToString();

            var bindings = refObj.GetBindings();
            if (!string.IsNullOrEmpty(searchText))  //搜索
            {
                var id2 = bingTarget.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0;
                if (id2)
                {
                    return false; //
                }
                if (bindings != null)
                {
                    foreach (var binding in bindings)
                    {
                        if (binding.propertyName.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            return false;
                        }
                        if (binding.path.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            return false;
                        }
                    }
                }

                return true; //不显示
            }

            return false;
        }

        void OnAddClick(object args)
        {
            if (args is BindableObject bindableObject)
            {
                var bindable = ((BindableObject)target);
                BindableUtility.AddEmptyBinding(bindable, string.Empty);

            }
            else if (args is object[] arr)
            {
                var per = (PropertyInfo)arr[0];
                var bindable = ((BindableObject)target);
                var property = per.Name;
                BindableUtility.AddEmptyBinding(bindable, property);
            }
        }


        void OnMonoAddClick(object args)
        {
            var bindableContainer = ((BindableContainer)target);
            AddMonos(bindableContainer, -1, null);
        }


        public override void OnInspectorGUI()
        {
            //Property
            base.OnInspectorGUI();

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
            // if (GUILayout.Button("auto add hierarchy  children to lua monos"))
            // {
            //     AutoAddToLuaMonosHierarchyChildren(temp);
            //     EditorUtility.SetDirty(target);
            // }
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
            if (children != null)
            {
                for (int i = 0; i < children.Count;)
                {
                    if (children[i] != null)
                        i++;
                    else
                        children.RemoveAt(i);
                }
            }


            var monos = container.monos;
            var names = container.names;
            if (monos != null && names != null)
            {
                for (int i = 0; i < monos.Count;)
                {
                    var obj = monos[i];
                    if (obj != null)
                    {
                        if (names.Count <= i)
                        {
                            var name = names.Contains(obj.name) ? obj.name + i.ToString() : obj.name;
                            names.Add(name);
                        }
                        else if (string.IsNullOrEmpty(names[i]))
                        {
                            var name = names.Contains(obj.name) ? obj.name + i.ToString() : obj.name;
                            names[i] = name;
                        }
                        i++;
                    }
                    else
                        monos.RemoveAt(i);
                }

                while (names.Count > monos.Count)
                {
                    names.RemoveAt(names.Count - 1);
                }
            }

            AddHierarchyChildren(container.transform, container, true);
        }

        public static void AddHierarchyChildren(Transform transform, BindableContainer container, bool toBinding = true)
        {
            List<BindableObject> oldChildren = null;
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

                oldChildren = container.children;

                var children = childTrans.GetComponents<BindableObject>(); //当前子节点的所有可绑定对象
                bool needDeep = true;
                bool isSelf = false;

                //
                foreach (var child in children)
                {
                    isSelf = System.Object.Equals(child, container);
                    var list = child.GetField("bindings");
                    if (!isSelf && list is List<Binding> bindings && bindings.Count > 0)
                    {
                        if (oldChildren == null || oldChildren.IndexOf(child) == -1)
                        {
                            container.AddChild(child);
                            oldChildren = container.children;
                        }
                    }

                    if (!isSelf && child is ICollectionBinder) //如果遇到容器不需要遍历
                        needDeep = false;

                }

                if (needDeep)
                {
                    AddHierarchyChildren(childTrans, container, toBinding);
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
            if (refer.monos == null || refer.names == null)
            {
                return;
            }

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

        [MenuItem("CONTEXT/BindableObject/Test Add All Bindings")]
        static void TestAllBindings(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableObject;

            if (bo != null)
            {
                //反射得到当前bo的所有非基层公开属性
                var type = bo.GetType();
                //不显示基础的属性

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var per in properties)
                {
                    var b = BindableUtility.AddEmptyBinding(bo, per.Name);
                    b.path = per.Name;
                }

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


        [MenuItem("CONTEXT/BindableContainer/ConvertToSharedContextBinder")]
        static void ConvertToSharedContextBinder(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableContainer;
            var gObj = bo.gameObject;
            if (bo != null)
            {
                //for self  
                var children = bo.children;
                var sharedContextBinder = gObj.CheckAddComponent<SharedContextBinder>();
                sharedContextBinder.GetBindings()?.Clear();

                var selfBindings = bo.GetBindings();
                if (selfBindings != null)
                {
                    foreach (var binding in selfBindings)
                    {
                        var newBinding = binding.Clone();
                        newBinding.target = sharedContextBinder;
                        BindableUtility.AddEmptyBinding(sharedContextBinder, newBinding);
                    }
                    bo.GetBindings().Clear();
                }

                if (children != null)
                {
                    for (int i = 0; i < children.Count;)
                    {
                        var child = children[i];
                        UnityEngine.Object target = child;
                        if (!PrefabUtility.IsPartOfAnyPrefab(child.gameObject))
                        {
                            CheckMoveChildBindingsToNewBO(child, sharedContextBinder, true);
                            children.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                            Debug.LogWarningFormat("{0} is part of prefab", child);
                        }
                    }

                }

            }
        }


        [MenuItem("CONTEXT/BindableContainer/MoveCheckedChildrenToBindings")]
        static void MoveChildrenToBindings(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableContainer;
            if (bo != null)
            {
                var children = bo.children;
                for (int i = 0; i < children.Count;)
                {
                    var child = children[i];
                    var isPartOfAnyPrefab = PrefabUtility.IsPartOfAnyPrefab(child.gameObject);
                    if (isPartOfAnyPrefab)
                        Debug.LogFormat("{0} isPartOfAnyPrefab={1}", child, isPartOfAnyPrefab);
                    if (!isPartOfAnyPrefab && (child is TextMeshProUGUIBinder))
                    {
                        CheckMoveChildBindingsToNewBO(child, bo);
                        children.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                EditorUtility.SetDirty(bo);

            }
        }


        static bool CheckMoveChildContextBindingToNewBO(BindableObject child, BindableObject newBindablobj, UnityEngine.Object target)
        {
            var contextBinding = child.GetContextBinding();
            if (contextBinding != null)
            {
                var newBinding = contextBinding.Clone();
                if (newBinding.target == null || newBinding.target == child)
                    newBinding.target = newBindablobj;

                BindableUtility.AddEmptyBinding(newBindablobj, newBinding);
                child.GetBindings().Remove(contextBinding); //移除旧的
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移动子节点到新的bo
        /// </summary>
        /// <param name="child"></param>
        /// <param name="newBindablobj"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        static bool CheckMoveChildBindingsToNewBO(BindableObject child, BindableObject newBindablobj, bool dontRm = false)
        {
            var bindings = child.GetBindings();
            var contextBinding = child.GetContextBinding();
            bool needRm = true;

            if (contextBinding != null) //如果有上下文不移除
            {
                needRm = false;
                var newBinding = contextBinding.Clone(); //保留上下文

                BindableUtility.AddEmptyBinding(newBindablobj, newBinding);
                Debug.LogFormat("{0} add contextBinding {1} ", newBindablobj, newBinding);
                child.GetBindings().Remove(contextBinding); //移除旧的
            }
            else
            {
                foreach (var binding in bindings)
                {
                    var newBinding = binding.Clone();
                    if (newBinding.target == null) //如果上下文的target为空 
                    {
                        newBinding.target = child;
                    }

                    if (child is TextMeshProUGUIBinder textMeshProUGUIBinder)
                    {
                        newBinding.target = textMeshProUGUIBinder.target;
                    }                   

                    //check property
                    if (newBinding.propertyName == "activeSelf")
                    {
                        newBinding.propertyName = "active";
                        newBinding.target = child.gameObject;
                    }
                    else if (!BindableUtility.CheckHasProperty(newBinding.target, newBinding.propertyName)) //属性对不上
                    {
                        needRm = false;
                        Debug.LogWarningFormat("{0} has no property {1} ,keep old target={2}", newBinding.target, newBinding.propertyName, child);
                        newBinding.target = child;//如果属性对不上则保留目标
                    }

                    BindableUtility.AddEmptyBinding(newBindablobj, newBinding);
                    Debug.LogFormat("{0} add binding {1} ", newBindablobj, newBinding);
                }
                child.GetBindings().Clear();
            }

            EditorUtility.SetDirty(child);
            //删除目标
            if (needRm && !dontRm)
            {
                GameObject.DestroyImmediate(child);
            }

            return needRm;
        }


        [MenuItem("CONTEXT/BindableContainer/MoveChildrenToBindings", true)]
        static bool ValidateMoveChildrenToBindings(MenuCommand menuCommand)
        {
            var bo = menuCommand.context as BindableContainer;
            if (bo != null)
            {
                return bo.children.Count > 0;
            }
            return false;
        }


    }
}