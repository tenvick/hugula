using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding.Binder;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableContainer), true)]
    public class BindableContainerEditor : UnityEditor.Editor
    {
        public const string DELETE_TIPS = "choose item for detail or  delete!";

        public List<Type> BinderCheckTypes = new List<Type>() {
            typeof (UnityEngine.UI.Text),
            typeof (UnityEngine.UI.Button),
            typeof (UnityEngine.UI.Image),
            typeof (UnityEngine.UI.InputField),
            typeof (UnityEngine.UI.Slider),
            typeof (Hugula.UIComponents.LoopScrollRect),
            typeof (Hugula.UIComponents.LoopVerticalScrollRect),
            typeof (UnityEngine.Animator),

#if USE_TMPro
            typeof (TMPro.TextMeshProUGUI),
            typeof(TMPro.TMP_InputField),
#endif
        };

        public List<Type> BinderCreateTypes = new List<Type>() {
            typeof (Hugula.Databinding.Binder.TextBinder),
            typeof (Hugula.Databinding.Binder.ButtonBinder),
            typeof (Hugula.Databinding.Binder.ImageBinder),
            typeof (Hugula.Databinding.Binder.InputFieldBinder),
            typeof (Hugula.Databinding.Binder.SliderBinder),
            typeof (Hugula.Databinding.Binder.LoopScrollRectBinder),
            typeof (Hugula.Databinding.Binder.LoopVerticalScrollRectBinder),
            typeof (Hugula.Databinding.Binder.AnimatorBinder),

#if USE_TMPro
            typeof (Hugula.Databinding.Binder.TextMeshProUGUIBinder),
            typeof(Hugula.Databinding.Binder.TMP_InputFieldBinder),
#endif
        };

        List<int> selectedList = new List<int>();

        SerializedProperty property_children;

        void OnEnable()
        {
            property_children = serializedObject.FindProperty("children");
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI ();
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
                var children = temp.children;
                for (int i = 0; i < children.Count;)
                {
                    if (children[i] != null)
                        i++;
                    else
                        children.RemoveAt(i);
                }
                AddHierarchyChildren(temp.transform, temp, true);
                EditorUtility.SetDirty (target);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bindable List");

            //
            if (property_children.isArray)
            {
                selectedList.Clear();
                serializedObject.Update();
                var len = property_children.arraySize;
                SerializedProperty bindingProperty;
                for (int i = 0; i < len; i++)
                {
                    bindingProperty = property_children.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(bindingProperty, false);
                    if (bindingProperty.isExpanded)
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
                            property_children.DeleteArrayElementAtIndex(i);
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
                    int index = 0;
                    int tpId = -1;
                    foreach (var comp in allcomps)
                    {
                        if (comp is BindableObject)
                        {
                            bindable = (BindableObject)comp;
                            break;
                        }
                        else if ((index = BinderCheckTypes.IndexOf(comp.GetType())) >= 0)
                        {
                            target = comp;
                            tpId = index;
                        }
                    }

                    if (bindable == null)
                    {
                        Type createBinderType = typeof(Hugula.Databinding.BindableObject);
                        if (tpId >= 0)
                        {
                            createBinderType = BinderCreateTypes[tpId];
                        }

                        bindable = (BindableObject)obj.gameObject.AddComponent(createBinderType);
                    }
                }
            }

            if (children.IndexOf(bindable) < 0)
            {
                refer.AddChild(bindable);
            }
        }

        // public void RemoveAtbindableObjects(BindableContainer refer, int index)
        // {
        //     var children = refer.children;
        //     children.RemoveAt(index);
        // }

        public void AddHierarchyChildren(Transform transform, BindableContainer container, bool checkChildren = false)
        {
            var children = transform.GetComponents<BindableObject>();


            var oldChildren = container.children;
            if (oldChildren == null)
            {
                oldChildren = new List<BindableObject>();
                container.children = oldChildren;
            }

            bool needDeep = true;
            bool isSelf = false;
            foreach (var child in children)
            {
                isSelf = System.Object.Equals(child, container);
                if (oldChildren.IndexOf(child) == -1 && !isSelf)
                {
                    container.AddChild(child);
                }

                if (!isSelf && (child is BindableContainer ||
                child is LoopScrollRectBinder ||
                child is LoopVerticalScrollRectBinder ||
                child is CollectionViewBinder)) //如果遇到容器不需要遍历
                    needDeep = false;
            }

            if (needDeep)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    AddHierarchyChildren(transform.GetChild(i), container);
                }
            }

        }

    }
}