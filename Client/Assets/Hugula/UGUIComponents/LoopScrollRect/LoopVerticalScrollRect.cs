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
    public class LoopVerticalScrollRect : VerticalScrollBase, ILoopSelect, IScrollLayoutChange
    {
        #region  rect属性
        [SerializeField]
        int m_PageSize = 1;
        /// <summary>
        /// 项目池的大小 
        /// </summary>
        public int pageSize { get { return m_PageSize; } set { m_PageSize = value; InitPages(); } } //分页数量 

        [SerializeField]
        float m_PaddingTop = 0;
        /// <summary>
        /// 间距
        /// </summary>
        public float paddingTop { 
            get { return m_PaddingTop; } 
        } //间隔

        [SerializeField]
        float m_PaddingBottom = 0;
        public float paddingBottom { get { return m_PaddingBottom; } } //底部间隔

        [SerializeField]
        int m_RenderPerFrames = 1;
        /// <summary>
        /// 次渲染的项目数量</br>
        /// </summary>
        public int renderPerFrames { get { return m_RenderPerFrames; } set { m_RenderPerFrames = value; } } //每次渲染item数

        [PopUpComponentsAttribute]
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
        public GameObject ceilBar { get { return m_CeilBar; } set { m_CeilBar = value; } } //

        [SerializeField]
        GameObject m_FloorBar;
        /// <summary>
        /// 拖拽到底部提示条</br>
        /// </summary>
        public GameObject floorBar { get { return m_FloorBar; } set { m_FloorBar = value; } } //

        [SerializeField]
        RectTransform m_LoadingBar;
        /// <summary>
        /// 加载时候的提示</br>
        /// </summary>
        public RectTransform loadingBar { get { return m_LoadingBar; } set { m_LoadingBar = value; } } //

        [SerializeField]
        int m_DragOffsetShow = 100;

        /// <summary>
        /// 拖拽显示距离</br>
        /// </summary>
        public int dragOffsetShow { get { return m_DragOffsetShow; } set { m_DragOffsetShow = value; } } //

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

        bool m_ISLoadingData = false;
        public bool isLoadingData
        {
            get
            {
                return m_ISLoadingData;
            }
            set
            {
                m_ISLoadingData = value;
                if (m_LoadingBar)
                {
                    m_LoadingBar.gameObject.SetActive(m_ISLoadingData);
                    if (m_ISLoadingData)
                    {
                        m_ContentInitializeOffset = m_LoadingBar.anchoredPosition;
                    }
                    else
                        m_ContentInitializeOffset = Vector2.zero;
                }
            }
        }

        ILoopSelectStyle m_SelecteStyle;
        /// <summary>
        /// 当item被clone时候调用
        /// </summary>
        public Action<object, object, int> onInstantiated { get; set; } //实例化

        /// <summary>
        /// 获取模板类型
        /// </summary>
        public Func<object, int, int> onGetItemTemplateType { get; set; } //

        /// <summary>
        /// 填充数据显示项目
        /// </summary>
        public Action<object, Component, int> onItemRender { get; set; } //渲染

        /// <summary>
        /// 选中某个项
        /// </summary>
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
        public Action<Vector2> onDropped { get; set; }

        /// <summary>
        /// 当在顶部或者底部释放的时候触发。
        /// </summary>
        public ICommand droppedCommand { get; set; }

        #endregion

        protected override void Awake()
        {
            m_VerticalPadding = float.NaN;
            virtualVertical.paddingTop = paddingTop;
            virtualVertical.paddingBottom = paddingBottom;
            base.Awake();
            InitPages();
            InitTemplatePool();
            // virtualVertical.padding = paddingTop;
            onDragChanged.AddListener(OnDraging);
            onEndDragChanged.AddListener(OnEndDrag);
        }

        protected void Update()
        {
            if (m_RenderQueue.Count > 0)
            {
                int idx = m_RenderQueue.Dequeue();
                var item = GetLoopVerticalItemAt(idx);
                RenderItem(item, idx);
            }

            ScrollLoopVerticalItem();
        }

        protected override void LateUpdate()
        {
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
        private int m_DataLength;

        /// <summary>
        /// 设置数据长度
        /// </summary>
        public int dataLength
        {
            get
            {
                return m_DataLength;
            }
            set
            {
                Clear();
                m_DataLength = value;
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

            if (m_AutoScrollToBottom && ((max == m_DataLength - 1 && index != 0) || max == int.MinValue)) //判断最后一条位置
            {
                m_autoScrollFloor = true;
            }

            m_DataLength += count;

            if (min != int.MaxValue && index < min) index = min;

            UpdateBegin(index);

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void RemoveAt(int index, int count = 1)
        {
            // m_DataLength -= count;
            // //刷新数据
            // UpdateBegin(index);

            if (index < 0 || index >= dataLength || count <= 0) return;

            m_DataLength -= count;
            // 移动数据 将当前index后面的数据向前移动cout个位置
            int min = int.MaxValue, max = int.MinValue;
            int remBeginIdx = int.MinValue, remEndIdx = int.MinValue;
            LoopItem repItem = null;
            int index1 = 0, nextIdx = 0;

            LoopItem item1;
            for (int i = 0; i < m_Pages.Count; i++)
            {
                item1 = m_Pages[i];
                index1 = item1.index;
                nextIdx = index1 + count;
                if (index1 != -1)
                    min = Math.Min(index1, min);

                max = Math.Max(index1, max);

                if (index1 >= index) //需要移动下面的项目来填充
                {
                    if (remBeginIdx == int.MinValue)
                    {
                        remBeginIdx = index1;
                        remEndIdx = index1;
                    }

                    if (m_SelectedIndex == index1)
                    {
                        selectedIndex = -1;
                        selectedIndex = index;
                    }

                    repItem = GetRealItemAt(nextIdx);
                    if (repItem != null)
                    {
                        if (repItem.onlyPosDirty == false)
                        {
                            ReplaceLoopItemAt(index1, nextIdx);
                            remEndIdx = nextIdx;
                        }
                    }
                    else
                    {
                        if (remBeginIdx > index) //如果第一个没有被替换 刷新到开头
                        {
                            ReplaceLoopItemAt(remEndIdx, index);
                        }
                        item1.index = -1; //强制刷新
                        item1.onlyPosDirty = true;
                        // LayOut(item1, index1+1); //刷新到下一个位置
                    }
                }
            }

            if (min == int.MaxValue) min = 0;
            int min_idx = min;
            int movIdx = 0;
            // if (m_Columns != 0) movIdx = min % Columns; //如果顶部没有对齐。
            int max_idx = min + m_PageSize - 1 + movIdx;

            int range_begin = index > min ? index : min; //取大
            int range_end = max_idx;
            if (max >= dataLength)
            { //有数据删除
                range_end = max;
            }
            else
            {
                if (range_end >= dataLength) range_end = dataLength - 1;
            }

            m_StartIdx = min;
            m_EndIdx = range_end;

            for (int i = range_begin; i <= range_end; i++)
                m_RenderQueue.Enqueue(i);

            CalcBounds();
        }

        /// <summary>
        /// 从指定位置开始刷新直到显示范围尾
        /// </summary>
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

            m_StartIdx = min;
            m_EndIdx = range_end;
            // Debug.LogFormat("range_begin{0},range_end:{1},min:{2},idx:{3},max:{4},max_idx:{5},m_PageSize={6},Datalength:{7} ", range_begin, range_end, min, idx, max, max_idx, m_PageSize, dataLength);
            for (int i = range_begin; i <= range_end; i++)
                m_RenderQueue.Enqueue(i);

        }

        /// <summary>
        /// 显示数据
        /// </summary>
        public void Refresh()
        {
            StopMovement();

            var sIdx = m_StartIdx;
            for (int i = 0; i < pageSize; i++)
            {
                m_EndIdx = sIdx + i;
                m_RenderQueue.Enqueue(m_EndIdx);
                if (m_EndIdx >= m_DataLength - 1)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        public void Clear()
        {
            StopMovement();
            BackToStart();
            m_DataLength = 0;
            m_RenderQueue.Clear();
            ClearItems();
            InitContentBounds();
        }

        private void BackToStart()
        {
            if (content)
                content.anchoredPosition = m_ContentInitializePosition;
        }

        public struct SelectArg
        {
            public int selectedIndex;
            public int lastSelectedIndex;
        }
        private SelectArg defaultArg = new SelectArg();

         protected bool CanSelect(int index, out object parameter) //选中
        {
            parameter = null;
            object para = null;
            if (itemParameter == null)
            {
                defaultArg.selectedIndex = index;
                defaultArg.lastSelectedIndex = m_SelectedIndex;
                para = defaultArg;
            }
            else
                para = itemParameter;


            if (itemCommand != null)
            {
                if (itemCommand.CanExecute(para)) //check can select
                {
                    parameter = para;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        
        public void OnSelect(LoopItem loopItem) //选中
        {
            ILoopSelectStyle loopSelectStyle = loopItem.loopSelectStyle;
            // if (onSelected != null) onSelected(this.parameter, loopItem.item, loopItem.index, lastIdx);
            var can = CanSelect(loopItem.index, out object para);
            if (can)
            {
                if (m_SelecteStyle != null) m_SelecteStyle.CancelStyle();
                m_SelecteStyle = loopSelectStyle;
                m_LastSelectedIndex = m_SelectedIndex;
                m_SelectedIndex = loopItem.index;
                loopSelectStyle.SelectedStyle();
                if (para != null) itemCommand?.Execute(para);
            }           
        }

        protected void TriggerStyleBySelectedIndex(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < dataLength)
            {
                //find exist
                var item = GetRealItemAt(selectedIndex);
                if (item != null)
                {
                    OnSelect(item);
                }
                else
                {
                    var  can = CanSelect(selectedIndex, out object para);
                    if (can)
                    {
                        m_SelectedIndex = selectedIndex;
                    }                    
                }
            }
        }

        #endregion

        #region 模板项目

        protected List<LoopItem> m_Pages = new List<LoopItem>();

        protected Hugula.Utils.GameObjectPool<BindableObject> m_Pool;
        void OnPoolGet(BindableObject comp)
        {
            comp.gameObject?.SetActive(true);
        }

        void OnPoolRealse(BindableObject comp)
        {
            comp.gameObject?.SetActive(false);
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
                m_Pages.Add(new LoopItem());
            }
        }

        protected LoopItem GetLoopVerticalItemAt(int idx)
        {
            int i = idx % pageSize;
            return m_Pages[i];
        }

        public LoopItem GetRealItemAt(int idx)
        {
            if (idx < 0) return null;
            int i = idx % pageSize;
            var loopItem = m_Pages[i];
            if (loopItem.index == idx)
                return loopItem;

            return null;
        }

        /// <summary>
        /// 替换指定位置的项目 idx的item与idx2的item交换
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        /// <returns></returns>
        protected bool ReplaceLoopItemAt(int idx1, int idx2)
        {
            int i = idx1 % m_PageSize;
            int j = idx2 % m_PageSize;
            if (i == j) return false;

            var idx1Item = m_Pages[i];
            var idx2Item = m_Pages[j];

            idx1Item.onlyPosDirty = true;
            var templateId = idx1Item.templateType;
            var temp = idx1Item.item;
            var tmptrans = idx1Item.transform;
            var tmpstyle = idx1Item.loopSelectStyle;
            var tmpstyle2 = idx2Item.loopSelectStyle;

            idx1Item.item = idx2Item.item;
            idx1Item.transform = idx2Item.transform;
            idx1Item.templateType = idx2Item.templateType;
            if (tmpstyle != null)
                tmpstyle.InitSytle(idx2Item, this);

            idx2Item.item = temp;
            idx2Item.transform = tmptrans;
            idx2Item.templateType = templateId;
            if (tmpstyle2 != null)
                tmpstyle2.InitSytle(idx1Item, this);

            idx2Item.onlyPosDirty = true;

            return true;
        }


        #endregion

        #region 渲染与布局        
#if UNITY_EDITOR
        //  [SerializeField]    
#endif
        int m_StartIdx = 0;
#if UNITY_EDITOR
        //  [SerializeField]    
#endif
        int m_EndIdx = 0;
        //每一项目的位置
        VirtualVerticalList virtualVertical = new VirtualVerticalList();
        //已经绑定需要刷新的项
        List<int> m_LayoutList = new List<int>();
        //显示的列表索引
        float m_CalcBoundyMin;
        float m_CalcBoundyMax;
        private float m_VerticalPadding = float.NaN;
        /// <summary>
        /// 垂直间距
        /// </summary>
        protected float verticalPadding
        {
            get
            {
                if (m_VerticalPadding.Equals(float.NaN))
                    m_VerticalPadding = paddingTop + paddingBottom;
                return m_VerticalPadding;
            }
        }

        protected Queue<int> m_RenderQueue = new Queue<int>(); //渲染队列

        Component OnCreateOrGetItem(int idx, int templateId, LoopItem loopItem, RectTransform content)
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

                var notifyLayoutElement = item.GetComponent<NotifyLayoutElement>();
                if (notifyLayoutElement != null) notifyLayoutElement.Init(loopItem, this);
            }

            return item;
        }

        protected void RenderItem(LoopItem loopItem, int idx)
        {
            var oldIdx = loopItem.index;

            if (idx < dataLength)
            {
                var templateId = 0;
                if (onGetItemTemplateType != null) templateId = onGetItemTemplateType(this.parameter, idx);
                var item = OnCreateOrGetItem(idx, templateId, loopItem, content); //
#if UNITY_EDITOR
                item.name = string.Format("{0}-{1}", idx, templateId);
#endif


                var rent = item.GetComponent<RectTransform>();
                loopItem.transform = rent;
                loopItem.item = item;
                loopItem.index = idx;
            }

            var gObj = loopItem?.item?.gameObject;
            if (idx >= dataLength)
            {
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

            AddLayoutList(idx);
            if (onItemRender != null) onItemRender(this.parameter, loopItem.item, loopItem.index); //填充内容
            //
            CalcItemBound(loopItem);
        }

        protected void RenderItemByIdx(int idx)
        {
            var loopItem = GetLoopVerticalItemAt(idx);
            if (loopItem != null) RenderItem(loopItem, idx);
        }

        ///<summary>
        /// 内容滚动
        ///</summary>
        protected void ScrollLoopVerticalItem()
        {
            if (m_DataLength <= m_PageSize) return;
            UpdateBounds();
            var yViewMin = -m_ViewBounds.yMin;
            var yViewMax = -m_ViewBounds.yMax;
            LoopItem itemToRender = null;
            int check = 0;

            var startSize = GetLoopVerticalItemSize(m_StartIdx);
            var endSize = GetLoopVerticalItemSize(m_EndIdx);

            if (startSize.y < yViewMin && endSize.x > yViewMax) //contains view bounds
            {

            }
            else if (startSize.x >= yViewMin && endSize.y > yViewMax)
            {
                while (GetLoopVerticalItemSize(m_StartIdx).x >= yViewMin && GetLoopVerticalItemSize(m_EndIdx).x > yViewMax && m_StartIdx > 0) //向前start方向渲染
                {
                    itemToRender = GetLoopVerticalItemAt(m_StartIdx - 1);
                    m_StartIdx--;
                    m_EndIdx--;
                    //  Debug.Log($" scroll loop vertical start:Render({m_StartIdx}) from {itemToRender.index} end:{m_EndIdx} ");
                    RenderItem(itemToRender, m_StartIdx);
                    if (check++ >= pageSize) break;

                }

            }
            else if (endSize.y <= yViewMax)
            {
                while (GetLoopVerticalItemSize(m_EndIdx).y <= yViewMax && GetLoopVerticalItemSize(m_StartIdx).x < yViewMin && m_EndIdx < m_DataLength - 1) //向end方向渲染
                {
                    itemToRender = GetLoopVerticalItemAt(m_EndIdx + 1);
                    m_StartIdx++;
                    m_EndIdx++;
                    // Debug.Log($" scroll loop vertical end:Render({m_EndIdx}) from {itemToRender.index} start:{m_StartIdx} ");
                    RenderItem(itemToRender, m_EndIdx);
                    if (check++ >= pageSize) break;

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

            var size = content.sizeDelta;
            size.y = Mathf.Abs(m_ContentBounds.height);
            content.sizeDelta = size;
        }

        ///<summary>
        /// 计算bound 高度
        ///</summary>
        protected float CalcItemBound(LoopItem loopItem)
        {
            RectTransform rectTran = loopItem.transform;
            if (rectTran.anchorMin != rectTran.anchorMax) //表示宽度适配
            {
                // var offsetMin = rectTran.offsetMin;
                // var offsetMax = rectTran.offsetMax;
                // offsetMin.x = verticalPadding;
                // offsetMax.x = -verticalPadding;
                // rectTran.offsetMin = offsetMin;
                // rectTran.offsetMax = offsetMax;
            }

            // LayoutRebuilder.ForceRebuildLayoutImmediate(rectTran); //立马计算布局
            var layoutEle = loopItem.item.GetComponent<LayoutElement>();

            var height = 0f;
            if (layoutEle != null)
            {
                height = layoutEle.preferredHeight + verticalPadding; //高度
            }
            else
            {
                height = Mathf.Abs(rectTran.rect.height) + verticalPadding;
            }

            virtualVertical.SetCellSize(loopItem.index, height);
            return height;
        }

        public void OnItemLayoutChange(int id)
        {
            // Debug.Log($" OnItemLayoutChange {id}");
            var item = GetRealItemAt(id);
            if (item != null)
                CalcItemBound(item);

            m_LayoutList.Add(id);
            Layout();

        }

        protected Vector2 GetLoopVerticalItemSize(int idx)
        {
            return virtualVertical.GetCellSize(idx);
        }
        ///<summary>
        /// 布局
        ///</summary>
        public void Layout()
        {
            if (m_LayoutList.Count > 0)
            {
                LoopItem item;
                int index = -1;
                for (int i = 0; i < m_Pages.Count; i++)
                {
                    item = m_Pages[i];
                    index = item.index;
                    if (index >= 0)
                    {
                        var size = virtualVertical.GetCellSize(index);
                        var rectTran = item.transform;
                        Vector2 pos = rectTran.anchoredPosition;
                        pos.y = -size.x + paddingTop; // rect.height * .5f ;
                        pos = pos + m_ContentInitializePosition; //开始位置
                        // rectTran.anchoredPosition3D = Vector3.zero;
                        // rectTran.anchoredPosition = pos;
                        // rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, rectTran.rect.width);
                        rectTran.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -pos.y, rectTran.rect.height);
                        m_CalcBoundyMin = Mathf.Min(m_CalcBoundyMin, size.x - verticalPadding);
                        if (index == 0)
                        {
                            m_CalcBoundyMin = size.x - verticalPadding;
                            m_ContentBounds.yMin = -m_CalcBoundyMin; //ContentBounds坐标系统与transform是反的
                        }
                        m_CalcBoundyMax = Mathf.Max(m_CalcBoundyMax, size.y);
                        if (index == m_DataLength - 1)
                        {
                            m_CalcBoundyMax = size.y;
                            m_ContentBounds.yMax = -m_CalcBoundyMax; //ContentBounds坐标系统与transform是反的
                        }
                    }
                }

                m_LayoutList.Clear();
                CalcBounds();
            }

        }

        void AddLayoutList(int id)
        {
            m_LayoutList.Add(id);
        }

        #endregion

        #region  跳转
        [SerializeField]
        float m_ScrollTime = 0.4f;
        /// <summary>
        /// 滚动时间</br>
        /// </summary>
        public float scrollTime { get { return m_ScrollTime; } set { m_ScrollTime = value; } }

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
                StartCoroutine(ScrollToIndex(value));
            }
        }

        public System.Action<object, int, int> onScrollIndexChanged;

        ///<summary>
        /// 滚动到指定索引
        ///</summary>
        internal IEnumerator ScrollToIndex(int idx)
        {
            yield return waitForend;
            if (isTweening) yield break;

            if (idx < 0 || idx >= dataLength) yield break;

            var oldIdx = m_ScrollToIndex;
            m_ScrollToIndex = idx;
            Vector2 curr = content.anchoredPosition;
            var cpos = GetLoopVerticalItemSize(idx);
            var tpos = cpos;

            tpos.y = tpos.x;
            tpos.x = curr.x;

            //CHECK RANGE
            yield return TweenMoveToPos(curr, tpos, scrollTime);

            if (onScrollIndexChanged != null)
                onScrollIndexChanged(this.parameter, m_ScrollToIndex, oldIdx);

        }

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
                cpos.y = m_ContentBounds.yMin + verticalPadding; //- m_ContentStartPosition.y;
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
        WaitForEndOfFrame waitForend = new WaitForEndOfFrame();

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
                    tweenDir = Vector2.zero;
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

            m_StartIdx = 0;
            m_EndIdx = 0;

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