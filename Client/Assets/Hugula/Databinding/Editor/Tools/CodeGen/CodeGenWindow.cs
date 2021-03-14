using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;

namespace HugulaEditor.Databinding.Editor
{
    public class CodeGenWindow : EditorWindow
    {
        static readonly string SaveLuaPath = "Assets/Lua/viewmodels/";

        [MenuItem("Hugula/Data Binding/CodeGen Window")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<CodeGenWindow>("CodeGen");
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
            templateOut = string.Empty;
            ReadTemplate();
        }

        void OnDisable()
        {
        }

        #region  template
        public string template = string.Empty;
        public static string templateNotifyTableAddUPRemove = string.Empty;
        void ReadTemplate()
        {
            template = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_viewmodel.lua");
            templateNotifyTableAddUPRemove = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_notifytable_add_up_remove.lua");
        }

        #endregion


        #region viewmodel tree

        static string TemplateByPropertyType(PropertyNode propNode)
        {
            System.Type propertyType = propNode.propertyType;
            if (propNode.isMethod)
            {
                return string.Format(@"function(arg)
                if arg then
                    --set 
                    {1}._{0} = arg
                else 
                    --get {1}._{0}
                end
        return  {1}._{0}
    end
    ", propNode.methName, propNode.upvalue);
            }
            else if (typeof(int) == propertyType || typeof(float) == propertyType)
            {
                return "0";
            }
            else if (typeof(ICommand) == propertyType)
            {
                return @" {
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)

    end
}
";
            }
            else if (typeof(string) == propertyType)
            {
                return @"""""";
            }
            else if (typeof(object) == propertyType)
            {
                return "{}";
            }
            else if (typeof(bool) == propertyType)
            {
                return "false";
            }

            return propertyType.ToString();

        }

        static string NewContextByType(ContextType cType)
        {
            switch (cType)
            {
                case ContextType.ViewModel:
                    return string.Empty;
                case ContextType.NotifyObject:
                    return "NotifyObject()";
                case ContextType.NotifyTable:
                    return "NotifyTable()";
                case ContextType.Object:
                    return "{}";
                default:
                    return @"""""";
            }
        }
        static bool IsProperty(System.Type propertyType)
        {
            if (propertyType.IsInterface || propertyType.IsGenericType)
                return false;
            else
            {
                return true;
            }
        }

        enum TemplateType
        {
            name,
            property,
            message,
            method,
            command
        }

        enum ContextType
        {
            ViewModel,
            NotifyObject,
            NotifyTable,
            Object,
        }
        class PropertyNode
        {
            public string propertyName;
            public System.Type propertyType;
            public bool isListProperty;
            public bool isMethod;
            internal string methName;
            internal string upvalue = null;

            public string ToTemplateString(string upvalue = null)
            {
                this.upvalue = upvalue;
                if (isMethod)
                {
                    var lbIndex = propertyName.IndexOf('(');
                    methName = propertyName.Substring(0, lbIndex);
                    return string.Format("{0}={1}", methName, TemplateByPropertyType(this));
                }
                else
                    return string.Format("{0}={1}", propertyName, TemplateByPropertyType(this));
            }
        }

        class ContextNode
        {
            public string name;
            public ContextType contextType;
            public ContextNode parent;
            public List<PropertyNode> peroperties = new List<PropertyNode>();
            public List<ContextNode> children = new List<ContextNode>();
            public PropertyNode AddProperty(string propertyName, System.Type propertyType, bool isListProperty)
            {
                var prop = peroperties.Find((PropertyNode t) =>
                {
                    if (propertyName == t.propertyName)
                        return true;
                    else
                    {
                        return false;
                    }
                });

                if (prop == null)
                {
                    prop = new PropertyNode()
                    {
                        isMethod = propertyName.IndexOf('(') != -1,
                        propertyName = propertyName,
                        propertyType = propertyType,
                        isListProperty = isListProperty,
                    };
                    peroperties.Add(prop);
                    peroperties.Sort((a, b) =>
                    {
                        return b.propertyType.GetHashCode() - a.propertyType.GetHashCode();
                    });
                }
                return prop;
            }

            public void AddChildContext(ContextNode child)
            {
                if (children.IndexOf(child) < 0)
                {
                    children.Add(child);
                }
            }

            public ContextNode FindContext(Binding binding, ContextType cType)
            {
                binding.ParsePath();
                var m_Parts = binding.parts;
                ContextNode m_Current = this;
                ContextNode m_Last = this;

                BindingPathPart part = null;
                for (var i = 0; i < m_Parts.Count; i++)
                {
                    part = m_Parts[i];
                    if (!part.isSelf && m_Current != null)
                    {
                        {
                            m_Last = m_Current;
                            m_Current = (ContextNode)m_Current[part.path];
                            if (m_Current == null)
                            {
                                var context = new ContextNode() { name = part.path, contextType = cType };
                                m_Current = context;
                                m_Last[part.path] = context;
                            }
                        }
                    }

                    if (!part.isSelf && m_Current == null)
                        break;
                }

                return m_Current;
            }

            public object this[string property]
            {
                get
                {
                    foreach (var c in children)
                    {
                        if (c.name == property)
                            return c;
                    }

                    return null;
                }
                set
                {
                    if (value is ContextNode)
                    {
                        AddChildContext((ContextNode)value);
                    }
                    else if (value is PropertyNode)
                        peroperties.Add((PropertyNode)value);
                }
            }

            public static Dictionary<string, StringBuilder> templateDic = new Dictionary<string, StringBuilder>();
            StringBuilder GetSBTemplate(bool isProperty)
            {
                StringBuilder sb = null;
                string propName = "property";
                if (!isProperty)
                {
                    propName = "command";
                }

                if (!templateDic.TryGetValue(propName, out sb))
                {
                    sb = new StringBuilder();
                    templateDic.Add(propName, sb);
                }
                return sb;
            }

