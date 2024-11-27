using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Hugula.Databinding.Binder;
using UnityEngine.UI;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(SharedContextBinder), true)]
    public class SharedContextBinderEditor : UnityEditor.Editor
    {
        // SerializedProperty property_monos;
        // SerializedProperty property_children;
        SerializedProperty m_Property_bindings;
        ReorderableList reorderableList_bindings;
        ReorderableList reorderableList_children;

        ReorderableList reorderableList_monos;

        void OnEnable()
        {
            m_Property_bindings = serializedObject.FindProperty("bindings");

            reorderableList_bindings = BindableUtility.CreateMulitTargetsBinderReorder(serializedObject, m_Property_bindings,
            target, true, true, true, true, OnAddClick, null);


            if (target is SharedContextBinder)
                CheckNamesAndMonos((SharedContextBinder)target);

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
            var bindable = (BindableObject)args;
            BindableUtility.AddEmptyBinding(bindable, bindable, string.Empty);
        }



        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();
            var temp = target as SharedContextBinder;
            var rect = EditorGUILayout.BeginVertical(GUILayout.Height(100));
            EditorGUI.HelpBox(rect, "", MessageType.None);

            EditorGUILayout.LabelField("Drag(Componet) to there for add to Bindings list", GUILayout.Height(20));
            UnityEngine.Component addComponent = null;
            addComponent = (UnityEngine.Component)EditorGUILayout.ObjectField(addComponent, typeof(UnityEngine.Component), true, GUILayout.Height(40));
            if (GUILayout.Button("auto add hierarchy bindings children"))
            {
                AutoAddHierarchyChildren(temp);
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (addComponent)
            {
                AddBindings(temp, addComponent); //allcomps[allcomps.Length - 1]);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

        }

        public void AddBindings(SharedContextBinder refer, UnityEngine.Component obj)
        {

            Debug.Log($"AddbindableObjects {refer} {obj}");

            Component[] allcomps = obj.GetComponents<Component>(); //默认添加最后一个组件
            Component target = allcomps[allcomps.Length - 1];

            BindableUtility.AddEmptyBinding(refer, target, string.Empty);

        }

        internal static void CheckNamesAndMonos(SharedContextBinder refer)
        {

        }


        public static void AutoAddHierarchyChildren(BindableObject container)
        {
            var children = container.GetBindings();

            Hugula.Collections.Dictionary<UnityEngine.Object, string, string> bindingTargets = new Hugula.Collections.Dictionary<UnityEngine.Object, string, string>(); //new Dictionary<UnityEngine.Object, string>();
            if (children != null)
            {
                Binding binding;
                for (int i = 0; i < children.Count;i++)
                {
                    binding = children[i];

                    if (bindingTargets.TryGetValue(binding.target, binding.propertyName, out string path))
                    {
                        binding.path = path;
                        // children.RemoveAt(i);
                        Debug.LogError($"AutoAddHierarchyChildren index:{i} {binding} 重复添加  BindableObject：{Hugula.Utils.CUtils.GetFullPath(container)} ");
                    }
                    else
                    {
                        bindingTargets.Add(binding.target, binding.propertyName, binding.path);
                        bindingTargets[binding.target, string.Empty] = binding.path;   //添加一个空的标记控件有被绑定过     
                    }
                }
            }


            AddHierarchyChildren(container.transform, container, bindingTargets);
        }

        static void CheckContextOrAddBinding(BindableObject container, BindableObject child, string propertName, Hugula.Collections.Dictionary<UnityEngine.Object, string, string> bindingTargets)
        {
            var list = child.GetBindings();
            if (list == null || list.Count == 0) //没有绑定
            {
                BindableUtility.AddEmptyBinding(container, child, propertName, propertName.ToLower());
                bindingTargets.Add(child, propertName, propertName.ToLower());
            }
            else //有binding 绑定context
            {

                if (!bindingTargets.TryGetValue(child, "context", out string path))
                {
                    child.SetField("m_InitBindings", false);
                    var contextBinding = child.GetContextBinding();

                    if (contextBinding == null)
                    {
                        BindableUtility.AddEmptyBinding(container, child, "context", ".");
                    }
                    else
                    {
                        var Clone = contextBinding.Clone();
                        BindableUtility.AddEmptyBinding(container, Clone);
                        child.GetBindings()?.Remove(contextBinding);
                    }
                        bindingTargets.Add(child, "context", ".");
                }
            }
        }

        public static void AddHierarchyChildren(Transform transform, BindableObject container, Hugula.Collections.Dictionary<UnityEngine.Object, string, string> bindingTargets)
        {
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


                var children = childTrans.GetComponents<MonoBehaviour>(); //当前子节点的所有可绑定对象
                bool needDeep = true;
                bool isSelf = false;

                //
                foreach (var child in children)
                {
                    isSelf = System.Object.Equals(child, container);
                    if (!isSelf)
                    {
                        if (child is ButtonBinder buttonBinder)
                        {
                            if (!bindingTargets.TryGetValue(buttonBinder, "onClickCommand", out string path))
                            {
                                CheckContextOrAddBinding(container, buttonBinder, "onClickCommand", bindingTargets);
                            }
                        }
                        else if (child is TMPro.TextMeshProUGUI || child is Text)
                        {
                            if (!bindingTargets.TryGetValue(child, "text", out string path))
                            {
                                BindableUtility.AddEmptyBinding(container, child, "text", "text");
                                bindingTargets.Add(child, "text", "text");
                            }
                        }
                        else if (child is ImageBinder imageBinder)
                        {
                            CheckContextOrAddBinding(container, imageBinder, "spriteName", bindingTargets);                           
                        }
                        else if (child is TextBinder)
                        {

                        }
                        else if (child is BindableObject bindableObject)
                        {
                            if (!bindingTargets.TryGetValue(bindableObject, string.Empty, out string path))
                            {
                                bindableObject.SetField("m_InitBindings", false);
                                var contextBinding = bindableObject.GetContextBinding();

                                if (contextBinding == null)
                                    BindableUtility.AddEmptyBinding(container, child, "context", ".");
                                else
                                {
                                    var Clone = contextBinding.Clone();
                                    BindableUtility.AddEmptyBinding(container, Clone);
                                    bindableObject.GetBindings()?.Remove(contextBinding);
                                }
                                bindingTargets.Add(bindableObject, "context", ".");
                            }
                        }
                    }

                    if (!isSelf && (child is ICollectionBinder || child is ISharedContext || child is IIgnoreChildrenBinder)) //如果遇到容器不需要遍历
                        needDeep = false;

                }

                if (needDeep)
                {
                    AddHierarchyChildren(childTrans, container, bindingTargets);
                }
            }
        }

    }

    public static class SharedContextBinderMenu
    {

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

        /**
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
    
            **/
    }
}