using System;
using System.Collections.Generic;
using System.Reflection;
using Hugula.Databinding;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.UIComponents {

    public partial class AssetsMenu {

        [MenuItem ("GameObject/UI/Create LoopScrollRect")]
        static public void CreateLoopScrollRect (MenuCommand menuCommand) {
            GameObject root = new GameObject ("Loop Scroll View", typeof (RectTransform), typeof (CanvasRenderer));
            RectTransform rootRT = root.GetComponent<RectTransform> ();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.sizeDelta = Vector2.zero;
            rootRT.pivot = new Vector2 (.5f, .5f);
            root.SetActive (false);
            AddImage (root);
            LoopScrollRect loopScrollRect = root.AddComponent<LoopScrollRect> ();

            //create ViewPort
            var viewport = new GameObject ("ViewPort", typeof (RectTransform), typeof (CanvasRenderer));
            viewport.AddComponent<Image> ();
            viewport.AddComponent<Mask> ();
            viewport.transform.SetParent (root.transform, false);
            RectTransform viewportRT = viewport.GetComponent<RectTransform> ();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            loopScrollRect.viewport = viewportRT;

            //create content 
            var content = new GameObject ("Content", typeof (RectTransform));
            content.transform.SetParent (viewport.transform, false);
            RectTransform contentRT = content.GetComponent<RectTransform> ();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2 (0, 300);
            contentRT.pivot = Vector2.up;
            loopScrollRect.content = contentRT;

            //create itemSource
            var itemContainer = new GameObject ("ItemContainer", typeof (RectTransform));
            itemContainer.SetActive (false);
            itemContainer.transform.SetParent (root.transform, false);
            var itemSource = new GameObject ("ItemSource", typeof (RectTransform));
            itemSource.transform.SetParent (itemContainer.transform, false);
            RectTransform rt2 = itemSource.GetComponent<RectTransform> ();
            rt2.anchorMin = Vector2.up;
            rt2.anchorMax = Vector2.up;
            rt2.sizeDelta = new Vector2 (100, 100);
            rt2.pivot = Vector2.up;
            var bindableContainer = itemSource.AddComponent<BindableContainer> ();

            loopScrollRect.itemSource = bindableContainer;

            GameObject parent = menuCommand.context as GameObject; // Selection.activeGameObject;
            root.SetActive (true);
            GameObjectUtility.SetParentAndAlign (root, parent);

        }

        private static Sprite standard;
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        static Image AddImage (GameObject obj) {
            var image = obj.AddComponent<Image> ();
            if (standard == null) standard = AssetDatabase.GetBuiltinExtraResource<Sprite> (kStandardSpritePath);
            image.sprite = standard;
            image.type = Image.Type.Sliced;
            return image;
        }
    }

    [CustomEditor (typeof (LoopScrollBase), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the ScrollRect Component.
    ///   Extend this class to write a custom editor for an ScrollRect-derived component.
    /// </summary>
    public class LoopScrollRectEditor : UnityEditor.Editor {

        protected SerializedProperty m_Columns;
        protected SerializedProperty m_ItemSource;
        protected SerializedProperty m_RenderPerFrames;
        protected SerializedProperty m_ItemSize;
        protected SerializedProperty m_Padding;

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

        protected virtual void OnEnable () {
            m_Content = serializedObject.FindProperty ("m_Content");
            m_Horizontal = serializedObject.FindProperty ("m_Horizontal");
            m_Vertical = serializedObject.FindProperty ("m_Vertical");
            m_MovementType = serializedObject.FindProperty ("m_MovementType");
            m_Elasticity = serializedObject.FindProperty ("m_Elasticity");
            m_Inertia = serializedObject.FindProperty ("m_Inertia");
            m_DecelerationRate = serializedObject.FindProperty ("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty ("m_ScrollSensitivity");
            m_Viewport = serializedObject.FindProperty ("m_Viewport");
            m_HorizontalScrollbar = serializedObject.FindProperty ("m_HorizontalScrollbar");
            m_VerticalScrollbar = serializedObject.FindProperty ("m_VerticalScrollbar");
            m_HorizontalScrollbarVisibility = serializedObject.FindProperty ("m_HorizontalScrollbarVisibility");
            m_VerticalScrollbarVisibility = serializedObject.FindProperty ("m_VerticalScrollbarVisibility");
            m_HorizontalScrollbarSpacing = serializedObject.FindProperty ("m_HorizontalScrollbarSpacing");
            m_VerticalScrollbarSpacing = serializedObject.FindProperty ("m_VerticalScrollbarSpacing");
            m_OnValueChanged = serializedObject.FindProperty ("m_OnValueChanged");

            m_ShowElasticity = new AnimBool (Repaint);
            m_ShowDecelerationRate = new AnimBool (Repaint);
            SetAnimBools (true);

            // m_Viewport.serializedObject =  (RectTransform)((GameObject)target).transform;
            //新增加属性
            m_ItemSource = serializedObject.FindProperty ("m_ItemSource");
            m_RenderPerFrames = serializedObject.FindProperty ("m_RenderPerFrames");
            m_ItemSize = serializedObject.FindProperty ("m_ItemSize");
            m_Padding = serializedObject.FindProperty ("m_Padding");
            m_Columns = serializedObject.FindProperty ("m_Columns");

            Init ();
        }

        protected virtual void OnDisable () {
            m_ShowElasticity.valueChanged.RemoveListener (Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener (Repaint);
        }

        protected void SetAnimBools (bool instant) {
            SetAnimBool (m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int) ScrollRect.MovementType.Elastic, instant);
            SetAnimBool (m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        protected void SetAnimBool (AnimBool a, bool value, bool instant) {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        protected virtual void Init () {
            if (targets.Length == 1) {
                RectTransform transform = (RectTransform) ((LoopScrollBase) target).transform;
                int colums = m_Columns.intValue;
                if (m_ItemSource.objectReferenceValue == null) {
                    var item = transform.Find ("ItemSource");
                    if (item != null) {
                        if (item.childCount > 0) {
                            item = item.GetChild (0);
                            m_ItemSource.objectReferenceValue = item;
                            Debug.LogFormat (" m_ItemSource = {0}", item);
                        }
                    }
                }

                if (m_Content.objectReferenceValue == null) {
                    var find1 = transform.Find ("Content");
                    m_Content.objectReferenceValue = find1;
                    Debug.LogFormat (" m_Content = {0}", m_Content.objectReferenceValue);
                }

                if (m_Viewport.objectReferenceValue == null) {
                    m_Viewport.objectReferenceValue = transform.Find ("Viewport");
                    Debug.LogFormat (" m_Viewport = {0}", m_Viewport.objectReferenceValue);
                }

                if (m_Columns.intValue == 0) {
                    // if(m_Horizontal.boolValue) = true; //水平
                    m_Vertical.boolValue = false;
                } else {
                    m_Horizontal.boolValue = false;
                    // m_Vertical.boolValue = true;
                }

                //设置对齐方式
                // RectTransform rect;
                // transform.pivot = new Vector2 (0f, 1f);
                // if ((rect = (RectTransform) m_Content.objectReferenceValue) != null && !EditorApplication.isPlaying) {
                //     // rect.pivot = Vector2.zero;
                //     var pivot = rect.pivot;
                //     if (colums == 0) //单列
                //     {
                //         pivot.x = 0; //左对齐

                //     } else if (colums == 1) // 上对齐
                //     {
                //         pivot.y = 1;
                //         if (rect.anchorMax.y != 1) //锚点必须上对齐
                //         {
                //             var anchor = rect.anchorMax;
                //             anchor.y = 1;
                //             rect.anchorMax = anchor;
                //             Debug.LogWarningFormat ("当({0})只有一列的时候{1}锚点必须上对齐 anchorMax= {2}", transform, rect, rect.anchorMax);
                //         }
                //         if (rect.anchorMin.y != 1) //锚点必须上对齐
                //         {
                //             var anchor = rect.anchorMin;
                //             anchor.y = 1;
                //             rect.anchorMin = anchor;
                //             Debug.LogWarningFormat ("当({0})只有一列的时候{1}锚点必须上对齐 anchorMin= {2}", transform, rect, rect.anchorMin);
                //         }
                //     } else //左上角对齐
                //     {
                //         pivot = new Vector2 (0f, 1f);
                //         rect.anchorMin = new Vector2 (0, 1);
                //         rect.anchorMax = new Vector2 (0, 1);
                //     }

                //     rect.pivot = pivot;
                //     rect.anchoredPosition3D = Vector3.zero;
                //     rect.anchoredPosition = Vector2.zero;
                //     // Debug.LogFormat("m_Content {0}.pivot = {1}", rect, pivot);
                // }

                // UnityEngine.Component obj;
                // if ((obj = (Component) m_ItemSource.objectReferenceValue) != null && (rect = (RectTransform) obj.transform) != null && !EditorApplication.isPlaying) {
                //     var pivot = rect.pivot;
                //     if (colums == 0) //单列
                //     {
                //         pivot.x = 0; //左对齐
                //     } else if (colums == 1) // 上对齐
                //     {
                //         pivot.y = 1;
                //     } else {
                //         pivot = new Vector2 (0f, 1f);
                //     }

                //     if (!rect.pivot.Equals (pivot)) {
                //         rect.pivot = pivot;
                //         Debug.LogFormat ("m_ItemSource {0}.pivot = {1}", rect, pivot);
                //     }

                // }
                serializedObject.ApplyModifiedProperties ();
            }
        }

        protected void CalculateCachedValues () {
            m_ViewportIsNotChild = false;
            m_HScrollbarIsNotChild = false;
            m_VScrollbarIsNotChild = false;
            if (targets.Length == 1) {
                Transform transform = ((ScrollRect) target).transform;

                if (m_Viewport.objectReferenceValue == null || ((RectTransform) m_Viewport.objectReferenceValue).parent != transform)
                    m_ViewportIsNotChild = true;
                if (m_HorizontalScrollbar.objectReferenceValue == null || ((Scrollbar) m_HorizontalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_HScrollbarIsNotChild = true;
                if (m_VerticalScrollbar.objectReferenceValue == null || ((Scrollbar) m_VerticalScrollbar.objectReferenceValue).transform.parent != transform)
                    m_VScrollbarIsNotChild = true;
            }
        }

        public override void OnInspectorGUI () {
            SetAnimBools (false);

            serializedObject.Update ();
            // Once we have a reliable way to know if the object changed, only re-cache in that case.
            CalculateCachedValues ();

            EditorGUILayout.PropertyField (m_Content);

            PropertyFieldChooseMono (m_ItemSource);
            // EditorGUILayout.PropertyField(m_ItemSource);

            EditorGUILayout.PropertyField (m_ItemSize);
            EditorGUILayout.PropertyField (m_Padding);
            EditorGUILayout.PropertyField (m_Columns);
            EditorGUILayout.PropertyField (m_RenderPerFrames);

            EditorGUILayout.PropertyField (m_Horizontal);
            EditorGUILayout.PropertyField (m_Vertical);

            EditorGUILayout.PropertyField (m_MovementType);
            if (EditorGUILayout.BeginFadeGroup (m_ShowElasticity.faded)) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField (m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup ();

            EditorGUILayout.PropertyField (m_Inertia);
            if (EditorGUILayout.BeginFadeGroup (m_ShowDecelerationRate.faded)) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField (m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup ();

            EditorGUILayout.PropertyField (m_ScrollSensitivity);

            EditorGUILayout.Space ();

            EditorGUILayout.PropertyField (m_Viewport);

            EditorGUILayout.PropertyField (m_HorizontalScrollbar);
            if (m_HorizontalScrollbar.objectReferenceValue && !m_HorizontalScrollbar.hasMultipleDifferentValues) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField (m_HorizontalScrollbarVisibility, EditorGUIUtility.TrTextContent ("Visibility"));

                if ((ScrollRect.ScrollbarVisibility) m_HorizontalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
                    !m_HorizontalScrollbarVisibility.hasMultipleDifferentValues) {
                    if (m_ViewportIsNotChild || m_HScrollbarIsNotChild)
                        EditorGUILayout.HelpBox (s_HError, MessageType.Error);
                    EditorGUILayout.PropertyField (m_HorizontalScrollbarSpacing, EditorGUIUtility.TrTextContent ("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField (m_VerticalScrollbar);
            if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField (m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent ("Visibility"));

                if ((ScrollRect.ScrollbarVisibility) m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
                    !m_VerticalScrollbarVisibility.hasMultipleDifferentValues) {
                    if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                        EditorGUILayout.HelpBox (s_VError, MessageType.Error);
                    EditorGUILayout.PropertyField (m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent ("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space ();

            EditorGUILayout.PropertyField (m_OnValueChanged);

            serializedObject.ApplyModifiedProperties ();
        }

        static List<string> allowTypes = new List<string> ();

        static UnityEngine.Component PopupGameObjectComponents (UnityEngine.Object obj, int i) {
            UnityEngine.Component selected = null;

            int selectIndex = 0;
            allowTypes.Clear ();
            if (obj != null) {

                Type currentType = obj.GetType ();
                GameObject go = null;
                if (currentType == typeof (GameObject))
                    go = (GameObject) obj;
                else if (currentType.IsSubclassOf (typeof (Component)))
                    go = ((Component) obj).gameObject;

                // allowTypes.Add (string.Format ("{0}({1})", go.GetType ().Name, go.name));

                if (go != null) {
                    Component[] comps = go.GetComponents (typeof (Component)); //GetComponents<Component>();
                    Component item = null;
                    for (int j = 0; j < comps.Length; j++) {
                        item = comps[j];
                        if (item)
                            allowTypes.Add (string.Format ("{0}({1})", item.GetType ().Name, item.name));
                        if (obj == item)
                            selectIndex = j;
                    }
                    selectIndex = EditorGUILayout.Popup (selectIndex, allowTypes.ToArray ());
                    selected = comps[selectIndex];
                }

            }
            return selected;
        }

        public static void PropertyFieldChooseMono (SerializedProperty prop) {
            EditorGUILayout.LabelField ("Template item type must is BindableContainer", GUILayout.MaxWidth (300));

            EditorGUILayout.BeginHorizontal ();

            EditorGUILayout.PropertyField (prop);

            if (prop.isArray) {

                var size = prop.arraySize;
                for (int i = 0; i < size; i++) {
                    var item = prop.GetArrayElementAtIndex (i);
                    var obj = item.objectReferenceValue as Component;
                    if (obj && !(obj is BindableContainer)) {
                        var bindable = obj.GetComponent<BindableContainer> ();
                        item.objectReferenceValue = bindable;
                        if (bindable == null) {
                            Debug.LogWarning ("Template item type must is BindableContainer .");
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal ();
        }

        public static string[] ConvertTypeArrayToStringArray (List<Type> tps) {
            List<string> temp = new List<string> ();
            for (int i = 0; i < tps.Count; i++) {
                string s = tps[i].ToString ();
                int index = s.LastIndexOf ('.');
                if (index != -1) {
                    index += 1;
                    s = s.Substring (index);
                }

                int n = 0;
                for (int j = 0; j < temp.Count; j++) {
                    string ts = temp[j].Split ('|') [0];
                    if (ts == s)
                        n += 1;
                }
                if (n > 0)
                    s += "|  " + n;
                temp.Add (s);
            }
            return temp.ToArray ();
        }
    }
}