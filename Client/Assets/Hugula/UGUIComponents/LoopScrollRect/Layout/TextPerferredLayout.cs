using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Hugula.UIComponents;

public class TextPerferredLayout : UIBehaviour
{
    public float offsetHeight = 100f;
    public float layoutMinHeight = 145;

    public Text m_Text;
    public LayoutElement m_LayoutElement;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Text.RegisterDirtyVerticesCallback(new UnityAction(_textDirtyVerticesCallback));
    }

    protected override void OnDisable()
    {
        m_Text.UnregisterDirtyVerticesCallback(new UnityAction(_textDirtyVerticesCallback));
        base.OnDisable();
    }

    private void _textDirtyVerticesCallback()
    {
        // if (m_LayoutElement == null)
        // {
        //     m_LayoutElement = GetComponentInParent<LayoutElement>();
        // }

        float fTextPreferredHeight = m_Text.preferredHeight;
        // Debug.LogFormat("{0},m_kInlieText.perferredHeight = {1},height={2},frame={3}", m_LayoutElement, m_Text.preferredHeight, m_Text.rectTransform.rect, Time.frameCount);
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
