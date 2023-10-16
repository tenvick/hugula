using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Hugula.UIComponents;
#if USE_TMPro
using TMPro;
#endif
public class TextMeshProUGUIPerferredLayout : UIBehaviour, ILayoutElement, ILayoutIgnorer
{

    #region  ILayoutElement
        [Tooltip("文本自动适应的最小宽度")]
        [SerializeField] private float m_MinWidth = -1;
        [Tooltip("文本自动适应的最小高度")]
        [SerializeField] private float m_MinHeight = -1;
        [SerializeField] private float m_PreferredWidth = -1;
        [SerializeField] private float m_PreferredHeight = -1;
        [Tooltip("文本自动适应的最大宽度")]
        [SerializeField] private float m_FlexibleWidth = -1;
        [Tooltip("文本自动适应的最大高度")]
        [SerializeField] private float m_FlexibleHeight = -1;
        [SerializeField] private int m_LayoutPriority = 1;

        public virtual void CalculateLayoutInputHorizontal() {}
        public virtual void CalculateLayoutInputVertical() {}

        /// <summary>
        /// The minimum width this layout element may be allocated.
        /// </summary>
        public virtual float minWidth { get { return m_MinWidth; } set { if (SetStruct(ref m_MinWidth, value)) SetDirty(); } }

        /// <summary>
        /// The minimum height this layout element may be allocated.
        /// </summary>
        public virtual float minHeight { get { return m_MinHeight; } set { if (SetStruct(ref m_MinHeight, value)) SetDirty(); } }

        /// <summary>
        /// The preferred width this layout element should be allocated if there is sufficient space. The preferredWidth can be set to -1 to remove the size.
        /// </summary>
        public virtual float preferredWidth { get { return m_PreferredWidth; } set { if (SetStruct(ref m_PreferredWidth, value)) SetDirty(); } }
     
        public virtual float preferredHeight { get { return m_PreferredHeight; } set { if (SetStruct(ref m_PreferredHeight, value)) SetDirty(); } }

        /// <summary>
        /// The extra relative width this layout element should be allocated if there is additional available space.
        /// </summary>
        public virtual float flexibleWidth { get { return m_FlexibleWidth; } set { if (SetStruct(ref m_FlexibleWidth, value)) SetDirty(); } }

        /// <summary>
        /// The extra relative height this layout element should be allocated if there is additional available space.
        /// </summary>
        public virtual float flexibleHeight { get { return m_FlexibleHeight; } set { if (SetStruct(ref m_FlexibleHeight, value)) SetDirty(); } }

        /// <summary>
        /// The Priority of layout this element has.
        /// </summary>
        public virtual int layoutPriority { get { return m_LayoutPriority; } set { if (SetStruct(ref m_LayoutPriority, value)) SetDirty(); } }


        protected override void OnTransformParentChanged()
        {
            SetDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }

        protected override void OnBeforeTransformParentChanged()
        {
            SetDirty();
        }
    #endregion

    #region  ILayoutIgnorer
        [SerializeField] private bool m_IgnoreLayout = false;
        public virtual bool ignoreLayout { get { return m_IgnoreLayout; } set { if (SetStruct(ref m_IgnoreLayout, value)) SetDirty(); } }
    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

    #endif

        static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }
        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    #endregion

    public float offsetHeight = 100f;
    public float layoutMinHeight = 145;

    [SerializeField] 
    ContentSizeFitter m_SelfFitter;

    public TextMeshProUGUI m_Text;
    public LayoutElement m_LayoutElement;

    protected override void  Awake()
    {
        base.Awake();
        textDirtyVerticesCallback = new UnityAction(_textDirtyVerticesCallback);
    }

    UnityAction textDirtyVerticesCallback;

    protected override void OnEnable()
    {
        m_Text.RegisterDirtyVerticesCallback(textDirtyVerticesCallback);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        m_Text.UnregisterDirtyVerticesCallback(textDirtyVerticesCallback);
        base.OnDisable();
    }

    private void _textDirtyVerticesCallback()
    {
            float curTextWidth = m_Text.preferredWidth;
            var textRect = m_Text.rectTransform;
            if(m_SelfFitter!=null)
            {
                this.m_PreferredWidth = m_Text.preferredWidth;

                if (curTextWidth > this.flexibleWidth) //flexibleWidth 表示最大宽度
                {
                    this.m_PreferredWidth = flexibleWidth;
                    m_SelfFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
                else
                {
                    m_SelfFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }

                float curTextHeight = m_Text.preferredHeight;
                this.m_PreferredHeight = m_Text.preferredHeight;

                if (curTextHeight > flexibleHeight) //flexibleHeight 表示最大高度
                {
                    this.m_PreferredHeight = flexibleHeight;
                    m_SelfFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
                else
                    m_SelfFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
          

        float fTextPreferredHeight = m_Text.preferredHeight;

        if (m_LayoutElement != null)
        {
            float fHeight = fTextPreferredHeight + offsetHeight;
            fHeight = Mathf.Max(fHeight, layoutMinHeight);
            if(Mathf.Abs(m_LayoutElement.preferredHeight - fHeight)>0.01f  )
            {
                m_LayoutElement.preferredHeight = fHeight;
                if(m_LayoutElement is NotifyLayoutElement)
                    ((NotifyLayoutElement)m_LayoutElement).OnDirty();
            }
        }

    }
}
