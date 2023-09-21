using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Hugula.UIComponents;

///
public class RectTransformDimensionsChangeLayout : UIBehaviour
{
    [SerializeField]
     LayoutElement m_LayoutElement;
    public float offsetHeight = 100f;
    public float layoutMinHeight = 10f;

    private RectTransform m_Transform;
     RectTransform mtransform
     {
         get
         {
             if(m_Transform==null)
             {
                 m_Transform = GetComponent<RectTransform>();
             }

             return m_Transform;
         }
     }

    protected override void OnRectTransformDimensionsChange()
    {
        NotifyLayout(mtransform.rect.height);
    }

    void OnEnable()
    {
        NotifyLayout(mtransform.rect.height);
    }

    void OnDisable()
    {
        NotifyLayout(0);
    }

    void NotifyLayout(float fTextPreferredHeight)
    {
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
