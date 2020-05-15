using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace HugulaEditor.Databinding
{
    [CustomEditor(typeof(BindableContainer), true)]
    public class BindableContainerEditor : UnityEditor.Editor
    {

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
#endif
        };

        static List<string> names; //= new List<string>();
        static GameObject[] refers; //=new List<GameObject>();
        static UnityEngine.Object[] bindableObjects; // = new List<Behaviour>();
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI ();
            EditorGUILayout.Separator();
            var temp = target as BindableContainer;

            EditorGUILayout.LabelField("Drag(BindableObject) to add", GUILayout.Width(200));
            EditorGUILayout.Space();
            UnityEngine.Component addComponent = null;
            addComponent = (UnityEngine.Component)EditorGUILayout.ObjectField(addComponent, typeof(UnityEngine.Component), true, GUILayout.Height(40));

            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            if (GUILayout.Button("auto add hierarchy  children", GUILayout.MaxWidth(300)))
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

            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bindable List", GUILayout.Width(200));
            EditorGUILayout.Space();

            Undo.RecordObject(target, "F");
            BindableObject objComponent;
            if (temp.children != null)
            {
                for (int i = 0; i < temp.children.Count; i++)
                {
                    objComponent = temp.children[i];
                    BindalbeObjectUtilty.BindableObjectField(objComponent, i);
                    if (GUILayout.Button("Del", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }

                    //设置binding属性
                    SetBindingProperties(temp, i);
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.Space();
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

        public void RemoveAtbindableObjects(BindableContainer refer, int index)
        {
            var children = refer.children;
            children.RemoveAt(index);
        }

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
            foreach (var child in children)
            {
                if (oldChildren.IndexOf(child) == -1 && !System.Object.Equals(child, container))
                {
                    container.AddChild(child);
                }

                if (child is BindableContainer && !checkChildren) //如果遇到容器不需要遍历
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