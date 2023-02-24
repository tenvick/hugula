using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Hugula.Databinding;
using System.Text.RegularExpressions;
using UnityEditor.IMGUI.Controls;

namespace HugulaEditor.Databinding
{


    // Add menu named "My Window" to the Window menu


    public class CompareWindow : EditorWindow
    {
        [MenuItem("Hugula/Data Binding/Compare Window")]
        static void OpenCompareWindow()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<CompareWindow>();
            window.titleContent = new GUIContent("Compare Window");
            window.Show();
        }

        [MenuItem("Assets/1. Compare Left Prefab", false, 4)]
        static void SelectLeft()
        {
            m_LeftChooseGO = Selection.activeGameObject;
        }

        static GameObject m_LeftChooseGO;

        [MenuItem("Assets/2. Compare Right Prefab", false, 5)]
        static void SelectRight()
        {
            var window = GetWindow<CompareWindow>();
            window.titleContent = new GUIContent("Compare Window");
            window.m_LeftGORoot = m_LeftChooseGO;
            window.m_RightGORoot = Selection.activeGameObject;
            window.Show();
        }

        [MenuItem("Assets/1. Compare Left Prefab", true, 4)]
        [MenuItem("Assets/2. Compare Right Prefab", true, 5)]
        static bool ValidatePrefabFile()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                if (System.IO.Path.GetExtension(AssetDatabase.GetAssetPath(obj)) == ".prefab")
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Assets/2. Compare Right Prefab", true, 5)]
        static bool ValidateRight()
        {
            return m_LeftChooseGO != null;
        }

        [SerializeField] TreeViewState m_LeftTreeViewState;
        [SerializeField] TreeViewState m_RightTreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        SimpleTreeView m_LeftTreeView;
        SimpleTreeView m_RightTreeView;

        SearchField m_LeftSearchField;
        SearchField m_RighttSearchField;

        internal GameObject m_LeftGORoot;
        internal GameObject m_RightGORoot;

        GameObject m_LeftSelectGo;
        GameObject m_RightSelectGo;

        SerializedObject m_LeftSelectSerial;
        SerializedObject m_RightSelectSerial;


        void OnEnable()
        {
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_LeftTreeViewState == null)
                m_LeftTreeViewState = new TreeViewState();

            if (m_RightTreeViewState == null)
                m_RightTreeViewState = new TreeViewState();

            m_LeftTreeView = new SimpleTreeView(m_LeftTreeViewState);
            m_RightTreeView = new SimpleTreeView(m_RightTreeViewState);

            m_LeftSearchField = new SearchField();
            m_RighttSearchField = new SearchField();

            m_LeftSearchField.downOrUpArrowKeyPressed += m_LeftTreeView.SetFocusAndEnsureSelectedItem;
            m_RighttSearchField.downOrUpArrowKeyPressed += m_RightTreeView.SetFocusAndEnsureSelectedItem;

