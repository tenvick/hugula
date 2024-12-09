using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;

namespace HugulaEditor.Databinding
{
    public class CodeGenWindow : EditorWindow
    {
        static readonly string SaveLuaPath = "Assets/Lua/viewmodels/";

        [MenuItem("Hugula/Data Binding/CodeGen Window")]
        internal static void Init()
        {
            var window = EditorWindow.GetWindow<CodeGenWindow>("CodeGen");
            window.Close();
            window = EditorWindow.GetWindow<CodeGenWindow>("CodeGen");
            window.Show();
        }

        void OnGUI()
        {
            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawGraph();
            }

        }

        void OnEnable()
        {
            bindableContainerCount = 0;
            binderCount = 0;
            bindableObjectCount = 0;
            templateOut = string.Empty;
            ReadTemplate();
        }

        void OnDisable()
        {
        }

        #region  template
        public string template = string.Empty;
        public static string templateNotifyTableAddUPRemove = string.Empty;
        virtual internal void ReadTemplate()
        {
            template = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_viewmodel.lua");
            templateNotifyTableAddUPRemove = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_notifytable_add_up_remove.lua");
        }

        #endregion

        internal GraphBackground background = new GraphBackground();
        internal Vector2 scrollPos;
        internal Rect graphRegion;
        internal int bindableContainerCount;
        internal int bindableObjectCount;

        internal int binderCount;

        internal virtual void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var toolbarHeight = GUILayout.Height(GraphEditorSettings.ToolbarHeight);
                if (GUILayout.Button("Generate ViewModel", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    ContextNode.codeGenTemplateUtils = new CodeGenTemplateUtils();
                    CreatePreViewCode();
                }

                EditorGUILayout.LabelField(string.Format("Binder: {0}", binderCount), GUILayout.Width(100), toolbarHeight);
                EditorGUILayout.LabelField(string.Format("BindableContainer: {0}", bindableContainerCount), GUILayout.Width(150), toolbarHeight);
                EditorGUILayout.LabelField(string.Format("BindableObject: {0}", bindableObjectCount), GUILayout.Width(150), toolbarHeight);

                if (!string.IsNullOrEmpty(templateOut) && GUILayout.Button("Save File", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    SaveFile();
                }
            }

        }


        public void CreatePreViewCode()
        {
            var selectedTransform = Selection.activeTransform;

            if (selectedTransform == null)
            {
                Debug.LogError("No transform is selected");
                return;
            }
            else
            {
                var transformPath = GraphEditorWindow.GetGameObjectPath(selectedTransform);
                Debug.Log(string.Format("Create binding graph for {0}", transformPath), selectedTransform);
            }
            ReadTemplate();
            templateOut = string.Empty;
            var container = selectedTransform.GetComponent<Hugula.Databinding.BindableObject>();
            var name = BindableUtility.GetSafeName(container.name);
            ContextNode.templateDic.Clear();
            ContextNode.templateDic["name"] = new StringBuilder().Append(name);
            ContextNode.templateDic["property"] = new StringBuilder();
            ContextNode.templateDic["command"] = new StringBuilder();
            ContextNode.templateDic["author"] = new StringBuilder().Append(System.Environment.UserName);
            ContextNode.templateDic["date"] = new StringBuilder().Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            ContextNode.templateNotifyTableAddUPRemove = templateNotifyTableAddUPRemove;


            var root = new ContextNode() { name = name, contextType = ContextType.ViewModel };
            ContextNode.rootName = name;
            Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext = new Dictionary<BindableObject, ContextNode>();
            BuildContextTree(container, root, sourceContext);
            Debug.Log(root.name);
            root.GenTemplateString();


            var tempOut = template;
            string key = string.Empty;
            string content = string.Empty;
            var templateTypes = System.Enum.GetValues(typeof(TemplateType));
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (var en in templateTypes)
            {
                key = System.Enum.GetName(typeof(TemplateType), en);//获取名称
                content = string.Empty;
                if (ContextNode.templateDic.TryGetValue(key, out sb))
                {
                    content = sb.ToString();
                }

                key = "{" + key + "}";
                tempOut = tempOut.Replace(key, content);
            }

            Debug.Log(tempOut);
            templateOut = tempOut;
        }

        internal string templateOut = "";
        public void DrawGraph()
        {
            background.Draw(graphRegion, scrollPos);

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scrollScope.scrollPosition;
                BeginWindows();
                GUILayout.TextArea(templateOut);
                EndWindows();
            }

