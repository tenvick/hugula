using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hugula.UIComponents
{
    /// <summary>
    /// 内容适应滚动列表</br>
    ///  1 支持数据批量插入。</br>
    ///  2 末尾插入数据自动滚动</br>
    ///  3 跳转到头部或者尾部  </br>
    /// </summary>
    public class LoopVerticalScrollRect : VerticalScrollBase, ILoopSelect
    {
        #region  rect属性
        [SerializeField]
        int m_PageSize = 6;
        /// <summary>
        /// 项目池的大小 
        /// </summary>
        public int pageSize { get { return m_PageSize; } set { m_PageSize = value; InitPages(); } } //分页数量 

        [SerializeField]
        float m_Padding = 0;
        /// <summary>
        /// 间距
        /// </summary>
        public float padding { get { return m_Padding; } set { m_Padding = value; m_HalfPadding = float.NaN; } } //间隔

        [SerializeField]
        int m_RenderPerFrames = 1;
        /// <summary>
        /// 次渲染的项目数量</br>
        /// </summary>
        public int renderPerFrames { get { return m_RenderPerFrames; } set { m_RenderPerFrames = value; } } //每次渲染item数

        [SerializeField]
        BindableObject[] m_Templates;

        /// <summary>
        /// clone模板项目</br>
        /// </summary>
        public BindableObject[] templates { get { return m_Templates; } set { m_Templates = value; } } //clone的项目

        [SerializeField]
        GameObject m_CeilBar;
        /// <summary>
        /// 拖拽到顶部提示条</br>
        /// </summary>
        public GameObject ceilBar { get { return m_CeilBar; } set { m_CeilBar = value; } } //clone的项目

        [SerializeField]
        GameObject m_FloorBar;
        /// <summary>
        /// 拖拽到底部提示条</br>
        /// </summary>
        public GameObject floorBar { get { return m_FloorBar; } set { m_FloorBar = value; } } //clone的项目

        [SerializeField]
        int m_DragOffsetShow = 100;

        /// <summary>
        /// 拖拽显示距离</br>
        /// </summary>
        public int dragOffsetShow { get { return m_DragOffsetShow; } set { m_DragOffsetShow = value; } } //clone的项目

        int m_SelectedIndex = -1;
        /// <summary>
        /// 通过OnSelected方法 选中项目的索引</br>
        /// </summary>
        public int selectedIndex { get { return m_SelectedIndex; } }

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
        ///         myScrollRect.OnInstantiated = (object o, int idx) => //o是ItemSource(Clone) ,idx是数据索引
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
        public Action<object, object, int> onInstantiated { get; set; } //实例化

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
        ///         myScrollRect.OnGetItemTemplateType = (int idx/*当前索引 */) =>
        ///         {
        ///             var typeid = source[idx]; //从索引找到数据的模板类型
        ///             return typeid
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Func<object, int, int> onGetItemTemplateType { get; set; } //

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
        public Action<object, Component, int> onItemRender { get; set; } //渲染

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
        ///         myScrollRect.OnSelected = (object o, int idx,int last idx) => //o是ItemSource(Clone) ,idx是数据索引
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

        public object parameter { get; set; }

        /// <summary>
        /// 当在顶部或者底部释放的时候触发。
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
        ///         myScrollRect.OnDroped = (Vector2 offest) =>
        ///         {
        ///             
        ///             if(offest.y > 0 ) //ceil   
        ///             {
        ///                 print("end drag in ceil") ;   
        ///             }else //if(offest.y < 0) { //floor
        ///                 print("end drag in floor") ; 
        ///             }
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        public Action<Vector2> onDropped { get; set; }

        /// <summary>
        /// 当在顶部或者底部释放的时候触发。
        /// </summary>
        public ICommand droppedCommand { get; set; }

        #endregion

        protected override void Awake()
        {
            m_HalfPadding = float.NaN;
            base.Awake();
            InitPages();
            InitTemplatePool();
            onDragChanged.AddListener(OnDraging);
            onEndDragChanged.AddListener(OnEndDrag);
        }

        protected override void LateUpdate()
        {
            if (m_RenderPerFrames < 1) m_RenderPerFrames = 1;
            for (int i = 0; i < this.m_RenderPerFrames; i++)
            {
                QueueToRender();
                if ((m_RenderQueue.Count == 0)) break;
            }

            ScrollLoopVerticalItem();
            Layout();
            base.LateUpdate();
        }

        protected override void OnDestroy()
        {
            onDragChanged.RemoveListener(OnDraging);
            onEndDragChanged.RemoveListener(OnEndDrag);

            onItemRender = null;
            onDropped = null;
            onInstantiated = null;
            content = null;
            itemCommand = null;
            droppedCommand = null;
            m_SelecteStyle = null;
            for (int i = 0; i < m_Templates.Length; i++)
            {
                m_Pool.Clear(i);
            }
            m_Pool = null;
            foreach (var loopItem in m_Pages)
            {
                if (loopItem.item != null)
                {
                    GameObject.Destroy(loopItem.item.gameObject);
                }
                loopItem.item = null;
            }
            base.OnDestroy();
        }

        #region  拖动与数据联动

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_autoScrollFloor = false;
            base.OnBeginDrag(eventData);
        }

        void OnDraging(Vector2 offset)
        {
            m_autoScrollFloor = false;

            if (offset.y >= m_DragOffsetShow) //top
            {
                if (m_CeilBar != null && !m_CeilBar.activeSelf) m_CeilBar.SetActive(true);
            }
            else if (offset.y <= -m_DragOffsetShow) //bottom
            {
                if (m_FloorBar != null && !m_FloorBar.activeSelf) m_FloorBar.SetActive(true);
            }
            else
            {
                if (m_CeilBar != null && m_CeilBar.activeSelf) m_CeilBar.SetActive(false);
                if (m_FloorBar != null && m_FloorBar.activeSelf) m_FloorBar.SetActive(false);
            }
        }

        void OnEndDrag(Vector2 offset)
        {
            if (m_CeilBar != null && m_CeilBar.activeSelf) m_CeilBar.SetActive(false);
            if (m_FloorBar != null && m_FloorBar.activeSelf) m_FloorBar.SetActive(false);
            if (offset.y >= m_DragOffsetShow || offset.y <= -m_DragOffsetShow)
            {
                if (onDropped != null) onDropped(offset);
                if (droppedCommand != null && droppedCommand.CanExecute(offset))
                    droppedCommand.Execute(offset);
            }
        }

        #endregion

        #region  数据操作相关
        //索引后面的项目都需要重新刷新
        // private int m_DirtyIndex = 0;

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
            }
        }

        [Tooltip("whether auto scroll to bottom")]
        [SerializeField]
        bool m_AutoScrollToBottom = true;
        bool m_autoScrollFloor = false; //自动滚动到底部

        bool needScrollToBottom
        {
            get
            {
                return m_AutoScrollToBottom && m_autoScrollFloor;
            }
        }
        /// <summary>
        /// 改变数据长度 <br/> 后刷新显示
        /// </summary>
        /// 
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
        ///         //增加6条数据后从当前位置刷新显示
        ///         myScrollRect.InsertRange(0,6);
        ///
        ///     }
        /// }
        /// </code>
        /// </example>
        public void InsertRange(int index, int count) //批量插入
        {

            //数据变更范围
            // int dataBIdx = index, dataEIdx = index + count; //数据变更范围 dataBIdx~dataEIdx
            //当前数据范围 m_DataLength
            //寻找当前显示范围
            int min = int.MaxValue, max = int.MinValue;
            StopMovement();
            foreach (var item1 in m_Pages)
            {
                if (item1.index != -1)
                {
                    min = Math.Min(item1.index, min);
                    max = Math.Max(item1.index, max);
                }
            }

            if ((max == m_DataLength - 1 && index != 0) || max == int.MinValue) //判断最后一条位置
            {
                m_autoScrollFloor = true;
            }

            int eIdx, bIdx;
            m_DataLength += count;

            //显示规则 1如果min=MaxValue,刷新  m_DataLength - pagesize - 1 ~ pageSize     索引并判断是否滚动到底部。 
            if (min == int.MaxValue)
            {
                bIdx = m_DataLength - pageSize - 1;
                eIdx = m_DataLength - 1;

                if (bIdx < 0) bIdx = 0;
                // Debug.LogFormat("bIdx={0},eIdx={1},m_DataLength={2}", bIdx, eIdx, m_DataLength);
                for (int i = bIdx; i <= eIdx; i++)
                    m_RenderQueue.Enqueue(i);
            }
            else //if (index == 0) //从开始位置插入数据
            {

                eIdx = min + m_PageSize - 1; //本来应该的最大索引
                if (eIdx >= m_DataLength) eIdx = m_DataLength - 1; //最大索引不能超过datalength
                // if (eIdx <= 0) eIdx = 0;

                bIdx = index > min ? index : min; //最小开始位置索引 

                if (needScrollToBottom) //m_autoScrollFloor)
                {
                    if (max < 0) max = 0;
                    // Debug.LogFormat("m_FloorDataIndex={0},max={1},m_DataLength={2},count={3}", min, max, dataLength, count);
                    for (int i = max; i < m_DataLength; i++)
                        m_RenderQueue.Enqueue(i);
                }
                else if (max > eIdx && eIdx >= 0) //有数据删除 往前渲染
                {
                    int bi = min - (max - eIdx);
                    if (bi < 0) bi = 0;
                    // Debug.LogFormat("delete min={0},bi = {1},max={2},DataLength={3}", min, bi, max, dataLength);
                    for (int i = min; i >= bi; i--)
                        m_RenderQueue.Enqueue(i);
                }
                else //从idx到end刷新
                {
                    // Debug.LogFormat("min={0},eIdx = {1},max={2},DataLength={3}", min, eIdx, max, dataLength);
                    for (int i = min; i <= eIdx; i++)
                        m_RenderQueue.Enqueue(i);
                }

            }

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
            UpdateBegin(index);
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
        public void UpdateBegin(int idx)
        {
            int min = int.MaxValue, max = int.MinValue;
            foreach (var item1 in m_Pages)
            {
                if (item1.index != -1)
                    min = Math.Min(item1.index, min);
                max = Math.Max(item1.index, max);
            }

            if (min == int.MaxValue) min = 0;
            int min_idx = min;
            int movIdx = 0;
            // if (m_Columns != 0) movIdx = min % Columns; //如果顶部没有对齐。
            int max_idx = min + m_PageSize - 1 + movIdx;

            int range_begin = idx > min ? idx : min; //取大
            int range_end = max_idx;
            if (max >= dataLength)
            { //有数据删除
                range_end = max;
            }
            else
            {
                if (range_end >= dataLength) range_end = dataLength - 1;
            }

            // Debug.LogFormat("range_begin{0},range_end:{1},min:{2},idx:{3},max:{4},max_idx:{5},m_PageSize={6},Datalength:{7} ", range_begin, range_end, min, idx, max, max_idx, m_PageSize, dataLength);
            for (int i = range_begin; i <= range_end; i++)
                m_RenderQueue.Enqueue(i);

        }

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
        public void Refresh()
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

            if (min == int.MaxValue) min = 0;
            int eIdx = min + m_PageSize - 1; //本来应该的最大索引
            if (eIdx >= m_DataLength) eIdx = m_DataLength - 1; //最大索引不能超过datalength

            if (max > eIdx && eIdx >= 0) //有数据删除 往前渲染
            {
                int bi = min - (max - eIdx);
                if (bi < 0) bi = 0;
                // Debug.LogFormat("delete min={0},bi = {1},max={2},DataLength={3}", min, bi, max, DataLength);
                for (int i = min; i >= bi; i--)
                    m_RenderQueue.Enqueue(i);
            }
            else //从idx到end刷新
            {
                // Debug.LogFormat("min={0},eIdx = {1},max={2},DataLength={3}", min, eIdx, max, DataLength);
                for (int i = min; i <= eIdx; i++)
                    m_RenderQueue.Enqueue(i);
            }

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
            BackToStart();
            m_DataLength = 0;
            m_DataBounds.Clear();
            m_ShowList.Clear();
            m_RenderQueue.Clear();
            ClearItems();
            InitContentBounds();
        }

        private void BackToStart()
        {
            if (content)
                content.anchoredPosition = m_ContentInitializePosition;
        }

        private int[] defaultArg = new int[2];
        public void OnSelect(ILoopSelectStyle loopSelectStyle) //选中
        {
            if (m_SelecteStyle != null) m_SelecteStyle.CancelStyle();
            var loopItem = loopSelectStyle.loopItem;
            m_SelecteStyle = loopSelectStyle;
            loopSelectStyle.SelectedStyle();
            int lastIdx = m_SelectedIndex;
            m_SelectedIndex = loopItem.index;
            if (onSelected != null) onSelected(this.parameter, loopItem.item, loopItem.index, lastIdx);
            defaultArg[0] = loopItem.index;
            defaultArg[1] = lastIdx;
            if (itemCommand != null && itemCommand.CanExecute(defaultArg))
            {
                itemCommand.Execute(defaultArg);
            }
        }

        #endregion

        #region 模板项目

        protected List<LoopVerticalItem> m_Pages = new List<LoopVerticalItem>();

        protected Hugula.Utils.GameObjectPool<BindableObject> m_Pool;
        void OnPoolGet(BindableObject comp)
        {
            comp.gameObject.SetActive(true);
        }

        void OnPoolRealse(BindableObject comp)
        {
            comp.gameObject.SetActive(false);
        }

        void InitTemplatePool()
        {
            if (m_Pool == null)
            {
                m_Pool = new Hugula.Utils.GameObjectPool<BindableObject>(OnPoolGet, OnPoolRealse);
                for (int i = 0; i < templates.Length; i++)
                    m_Pool.Add(i, templates[i]);
            }
        }

        void InitPages()
        {
            for (int i = m_Pages.Count; i < m_PageSize; i++)
            {
                m_Pages.Add(new LoopVerticalItem());
            }
        }

        protected LoopVerticalItem GetLoopVerticalItemAt(int idx)
        {
            int i = idx % pageSize;
            return m_Pages[i];
        }

        public Component GetItemAt(int idx)
        {
            if (idx < 0) return null;
            var loopItem = GetLoopVerticalItemAt(idx);
            if (loopItem.index == idx)
                return loopItem.item;

            return null;
        }

        #endregion

        #region 渲染与布局        

        //每一项目的位置
        Dictionary<int, Vector2> m_DataBounds = new Dictionary<int, Vector2>();
        //显示的列表索引
        List<int> m_ShowList = new List<int>();
        float m_CalcBoundyMin;
        float m_CalcBoundyMax;
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

        protected Queue<int> m_RenderQueue = new Queue<int>(); //渲染队列

        void AddShowList(int index)
        {
            m_ShowList.Add(index);
        }

        protected void QueueToRender()
        {
            if (m_RenderQueue.Count > 0)
            {
                int idx = m_RenderQueue.Dequeue();
                var item = GetLoopVerticalItemAt(idx);
                RenderItem(item, idx);
            }
        }

        //立即完成等待队列
        protected void RenderWaitQueue()
        {
            while (m_RenderQueue.Count > 0)
            {
                int idx = m_RenderQueue.Dequeue();
                var item = GetLoopVerticalItemAt(idx);
                RenderItem(item, idx);
            }
        }

        Component OnCreateOrGetItem(int idx, int templateId, LoopVerticalItem loopItem, RectTransform content)
        {
            Component item = loopItem.item;
            if (item == null || loopItem.templateType != templateId)
            {
                //还对象到缓存池
                m_Pool.Release(loopItem.templateType, (BindableObject)item);
                bool isNew = false;
                item = m_Pool.Get(templateId, content, out isNew); //创建或者从缓存中获取
                loopItem.templateType = templateId;
                if (isNew)
                {
                    var sourceRT = templates[templateId].GetComponent<RectTransform>();
                    var itemTrans = item.transform;
                    // itemTrans.SetParent(content);
                    // itemTrans.SetAsLastSibling();
                    itemTrans.localScale = sourceRT.localScale;
                    itemTrans.localRotation = sourceRT.localRotation;
                    itemTrans.localPosition = sourceRT.localPosition;
                    if (onInstantiated != null)
                        onInstantiated(this.parameter, item, idx);

                }
                var selecteStyle = item.GetComponent<ILoopSelectStyle>();
                if (selecteStyle != null) selecteStyle.InitSytle(loopItem, this);
            }

            return item;
        }

        protected void RenderItem(LoopVerticalItem loopItem, int idx)
        {
            var oldRect = loopItem.rect;
            var oldIdx = loopItem.index;

            if (idx < dataLength)
            {
                var templateId = 0;
                if (onGetItemTemplateType != null) templateId = onGetItemTemplateType(this.parameter, idx);
                var item = OnCreateOrGetItem(idx, templateId, loopItem, content); //
                // item.name = string.Format("item {0} {1}", idx, templateId);

                // Debug.LogFormat("RenderItem oldIdx={0},item={1},loopItem={2},idx={3},templateId={4} ", oldIdx, item, loopItem.item, idx, templateId);

                var rent = item.GetComponent<RectTransform>();
                loopItem.transform = rent;
                loopItem.item = item;
                loopItem.index = idx;
            }

            var gObj = loopItem?.item.gameObject;
            if (idx >= dataLength)
            {
                // CalcBounds (loopItem, oldIdx, oldRect); //删除时候需要计算bounds
                if (gObj) gObj.SetActive(false);
                loopItem.index = -1;
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

            AddShowList(idx);
            if (onItemRender != null) onItemRender(this.parameter, loopItem.item, loopItem.index); //填充内容
            //
            CalcItemBound(loopItem);
            // CalcBounds (loopItem, oldIdx, oldRect);
        }

        ///<summary>
        /// 内容滚动
        ///</summary>
        protected void ScrollLoopVerticalItem()
        {
            if (velocity.y == 0 && !isTweening) return; //没有移动不需要计算
            int min = int.MaxValue, max = int.MinValue;
            int max1; //= GetBottomItem ();
            int min1; //= GetTopItem ();
            LoopVerticalItem itemToRender = null;
            for (int i = 0; i < m_Pages.Count; i++)
            {
                var item = m_Pages[i];
                if (item.index == -1)
                    return;
                else
                    min = Math.Min(item.index, min);

                max = Math.Max(item.index, max);
            }

            if (velocity.y > 0 || tweenDir.y > 0)
            {
                UpdateBounds();
                // RenderWaitQueue ();
                max1 = max;
                min1 = min;
                // Debug.LogFormat ("m_ViewBounds={0},m_ViewBounds.yMin{1},m_ViewBounds.yMax{2},(min1).yMax={3},(min1).yMax<m_ViewBounds.yMin:{4}",m_ViewBounds, m_ViewBounds.yMin,m_ViewBounds.yMax,GetLoopVerticalItemAt (min1).yMax,Mathf.Abs(GetLoopVerticalItemAt (min1).yMax) < Mathf.Abs (m_ViewBounds.yMin));
                while (m_DataLength > max1 && GetLoopVerticalItemAt(min1).yMax < -m_ViewBounds.yMin) //!m_ViewBounds.Overlaps (m_Pages[min1 % pageSize].rect, true)) // move top do down
                {
                    max1 = max1 + 1;
                    // Debug.LogFormat ("min1={0},max1:{1} yMax:{2}>m_ViewBounds.yMin{3} ", min1, max1, GetLoopVerticalItemAt (min1).yMax, m_ViewBounds.yMin);
                    if (m_DataLength > max1 && (itemToRender = GetLoopVerticalItemAt(max1)) != null && itemToRender.index != max1)
                    {
                        min1 = min1 + 1;
                        RenderItem(itemToRender, max1);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (velocity.y < 0 || tweenDir.y < 0)
            {
                UpdateBounds();
                // RenderWaitQueue ();
                max1 = max;
                min1 = min;
                while (min1 >= 0 && GetLoopVerticalItemAt(max1).yMin > -m_ViewBounds.yMin + Mathf.Abs(m_ViewBounds.height)) //!m_ViewBounds.Overlaps (m_Pages[max1 % pageSize].rect, true)) //move bottom item to top
                {
                    min1 = min1 - 1;
                    // Debug.LogFormat ("min1={0},max1:{1} yMin:{2}>m_ViewBounds.yMax{3} ", min1, max1, GetLoopVerticalItemAt (max1).yMin, m_ViewBounds.yMax);
                    if (min1 >= 0 && (itemToRender = GetLoopVerticalItemAt(min1)) != null && itemToRender.index != min1)
                    {
                        max1 = max1 - 1;
                        RenderItem(itemToRender, min1);
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

        ///<summary>
        /// 计算内容Bounds
        ///</summary>
        protected void CalcBounds()
        {

            if (m_CalcBoundyMax > -m_ContentBounds.yMax)
            {
                m_ContentBounds.yMax = -m_CalcBoundyMax;
            }

            // if (m_autoScrollFloor && m_RenderQueue.Count == 0)
            if (needScrollToBottom && m_RenderQueue.Count == 0)
            {
                Vector2 curr = content.anchoredPosition;
                curr.y = m_ContentBounds.yMax - m_ViewBounds.height; //如果标记了需要滚动到末尾
                if (curr.y < 0)
                {
                    curr.y = -curr.y;
                    content.anchoredPosition = curr;
                }
            }

            if (m_CalcBoundyMin < m_ContentBounds.yMin) m_ContentBounds.yMin = -m_CalcBoundyMin;

            // Debug.LogFormat ("{0},ymax{1}.ymin={2},height={5}m_CalcBoundyMin={3},m_CalcBoundyMax={4}", m_ContentBounds, m_ContentBounds.yMax, m_ContentBounds.yMin, m_CalcBoundyMin, m_CalcBoundyMax,m_ContentBounds.height);
            var size = content.sizeDelta;
            size.y = Mathf.Abs(m_ContentBounds.height);
            content.sizeDelta = size;
            // Debug.LogFormat ("content.sizeDelta={0} ", content.sizeDelta);
        }

        ///<summary>
        /// 计算bound 高度
        ///</summary>
        protected void CalcItemBound(LoopVerticalItem loopItem)
        {
            RectTransform rectTran = loopItem.transform;
            if (rectTran.anchorMin != rectTran.anchorMax) //表示宽度适配
            {
                var offsetMin = rectTran.offsetMin;
                var offsetMax = rectTran.offsetMax;
                offsetMin.x = halfPadding;
                offsetMax.x = -halfPadding;
                rectTran.offsetMin = offsetMin;
                rectTran.offsetMax = offsetMax;
            }

            // Debug.LogFormat("LayoutRebuilder.ForceRebuildLayoutImmediate= {0},frame={1}", loopItem.item,  Time.frameCount);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTran); //立马计算布局
            var layoutEle = loopItem.item.GetComponent<LayoutElement>();
            Vector2 bound = Vector2.zero;
            bound.x = halfPadding;
            if (layoutEle != null)
            {
                bound.y = layoutEle.preferredHeight + halfPadding; //高度
            }
            else
            {
                bound.y = Mathf.Abs(rectTran.rect.height) + halfPadding;
            }
            loopItem.SetHeight(bound.y);
            // Debug.LogFormat ("End item= {0},layoutEle={1},bound={2},frame={3}", loopItem.item, layoutEle, bound, Time.frameCount);

            // int index = loopItem.index;

            // //寻找上一条
            // //判断方向
            // bool findLast = false;
            // int idx = index - 1;

            // Vector2 lastPos = Vector2.zero;
            // if (idx >= 0) {
            //     var lastItem = GetLoopVerticalItemAt (idx);
            //     if (lastItem.index == idx) {
            //         lastPos.y = lastItem.yMax;
            //         bound.x = lastPos.y + halfPadding;
            //         findLast = true;
            //     }
            // }

            // if (!findLast && (idx = index + 1) < m_DataLength) //寻找下一条
            // {
            //     var lastItem = GetLoopVerticalItemAt (idx);
            //     if (lastItem.index == idx) {
            //         lastPos.y = lastItem.yMin - bound.y;
            //         bound.x = lastPos.y + halfPadding;
            //         // lastPos.y = lastItem.rect.yMin - (rect.height - this.padding);
            //     }
            // }

            // loopItem.bounds = bound;
            // Vector2 pos = rectTran.anchoredPosition;

            // pos.y = -bound.x ; // rect.height * .5f ;
            // pos = pos + m_ContentInitializePosition; //开始位置

            // rectTran.anchoredPosition3D = Vector3.zero;
            // rectTran.anchoredPosition = pos;
        }

        bool GetItemYMax(int index, bool isYmax, out float posY)
        {
            posY = 0;
            Vector2 lastBound;
            if (m_DataBounds.TryGetValue(index, out lastBound))
            {
                if (isYmax)
                    posY = lastBound.x + lastBound.y;
                else
                    posY = lastBound.x;
                return true;
            }
            return false;
        }

        void SetItemBound(int index, Vector2 bound)
        {
            m_DataBounds[index] = bound;
        }

        ///<summary>
        /// 布局
        ///</summary>
        public void Layout()
        {
            LoopVerticalItem item;
            float lastPosY = halfPadding;
            int index = 0;
            for (int i = 0; i < m_ShowList.Count; i++)
            {
                index = m_ShowList[i];
                item = GetLoopVerticalItemAt(index);
                if (item.isDirty)
                {
                    if (!GetItemYMax(index - 1, true, out lastPosY))
                        if (GetItemYMax(index + 1, false, out lastPosY))
                            lastPosY = lastPosY - item.bound.y - padding;
                    item.SetPos(lastPosY + halfPadding);
                    // Debug.LogFormat (" index={0},lastPosY={1},item.bound={2}", index, lastPosY, item.bound);
                    SetItemBound(index, item.bound);

                    var rectTran = item.transform;
                    Vector2 pos = rectTran.anchoredPosition;
                    pos.y = -item.yMin; // rect.height * .5f ;
                    pos = pos + m_ContentInitializePosition; //开始位置
                    rectTran.anchoredPosition3D = Vector3.zero;
                    rectTran.anchoredPosition = pos;
                    item.isDirty = false;
                    m_CalcBoundyMin = Mathf.Min(m_CalcBoundyMin, item.yMin - halfPadding);
                    if (index == 0)
                    {
                        m_CalcBoundyMin = item.yMin - halfPadding;
                        m_ContentBounds.yMin = -m_CalcBoundyMin; //ContentBounds坐标系统与transform是反的
                    }
                    m_CalcBoundyMax = Mathf.Max(m_CalcBoundyMax, item.yMax);
                    if (index == m_DataLength - 1)
                    {
                        m_CalcBoundyMax = item.yMax;
                        m_ContentBounds.yMax = -m_CalcBoundyMax; //ContentBounds坐标系统与transform是反的
                    }
                    // Debug.LogFormat ("m_CalcBoundyMax={0},m_CalcBoundyMin={1},index = {2} ", m_CalcBoundyMax, m_CalcBoundyMin, index);
                }
            }
            m_ShowList.Clear();

            CalcBounds();
        }

        #endregion

        #region  跳转
        ///<summary>
        /// 滚动到底部或者顶部
        /// 对方法的包装方便数据绑定
        ///</summary>
        public bool scrollToBottom
        {
            get
            {
                return false;
            }
            set
            {
                ScrollTo(value);
            }
        }

        ///<summary>
        /// 滚动到底部或者顶部
        ///</summary>
        public void ScrollTo(bool isBottom = true)
        {
            StopMovement();
            var cpos = Vector2.zero; //开始位置
            Vector2 curr = content.anchoredPosition;

            if (isBottom)
            {
                cpos.x = curr.x;
                cpos.y = Mathf.Abs(m_ContentBounds.yMax - m_ViewBounds.height); //
            }
            else
            {
                cpos.x = curr.x;
                cpos.y = m_ContentBounds.yMin + halfPadding; //- m_ContentStartPosition.y;
            }

            if (m_Coroutine != null) StopCoroutine(m_Coroutine);
            m_Coroutine = StartCoroutine(TweenMoveToPos(curr, cpos, 0.5f));

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

        protected IEnumerator TweenMoveToPos(Vector2 pos, Vector2 v2Pos, float delay)
        {
            var waitForend = new WaitForEndOfFrame();
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
                    tweenDir = Vector2.zero;
                    StopCoroutine(m_Coroutine);
                    m_Coroutine = null;
                }
                else
                {
                    vCur = Vector2.Lerp(pos, v2Pos, passedTime / delay);
                }
                content.anchoredPosition = vCur;
            }

        }
        protected Coroutine m_Coroutine = null;

        #endregion

        #region  tool
        protected void ClearItems()
        {
            foreach (var item in m_Pages)
            {
                if (item.transform)
                {
                    item.transform.gameObject.SetActive(false);
                    item.transform.GetComponent<BindableObject>()?.Unapply();
                } 
                item.index = -1;
            }
        }
        #endregion

    }
}