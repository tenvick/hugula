// #define TEST
using Hugula.Databinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hugula.UIComponents
{
    [XLua.LuaCallCSharp]
    public class LoopScrollRect : LoopScrollBase
    {
        /// <summary>
        /// 当在顶部或者底部释放的时候触发。
        /// </summary>
        public ICommand droppedCommand { get; set; }

        /// <summary>
        /// 当在顶部或者底部释放的时候触发。
        /// </summary>
        public Action<Vector2> onDropped { get; set; }

        protected override void ScrollLoopItem()
        {
            bool tween = isTweening;
            if (columns == 0 && (IsHorizontalScroll || tween))
            {
                m_HeadDataIndex = Mathf.FloorToInt(-m_ViewPointRect.x / (itemSize.x + this.halfPadding)); //头
                if (m_HeadDataIndex < 0) m_HeadDataIndex = 0;
                m_FootDataIndex = m_HeadDataIndex + pageSize > dataLength ? dataLength : m_HeadDataIndex + pageSize;
                for (int i = m_HeadDataIndex; i < m_FootDataIndex; i++)
                {
                    var item = GetLoopItemAt(i);
                    if (item.index != i)
                    {
                        RenderItem(item, i);
                    }
                }
            }
            else if (columns > 0 && (IsVerticalScroll || tween))
            {
                int cloumnIndex = Mathf.FloorToInt(-m_ViewPointRect.y / (itemSize.y + this.halfPadding));
                m_HeadDataIndex = Mathf.CeilToInt((float)(cloumnIndex * this.columns) / (float)this.columns) * columns; //
                if (m_HeadDataIndex < 0) m_HeadDataIndex = 0;
                m_FootDataIndex = m_HeadDataIndex + pageSize > dataLength ? dataLength : m_HeadDataIndex + pageSize;
                if (m_FootDataIndex > dataLength) m_FootDataIndex = dataLength;
                for (int i = m_HeadDataIndex; i < m_FootDataIndex; i++)
                {
                    var item = GetLoopItemAt(i);
                    if (item.index != i)
                    {
                        RenderItem(item, i);
                    }
                }
            }
        }

        protected override void LayOut(LoopItem loopItem, int index) //
        {

            RectTransform rectTran = loopItem.transform;
            // int index = loopItem.index;
            var rect = rectTran.rect;
            rect.height = -Mathf.Abs(rect.height);
            Vector2 pos = Vector2.zero;
            if (this.columns == 0) //单行
            {
                pos.x = (rect.width + this.halfPadding) * index + this.halfPadding; // + rect.width * .5f;
                pos.y = -halfPadding;
            }
            else if (columns == 1) //单列 需要宽度适配
            {
                pos.x = halfPadding;
                pos.y = (rect.height - this.halfPadding) * index - this.halfPadding; // rect.height * .5f ;
            }
            else // 多行
            {
                int x = index % columns;
                pos.x = (rect.width + this.halfPadding) * x + this.halfPadding; // + rect.width * .5f;
                int y = index / columns;
                pos.y = (rect.height - this.halfPadding) * y - this.halfPadding; // rect.height * .5f ;
            }
            rect.position = pos;
            pos = pos + m_ContentLocalStart; //开始位置

            rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, rectTran.rect.width);
            rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -pos.y, rectTran.rect.height);
        }

        protected Vector2 EasingLayout(LoopItem loopItem, int index, float time)
        {
            RectTransform rectTran = loopItem.transform;
            var rect = rectTran.rect;
            rect.height = -Mathf.Abs(rect.height);
            Vector2 pos = Vector2.zero;
            if (this.columns == 0) //单行
            {
                pos.x = (rect.width + this.halfPadding) * index + this.halfPadding; // + rect.width * .5f;
                pos.y = -halfPadding;
            }
            else if (columns == 1) //单列 需要宽度适配
            {
                pos.x = halfPadding;
                pos.y = (rect.height - this.halfPadding) * index - this.halfPadding; // rect.height * .5f ;
            }
            else // 多行
            {
                int x = index % columns;
                pos.x = (rect.width + this.halfPadding) * x + this.halfPadding; // + rect.width * .5f;
                int y = index / columns;
                pos.y = (rect.height - this.halfPadding) * y - this.halfPadding; // rect.height * .5f ;
            }
            pos = pos + m_ContentLocalStart; //开始位置
            var begin = rectTran.anchoredPosition;
            var pos1 = Vector2.Lerp(begin, pos, time);

            return pos1;
        }


        protected override void CalcBounds()
        {

            if (content != null)
            {
                var rect = content.rect;
                Vector2 delt = new Vector2(rect.width, rect.height);
                if (columns <= 0) //只有一行，为了高度适配不设置sieDelta.y
                {
                    delt.x = dataLength * (itemSize.x + this.halfPadding) + this.halfPadding + Mathf.Abs(m_ContentLocalStart.x);
                }
                else if (columns == 1) //只有一列的时候为了 宽度适配不设置sieDelta.x
                {
                    delt.y = (itemSize.y + this.halfPadding) * dataLength + this.halfPadding + Mathf.Abs(m_ContentLocalStart.y);
                }
                else
                {
                    delt.x = columns * (itemSize.x + this.halfPadding) + this.halfPadding + Mathf.Abs(m_ContentLocalStart.x);
                    int y = (int)Mathf.Ceil((float)dataLength / (float)columns);
                    delt.y = (itemSize.y + this.halfPadding) * y + this.halfPadding + Mathf.Abs(m_ContentLocalStart.y);
                }

                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, delt.x);
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, delt.y);
            }
        }

        public override void RemoveAt(int index, int count = 1)
        {
            base.RemoveAt(index, count);

            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            StartCoroutine(DeleteTweenMoveToPos());
        }

        [SerializeField]
        float m_ScrollTime = 0.4f;
        /// <summary>
        /// 滚动时间</br>
        /// </summary>
        public float scrollTime { get { return m_ScrollTime; } set { m_ScrollTime = value; } }

        [Tooltip("每次滚动移动的单位距离，需要取消Inertia选项")]
        [SerializeField] int m_ScrollDataSize = 0;
        public int scrollDataSize
        {
            get { return this.m_ScrollDataSize; }
            set
            {
                m_ScrollDataSize = value;
            }
        }

        private int m_ScrollToIndex = 0;
        ///<summary>
        /// 通过属性的方式指定滚动到制定索引 
        /// 为了数据绑定
        ///</summary>
        public int setScrollIndex
        {
            get { return this.m_ScrollToIndex; }
            set
            {
                ScrollToIndex(value);
            }
        }

        public System.Action<object, int, int> onScrollIndexChanged;

        ///<summary>
        /// 滚动到指定索引
        ///</summary>
        public void ScrollToIndex(int idx)
        {

            if (isTweening) return;
            if (idx < 0) idx = 0;
            if (idx >= dataLength) idx = dataLength - 1;
            if (idx < 0) return;
            var oldIdx = m_ScrollToIndex;
            m_ScrollToIndex = idx;

            var cpos = Vector2.zero; //开始位置
            Vector2 curr = content.anchoredPosition;
            var contentSize = content.rect;
            var viewPortSize = viewRect.rect;

            int columns = base.columns;
            if (columns == 0) //x轴 
            {
                var minX = Mathf.Min(0, viewPortSize.width - contentSize.width);
                cpos.x = Mathf.Max(minX, -(itemSize.x + this.halfPadding) * idx - m_ContentLocalStart.x); //开始位置
            }
            else
            {
                var maxY = Mathf.Max(0, contentSize.height - viewPortSize.height);
                int row = idx / columns;
                cpos.y = Mathf.Min(maxY, (itemSize.y + this.halfPadding) * row + m_ContentLocalStart.y); //开始位置
            }

            if (gameObject.activeInHierarchy)
            {
                m_Coroutine = StartCoroutine(TweenMoveToPos(curr, cpos, scrollTime));
            }

            if (onScrollIndexChanged != null)
                onScrollIndexChanged(this.parameter, m_ScrollToIndex, oldIdx);

        }

        Vector2 tweenDir;
        //在使用scrollto的时候需要控制drag
        protected bool isTweening
        {
            get
            {
                return !tweenDir.Equals(Vector2.zero);
            }
        }

        protected bool IsVerticalScroll
        {
            get
            {
                return velocity.y != 0 || scrollBarDragging || m_IsOnDrag;
            }

        }

        protected bool IsHorizontalScroll
        {
            get
            {
                return velocity.x != 0 || scrollBarDragging || m_IsOnDrag;
            }
        }
        private WaitForEndOfFrame waitForend = new WaitForEndOfFrame();
        protected IEnumerator TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
        {
            float passedTime = 0f;
            tweenDir = (v2Pos - pos).normalized;

            while (!tweenDir.Equals(Vector2.zero))
            {
                yield return waitForend;
                passedTime += Time.deltaTime;
                Vector2 vCur;
                if (passedTime >= delay)
                {
                    vCur = v2Pos;
                    content.anchoredPosition = v2Pos;
                    UpdateViewPointBounds();
                    ScrollLoopItem();
                    tweenDir = Vector2.zero;
                    if (m_Coroutine != null)
                    {
                        StopCoroutine(m_Coroutine);
                        m_Coroutine = null;
                    }

                }
                else
                {
                    vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
                }
                content.anchoredPosition = vCur;
            }

        }
        protected Coroutine m_Coroutine = null;

        protected IEnumerator DeleteTweenMoveToPos()
        {
            float t = removeEasing?0:1;
            while (true)
            {
                t += Time.deltaTime;
                for (int i = 0; i < m_Pages.Count; i++)
                {
                    var item = m_Pages[i];
                    if (item.onlyPosDirty && item.index >= 0)
                    {
                        var pos = EasingLayout(item, item.index, t);//得到目标点位置
                        RectTransform rectTran = item.transform;
                        rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, rectTran.rect.width);
                        rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -pos.y, rectTran.rect.height);
                        if (t >= 1)
                        {
                            item.onlyPosDirty = false;
                        }
                    }
                }
                if (t >= 1) yield break;
                yield return null;
            }
        }

        #region  drag
        private int m_CurrHeadIdx;
        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_CurrHeadIdx = m_HeadDataIndex;
            base.OnBeginDrag(eventData);

        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            if (m_ScrollDataSize > 0 && columns == 0)
            {
                var idx = m_CurrHeadIdx;//// Mathf.FloorToInt(-m_ViewPointRect.x / (itemSize.x + this.halfPadding)); //头
                if (m_DragVelocity.x > 0)
                {
                    idx = idx - m_ScrollDataSize;
                }
                else if (m_DragVelocity.x < 0)
                {
                    idx = idx + m_ScrollDataSize;
                }

                // Debug.LogFormat("drag end {0} ,idx = {1}", m_DragVelocity, idx);
                // ScrollToIndex(idx);
                setScrollIndex = idx;
            }

            if (onDropped != null) onDropped(content.anchoredPosition);
            if (droppedCommand != null && droppedCommand.CanExecute(eventData.position))
                droppedCommand.Execute(eventData.position);
            //滚动到
        }
        #endregion
    }

    public static class RectTransformExtension
    {
        /// <summary>
        /// 判断宽度适配还是高度适配 0 宽度x水平  1 高度y竖直
        /// </summary>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.</param>
        public static bool IsMatchOffset(this RectTransform transform, RectTransform.Axis axis)
        {
            var a = (int)axis;
            return transform.offsetMin[a] != transform.offsetMax[a];
        }
    }
}