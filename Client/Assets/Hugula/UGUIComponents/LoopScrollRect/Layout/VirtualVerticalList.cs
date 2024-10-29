using System.Collections;
using System.Collections.Generic;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.UIComponents
{

    public class VirtualVerticalList
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

        public Vector2 GetCellSize(int index)
        {
            if (index >= m_CellPos.Count)
                return Vector2.zero;
            else
            {
                if (m_DirtyIndex <= index)
                    CalcDirty(m_DirtyIndex, index);

                return m_CellPos[index];
            }
        }

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
                var last = i - 1; //判断上一条
                var lastCell = m_CellPos[last];
                var lastyItemY = lastCell.y + verticalPadding;

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