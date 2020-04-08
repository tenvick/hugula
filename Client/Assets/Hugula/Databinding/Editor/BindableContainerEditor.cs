using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEngine;

namespace Hugula.Databinding.Editor
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

            EditorGUILayout.LabelField("Drag(BindableObject) to add", GUILayout.Width(200));
            EditorGUILayout.Space();
            UnityEngine.Component addComponent = null;
            addComponent = (UnityEngine.Component)EditorGUILayout.ObjectField(addComponent, typeof(UnityEngine.Component), true, GUILayout.Height(40));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Bindable List", GUILayout.Width(200));
            EditorGUILayout.Space();

            var temp = target as BindableContainer;
            Undo.RecordObject(target, "F");
            BindableObject objComponent;
            if (temp.children != null)
            {
                for (int i = 0; i < temp.children.Count; i++)
                {
                    // EditorGUILayout.BeginHorizontal ();
                    // GUILayout.Label ((i + 1).ToString (), GUILayout.Width (20));
                    objComponent = temp.children[i];
                    // EditorGUILayout.ObjectField (objComponent, typeof (UnityEngine.Object));
                    // objComponent = PopupGameObjectComponents (GetbindableObjects (temp, i).target, i); //选择绑定的component type类型
                    // if (objComponent != null) AddbindableObjects (temp, i, objComponent); //绑定选中的类型
                    // //显示选中的对象
                    // AddbindableObjects (temp, i, EditorGUILayout.ObjectField (GetbindableObjects (temp, i).target, typeof (UnityEngine.Object), true, GUILayout.MaxWidth (80)));
                    // //选择可绑定属性
                    // PopupComponentsProperty (temp, i);
                    BindalbeObjectUtilty.BindableObjectField(objComponent, i);
                    if (GUILayout.Button("Del", GUILayout.Width(30)))
                    {
                        RemoveAtbindableObjects(temp, i);
                    }

                    // EditorGUILayout.EndHorizontal ();
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
            // BindingPropertiesStyle.
            var binding = GetbindableObjects(refer, i);
            if (binding != null)
            {
                // BindableExpression.Expression (binding, i);
                // EditorGUILayout.LabelField (new GUIContent ("expression:"), BindingPropertiesStyle,GUILayout.Width(55));
                // EditorGUILayout.EndHorizontal ();
            }
        }

        public void AddbindableObjects(BindableContainer refer, UnityEngine.Component obj)
        {

            if (refer.children == null) //需要新增加
            {
                List<Binding> bindings = new List<Binding>();
                // if (refer.bindableObjects != null) bindings.AddRange (refer.bindableObjects);
                // if (i == -1) i = bindings.Count;
                // while (bindings.Count <= i)
                //     bindings.Add (new Binding () { expression = @"{}" });

                refer.children = new List<BindableObject>(); // bindings.ToArray ();
                // Debug.Log (bindings.Count);
            }

            var children = refer.children;
            BindableObject bindable = null;

            if (obj is BindableObject)
            {
                bindable = obj as BindableObject;
                // bindable.target = obj;
            }
            else
            {

                // BindableObject bindable = obj.<BindableObject> ();

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
                        Type createBinderType = typeof(Hugula.Databinding.Binder.UIBehaviourBinder);
                        if (tpId >= 0)
                        {
                            createBinderType = BinderCreateTypes[tpId];
                        }

                        bindable = (BindableObject)obj.gameObject.AddComponent(createBinderType);
                        bindable.target = target;
                    }
                }
            }

            // bindable.targetName = obj.name;
            // bindable.binderType = GetBinderType (bindable.target.GetType ());
            if (children.IndexOf(bindable) < 0)
            {
                refer.AddChild(bindable);
            }

            // Binding binditem = bindableObjects[i];
            // binditem.target = obj;
            // if (obj) {
            //     binditem.binderType = GetBinderType (obj.GetType ());
            //     binditem.targetName = obj.name.Trim ();
            // }
        }

        public void RemoveAtbindableObjects(BindableContainer refer, int index)
        {
            // List<Binding> bindableObjects = new List<Binding> (refer.bindableObjects);
            var children = refer.children;
            children.RemoveAt(index);
            // bindableObjects.RemoveAt (index);
            // refer.bindableObjects = bindableObjects.ToArray ();
        }

        public Binding GetbindableObjects(BindableContainer refer, int index)
        {
            // if (index >= 0 && index < refer.bindableObjects.Length)
            //     return refer.bindableObjects[index];
            return null;
        }

        private static string GetBinderType(Type type)
        {
            string tp = type.Name;

            return tp;
        }
        public static string GetLuaType(Type type)
        {
            string luaType = string.Empty;
            if (type == null)
                luaType = "nil";
            else if (type.Equals(typeof(string)))
                luaType = "string";
            else if (type.Equals(typeof(bool)))
                luaType = "boolean";
            else if (type.Equals(typeof(float)) || type.Equals(typeof(double)) || type.Equals(typeof(int)))
                luaType = "number";
            // else if(type.IsSubclassOf(typeof(UnityEngine.Events.UnityEvent)) || type.IsSubclassOf(typeof(UnityEngine.Events.UnityAction)))
            //     luaType = "table";
            else
                luaType = "userdata"; //function

            return luaType;
        }
    }
}