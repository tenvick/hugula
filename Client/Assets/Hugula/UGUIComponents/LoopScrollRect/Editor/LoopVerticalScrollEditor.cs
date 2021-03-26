using System;
using System.Collections.Generic;
using Hugula.Databinding;
using Hugula.UIComponents;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace HugulaEditor.UIComponents
{

    public partial class AssetsMenu
    {

        [MenuItem("GameObject/UI/Create LoopVerticalScrollRect")]
        static public void CreateLoopVerticalScrollRect(MenuCommand menuCommand)
        {
            GameObject root = new GameObject("Loop Vertical Scroll View", typeof(RectTransform), typeof(CanvasRenderer));
            RectTransform rootRT = root.GetComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;
            rootRT.pivot = new Vector2(.5f, .5f);
            root.SetActive(false);
            AddImage(root);
            root.AddComponent<RectMask2D>();
            var loopScrollRect = root.AddComponent<LoopVerticalScrollRect>();

            //create content 
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(rootRT, false);
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;
            loopScrollRect.content = contentRT;

            //create Templates container
            var templateContainer = new GameObject("TemplatesContainer", typeof(RectTransform));
            templateContainer.transform.SetParent(rootRT, false);
            var itemContainerRT = templateContainer.GetComponent<RectTransform>();
            itemContainerRT.anchorMin = Vector2.up;
            itemContainerRT.anchorMax = Vector2.one;
            itemContainerRT.sizeDelta = new Vector2(0, 300);
            itemContainerRT.pivot = Vector2.up;
            templateContainer.SetActive(false);

            var itemTemplate = new GameObject("ItemTemplate", typeof(RectTransform));
            var itemTemplateRT = itemTemplate.GetComponent<RectTransform>();
            itemTemplateRT.SetParent(templateContainer.transform, false);
            itemTemplateRT.anchorMin = Vector2.up;
            itemTemplateRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            itemTemplateRT.pivot = Vector2.up;
            var bindableContainer = itemTemplate.AddComponent<BindableContainer>();

            loopScrollRect.templates = new BindableObject[1];
            loopScrollRect.templates[0] = bindableContainer;

            GameObject parent = menuCommand.context as GameObject; // Selection.activeGameObject;
            root.SetActive(true);
            GameObjectUtility.SetParentAndAlign(root, parent);

        }
    }

    [CustomEditor(typeof(LoopVerticalScrollRect), true)]
    [CanEditMultipleObjects]
    public class LoopVerticalScrollEditor : UnityEditor.Editor
    {
        protected SerializedProperty m_Templates;
        protected SerializedProperty m_RenderPerFrames;
        protected SerializedProperty m_PageSize;
        protected SerializedProperty m_Padding;

        protected SerializedProperty m_Content;
        protected SerializedProperty m_Elasticity;
        protected SerializedProperty m_Inertia;
        protected SerializedProperty m_DecelerationRate;
        protected SerializedProperty m_ScrollSensitivity;
        // protected SerializedProperty m_Viewport;
        protected SerializedProperty m_VerticalScrollbar;
        protected SerializedProperty m_VerticalScrollbarVisibility;
        protected SerializedProperty m_VerticalScrollbarSpacing;

        // protected SerializedProperty m_OnValueChanged;
        protected SerializedProperty m_AutoScrollToBottom;
        protected SerializedProperty m_CeilBar;
        protected SerializedProperty m_FloorBar;
        protected SerializedProperty m_DragOffsetShow;
        protected SerializedProperty m_OnEndDragChanged;
        protected SerializedProperty m_OnDragChanged;
        protected SerializedProperty m_PointerClickEvent;

        protected SerializedProperty m_OnBeginDragChanged;

        protected AnimBool m_ShowElasticity;
        protected AnimBool m_ShowDecelerationRate;
        protected bool m_ViewportIsNotChild, m_HScrollbarIsNotChild, m_VScrollbarIsNotChild;
        protected static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
        protected static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

        protected virtual void OnEnable()
        {
            m_Content = serializedObject.FindProperty("m_Content");

            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_Inertia = serializedObject.FindProperty("m_Inertia");
            m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            // m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_VerticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
            m_VerticalScrollbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
            // m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_PointerClickEvent = serializedObject.FindProperty("m_PointerClickEvent");
            m_OnBeginDragChanged = serializedObject.FindProperty("m_OnBeginDragChanged");
            m_OnDragChanged = serializedObject.FindProperty("m_OnDragChanged");
            m_OnEndDragChanged = serializedObject.FindProperty("m_OnEndDragChanged");

            m_AutoScrollToBottom = serializedObject.FindProperty("m_AutoScrollToBottom");
            m_CeilBar = serializedObject.FindProperty("m_CeilBar");
            m_FloorBar = serializedObject.FindProperty("m_FloorBar");
            m_DragOffsetShow = serializedObject.FindProperty("m_DragOffsetShow");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);

            // m_Viewport.serializedObject =  (RectTransform)((GameObject)target).transform;
            //新增加属性
            m_Templates = serializedObject.FindProperty("m_Templates");
            m_RenderPerFrames = serializedObject.FindProperty("m_RenderPerFrames");
            m_PageSize = serializedObject.FindProperty("m_PageSize");
            m_Padding = serializedObject.FindProperty("m_Padding");
            // m_Columns = serializedObject.FindProperty("m_Columns");

            Init();
        }

        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        protected void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, true, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        protected void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }
        protected virtual void Init()
        {
            if (targets.Length == 1)
            {
                RectTransform transform = (RectTransform)((MonoBehaviour)target).transform;
                // int colums = m_Columns.intValue;
                // if (m_ItemSource.objectReferenceValue == null) {
                //     var item = transform.GetChild (0);
                //     if (item != null) {
                //         if (item.childCount > 0) {
                //             item = item.GetChild (0);
                //             m_ItemSource.objectReferenceValue = item;
                //             Debug.LogFormat (" m_ItemSource = {0}", item);
                //         }
                //     }
                // }

                if (m_Content.objectReferenceValue == null)
                {
                    m_Content.objectReferenceValue = transform.GetChild(0);
                    Debug.LogFormat(" m_Content = {0}", m_Content.objectReferenceValue);
                }

                //设置对齐方式
                RectTransform rect;
                transform.pivot = new Vector2(0f, 1f);
                if ((rect = (RectTransform)m_Content.objectReferenceValue) != null && !EditorApplication.isPlaying)
                {
                    // rect.pivot = Vector2.zero;
                    var pivot = rect.pivot;
                    // if (colums == 0) //单列
                    // {
                    //     pivot.x = 0;//左对齐

                    // }
                    // else if (colums == 1) // 上对齐
                    {
                        pivot.y = 1;
                        if (rect.anchorMax.y != 1) //锚点必须上对齐
                        {
                            var anchor = rect.anchorMax;
                            anchor.y = 1;
                            rect.anchorMax = anchor;
                            Debug.LogWarningFormat("当({0})只有一列的时候{1}锚点必须上对齐 anchorMax= {2}", transform, rect, rect.anchorMax);
                        }
                        if (rect.anchorMin.y != 1) //锚点必须上对齐
                        {
                            var anchor = rect.anchorMin;
                            anchor.y = 1;
                            rect.anchorMin = anchor;
                            Debug.LogWarningFormat("当({0})只有一列的时候{1}锚点必须上对齐 anchorMin= {2}", transform, rect, rect.anchorMin);
                        }
                    }
                    // else //左上角对齐
                    // {
                    //     pivot = new Vector2(0f, 1f);
                    //     rect.anchorMin = new Vector2(0, 1);
                    //     rect.anchorMax = new Vector2(0, 1);
                    // }

                    rect.pivot = pivot;
                    rect.anchoredPosition3D = Vector3.zero;
                    rect.anchoredPosition = Vector2.zero;
                    // Debug.LogFormat("m_Content {0}.pivot = {1}", rect, pivot);
                }

                // UnityEngine.Component obj;
                // if ((obj = (Component) m_ItemSource.objectReferenceValue) != null && (rect = (RectTransform) obj.transform) != null && !EditorApplication.isPlaying) {
                //     var pivot = rect.pivot;
                //     // if (colums == 0) //单列
                //     // {
                //     //     pivot.x = 0;//左对齐
                //     // }
                //     // else if (colums == 1) // 上对齐
                //     {
                //         pivot.y = 1;
                //     }
                //     // else
                //     // {
                //     //     pivot = new Vector2(0f, 1f);
                //     // }

                //     if (!rect.pivot.Equals (pivot)) {
                //         rect.pivot = pivot;
                //         Debug.LogFormat ("m_ItemSource {0}.pivot = {1}", rect, pivot);
                //     }

                // }
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
                Transform transform = ((MonoBehaviour)target).transform;
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

            EditorGUILayout.PropertyField(m_Content);
            EditorGUILayout.PropertyField(m_CeilBar);
            EditorGUILayout.PropertyField(m_FloorBar);
            EditorGUILayout.PropertyField(m_AutoScrollToBottom);
            PropertyFieldChooseMono(m_Templates);
            GUILayout.Space(10);
            GUILayout.Label(new GUIContent("____________________________________________________________________________________________________"), GUILayout.MaxWidth(500));
            EditorGUILayout.PropertyField(m_PageSize);
            EditorGUILayout.PropertyField(m_Padding);
            // EditorGUILayout.PropertyField(m_Columns);
            EditorGUILayout.PropertyField(m_RenderPerFrames);
            EditorGUILayout.PropertyField(m_DragOffsetShow);

            // EditorGUILayout.PropertyField(m_Horizontal);
            // EditorGUILayout.PropertyField(m_Vertical);
            // EditorGUILayout.PropertyField(m_MovementType);

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

            // EditorGUILayout.PropertyField(m_Viewport);

            EditorGUILayout.PropertyField(m_VerticalScrollbar);
            if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;

                if (m_VerticalScrollbarVisibility != null)
                {
                    EditorGUILayout.PropertyField(m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                    if ((ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
                        !m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
                    {
                        if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                            EditorGUILayout.HelpBox(s_VError, MessageType.Error);
                        EditorGUILayout.PropertyField(m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_PointerClickEvent);
            EditorGUILayout.PropertyField(m_OnBeginDragChanged);
            EditorGUILayout.PropertyField(m_OnDragChanged);
            EditorGUILayout.PropertyField(m_OnEndDragChanged);

            EditorGUILayout.LabelField("Layout ", GUILayout.Width(200));

            serializedObject.ApplyModifiedProperties();
            SimulateLayout();
        }

        void PropertyFieldChooseMono(SerializedProperty prop)
        {
            EditorGUILayout.LabelField("Template item type must is BindableContainer", GUILayout.MaxWidth(300));

            // EditorGUILayout.PropertyField (prop);

            if (prop.isArray)
            {
                // EditorGUILayout.PropertyField (prop);
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
                    if (obj && !(obj is BindableContainer))
                    {
                        var bindable = obj.GetComponent<BindableContainer>();
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

        protected void SimulateLayout()
        {
            var temp = target as LoopVerticalScrollRect;

            if (GUILayout.Button("Simulate Layout"))
            {
                temp.CallMethod("Awake");
                temp.CallMethod("Start");
                var len = temp.templates.Length;
                temp.onGetItemTemplateType = (object obj, int idx) =>
                  {
                      return UnityEngine.Random.RandomRange(0, len - 1);
                  };

                temp.dataLength = 50;

                temp.CallMethod("Refresh");
                for (int i = 0; i < temp.pageSize; i++)
                    temp.CallMethod("LateUpdate");

                Debug.LogFormat("Simulate Layout {0}.dataLength= {1} ", temp, temp.dataLength);
            }

            if (GUILayout.Button("Clear Simulate"))
            {
                temp.dataLength = 0;
                var content = temp.content;
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(content.GetChild(i).gameObject);
                }
                Debug.Log("Clear Simulate");
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();

            }


        }

    }
}