            m_LeftTreeView.onViewItemSelected += onViewItemSelected;
            m_RightTreeView.onViewItemSelected += onViewItemSelected;
        }

        void OnGUI()
        {
            DoToolbar();
            DoTreeView(middleTreeViewRect);
            DrawInspector(middleTreeViewRectBottom);
            BottomToolBar(bottomToolbarRect);
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("target:");
            m_LeftTreeView.searchString = m_LeftSearchField.OnToolbarGUI(m_LeftTreeView.searchString, GUILayout.MaxWidth(200));
            GUILayout.FlexibleSpace();
            GUILayout.Label("source:");
            m_RightTreeView.searchString = m_RighttSearchField.OnToolbarGUI(m_RightTreeView.searchString);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.boldLabel);
            m_BarLabel.text = "target:";
            var newObj = (GameObject)EditorGUILayout.ObjectField(m_BarLabel, m_LeftGORoot, typeof(GameObject), true);

            if (newObj != m_LeftGORoot || (m_LeftGORoot && m_LeftTreeView.GetRows().Count == 0))
            {
                m_LeftGORoot = newObj; //change
                m_LeftTreeView.SetGameObject(m_LeftGORoot);
            }

            m_BarLabel.text = "source:";
            newObj = (GameObject)EditorGUILayout.ObjectField(m_BarLabel, m_RightGORoot, typeof(GameObject), true);
            if (newObj != m_RightGORoot || (m_RightGORoot && m_RightTreeView.GetRows().Count == 0))
            {
                m_RightGORoot = newObj; //change
                m_RightTreeView.SetGameObject(m_RightGORoot);
            }
            GUILayout.EndHorizontal();

        }

        GUIContent m_BarLabel = new GUIContent();

        #region  position & style


        Rect middleTreeViewRect
        {
            get { return new Rect(20, 30, position.width - 40, position.height * .5f - 60); }
        }

        Rect middleTreeViewRectBottom
        {
            get { return new Rect(20, position.height * .5f - 30, position.width - 40, position.height * .5f - 60); }
        }

        Rect toolbarRect
        {
            get { return new Rect(20f, 10f, position.width - 40f, 20f); }
        }

        Rect bottomToolbarRect
        {
            get { return new Rect(20f, position.height - 18f, position.width - 40f, 16f); }
        }
        Vector2 m_ScrollLeftPosition, m_ScrollRightPosition, m_ScrollMiddlePosition;

        #endregion


        string m_CopySToLeftStr = "<< all";
        string m_CopySToRightStr = "all >>";
        void DoTreeView(Rect rect)
        {
            //left:
            Rect rectL = GUILayoutUtility.GetRect(rect.width * 0.2f, rect.width * 0.45f, rect.height * 0.5f, rect.height);
            rectL = new Rect(rect.x, rectL.y, rect.width * 0.45f, rectL.height);
            m_LeftTreeView.OnGUI(rectL);

            //middle
            var pos = new Rect(rectL);
            pos.y += 25;
            pos.x = rectL.xMax + 2;
            pos.width = rect.width * 0.08f;

            pos.height = 30;
            var style = "miniButton";

            if (GUI.Button(pos, m_CopySToLeftStr, style))
            {
                CopyAllComp(m_RightSelectGo, m_LeftSelectGo);
            }
            pos.y += pos.height - 5;
            if (GUI.Button(pos, m_CopySToRightStr, style))
            {
                CopyAllComp(m_LeftSelectGo, m_RightSelectGo);
            }

            pos.y += pos.height;//* 1.8f;
            if (GUI.Button(pos, "< bind", style))
            {
                CopyAllBindableObject(m_RightSelectGo, m_LeftSelectGo);
            }
            pos.y += pos.height - 5;
            if (GUI.Button(pos, "bind >", style))
            {
                CopyAllBindableObject(m_LeftSelectGo, m_RightSelectGo);
            }

            pos.y += pos.height;//* 2;
            if (GUI.Button(pos, "对比全部", style))
            {
                sb.Clear();
                CompareMonoBehaviours(m_RightSelectGo, m_LeftSelectGo);
                HugulaEditor.EditorUtils.WriteToTmpFile($"{GetSafeFileName(m_RightSelectGo.name)} {GetSafeFileName(m_LeftSelectGo.name)}__diff.txt", sb.ToString());
                Debug.Log(sb.ToString());
            }
            pos.y += pos.height - 5;
            if (GUI.Button(pos, "对比绑定", style))
            {
                sb.Clear();
                CompareBindables(m_RightSelectGo, m_LeftSelectGo);
                HugulaEditor.EditorUtils.WriteToTmpFile($"{GetSafeFileName(m_RightSelectGo.name)} {GetSafeFileName(m_LeftSelectGo.name)}_bindable_diff.txt", sb.ToString());
                Debug.Log(sb.ToString());

            }
            pos.y += pos.height + 5;
            if (GUI.Button(pos, "左右换位", style))
            {
                var left = m_LeftGORoot;
                m_LeftGORoot = m_RightGORoot;
                m_LeftTreeView.SetGameObject(m_LeftGORoot);

                m_RightGORoot = left;
                m_RightTreeView.SetGameObject(m_RightGORoot);
            }

            //right
            Rect rectR = GUILayoutUtility.GetRect(rect.width * 0.2f, rect.width * 0.45f, rect.height * 0.5f, rect.height);
            rectR = new Rect(rect.width * .55f, rectL.y, rect.width * 0.45f, rectR.height);

            m_RightTreeView.OnGUI(rectR);
        }

        void DrawInspector(Rect rect)
        {
            //left
            Rect rectL = rect;
            rectL.width = rect.width * .48f;
            GUILayout.BeginArea(rectL);
            m_ScrollLeftPosition = EditorGUILayout.BeginScrollView(m_ScrollLeftPosition);
            using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(rectL.width)))
            {
                if (m_LeftSelectGo != null)
                {
                    DrawMonoItem(m_LeftSelectGo, rectL);
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            //right
            Rect rectR = rect;
            rectR.width = rect.width * .5f;
            rectR.x = rect.width * .5f + 10;
            GUILayout.BeginArea(rectR);
            m_ScrollRightPosition = EditorGUILayout.BeginScrollView(m_ScrollRightPosition);
            using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(rectR.width)))
            {
                if (m_RightSelectGo != null)
                {
                    DrawMonoItem(m_RightSelectGo, rectR);
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

        }

        void DrawMonoItem(GameObject item, Rect rect)
        {
            MonoBehaviour[] bindable;
            if (monoFilterIndex == 0)
                bindable = item.GetComponents<MonoBehaviour>();
            else
                bindable = item.GetComponents<BindableObject>();
            foreach (var obj in bindable)
            {
                if (monoStyleIndex >= 2)
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(rect.width)))
                    {
                        bool m_IsSelected = false;
                        int m_SIdx = -1;
                        if (m_SelectedMonos.TryGetValue(item, out var list))
                        {
                            m_SIdx = list.IndexOf(obj);
                        }

                        m_IsSelected = m_SIdx >= 0;
                        var sel = EditorGUILayout.Toggle(m_IsSelected, GUILayout.MaxWidth(20));
                        if (sel && m_SIdx == -1) //初次选择
                        {
                            if (list == null)
                            {
                                list = new List<MonoBehaviour>();
                                m_SelectedMonos.Add(item, list);
                            }
                            list.Add(obj);
                            SetCopySelectStr(item);
                        }
                        else if (!sel && list != null) //取消选中
                        {
                            list.Remove(obj);
                            if (list.Count == 0) m_SelectedMonos.Remove(item);
                            SetCopySelectStr(item);
                        }
                        m_BarLabel.text = obj.name;
                        EditorGUILayout.ObjectField(m_BarLabel, obj, obj.GetType(), true, GUILayout.MaxWidth(rect.width - 25));
                    }
                }
                else
                {
                    var editor = Editor.CreateEditor(obj);
                    editor.DrawHeader();
                    if (monoStyleIndex == 0)
                        editor.OnInspectorGUI();
                    else
                        editor.DrawDefaultInspector();
                }
            }
        }

        bool m_ExpandAllTreeView = true;
        string[] options = new string[] { "Mono Detail", "Mono Simple", "Mono Title" };
        int monoStyleIndex = 0;
        string[] optionsMono = new string[] { "Show All Mono", "Show BindableObject" };
        int monoFilterIndex = 1;
        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);
            using (new EditorGUILayout.HorizontalScope())
            {

                bool expandAll = GUILayout.Toggle(m_ExpandAllTreeView, "Expand All Tree");
                if (expandAll != m_ExpandAllTreeView)
                {
                    m_ExpandAllTreeView = expandAll;
                    if (m_ExpandAllTreeView)
                    {
                        m_LeftTreeView.ExpandAll();
                        m_RightTreeView.ExpandAll();
                    }
                    else
                    {
                        m_LeftTreeView.CollapseAll();
                        m_RightTreeView.CollapseAll();
                    }
                }
                GUILayout.Space(10);
                monoStyleIndex = EditorGUILayout.Popup(monoStyleIndex, options, GUILayout.MaxWidth(150));
                GUILayout.Space(10);
                monoFilterIndex = EditorGUILayout.Popup(monoFilterIndex, optionsMono, GUILayout.MaxWidth(150));
                GUILayout.FlexibleSpace();

            }
            GUILayout.EndArea();
        }


        #region 
        void onViewItemSelected(TreeView treeView, TreeViewItem clickedItem, bool keepMultiSelection, bool useActionKeyAsShift)
        {
            var sTreeView = treeView as SimpleTreeView;
            Selection.activeInstanceID = clickedItem.id;
            GameObject targetGameObject = sTreeView.GetGameObject(clickedItem.id);//  EditorUtility.InstanceIDToObject(clickedItem.id) as GameObject;
            if (targetGameObject == null)
            {
                Debug.LogWarning("GameObject with instance ID " + clickedItem.id + " not found in scene.");
                return;
            }

            if (treeView == m_LeftTreeView)
            {
                m_LeftSelectGo = targetGameObject;
                m_LeftSelectSerial = new SerializedObject(m_LeftSelectGo);
            }
            else
            {
                m_RightSelectGo = targetGameObject;
            }

            SetCopySelectStr(targetGameObject);

        }

        void SetCopySelectStr(GameObject sel)
        {
            if (sel == m_LeftSelectGo) //选中左边
            {
                if (m_SelectedMonos.ContainsKey(m_LeftSelectGo))
                    m_CopySToRightStr = "sel >>";
                else
                    m_CopySToRightStr = "all >>";
            }
            else //右边
            {
                if (m_SelectedMonos.ContainsKey(sel))
                    m_CopySToLeftStr = "<< sel";
                else
                    m_CopySToLeftStr = "<< all";
            }
        }

        Dictionary<GameObject, List<MonoBehaviour>> m_SelectedMonos = new Dictionary<GameObject, List<MonoBehaviour>>();
        #endregion

        #region  copy
        //拷贝所有脚本
        void CopyAllComp(GameObject source, GameObject target)
        {
            if (!source || !target) return;

            if (EditorUtility.DisplayDialog("MonoBehaviour覆盖", $"copy:\n{GetParentPath(source)} \n\n to:\n{GetParentPath(target)}", "确定", "取消"))
            {
                var allSource = source.GetComponents<MonoBehaviour>();
                Type typ;
                foreach (var s in allSource)
                {
                    typ = s.GetType();
                    var t = target.GetComponent(typ);
                    if (t == null)
                    {
                        t = target.AddComponent(typ);
                    }
                    if (t != null)
                    {
                        Undo.RecordObject(t, "Paste Component");
                        EditorUtility.CopySerialized(s, t);
                        Debug.Log($"paste to {t} success!");
                        if (t is BindableObject)
                            HugulaEditor.ObjectExtension.CallMethod(t, "ClearBindRef");//清理旧的引用。

                    }
                    else
                    {
                        Debug.LogError($"paste to {t} fail! 目标类型:{typ}不能添加！");
                    }
                }
            }
        }
        //拷贝绑定的脚本
        void CopyAllBindableObject(GameObject source, GameObject target)
        {
            if (!source || !target) return;

            if (EditorUtility.DisplayDialog("BindableObject覆盖", $"copy:\n{GetParentPath(source)} \n\n to:\n{GetParentPath(target)}", "确定", "取消"))
            {
                var allSource = source.GetComponents<BindableObject>();
                Type typ;
                foreach (var s in allSource)
                {
                    typ = s.GetType();
                    var t = target.GetComponent(typ);
                    if (t == null)
                    {
                        t = target.AddComponent(typ);
                    }
                    if (t != null)
                    {
                        Undo.RecordObject(t, "Paste Component");
                        EditorUtility.CopySerialized(s, t);
                        Debug.Log($"paste to {t} success!");

                        HugulaEditor.ObjectExtension.CallMethod(t, "ClearBindRef");//清理旧的引用。
                    }
                    else
                    {
                        Debug.LogError($"paste to {t} fail! 目标类型:{typ}不能添加！");
                    }
                }
            }

        }
        StringBuilder sb = new StringBuilder();
        private void CompareMonoBehaviours(GameObject source, GameObject target)
        {
            if (!source || !target) return;

            Debug.Log($"Comparing MonoBehaviours on {source.name} and {target.name}");
            sb.AppendLine($"Comparing MonoBehaviours on {source.name} and {target.name}");

            foreach (var component in source.GetComponents<MonoBehaviour>())
            {
                var otherComponent = target.GetComponent(component.GetType());

                if (otherComponent == null)
                {
                    sb.AppendLine($"warning:        {GetParentPath(target)} doesn't have {component.GetType().Name}");
                    // Debug.Log($"{GetParentPath(target)} doesn't have {component.GetType().Name}");
                }
                else
                {
                    var componentSerializedObject = new SerializedObject(component);
                    var otherComponentSerializedObject = new SerializedObject(otherComponent);
                    var property = componentSerializedObject.GetIterator();
                    var otherProperty = otherComponentSerializedObject.GetIterator();

                    sb.AppendLine($"    {component.GetType().Name}: {property.displayName} differs");

                    while (property.NextVisible(true) && otherProperty.NextVisible(true))
                    {
                        if (property.propertyType != otherProperty.propertyType)
                        {
                            continue;
                        }

                        if (property.type != otherProperty.type)
                        {
                            continue;
                        }

                        if (property.name == "m_Script" || otherProperty.name == "m_Script")
                        {
                            continue;
                        }

                        if (!SerializedProperty.EqualContents(property, otherProperty))
                        {
                            var propertyValue = GetSerializedPropertyValue(property);
                            var otherPropertyValue = GetSerializedPropertyValue(otherProperty);
                            sb.AppendLine($"                {source.name}.{component.GetType().Name}.{property.displayName}!= {target.name}.{component.GetType().Name}.{otherProperty.displayName}   [{propertyValue}!={otherPropertyValue}]");
                        }
                    }

                }
            }
        }
        string GetSerializedPropertyValue(SerializedProperty property)
        {
            var re = string.Empty;
            var propertyType = (SerializedPropertyType)property.propertyType;

            switch (propertyType)
            {
                case SerializedPropertyType.ManagedReference:
                    // re = property.managedReferenceValue.ToString();
                    break;
                case SerializedPropertyType.Generic:
                    // re = property.ToString();
                    break;
                case SerializedPropertyType.ObjectReference:
                    re = property.objectReferenceValue?.name;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    re = property.animationCurveValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    re = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Integer:
                    re = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Color:
                    re = property.colorValue.ToString();
                    break;
                case SerializedPropertyType.Bounds:
                    re = property.boundsValue.ToString();
                    break;
                case SerializedPropertyType.Rect:
                    re = property.rectValue.ToString();
                    break;
                case SerializedPropertyType.Enum:
                    re = string.Concat(property.enumDisplayNames, ",");
                    break;
                case SerializedPropertyType.Quaternion:
                    re = property.quaternionValue.ToString();
                    break;
                case SerializedPropertyType.Vector2:
                    re = property.vector2Value.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    re = property.vector3Value.ToString();
                    break;
                case SerializedPropertyType.Vector4:
                    re = property.vector4Value.ToString();
                    break;
                case SerializedPropertyType.Float:
                    re = property.floatValue.ToString();
                    break;
                case SerializedPropertyType.ArraySize:

                    break;
                case SerializedPropertyType.BoundsInt:
                    re = property.boundsIntValue.ToString();
                    break;
                case SerializedPropertyType.String:
                    re = property.stringValue;
                    break;
                default:
                    break;
            }

            return re;
        }


        private void CompareBindables(GameObject source, GameObject target)
        {
            if (!source || !target) return;

            var sArr = source.GetComponents<BindableObject>();
            Debug.Log($"Comparing Bindables on {source.name} and {target.name}");
            sb.AppendLine($"Comparing Bindables on {source.name} and {target.name}");
            foreach (var bindable in sArr)
            {
                var otherComponent = (BindableObject)target.GetComponent(bindable.GetType());

                if (otherComponent == null)
                {
                    sb.AppendLine($"warning:        {GetParentPath(target)} doesn't have {bindable.GetType().Name}");
                }
                else
                {
                    sb.AppendLine($"    Comparing {bindable} and {otherComponent}");
                    sb.AppendLine($"                ");
                    //check bindings
                    var bindings = bindable.GetBindings();
                    foreach (var bind in bindings)
                    {
                        var tarBinding = otherComponent.GetBinding(bind.propertyName);
                        if (tarBinding == null)
                        {
                            sb.AppendLine($"                {target.name} doesn't have Binding({bind.propertyName})");
                        }
                        else
                        {
                            var str = "";
                            if (bind.path != tarBinding.path)
                            {
                                str += $"    path:{bind.path} != {tarBinding.path} ";
                            }
                            else if (bind.format != tarBinding.format)
                            {
                                str += $"    format:  {bind.format} != {tarBinding.format} ";
                            }
                            else if (bind.target != tarBinding.target)
                            {
                                // str+= $"target:{bind.target} != {tarBinding.target} ";
                            }
                            else if (bind.source != tarBinding.source)
                            {
                                str += $"    source:{bind.source} != {tarBinding.source} ";

                            }
                            else if (bind.converter != tarBinding.converter)
                            {
                                str += $"    converter:{bind.converter} != {tarBinding.converter} ";

                            }
                            else if (bind.mode != tarBinding.mode)
                            {
                                str += $"    mode:{bind.mode} != {tarBinding.mode} ";
                            }

                            if (!string.IsNullOrEmpty(str)) sb.Append(str);
                        }
                    }
                }
            }


        }

        string pattern = "[" + Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())) + "]";
        string GetSafeFileName(string fileName)
        {
            fileName = Regex.Replace(fileName, pattern, "_");
            return fileName;
        }

        string GetParentPath(GameObject go)
        {
            string path = go.ToString();
            var parent = go.transform.parent;
            int count = 0;
            while (parent != null)
            {
                path = parent.name + "\n  " + path;
                parent = parent.parent;
                count++;

                if (count >= 2) break;
            }
            return path;
        }

        #endregion
    }
}