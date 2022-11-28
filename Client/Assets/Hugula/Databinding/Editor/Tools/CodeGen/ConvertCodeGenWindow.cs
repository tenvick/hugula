using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Hugula.Databinding;
using Hugula.Databinding.Binder;

namespace HugulaEditor.Databinding
{
    public class ConvertCodeGenWindow : CodeGenWindow
    {
        static readonly string SaveLuaPath = "Assets/Lua/viewmodels/";

        // [MenuItem("Hugula/Data Binding/Convert CodeGen Window")]
        internal static void Init2()
        {
            var get = EditorWindow.GetWindow<ConvertCodeGenWindow>("Convert CodeGen Single BindableContainer");
            get.Close();
            var window = EditorWindow.GetWindow<ConvertCodeGenWindow>("Convert CodeGen Single BindableContainer");
            window.Show();
        }

        internal override void ReadTemplate()
        {
            template = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_simple_BindableContainer.lua");
            templateNotifyTableAddUPRemove = System.IO.File.ReadAllText("Assets/Hugula/Databinding/Editor/Tools/CodeGen/template_notifytable_add_up_remove.lua");
        }

        internal override void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var toolbarHeight = GUILayout.Height(GraphEditorSettings.ToolbarHeight);
                if (GUILayout.Button("Generate BindableContainer Code", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    ContextNode.codeGenTemplateUtils = new ConvertCodeCodeGenTemplateUtils();
                    CreatePreViewCode();
                }

                if (GUILayout.Button("Convert To Mono", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    ConvertToMonos();
                }

                EditorGUILayout.LabelField(string.Format("Binder: {0}", binderCount), GUILayout.Width(100), toolbarHeight);
                EditorGUILayout.LabelField(string.Format("BindableContainer: {0}", bindableContainerCount), GUILayout.Width(150), toolbarHeight);

                if (!string.IsNullOrEmpty(templateOut) && GUILayout.Button("Save File", EditorStyles.toolbarButton, GUILayout.Width(200), toolbarHeight))
                {
                    SaveFile();
                }
            }
        }

        internal override void BuildContextTree(BindableObject root, ContextNode context, Dictionary<Hugula.Databinding.BindableObject, ContextNode> sourceContext)
        {
            binderCount = 0;
            // Debug.LogFormat("BuildContextTree({0}) ", root);
            binderCount += root.GetBindings().Count;
            if (root is BindableContainer) bindableContainerCount += 1;

            ContextNode currContext = context;
            var contextBinding = root.GetBinding("context");
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
                        // BuildContextTree(item, currContext, sourceContext);
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

        internal void ConvertToMonos()
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
                Debug.Log(string.Format("selected {0}", transformPath), selectedTransform);
            }

            var bc = selectedTransform.GetComponent<BindableContainer>();
            var sbCKTitle = new StringBuilder();

            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();

            List<BindableObject> moveChildren = new List<BindableObject>();
            List<string> names = new List<string>();
            Object mono;
            Dictionary<string, bool> chkDup = new Dictionary<string, bool>();
            bool globalConvert = false;
            //may 可以转换的解析
            for (int i = 0; i < bc.children.Count; i++)
            {
                var child = bc.children[i];
                if (ConvertCodeCodeGenTemplateUtils.CanConvert(child))
                {
                    moveChildren.Add(child);
                    sbCKTitle.AppendLine($@"{child.name}");

                    var safeVar = BindableUtility.GetSafeName(child.name);
                    if (chkDup.ContainsKey(safeVar))
                    {
                        safeVar += i.ToString();
                    }
                    else
                    {
                        chkDup.Add(safeVar, true);
                    }

                    names.Add(safeVar);

                    sbTitle.AppendLine($@"        local _{safeVar} = bc[""]{safeVar}""]");

                    var bindings = child.GetBindings();

                    foreach (var binding in bindings)
                    {
                        if (!string.IsNullOrEmpty(binding.converter))
                        {
                            if (!globalConvert)
                            {
                                sbTitle.AppendLine($@"        local __conv_reg = CS.Hugula.Databinding.ValueConverterRegister.instance");
                            }
                            sb.AppendLine($@"        _{safeVar}.{binding.propertyName} =  __conv_reg:Get(""{binding.converter}""):Convert(item.{binding.path})  ");
                        }
                        else
                            sb.AppendLine($@"        _{safeVar}.{binding.propertyName} = item.{binding.path}  --{child.GetType().Name}");
                    }
                }
            }