            if (Event.current.type == EventType.Repaint)
            {
                var newRect = GUILayoutUtility.GetLastRect();
                if (newRect != graphRegion)
                {
                    graphRegion = newRect;
                    Repaint();
                }
            }
        }

        public void SaveFile()
        {
            string fileName = ContextNode.templateDic["name"].ToString();
            var file = EditorUtility.SaveFilePanelInProject(fileName, fileName, "lua", "", SaveLuaPath);
            if (file.Length > 0)
                System.IO.File.WriteAllText(file, templateOut);
        }


        internal void AddPropertyToContext(object target, Binding binder, bool isSelf, ContextNode context, Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext)
        {
            var tp = target.GetType();

            if (tp == typeof(Hugula.Databinding.Binder.CustomBinder))
            {
                var prop_target = tp.GetProperty("target", BindingFlags.Public | BindingFlags.Instance);
                if (prop_target != null)
                {
                    var real_target = prop_target.GetValue(target);
                    if (real_target != null)
                        tp = real_target.GetType();
                }
            }
            var prop = tp.GetProperty(binder.propertyName);

            if (prop == null)
            {
                Debug.LogError($"AddPropertyToContext Error:  target:{target}  Type:{tp}  Property:{binder.propertyName} not found");
                return;
            }

            bool isListProp = false;
            if (!isSelf && (context.contextType == ContextType.NotifyTable || (context.parent?.contextType == ContextType.NotifyTable))) isListProp = true;
            if (binder.source is BindableObject bindableObject) //从source寻找上下文
            {
                var contextBinding = bindableObject.GetContextBinding();
                if (contextBinding != null && contextBinding == binder) //如果有context绑定则使用context
                {
                    var path = binder.path.Replace("context.", "");
                    if (sourceContext.TryGetValue((BindableObject)binder.source, out var sourceContextItem))
                    {
                        if (sourceContextItem.contextType == ContextType.NotifyTable) isListProp = false;
                        sourceContextItem?.AddProperty(target, prop.Name, path, prop?.PropertyType, isListProp);
                    }
                    else
                    {
                        context.AddProperty(target, prop.Name, path, prop?.PropertyType, isListProp);
                    }

                    return;
                }
            }

            context.AddProperty(target, prop.Name, binder.path, prop?.PropertyType, isListProp);
        }

        internal virtual void BuildContextTree(BindableObject root, ContextNode context, Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext)
        {
            // Debug.LogFormat("BuildContextTree({0}) ", root);
            var bindings = root.GetBindings();
            if (root is BindableContainer) 
                bindableContainerCount += 1;
            else
                bindableObjectCount += 1;

            ContextNode currContext = context;
            var contextBinding = root.GetContextBinding(); //root.GetBinding("context");
            if (contextBinding != null)
            {
                var ctype = ContextType.NotifyObject;
                if (root is BindableContainer)
                {
                    ctype = ContextType.NotifyObject;
                }
                else if (root is ICollectionBinder)
                {
                    ctype = ContextType.NotifyTable;
                }
                currContext = context.FindContext(contextBinding, ctype);//寻找上下文

            }
            else
            {
                // Debug.LogFormat("{0}.context = nil ", root);
            }

            sourceContext[root] = currContext;

            object target = root;
            var binders = root.GetBindings();

            if (binders != null)
            {
                binderCount += bindings.Count;

                foreach (var binder in binders)
                {
                    if (binder != contextBinding && binder.path.Trim() != ".")
                    {
                        if (binder.target != null)
                            target = binder.target;
                        else
                            target = root;

                        AddPropertyToContext(target, binder, true, currContext, sourceContext);
                    }
                }
            }

            //children
            if (root is ICollectionBinder container)
            {
                var children = container.GetChildren();
                if (children == null) return;

                foreach (var item in children)
                {
                    if (item is ICollectionBinder) //如果是容器
                    {
                        BuildContextTree(item, currContext, sourceContext);
                    }
                    else if (item != null)
                    {
                        bindableObjectCount += 1;
                        binders = item.GetBindings();
                        target = item;
                        if (binders != null)
                        {
                            binderCount += binders.Count;
                            foreach (var binder in binders)
                            {
                                if (binder.propertyName != "context" && binder.path != ".")
                                {
                                    if (binder.target != null)
                                        target = binder.target;
                                    else
                                        target = item;

                                    AddPropertyToContext(target, binder, false, currContext, sourceContext);
                                }
                            }
                        }
                    }
                }
            }


        }

    }
}
