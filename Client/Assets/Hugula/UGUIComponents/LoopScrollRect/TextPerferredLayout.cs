using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextPerferredLayout : UIBehaviour, ILayoutElement
{

    public float offsetHeight = 100f;
    public float layoutMinHeight = 145;
    #region  interface ILayoutElement
    //
    // 摘要:
    //     The minimum width this layout element may be allocated.
    public float minWidth { get; }
    //
    // 摘要:
    //     The preferred width this layout element should be allocated if there is sufficient
    //     space.
    public float preferredWidth { get; }
    //
    // 摘要:
    //     The extra relative width this layout element should be allocated if there is
    //     additional available space.
    public float flexibleWidth { get; }
    //
    // 摘要:
    //     The minimum height this layout element may be allocated.
    public float minHeight { get; }
    //
    // 摘要:
    //     The preferred height this layout element should be allocated if there is sufficient
    //     space.
    public float preferredHeight { get { return m_Text.preferredHeight; } }
    //
    // 摘要:
    //     The extra relative height this layout element should be allocated if there is
    //     additional available space.
    public float flexibleHeight { get { return m_Text.preferredWidth; } }
    //
    // 摘要:
    //     The layout priority of this component.
    public int layoutPriority { get; }

    //
    // 摘要:
    //     The minWidth, preferredWidth, and flexibleWidth values may be calculated in this
    //     callback.
    public void CalculateLayoutInputHorizontal()
    {

    }
    //
    // 摘要:
    //     The minHeight, preferredHeight, and flexibleHeight values may be calculated in
    //     this callback.
    public void CalculateLayoutInputVertical()
    {
        // _textDirtyVerticesCallback();
    }

    #endregion

    protected override void OnRectTransformDimensionsChange()
    {
        // Debug.LogFormat("OnRectTransformDimensionsChange {0},frameCount = {1}", this,Time.frameCount);
         float fTextPreferredHeight = m_Text.preferredHeight;
        // Debug.LogFormat("{0},m_kInlieText.perferredHeight = {1},height={2},frame={3}", m_LayoutElement, m_Text.preferredHeight, m_Text.rectTransform.rect, Time.frameCount);

    }

    protected override void OnCanvasGroupChanged()
    {
        // Debug.LogFormat("OnCanvasGroupChanged {0},frameCount = {1}", this,Time.frameCount);
    }

    #region ILayoutGroup
    //
    // 摘要:
    //     Callback invoked by the auto layout system which handles horizontal aspects of
    //     the layout.
    public void SetLayoutHorizontal()
    {
        // Debug.LogFormat("SetLayoutHorizontal {0},frame={1}", this, Time.frameCount);
    }

    //
    // 摘要:
    //     Callback invoked by the auto layout system which handles vertical aspects of
    //     the layout.
    public void SetLayoutVertical()
    {
        // Debug.LogFormat("SetLayoutVertical {0},frame={1}", this, Time.frameCount);
    }


    #endregion

    public Text m_Text;
    public LayoutElement m_LayoutElement;

    private void OnEnable()
    {
        m_Text.RegisterDirtyVerticesCallback(new UnityAction(_textDirtyVerticesCallback));
    }

    private void OnDisable()
    {
        m_Text.UnregisterDirtyVerticesCallback(new UnityAction(_textDirtyVerticesCallback));
    }

    private void _textDirtyVerticesCallback()
    {
        if (m_LayoutElement == null)
        {
            m_LayoutElement = GetComponentInParent<LayoutElement>();
        }

        float fTextPreferredHeight = m_Text.preferredHeight;
        // Debug.LogFormat("{0},m_kInlieText.perferredHeight = {1},height={2},frame={3}", m_LayoutElement, m_Text.preferredHeight, m_Text.rectTransform.rect, Time.frameCount);

        if (m_LayoutElement != null)
        {
            float fHeight = fTextPreferredHeight + offsetHeight;
            fHeight = Mathf.Max(fHeight, layoutMinHeight);
            m_LayoutElement.preferredHeight = fHeight;
        }

    }
}
