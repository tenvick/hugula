using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// [ExecuteInEditMode]
namespace Hugula.UIComponents
{
    [XLua.LuaCallCSharp]
    public abstract class LoopScrollBase : ScrollRect, ILoopSelect //: UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        [SerializeField]
        int m_Columns = 0; //显示的列数

        int m_RealColumns = 0;
        /// <summary>
        /// 显示列数 排版方式 </br>
        /// Columns == 0 显示一行 </br>
        /// Columns == 1 显示一列 </br>
        /// Columns = n n>1  按照格子方式显示n列的数据 </br>
        /// </summary>
        public int columns { get { return m_RealColumns; } set { m_RealColumns = value; } } //显示列数 

        int m_PageSize = 1;
        // [SerializeField]

        /// <summary>
        /// 项目池的大小 </br>
        /// 自动计算
        /// </summary>
        public int pageSize
        {
            get
            {
                return m_PageSize;
            }
        }

        [SerializeField]
        Vector2 m_ItemSize = Vector2.zero;
        Vector2 m_RuntimeItemSize = Vector2.zero;

        /// <summary>
        /// ItemClone的默认size 当不为0的时候强制使用当前size
        /// </summary>
        public Vector2 itemSize { get { return m_RuntimeItemSize; } set { m_RuntimeItemSize = value; } } //默认size

        [SerializeField]
        float m_Padding = 0;
        /// <summary>
        /// 间距</br>
        /// </summary>
        public float padding { get { return m_Padding; } set { m_Padding = value; m_HalfPadding = float.NaN; } } //间隔

        [SerializeField]
        int m_RenderPerFrames = 0;
        /// <summary>
        /// 次渲染的项目数量</br>
        /// </summary>
        public int renderPerFrames { get { return m_RenderPerFrames; } set { m_RenderPerFrames = value; } } //每次渲染item数

        [SerializeField]
        Component m_ItemSource;
        /// <summary>
        /// clone的原始项</br>
        /// </summary>
        public Component itemSource { get { return m_ItemSource; } set { m_ItemSource = value; } } //clone的项目

        [Tooltip("默认选中索引")]
        [SerializeField]
        int m_SelectedIndex = -1;
        /// <summary>
        /// 通过OnSelected方法 选中项目的索引</br>
        /// </summary>
        public int selectedIndex
        {
            get { return m_SelectedIndex; }
            set
            {
                m_SelectedIndex = value;
                TriggerStyleBySelectedIndex(value);
            }
        }

        int m_LastSelectedIndex = -1;
        /// <summary>
        /// 通过OnSelected方法 选中项目的索引</br>
        /// </summary>
        public int lastSelectedIndex { get { return m_LastSelectedIndex; } }