            StringBuilder GetSBTemplate(TemplateType templateType)
            {
                StringBuilder sb = null;
                string propName = templateType.ToString();
                if (!templateDic.TryGetValue(propName, out sb))
                {
                    sb = new StringBuilder();
                    templateDic.Add(propName, sb);
                }
                return sb;
            }

            void AppendLine(string line, System.Type propertyType)
            {
                var sb = GetSBTemplate(IsProperty(propertyType));
                sb.AppendLine(line);
            }

            void AppendLineMethod(string line)
            {
                var sb = GetSBTemplate(TemplateType.method);
                sb.AppendLine();
                sb.AppendLine(line);
            }

            void AppendLineProperty(PropertyNode prop)
            {

                if (prop.isListProperty)
                {
                    AppendLine(string.Format("-- {0}.{1}", name + "_item", prop.ToTemplateString(name)), prop.propertyType);
                }
                else if (prop.isListProperty && prop.isMethod)
                {
                    AppendLineMethod(string.Format("--[[ {0}.{1} ]]--", name + "_item", prop.ToTemplateString(name)));
                }
                else if (prop.isMethod)
                {
                    AppendLineMethod(string.Format("{0}.{1}", name, prop.ToTemplateString(name)));
                }
                else
                {
                    AppendLine(string.Format("{0}.{1}", name, prop.ToTemplateString(name)), prop.propertyType);
                }
            }

            string GenNotifyTableAddUpRemove(string name, string childName)
            {
                var templateOut = templateNotifyTableAddUPRemove;
                templateOut = templateOut.Replace("{name}", name);
                templateOut = templateOut.Replace("{0}", childName);
                return templateOut;
            }

            private System.Text.StringBuilder sb = new System.Text.StringBuilder();
            public void GenTemplateString()
            {

                foreach (var child in children)
                {
                    AppendLine(string.Format("local {0}={1}", child.name, NewContextByType(child.contextType)), typeof(object));
                    AppendLine(string.Format("{0}.{1}={1}", name, child.name), typeof(object));
                    if (child.contextType == ContextType.NotifyTable) // notify table add remove update
                    {
                        AppendLineMethod(GenNotifyTableAddUpRemove(name, child.name));
                    }
                }

                foreach (var prop in peroperties)
                {
                    AppendLineProperty(prop);
                }

                foreach (var child in children)
                {
                    child.GenTemplateString();
                }

            }
        }


        #endregion

        private GraphBackground background = new GraphBackground();
        private Vector2 scrollPos;
        private Rect graphRegion;
        private int bindableContainerCount;
        private int binderCount;

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var toolbarHeight = GUILayout.Height(GraphEditorSettings.ToolbarHeight);
                if (GUILayout.Button("Generate ViewModel", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    CreatePreViewCode();
                }

                EditorGUILayout.LabelField(string.Format("Binder: {0}", binderCount), GUILayout.Width(100), toolbarHeight);

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
            ContextNode.templateDic.Clear();
            ContextNode.templateDic["name"] = new StringBuilder().Append(container.name);
            ContextNode.templateDic["property"] = new StringBuilder();
            ContextNode.templateDic["command"] = new StringBuilder();


            var root = new ContextNode() { name = container.name, contextType = ContextType.ViewModel };
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

        string templateOut = "";
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


        private void AddPropertyToContext(object target, Binding binder, bool isSelf, ContextNode context, Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext)
        {
            var tp = target.GetType();
            var prop = tp.GetProperty(binder.propertyName);
            bool isListProp = false;
            if (!isSelf && context.contextType == ContextType.NotifyTable) isListProp = true;

            if (binder.source is BindableObject) //从source寻找上下文
            {
                binder.ParsePath();
                if (binder.parts.Count > 1 && binder.parts[1].path == "context")
                {
                    context = sourceContext[(BindableObject)binder.source];
                    var path = binder.path.Replace("context.", "");
                    if (context.contextType == ContextType.NotifyTable) isListProp = false;
                    context?.AddProperty(path, prop?.PropertyType, isListProp);
                    return;
                }
            }

            context.AddProperty(binder.path, prop?.PropertyType, isListProp);
        }

        private void BuildContextTree(BindableObject root, ContextNode context, Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext)
        {
            // Debug.LogFormat("BuildContextTree({0}) ", root);
            binderCount += root.GetBindings().Count;
            ContextNode currContext = context;
            var contextBinding = root.GetBindingByName("context");
            if (contextBinding != null)
            {
                var ctype = ContextType.NotifyObject;
                if (root is BindableContainer) ctype = ContextType.NotifyObject;
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

            foreach (var binder in binders)
            {
                if (binder != contextBinding && binder.path.Trim() != ".")
                {
                    AddPropertyToContext(target, binder, true, currContext, sourceContext);
                }
            }

            //children
            if (root is ICollectionBinder)
            {
                var container = root as Hugula.Databinding.ICollectionBinder;
                var children = container.GetChildren();
                foreach (var item in children)
                {
                    // NodeGUI node = null;
                    if (item is ICollectionBinder) //如果是容器
                    {
                        BuildContextTree(item, currContext, sourceContext);
                    }
                    else if (item != null)
                    {
                        binderCount += item.GetBindings().Count;
                        binders = item.GetBindings();
                        target = item;
                        foreach (var binder in binders)
                        {
                            if (binder.propertyName != "context" && binder.path != ".")
                            {
                                AddPropertyToContext(target, binder, false, currContext, sourceContext);
                            }
                        }
                    }
                }
            }


        }

    }
}
