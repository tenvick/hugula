using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using Hugula.UIComponents;
using System.IO;
using System.Text;
using System.Linq;
using Hugula.Utils;


namespace HugulaEditor.UIComponents
{

    public partial class AssetsMenu
    {

        [MenuItem("GameObject/UI/Create LoopScrollRect")]
        static public void CreateLoopScrollRect(MenuCommand menuCommand)
        {
            GameObject root = new GameObject("Loop Scroll View", typeof(RectTransform), typeof(CanvasRenderer));
            RectTransform rootRT = root.GetComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;
            rootRT.pivot = new Vector2(.5f, .5f);
            root.SetActive(false);
            AddImage(root);
            LoopScrollRect loopScrollRect = root.AddComponent<LoopScrollRect>();

            //create ViewPort
            var viewport = new GameObject("ViewPort", typeof(RectTransform), typeof(CanvasRenderer));
            viewport.AddComponent<Image>();
            viewport.AddComponent<Mask>();
            viewport.transform.SetParent(root.transform, false);
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            loopScrollRect.viewport = viewportRT;

            //create content 
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;
            loopScrollRect.content = contentRT;

            //create ItemContainer
            var itemContainer = new GameObject("ItemContainer", typeof(RectTransform));
            itemContainer.transform.SetParent(root.transform, false);
            var itemContainerRT = itemContainer.GetComponent<RectTransform>();
            itemContainerRT.anchorMin = Vector2.zero;
            itemContainerRT.anchorMax = Vector2.one;
            itemContainerRT.sizeDelta = Vector2.zero;
            itemContainerRT.pivot = Vector2.up;
            itemContainer.SetActive(false);

            //create ItemSource
            var itemSource = new GameObject("ItemSource", typeof(RectTransform));
            itemSource.transform.SetParent(itemContainer.transform, false);
            RectTransform rt2 = itemSource.GetComponent<RectTransform>();
            rt2.anchorMin = Vector2.up;
            rt2.anchorMax = Vector2.up;
            rt2.sizeDelta = new Vector2(100, 100);
            rt2.pivot = Vector2.up;
            var bindableContainer = itemSource.AddComponent<BindableContainer>();

            // loopScrollRect.itemSource = bindableContainer;
            loopScrollRect.templates = new BindableObject[1];
            loopScrollRect.templates[0] = bindableContainer;

            GameObject parent = menuCommand.context as GameObject; // Selection.activeGameObject;
            root.SetActive(true);
            GameObjectUtility.SetParentAndAlign(root, parent);

        }

        private static Sprite standard;
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        static Image AddImage(GameObject obj)
        {
            var image = obj.AddComponent<Image>();
            if (standard == null) standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.sprite = standard;
            image.type = Image.Type.Sliced;
            return image;
        }
    }

