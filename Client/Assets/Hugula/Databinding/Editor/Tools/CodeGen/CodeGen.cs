using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;

namespace HugulaEditor.Databinding
{

    public class CodeGenTemplateUtils
    {

        /// <summary>
        /// 根据属性type类型生成对应模板代码
        /// </summary>
        /// <param name="propNode"></param>
        /// <returns></returns>
        internal virtual string TemplateByPropertyType(PropertyNode propNode, string tabstr = "")
        {
            System.Type propertyType = propNode.propertyType;
            if (propNode.isMethod)
            {
                return $@"function(arg)
   if arg then  {propNode.upvalue}._{propNode.methName} = arg end
   return  {propNode.upvalue}._{propNode.methName} --get
end";
            }
            else if (typeof(int) == propertyType || typeof(float) == propertyType)
            {
                return $" 0";
            }
            else if (typeof(ICommand) == propertyType)
            {
                return $@"{{
   CanExecute = function(self, arg)
       return true
   end,
   Execute = function(self, arg)

   end
}}";
            }
            else if (typeof(IExecute) == propertyType)
            {
                return $@"{{
    Execute = function(self, arg)

    end
}}";
            }
            else if (propNode.targetPropertyName.Equals("onClickCommand")) //点击事件
            {
                return $@"{{
    CanExecute = function(self, arg)
        return true
    end,
    Execute = function(self, arg)

    end
}}";
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
            else if (propertyType.IsSubclassOf(typeof(System.Delegate))) //是委托
            {
                if (propNode.targetPropertyName.Equals("onContextChanged") && propNode.target is BindableContainer) //自定义设置
                {
                    return $@"function(bc,item){GenGenericonContextChangedFunction((BindableContainer)propNode.target)}
end";
                }
                else
                {
                    return $@"function({GenGenericTypeArgumentsForLua(propertyType)})
--
end ";
                }
            }

            if (propertyType.IsClass)
                return propertyType.ToString().Replace("+", ".") + "()";
            else
            {
                return propertyType.ToString().Replace("+", ".");

            }


        }

        //展开泛型参数为lua function参数
        internal virtual string GenGenericTypeArgumentsForLua(System.Type type)
        {
            var sb = new StringBuilder();
            int i = 0;
            var sp = "";
            var len = 1;
            foreach (var t in type.GenericTypeArguments)
            {
                if (t.IsValueType)
                {
                    len = t.Name.Length <= 3 ? t.Name.Length : 3;
                    sb.Append($"{sp}{t.Name.Substring(0, len).ToLower()}_{i}");
                }
                else
                    sb.Append($"{sp}obj{i}");

                i++;
                sp = ",";
            }
            return sb.ToString();
        }

        //获取生成代码的主要属性
        internal virtual string GetComponentMainProperty(object comp)
        {
            var tp = comp.GetType();
            if (tp.IsSubclassOf(typeof(UnityEngine.UI.Text)) || tp == (typeof(UnityEngine.UI.Text)))
            {
                return "text";
            }
            else if (tp == typeof(UnityEngine.UI.Image))
            {
                return "sprite";
            }
            else if (tp == typeof(Hugula.Databinding.Binder.ImageBinder))
            {
                return "spriteName";
            }
            else if (tp == typeof(Hugula.Databinding.Binder.TextBinder) || tp == typeof(Hugula.Databinding.Binder.TextMeshProUGUIBinder))
            {
                return "text";
            }

            return "name";
        }

        //生成BindableContainer  onContextChanged 默认绑定内容
        internal virtual string GenGenericonContextChangedFunction(BindableContainer bc)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            Object mono;
            sbTitle.AppendLine("\r\nif item == nil then return end");
            sbTitle.AppendLine("\r\n-- declare ");
            sb.AppendLine("\r\n-- fill");

            for (int i = 0; i < bc.monos.Count; i++)
            {
                mono = bc.monos[i];
                var safeVar = BindableUtility.GetSafeName(bc.names[i]);
                sbTitle.AppendLine($@"        local _{safeVar} = bc[""{safeVar}""] --{mono.GetType().Name}");
                sb.AppendLine($@"        _{safeVar}.{GetComponentMainProperty(mono)} = item.{safeVar}");
            }

            sbTitle.AppendLine(sb.ToString());
            return sbTitle.ToString();
        }

        //生成PropertyNode的代码
        internal virtual string ToTemplateString(PropertyNode prop, string upvalue = null)
        {
            prop.upvalue = upvalue;
            if (prop.isMethod)
            {
                var lbIndex = prop.propertyName.IndexOf('(');
                prop.methName = prop.propertyName.Substring(0, lbIndex);
                return string.Format("{0} = {1}", prop.methName, TemplateByPropertyType(prop));
            }
            else
                return string.Format("{0} = {1}", prop.propertyName, TemplateByPropertyType(prop));
        }

    }


    public enum TemplateType
    {
        name,
        property,
        message,
        method,
        command,
        author,
        date
    }

    public enum ContextType
    {
        ViewModel,
        NotifyObject,
        NotifyTable,
        Object,
    }
    public class PropertyNode
    {
        //ObjectReference
        public object target;
        public string propertyName;
        public string targetPropertyName;
        public System.Type propertyType;
        public bool isListProperty;
        public bool isMethod;
        internal string methName;
        internal string upvalue = null;

        public override string ToString()
        {
            return $"{target}.{propertyName} {propertyType.Name} isListProperty={isListProperty} ,isMethod={isMethod} methName={methName}";
        }

        // public string ToTemplateString(string upvalue = null)
        // {

        // }
    }

    public class ContextNode
    {

        #region static
        internal static bool IsProperty(System.Type propertyType)
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

        internal static string NewContextByType(ContextType cType)
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

        internal static string rootName = "";
        #endregion


        #region 
        // public static System.Func<PropertyNode, string,string> ToTemplateString;
        public static CodeGenTemplateUtils codeGenTemplateUtils;
        #endregion
        //名字
        public string name;

        private string m_LocalName;
        //
        public string localName
        {
            get
            {
                if (string.IsNullOrEmpty(m_LocalName))
                {
                    m_LocalName = name;
                    var currParent = parent;
                    while (currParent != null)
                    {
                        m_LocalName = currParent.name.Substring(0,3) + "_" + m_LocalName;
                        currParent = currParent.parent;
                    }
                }
                return m_LocalName;
            }
        }

        private string m_FullName;
        public string fullContextName{
            get{
                if(string.IsNullOrEmpty(m_FullName))
                {
                    m_FullName = name;
                    var currParent = parent;
                    while(currParent!=null)
                    {
                        m_FullName = currParent.name+"."+m_FullName;
                        currParent = currParent.parent;
                    }
                }             

                return m_FullName;
            }
        }

        public static string templateNotifyTableAddUPRemove;
        public ContextType contextType;
        public ContextNode parent;
        public List<PropertyNode> peroperties = new List<PropertyNode>();
        public List<ContextNode> children = new List<ContextNode>();

        /// <summary>
        /// 父节点是NotifyTable
        /// </summary>
        /// <returns></returns>
        public bool ParentIsNotifyTable()
        {
            if (parent != null && parent.contextType == ContextType.NotifyTable)
                return true;
            return false;
        }
        public PropertyNode AddProperty(object target, string targetPropertyName, string propertyName, System.Type propertyType, bool isListProperty)
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
                    target = target,
                    targetPropertyName = targetPropertyName,
                    isMethod = propertyName.IndexOf('(') != -1,
                    propertyName = propertyName,
                    propertyType = propertyType ?? typeof(object),
                    isListProperty = isListProperty,
                };

                peroperties.Add(prop);
                peroperties.Sort((a, b) =>
                {
                    // return b.propertyType.GetHashCode() - a.propertyType.GetHashCode();
                    return string.CompareOrdinal(a.propertyName, b.propertyName);
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
            // binding.ParsePath();
            var m_Parts = binding.GetField("partConfigs") as  BindingPathPartConfig[];
            ContextNode m_Current = this;
            ContextNode m_Last = this;

            // BindingPathPart part = null;
            for (var i = 0; i < m_Parts.Length; i++)
            {
                var part = m_Parts[i];
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

            var tempStr = codeGenTemplateUtils.ToTemplateString(prop, name);
            if (prop.isListProperty)
            {
                if (tempStr.Contains("\n") || tempStr.Contains("\r\n"))
                {
                    if (prop.isMethod)
                        AppendLineMethod(string.Format(GetTabByDepth() + "--[[{0}.{1}]]--", name + "_item", tempStr));
                    else
                        AppendLine(string.Format(GetTabByDepth() + "--[[{0}.{1}]]--", name + "_item", tempStr), prop.propertyType);
                }
                else
                {
                    if (prop.isMethod)
                        AppendLineMethod(string.Format(GetTabByDepth() + "--{0}.{1} ", name + "_item", tempStr));
                    else
                        AppendLine(string.Format(GetTabByDepth() + "--{0}.{1} ", name + "_item", tempStr), prop.propertyType);
                }
            }
            else if (prop.isMethod)
            {
                AppendLineMethod(string.Format("{0}.{1}", localName, tempStr));
            }
            else
            {
                AppendLine(string.Format(GetTabByDepth() + "{0}.{1}", localName, tempStr), prop.propertyType);
            }
        }

        string GenNotifyTableAddUpRemove(string name, string dataFullPath,string propertyName)
        {
            var templateOut = templateNotifyTableAddUPRemove;
            templateOut = templateOut.Replace("{name}", name);
            templateOut = templateOut.Replace("{data_path}", dataFullPath);
            templateOut = templateOut.Replace("{property}", propertyName);
            return templateOut;
        }

        private System.Text.StringBuilder sb = new System.Text.StringBuilder();
        public void GenTemplateString()
        {
            AppendLine(GetTabByDepth() + string.Format("----  {0} begin  ----", name), typeof(object));

            foreach (var child in children)
            {
                AppendLine("", typeof(object));
                AppendLine(string.Format(GetTabByDepth() + "local {0} = {1}", child.localName, NewContextByType(child.contextType)), typeof(object));
                AppendLine(string.Format(GetTabByDepth() + "{0} = {1}", child.fullContextName, child.localName), typeof(object));
                if (child.contextType == ContextType.NotifyTable) // notify table add remove update
                {
                    AppendLineMethod(GenNotifyTableAddUpRemove(ContextNode.rootName, child.fullContextName, child.name));
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
                    AppendLineMethod(GenNotifyTableAddUpRemove(ContextNode.rootName, fullContextName,prop.propertyName));
                }
            }
            AppendLine(GetTabByDepth() + string.Format("----  {0} end  --", name), typeof(object));

        }
    }

}