        ILoopSelectStyle m_SelecteStyle;
        /// <summary>
        /// 当item被clone时候调用
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.OnInstantiateFunction = (object o, int idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             var btn = gobj.GetComponentInChildren<Button>();
        ///             btn.OnClick.AddListener(() =>
        ///              {
        ///                    print("Click "+mData.ToString());
        ///               });
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int> onInstantiated { get; set; } //渲染

        /// <summary>
        /// 此函数会接管 默认的item Clone
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///     public Transform text;
        ///     public Transform button;
        ///     void Awake()
        ///     {
        ///                                                         
        ///         myScrollRect.OnGetItem = (int idx/*当前索引 */,Component oldComp/*上一个持有对象 */,int oldIdx/*旧的索引 */,RectTransform parent/*挂接父对象 */) =>
        ///         {
        ///             GameObject gobj = null;
        ///             if (idx % 2 == 0 )
        ///                 gobj = GameObject.Instantiate(text);
        ///             else
        ///                 gobj = GameObject.Instantiate(button);
        ///             gobj.name = "item" + idx.ToString();
        ///             var btn = gobj.GetComponentInChildren<Button>();
        ///             return btn
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Func<object, int, Component, int, RectTransform, Component> onGetItem { get; set; } //

        /// <summary>
        /// 填充数据显示项目
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.onItemRender = (object o, int idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             Text txt = gobj.GetComponentInChildren<Text>();
        ///             string test = testData[idx];
        ///             txt.text = test;
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int> onItemRender { get; set; } //渲染

        /// <summary>
        /// 选中某个项
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.onSelected = (object o, int idx,int last idx) => //o是ItemSource(Clone) ,idx是数据索引
        ///         {
        ///             var gobj = (Transform)o;
        ///             gobj.name = "item" + idx.ToString();
        ///             Text txt = gobj.GetComponentInChildren<Text>();
        ///             string test = testData[idx];
        ///             txt.text = test;
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<object, object, int, int> onSelected { get; set; }

        /// <summary>
        /// 选中命令
        /// </summary>
        /// <example>
        /// <code>
        public ICommand itemCommand { get; set; }

        public object itemParameter { get; set; }

        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        /// <summary>
        /// 内容的最大范围
        /// </summary>
        protected Rect m_ContentRect = new Rect();
        /// <summary>
        /// 可视范围
        /// </summary>
        protected Rect m_ViewPointRect = new Rect();

        protected Vector2 m_ContentLocalStart;

        protected override void Awake()
        {
            base.Awake();
            m_HalfPadding = float.NaN;
            //m_ContentLocalStart = content.anchoredPosition;
            m_LastSelectedIndex = m_SelectedIndex;
            m_HeadDataIndex = 0;
            m_FootDataIndex = 0;
            CalcPageSize();
            // if (m_RenderPerFrames == 0) m_RenderPerFrames = pageSize;
        }

        // float m_Renderframs = 0;
        // protected void Update()
        // {
        //     UpdateViewPointBounds();
        //     // ScrollLoopItem();
        // }

        protected override void LateUpdate()
        {
            UpdateViewPointBounds();
            ScrollLoopItem();

            if (renderQueue.Count > 0)
            {
                // if (m_RenderPerFrames < columns) m_RenderPerFrames = columns;
                for (int i = 0; i < this.m_PageSize; i++)
                {
                    QueueToRender();
                }
            }

            base.LateUpdate(); //... CanvasUpdateRegistry.PerformUpdate() 75.23 ms

        }

        protected override void OnDestroy()
        {
            onItemRender = null;
            onInstantiated = null;
            content = null;
            m_SelecteStyle = null;

            foreach (var loopItem in m_Pages)
            {
                if (loopItem.item != null)
                {
                    GameObject.Destroy(loopItem.item.gameObject);
                }
                loopItem.item = null;
            }
            m_Pages.Clear();
            base.OnDestroy();
        }

        /// <summary>
        /// >Set the horizontal or vertical scroll position as a value between 0 and 1, with 0 being at the left or at the bottom.
        /// </summary>
        /// <param name="value">The position to set, between 0 and 1.</param>
        /// <param name="axis">The axis to set: 0 for horizontal, 1 for vertical.</param>
        protected override void SetNormalizedPosition(float value, int axis)
        {
            base.SetNormalizedPosition(value, axis);
            m_ScrollBarFrame = Time.frameCount;
        }

        private int m_ScrollBarFrame;
        protected bool scrollBarDragging
        {
            get
            {
                return m_ScrollBarFrame == Time.frameCount;
            }
        }

        #region  数据操作相关
        /// <summary>
        /// 对数据源的引用
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.sourceData = testData; //方便获取数据源
        ///     }
        /// }
        /// </code>
        /// </example>
        public object sourceData; //原始数据引用

        /// <summary>
        /// 所有回调函数的第一个自定义参数
        /// </summary>
        public object parameter { get; set; }

        // /// <summary>
        // /// 绑定上下文 用于lua binding
        // /// </summary>
        // public object context { get; set; }

        private int m_DataLength;

        /// <summary>
        /// 设置数据长度
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         myScrollRect.DataLength = testData.Count;
        ///     }
        /// }
        /// </code>
        /// </example>
        public int dataLength
        {
            get
            {
                return m_DataLength;
            }
            set
            {
                m_DataLength = value;
                ClearItems();
                CalcBounds();
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //向位置0插入一条数据,后刷新显示
        ///         myScrollRect.InsertAt(0);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void InsertAt(int index, int count = 1)
        {
            m_DataLength += count; //长度+1
            // Debug.LogFormat("insert index={0} real idx = {1},m_BeginRenderDataIndex={2},m_EndRenderDataIndex={3}", index, re, m_BeginRenderDataIndex, m_EndRenderDataIndex);
            CalcBounds();
            UpdateBegin(index, m_PageSize);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //删除位置0的数据,会触发OnRemoveFunction代理方法
        ///         myScrollRect.RemoveAt(0); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void RemoveAt(int index, int count = 1)
        {
            m_DataLength -= count;
            //刷新数据
            UpdateBegin(index, m_PageSize);
            CalcBounds();
        }

        /// <summary>
        /// 立即刷新索引处的项目
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //如果索引0在显示范围内就立马刷新
        ///         myScrollRect.UpdateAt(0); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void UpdateAt(int idx)
        {

            if (idx >= 0 && idx < dataLength)
            {
                var item = GetLoopItemAt(idx);
                if (item.index == idx)
                    RenderItem(item, idx);
            }

        }

        /// <summary>
        /// 从指定位置开始刷新直到显示范围尾
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //从索引3位置到PageSize结束的所有项目。
        ///         myScrollRect.UpdateBegin(3); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void UpdateBegin(int idx, int count = 0)
        {
            int min = int.MaxValue, max = int.MinValue;
            foreach (var item1 in m_Pages)
            {
                if (item1.index != -1)
                    min = Math.Min(item1.index, min);
                max = Math.Max(item1.index, max);
            }
            if (count == 0) count = m_PageSize;
            if (min == int.MaxValue) min = 0;
            if (idx < min) idx = min;
            int end_idx = idx + count;
            int movIdx = 0;
            if (columns != 0) movIdx = min % columns; //如果顶部没有对齐。
            int max_idx = min + m_PageSize - 1 + movIdx;
            if (end_idx > max_idx) end_idx = max_idx;

            int range_begin = idx > min ? idx : min; //取大
            int range_end = end_idx;
            if (max >= dataLength)
            { //有数据删除
                range_end = max;
            }
            else
            {
                if (range_end >= dataLength) range_end = dataLength - 1;
            }

            // Debug.LogFormat ("range_begin{0},range_end:{1},min:{2},idx:{3},max:{4},max_idx:{5},m_PageSize={6},Datalength:{7} ", range_begin, range_end, min, idx, max, max_idx, m_PageSize, dataLength);
            for (int i = range_begin; i <= range_end; i++)
                renderQueue.Enqueue(i);

        }

        // /// <summary>
        // /// 改变数据长度 <br/> 后刷新显示
        // /// </summary>
        // /// 
        // /// <example>
        // /// <code>
        // /// using UnityEngine;
        // /// using System.Collections;
        // /// using UnityEngine.UI;  // Required when Using UI elements.
        // ///
        // /// public class ExampleClass : MonoBehaviour
        // /// {
        // ///     public LoopScrollBase myScrollRect;
        // ///
        // ///     void Awake()
        // ///     {
        // ///         //增加6条数据后从当前位置刷新显示,并自动滚动到最后一条   
        // ///         myScrollRect.AppendLength(6);
        // ///         //删除6条数据并从当前位置开始刷新
        // ///          myScrollRect.AppendLength(-6);
        // ///
        // ///     }
        // /// }
        // /// </code>
        // /// </example>
        // public void AppendLength (int count) //批量插入
        // {
        //     if (m_FootDataIndex == m_DataLength - 1) //处于最后一条
        //     {
        //         m_DataLength += count;
        //         for (int i = m_FootDataIndex; i < m_DataLength; i++)
        //             renderQueue.Enqueue (i);
        //     } else {
        //         m_DataLength += count;
        //         Refresh ();
        //     }
        // }

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //显示数据
        ///         myScrollRect.Refresh(); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Refresh() //开始渲染
        {
            StopMovement();
            int min = int.MaxValue, max = int.MinValue;
            foreach (var item1 in m_Pages)
            {
                if (item1.index != -1)
                {
                    min = Math.Min(item1.index, min);
                    max = Math.Max(item1.index, max);
                }
            }

            UpdateBegin(0);
            // if (min == int.MaxValue) min = 0;
            // if (max == int.MinValue) max = PageSize - 1; //第一次刷新
            // if (max < min + PageSize - 1) max = min + PageSize - 1; //显示不完全
            // if (max >= m_DataLength) max = m_DataLength - 1; //数据长度判断

            // for (int i = min; i <= max; i++)
            //     renderQueue.Enqueue (i);
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public LoopScrollBase myScrollRect;
        ///
        ///     void Awake()
        ///     {
        ///         //清理数据
        ///         myScrollRect.Clear(); 
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Clear()
        {
            StopMovement();
            ClearItems();
            if (content)
            {
                content.anchoredPosition = m_ContentLocalStart;
            }
            m_DataLength = 0;
            m_HeadDataIndex = 0;
            m_FootDataIndex = 0;
        }

        public void OnSelect(ILoopSelectStyle loopSelectStyle) //选中
        {
            if (m_SelecteStyle != null) m_SelecteStyle.CancelStyle();
            var loopItem = loopSelectStyle.loopItem;
            m_SelecteStyle = loopSelectStyle;
            m_LastSelectedIndex = m_SelectedIndex;
            m_SelectedIndex = loopItem.index;
            loopSelectStyle.SelectedStyle();
            if (onSelected != null) onSelected(this.parameter, loopItem.item, m_SelectedIndex, m_LastSelectedIndex);
            object para = null;
            if (itemParameter == null)
                para = loopItem.item;
            else
                para = itemParameter;
            if (itemCommand != null && itemCommand.CanExecute(para))
            {
                itemCommand.Execute(para);
            }
        }

        protected void TriggerStyleBySelectedIndex(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < dataLength)
            {
                //find exist
                var item = GetItemAt(selectedIndex);
                ILoopSelectStyle style;
                if (item != null && (style = item.GetComponent<ILoopSelectStyle>()) != null)
                {
                    OnSelect(style);
                }
                else
                    m_SelectedIndex = selectedIndex;
            }

        }

        #endregion

        #region 模板项目

        List<LoopItem> m_Pages = new List<LoopItem>();
        protected int m_HeadDataIndex;
        protected int m_FootDataIndex;

        protected void CalcPageSize()
        {
            if (m_ItemSource != null)
            {
                var rect = m_ItemSource.GetComponent<RectTransform>().rect;
                if (m_ItemSize.x == 0)
                    SetItemSize(Mathf.Abs(rect.width), 0);
                else
                    SetItemSize(m_ItemSize.x, 0);

                if (m_ItemSize.y == 0)
                    SetItemSize(Mathf.Abs(rect.height), 1);
                else
                    SetItemSize(m_ItemSize.y, 1);
            }

            InitColumns();
            InitPages();
        }

        void InitColumns()
        {
            var vSize = rectTransform.rect; //
            m_RealColumns = m_Columns;
            if (!horizontal && m_Columns == 0 && rectTransform.anchorMin == Vector2.zero && rectTransform.anchorMax == Vector2.one) //自动适应
            {
                columns = Mathf.FloorToInt(Mathf.Abs(vSize.width) / (itemSize.x + this.halfPadding));
            }

            if (columns == 0)
            {
                m_PageSize = Mathf.CeilToInt(Mathf.Abs(vSize.width) / (itemSize.x + this.halfPadding)) + 1;
            }
            else
            {
                m_PageSize = columns * (Mathf.CeilToInt(Mathf.Abs(vSize.height) / (itemSize.y + this.halfPadding)) + 1);
            }

            // Debug.LogFormat("vSize：{0},m_PageSize：{1} ", vSize, m_PageSize);
        }

        void InitPages()
        {
            for (int i = m_Pages.Count; i < m_PageSize; i++)
            {
                m_Pages.Add(new LoopItem());
            }
        }

        protected LoopItem GetLoopItemAt(int idx)
        {
            int i = idx % m_PageSize;
            return m_Pages[i];
        }

        public Component GetItemAt(int idx)
        {
            if (idx < 0) return null;
            var loopItem = GetLoopItemAt(idx);
            if (loopItem.index == idx)
                return loopItem.item;

            return null;
        }

        #endregion

        #region 渲染与布局
        private float m_HalfPadding = float.NaN;
        protected float halfPadding
        {
            get
            {
                if (m_HalfPadding.Equals(float.NaN))
                    m_HalfPadding = padding * .5f;
                return m_HalfPadding;
            }
        }

        protected Queue<int> renderQueue = new Queue<int>(); //渲染队列

        protected void QueueToRender()
        {
            if (renderQueue.Count > 0)
            {
                int idx = renderQueue.Dequeue();
                var item = GetLoopItemAt(idx);
                RenderItem(item, idx);
            }
        }

        protected void RenderItem(LoopItem loopItem, int idx)
        {
            bool dispatchOnSelectedEvent = false;
            var oldRect = loopItem.rect;
            var oldIdx = loopItem.index;
            loopItem.index = idx;

            if (loopItem.item == null && onGetItem == null)
            {
                var trans = itemSource.GetComponent<RectTransform>();
                var item = GameObject.Instantiate(itemSource, trans.position, trans.rotation, content);
                loopItem.item = item;
                loopItem.transform = item.GetComponent<RectTransform>();
                if (onInstantiated != null)
                    onInstantiated(this.parameter, loopItem.item, loopItem.index);

                dispatchOnSelectedEvent = InitItemStyle(loopItem);
            }

            if (onGetItem != null)
            {
                var item = onGetItem(this.parameter, idx, loopItem.item, oldIdx, content); //
                var rent = item.GetComponent<RectTransform>();
                if (rent.parent == null)
                {
                    rent.SetParent(content);
                    rent.rotation = Quaternion.Euler(0, 0, 0);
                    rent.localScale = Vector3.one;
                    Vector3 newPos = Vector3.zero;
                    newPos.x = halfPadding;
                    rent.anchoredPosition = newPos;
                }
                loopItem.transform = rent;
                loopItem.item = item;

                dispatchOnSelectedEvent = InitItemStyle(loopItem);

            }

            var gObj = loopItem.item.gameObject;
            if (idx >= dataLength)
            {
                gObj.SetActive(false);
                return;
            }
            else if (!gObj.activeSelf)
            {
                gObj.SetActive(true);
            }

            //keep selected
            if (m_SelecteStyle != null)
            {
                if (m_SelecteStyle.loopItem.index == m_SelectedIndex)
                    m_SelecteStyle.SelectedStyle();
                else if (m_SelecteStyle.loopItem == loopItem)
                    m_SelecteStyle.CancelStyle();
            }

            if (onItemRender != null) onItemRender(this.parameter, loopItem.item, loopItem.index); //填充内容
            LayOut(loopItem);

            if (dispatchOnSelectedEvent)
            {
                OnSelect(m_SelecteStyle);
            }

        }

        protected void UpdateViewPointBounds()
        {
            Vector2 localp = content.anchoredPosition;
            var cpos = localp - m_ContentLocalStart; //开始位置
            m_ViewPointRect = rectTransform.rect; //
            m_ViewPointRect.height = -Mathf.Abs(m_ViewPointRect.height);
            m_ViewPointRect.x = cpos.x;
            m_ViewPointRect.y = -cpos.y;
        }

        protected bool InitItemStyle(LoopItem loopItem)
        {
            var loopItemSelect = loopItem.item.GetComponent<ILoopSelectStyle>();
            if (loopItemSelect != null)
            {
                loopItemSelect.InitSytle(loopItem, this);
                if (m_SelectedIndex == loopItem.index)
                {
                    m_SelecteStyle = loopItemSelect;
                    // m_SelecteStyle.SelectedStyle();
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        /// 内容滚动
        ///</summary>
        protected abstract void ScrollLoopItem();

        ///<summary>
        /// 计算内容范围
        ///</summary>
        protected abstract void CalcBounds();

        ///<summary>
        /// 布局
        ///</summary>
        protected abstract void LayOut(LoopItem loopItem);

        protected void SetItemSize(float size, int axis)
        {
            // #if UNITY_EDITOR
            //             if (Application.isPlaying)
            // #endif
            m_RuntimeItemSize[axis] = size;
        }
        #endregion

        #region  tool
        protected void ClearItems()
        {
            content.anchoredPosition = m_ContentLocalStart;
            foreach (var item in m_Pages)
            {
                if (item.transform) item.transform.gameObject.SetActive(false);
                item.index = -1;
            }
        }

        #endregion

        #region  static

        #endregion

    }

#if UNITY_EDITOR

    public static class EditorGUITools
    {

        private static Texture2D backgroundTexture = new Texture2D(1, 1);
        private static GUIStyle _staticRectStyle = new GUIStyle { normal = new GUIStyleState { background = backgroundTexture } };

        public static void DrawRect(Rect position, Color color, GUIContent content = null)
        {
            if (backgroundTexture == null) backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();

            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle { normal = new GUIStyleState { background = backgroundTexture } };
            }
            _staticRectStyle.normal.background = backgroundTexture;
            GUI.Box(position, content ?? GUIContent.none, _staticRectStyle);
        }

        public static void LayoutBox(Color color, GUIContent content = null)
        {
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUILayout.Box(content ?? GUIContent.none, _staticRectStyle);
            GUI.backgroundColor = backgroundColor;
        }
    }

#endif
}