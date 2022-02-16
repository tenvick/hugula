using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.UIComponents {
    public class NotifyLayoutElement:LayoutElement,INotifyLayoutElement {

        LoopItem m_LoopItem;
        IScrollLayoutChange m_LoopScrollBase;
        public void Init(LoopItem loopItem, IScrollLayoutChange loopScrollBase)
        {
            this.m_LoopItem = loopItem;
            this.m_LoopScrollBase = loopScrollBase;
        }

        public void OnDirty()
        {
            m_LoopScrollBase?.OnItemLayoutChange(m_LoopItem.index);
        }

    }
}