using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hugula.UIComponents {

    public class VerticalScrollBase : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler {
        [Serializable]
        /// <summary>
        /// Event type used by the ScrollRect.
        /// </summary>
        public class ScrollRectEvent : UnityEvent<Vector2> { }

        [SerializeField]
        private RectTransform m_Content;

        /// <summary>
        /// The content that can be scrolled. It should be a child of the GameObject with ScrollRect on it.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///     public RectTransform scrollableContent;
        ///
        ///     //Do this when the Save button is selected.
        ///     public void Start()
        ///     {
        ///         // assigns the contect that can be scrolled using the ScrollRect.
        ///         myScrollRect.content = scrollableContent;
        ///     }
        /// }
        /// </code>
        /// </example>
        public RectTransform content { get { return m_Content; } set { m_Content = value; } }

        [SerializeField]
        private Scrollbar m_VerticalScrollbar;

        /// <summary>
        /// Optional Scrollbar object linked to the vertical scrolling of the ScrollRect.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///     public Scrollbar newScrollBar;
        ///
        ///     public void Start()
        ///     {
        ///         // Assigns a scroll bar element to the ScrollRect, allowing you to scroll in the vertical axis.
        ///         myScrollRect.verticalScrollbar = newScrollBar;
        ///     }
        /// }
        /// </code>
        /// </example>
        public Scrollbar verticalScrollbar {
            get {
                return m_VerticalScrollbar;
            }
            set {
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.RemoveListener (SetVerticalNormalizedPosition);
                m_VerticalScrollbar = value;
                if (m_VerticalScrollbar)
                    m_VerticalScrollbar.onValueChanged.AddListener (SetVerticalNormalizedPosition);
                // SetDirtyCaching();
            }
        }

        private bool vScrollingNeeded {
            get {
                if (Application.isPlaying)
                    return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
                return true;
            }
        }

        /// <summary>
        /// The vertical scroll position as a value between 0 and 1, with 0 being at the bottom.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///     public Scrollbar newScrollBar;
        ///
        ///     public void Start()
        ///     {
        ///         //Change the current vertical scroll position.
        ///         myScrollRect.verticalNormalizedPosition = 0.5f;
        ///     }
        /// }
        /// </code>
        /// </example>

        public float verticalNormalizedPosition {
            get {
                UpdateBounds ();
                if (m_ContentBounds.yMax <= m_ViewBounds.yMax)
                    return (m_ViewBounds.x > m_ContentBounds.x) ? 1 : 0;

                return (m_ViewBounds.x - m_ContentBounds.x) / (m_ContentBounds.y - m_ViewBounds.y);
            }
            set {
                SetNormalizedPosition (value, 1);
            }
        }

        /// <summary>
        /// The horizontal scroll position as a value between 0 and 1, with 0 being at the left.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///     public Scrollbar newScrollBar;
        ///
        ///     public void Start()
        ///     {
        ///         //Change the current horizontal scroll position.
        ///         myScrollRect.horizontalNormalizedPosition = 0.5f;
        ///     }
        /// }
        /// </code>
        /// </example>
        public float horizontalNormalizedPosition {
            get {
                UpdateBounds ();
                if (m_ContentBounds.size.x <= m_ViewBounds.size.x)
                    return (m_ViewBounds.min.x > m_ContentBounds.min.x) ? 1 : 0;
                return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
            }
            set {
                SetNormalizedPosition (value, 0);
            }
        }

        [SerializeField]
        private float m_Elasticity = 0.1f;

        /// <summary>
        /// The amount of elasticity to use when the content moves beyond the scroll rect.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///
        ///     public void Start()
        ///     {
        ///         // assigns a new value to the elasticity of the scroll rect.
        ///         // The higher the number the longer it takes to snap back.
        ///         myScrollRect.elasticity = 3.0f;
        ///     }
        /// }
        /// </code>
        /// </example>
        public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

        [SerializeField]
        private bool m_Inertia = true;

        /// <summary>
        /// Should movement inertia be enabled?
        /// </summary>
        /// <remarks>
        /// Inertia means that the scrollrect content will keep scrolling for a while after being dragged. It gradually slows down according to the decelerationRate.
        /// </remarks>
        public bool inertia { get { return m_Inertia; } set { m_Inertia = value; } }

        [SerializeField]
        private float m_DecelerationRate = 0.135f; // Only used when inertia is enabled

        /// <summary>
        /// The rate at which movement slows down.
        /// </summary>
        /// <remarks>
        /// The deceleration rate is the speed reduction per second. A value of 0.5 halves the speed each second. The default is 0.135. The deceleration rate is only used when inertia is enabled.
        /// </remarks>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///
        ///     public void Start()
        ///     {
        ///         // assigns a new value to the decelerationRate of the scroll rect.
        ///         // The higher the number the longer it takes to decelerate.
        ///         myScrollRect.decelerationRate = 5.0f;
        ///     }
        /// }
        /// </code>
        /// </example>
        public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

        [SerializeField]
        private float m_ScrollSensitivity = 1.0f;

        /// <summary>
        /// The sensitivity to scroll wheel and track pad scroll events.
        /// </summary>
        /// <remarks>
        /// Higher values indicate higher sensitivity.
        /// </remarks>
        public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

        [SerializeField]
        private UnityEvent m_PointerClickEvent = new UnityEvent ();
        /// <summary>
        /// 点击事件
        /// </summary>
        public UnityEvent onPointerClick { get { return m_PointerClickEvent; } set { m_PointerClickEvent = value; } }

        [SerializeField]
        private ScrollRectEvent m_OnBeginDragChanged = new ScrollRectEvent ();

        /// <summary>
        /// 当开始拖动结束的时候触发的事件
        /// </summary>
        public ScrollRectEvent onBeginDragChanged { get { return m_OnBeginDragChanged; } set { m_OnBeginDragChanged = value; } }

        [SerializeField]
        private ScrollRectEvent m_OnEndDragChanged = new ScrollRectEvent ();

        /// <summary>
        /// 当拖动结束的时候触发事件
        /// </summary>
        public ScrollRectEvent onEndDragChanged { get { return m_OnEndDragChanged; } set { m_OnEndDragChanged = value; } }

        [SerializeField]
        private ScrollRectEvent m_OnDragChanged = new ScrollRectEvent ();
        /// <summary>
        /// 当拖动的时候触发事件
        /// </summary>
        public ScrollRectEvent onDragChanged { get { return m_OnDragChanged; } set { m_OnDragChanged = value; } }

        /// <summary>
        /// The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public ScrollRect myScrollRect;
        ///     public Vector2 myPosition = new Vector2(0.5f, 0.5f);
        ///
        ///     public void Start()
        ///     {
        ///         //Change the current scroll position.
        ///         myScrollRect.normalizedPosition = myPosition;
        ///     }
        /// }
        /// </code>
        /// </example>
        // public Vector2 normalizedPosition
        // {
        //     get
        //     {
        //         return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
        //     }
        //     set
        //     {
        //         SetNormalizedPosition(value.x, 0);
        //         SetNormalizedPosition(value.y, 1);
        //     }
        // }

        // The offset from handle position to mouse down position
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        protected Vector2 m_ContentStartPosition = Vector2.zero;

        private RectTransform m_ViewRect;

        protected RectTransform viewRect {
            get {
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform) transform;
                return m_ViewRect;
            }
        }
        private bool m_Dragging;
        private Vector2 m_PrevPosition = Vector2.zero;
        private Rect m_PrevContentBounds;
        private Rect m_PrevViewBounds;

        protected Rect m_ContentBounds;

        protected Rect m_ViewBounds;

        private Vector2 m_Velocity;
        /// <summary>
        /// The current velocity of the content.
        /// </summary>
        /// <remarks>
        /// The velocity is defined in units per second.
        /// </remarks>
        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        protected virtual void SetContentAnchoredPosition (Vector2 position) {
            // position.y = m_Content.anchoredPosition.y;
            position.x = m_Content.anchoredPosition.x;
            if (position != m_Content.anchoredPosition) {
                m_Content.anchoredPosition = position;
                // UpdateBounds();
            }
        }

        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform {
            get {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform> ();
                return m_Rect;
            }
        }

        protected Vector2 m_ContentInitializePosition;

        /// <summary>
        /// 绑定上下文 用于lua binding
        /// </summary>
        public object context { get; set; }

        #region  mono

        protected override void Awake () {
            base.Awake ();
            m_ContentInitializePosition = content.anchoredPosition;
            UpdateBounds ();
            InitContentBounds ();
        }

        /// <summary>
        /// Handling for when the content is beging being dragged.
        /// </summary>
        public virtual void OnBeginDrag (PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive ())
                return;

            UpdateBounds ();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle (viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
            m_OnBeginDragChanged.Invoke (Vector2.zero);
        }

        /// <summary>
        /// Handling for when the content is dragged.
        /// </summary>
        public virtual void OnDrag (PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive ())
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle (viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds ();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset (position - m_Content.anchoredPosition);
            m_drag_offset = offset;
            position += offset;
            if (offset.y != 0)
                position.y = position.y - RubberDelta (offset.y, Mathf.Abs (m_ViewBounds.height));

            SetContentAnchoredPosition (position);

            m_OnDragChanged.Invoke (m_drag_offset);

        }
        Vector2 m_drag_offset;
        /// <summary>
        /// Handling for when the content has finished being dragged.
        /// </summary>
        public virtual void OnEndDrag (PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;

            m_OnEndDragChanged.Invoke (m_drag_offset); //判断拖动的位置
        }

        //
        // 摘要:
        //     See: IInitializePotentialDragHandler.OnInitializePotentialDrag.
        //
        // 参数:
        //   eventData:
        public virtual void OnInitializePotentialDrag (PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        protected virtual void LateUpdate () {
            if (!m_Content)
                return;

            UpdateBounds ();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset (Vector2.zero);
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero)) {
                Vector2 position = m_Content.anchoredPosition;
                for (int axis = 1; axis < 2; axis++) {
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (offset[axis] != 0) {
                        float speed = m_Velocity[axis];
                        position[axis] = Mathf.SmoothDamp (m_Content.anchoredPosition[axis], m_Content.anchoredPosition[axis] + offset[axis], ref speed, m_Elasticity, Mathf.Infinity, deltaTime);
                        if (Mathf.Abs (speed) < 1)
                            speed = 0;
                        m_Velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (m_Inertia) {
                        m_Velocity[axis] *= Mathf.Pow (m_DecelerationRate, deltaTime);
                        if (Mathf.Abs (m_Velocity[axis]) < 1)
                            m_Velocity[axis] = 0;
                        position[axis] += m_Velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else {
                        m_Velocity[axis] = 0;
                    }
                }

                SetContentAnchoredPosition (position);
            }

            if (m_Dragging && m_Inertia) {
                Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
                m_Velocity = Vector3.Lerp (m_Velocity, newVelocity, deltaTime * 10);
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition) {
                UpdateScrollbars (offset);
                // m_OnValueChanged.Invoke(normalizedPosition);
                UpdatePrevData ();
            }
            UpdateScrollbarVisibility ();
        }

        protected override void OnEnable () {
            base.OnEnable ();

            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.AddListener (SetVerticalNormalizedPosition);

            // CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            SetDirty ();
        }

        protected override void OnDisable () {
            // CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.RemoveListener (SetVerticalNormalizedPosition);

            m_Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild (rectTransform);
            base.OnDisable ();
        }

        protected void SetDirty () {
            if (!IsActive ())
                return;

            LayoutRebuilder.MarkLayoutForRebuild (rectTransform);
        }

        public void OnPointerClick (PointerEventData eventData) {
            onPointerClick.Invoke ();
        }

        /// <summary>
        /// Helper function to update the previous data fields on a ScrollRect. Call this before you change data in the ScrollRect.
        /// </summary>
        protected void UpdatePrevData () {
            if (m_Content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = m_Content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        private void UpdateScrollbars (Vector2 offset) {

            if (m_VerticalScrollbar) {
                if (m_ContentBounds.size.y > 0)
                    m_VerticalScrollbar.size = Mathf.Clamp01 ((m_ViewBounds.size.y - Mathf.Abs (offset.y)) / m_ContentBounds.size.y);
                else
                    m_VerticalScrollbar.size = 1;

                m_VerticalScrollbar.value = verticalNormalizedPosition;
            }
        }

        void UpdateScrollbarVisibility () {
            UpdateOneScrollbarVisibility (vScrollingNeeded, true, m_VerticalScrollbar);
            // UpdateOneScrollbarVisibility(hScrollingNeeded, m_Horizontal, m_HorizontalScrollbarVisibility, m_HorizontalScrollbar);
        }

        private static void UpdateOneScrollbarVisibility (bool xScrollingNeeded, bool xAxisEnabled, Scrollbar scrollbar) {
            if (scrollbar) {

                {
                    if (scrollbar.gameObject.activeSelf != xScrollingNeeded)
                        scrollbar.gameObject.SetActive (xScrollingNeeded);
                }
            }
        }

        private void SetVerticalNormalizedPosition (float value) { SetNormalizedPosition (value, 1); }

        /// <summary>
        /// Sets the velocity to zero on both axes so the content stops moving.
        /// </summary>
        public virtual void StopMovement () {
            m_Velocity = Vector2.zero;
        }
        #endregion

        #region  tool

        /// <summary>
        /// Calculate the bounds the ScrollRect should be using.
        /// </summary>
        protected void UpdateBounds () {
            Vector2 localp = content.anchoredPosition;
            var cpos = localp - m_ContentInitializePosition; //开始位置
            m_ViewBounds = rectTransform.rect;
            // Debug.LogFormat ("m_ViewBounds {0}", m_ViewBounds);
            m_ViewBounds.height = -m_ViewBounds.height;
            m_ViewBounds.x = cpos.x;
            m_ViewBounds.y = -cpos.y;
            // Debug.LogFormat ("m_ViewBounds1 {0}", m_ViewBounds);

        }

        /// <summary>
        /// Init the bounds of content.
        /// </summary>
        protected void InitContentBounds () {
            m_ContentBounds = viewRect.rect;
            // Debug.LogFormat ("m_ContentBounds {0}", m_ContentBounds);
            m_ContentBounds.height = -Mathf.Abs (m_ContentBounds.height);
            m_ContentBounds.x = m_Content.anchoredPosition.x; //min y
            m_ContentBounds.y = m_Content.anchoredPosition.y; //max y
            // Debug.LogFormat ("m_ContentBounds1 {0}", m_ContentBounds);

        }

        protected virtual void SetNormalizedPosition (float value, int axis) {
            // EnsureLayoutHasRebuilt();
            UpdateBounds ();
            // How much the content is larger than the view.
            float hiddenLength = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
            // Where the position of the lower left corner of the content bounds should be, in the space of the view.
            float contentBoundsMinPosition = m_ViewBounds.x - value * hiddenLength;
            // The new content localPosition, in the space of the view.
            float newLocalPosition = m_Content.localPosition[axis] + contentBoundsMinPosition - m_ContentBounds.x;

            Vector3 localPosition = m_Content.localPosition;
            if (Mathf.Abs (localPosition[axis] - newLocalPosition) > 0.01f) {
                localPosition[axis] = newLocalPosition;
                m_Content.localPosition = localPosition;
                m_Velocity[axis] = 0;
                // UpdateBounds();
            }
        }
        private static float RubberDelta (float overStretching, float viewSize) {
            var re = (1 - (1 / ((Mathf.Abs (overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign (overStretching);
            return re;
        }
        private Vector2 CalculateOffset (Vector2 delta) {
            return InternalCalculateOffset (ref m_ViewBounds, ref m_ContentBounds, ref delta);
        }

        internal static Vector2 InternalCalculateOffset (ref Rect viewBounds, ref Rect contentBounds, ref Vector2 delta) {
            // return Vector2.zero;
            Vector2 offset = Vector2.zero;
            Vector2 min = contentBounds.min; //bottom
            Vector2 max = contentBounds.max; //top

            min.y += delta.y;
            max.y += delta.y;
            if (min.y < viewBounds.yMin || viewBounds.height <= contentBounds.height) // if (offset.y < max.y) offset.y = viewBounds.yMin - min.y; //如果高度不够以顶部为基准
            {
                offset.y = viewBounds.yMin - min.y;
            } else if (max.y > viewBounds.yMax) {
                offset.y = viewBounds.yMax - max.y;
            }

            return offset;
        }

        #endregion
    }

}