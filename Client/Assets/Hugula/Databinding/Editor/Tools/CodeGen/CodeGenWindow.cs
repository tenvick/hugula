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

        /// <summary>
        /// 根据属性type类型生成对应模板代码
        /// </summary>
        /// <param name="propNode"></param>
        /// <returns></returns>
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
            else if (typeof(INotifyTable) == propertyType)
            {
                return @"NotifyTable()";
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
            else if (typeof(UnityEngine.Events.UnityAction<int>) == propertyType ||
           typeof(System.Action<int>) == propertyType)
            {
                return @"function(i)
            --
end
";
            }
            else if (typeof(UnityEngine.Events.UnityAction<string>) == propertyType ||
          typeof(System.Action<string>) == propertyType)
            {
                return @"function(str)
            --
end
";
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
            if (typeof(INotifyTable) == propertyType ||
            typeof(IList) == propertyType
            )
                return true;
            else if (propertyType.IsInterface || propertyType.IsGenericType)
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
                    child.m_TabStr = null;
                    child.parent = this;
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

            internal int m_Depth = -1;
            public int depth
            {
                get
                {

                    // if (m_Depth == -1)
                    {

                        int i = 0;
                        var curr = this;
                        while (curr != null && curr.parent != null)
                        {
                            i++;
                            curr = curr.parent;
                        }
                        m_Depth = i;
                    }

                    return m_Depth;
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

            internal string m_TabStr = null;
            string GetTabByDepth()
            {
                if (m_TabStr == null)
                {
                    int d = depth;
                    m_TabStr = string.Empty;
                    while (d > 0)
                    {
                        m_TabStr += "    ";
                        d--;
                    }
                }

                return m_TabStr;
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
                    AppendLine(string.Format(GetTabByDepth() + "-- {0}.{1}", name + "_item", prop.ToTemplateString(name)), prop.propertyType);
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
                    AppendLine(string.Format(GetTabByDepth() + "{0}.{1}", name, prop.ToTemplateString(name)), prop.propertyType);
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
                AppendLine(GetTabByDepth() + string.Format("----  {0}  ----", name), typeof(object));

                foreach (var child in children)
                {
                    AppendLine("", typeof(object));
                    AppendLine(string.Format(GetTabByDepth() + "local {0}={1}", child.name, NewContextByType(child.contextType)), typeof(object));
                    AppendLine(string.Format(GetTabByDepth() + "{0}.{1}={1}", name, child.name), typeof(object));
                    if (child.contextType == ContextType.NotifyTable) // notify table add remove update
                    {
                        AppendLineMethod(GetTabByDepth() + GenNotifyTableAddUpRemove(name, child.name));
                    }
                    child.GenTemplateString();
                }

                // AppendLine(string.Format("\r\n---- {0} property   --", name), typeof(object));
                if (children.Count > 0)
                    AppendLine("\r\n", typeof(object));
                foreach (var prop in peroperties)
                {
                    AppendLineProperty(prop);
                    if (prop.propertyType == typeof(INotifyTable)) //notify
                    {
                        AppendLineMethod(GetTabByDepth() + GenNotifyTableAddUpRemove(name, prop.propertyName));
                    }
                }
                AppendLine(GetTabByDepth() + string.Format("----  {0} end  --", name), typeof(object));

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
                EditorGUILayout.LabelField(string.Format("BindableContainer: {0}", bindableContainerCount), GUILayout.Width(150), toolbarHeight);

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
            var name = GetSafeName(container.name);
            ContextNode.templateDic.Clear();
            ContextNode.templateDic["name"] = new StringBuilder().Append(name);
            ContextNode.templateDic["property"] = new StringBuilder();
            ContextNode.templateDic["command"] = new StringBuilder();


            var root = new ContextNode() { name = name, contextType = ContextType.ViewModel };
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
            if (root is BindableContainer) bindableContainerCount += 1;

            ContextNode currContext = context;
            var contextBinding = root.GetBindingByName("context");
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

        private string GetSafeName(string name)
        {
            int i = name.IndexOf("@");
            int j = name.IndexOf("(");
            if (i < j) i = j;
            if (i < 0) i = name.Length;
            return name.Substring(0, i);
        }
    }
}