            if (UnityEditor.EditorUtility.DisplayDialog("warnning", $"Do you want to move thos elements to monos? {sbCKTitle.ToString()} ", "sure", "canel"))
            {
                // foreach(var c in moveChildren)
                BindableContainerEditor.CheckNamesAndMonos(bc);

                for (int i = 0; i < moveChildren.Count; i++)
                {
                    var c = moveChildren[i];
                    bc.children.Remove(c);
                    c.ClearBinding();
                    bc.monos.Add(c);
                    bc.names.Add(names[i]);
                }
                Debug.Log($"moved  \r\n{sbCKTitle.ToString()}");
                sbTitle.AppendLine(sb.ToString());

                Debug.Log(sbTitle.ToString());
                EditorUtility.SetDirty(bc);
            }

        }
    }


    public class ConvertCodeCodeGenTemplateUtils : CodeGenTemplateUtils
    {
        public static bool CanConvert(BindableObject child)
        {
            return child is BindableContainer == false && child is ButtonBinder == false && child is ICollectionBinder == false;
        }

        internal override string GenGenericonContextChangedFunction(BindableContainer bc)
        {
            var sb = new StringBuilder();
            var sbTitle = new StringBuilder();
            Object mono;
            sbTitle.AppendLine("\r\n-- convert declare \r\n ");
            sb.AppendLine("\r\n -- convert fill");
            Dictionary<string, bool> chkDup = new Dictionary<string, bool>();
            bool globalConvert = false;

            //may 可以转换的解析
            for (int i = 0; i < bc.children.Count; i++)
            {
                var child = bc.children[i];
                if (CanConvert(child))
                {
                    var safeVar = BindableUtility.GetSafeName(child.name);
                    if (chkDup.ContainsKey(safeVar))
                    {
                        safeVar += i.ToString();
                    }
                    else
                    {
                        chkDup.Add(safeVar, true);
                    }

                    sbTitle.AppendLine($@"        local _{safeVar} = bc[""{safeVar}""] --{child.GetType().Name}");

                    var bindings = child.GetBindings();

                    foreach (var binding in bindings)
                    {
                        if (!string.IsNullOrEmpty(binding.converter))
                        {
                            if (!globalConvert)
                            {
                                sbTitle.AppendLine($@"        local __conv_reg = CS.Hugula.Databinding.ValueConverterRegister.instance");
                            }
                            sb.AppendLine($@"        _{safeVar}.{binding.propertyName} =  __conv_reg:Get(""{binding.converter}""):Convert(item.{binding.path})  ");
                        }
                        else
                            sb.AppendLine($@"        _{safeVar}.{binding.propertyName} = item.{binding.path}");
                    }
                }
            }

            sbTitle.AppendLine(sb.ToString());
            sb.Clear();
            sbTitle.AppendLine("\r\n");
            sbTitle.AppendLine("\r\n-- declare \r\n ");
            sb.AppendLine("\r\n -- fill");
            //real
            for (int i = 0; i < bc.monos.Count; i++)
            {
                mono = bc.monos[i];
                var safeVar = bc.names[i];

                sbTitle.AppendLine($@"        local _{safeVar} = bc[""{safeVar}""] --{mono.GetType().Name}");
                sb.AppendLine($@"        _{safeVar}.{GetComponentMainProperty(mono)} = item.{bc.names[i]} ");
            }
            sbTitle.AppendLine(sb.ToString());
            return sbTitle.ToString();
        }
    }
}