    [CustomEditor(typeof(LoopScrollBase), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the ScrollRect Component.
    ///   Extend this class to write a custom editor for an ScrollRect-derived component.
    /// </summary>
    public class LoopScrollRectEditor : UnityEditor.Editor
    {

        protected SerializedProperty m_Columns;
        protected SerializedProperty m_Templates;
        protected SerializedProperty m_RemoveEasing;
        protected SerializedProperty m_ItemSize;
        protected SerializedProperty m_Padding;

        protected SerializedProperty m_SelectedIndex;
        protected SerializedProperty m_ScrollTime;
        protected SerializedProperty m_ScrollDataSize;
        protected SerializedProperty m_ContentLocalStart;
        protected SerializedProperty m_Content;
        protected SerializedProperty m_Horizontal;
        protected SerializedProperty m_Vertical;
        protected SerializedProperty m_MovementType;
        protected SerializedProperty m_Elasticity;
        protected SerializedProperty m_Inertia;
        protected SerializedProperty m_DecelerationRate;
        protected SerializedProperty m_ScrollSensitivity;
        protected SerializedProperty m_Viewport;
        protected SerializedProperty m_HorizontalScrollbar;
        protected SerializedProperty m_VerticalScrollbar;
        protected SerializedProperty m_HorizontalScrollbarVisibility;
        protected SerializedProperty m_VerticalScrollbarVisibility;
        protected SerializedProperty m_HorizontalScrollbarSpacing;
        protected SerializedProperty m_VerticalScrollbarSpacing;
        protected SerializedProperty m_OnValueChanged;
        protected AnimBool m_ShowElasticity;
        protected AnimBool m_ShowDecelerationRate;
        protected bool m_ViewportIsNotChild, m_HScrollbarIsNotChild, m_VScrollbarIsNotChild;
        protected static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
        protected static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

        protected virtual void OnEnable()
        {
            m_ContentLocalStart = serializedObject.FindProperty("m_ContentLocalStart");
            m_Content = serializedObject.FindProperty("m_Content");
            m_Horizontal = serializedObject.FindProperty("m_Horizontal");
            m_Vertical = serializedObject.FindProperty("m_Vertical");
            m_MovementType = serializedObject.FindProperty("m_MovementType");
            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_Inertia = serializedObject.FindProperty("m_Inertia");
            m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_HorizontalScrollbar = serializedObject.FindProperty("m_HorizontalScrollbar");
            m_VerticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
            m_HorizontalScrollbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
            m_VerticalScrollbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
            m_HorizontalScrollbarSpacing = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
            m_VerticalScrollbarSpacing = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_SelectedIndex = serializedObject.FindProperty("m_SelectedIndex");
            m_ScrollTime = serializedObject.FindProperty("m_ScrollTime");
            m_ScrollDataSize = serializedObject.FindProperty("m_ScrollDataSize");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);

            // m_Viewport.serializedObject =  (RectTransform)((GameObject)target).transform;
            //新增加属性
            m_Templates = serializedObject.FindProperty("m_Templates");
            m_RemoveEasing = serializedObject.FindProperty("m_RemoveEasing");
            m_ItemSize = serializedObject.FindProperty("m_ItemSize");
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Columns = serializedObject.FindProperty("m_Columns");

            Init();
        }


        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        protected void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        protected void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        protected void SimulateLayout()
        {
            var temp = target as LoopScrollRect;

            if (GUILayout.Button("Simulate Layout"))
            {
                temp.CallMethod("Awake");
                temp.CallMethod("Start");

                temp.dataLength = 50;
                Debug.LogFormat("pagesize={0} ", temp.pageSize);
                temp.CallMethod("Refresh");
                temp.CallMethod("LateUpdate");

                Debug.LogFormat("Simulate Layout {0}.dataLength= {1} ", temp, temp.dataLength);
            }

            if (GUILayout.Button("Clear Simulate"))
            {
                temp.dataLength = 0;
                var content = temp.content;
                Transform trans;
                string pattern = @"^\d+-\d+$";
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    trans = content.GetChild(i);
                    var objName = trans.gameObject.name;
                    //objName匹配 数字 {}_{}
                    if (objName.Contains("(Clone)") || System.Text.RegularExpressions.Regex.IsMatch(objName, pattern))
                        GameObject.DestroyImmediate(trans.gameObject);
                }
                Debug.Log("Clear Simulate");
                AssetDatabase.Refresh();
            }


        }

        protected virtual void Init()
        {
            if (targets.Length == 1)
            {
                RectTransform transform = (RectTransform)((LoopScrollBase)target).transform;
                if (m_Templates.arraySize == 0)
                {
                    List<BindableContainer> templateContainer = new List<BindableContainer>();
                    var item = transform.Find("ItemContainer");
                    if (item != null)
                    {
                        for (int i = 0; i < item.childCount; i++)
                        {
                            var child = item.GetChild(i);
                            var bindable = child.GetComponent<BindableContainer>();
                            if (bindable != null)
                            {
                                templateContainer.Add(bindable);
                            }
                        }
                    }

                    if (item == null || templateContainer.Count == 0)
                    {
                        for (int i = 0; i < transform.childCount; i++)
                        {
                            var child = transform.GetChild(i);
                            if (child.gameObject.activeSelf == false) //查找隐藏的模板项目
                            {
                                var bindable = child.GetComponent<BindableContainer>();
                                if (bindable != null)
                                {
                                    templateContainer.Add(bindable);
                                }
                            };
                        }
                    }

                    for (int i = 0; i < templateContainer.Count; i++)
                    {
                        var template = templateContainer[i];
                        m_Templates.InsertArrayElementAtIndex(0);
                        m_Templates.GetArrayElementAtIndex(i).objectReferenceValue = template;
                        Debug.LogFormat("<color=green> {0}.m_Templates = {1} </color>", CUtils.GetGameObjectFullPath(transform.gameObject), template);

                    }

                }

                if (m_Content.objectReferenceValue == null)
                {
                    var find1 = transform.Find("Content");
                    m_Content.objectReferenceValue = find1;
                    Debug.LogFormat(" m_Content = {0}", m_Content.objectReferenceValue);
                }

                if (m_Viewport.objectReferenceValue == null)
                {
                    m_Viewport.objectReferenceValue = transform.Find("Viewport");
                    Debug.LogFormat(" m_Viewport = {0}", m_Viewport.objectReferenceValue);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected void CalculateCachedValues()
        {
            m_ViewportIsNotChild = false;
            m_HScrollbarIsNotChild = false;
            m_VScrollbarIsNotChild = false;
            if (targets.Length == 1)
            {
                Transform transform = ((ScrollRect)target).transform;

                if (m_Viewport.objectReferenceValue == null || ((RectTransform)m_Viewport.objectReferenceValue).parent != transform)
                    m_ViewportIsNotChild = true;
                if (m_HorizontalScrollbar.objectReferenceValue == null || ((Scrollbar)m_HorizontalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_HScrollbarIsNotChild = true;
                if (m_VerticalScrollbar.objectReferenceValue == null || ((Scrollbar)m_VerticalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_VScrollbarIsNotChild = true;
            }
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);

            serializedObject.Update();
            // Once we have a reliable way to know if the object changed, only re-cache in that case.
            CalculateCachedValues();
            EditorGUILayout.PropertyField(m_ContentLocalStart);
            EditorGUILayout.PropertyField(m_Content);

            PropertyFieldChooseMono(m_Templates);
            // EditorGUILayout.PropertyField(m_Templates);

            EditorGUILayout.PropertyField(m_ItemSize);
            EditorGUILayout.PropertyField(m_Padding);
            EditorGUILayout.PropertyField(m_Columns);
            EditorGUILayout.PropertyField(m_RemoveEasing);
            EditorGUILayout.PropertyField(m_SelectedIndex);
            EditorGUILayout.PropertyField(m_ScrollTime);
            EditorGUILayout.PropertyField(m_ScrollDataSize);
            EditorGUILayout.PropertyField(m_Horizontal);
            EditorGUILayout.PropertyField(m_Vertical);

            EditorGUILayout.PropertyField(m_MovementType);
            if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Inertia);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Viewport);

            EditorGUILayout.PropertyField(m_HorizontalScrollbar);
            if (m_HorizontalScrollbar.objectReferenceValue && !m_HorizontalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_HorizontalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_HorizontalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
                    !m_HorizontalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_HScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_HError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_HorizontalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_VerticalScrollbar);
            if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
                    !m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_VError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OnValueChanged);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.LabelField("Layout ", GUILayout.Width(200));
            SimulateLayout();

        }

        static List<string> allowTypes = new List<string>();

        static UnityEngine.Component PopupGameObjectComponents(UnityEngine.Object obj, int i)
        {
            UnityEngine.Component selected = null;

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

                // allowTypes.Add (string.Format ("{0}({1})", go.GetType ().Name, go.name));

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
                            selectIndex = j;
                    }
                    selectIndex = EditorGUILayout.Popup(selectIndex, allowTypes.ToArray());
                    selected = comps[selectIndex];
                }

            }
            return selected;
        }

        public static void PropertyFieldChooseMono(SerializedProperty prop)
        {

            EditorGUILayout.LabelField("Template item type must is BindableObject", GUILayout.MaxWidth(300));

            // EditorGUILayout.PropertyField (prop);

            if (prop.isArray)
            {
                var size = prop.arraySize;
                GUILayout.Label(new GUIContent(prop.name.Replace("m_", "")), GUILayout.Width(160));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Size"), GUILayout.Width(60));
                GUILayout.TextField(size.ToString(), GUILayout.Width(60));
                if (GUILayout.Button("+"))
                {
                    prop.InsertArrayElementAtIndex(size);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical();
                for (int i = 0; i < size; i++)
                {
                    var item = prop.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(item);
                    var obj = item.objectReferenceValue as Component;
                    if (obj && !(obj is BindableObject))
                    {
                        var bindable = obj.GetComponent<BindableContainer>() ?? obj.GetComponent<BindableObject>();

                        item.objectReferenceValue = bindable;
                        if (bindable == null)
                        {
                            Debug.LogWarning("Template item type must is BindableContainer .");
                        }
                    }
                }
                if (GUILayout.Button("-"))
                {
                    prop.DeleteArrayElementAtIndex(size - 1);
                }
                EditorGUILayout.EndVertical();
            }
        }

        public static string[] ConvertTypeArrayToStringArray(List<Type> tps)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < tps.Count; i++)
            {
                string s = tps[i].ToString();
                int index = s.LastIndexOf('.');
                if (index != -1)
                {
                    index += 1;
                    s = s.Substring(index);
                }

                int n = 0;
                for (int j = 0; j < temp.Count; j++)
                {
                    string ts = temp[j].Split('|')[0];
                    if (ts == s)
                        n += 1;
                }
                if (n > 0)
                    s += "|  " + n;
                temp.Add(s);
            }
            return temp.ToArray();
        }
    }


    public class LoopScrollRectMenu
    {

        [MenuItem("Assets/LoopScrollRect/Refresh Templates Error Asset from Folder", true, 101)]
        private static bool ValidateRefreshLoopScrollRectByFolder()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
                {
                    return true;
                }
            }
            return false;
        }


        [MenuItem("Assets/LoopScrollRect/Refresh Templates Error Asset from Folder")]
        public static void RefreshLoopScrollRectByFolder()
        {
            // string path = string.Empty;
            StringBuilder sb = new StringBuilder();
            string tagName = string.Empty;

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assPath = AssetDatabase.GetAssetPath(obj);
                Debug.LogFormat("assPath={0} Exists={1}", assPath, Directory.Exists(assPath));
                // 如果是文件夹
                if (Directory.Exists(assPath))
                {
                    var prefabPaths = Directory.GetFiles(assPath, "*.prefab", SearchOption.AllDirectories);
                    Debug.LogFormat("foreach len ={0} ", prefabPaths.Length);

                    // 遍历文件夹下的prefab 找出所有的LoopScrollRect
                    foreach (var prefabPath in prefabPaths)
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        var loopScrollRects = prefab.GetComponentsInChildren<LoopScrollRect>(true);

                        foreach (var loopScrollRect in loopScrollRects)
                        {
                            var templates = loopScrollRect.templates;
                            var transform = loopScrollRect.transform;
                            if (templates.Length == 0)
                            {

                                List<BindableContainer> templateContainer = new List<BindableContainer>();
                                var item = transform.Find("ItemContainer");
                                if (item != null)
                                {
                                    for (int i = 0; i < item.childCount; i++)
                                    {
                                        var child = item.GetChild(i);
                                        var bindable = child.GetComponent<BindableContainer>();
                                        if (bindable != null)
                                        {
                                            templateContainer.Add(bindable);
                                        }
                                    }
                                }

                                if (item == null || templateContainer.Count == 0)
                                {
                                    for (int i = 0; i < transform.childCount; i++)
                                    {
                                        var child = transform.GetChild(i);
                                        if (child.gameObject.activeSelf == false) //查找隐藏的模板项目
                                        {
                                            var bindable = child.GetComponent<BindableContainer>();
                                            if (bindable != null)
                                            {
                                                templateContainer.Add(bindable);
                                            }
                                        };
                                    }
                                }
                           
                                if (templateContainer.Count > 0)
                                {
                                    loopScrollRect.templates = templateContainer.ToArray();//new BindableObject[1];
                                    Debug.LogFormat("<color=#90EE90> {0}.m_Templates = {1} </color>", CUtils.GetGameObjectFullPath(loopScrollRect.gameObject), templateContainer.Count);
                                    UnityEditor.EditorUtility.SetDirty(loopScrollRect);
                                }
                                else
                                {
                                    Debug.LogFormat($"<color=red> 没找到模板  {CUtils.GetGameObjectFullPath(loopScrollRect.gameObject)}  LoopScrollRect({loopScrollRect}) </color>");
                                }
                            }
                            else
                            {
                                Debug.Log($"<color=green>loopScrollRects={CUtils.GetGameObjectFullPath(loopScrollRect.gameObject)} templates.Length={templates.Length} </color>");
                            }
                        }
                    }
                }
            }
        }

    }

}