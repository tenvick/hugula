using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.UIComponents
{

    internal class VirtualVerticalList
    {
        internal float paddingTop = 0;
        internal float paddingBottom = 0;

        private float m_VerticalPadding = float.NaN;
        protected float verticalPadding
        {
            get
            {
                if (m_VerticalPadding.Equals(float.NaN))
                    m_VerticalPadding = paddingTop + paddingBottom;
                return m_VerticalPadding;
            }
        }

        private int m_DirtyIndex = int.MaxValue;

        private List<float> m_CellHeight = new List<float>();
        private List<Vector2> m_CellPos = new List<Vector2>();

        /// <summary>
        /// 获取当前索引的位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool GetCellSize(int index,out Vector2 pos)
        {
            if (index < m_CellPos.Count && index>= 0)
            {
                if (m_DirtyIndex <= index)
                    CalcDirty(m_DirtyIndex, index);

                pos = m_CellPos[index];
                return true;
            }
            else if(index >= m_CellPos.Count)
            {
                if (m_CellPos.Count > 0)
                {
                    pos = m_CellPos[m_CellPos.Count-1];
                }
                else
                {
                    pos = Vector2.zero;
                }
                return false;
            }
            else 
            {
                pos = Vector2.zero;
                return false;
            }
        }

        /// <summary>
        /// 元素数据从index处删除了count个元素
        /// 从 maxIdx处向上移动直到index获取当前索引的位置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="endIdx"></param>
        /// <param name="count"></param>
        public void RemoveAt(int index, int count = 1)
        {
            if (index >= m_CellHeight.Count || index < 0) return;

            int reEndIdx = index + count - 1;
            if (reEndIdx >= m_CellHeight.Count) reEndIdx = m_CellHeight.Count - 1;
            int beginIdx = index;

            for (int i = reEndIdx; i >= beginIdx; i--)
            {
                m_CellPos.RemoveAt(i);
                m_CellHeight.RemoveAt(i);
            }

            if (reEndIdx < m_CellHeight.Count && beginIdx < m_CellHeight.Count)
                CalcDirty(reEndIdx, beginIdx);

        }

        /// <summary>
        /// 设置当前索引大小
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        public void SetCellSize(int index, float size)
        {
            for (int i = m_CellHeight.Count; i <= index; i++)
            {
                m_CellHeight.Add(0);
                m_CellPos.Add(Vector2.zero);
            }

            if (m_CellHeight[index] != size)
            {
                m_CellHeight[index] = size;
                if (index > 0)
                {
                    var last = index - 1; //判断上一条是不是脏数据
                    if (m_DirtyIndex <= last) //如果上一条脏了
                    {
                        CalcDirty(m_DirtyIndex, last);
                    }
                    var lastCell = m_CellPos[last];
                    var lasty = lastCell.y + verticalPadding;
                    m_CellPos[index] = new Vector2(lasty, lasty + m_CellHeight[index] + verticalPadding);
                    m_DirtyIndex = index + 1;
                }
                else if (index == 0)
                {
                    m_CellPos[index] = new Vector2(verticalPadding, verticalPadding + m_CellHeight[index] + verticalPadding);
                    m_DirtyIndex = index + 1; //下一条数据标记为脏
                }
            }

        }

        private void CalcDirty(int dirtyIndex, int indx)
        {
            float lasty = 0;
            for (int i = dirtyIndex; i <= indx; i++)
            {
                var lastyItemY = verticalPadding;
                var last = i - 1; //判断上一条
                if (last >= 0)
                {
                    var lastCell = m_CellPos[last];
                    lastyItemY = lastCell.y + verticalPadding;
                }

                m_CellPos[i] = new Vector2(lastyItemY, lastyItemY + m_CellHeight[i] + verticalPadding);
                lasty += m_CellPos[i].y;
            }
            m_DirtyIndex = indx + 1;
        }

        // private void CalcBoundsFrom(int idx)
        // {
        //     float lasty = 0;
        //     // for(int i=idx;i<=indx;i++)
        //     // {
        //     //     m_CellPos[i] = new Vector2(lasty+halfPadding, m_CellHeight[i]+halfPadding);
        //     //     lasty += m_CellPos[i].y;
        //     // }
        // }

    }

}